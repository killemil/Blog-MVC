using Blog_MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;

namespace Blog_MVC.Controllers
{
    public class TagController : Controller
    {
        // GET: Tag
        public ActionResult Index(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var db = new ApplicationDbContext();

            var articles = db.Tags.Include(t => t.Articles.Select(a => a.Tags))
                .Include(t => t.Articles.Select(a => a.Author))
                .FirstOrDefault(t => t.Id == id)
                .Articles
                .OrderByDescending(a=> a.Date)
                .ToList();

            ViewBag.Tag = db.Tags.Where(t => t.Id == id).First();

            return View(articles);
        }
    }
}