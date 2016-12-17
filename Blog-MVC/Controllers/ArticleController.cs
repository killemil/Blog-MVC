using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Blog_MVC.Models;
using PagedList;

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
        public ActionResult Index(string sortOrder, int page = 1, int pageSize = 5)
        {
            ViewBag.CurrentSort = sortOrder;

            ViewBag.TitleSort = sortOrder == "Title" ?  "title_desc" : "Title";
            ViewBag.BodySort = sortOrder == "Body" ? "body_desc" : "Body";
            ViewBag.DateSort = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.AuthorSort = sortOrder == "Author" ? "author_desc" : "Author";
            ViewBag.ViewSort = sortOrder == "ViewCount" ? "view_desc" : "ViewCount";

            var articlesWithAuthor = db.Articles
                .Include(p => p.Author).Include(a => a.Tags);

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
                case "view_desc":
                    articlesWithAuthor = articlesWithAuthor.OrderByDescending(a => a.ViewCount);
                    break;
                case "ViewCount":
                    articlesWithAuthor = articlesWithAuthor.OrderBy(a => a.ViewCount);
                    break;
                default: articlesWithAuthor = articlesWithAuthor.OrderByDescending(a => a.Date);
                    break;
            }

            var model = new PagedList<Article>(articlesWithAuthor, page, pageSize);


            return View(model);
        }

        // GET: Article/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var model = new ArticleViewModel();
            Article article = db.Articles
                .Where(a=> a.Id == id)
                .Include(a=> a.Author)
                .Include(a=> a.Tags)
                .First();

            if (article == null)
            {
                return HttpNotFound();
            }
            article.ViewCount += 1;
            db.SaveChanges();

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

                this.SetArticleTags(article, model, db);

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

            model.Tags = string.Join(", ", article.Tags.Select(t => t.Name));

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
                this.SetArticleTags(article, model, db);

                db.Entry(article).State = EntityState.Modified;
                article.Author = db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        private void SetArticleTags(Article article, ArticleViewModel model, ApplicationDbContext db)
        {
            var tagStrings = model.Tags
                .Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.ToLower())
                .Distinct();

            article.Tags.Clear();

            foreach (var tagString in tagStrings)
            {
                Tag tag = db.Tags.FirstOrDefault(t => t.Name.Equals(tagString));

                if (tag == null)
                {
                    tag = new Tag() { Name = tagString };
                    db.Tags.Add(tag);
                }
                article.Tags.Add(tag);
            }
        }

        // GET: Articles/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Article article = db.Articles
                .Where(a=> a.Id == id)
                .Include(a=> a.Author)
                .Include(a=> a.Category)
                .First();

            if (!IsUserAuthorizedToEdit(article))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            ViewBag.TagString = string.Join(", ", article.Tags
                .Select(t => t.Name));

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
