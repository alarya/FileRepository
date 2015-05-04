using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Xml.Linq;

namespace FileRepository.Controllers
{
    public class FileUploadOldController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage UploadFile()
        {
            if (HttpContext.Current.Request.Files.AllKeys.Any())
            {
                // Get the uploaded image from the Files collection
                var httpPostedFile = HttpContext.Current.Request.Files["UploadedImage"];
                ///NameValueCollection nvc = HttpContext.Current.Request.Form;
                //NameValueCollection nvc = HttpContext.Current.Request.Headers;
                NameValueCollection nvc = HttpContext.Current.Request.Form;
                String uploadPath = nvc["path"];
                if (httpPostedFile != null)
                {

                    // Get the complete file path
                    var fileSavePath = Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data"),uploadPath);
                    fileSavePath = Path.Combine(fileSavePath, httpPostedFile.FileName);

                    // Save the uploaded file to "UploadedFiles" folder
                    httpPostedFile.SaveAs(fileSavePath);

                    return Request.CreateResponse(HttpStatusCode.OK, "");
                }
                else return Request.CreateErrorResponse(HttpStatusCode.NoContent, "");
            }
            else return Request.CreateErrorResponse(HttpStatusCode.NoContent, "No File Found");
        }       
    }
}