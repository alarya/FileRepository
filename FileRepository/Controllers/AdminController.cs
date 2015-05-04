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
using System.Text.RegularExpressions;

namespace FileRepository.Controllers
{
[Authorize(Roles="admin")]
    public class AdminController : Controller
    {
        private FileRepositoryDb db = new FileRepositoryDb();
        protected ApplicationDbContext ApplicationDbContext { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }
        // GET: Developer

        public AdminController()
        {
            this.ApplicationDbContext = new ApplicationDbContext();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));
        }

        public ActionResult Index()
        {
            ViewBag.CurrentPath = "root";

            //Test to see currently logged in User
            //var user = UserManager.FindById(User.Identity.GetUserId());
            //String UserMailId = user.Email;

            //FileNavigatorModel fn = new FileNavigatorModel();
            //fn.FileForModel = db.Files.Where(i => i.Path == "root").ToList();

            //return View(fn);
            return View(db.Files.Where(i => i.Path == "root"));

            
        }

        //----------Display files and folders of the selected folder---------------------------//
        public ActionResult DirectoryChangeIn(String path)
        {
            ViewBag.CurrentPath = path;
            return View("Index",db.Files.Where(i => i.Path == path));
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
            using(StreamReader s = new StreamReader(Path.Combine(Server.MapPath("~/App_data"),path)))
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
            foreach(String dir in dirs)
            {
                allFiles.AddRange(returnFilesInFolder(dir));
            }

            Dictionary<string,string> FilesData = new Dictionary<string,string>();
            foreach(String file in allFiles)
            {
                String FileData;
                using (StreamReader s = new StreamReader(file))
                {
                    FileData = s.ReadToEnd();
                }

                String[] split = file.Split('\\');
                //int index = split;
                int length = split.Count();
                String FileName = split[length-1];
                FilesData.Add(FileName,FileData);
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

        //---------------------To upload a file to current path in navigator ---------------------------------//
        [HttpPost]
        public ActionResult UploadFile(String CurrentPath)
        {
            //Pending
            // Check for overwriting condition 

            
            //save file to server directory
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];

                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/App_Data"),CurrentPath, fileName);
                    file.SaveAs(path);

                    //add file to database
                    FileRepository.Models.File newFile = new Models.File();
                    newFile.FileName = fileName;
                    var user = UserManager.FindById(User.Identity.GetUserId());
                    String UserMailId = user.Email;
                    newFile.OwnerName = UserMailId;
                    newFile.Type = "file";
                    newFile.Path = CurrentPath;
                    db.Files.Add(newFile);
                    db.SaveChanges();
                    UpdateLogs(newFile.FileName + " is uploaded");
                }
            }
            ViewBag.CurrentPath = CurrentPath;
            return View("Index", db.Files.Where(i => i.Path == CurrentPath));
        }

        //-------------------------Delete a file by Id ------------------------------------------------------------//
        public ActionResult DeleteFile(int id)
        {                                 
            FileRepository.Models.File deleteFile = db.Files.Find(id);

            //First delete file from folder
            var path = Path.Combine(Server.MapPath("~/App_Data"), deleteFile.Path, deleteFile.FileName);

            try
            {
                System.IO.File.Delete(path);

            }
            catch
            {
                ViewBag.CurrentPath = deleteFile.Path;
                return View("Index", db.Files.Where(i => i.Path == deleteFile.Path));
            }

            //if successfully deleted, delete from database as well
            var dependencies1 = db.Dependencies.Where(i => i.FileId1 == id);
            db.Dependencies.RemoveRange(dependencies1);
            var dependencies2 = db.Dependencies.Where(i => i.FileId2 == id);
            db.Dependencies.RemoveRange(dependencies2);
            db.Files.Remove(deleteFile);
            db.SaveChanges();

            ViewBag.CurrentPath = deleteFile.Path;
            return View("Index", db.Files.Where(i => i.Path == deleteFile.Path));
        }

        //---------------------Create a new folder in current path------------------------------------//
        [HttpPost]
        public ActionResult CreateFolder(String CurrentPath)
        {
   
            String folder = Request.Form["folder"] ;
            
            //Create the folder first
            Regex r = new Regex(@"^[a-zA-Z0-9\s]*$");
            if (r.IsMatch(folder))//test for illegal characters in folder name
            {
                if (!String.IsNullOrEmpty(folder))
                {
                    var path = Path.Combine(Server.MapPath("~/App_Data"), CurrentPath, folder);

                    try
                    {
                        Directory.CreateDirectory(path);
                    }
                    catch
                    {
                        ViewBag.CurrentPath = CurrentPath;
                        return View("Index", db.Files.Where(i => i.Path == CurrentPath));
                    }

                    //if folder successfully created add to database
                    FileRepository.Models.File newFolder = new Models.File();
                    newFolder.FileName = folder;
                    var user = UserManager.FindById(User.Identity.GetUserId());
                    String UserMailId = user.Email;
                    newFolder.OwnerName = UserMailId;
                    newFolder.Type = "folder";
                    newFolder.Path = CurrentPath;
                    db.Files.Add(newFolder);
                    db.SaveChanges();
                    UpdateLogs("A new folder " + folder + " is uploaded");
                    ViewBag.alertMessage = "Folder created";
                }
            }
            else
            {
                ViewBag.alertMessage = "Invalid characters in folder name";
            }
            ViewBag.CurrentPath = CurrentPath;
            return View("Index", db.Files.Where(i => i.Path == CurrentPath));
        }

        //----------------returns dependencies View---------------------------------------------------//
        public ActionResult Dependencies(int id)
        {
            FileRepository.Models.File file = db.Files.Find(id);

            //var dependencyFiles = new List<string>();
            var dependencyFiles1 = db.Dependencies.Where(i => i.FileId1 == id);
            var dependencyFiles2 = db.Dependencies.Where(i => i.FileId2 == id);

            List<string> dependentFiles = new List<string>();
            foreach(var item in dependencyFiles1)
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
            try
            {
                ViewBag.alertMessage = TempData["alertMessage"].ToString(); // will not be set first time when view is loaded
            }
            catch
            {

            }
            ViewBag.SelectFile = new SelectList(db.Files.Where(i => i.Type == "file" && i.FileId != id),"FileId","FileName");
            return View();
        }

        //-----------------post request to Add Dependency -----------------------------------------------------//
        [HttpPost]
        public ActionResult AddDependency(string CurrentPath)
        {

            String dependentFileId = Request.Form["SelectFile"] ;
            //var dependentFile = db.Files.Find(dependentFileId);

            FileRepository.Models.Dependency dep = new FileRepository.Models.Dependency();
            dep.FileId1 = Convert.ToInt32(Request.Form["FileId"]);
            dep.FileId2 = Convert.ToInt32(dependentFileId);
            //check if dependency already exists
            var dep1 = db.Dependencies.Where(i => i.FileId1 == dep.FileId1 && i.FileId2 == dep.FileId2);
            var dep2 = db.Dependencies.Where(i => i.FileId1 == dep.FileId2 && i.FileId2 == dep.FileId1);
            if (dep1.Count() == 0 && dep2.Count() == 0)
            {
                db.Dependencies.Add(dep);
                db.SaveChanges();
                TempData["alertMessage"] = "Dependency has been added";

            }
            else
                TempData["alertMessage"] = "Duplicate: Dependency not added";


            return RedirectToAction("Dependencies", new {id = dep.FileId1});
        }

        //--------------Download File view-------------------------------------------------------//
        public ActionResult DownloadFile(int? id)
        {
            FileRepository.Models.File file = db.Files.Find(id);

            var dependencyFiles1 = db.Dependencies.Where(i => i.FileId1 == id);
            var dependencyFiles2 = db.Dependencies.Where(i => i.FileId2 == id);

            //Dictionary<int,string> dependentFiles = new Dictionary<int,string>();
            //foreach (var item in dependencyFiles1)
            //{
            //    //if(!dependentFiles.ContainsKey(item.FileName1.FileId))
            //       dependentFiles.Add(item.FileName2.FileId ,item.FileName2.FileName);
            //}
            //foreach (var item in dependencyFiles2)
            //{
            //    //if(!dependentFiles.ContainsKey(item.FileName1.FileId))
            //       dependentFiles.Add(item.FileName1.FileId,item.FileName1.FileName);
            //}

            List<FileRepository.Models.File> dependentfiles = new List<Models.File>();
            foreach(var item in dependencyFiles1)
                dependentfiles.AddRange(db.Files.Where(i => i.FileId == item.FileId2));

            foreach (var item in dependencyFiles2)
                dependentfiles.AddRange(db.Files.Where(i => i.FileId == item.FileId1));

            ViewBag.FileName = file.FileName;
            ViewBag.FileId = file.FileId;
            //ViewBag.FileList = dependentFiles;
            ViewBag.CurrentPath = file.Path + '/';

            return View(dependentfiles);
        }

        //--------This method returns the files to be downloaded----------------------------------//
        [HttpPost]
        public FileResult Download(int? id, Dictionary<int,string> files)
        {
            string[] Ids = Request.Form.GetValues("Files");
            List<string> tempId = new List<string>();
            if (Ids.Length != 0)
            foreach(string Id in Ids)
            {
                if(Id != "false")
                tempId.Add(Id);
            }

            if (tempId.Count == 0)
            {
                string path = db.Files.Find(id).Path;
                string physicalPath = Path.Combine(Server.MapPath("~/App_data"), path, db.Files.Find(id).FileName);

                return File(physicalPath, System.Net.Mime.MediaTypeNames.Application.Octet);
            }
            else
            {
                var outputStream = new MemoryStream();

                using (var zip = new ZipFile())
                {
                    string path = db.Files.Find(id).Path;
                    string physicalPath = Path.Combine(Server.MapPath("~/App_data"), path, db.Files.Find(id).FileName);
                    zip.AddFile(physicalPath,"");
                    foreach(var item in tempId)
                    {
                        path = db.Files.Find(Convert.ToInt32(item)).Path;
                        physicalPath = Path.Combine(Server.MapPath("~/App_data"), path, db.Files.Find(Convert.ToInt32(item)).FileName);
                        zip.AddFile(physicalPath,"DependentFiles");
                    }
                    zip.Save(outputStream);
                }

                outputStream.Position = 0 ;
                return File(outputStream,"application/zip",db.Files.Find(id).FileName + ".zip");
            }
        }
    
        // Developer/RenamFolder: GET
        //-------------------returns Rename Folder View---------------------------------------------------//
        public ActionResult RenameFolder(int? id)
        {
            FileRepository.Models.File file = new FileRepository.Models.File();
            file = db.Files.Find(id);
            
            ViewBag.FileName = file.FileName;
            ViewBag.FileId = file.FileId;

            ViewBag.CurrentPath = file.Path + "/";
            return View();
        }
        //---------------------------Post request to change folder name----------------------------------// 
        [HttpPost]
        public ActionResult Rename(int? id)
        {
            string newName = Request.Form["newFileName"];
            
            FileRepository.Models.File file = new FileRepository.Models.File();
            file = db.Files.Find(id);
            string oldName = file.FileName;

            String source = Path.Combine(Server.MapPath("~/App_Data"), file.Path, file.FileName);
            String target = Path.Combine(Server.MapPath("~/App_Data"), file.Path, newName);
            String searchPathString = file.Path + "/" + file.FileName;

            Directory.Move(source, target);
            db.Files.Find(id).FileName = newName;
            db.SaveChanges();

            //Now have to change the path of all the files and folders recursively inside the renamed folder in the database
            //String searchPathString = file.Path + "/" + file.FileName;
            //var files = db.Files.Where(i => i.Path.StartsWith(searchPathString));
            var files = db.Files;

            String filePath;
            String newPath;
            foreach (var file_ in files)
            {
                if (file_.Path.Length >= searchPathString.Length)
                if (file_.Path.Substring(0,searchPathString.Length) == searchPathString)  //update path of only those files/folders which are affected
                {
                    filePath = file_.Path;
                    newPath = filePath.Replace(oldName,newName);                    
                    db.Files.Find(file_.FileId).Path = newPath;                    
                }
            }
            db.SaveChanges();
            return RedirectToAction("RenameFolder", new {id});
        }

        //--------------------------Update the feeds file------------------------------------------------//
        public void UpdateLogs(string message)
        {
            Feeds newFeed = new Feeds();

            newFeed.message = message;
            var loggedInUser = UserManager.FindById(User.Identity.GetUserId());
            String UserMailId = loggedInUser.Email;
            newFeed.user = UserMailId;
            newFeed.date = DateTime.Now.ToString();
            XDocument doc = new XDocument();
            string path = Server.MapPath("~/Content/Feeds.xml");
            doc = XDocument.Load(path);
            XElement root = doc.Root;
            XElement item_ = new XElement("feed");
            XElement T1 = new XElement("message", message);
            XElement T2 = new XElement("user", newFeed.user);
            XElement T3 = new XElement("date", newFeed.date);
            item_.Add(T1);
            item_.Add(T2);
            item_.Add(T3);

            root.AddFirst(item_);

            doc.Save(path);
        }
    }
}
