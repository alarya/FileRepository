using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CodeFirst.Models;
using System.IO;

namespace CodeFirst.Controllers
{
    public class FilesController : Controller
    {
        private FileRepositoryDb db = new FileRepositoryDb();

        // GET: Files
        public ActionResult Index()
        {
            return View(db.Files.ToList());
        }

        // GET: Files/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CodeFirst.Models.File file = db.Files.Find(id);
            if (file == null)
            {
                return HttpNotFound();
            }
            return View(file);
        }

        //GET: Files/Index + filedata
        public ActionResult View(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CodeFirst.Models.File file = db.Files.Find(id);
            if (file == null)
            {
                return HttpNotFound();
            }
            
            //Read fileData
            //String path = Path.Combine(Server.MapPath("~/App_data"),file.FileName);
            
            //try
            //{
            //  using (StreamReader sr = new StreamReader(path))
            //    {
            //      String line = sr.ReadToEnd();
            //      ViewBag.FileData = line;
            //    }
            //}
            //catch (Exception e)
            //{
            //    ViewBag.FileData = "";   
            //}

            ViewData["FileName"] = file.FileName.ToString();
            return View();
        }
        // GET: Files/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Files/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FileId,FileName,OwnerName,OwnerEmail")] CodeFirst.Models.File file)
        {
            if (ModelState.IsValid)
            {
                file.Path = ".";         //giving default path as server root directory for now
                file.LastModifiedBy = file.OwnerName;
                db.Files.Add(file);
                db.SaveChanges();

                
                return RedirectToAction("Index");
            }

            return View(file);
        }

        // GET: Files/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CodeFirst.Models.File file = db.Files.Find(id);
            if (file == null)
            {
                return HttpNotFound();
            }
            return View(file);
        }

        // POST: Files/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FileId,FileName,OwnerName,OwnerEmail")] CodeFirst.Models.File file)
        {
            if (ModelState.IsValid)
            {
                db.Entry(file).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(file);
        }

        // GET: Files/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CodeFirst.Models.File file = db.Files.Find(id);
            if (file == null)
            {
                return HttpNotFound();
            }
            return View(file);
        }

        // POST: Files/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CodeFirst.Models.File file = db.Files.Find(id);
            var dependencies1 = db.Dependencies.Where(i => i.FileId1 == id);
            db.Dependencies.RemoveRange(dependencies1);
            var dependencies2 = db.Dependencies.Where(i => i.FileId2 == id);
            db.Dependencies.RemoveRange(dependencies2);
            db.Files.Remove(file);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
