using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uploader.Services.Models
{

    public class FileDescription : FileMessage
    {
        public string ID { get; set; }
        public DateTime CreateDate { get; set; }
        public Yahv.Services.Models.FileDescriptionStatus Status { get; set; }
    }
}
