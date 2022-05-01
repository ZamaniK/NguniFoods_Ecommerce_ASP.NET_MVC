using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NguniDemo.Models
{
    public class VenueTime
    {
        [Key]
        public int VenueTimeId { get; set; }
        public int VenueId { get; set; }
        public virtual Venue Venues { get; set; }
        [Display(Name = "Time Slot")]
        public int VenueTimesId { get; set; }
        public virtual VenueTimes VenueTimes { get; set; }
    }
}