using Blog_MVC.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace Blog_MVC.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        public EntityState EntityStat { get; private set; }

        // GET: Category
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List(int page = 1, int pageSize = 5)
        {
            using (var db = new ApplicationDbContext())
            {
                var categories = db.Categories.OrderBy(c=> c.Name).ToList();

                var model = new PagedList<Category>(categories, page, pageSize);

                return View(model);
            }
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                using (var db = new ApplicationDbContext())
                {
                    db.Categories.Add(category);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            return View(category);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var db = new ApplicationDbContext())
            {
                var category = db.Categories.FirstOrDefault(c => c.Id == id);

                if (category == null)
                {
                    return HttpNotFound();
                }
                return View(category);
            }
        }

        [HttpPost]
        public ActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                using (var db = new ApplicationDbContext())
                {
                    db.Entry(category).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            return View(category);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var db = new ApplicationDbContext())
            {
                var category = db.Categories.FirstOrDefault(c => c.Id == id);

                if (category == null)
                {
                    return HttpNotFound();
                }

                return View(category);
            }
        }

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(int? id)
        {
            using (var db = new ApplicationDbContext())
            {
                var category = db.Categories.FirstOrDefault(c => c.Id == id);

                var categoryArticles = category.Articles.ToList();

                foreach (var article in categoryArticles)
                {
                    db.Articles.Remove(article);
                }

                db.Categories.Remove(category);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

        }
    }
}