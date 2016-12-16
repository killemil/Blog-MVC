using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Blog_MVC.Models;

namespace Blog_MVC.Controllers
{
    [ValidateInput(false)]
    public class ArticleController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public bool IsUserAuthorizedToEdit(Article article)
        {
            bool isAdmin = this.User.IsInRole("Admin");
            bool isAuthor = article.IsAuthor(this.User.Identity.Name);

            return isAdmin || isAuthor;
        }

        // GET: Article
        public ActionResult Index(string sortOrder)
        {
            ViewBag.TitleSort = sortOrder == "Title" ?  "title_desc" : "Title";
            ViewBag.BodySort = sortOrder == "Body" ? "body_desc" : "Body";
            ViewBag.DateSort = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.AuthorSort = sortOrder == "Author" ? "author_desc" : "Author";

            var articlesWithAuthor = db.Articles
                .Include(p => p.Author);

            switch (sortOrder)
            {
                case "title_desc":
                    articlesWithAuthor = articlesWithAuthor.OrderByDescending(a => a.Title);
                    break;
                case "Title":
                    articlesWithAuthor = articlesWithAuthor.OrderBy(a => a.Title);
                    break;
                case "body_desc":
                    articlesWithAuthor = articlesWithAuthor.OrderByDescending(a => a.Body);
                    break;
                case "Body":
                    articlesWithAuthor = articlesWithAuthor.OrderBy(a => a.Body);
                    break;
                case "date_desc":
                    articlesWithAuthor = articlesWithAuthor.OrderByDescending(a => a.Date);
                    break;
                case "Date":
                    articlesWithAuthor = articlesWithAuthor.OrderBy(a => a.Date);
                    break;
                case "author_desc":
                    articlesWithAuthor = articlesWithAuthor.OrderByDescending(a => a.Author.FullName);
                    break;
                case "Author":
                    articlesWithAuthor = articlesWithAuthor.OrderBy(a => a.Author.FullName);
                    break;
                default:
                    break;
            }

            return View(articlesWithAuthor.ToList());
        }

        // GET: Article/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var model = new ArticleViewModel();
            Article article = db.Articles.Find(id);
            if (article == null)
            {
                return HttpNotFound();
            }
            return View(article);
        }

        // GET: Articles/Create
        [Authorize]
        public ActionResult Create()
        {
            using (var db = new ApplicationDbContext())
            {
                var model = new ArticleViewModel();
                model.Categories = db.Categories.OrderBy(a => a.Name).ToList();

                return View(model);
            }
        }

        // POST: Articles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create(ArticleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var authorId = db.Users.Where(u => u.UserName == this.User.Identity.Name).First().Id;

                var article = new Article(authorId, model.Title, model.Body, model.CategoryId);

                db.Articles.Add(article);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
        }

        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var article = db.Articles.Where(a => a.Id == id).First();

            if (!IsUserAuthorizedToEdit(article))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            if (article == null)
            {
                return HttpNotFound();
            }

            var model = new ArticleViewModel();
            model.Id = article.Id;
            model.Title = article.Title;
            model.Body = article.Body;
            model.CategoryId = article.CategoryId;
            model.Categories = db.Categories.OrderBy(c => c.Name).ToList();

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ArticleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var article = db.Articles.FirstOrDefault(a => a.Id == model.Id);
                article.Title = model.Title;
                article.Body = model.Body;
                article.CategoryId = model.CategoryId;

                db.Entry(article).State = EntityState.Modified;
                article.Author = db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // GET: Articles/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Article article = db.Articles.Find(id);
            if (article == null)
            {
                return HttpNotFound();
            }
            return View(article);
        }

        // POST: Articles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {
            Article article = db.Articles.Find(id);
          
            db.Articles.Remove(article);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
