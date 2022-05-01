using Microsoft.AspNet.Identity;
using NguniDemo.Models;
using NguniDemo.Repositories;
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
    public class TablesController : Controller
    {
        // GET: Admin/Tables
        TablesService accomodationPackageService = new TablesService();
        TableTypeService accomodationTypeService = new TableTypeService();
        DashboardService dashboardService = new DashboardService();
        // GET: Dashboard/AccomodationTypes
        public ActionResult Index(string searchTerm, int? accomodationTypeID, int? page)
        {
            TableListingModel model = new TableListingModel();

            int recordSize = 5;
            page = page ?? 1;

            model.SearchTerm = searchTerm;
            model.TableTypeID = accomodationTypeID;

            model.Tables = accomodationPackageService.SearchTables(searchTerm, accomodationTypeID, page.Value, recordSize);

            model.TableTypes = accomodationTypeService.GetAllTableTypes();

            var totalRecords = accomodationPackageService.SearchTableCount(searchTerm, accomodationTypeID);
            model.Pager = new Pager(totalRecords, page, recordSize);

            return View(model);
        }

        [HttpGet]
        public ActionResult Action(int? ID)
        {
            TableActionModel model = new TableActionModel();

            if (ID.HasValue)  //we are trying to edit a record
            {
                var accomodationPackage = accomodationPackageService.GetTableByID(ID.Value);

                model.ID = accomodationPackage.TableId;
                model.TableTypeID = accomodationPackage.TabletypeId;
                model.Name = accomodationPackage.TableName;
                model.Description = accomodationPackage.TableDescription;
                model.NoOfSeats = accomodationPackage.TableCapacity;
                model.TablePictures = accomodationPackageService.GetPicturesByAccomodationPackageID(accomodationPackage.TableId);
            }
            model.TableTypes = accomodationTypeService.GetAllTableTypes();
            return PartialView("_Action", model);
        }


        [HttpPost]
        public JsonResult Action(TableActionModel model)
        {
            JsonResult json = new JsonResult();

            var result = false;
            List<int> pictureIDs = !string.IsNullOrEmpty(model.PictureIDs) ? model.PictureIDs.Split(',').Select(x => int.Parse(x)).ToList() : new List<int>();
            var pictures = dashboardService.GetPicturesByID(pictureIDs);

            if (model.ID > 0)
            {
                var accomodationPackage = accomodationPackageService.GetTableByID(model.ID);

                accomodationPackage.TabletypeId = model.TableTypeID;
                accomodationPackage.TableName = model.Name;
                accomodationPackage.TableDescription = model.Description;
                accomodationPackage.TableCapacity = model.NoOfSeats;

                accomodationPackage.TablePictures.Clear();
                accomodationPackage.TablePictures.AddRange(pictures.Select(x => new Models.TablePictures() { TableID = accomodationPackage.TableId, PictureID = x.ID }));

                result = accomodationPackageService.UpdateTable(accomodationPackage);
            }
            else
            {
                Table accomodationPackage = new Table();

                accomodationPackage.TabletypeId = model.TableTypeID;
                accomodationPackage.TableName = model.Name;
                accomodationPackage.TableDescription = model.Description;
                accomodationPackage.TableCapacity = model.NoOfSeats;

                accomodationPackage.TablePictures = new List<TablePictures>();
                accomodationPackage.TablePictures.AddRange(pictures.Select(x => new TablePictures() { PictureID = x.ID }));

                result = accomodationPackageService.SaveAccomodationPackage(accomodationPackage);
            }


            if (result)
            {
                json.Data = new { Success = true };
            }
            else
            {
                json.Data = new { Success = false, Message = "Unable to perform action on Accomodation Package." };
            }
            return json;
        }


        [HttpGet]
        public ActionResult Delete(int ID)
        {
            TableActionModel model = new TableActionModel();

            var accomodationPackage = accomodationPackageService.GetTableByID(ID);

            model.ID = accomodationPackage.TableId;


            return PartialView("_Delete", model);
        }


        [HttpPost]
        public JsonResult Delete(TableActionModel model)
        {
            JsonResult json = new JsonResult();

            var result = false;

            var accomodationPackage = accomodationPackageService.GetTableByID(model.ID);



            result = accomodationPackageService.DeleteTable(accomodationPackage);

            if (result)
            {
                json.Data = new { Success = true };
            }
            else
            {
                json.Data = new { Success = false, Message = "Unable to perform action on Accomodation Packages." };
            }
            return json;
        }

        public ApplicationDbContext db = new ApplicationDbContext();
        // GET: Rooms/Create
        public ActionResult Create()
        {
            var userName = User.Identity.GetUserName();
            ViewBag.TabletypeId = new SelectList(db.TableTypes, "TabletypeId", "Name");
            return View();
        }

        // POST: Rooms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TableId,TabletypeId,TableCapacity,TableDescription,TablePicture,Status")] Table rooms, HttpPostedFileBase photoUpload)
        {

            byte[] photo = null;
            photo = new byte[photoUpload.ContentLength];
            photoUpload.InputStream.Read(photo, 0, photoUpload.ContentLength);
            rooms.TablePicture = photo;

            if (ModelState.IsValid)
            {
                db.Table.Add(rooms);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TabletypeId = new SelectList(db.TableTypes, "TabletypeId", "Name", rooms.TabletypeId);
            return View(rooms);
        }

        // GET: Rooms/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Table rooms = db.Table.Find(id);
            if (rooms == null)
            {
                return HttpNotFound();
            }
            ViewBag.TabletypeId = new SelectList(db.TableTypes, "TabletypeId", "Name", rooms.TabletypeId);
            return View(rooms);
        }

        // POST: Rooms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TableId,TabletypeId,TableCapacity,TableDescription,TablePicture,Status")] Table rooms)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rooms).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TabletypeId = new SelectList(db.TableTypes, "TabletypeId", "Name", rooms.TabletypeId);
            return View(rooms);
        }

        // GET: Rooms/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Table rooms = db.Table.Find(id);
            if (rooms == null)
            {
                return HttpNotFound();
            }
            return View(rooms);
        }

        // POST: Rooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Table rooms = db.Table.Find(id);
            db.Table.Remove(rooms);
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