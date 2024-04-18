using Needs.Ccs.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services
{
    public static class ProductClassifyChangeLogExtends
    {
        /// <summary>
        /// 写入产品归类变更日志
        /// </summary>
        /// <param name="classifyProduct"></param>
        /// <param name="summary"></param>
        public static void Log(this Models.ClassifyProduct classifyProduct, string summary)
        {
            ProductClassifyChangeLog log = new ProductClassifyChangeLog();
            log.Model = classifyProduct.Model;
            log.Manufacturer = classifyProduct.Manufacturer;
            log.Declarant = classifyProduct.Admin;
            log.Summary = summary;
            log.Enter();
        }

        /// <summary>
        /// 写入产品预归类变更日志
        /// </summary>
        /// <param name="classifyProduct"></param>
        /// <param name="summary"></param>
        public static void Log(this Models.PreClassifyProduct classifyProduct, string summary)
        {
            ProductClassifyChangeLog log = new ProductClassifyChangeLog();
            log.Model = classifyProduct.Model;
            log.Manufacturer = classifyProduct.Manufacturer;
            log.Declarant = classifyProduct.Admin;
            log.Summary = summary;
            log.Enter();
        }
    }
}
