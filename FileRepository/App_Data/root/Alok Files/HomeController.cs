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
    }
}