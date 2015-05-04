using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FileRepository.Models;
using System.Web;
using System.Net.Http.Formatting;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FileRepository.Controllers
{   
    public class FileUploadController : ApiController
    {
        private FileRepositoryDb db = new FileRepositoryDb();
        protected ApplicationDbContext ApplicationDbContext { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }
        // GET: Developer

        public FileUploadController()
        {
            this.ApplicationDbContext = new ApplicationDbContext();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));
        }

        public HttpResponseMessage Get()
        {
            string path = HttpContext.Current.Server.MapPath("~/App_Data/root");

            var files = db.Files.Where(i => i.Type == "folder");
            List<string> filesResult = new List<string>();
            foreach (FileRepository.Models.File file in files)
            {
                filesResult.Add(file.Path + '/' + file.FileName);
            }

            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new ObjectContent<IEnumerable<string>>(filesResult, new JsonMediaTypeFormatter());
            return response;
        }

        public Task<IEnumerable<FileDesc>> Post(string selectedFolder, string selectedFile)
        {
            string path = HttpContext.Current.Server.MapPath("~/App_Data");

            string[] pathParts = selectedFolder.Split('/');
            foreach (string part in pathParts)
                path = path + "\\" + part  ; 

            var rootUrl = Request.RequestUri.AbsoluteUri.Replace(Request.RequestUri.AbsolutePath, String.Empty);

            if (Request.Content.IsMimeMultipartContent())
            {

                var streamProvider = new CustomMultipartFormDataStreamProvider(path);
                var task = Request.Content.ReadAsMultipartAsync(streamProvider).ContinueWith<IEnumerable<FileDesc>>(t =>
                {
                   if (t.IsFaulted || t.IsCanceled)
                    {
                        throw new HttpResponseException(HttpStatusCode.InternalServerError);
                    }

                   Models.File newFile= new Models.File();
                   newFile.FileName = selectedFile;
                   newFile.Type = "file";
                   //newFile.OwnerName = UserManager.FindById(User.Identity.GetUserId()).Email;
                   newFile.Path = selectedFolder;
                   db.Files.Add(newFile);
                   db.SaveChanges();
                   var fileInfo = streamProvider.FileData.Select(i =>
                   {
                       var info = new FileInfo(i.LocalFileName);
                       return new FileDesc(info.Name, rootUrl + "/" + selectedFolder + "/" + info.Name, info.Length / 1024);
                   });
                   return fileInfo;
                });
                return task;
            }
            else
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotAcceptable, "This request is not properly formatted"));
            }
        }
    }

    public class FileDesc
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public long Size { get; set; }
        public FileDesc(string n, string p, long s)
        {
            Name = n;
            Path = p;
            Size = s;
        }
    }
}
