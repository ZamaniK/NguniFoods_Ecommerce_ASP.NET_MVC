using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NguniDemo.Models
{
    public class Table
    { 
        [Key]
        public int TableId { get; set; }
        public string TableName { get; set; }

        [DisplayName("Table Type")]
        public int TabletypeId { get; set; }
        [DisplayName("Table Capacity")]
        public int TableCapacity { get; set; }
        [DisplayName("Table Description")]
        public string TableDescription { get; set; }
        public byte[] TablePicture { get; set; }

        [DisplayName("Table Status")]
        public string Status { get; set; }
        public virtual TableType TableTypes { get; set; }
        public virtual List<TablePictures> TablePictures { get; set; }


    }
}