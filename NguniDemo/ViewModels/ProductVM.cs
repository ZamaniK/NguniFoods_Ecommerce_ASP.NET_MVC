using NguniDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NguniDemo.ViewModels
{
    public class ProductVM
    {
        public ProductVM()
        {
        }



        public ProductVM(FoodItem row)
        {
            Id = row.FoodItemID;
            Name = row.FoodItemName;
            ShortDescription = row.ShortDesc;
            LongDescription = row.LongDesc;
            Price = row.Price;
            FoodType = row.FoodType;
            FoodId = row.FoodId;
            ImageName = row.ImageUrl;
        }

        public int Id { get; set; }
        public string Name { get; set; }
       
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }

        public decimal Price { get; set; }
        public string FoodType { get; set; }
        public int FoodId { get; set; }
        public string ImageName { get; set; }

        public IEnumerable<SelectListItem> Foods { get; set; }
    }

    public class ProductListingModel
    {
        public IEnumerable<FoodItem> FoodItems { get; set; }
        public string SearchTerm { get; set; }
        public IEnumerable<Food> FoodTypes { get; set; }
        public int? FoodTypeID { get; set; }
        public Pager Pager { get; set; }

    }
}