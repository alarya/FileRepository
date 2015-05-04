using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Owin;
using FileRepository.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using Ionic.Zip;
using System.Text;
using System.Xml.Linq;

namespace FileRepository.Controllers
{               
    public class HomeController : Controller
    {
         private FileRepositoryDb db = new FileRepositoryDb();
        protected ApplicationDbContext ApplicationDbContext { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }
        // GET: Developer

        public HomeController()
        {
            this.ApplicationDbContext = new ApplicationDbContext();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));
        }
        public ActionResult Index()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                var role = UserManager.GetRoles(user.Id);
                if(role.Count != 0)              // if role is not already assigned to the newly registered user
                switch (role[0].ToString())
                {
                    case "admin":
                        return RedirectToAction("Index", "Admin");
                    case "developer":
                        return RedirectToAction("Index", "Developer");
                    case "user":
                        return RedirectToAction("Index", "User");
                }
            }
                    return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Feeds()
        {
            XDocument doc = new XDocument();
            string path = Server.MapPath("~/Content/Feeds.xml");
            doc = XDocument.Load(path);

            List<Feeds> feeds = new List<Feeds>();
            foreach(XElement T in doc.Descendants("feed"))
            {
                Feeds feed_ = new Feeds();
                feed_.message = T.Element("message").Value;
                feed_.user = T.Element("user").Value;
                feed_.date = T.Element("date").Value;

                feeds.Add(feed_);
            }

            ViewBag.feedList = feeds;
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult Search()
        {
            string searchString = Request.Form["searchString"];

            if (string.IsNullOrEmpty(searchString))
                return RedirectToAction("Index");

            var files = db.Files.Where(i => i.FileId >= 1);
            List<Models.File> searchResult = new List<Models.File>();
            
            foreach(var item in files)
            {
                string input = item.FileName.ToString();
                if (input.Contains(searchString))
                {
                    Models.File resultMatch = new Models.File();
                    resultMatch.FileName = item.FileName;
                    resultMatch.Path = item.Path;
                    searchResult.Add(resultMatch);                
                }
            }

            ViewBag.searchResult = searchResult;
            return View();
        }
    }
    public class Feeds
    {
        public string message { get; set; }
        public string user { get; set; }
        public string date { get; set; }
    }
}