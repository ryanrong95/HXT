using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Services.Enums;

namespace Yahv.Services.Models
{
    public class Form
    {
        public string ID { get; set; }
        public string StorageID { get; set; }
        public decimal Quantity { get; set; }
        public string NoticeID { get; set; }
        public FormStatus Status { get; set; }
    }
}
