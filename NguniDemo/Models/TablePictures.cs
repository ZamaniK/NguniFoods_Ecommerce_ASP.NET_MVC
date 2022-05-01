using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NguniDemo.Models
{
    public class TablePictures
    {
        public int ID { get; set; }

        public int TableID { get; set; }

        public int PictureID { get; set; }
        public virtual Picture Picture { get; set; }
    }
}