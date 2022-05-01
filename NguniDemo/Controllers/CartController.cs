using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNet.Identity;
using NguniDemo.Models;
using NguniDemo.Repositories;
using NguniDemo.ViewModels;
using PayFast;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace NguniDemo.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        public ActionResult Index()
        {
            // Init the cart list
            var cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            // Check if cart is empty
            if (cart.Count == 0 || Session["cart"] == null)
            {
                ViewBag.Message = "Your cart is empty.";
                return View();
            }

            // Calculate total and save to ViewBag

            decimal total = 0m;

            foreach (var item in cart)
            {
                total += item.Total;
            }

            ViewBag.GrandTotal = total;

            // Return view with list
            return View(cart);
        }

        public ActionResult CartPartial()
        {
            // Init CartVM
            CartVM model = new CartVM();

            // Init quantity
            int qty = 0;

            // Init price
            decimal price = 0m;

            // Check for cart session
            if (Session["cart"] != null)
            {
                // Get total qty and price
                var list = (List<CartVM>)Session["cart"];

                foreach (var item in list)
                {
                    qty += item.Quantity;
                    price += item.Quantity * item.Price;
                }

                model.Quantity = qty;
                model.Price = price;

            }
            else
            {
                // Or set qty and price to 0
                model.Quantity = 0;
                model.Price = 0m;
            }

            // Return partial view with model
            return PartialView(model);
        }

        public ActionResult AddToCartPartial(int id)
        {
            // Init CartVM list
            List<CartVM> cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            // Init CartVM
            CartVM model = new CartVM();

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                // Get the product
                FoodItem product = db.FoodItems.Find(id);

                // Check if the product is already in cart
                var productInCart = cart.FirstOrDefault(x => x.ProductId == id);

                // If not, add new
                if (productInCart == null)
                {
                    cart.Add(new CartVM()
                    {
                        ProductId = product.FoodItemID,
                        ProductName = product.FoodItemName,
                        Quantity = 1,
                        Price = product.Price,
                        Image = product.ImageUrl
                    });
                }
                else
                {
                    // If it is, increment
                    productInCart.Quantity++;
                }
            }

            // Get total qty and price and add to model

            int qty = 0;
            decimal price = 0m;

            foreach (var item in cart)
            {
                qty += item.Quantity;
                price += item.Quantity * item.Price;
            }

            model.Quantity = qty;
            model.Price = price;

            // Save cart back to session
            Session["cart"] = cart;

            // Return partial view with model
            return PartialView(model);
        }

        // GET: /Cart/IncrementProduct
        public JsonResult IncrementProduct(int productId)
        {
            // Init cart list
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                // Get cartVM from list
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

                // Increment qty
                model.Quantity++;

                // Store needed data
                var result = new { qty = model.Quantity, price = model.Price };

                // Return json with data
                return Json(result, JsonRequestBehavior.AllowGet);
            }

        }

        // GET: /Cart/DecrementProduct
        public ActionResult DecrementProduct(int productId)
        {
            // Init cart
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                // Get model from list
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

                // Decrement qty
                if (model.Quantity > 1)
                {
                    model.Quantity--;
                }
                else
                {
                    model.Quantity = 0;
                    cart.Remove(model);
                }

                // Store needed data
                var result = new { qty = model.Quantity, price = model.Price };

                // Return json
                return Json(result, JsonRequestBehavior.AllowGet);
            }

        }

        // GET: /Cart/RemoveProduct
        public void RemoveProduct(int productId)
        {
            // Init cart list
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                // Get model from list
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

                // Remove model from list
                cart.Remove(model);
            }

        }

        public ActionResult PaypalPartial()
        {
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            return PartialView(cart);
        }
        [HttpGet]
        public ActionResult PlaceOrder(int? id)
        {


            return View();
        }
        public ActionResult CancelOrder()
        {
            return RedirectToAction("Index");
        }
        public ActionResult YourOrder(int? id)
        {
            id = 1;
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            // Calculate total and save to ViewBag

            decimal total = 0m;

            foreach (var item in cart)
            {
                total += item.Total;
            }

            ViewBag.GrandTotal = total;
            return View(cart);
        }

        // POST: /Cart/PlaceOrder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult YourOrder()
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
        public ActionResult ConfirmOrder(int? id)
        {
            // OrdersForAdminVM ordersForAdmin = new OrdersForAdminVM();
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            ApplicationDbContext db = new ApplicationDbContext();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order orders = db.Orders.Find(id);
            // Init product dict
            Dictionary<string, int> productsAndQty = new Dictionary<string, int>();
            OrderDetail orderDetail = db.OrderDetails.Where(X => X.OrderId == orders.OrderId).FirstOrDefault();
            ApplicationUser user = db.Users.Where(x => x.Id == orders.ApplicationUserId).FirstOrDefault();
            string username = user.Email;

            // Declare total

            //Fix


            decimal total = 0m;

            foreach (var item in cart)
            {
                total += item.Total;
            }

            ViewBag.GrandTotal = total;









            // Get product


            FoodItem product = db.FoodItems.FirstOrDefault();

            // Get product price
            decimal price = product.Price;

            // Get product name
            string productName = product.FoodItemName;

            // Add to product dict
            productsAndQty.Add(productName, orderDetail.Quantity);

            // Get total
            total += orderDetail.Quantity * price;
            if (orders == null)
            {
                return HttpNotFound();
            }
            OrdersForAdminVM ordersForAdmin = new OrdersForAdminVM()
            {
                OrderNumber = orders.OrderId,
                Username = username,
                Total = total,
                ProductsAndQty = productsAndQty,
                CreatedAt = orders.OrderDate
            };
            return View(ordersForAdmin);
        }

        #region Fields

        private readonly PayFastSettings payFastSettings;

        #endregion Fields

        #region Constructor

        public CartController()
        {
            this.payFastSettings = new PayFastSettings();
            this.payFastSettings.MerchantId = ConfigurationManager.AppSettings["MerchantId"];
            this.payFastSettings.MerchantKey = ConfigurationManager.AppSettings["MerchantKey"];
            this.payFastSettings.PassPhrase = ConfigurationManager.AppSettings["PassPhrase"];
            this.payFastSettings.ProcessUrl = ConfigurationManager.AppSettings["ProcessUrl"];
            this.payFastSettings.ValidateUrl = ConfigurationManager.AppSettings["ValidateUrl"];
            this.payFastSettings.ReturnUrl = ConfigurationManager.AppSettings["ReturnUrl"];
            this.payFastSettings.CancelUrl = ConfigurationManager.AppSettings["CancelUrl"];
            this.payFastSettings.NotifyUrl = ConfigurationManager.AppSettings["NotifyUrl"];
        }

        #endregion Constructor

        #region Methods
        ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult OnceOff()
        {
            var onceOffRequest = new PayFastRequest(this.payFastSettings.PassPhrase);

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
                var q = db.Users.FirstOrDefault(x => x.Email == username);
                string userId = q.Id;

                // Add to OrderDTO and save
                orderDTO.ApplicationUserId = userId;
                orderDTO.OrderDate = DateTime.Now;

                db.Orders.Add(orderDTO);

                db.SaveChanges();

                // Get inserted id
                orderId = orderDTO.OrderId;

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

                }
            }








            // Init CartVM
            CartVM model = new CartVM();

            // Init quantity
            int qty = 0;

            // Init price
            decimal price = 0m;
            if (Session["cart"] != null)
            {
                // Get total qty and price
                var list = (List<CartVM>)Session["cart"];

                foreach (var item in list)
                {
                    qty += item.Quantity;
                    price += item.Quantity * item.Price;
                }

                model.Quantity = qty;
                model.Price = price;

            }
            else
            {
                // Or set qty and price to 0
                model.Quantity = 0;
                model.Price = 0m;
            }

            



            // Merchant Details
            onceOffRequest.merchant_id = this.payFastSettings.MerchantId;
            onceOffRequest.merchant_key = this.payFastSettings.MerchantKey;
            onceOffRequest.return_url = this.payFastSettings.ReturnUrl;
            onceOffRequest.cancel_url = this.payFastSettings.CancelUrl;
            onceOffRequest.notify_url = this.payFastSettings.NotifyUrl;

            // Buyer Details
            onceOffRequest.email_address = "sbtu01@payfast.co.za";

            // Transaction Details
            onceOffRequest.m_payment_id = "8d00bf49-e979-4004-228c-08d452b86380";
            //onceOffRequest.amount = 30;
            onceOffRequest.amount = Convert.ToDouble(price);

            onceOffRequest.item_name = "Shop Once off option";
            onceOffRequest.item_description = "Some details about the once off payment";

            // Transaction Options
            onceOffRequest.email_confirmation = true;
            onceOffRequest.confirmation_address = "sbtu01@payfast.co.za";

            var redirectUrl = $"{this.payFastSettings.ProcessUrl}{onceOffRequest.ToString()}";


            var attachments = new List<Attachment>();
            attachments.Add(new Attachment(new MemoryStream(GeneratePDF(orderId)), "Nguni Shop Receipt", "application/pdf"));


            var mailTo = new List<MailAddress>();
            mailTo.Add(new MailAddress(User.Identity.GetUserName(), username));
            var body = $"Hello {username}, please see attached receipt for the recent shopping you made. <br/>Make sure you bring along your receipt when you collect your Order.<br/>";
            mailService emailService = new mailService();
            emailService.SendEmail(new mailContent()
            {
                mailTo = mailTo,
                mailCc = new List<MailAddress>(),
                mailSubject = "Nguni Shop Order | Order No.:" + orderId,
                mailBody = body,
                mailFooter = "<br/> Many Thanks, <br/> <b>Nguni Foods</b>",
                mailPriority = MailPriority.High,
                mailAttachments = attachments

            });
            // Reset session
            Session["cart"] = null;

            return Redirect(redirectUrl);

        }


        public ActionResult Return()
        {
            return View();
        }

        public ActionResult Cancel()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Notify([ModelBinder(typeof(PayFastNotifyModelBinder))] PayFastNotify payFastNotifyViewModel)
        {
            payFastNotifyViewModel.SetPassPhrase(this.payFastSettings.PassPhrase);

            var calculatedSignature = payFastNotifyViewModel.GetCalculatedSignature();

            var isValid = payFastNotifyViewModel.signature == calculatedSignature;

            System.Diagnostics.Debug.WriteLine($"Signature Validation Result: {isValid}");

            // The PayFast Validator is still under developement
            // Its not recommended to rely on this for production use cases
            var payfastValidator = new PayFastValidator(this.payFastSettings, payFastNotifyViewModel, IPAddress.Parse(this.HttpContext.Request.UserHostAddress));

            var merchantIdValidationResult = payfastValidator.ValidateMerchantId();

            System.Diagnostics.Debug.WriteLine($"Merchant Id Validation Result: {merchantIdValidationResult}");

            var ipAddressValidationResult = payfastValidator.ValidateSourceIp();

            System.Diagnostics.Debug.WriteLine($"Ip Address Validation Result: {merchantIdValidationResult}");

            // Currently seems that the data validation only works for successful payments
            if (payFastNotifyViewModel.payment_status == PayFastStatics.CompletePaymentConfirmation)
            {
                var dataValidationResult = await payfastValidator.ValidateData();

                System.Diagnostics.Debug.WriteLine($"Data Validation Result: {dataValidationResult}");
            }

            if (payFastNotifyViewModel.payment_status == PayFastStatics.CancelledPaymentConfirmation)
            {
                System.Diagnostics.Debug.WriteLine($"Subscription was cancelled");
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public ActionResult Error()
        {
            return View();
        }


        #endregion Methods
        public byte[] GeneratePDF(int PackBookingID)
        {
            MemoryStream memoryStream = new MemoryStream();
            Document document = new Document(PageSize.A5);
            PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
            document.Open();

            Order actBooking = new Order();
            actBooking = db.Orders.Find(PackBookingID);
            var userName = User.Identity.GetUserName();
            var guest = db.Users.Where(x => x.Email == userName).FirstOrDefault();

            Font font_heading_3 = FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, Font.BOLD, BaseColor.RED);
            Font font_body = FontFactory.GetFont(FontFactory.TIMES_ROMAN, 9, BaseColor.BLUE);

            // Create the heading paragraph with the headig font
            PdfPTable table1 = new PdfPTable(1);
            PdfPTable table2 = new PdfPTable(5);
            PdfPTable table3 = new PdfPTable(1);

            iTextSharp.text.pdf.draw.VerticalPositionMark seperator = new iTextSharp.text.pdf.draw.LineSeparator();
            seperator.Offset = -6f;
            // Remove table cell
            table1.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
            table3.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;

            table1.WidthPercentage = 80;
            table1.SetWidths(new float[] { 100 });
            table2.WidthPercentage = 80;
            table3.SetWidths(new float[] { 100 });
            table3.WidthPercentage = 80;

            PdfPCell cell = new PdfPCell(new Phrase(""));
            cell.Colspan = 3;
            table1.AddCell("\n");
            table1.AddCell(cell);
            table1.AddCell("\n\n");
            table1.AddCell(
                "\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t" +
                "Nguni Foods Receipt \n" +
                "Email : Zamanikat100@gmail.com" + "\n" +
                "\n" + "\n");
            table1.AddCell("------------Your Details--------------!");

            table1.AddCell("Email : \t" + guest.Email);
            table1.AddCell("Phone Number : \t" + guest.PhoneNumber);

            table1.AddCell("\n------------Order details--------------!\n");

            table1.AddCell("Order No. : \t" + actBooking.OrderId);
            table1.AddCell("Order Details. : \t" + actBooking.OrderDetails);

            table1.AddCell("Order Date : \t" + actBooking.OrderDate.ToString());
            table1.AddCell("\n");
            table1.AddCell("Total Cost: \t" + actBooking.Total.ToString("C"));

            table1.AddCell("\n");

            table3.AddCell("------------Looking forward to hear from you soon--------------!");

            //////Intergrate information into 1 document
            //var qrCode = iTextSharp.text.Image.GetInstance(reservation.QrCodeImage);
            //qrCode.ScaleToFit(200, 200);
            table1.AddCell(cell);
            document.Add(table1);
            //document.Add(qrCode);
            document.Add(table3);
            document.Close();

            byte[] bytes = memoryStream.ToArray();
            memoryStream.Close();
            return bytes;
        }

    }
}


