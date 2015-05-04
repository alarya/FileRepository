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

        public HttpResponseMessage Get(string path)
        {
            string filePath = Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data"), path);
            CustomMultipartFormDataStreamProvider provider = new CustomMultipartFormDataStreamProvider(filePath);
            if (!System.IO.File.Exists(filePath))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

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
    }

}