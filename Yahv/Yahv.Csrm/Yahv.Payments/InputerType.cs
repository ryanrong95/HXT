using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.Payments
{
    //public enum MasterType
    //{
    //    OrderID,
    //    Waybill
    //}

    public enum InputerType
    {
        Erp,
        Site
    }

    /// <summary>
    /// 操作人
    /// </summary>
    public struct Inputer
    {
        public string ID;
        public InputerType Type;

        public Inputer(string id, InputerType type)
        {
            this.ID = id;
            this.Type = type;
        }
    }
}
