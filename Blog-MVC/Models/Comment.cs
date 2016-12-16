using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Blog_MVC.Models
{
    public class Comment
    {
        public Comment()
        {
            this.Date = DateTime.Now;
        }

        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Text { get; set; }

        [Required]
        public DateTime Date { get; set; }
        
        [ForeignKey("Author")]
        public string AuthorId { get; set; }
        
        public virtual ApplicationUser Author { get; set; }

        public int ArticleId { get; set; }
        
        [Required]
        public virtual Article Article { get; set; }
    }
}