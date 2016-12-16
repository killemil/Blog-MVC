
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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

        public ActionResult ListCategories()
        {
            var db = new ApplicationDbContext();
            var categories = db.Categories;
            
            return View(categories.ToList());
            
        }

        public ActionResult ListByCategory(int? id)
        {
                var db = new ApplicationDbContext();
            
                var articles = db.Articles.Where(a => a.CategoryId == id);
                var categoryName = db.Categories.Find(id);

                ViewBag.Category = categoryName;

                return View(articles.ToList());
            
        }
    }
}