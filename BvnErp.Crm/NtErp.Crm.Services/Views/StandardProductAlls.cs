using Layer.Data.Sqls;
using Needs.Erp.Generic;
using Needs.Linq;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Views
{
    public class StandardProductAlls : UniqueView<StandardProduct, BvCrmReponsitory>, Needs.Underly.IFkoView<StandardProduct>
    {
        /// <summary>
        /// 无参构造函数
        /// </summary>
        public StandardProductAlls()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库实体</param>
        internal StandardProductAlls(BvCrmReponsitory reponsitory) : base(reponsitory)
        {
        }


        /// <summary>
        /// 获取标准产品数据集合
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<StandardProduct> GetIQueryable()
        {
            CompanyAlls companyall = new CompanyAlls(this.Reponsitory);//公司视图

            return from product in base.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.StandardProducts>()
                   join manufacture in companyall on product.ManufacturerID equals manufacture.ID
                   select new StandardProduct
                   {
                       CreateDate = product.CreateDate,
                       ID = product.ID,
                       Name = product.Name,
                       Origin = product.Origin,
                       ManufacturerID = product.ManufacturerID,
                       Manufacturer = manufacture,
                       Packaging = product.Packaging,
                       PackageCase = product.PackageCase,
                       Batch = product.Batch,
                       DateCode = product.DateCode,
                   };
        }
    }
}
