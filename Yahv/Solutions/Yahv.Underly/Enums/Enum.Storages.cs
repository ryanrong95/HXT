using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.Underly
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

    /// <summary>
    /// 库存类型
    /// </summary>
    public enum CgStoragesType
    {
        [Description("流水库")]
        Unknown = 0,

        [Description("流水库")]
        Flows = 200,

        [Description("库存库")]
        Stores = 300,
        
        [Description("运营库")]
        Operatings = 400,
        
        //[Description("报关库")] // 不再使用报关库
        //Customs = 500,
        
        [Description("报废库")]
        Trashs = 600,
        
        [Description("检测库")]
        Testing = 700,
        
        [Description("暂存库")]
        Staging = 800,
        
        [Description("异常库")]
        Abnormal = 810,
        
        [Description("退货库")]
        Returns = 900,
    }
}
