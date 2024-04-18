using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.PsWms.PdaApi.Services.Attributes;

namespace Yahv.PsWms.PdaApi.Services.Models
{
    [Table(name: "#tempStorages", target: "Storages")]
    public class TempStorage
    {
        [Column(name: "ID", definition: "varchar(50) not null", target: "ID")]
        public string ID { get; set; }

        [Column(name: "PackageNumber", definition: "int not null", target: "PackageNumber")]
        public int PackageNumber { get; set; }

        [Column(name: "Total", definition: "int not null", target: "Total")]
        public int Total { get; set; }

        [Column(name: "ModifyDate", definition: "datetime not null", target: "ModifyDate")]
        public DateTime ModifyDate { get; set; }
    }
}
