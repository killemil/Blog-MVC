
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

using Blog_MVC.Models;

namespace Blog_MVC.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var db = new ApplicationDbContext();
            var articles = db.Articles;

            return View(articles.ToList());
        }

        public ActionResult ListCategories(int page = 1, int pageSize = 5)
        {
            var db = new ApplicationDbContext();
            var categories = db.Categories.OrderBy(c => c.Name);

            var model = new PagedList<Category>(categories, page, pageSize);
            
            return View(model);
            
        }

        public ActionResult ListByCategory(int? id, int page = 1, int pageSize = 5)
        {
            var db = new ApplicationDbContext();
            
            var articles = db.Articles.Where(a => a.CategoryId == id).OrderByDescending(a=> a.Date);
            var categoryName = db.Categories.Find(id);

            ViewBag.Category = categoryName;

            var model = new PagedList<Article>(articles, page, pageSize);

            return View(model);
            
        }
    }
}