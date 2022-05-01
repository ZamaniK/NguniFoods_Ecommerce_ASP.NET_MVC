using NguniDemo.Models;
using NguniDemo.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NguniDemo.Controllers
{
    public class OrderController : Controller
    {
        // GET: Order
        public ActionResult CreateOrder(int? id)
        {
            Session["OrderId"] = id;
            List<CartVM> cart = Session["cart"] as List<CartVM>;
            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PlaceOrder()
        {
            // Get cart list
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            // Get username
            string username = User.Identity.Name;

            int orderId = 0;

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                // Init OrderDTO
                Order orderDTO = new Order();

                // Get user id
                var q = db.Users.FirstOrDefault(x => x.UserName == username);
                string userId = q.Id;

                // Add to OrderDTO and save
                orderDTO.ApplicationUserId = userId;
                orderDTO.OrderDate = DateTime.Now;

                db.Orders.Add(orderDTO);

                db.SaveChanges();

                // Get inserted id
                orderId = orderDTO.OrderId;
                Session["OrderId"] = orderId;

                // Init OrderDetailsDTO
                OrderDetail orderDetailsDTO = new OrderDetail();

                // Add to OrderDetailsDTO
                foreach (var item in cart)
                {
                    orderDetailsDTO.OrderId = orderId;
                    orderDetailsDTO.ApplicationUserId = userId;
                    orderDetailsDTO.FoodItemID = item.ProductId;
                    orderDetailsDTO.Quantity = item.Quantity;

                    db.OrderDetails.Add(orderDetailsDTO);

                    db.SaveChanges();
                    Session["cart"] = null;
                }

                TempData["AlertMessage"] = "Thank you for for placing your Order! \n An Email has been sent with your Order details";
                return RedirectToAction("ConfirmOrder", new { id = orderDetailsDTO.OrderId });
            }
            // Email admin
            //var client = new SmtpClient("mailtrap.io", 2525)
            //{
            //    Credentials = new NetworkCredential("21f57cbb94cf88", "e9d7055c69f02d"),
            //    EnableSsl = true
            //};
            //client.Send("admin@example.com", "admin@example.com", "New Order", "You have a new order. Order number " + orderId);

            // Reset session

        }

    }
}