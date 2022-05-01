using NguniDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;
using System.Net;
using NguniDemo.ViewModels;

namespace NguniDemo.Areas.Admin.Controllers
{
    public class AdminVenueTimesController : Controller
    {
        // GET: Admin/AdminVenueTimes
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ActivityTimes
        public ActionResult Index(string searchTerm, int? venueID, int? page)
        {
            VenueListingModel model = new VenueListingModel();

            int recordSize = 5;
            page = page ?? 1;

            model.SearchTerm = searchTerm;
            model.TimeID = venueID;
            model.Venues = GetAllVenueTimes();

            model.Time = SearchLesson(searchTerm, venueID, page.Value, recordSize);



            var totalRecords = SearchLessonCount(searchTerm);
            model.Pager = new Pager(totalRecords, page, recordSize);

            return View(model);
        }

        public int SearchLessonCount(string searchTerm)
        {
            var context = new ApplicationDbContext();
            var subjects = context.VenueTimes1.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                subjects = subjects.Where(a => a.Venues.VenueName.ToLower().Contains(searchTerm.ToLower()));
            }


            return subjects.Count();
        }


        public IEnumerable<VenueTime> GetAllVenueTimes()
        {
            var context = new ApplicationDbContext();

            return context.VenueTimes1.Include(a => a.Venues).Include(a => a.VenueTimes).ToList();
        }
        


        public IEnumerable<VenueTime> SearchLesson(string searchTerm, int? assignID, int page, int recordSize)
        {
            var context = new ApplicationDbContext();
            var assignment = context.VenueTimes1.Include(a => a.Venues).Include(a => a.VenueTimes).AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                assignment = assignment.Where(a => a.Venues.VenueName.ToLower().Contains(searchTerm.ToLower()));
            }

            if (assignID.HasValue && assignID.Value > 0)
            {
                assignment = assignment.Where(a => a.VenueTimesId == assignID.Value);
            }


            var skip = (page - 1) * recordSize;

            return assignment.OrderBy(t => t.VenueTimesId == assignID).Skip(skip).Take(recordSize).ToList();
        }
       // public ActionResult Index()
       // {
       //     var activityTimes1 = db.VenueTimes1.Include(a => a.Venues).Include(a => a.VenueTimes);
       //     return View(activityTimes1.ToList());
      //  }
        public ActionResult ActivityBooking(int id)
        {
            var activityTimes1 = db.VenueTimes1.Include(a => a.Venues).Include(a => a.VenueTimes);
            return View(activityTimes1.ToList().Where(x => x.VenueId == id));
        }
        // GET: ActivityTimes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VenueTime activityTime = db.VenueTimes1.Find(id);
            if (activityTime == null)
            {
                return HttpNotFound();
            }
            return View(activityTime);
        }

        // GET: ActivityTimes/Create
        public ActionResult Create()
        {
            ViewBag.VenueId = new SelectList(db.Venues, "VenueId", "VenueName");
            ViewBag.VenueTimesId = new SelectList(db.VenueTimes, "VenueTimesId", "SlotTime");
            return View();
        }

        // POST: ActivityTimes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "VenueTimeId,VenueId,VenueTimesId")] VenueTime activityTime)
        {
            if (ModelState.IsValid)
            {
                db.VenueTimes1.Add(activityTime);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.VenueId = new SelectList(db.Venues, "VenueId", "VenueName", activityTime.VenueId);
            ViewBag.VenueTimesId = new SelectList(db.VenueTimes, "VenueTimesId", "SlotTime", activityTime.VenueTimesId);
            return View(activityTime);
        }

        // GET: ActivityTimes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VenueTime activityTime = db.VenueTimes1.Find(id);
            if (activityTime == null)
            {
                return HttpNotFound();
            }
            ViewBag.VenueId = new SelectList(db.Venues, "VenueId", "VenueName", activityTime.VenueId);
            ViewBag.VenueTimesId = new SelectList(db.VenueTimes, "VenueTimesId", "SlotTime", activityTime.VenueTimesId);
            return View(activityTime);
        }

        // POST: ActivityTimes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "VenueTimeId,VenueId,VenueTimesId")] VenueTime activityTime)
        {
            if (ModelState.IsValid)
            {
                db.Entry(activityTime).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.VenueId = new SelectList(db.Venues, "VenueId", "VenueName", activityTime.VenueId);
            ViewBag.VenueTimesId = new SelectList(db.VenueTimes, "VenueTimesId", "SlotTime", activityTime.VenueTimesId);
            return View(activityTime);
        }

        // GET: ActivityTimes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VenueTime activityTime = db.VenueTimes1.Find(id);
            if (activityTime == null)
            {
                return HttpNotFound();
            }
            return View(activityTime);
        }

        // POST: ActivityTimes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            VenueTime activityTime = db.VenueTimes1.Find(id);
            db.VenueTimes1.Remove(activityTime);
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