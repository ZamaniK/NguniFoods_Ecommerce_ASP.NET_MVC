using NguniDemo.Models;
using NguniDemo.Repositories;
using NguniDemo.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NguniDemo.Areas.Admin.Controllers
{
    public class ManageFoodController : Controller
    {
        // GET: Admin/ManageFood



        FoodTypeService gradesService = new FoodTypeService();
        // GET: Dashboard/Grades
        public ActionResult Index(string searchTerm, int? page)
        {
            GradeListingModel model = new GradeListingModel();

            int recordSize = 5;
            page = page ?? 1;

            model.SearchTerm = searchTerm;

            model.FoodTypes = gradesService.SearchGrades(searchTerm, page.Value, recordSize);

            var totalRecords = gradesService.SearchGradeCount(searchTerm);
            model.Pager = new Pager(totalRecords, page, recordSize);

            return View(model);
        }

        [HttpGet]
        public ActionResult Action(int? ID)
        {
            GradeActionModel model = new GradeActionModel();

            if (ID.HasValue)  //we are trying to edit a record
            {
                var grade = gradesService.GetGradeByID(ID.Value);

                model.ID = grade.FoodID;
                model.FoodType = grade.FoodType;
            }
            return PartialView("Action", model);
        }


        [HttpPost]
        public JsonResult Action(GradeActionModel model)
        {
            JsonResult json = new JsonResult();

            var result = false;

            if (model.ID > 0)
            {
                var grade = gradesService.GetGradeByID(model.ID);

                grade.FoodType = model.FoodType;

                result = gradesService.UpdateGrade(grade);
            }
            else
            {
                Food grade = new Food
                {
                    FoodType = model.FoodType
                };

                result = gradesService.SaveGrade(grade);
            }


            if (result)
            {
                json.Data = new { Success = true };
            }
            else
            {
                json.Data = new { Success = false, Message = "Unable to perform action on Grade." };
            }
            return json;
        }


        [HttpGet]
        public ActionResult Delete(int ID)
        {
            GradeActionModel model = new GradeActionModel();

            var grade = gradesService.GetGradeByID(ID);

            model.ID = grade.FoodID;


            return PartialView("_Delete", model);
        }


        [HttpPost]
        public JsonResult Delete(GradeActionModel model)
        {
            JsonResult json = new JsonResult();

            var result = false;

            var grade = gradesService.GetGradeByID(model.ID);

            result = gradesService.DeleteGrade(grade);

            if (result)
            {
                json.Data = new { Success = true };
            }
            else
            {
                json.Data = new { Success = false, Message = "Unable to perform action on Grade." };
            }
            return json;
        }
        public ActionResult FoodTypes()
        {
            // Declare a list of models
            List<Food> categoryVMList;

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                // Init the list
                categoryVMList = db.Foods
                                .ToArray()
                                .ToList();
            }

            // Return view with list
            return View(categoryVMList);
        }

        // POST: Admin/ManageFood/AddNewCategory
        [HttpPost]
        public string AddNewCategory(string catName)
        {
            // Declare id
            string id;

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                // Check that the category name is unique
                if (db.Foods.Any(x => x.FoodType == catName))
                    return "titletaken";

                // Init DTO
                Food dto = new Food();

                // Add to DTO
                dto.FoodType = catName;
                

                // Save DTO
                db.Foods.Add(dto);
                db.SaveChanges();

                // Get the id
                id = dto.FoodID.ToString();
            }

            // Return id
            return id;
        }

        public ActionResult Products(int? page, int? catId)
        {
            // Declare a list of ProductVM
            List<ProductVM> listOfProductVM;

            // Set page number
            var pageNumber = page ?? 1;

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                // Init the list
                listOfProductVM = db.FoodItems.ToArray()
                                  .Where(x => catId == null || catId == 0 || x.FoodId == catId)
                                  .Select(x => new ProductVM(x))
                                  .ToList();

                // Populate categories select list
                ViewBag.Categories = new SelectList(db.Foods.ToList(), "FoodId", "FoodType");

                // Set selected category
                ViewBag.SelectedCat = catId.ToString();
            }

            // Set pagination
            var onePageOfProducts = listOfProductVM.ToPagedList(pageNumber, 3);
            ViewBag.OnePageOfProducts = onePageOfProducts;

            // Return view with list
            return View(listOfProductVM);
        }

        public ActionResult Orders()
        {
            // Init list of OrdersForAdminVM
            List<OrdersForAdminVM> ordersForAdmin = new List<OrdersForAdminVM>();

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                // Init list of OrderVM
                List<OrderVM> orders = db.Orders.ToArray().Select(x => new OrderVM(x)).ToList();

                // Loop through list of OrderVM
                foreach (var order in orders)
                {
                    // Init product dict
                    Dictionary<string, int> productsAndQty = new Dictionary<string, int>();

                    // Declare total
                    decimal total = 0m;

                    // Init list of OrderDetailsDTO
                    List<OrderDetail> orderDetailsList = db.OrderDetails.Where(X => X.OrderId == order.OrderId).ToList();

                    // Get username
                    ApplicationUser user = db.Users.Where(x => x.Id == order.UserId).FirstOrDefault();
                    string username = user.Email;

                    // Loop through list of OrderDetailsDTO
                    foreach (var orderDetails in orderDetailsList)
                    {
                        // Get product
                        FoodItem product = db.FoodItems.Where(x => x.FoodItemID == orderDetails.FoodItemID).FirstOrDefault();

                        // Get product price
                        decimal price = product.Price;

                        // Get product name
                        string productName = product.FoodItemName;

                        // Add to product dict
                        productsAndQty.Add(productName, orderDetails.Quantity);

                        // Get total
                        total += orderDetails.Quantity * price;
                    }

                    // Add to ordersForAdminVM list
                    ordersForAdmin.Add(new OrdersForAdminVM()
                    {
                        OrderNumber = order.OrderId,
                        Username = username,
                        Total = total,
                        ProductsAndQty = productsAndQty,
                        CreatedAt = order.CreatedAt
                    });
                }
            }

            // Return view with OrdersForAdminVM list
            return View(ordersForAdmin);
        }
    }
}