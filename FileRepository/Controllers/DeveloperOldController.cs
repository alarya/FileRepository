using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FileRepository.Controllers
{
    public class DeveloperoldController : Controller
    {
        // GET: Developer
        [Authorize(Roles="developer")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles="developer")]
        public ActionResult GetFilesAndFolders()
        {
            
            String path = Server.MapPath("~/App_data/root");
            string[] files = System.IO.Directory.GetFiles(path);
            string[] dirs = System.IO.Directory.GetDirectories(path);

            IList<FilesandFolders> result = new List<FilesandFolders>();
            FilesandFolders f ;
            foreach (string dir in dirs)                  //Add all folders
            {
                f = new FilesandFolders();
                f.type = "folder";
                int length = dir.Split('\\').Length;
                f.name = dir.Split('\\')[length-1];
                result.Add(f);
            }

            foreach(string file in files)                  //Add all files
            {
                f = new FilesandFolders();
                f.type = "file";
                int length = file.Split('\\').Length;
                f.name = file.Split('\\')[length-1];
                result.Add(f);
            }            
            
            return Json(result);
        }
    }


    //--------defines Structure of the Json result to send back for File Explorer --------------//
    public class FilesandFolders
    {
        public String type { get; set; }
        public String name { get; set; }
    }
}