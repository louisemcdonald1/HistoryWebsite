using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HistoryWebsite.Models;

namespace HistoryWebsite.Controllers
{
    public class BiographiesController : Controller
    {
        private louisemc_tudorsEntities _entities = new louisemc_tudorsEntities();

        // GET: Biographies
        public ActionResult Index()
        {
            return View(_entities.Articles.ToList());
        }

        public ActionResult Article(int articleId)
        {
            /* get the article from the DB */
            Article articleToDisplay = _entities.Articles.Find(articleId);

            return View(articleToDisplay);
        }

        public ActionResult GetImage (int id)
        {
            var firstOrDefault = _entities.Images.Where(i => i.Id == id).FirstOrDefault();
            if (firstOrDefault != null)
            {
                byte[] image = firstOrDefault.Image1;
                return File(image, "image/jpg");
            }
            else
            {
                return null;
            }
            
        }

    }
}