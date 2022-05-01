using NguniDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NguniDemo.ViewModels
{
    public class TimeVenueVM
    {
        public IEnumerable<VenueTimes> TimeVenue { get; set; }
        public string SearchTerm { get; set; }
        public Pager Pager { get; set; }
    }
}