using Needs.Ccs.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services
{
    public static class ProductClassifyLogExtends
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="classifyProduct">归类产品</param>
        /// <param name="logType">日志类型(锁定、归类)</param>
        /// <param name="summary">操作记录</param>
        public static void Log(this Interfaces.IClassifyProduct classifyProduct, Enums.LogTypeEnums logType, string summary)
        {
            ProductClassifyLog log = new ProductClassifyLog();
            log.ClassifyProductID = classifyProduct.ID;
            log.Declarant = classifyProduct.Admin;
            log.LogType = logType;
            log.OperationLog = summary;
            log.Enter();
        }
    }
}
