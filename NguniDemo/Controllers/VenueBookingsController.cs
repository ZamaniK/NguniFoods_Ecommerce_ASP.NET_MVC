using NguniDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Net;
using System.Configuration;
using PayFast;
using System.Net.Mail;
using System.IO;
using System.Threading.Tasks;
using iTextSharp.text.pdf;
using iTextSharp.text;
using NguniDemo.Repositories;

namespace NguniDemo.Controllers
{
    public class VenueBookingsController : Controller
    {
        // GET: VenueBookings
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ActivityBookings
        public ActionResult Index()
        {
            var packageBookings = db.VenueBookings.Include(a => a.VenueTime);
            return View(packageBookings.ToList());
        }
        public ActionResult Index2()
        {
            var userName = User.Identity.GetUserName();
            var packageBookings = db.VenueBookings.Include(a => a.VenueTime);
            return View(packageBookings.ToList().Where(x => x.CustomerEmail == userName));
        }

        // GET: ActivityBookings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VenueBooking packageBookings = db.VenueBookings.Find(id);
            if (packageBookings == null)
            {
                return HttpNotFound();
            }
            return View(packageBookings);
        }

        // GET: ActivityBookings/Create
        public ActionResult Create(int? id)
        {
            Session["TimePackageId"] = id;
            ViewBag.Id = id;
            ViewBag.VenueTimeId = new SelectList(db.VenueTimes1, "VenueTimeId", "VenueTimeId");
            return View();
        }

        // POST: ActivityBookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "VenueBookingId,ArrivalDate,numOfPeople,BasicPrice,CustomerEmail,CustomerName,CustomerLastName,Status,VenueName,VenueType,TimeSlot,VenueTimeId")] VenueBooking packageBookings)
        {
            var userName = User.Identity.GetUserName();
            if (ModelState.IsValid)
            {
                if (packageBookings.CheckBooking(packageBookings) == false)
                {
                    if (packageBookings.ArrivalDate >= DateTime.Now.Date)
                    {
                        packageBookings.VenueTimeId = int.Parse(Session["TimePackageId"].ToString());

                        packageBookings.BasicPrice = packageBookings.getVenuePrice();
                        packageBookings.VenueType = packageBookings.getVenueType();
                        packageBookings.VenueName = packageBookings.getVenueName();
                        packageBookings.TimeSlot = packageBookings.getTimeSlot();
                        packageBookings.CustomerEmail = userName;
                        packageBookings.Status = "Pending";
                        packageBookings.CustomerName = packageBookings.getCustomerName(userName);
                        packageBookings.CustomerLastName = packageBookings.getCustomerName(userName);

                        db.VenueBookings.Add(packageBookings);
                        db.SaveChanges();
                        Session["packageID"] = packageBookings.VenueBookingId;
                        VenueBooking.SendEmail(packageBookings);
                        TempData["AlertMessage"] = "Thank you for your booking. \n An Email has been sent with your booking details";
                        return RedirectToAction("ConfirmBooking", new { id = packageBookings.VenueBookingId });
                    }
                    else
                    {
                        ModelState.AddModelError("", "Can't book for a date that has already passed!!");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Time already booked for, Please try other time slots!!");
                }

            }

            ViewBag.VenueTimeId = new SelectList(db.VenueTimes1, "VenueTimeId", "VenueTimeId", packageBookings.VenueTimeId);
            return View(packageBookings);
        }


        


        // GET: ActivityBookings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VenueBooking packageBookings = db.VenueBookings.Find(id);
            if (packageBookings == null)
            {
                return HttpNotFound();
            }
            ViewBag.VenueTimeId = new SelectList(db.VenueTimes1, "VenueTimeId", "VenueTimeId", packageBookings.VenueTimeId);
            return View(packageBookings);
        }

        // POST: ActivityBookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "VenueBookingId,ArrivalDate,numOfPeople,BasicPrice,CustomerEmail,CustomerName,CustomerLastName,Status,VenueName,VenueType,TimeSlot,VenueTimeId")] VenueBooking packageBookings)
        {
            if (ModelState.IsValid)
            {
                db.Entry(packageBookings).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index2");
            }
            ViewBag.VenueTimeId = new SelectList(db.VenueTimes1, "VenueTimeId", "VenueTimeId", packageBookings.VenueTimeId);
            return View(packageBookings);
        }

        // GET: ActivityBookings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VenueBooking packageBookings = db.VenueBookings.Find(id);
            if (packageBookings == null)
            {
                return HttpNotFound();
            }
            return View(packageBookings);
        }

        // POST: ActivityBookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            VenueBooking packageBookings = db.VenueBookings.Find(id);
            db.VenueBookings.Remove(packageBookings);
            db.SaveChanges();
            return RedirectToAction("Index2");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public ActionResult ConfirmBooking(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VenueBooking roomBooking = db.VenueBookings.Find(id);
            if (roomBooking == null)
            {
                return HttpNotFound();
            }
            return View(roomBooking);
        }
        public VenueBookingsController()
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
        //Payment
        #region Fields

        private readonly PayFastSettings payFastSettings;

        #endregion Fields

        #region Constructor

        //public ApprovedOwnersController()
        //{

        //}

        #endregion Constructor

        #region Methods



       

        public ActionResult OnceOff()
        {
            var onceOffRequest = new PayFastRequest(this.payFastSettings.PassPhrase);
            int ReservationID = int.Parse(Session["packageID"].ToString());
            VenueBooking packBook = new VenueBooking();
            packBook = db.VenueBookings.Find(ReservationID);
            var userName = User.Identity.GetUserName();
            var guest = db.Users.Where(x => x.Email == userName).FirstOrDefault();

            var attachments = new List<Attachment>();
            attachments.Add(new Attachment(new MemoryStream(GeneratePDF(ReservationID)), "Venue Booking Receipt", "application/pdf"));


            var mailTo = new List<MailAddress>();
            mailTo.Add(new MailAddress(User.Identity.GetUserName(), guest.Email));
            var body = $"Hello {guest.Email}, please see attached receipt for the recent reservation you made. <br/>Make sure you bring along your receipt when you check in the venue.<br/>";
            mailService emailService = new mailService();
            emailService.SendEmail(new mailContent()
            {
               mailTo = mailTo,
               mailCc = new List<MailAddress>(),
               mailSubject = "Nguni Venue Booking | Ref No.:" + packBook.VenueBookingId,
               mailBody = body,
               mailFooter = "<br/> Many Thanks, <br/> <b>Nguni Traditional Foods</b>",
               mailPriority = MailPriority.High,
               mailAttachments = attachments

            });
            // Merchant Details
            onceOffRequest.merchant_id = this.payFastSettings.MerchantId;
            onceOffRequest.merchant_key = this.payFastSettings.MerchantKey;
            onceOffRequest.return_url = this.payFastSettings.ReturnUrl;
            onceOffRequest.cancel_url = this.payFastSettings.CancelUrl;
            onceOffRequest.notify_url = this.payFastSettings.NotifyUrl;

            // Buyer Details

            onceOffRequest.email_address = "sbtu01@payfast.co.za";
            //onceOffRequest.email_address = "sbtu01@payfast.co.za";

            // Transaction Details
            //onceOffRequest.m_payment_id = "";



            onceOffRequest.m_payment_id = "8d00bf49-e979-4004-228c-08d452b86380";
            //onceOffRequest.amount = 350;
            onceOffRequest.amount = Convert.ToDouble(packBook.BasicPrice);
            onceOffRequest.item_name = "Venue Booking payment";
            onceOffRequest.item_description = "Some details about the once off payment";


            // Transaction Options
            onceOffRequest.email_confirmation = true;
            onceOffRequest.confirmation_address = "sbtu01@payfast.co.za";

            var redirectUrl = $"{this.payFastSettings.ProcessUrl}{onceOffRequest.ToString()}";
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

            VenueBooking actBooking = new VenueBooking();
            actBooking = db.VenueBookings.Find(PackBookingID);
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
                "Nguni Venue Receipt \n" +
                "Email : Zamanikat100@gmail.com" + "\n" +
                "\n" + "\n");
            table1.AddCell("------------Your Details--------------!");

            table1.AddCell("Email : \t" + guest.Email);
            table1.AddCell("Phone Number : \t" + guest.PhoneNumber);

            table1.AddCell("\n------------Booking details--------------!\n");

            table1.AddCell("Booking Ref No. : \t" + actBooking.VenueBookingId);
            table1.AddCell("Payment Status. : \t" + actBooking.Status);
            table1.AddCell("Venue Type : \t" + actBooking.VenueType);
            table1.AddCell("Venue Name : \t" + actBooking.VenueName);

            table1.AddCell("Arrival date : \t" + actBooking.ArrivalDate.ToString());
            table1.AddCell("Arrival time : \t" + actBooking.TimeSlot.ToString());
            table1.AddCell("Number Of People : \t" + actBooking.numOfPeople);
            table1.AddCell("\n");
            table1.AddCell("Basic Cost: \t" + actBooking.BasicPrice.ToString("C"));

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