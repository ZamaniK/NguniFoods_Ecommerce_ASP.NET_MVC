using NguniDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NguniDemo.ViewModels
{
    public class GradeListingModel
    {
        public IEnumerable<Food> FoodTypes { get; set; }
        public string SearchTerm { get; set; }
        public Pager Pager { get; set; }

    }

    public class GradeActionModel
    {
        public int ID { get; set; }

        public string FoodType { get; set; }

    }
}