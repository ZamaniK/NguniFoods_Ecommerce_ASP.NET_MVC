using NguniDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NguniDemo.ViewModels
{
    public class VenueListingModel
    {
        public IEnumerable<VenueTime> Venues { get; set; }
        public string SearchTerm { get; set; }
        public IEnumerable<VenueTime> Time { get; set; }
        public int? TimeID { get; set; }
        public int? VenueID { get; set; }


        public Pager Pager { get; set; }
    }


}