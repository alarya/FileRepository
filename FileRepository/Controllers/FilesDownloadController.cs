using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;
using FileRepository.Models;
using Ionic.Zip;
using System.Web.Mvc;
namespace FileRepository.Controllers
{

    public class CustomMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
    {
        public CustomMultipartFormDataStreamProvider(string path)
            : base(path)
        { }

        public override string GetLocalFileName(System.Net.Http.Headers.HttpContentHeaders headers)
        {
            var name = !string.IsNullOrWhiteSpace(headers.ContentDisposition.FileName) ? headers.ContentDisposition.FileName : "NoName";
            return name.Replace("\"", string.Empty);
            //this is here because Chrome submits files in quotation marks which get treated as part of the filename and get escaped
        }
    }
    public class FilesDownloadController : ApiController
    {
        private FileRepositoryDb db = new FileRepositoryDb();
        public HttpResponseMessage Get()
        {
            string path = HttpContext.Current.Server.MapPath("~/App_Data/root");

            var files = db.Files.Where(i => i.Type == "file");
            List<string> filesResult = new List<string>();
            foreach(FileRepository.Models.File file in files)
            {
                filesResult.Add(file.Path + '/' + file.FileName);
            }
            
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new ObjectContent<IEnumerable<string>>(filesResult, new JsonMediaTypeFormatter());
            return response;
        }

        public HttpResponseMessage Get(string path, bool dependencies)
        {
            string filePath = Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data"), path);
            CustomMultipartFormDataStreamProvider provider = new CustomMultipartFormDataStreamProvider(filePath);
            if (!System.IO.File.Exists(filePath))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            if (dependencies == false)                        //download without dependencies
            {
                try
                {
                    MemoryStream responseStream = new MemoryStream();
                    Stream fileStream = System.IO.File.Open(filePath, FileMode.Open);
                    bool fullContent = true;
                    fileStream.CopyTo(responseStream);
                    fileStream.Close();
                    responseStream.Position = 0;

                    HttpResponseMessage response = new HttpResponseMessage();
                    response.StatusCode = fullContent ? HttpStatusCode.OK : HttpStatusCode.PartialContent;
                    response.Content = new StreamContent(responseStream);
                    return response;
                }
                catch (IOException)
                {
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }
            else                                                 // download with dependencies
            {
                //get all dependencies
                int index = path.LastIndexOf('/');
                string path_ = path.Substring(0, index);
                string name = path.Substring(index + 1);

                var files = db.Files.Where(i => i.FileName == name && i.Path == path_);
                int id = 0 ;
                foreach(var file in files)
                     id = file.FileId; 


                var dependencyFiles1 = db.Dependencies.Where(i => i.FileId1 == id);
                var dependencyFiles2 = db.Dependencies.Where(i => i.FileId2 == id);

                List<FileRepository.Models.File> dependentfiles = new List<Models.File>();
                  foreach(var item in dependencyFiles1)
                        dependentfiles.AddRange(db.Files.Where(i => i.FileId == item.FileId2));
    
                  foreach (var item in dependencyFiles2)
                        dependentfiles.AddRange(db.Files.Where(i => i.FileId == item.FileId1));

                 
                  List<int> tempId = new List<int>();
                  foreach (FileRepository.Models.File depfile in dependentfiles)
                          tempId.Add(depfile.FileId);
                  var outputStream = new MemoryStream();        //save zip
                  using (var zip = new ZipFile())
                  {
                      string filepath = db.Files.Find(id).Path;
                      string physicalPath = Path.Combine(HttpContext.Current.Server.MapPath("~/App_data"), filepath, db.Files.Find(id).FileName);
                      zip.AddFile(physicalPath, "");
                      foreach (var item in tempId)
                      {
                          path = db.Files.Find(Convert.ToInt32(item)).Path;
                          physicalPath = Path.Combine(HttpContext.Current.Server.MapPath("~/App_data"), path, db.Files.Find(Convert.ToInt32(item)).FileName);
                          zip.AddFile(physicalPath, "DependentFiles");
                      }
                      zip.Save(outputStream);
                  }

                  try
                  {
                      bool fullContent = true;

                      outputStream.Position = 0;
                      FileStreamResult result = new FileStreamResult(outputStream, "application/zip");
                      HttpResponseMessage response = new HttpResponseMessage();
                      response.StatusCode = fullContent ? HttpStatusCode.OK : HttpStatusCode.PartialContent;
                      response.Content = new StreamContent(outputStream);
                      return response;
                  }
                  catch (IOException)
                  {
                      throw new HttpResponseException(HttpStatusCode.InternalServerError);
                  }
            }   
        }
    }

}