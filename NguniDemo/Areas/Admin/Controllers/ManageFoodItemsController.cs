using Microsoft.AspNet.Identity;
using NguniDemo.Models;
using NguniDemo.Repositories;
using NguniDemo.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace NguniDemo.Areas.Admin.Controllers
{
    public class ManageFoodItemsController : Controller
    {
        public ApplicationDbContext db = new ApplicationDbContext();

        // GET: FoodItems

        public ActionResult Index(string searchTerm, int? subjectID, int? page)
        {
            ProductListingModel model = new ProductListingModel();
            string getuser = User.Identity.GetUserId();

            int recordSize = 5;
            page = page ?? 1;

            model.SearchTerm = searchTerm;
            model.FoodTypeID = subjectID;
            model.FoodTypes = GetAllSubjects();

            model.FoodItems = SearchLesson(searchTerm, subjectID, page.Value, recordSize);



            var totalRecords = SearchLessonCount(searchTerm);
            model.Pager = new Pager(totalRecords, page, recordSize);

            return View(model);
        }

        public int SearchLessonCount(string searchTerm)
        {
            var context = new ApplicationDbContext();
            var subjects = context.FoodItems.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                subjects = subjects.Where(a => a.FoodItemName.ToLower().Contains(searchTerm.ToLower()));
            }


            return subjects.Count();
        }


        public IEnumerable<FoodItem> GetAllLessons()
        {
            var context = new ApplicationDbContext();

            return context.FoodItems.Include(l => l.FoodItemName).ToList();

        }
        public IEnumerable<Food> GetAllSubjects()
        {
            var context = new ApplicationDbContext();

            return context.Foods.ToList();
        }


        public IEnumerable<FoodItem> SearchLesson(string searchTerm, int? assignID, int page, int recordSize)
        {
            var context = new ApplicationDbContext();
            var assignment = context.FoodItems.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                assignment = assignment.Where(a => a.FoodItemName.ToLower().Contains(searchTerm.ToLower()));
            }

            if (assignID.HasValue && assignID.Value > 0)
            {
                assignment = assignment.Where(a => a.FoodItemID == assignID.Value);
            }


            var skip = (page - 1) * recordSize;

            return assignment.OrderBy(t => t.FoodItemID == assignID).Skip(skip).Take(recordSize).ToList();
        }











        public async Task<ActionResult> Index1(string type = null)
        {

            if (type != null)
            {
                var foodItems = from a in db.FoodItems
                                where a.FoodType == type
                                select a;
                return View(await foodItems.ToListAsync());
            }
            else
            {
                var foodItems = db.FoodItems.Include(f => f.Food);

                return View(await foodItems.ToListAsync());
            }

        }

        // GET: FoodItems/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FoodItem foodItem = await db.FoodItems.FindAsync(id);
            if (foodItem == null)
            {
                return HttpNotFound();
            }
            return View(foodItem);
        }

        // GET: FoodItems/Create
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

        // GET: FoodItems/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FoodItem foodItem = await db.FoodItems.FindAsync(id);
            if (foodItem == null)
            {
                return HttpNotFound();
            }
            ViewBag.FoodType = new SelectList(db.Foods, "FoodType", "FoodType", foodItem.FoodType);
            return View(foodItem);
        }

        // POST: FoodItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "FoodItemID,FoodItemName,FoodType,ShortDesc,LongDesc,Price,ImageUrl")] FoodItem foodItem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(foodItem).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.FoodType = new SelectList(db.Foods, "FoodType", "FoodType", foodItem.FoodType);
            return View(foodItem);
        }

        // GET: FoodItems/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FoodItem foodItem = await db.FoodItems.FindAsync(id);
            if (foodItem == null)
            {
                return HttpNotFound();
            }
            return View(foodItem);
        }

        // POST: FoodItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            FoodItem foodItem = await db.FoodItems.FindAsync(id);
            db.FoodItems.Remove(foodItem);
            await db.SaveChangesAsync();
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