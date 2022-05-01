using NguniDemo.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace NguniDemo.Controllers
{
    public class HomeController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        // GET: Index
        public ActionResult Index()
        {
            List<Food> foods = db.Foods.ToList();

            return View(foods);
        }
        // GET: FoodItems
        public async Task<ActionResult> Browse(string type = "Appetizers")
        {
            var foodItems = from a in db.FoodItems
                            where a.FoodType == type
                            select a;
            ViewBag.Title = type;
            return View(await foodItems.ToListAsync());
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

        public ActionResult About()
        {
            List<Food> foods = db.Foods.ToList();

            return View(foods);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult ThankYouPage()
        {
            return View();
        }
        public ActionResult CancelPage()
        {
            return View();
        }
    }
}