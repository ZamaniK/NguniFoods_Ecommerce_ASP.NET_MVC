using NguniDemo.Models;
using NguniDemo.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;
using System.Net;

namespace NguniDemo.Areas.Admin.Controllers
{
    public class AdminDashboardController : Controller
    {
        // GET: Admin/AdminDashboard
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult UploadPictures()
        {
            JsonResult result = new JsonResult();

            var dashboardService = new DashboardService();
            var picsList = new List<Picture>();
            var files = Request.Files;

            for (int i = 0; i < files.Count; i++)
            {
                var picture = files[i];
                var fileName = Guid.NewGuid() + Path.GetExtension(picture.FileName);
                var filePath = Server.MapPath("~/images/site/") + fileName;
                picture.SaveAs(filePath);

                var dbPicture = new Picture();
                dbPicture.URL = fileName;

                if (dashboardService.SavePicture(dbPicture))
                {
                    picsList.Add(dbPicture);
                }
            }
            result.Data = picsList;
            return result;
        }


        public ActionResult TableBookings()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var roomBookings = db.TableReservations.Include(r => r.Table);
            return View(roomBookings.ToList());
        }

        public ActionResult VenueBookings()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var roomBookings = db.VenueBookings;
            return View(roomBookings.ToList());
        }


        public ActionResult Edit(int? id)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TableReservation roomBooking = db.TableReservations.Find(id);
            if (roomBooking == null)
            {
                return HttpNotFound();
            }
            ViewBag.TableId = new SelectList(db.Table, "TableId", "TableDescription", roomBooking.TableId);
            return View(roomBooking);
        }

        // POST: RoomBookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TableReservationId,TableType,TableId,CustomerEmail,CheckInTime,CheckOutTime,NumberOfHours,NumberOfPeople")] TableReservation roomBooking)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            if (ModelState.IsValid)
            {
                db.Entry(roomBooking).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TableId = new SelectList(db.Table, "TableId", "roomDescription", roomBooking.TableId);
            return View(roomBooking);
        }

        // GET: RoomBookings/Delete/5
        public ActionResult Delete(int? id)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TableReservation roomBooking = db.TableReservations.Find(id);
            if (roomBooking == null)
            {
                return HttpNotFound();
            }
            return View(roomBooking);
        }

        // POST: RoomBookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            TableReservation roomBooking = db.TableReservations.Find(id);
            db.TableReservations.Remove(roomBooking);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
