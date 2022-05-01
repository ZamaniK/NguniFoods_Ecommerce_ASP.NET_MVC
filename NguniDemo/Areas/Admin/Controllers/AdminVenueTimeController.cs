using NguniDemo.Models;
using NguniDemo.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace NguniDemo.Areas.Admin.Controllers
{
    public class AdminVenueTimeController : Controller
    {
        // GET: Admin/AdminVenueTime
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: TimeActivity
        public ActionResult Index1(string searchTerm, int? page)
        {
            TimeVenueVM model = new TimeVenueVM();

            int recordSize = 5;
            page = page ?? 1;

            model.SearchTerm = searchTerm;

            model.TimeVenue = SearchLesson(searchTerm, page.Value, recordSize);

            var totalRecords = SearchLessonCount(searchTerm);
            model.Pager = new Pager(totalRecords, page, recordSize);

            return View(model);
        }
        public int SearchLessonCount(string searchTerm)
        {
            var context = new ApplicationDbContext();
            var subjects = context.VenueTimes.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                subjects = subjects.Where(a => a.SlotTime.ToLongTimeString().Contains(searchTerm.ToLower()));
            }


            return subjects.Count();
        }


        public IEnumerable<VenueTimes> GetAllVenueTimes()
        {
            var context = new ApplicationDbContext();

            return context.VenueTimes.ToList();
        }



        public IEnumerable<VenueTimes> SearchLesson(string searchTerm, int? assignID, int recordSize)
        {
            var context = new ApplicationDbContext();
            var assignment = context.VenueTimes.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                assignment = assignment.Where(a => a.SlotTime.ToLongTimeString().Contains(searchTerm.ToLower()));
            }

            if (assignID.HasValue && assignID.Value > 0)
            {
                assignment = assignment.Where(a => a.VenueTimesId == assignID.Value);
            }



            return assignment.OrderBy(t => t.VenueTimesId == assignID).ToList();
        }
        public ActionResult Index()
        {
            return View(db.VenueTimes.ToList());
        }

        // GET: TimeActivity/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VenueTimes activityTimes = db.VenueTimes.Find(id);
            if (activityTimes == null)
            {
                return HttpNotFound();
            }
            return View(activityTimes);
        }

        // GET: TimeActivity/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TimeActivity/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "VenueTimesId,SlotTime")] VenueTimes activityTimes)
        {
            if (ModelState.IsValid)
            {
                db.VenueTimes.Add(activityTimes);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(activityTimes);
        }

        // GET: TimeActivity/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VenueTimes activityTimes = db.VenueTimes.Find(id);
            if (activityTimes == null)
            {
                return HttpNotFound();
            }
            return View(activityTimes);
        }

        // POST: TimeActivity/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "VenueTimesId,SlotTime")] VenueTimes activityTimes)
        {
            if (ModelState.IsValid)
            {
                db.Entry(activityTimes).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(activityTimes);
        }

        // GET: TimeActivity/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VenueTimes activityTimes = db.VenueTimes.Find(id);
            if (activityTimes == null)
            {
                return HttpNotFound();
            }
            return View(activityTimes);
        }

        // POST: TimeActivity/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            VenueTimes activityTimes = db.VenueTimes.Find(id);
            db.VenueTimes.Remove(activityTimes);
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