using NguniDemo.Models;
using NguniDemo.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NguniDemo.Areas.Admin.Controllers
{
    public class ManageProductsController : Controller
    {

        public ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/ManageProducts
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.FoodType = new SelectList(db.Foods, "FoodType", "FoodType");
            return View();
        }

        // POST: FoodItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FoodItemID,FoodItemName,FoodType,ShortDesc,Imageurl,LongDesc,Price")] FoodItem foodItem, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {

                string uploadedFileName = FileUtils.UploadFile(file);
                var fooditem = new FoodItem
                {
                    FoodItemName = foodItem.FoodItemName,
                    FoodType = foodItem.FoodType,
                    ShortDesc = foodItem.ShortDesc,
                    LongDesc = foodItem.LongDesc,
                    Price = foodItem.Price,
                    ImageUrl = uploadedFileName
                };
                db.FoodItems.Add(fooditem);
                db.SaveChanges();

                return RedirectToAction("Index");


            }


            ViewBag.FoodType = new SelectList(db.Foods, "FoodType", "FoodType", foodItem.FoodType);
            return View(foodItem);
        }

    }
}