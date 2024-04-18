using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    /// <summary>
    /// 库存类型
    /// </summary>
    public enum StoragesType
    {
        //[Description("流水库")]
        //FlowStock = 200,
        [Description("库存库")]
        Inventory = 300,
        //[Description("运营库")]
        //OperateStock = 400,
        //[Description("在途库")]
        //WayStock = 500,
        //[Description("报废库")]
        //AbandonmentStock = 600,
        //[Description("检测库")]
        //TestingStock = 700,
        [Description("暂存库")]
        StagingStock = 800,
        [Description("异常库")]
        AbnormalStock = 810
    }
}
