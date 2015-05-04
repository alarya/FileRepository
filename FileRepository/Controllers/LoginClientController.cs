using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
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
using System.Net.Http.Formatting;

namespace FileRepository.Controllers
{
     
    public class LoginClientController : ApiController
    {
        private FileRepositoryDb db = new FileRepositoryDb();
        protected ApplicationDbContext ApplicationDbContext { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }
        // GET: Developer

        public LoginClientController()
        {
            this.ApplicationDbContext = new ApplicationDbContext();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));
        }

        // GET api/<controller>
        public String Get(string username, string password)
        {
            
            //String Email = HttpContext.Current.Request.Headers["username"];
            ///String Password = HttpContext.Current.Request.Headers["password"];

            //String Emai2 = HttpContext.Current.

            try
            {
                var user = UserManager.Find(username, password);
                if (user != null)
                {
                    //var response = new HttpResponseMessage(HttpStatusCode.OK);
                    //response.Content = new ObjectContent("successful",);
                    return "successful";
                }
            }
            catch
            {
                return "unsuccessful";
            }
            
            return "unsuccessful";
        }
        
    }
}