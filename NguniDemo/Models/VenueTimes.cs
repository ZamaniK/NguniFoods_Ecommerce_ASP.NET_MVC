using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NguniDemo.Models
{
    public class VenueTimes
    {
        [Key]
        public int VenueTimesId { get; set; }
        [DisplayName("Time"), DataType(DataType.Time)]
        public DateTime SlotTime { get; set; }
    }
}