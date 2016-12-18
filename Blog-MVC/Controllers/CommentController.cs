using Blog_MVC.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Blog_MVC.Controllers
{
    public class CommentController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Comment
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Create (int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var article = db.Articles.FirstOrDefault(a => a.Id == id);

            if (article == null)
            {
                return HttpNotFound();
            }
          
            ViewBag.Article = article;

            return View();
        }

        [HttpPost]
        public ActionResult Create(Comment comment, int? id)
        {
            
            var article = db.Articles.FirstOrDefault(a => a.Id == id);

            if (article == null)
            {
                return HttpNotFound();
            }

            if (User.Identity.IsAuthenticated)
            {
                comment.Author = db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
            }

            if (comment.Text.Count() > 255)
            {
                comment.Text = comment.Text.Substring(0, 255);
            }
            article.Comments.Add(comment);
            db.SaveChanges();
            
            return RedirectToAction("Details", new RouteValueDictionary(
                                                     new { controller = "Article", action = "Details", Id = id }));
            
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var comment = db.Comments.FirstOrDefault(c => c.Id == id);

            if (comment == null)
            {
                return HttpNotFound();
            }

            return View(comment);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int? id)
        {
            var comment = db.Comments.FirstOrDefault(c => c.Id == id);
            var articleId = comment.ArticleId;

            db.Comments.Remove(comment);
            db.SaveChanges();

            return RedirectToAction ("Details", new RouteValueDictionary(
                                                     new { controller = "Article", action = "Details", Id = articleId }));
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var comment = db.Comments.FirstOrDefault(c => c.Id == id);

            if (comment == null)
            {
                return HttpNotFound();
            }

            return View(comment);
        }

        [HttpPost, ActionName("Edit")]
        public ActionResult EditConfirmed(Comment comment)
        {
            if (ModelState.IsValid)
            {
                var articleId = comment.ArticleId;

                db.Entry(comment).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Details", new RouteValueDictionary(
                       new { controller = "Article", action = "Details", Id = articleId }));
            }
            return View(comment);
        }
    }
}