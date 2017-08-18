using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Assignment1.Models;

namespace Assignment1.Controllers
{
    public class HomeController : Controller
    {
        private Assignment1DataContext dataContext;

        public HomeController(Assignment1DataContext context)
        {
            dataContext = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(dataContext.BlogPosts.Include(p => p.User).ToList());
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult EditBlogPost(int id)
        {
            var blogPostToUpdate = (from m in dataContext.BlogPosts where m.BlogPostId == id select m).FirstOrDefault();
            blogPostToUpdate.Content = blogPostToUpdate.Content.Replace("<br />", "\n");
            return View(blogPostToUpdate);
        }

        [HttpPost]
        public IActionResult ModifyBlogPost(BlogPost BlogPost)
        {
            var postid = Convert.ToInt32(Request.Form["BlogPostId"]);

            var blogPostToUpdate = (from m in dataContext.BlogPosts where m.BlogPostId == postid select m).FirstOrDefault();
            blogPostToUpdate.Title = BlogPost.Title;
            blogPostToUpdate.Content = BlogPost.Content;
            blogPostToUpdate.Content = blogPostToUpdate.Content.Replace(System.Environment.NewLine, "<br />");
            
            dataContext.SaveChanges();
            return RedirectToAction("DisplayFullBlogPost", new { id = postid });

        }

        public IActionResult DeleteBlogPost(int id)
        {
            var blogPostToDelete = (from m in dataContext.BlogPosts where m.BlogPostId == id select m).FirstOrDefault();
            var comments = (from m in dataContext.Comments where m.BlogPostId == id select m);
            if (comments != null)
            {
                blogPostToDelete.Comments = (from m in dataContext.Comments where m.BlogPostId == id select m).Include(p => p.User).ToList();
            }

            foreach(var comment in blogPostToDelete.Comments)
            {
                dataContext.Remove(comment);
            }
            dataContext.Remove(blogPostToDelete);
            dataContext.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult AddBlogPost()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AddComment(int id)
        {
            return View(id);
        }


        [HttpGet]
        public IActionResult DisplayFullBlogPost(int id)
        {
            var postToDisplay = (from m in dataContext.BlogPosts where m.BlogPostId == id select m).Include(p=>p.User).FirstOrDefault();
            var comments = (from m in dataContext.Comments where m.BlogPostId == id select m);
            if(comments != null)
            {
                postToDisplay.Comments = (from m in dataContext.Comments where m.BlogPostId == id select m).Include(p=>p.User).ToList();
            }
            return View(postToDisplay);
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        public IActionResult CreateBlogPost(BlogPost blogpost)
        {
            blogpost.UserId = Int32.Parse(HttpContext.Session.GetString("userId"));
            blogpost.Posted = DateTime.Now;
            blogpost.Content = blogpost.Content.Replace(System.Environment.NewLine, "<br />");
            blogpost.User = (from m in dataContext.Users where m.UserId == Int32.Parse(HttpContext.Session.GetString("userId")) select m).FirstOrDefault();
            dataContext.BlogPosts.Add(blogpost);
            dataContext.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult CreateComment(Comment comment)
        {
            comment.UserId = Int32.Parse(HttpContext.Session.GetString("userId"));
            var blogpostid = comment.BlogPostId = Int32.Parse(Request.Form["BlogPostId"]);
            dataContext.Comments.Add(comment);
            dataContext.SaveChanges();
            
            return RedirectToAction("DisplayFullBlogPost", new { id = blogpostid });
        }

        [HttpPost]
        public IActionResult RegisterAction(User user)
        {
            dataContext.Users.Add(user);
            dataContext.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult LoginAction(User user)
        {
            if (!dataContext.Users.Any(x => x.EmailAddress == user.EmailAddress))
            {
                return RedirectToAction("Index");
            }

            var currentUser = dataContext.Users.Where(x => x.EmailAddress == user.EmailAddress).FirstOrDefault();

            string password = dataContext.Users.Where(x => x.EmailAddress == user.EmailAddress).Select(x => x.Password).Single();

            if(!(password.Equals(user.Password)))
            {
                return RedirectToAction("Index");
            }

            HttpContext.Session.SetString("emailAddress", currentUser.EmailAddress);
            HttpContext.Session.SetString("firstName", currentUser.FirstName);
            HttpContext.Session.SetString("lastName", currentUser.LastName);
            HttpContext.Session.SetString("roleId", currentUser.RoleId.ToString());
            HttpContext.Session.SetString("userId", currentUser.UserId.ToString());

            return RedirectToAction("Index");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
