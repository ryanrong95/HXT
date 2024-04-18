using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layer.Data.Sqls;
using Needs.Linq;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Models;

namespace NtErp.Crm.Services.Views
{
    public class CompeteProductAlls : UniqueView<CompeteProduct,BvCrmReponsitory>, Needs.Underly.IFkoView<CompeteProduct>
    {
        /// <summary>
        /// 无参构造函数
        /// </summary>
        public CompeteProductAlls()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库实体</param>
        internal CompeteProductAlls(BvCrmReponsitory reponsitory) : base(reponsitory)
        {

        }

        /// <summary>
        /// 获取竞争产品数据集合
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<CompeteProduct> GetIQueryable()
        {
            return from product in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.CompeteProducts>()
                   select new CompeteProduct
                   {
                       ID = product.ID,
                       Name = product.Name,
                       Origin = product.Origin,
                       ManufacturerID = product.ManufacturerID,
                       Packaging = product.Packaging,
                       PackageCase = product.PackageCase,
                       Batch = product.Batch,
                       DateCode = product.DateCode,
                       CreateDate = product.CreateDate,
                       Currency = (Needs.Underly.Currency?)product.Currency,
                       UnitPrice = (decimal?)product.UnitPrice,
                       OriginNumber = product.OriginNumber,
                   };
        }
    }
}
