using HistoryWebsite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HistoryWebsite.Controllers
{
    public class HomeController : Controller
    {
        private louisemc_tudorsEntities _entities = new louisemc_tudorsEntities();

        // GET: Home
        public ActionResult Index()
        {
            return View(_entities.Articles.ToList());
        }
    }
}