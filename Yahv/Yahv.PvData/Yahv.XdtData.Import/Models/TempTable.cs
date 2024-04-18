using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.XdtData.Import.Attributes;

namespace Yahv.XdtData.Import.Models
{
    [Table(name: "#tmpInputs", target: "Inputs")]
    public class TmpInput
    {
        [Column(name: "ID", definition: "varchar(50) not null", target: "ID")]
        public string ID { get; set; }

        [Column(name: "Currency", definition: "int not null", target: "Currency")]
        public int Currency { get; set; }

        [Column(name: "UnitPrice", definition: "decimal(18,7) not null", target: "UnitPrice")]
        public decimal UnitPrice { get; set; }
    }

    [Table(name: "#tmpOutputs", target: "Outputs")]
    public class TmpOutput
    {
        [Column(name: "ID", definition: "varchar(50) not null", target: "ID")]
        public string ID { get; set; }

        [Column(name: "Currency", definition: "int not null", target: "Currency")]
        public int Currency { get; set; }

        [Column(name: "Price", definition: "decimal(18,7) not null", target: "Price")]
        public decimal Price { get; set; }
    }
}
