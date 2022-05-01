using NguniDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;

namespace NguniDemo.Controllers
{
    public class VenueTimersController : Controller
    {
        // GET: VenueTimers
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var activityTimes1 = db.VenueTimes1.Include(a => a.Venues).Include(a => a.VenueTimes);
            return View(activityTimes1.ToList());
        }
        public ActionResult VenueBooking(int id)
        {
            var activityTimes1 = db.VenueTimes1.Include(a => a.Venues).Include(a => a.VenueTimes);
            return View(activityTimes1.ToList().Where(x => x.VenueId == id));
        }

    }
}