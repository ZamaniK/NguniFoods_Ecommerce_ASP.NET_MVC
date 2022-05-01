using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace NguniDemo.Models
{
    public class VenueBooking
    {
        [Key]
        public int VenueBookingId { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Arrival Date")]
        public DateTime? ArrivalDate { get; set; }
        [Display(Name = "No. People")]
        public int numOfPeople { get; set; }
        [Display(Name = "Basic Price")]
        [DataType(DataType.Currency)]
        public decimal BasicPrice { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerName { get; set; }
        public string VenueName { get; set; }

        public string CustomerLastName { get; set; }
        public string Status { get; set; }
        public string VenueType { get; set; }
        [DisplayName("Time"), DataType(DataType.Time)]
        public DateTime TimeSlot { get; set; }
        public int VenueTimeId { get; set; }
        public virtual VenueTime VenueTime { get; set; }
        ApplicationDbContext context = new ApplicationDbContext();

        //Activity Price
        public decimal getVenuePrice()
        {
            var id = (from a in context.VenueTimes1
                      where a.VenueTimeId == VenueTimeId
                      select a.VenueId).FirstOrDefault();

            var Price = (from a in context.Venues
                         where a.VenueId == id
                         select a.VenuePrice).FirstOrDefault();
            return (Price);
        }

        public string  getVenueName()
        {
            var id = (from a in context.VenueTimes1
                      where a.VenueTimeId == VenueTimeId
                      select a.VenueId).FirstOrDefault();

            var Name = (from a in context.Venues
                         where a.VenueId == id
                         select a.VenueName).FirstOrDefault();
            return (Name);
        }
        //Activity Type
        public string getVenueType()
        {
            var id = (from a in context.VenueTimes1
                      where a.VenueTimeId == VenueTimeId
                      select a.VenueId).FirstOrDefault();

            var Atype = (from a in context.Venues
                         where a.VenueId == id
                         select a.VenueName).FirstOrDefault();
            return (Atype);
        }
        //Time Slot
        public DateTime getTimeSlot()
        {
            var id = (from a in context.VenueTimes1
                      where a.VenueTimeId == VenueTimeId
                      select a.VenueTimesId).FirstOrDefault();

            var time = (from a in context.VenueTimes
                        where a.VenueTimesId == id
                        select a.SlotTime).FirstOrDefault();
            return time;
        }
        //Customer Name

        public string getCustomerName(string CusEmail)
        {
            var Fname = (from c in context.Users
                         where c.Email == CusEmail
                         select c.Email).FirstOrDefault();
            return (Fname);
        }
       

        public bool CheckBooking(DateTime datet)
        {
            bool result = false;
            //var datee = context.ActivityBookings.Where(x => x.ArrivalDate == ArrivalDate).Count();
            var datee = context.VenueBookings.Where(x => x.TimeSlot == datet).Count();
            if (datee > 1)
            {
                result = true;
            }
            return result;
        }
        public bool CheckBooking(VenueBooking booking)
        {
            bool result = false;
            var dbRecords = context.VenueBookings.Where(x => x.ArrivalDate == booking.ArrivalDate).ToList();
            foreach (var item in dbRecords)
            {
                if (booking.VenueTimeId == booking.VenueTimeId)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        public static void SendEmail(VenueBooking booking)
        {
            var mailTo = new List<MailAddress>();
            mailTo.Add(new MailAddress(booking.CustomerEmail));

            var body = $"Good Day {booking.CustomerName}, " + "<br/>" +
                $"You booked: {booking.VenueType}" + "<br/>" +
                $"Date: {booking.ArrivalDate}" + "<br/>" +
                $"Time: {booking.TimeSlot}" + "<br/>" +
                $"Cost: {booking.BasicPrice}";

            NguniDemo.Models.EmailService emailService = new NguniDemo.Models.EmailService();
            emailService.SendEmail(new EmailContent()
            {
                mailTo = mailTo,
                mailCc = new List<MailAddress>(),
                mailSubject = "Booking Confirmation!!  | Ref No.:" + booking.VenueBookingId,
                mailBody = body,
                mailFooter = $"<br/> Kind Regards, <br/> <b>Nguni Foods  </b>",
                mailPriority = MailPriority.High,
                mailAttachments = new List<Attachment>()

            });
        }
    }
}