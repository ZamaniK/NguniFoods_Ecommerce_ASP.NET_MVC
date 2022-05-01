using Microsoft.AspNet.Identity;
using NguniDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;
using System.Net;
using NguniDemo.Repositories;

namespace NguniDemo.Controllers
{
    public class TableReservationController : Controller
    {
        // GET: TableReservation
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: RoomBookings
        public ActionResult Index()
        {
            var roomBookings = db.TableReservations.Include(r => r.Table);
            return View(roomBookings.ToList());
        }
        public ActionResult ChckIn()
        {
            var userName = User.Identity.GetUserName();
            var roomBookings = db.TableReservations.Include(r => r.Table);
            return View(roomBookings.ToList().Where(x => x.CustomerEmail == userName));
        }
        public ActionResult CheckInn(int? id)
        {
            TableReservation bookingRoom = db.TableReservations.Find(id);
            var guestName = db.Users.ToList().Where(p => p.Email == User.Identity.GetUserName()).Select(p => p.Email).FirstOrDefault();
            if (bookingRoom.Status == "Checked Out")
            {
                TempData["AlertMessage"] = "Cannot check in a person whohas already been checked out";
                return RedirectToAction("ChckIn");
            }
            else
            if (bookingRoom.Status == "Checked In")
            {
                TempData["AlertMessage"] = "Cannot check in " + guestName + " twice";
                return RedirectToAction("ChckIn");
            }
            else
            if (id != null)
            {
                bookingRoom.Status = "Checked In";
                db.Entry(bookingRoom).State = EntityState.Modified;
                db.SaveChanges();
                TempData["AlertMessage"] = guestName + " Has been Successfully checked in";
                return RedirectToAction("ChckIn");
            }
            return View();
        }
        public ActionResult CheckOut(int? id)
        {
            TableReservation bookingRoom = db.TableReservations.Find(id);
            var guestName = db.Users.ToList().Where(p => p.Email == User.Identity.GetUserName()).Select(p => p.Email).FirstOrDefault();

            if (bookingRoom.Status == "Not yet Checked In!!")
            {
                TempData["AlertMessage"] = "Cannot check out a person who has not been checked in";
                return RedirectToAction("ChckIn");
            }
            else if (bookingRoom.Status == "Checked Out")
            {
                TempData["AlertMessage"] = "Cannot check out " + guestName + " twice";
                return RedirectToAction("ChckIn");
            }
            else if (id != null)
            {
                bookingRoom.Status = "Checked Out";
                db.Entry(bookingRoom).State = EntityState.Modified;
                db.SaveChanges();

                var q = db.Table.Where(p => p.TableId == bookingRoom.TableId).Select(p => p.TabletypeId).FirstOrDefault();
                var rty = db.TableTypes.Where(p => p.TabletypeId == q).FirstOrDefault();
                rty.TableAvailable++;
                db.Entry(rty).State = EntityState.Modified;
                db.SaveChanges();

                TempData["AlertMessage"] = guestName + " Successfully checked Out";
                return RedirectToAction("Index");
            }
            return View();
        }
        public ActionResult MyBookings()
        {
            var userName = User.Identity.GetUserName();
            var roomBookings = db.TableReservations.Include(r => r.Table);
            return View(roomBookings.ToList().Where(x => x.CustomerEmail == userName));
        }
        // GET: RoomBookings/Details/5
        public ActionResult Details(int? id)
        {
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
        public ActionResult ConfirmBooking(int? id)
        {
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

        // GET: RoomBookings/Create
        public ActionResult Create(int? id)
        {
            ViewBag.Id = id;
            Session["TableId"] = id;

            ViewBag.TableId = new SelectList(db.Table, "TableId", "TableDescription");
            return View();
        }

        // POST: RoomBookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TableReservationId,TableType,TableId,CustomerEmail,CheckInTime,CheckOutTime,NumberOfHours,NumberOfPeople")] TableReservation roomBooking)
        {
            var userName = User.Identity.GetUserName();
            if (ModelState.IsValid)
            {
                if (BusinessService.dateLessOutChecker(roomBooking) == true)
                {
                    TempData["AlertMessage"] = " Booked";
                    return View(roomBooking);
                }
                else
                {
                    roomBooking.TableId = (int)Session["TableId"];
                    roomBooking.CustomerEmail = userName;
                    roomBooking.TableType = BusinessService.GetTableType(roomBooking.TableId);
                    roomBooking.NumberOfHours = BusinessService.GetNumberHours(roomBooking.CheckInTime, roomBooking.CheckOutTime);
                    roomBooking.Status = "Not yet Checked In!!";


                    db.TableReservations.Add(roomBooking);
                    db.SaveChanges();
                    Session["bookID"] = roomBooking.TableBookingId;
                    return RedirectToAction("ConfirmBooking" ,new { id = roomBooking.TableBookingId });
                }
            }
            ViewBag.TableId = new SelectList(db.Table, "TableId", "TableDescription", roomBooking.TableId);
            return View(roomBooking);
        }

        // GET: RoomBookings/Edit/5
        public ActionResult Edit(int? id)
        {
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
            TableReservation roomBooking = db.TableReservations.Find(id);
            db.TableReservations.Remove(roomBooking);
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