using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NguniDemo.Models
{
    public class Venue
    {
        [Key]
        public int VenueId { get; set; }
        [Required, DisplayName("Venue Type")]
        public string VenueName { get; set; }
        [Required, DisplayName("Description")]
        public string Description { get; set; }
        [DataType(DataType.Currency), DisplayName("Venue Price")]
        public decimal VenuePrice { get; set; }
        public int Capacity { get; set; }
    }
}