using NguniDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NguniDemo.ViewModels
{
    public class TablesViewModel
    {

        public IEnumerable<TableType> TableTypes { get; set; }
        public IEnumerable<Table> Tables { get; set; }
    }
}