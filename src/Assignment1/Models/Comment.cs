using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment1.Models
{
    public class Comment
    {
        [Key]
        public int CommentId
        {
            get;
            set;
        }

        [ForeignKey("BlogPost")]
        public int BlogPostId
        {
            get;
            set;
        }

        [ForeignKey("User")]
        public int UserId
        {
            get;
            set;
        }

        [StringLength(2048)]
        public string Content
        {
            get;
            set;
        }

        public BlogPost BlogPost { get; set; }

        public User User { get; set; }
    }
}
