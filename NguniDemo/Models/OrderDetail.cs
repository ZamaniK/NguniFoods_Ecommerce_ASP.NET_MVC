using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NguniDemo.Models
{
    public class OrderDetail
    {
        public virtual int OrderDetailId { get; set; }
        public virtual int Quantity { get; set; }
        public virtual decimal UnitPrice { get; set; }
        public virtual FoodItem FoodItem { get; set; }
        public virtual int FoodItemID { get; set; }
        public virtual string ApplicationUserId { get; set; }
        public virtual Order Order { get; set; }
        public virtual int OrderId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser User { get; set; }
    }
}