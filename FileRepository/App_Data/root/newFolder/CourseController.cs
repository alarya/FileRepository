/////////////////////////////////////////////////////////////
// CourseController.cs - Controller for course selection   //
//                                                         //
// Jim Fawcett, CSE686 - Internet Programming, Spring 2013 //
/////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcState.Models;

namespace MvcState.Controllers
{
    public class CourseController : Controller
    {
        private CourseContext db = new CourseContext();

        //
        // GET: /Course/

        public ActionResult Index()
        {
            return View(db.Courses.ToList());
        }

        // GET: /Catalog/

        public ActionResult Catalog()
        {
          ((List<Course>)Session["ShoppingCart"]).Clear();
          return View(db.Courses.ToList());
        }

        // GET: /AddToCart

        public ActionResult AddToCart(int id = 0)
        {
          Course course = db.Courses.Find(id);
          if (course == null)
          {
            return HttpNotFound();
          }
          ((List<Course>)(HttpContext.Session["ShoppingCart"])).Add(course);
          //return RedirectToAction("Catalog");
          return View("Catalog", db.Courses.ToList());
        }

        public ActionResult CheckOut()
        {
          return View();
        }
        //
        // GET: /Course/Details/5

        public ActionResult Details(int id = 0)
        {
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        //
        // GET: /Course/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Course/Create

        [HttpPost]
        public ActionResult Create(Course course)
        {
            if (ModelState.IsValid)
            {
                db.Courses.Add(course);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(course);
        }

        //
        // GET: /Course/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        //
        // POST: /Course/Edit/5

        [HttpPost]
        public ActionResult Edit(Course course)
        {
            if (ModelState.IsValid)
            {
                db.Entry(course).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(course);
        }

        //
        // GET: /Course/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        //
        // POST: /Course/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Course course = db.Courses.Find(id);
            db.Courses.Remove(course);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}