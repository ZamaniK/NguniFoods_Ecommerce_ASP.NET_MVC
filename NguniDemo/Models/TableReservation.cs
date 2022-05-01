using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NguniDemo.Models
{
    public class TableReservation
    {
        [Key]
        public int TableBookingId { get; set; }
        [DisplayName("Table Type")]
        public string TableType { get; set; }
        public int TableId { get; set; }
        [DisplayName("Customer Email")]
        public string CustomerEmail { get; set; }

        public decimal Total { get; set; }
        [DisplayName("Check-In-Time"), DataType(DataType.Time)]
        public DateTime CheckInTime { get; set; }
        [DisplayName("Check-Out-Time"), DataType(DataType.Time)]
        public DateTime CheckOutTime { get; set; }
        [DisplayName("Number Of Hours")]
        public int NumberOfHours { get; set; }
        [DisplayName("Number Of People")]
        public int NumberOfPeople { get; set; }
        public string Status { get; set; }
        public virtual Table Table { get; set; }
    }
}