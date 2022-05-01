using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NguniDemo.Models
{
    public class TableType
    {
        [Key]
        public int TabletypeId { get; set; }

        public string Name { get; set; }
        [DisplayName("Tables Available")]
        [Range(0, 300)]
        public Nullable<int> TableAvailable { get; set; }
        public string Description { get; set; }
    }
}