using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Origins
{
    public class AutoQuotesOrign : Yahv.Linq.UniqueView<AutoQuote, PvbCrmReponsitory>
    {
        /// <summary>
        /// 默认构造器
        /// </summary>
        internal AutoQuotesOrign()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal AutoQuotesOrign(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<AutoQuote> GetIQueryable()
        {
            //var adminsView = new Origins.AdminsOrigin(this.Reponsitory);
            return from entity in Reponsitory.GetTable<Layers.Data.Sqls.PvbCrm.AutoQuotes>()
                   //join admin in adminsView on entity.ReporterID equals admin.ID
                   select new AutoQuote
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       SupplierID = entity.SupplierID,
                       Supplier = entity.Supplier,
                       Manufacturer = entity.Manufacturer,
                       DateCode = entity.DateCode,
                       PackageCase = entity.PackageCase,
                       Packaging = entity.Packaging,
                       Prices = entity.Prices,
                       UnitPrice = entity.UnitPrice,
                       Quantity = entity.Quantity,
                       ReporterID = entity.ReporterID,
                       Deadline = entity.Deadline,
                       CreateDate = entity.CreateDate,
                       //Admin = admin
                   };
        }
    }
}
