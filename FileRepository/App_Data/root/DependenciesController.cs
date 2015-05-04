using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CodeFirst.Models;

namespace CodeFirst.Controllers
{
    public class DependenciesController : Controller
    {
        private FileRepositoryDb db = new FileRepositoryDb();

        // GET: Dependencies
        public ActionResult Index()
        {
            var dependencies = db.Dependencies.Include(d => d.FileName1).Include(d => d.FileName2);
            return View(dependencies.ToList());
        }

        // GET: Dependencies/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dependency dependency = db.Dependencies.Find(id);
            if (dependency == null)
            {
                return HttpNotFound();
            }
            return View(dependency);
        }

        // GET: Dependencies/Create
        public ActionResult Create()
        {
            ViewBag.FileId1 = new SelectList(db.Files, "FileId", "FileName");
            ViewBag.FileId2 = new SelectList(db.Files, "FileId", "FileName");
            return View();
        }

        // POST: Dependencies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DependencyId,FileId1,FileId2")] Dependency dependency)
        {
            if (ModelState.IsValid)
            {
                db.Dependencies.Add(dependency);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.FileId1 = new SelectList(db.Files, "FileId", "FileName", dependency.FileId1);
            ViewBag.FileId2 = new SelectList(db.Files, "FileId", "FileName", dependency.FileId2);
            return View(dependency);
        }

        // GET: Dependencies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dependency dependency = db.Dependencies.Find(id);
            if (dependency == null)
            {
                return HttpNotFound();
            }
            ViewBag.FileId1 = new SelectList(db.Files, "FileId", "FileName", dependency.FileId1);
            ViewBag.FileId2 = new SelectList(db.Files, "FileId", "FileName", dependency.FileId2);
            return View(dependency);
        }

        // POST: Dependencies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DependencyId,FileId1,FileId2")] Dependency dependency)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dependency).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FileId1 = new SelectList(db.Files, "FileId", "FileName", dependency.FileId1);
            ViewBag.FileId2 = new SelectList(db.Files, "FileId", "FileName", dependency.FileId2);
            return View(dependency);
        }

        // GET: Dependencies/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dependency dependency = db.Dependencies.Find(id);
            if (dependency == null)
            {
                return HttpNotFound();
            }
            return View(dependency);
        }

        // POST: Dependencies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Dependency dependency = db.Dependencies.Find(id);
            db.Dependencies.Remove(dependency);
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
