using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Models
{
    class Customs : Yahv.Settings.JsonManager
    {
        public bool IsCheckUser { get; set; } = false;
    }
}
