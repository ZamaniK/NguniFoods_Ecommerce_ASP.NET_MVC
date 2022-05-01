using NguniDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NguniDemo.ViewModels
{
    public class TablesDashboardVM
    {
        public TableType TableTypes { get; set; }
        public IEnumerable<Table> Tables { get; set; }
        public int SelectedTableID { get; set; }
    }

    public class TablesDetailsViewModel
    {
        public Table Tables { get; set; }
    }
}