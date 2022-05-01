using NguniDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NguniDemo.ViewModels
{
    public class TableTypeListingModel
    {
        public IEnumerable<TableType> TableTypes { get; set; }
        public string SearchTerm { get; set; }
        public Pager Pager { get; set; }

    }

    public class TableTypeActionModel
    {
        public int ID { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }


    }

    public class TableListingModel
    {
        public IEnumerable<Table> Tables { get; set; }
        public string SearchTerm { get; set; }
        public IEnumerable<TableType> TableTypes { get; set; }
        public int? TableTypeID { get; set; }

        public Pager Pager { get; set; }
    }

    public class TableActionModel
    {
        public int ID { get; set; }

        public int TableTypeID { get; set; }
        public TableType TableType { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public int NoOfSeats { get; set; } 
        public string PictureIDs { get; set; }

        public IEnumerable<TableType> TableTypes { get; set; }
        public List<TablePictures> TablePictures { get; set; }
    }


}