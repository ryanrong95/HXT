using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Services;
using Yahv.Services.Enums;
using Yahv.Underly.Erps;

namespace Yahv.Models
{
    public class CenterLog : OperatingLogger
    {
        IErpAdmin admin;
        internal CenterLog(IErpAdmin admin) : base(admin.ID)
        {
            this.admin = admin;
        }
    }


    //class MyClass
    //{
    //    public MyClass()
    //    {
    //        Erp.Current.OperatingLog[LogType.WsOrder].Log("");

    //        Erp.Current.OperatingLog.Log("", "");
    //    }
    //}
}
