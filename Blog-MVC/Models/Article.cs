using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Blog_MVC.Models
{
    public class Article
    {
        public Article()
        {
            
        }

        public Article(string authorId, string title, string body, int categoryId)
        {
            this.AuthorId = authorId;
            this.Title = title;
            this.Body = body;
            this.CategoryId = categoryId;
            this.Date = DateTime.Now;
            this.Comments = new HashSet<Comment>();
        }

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage ="Това поле е задължително")]
        [StringLength(200)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Това поле е задължително")]
        [DataType(DataType.MultilineText)]
        public string Body { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [ForeignKey("Author")]
        public string AuthorId { get; set; }

        public virtual ApplicationUser Author { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        public virtual Category Category { get; set; }

        public bool IsAuthor(string name)
        {
            return this.Author.UserName.Equals(name);
        }

    }
}