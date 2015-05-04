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
using System;
using System.Collections.Generic;

namespace FileRepository.Controllers
{
    [Authorize(Roles = "user")]
    public class UserController : Controller
    {
        private FileRepositoryDb db = new FileRepositoryDb();
        protected ApplicationDbContext ApplicationDbContext { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }
        
        
         public UserController()
        {
            this.ApplicationDbContext = new ApplicationDbContext();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));
        }
        // GET: User
        public ActionResult Index()
        {
            ViewBag.CurrentPath = "root";
            return View(db.Files.Where(i => i.Path == "root"));
        }

        //----------Display files and folders of the selected folder---------------------------//
        public ActionResult DirectoryChangeIn(String path)
        {
            ViewBag.CurrentPath = path;
            return View("Index", db.Files.Where(i => i.Path == path));
        }

        //---------Display files and folders in the directory one level up---------------------//
        public ActionResult DirectoryChangeOut(String path)
        {
            int index = path.LastIndexOf('/');
            if (index != -1)
            {
                String newPath = path.Substring(0, index);
                ViewBag.CurrentPath = newPath;
                return View("Index", db.Files.Where(i => i.Path == newPath));
            }
            else
            {
                ViewBag.CurrentPath = "root";
                return View("Index", db.Files.Where(i => i.Path == "root"));
            }
        }

        //-----------------------View a single selected files ---------------------------------------//
        // Pending: error checking if file does not exist
        public ActionResult ViewFiles(String path)
        {
            String FileData;
            using (StreamReader s = new StreamReader(Path.Combine(Server.MapPath("~/App_data"), path)))
            {
                FileData = s.ReadToEnd();
            }

            ViewBag.FileData = FileData;
            ViewBag.CurrentPath = path;

            return View();
        }

        //----------------View all files in a folder ----------------------------------------------//
        public ActionResult ViewFolder(String path)
        {

            String physicalPath = Path.Combine(Server.MapPath("~/App_data"), path);
            String[] files = System.IO.Directory.GetFiles(physicalPath);

            List<String> allFiles = new List<string>();
            allFiles.AddRange(files);

            String[] dirs = System.IO.Directory.GetDirectories(physicalPath);
            foreach (String dir in dirs)
            {
                allFiles.AddRange(returnFilesInFolder(dir));
            }

            Dictionary<string, string> FilesData = new Dictionary<string, string>();
            foreach (String file in allFiles)
            {
                String FileData;
                using (StreamReader s = new StreamReader(file))
                {
                    FileData = s.ReadToEnd();
                }

                String[] split = file.Split('\\');
                //int index = split;
                int length = split.Count();
                String FileName = split[length - 1];
                FilesData.Add(FileName, FileData);
                //ViewBag.FileData["FileName"] = FileData;
            }

            ViewBag.CurrentPath = path;
            ViewBag.FilesData = FilesData;
            return View();
        }
        //------------Recursively search files in subfolders ----------------------------------//
        public List<String> returnFilesInFolder(String path)
        {
            String[] files = System.IO.Directory.GetFiles(path);

            List<String> allFiles = new List<string>();
            allFiles.AddRange(files);

            String[] dirs = System.IO.Directory.GetDirectories(path);
            foreach (String dir in dirs)
            {
                allFiles.AddRange(returnFilesInFolder(path));
            }

            return allFiles;

        }

        //----------------returns dependencies View---------------------------------------------------//
        public ActionResult Dependencies(int id)
        {
            FileRepository.Models.File file = db.Files.Find(id);

            //var dependencyFiles = new List<string>();
            var dependencyFiles1 = db.Dependencies.Where(i => i.FileId1 == id);
            var dependencyFiles2 = db.Dependencies.Where(i => i.FileId2 == id);

            List<string> dependentFiles = new List<string>();
            foreach (var item in dependencyFiles1)
            {
                dependentFiles.Add(item.FileName2.FileName);
            }
            foreach (var item in dependencyFiles2)
            {
                dependentFiles.Add(item.FileName1.FileName);
            }

            ViewBag.FileName = file.FileName;
            ViewBag.FileId = file.FileId;
            ViewBag.CurrentPath = file.Path + "/";
            ViewBag.dependencies = dependentFiles;
            ViewBag.SelectFile = new SelectList(db.Files.Where(i => i.Type == "file" && i.FileId != id), "FileId", "FileName");
            return View();
        }

        //--------------Download File view-------------------------------------------------------//
        public ActionResult DownloadFile(int? id)
        {
            FileRepository.Models.File file = db.Files.Find(id);

            var dependencyFiles1 = db.Dependencies.Where(i => i.FileId1 == id);
            var dependencyFiles2 = db.Dependencies.Where(i => i.FileId2 == id);

            List<FileRepository.Models.File> dependentfiles = new List<Models.File>();
            foreach (var item in dependencyFiles1)
                dependentfiles.AddRange(db.Files.Where(i => i.FileId == item.FileId2));

            foreach (var item in dependencyFiles2)
                dependentfiles.AddRange(db.Files.Where(i => i.FileId == item.FileId1));

            ViewBag.FileName = file.FileName;
            ViewBag.FileId = file.FileId;
            //ViewBag.FileList = dependentFiles;
            ViewBag.CurrentPath = file.Path + '/';

            return View(dependentfiles);
        }

    }
}