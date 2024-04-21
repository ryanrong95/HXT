using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Underly;

namespace Yahv.CrmPlus.Service.Views.Origins
{
    public class FixedSuppliersOrigin : Yahv.Linq.UniqueView<FixedSupplier, PvdCrmReponsitory>
    {
        internal FixedSuppliersOrigin()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal FixedSuppliersOrigin(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<FixedSupplier> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.FixedSuppliers>()
                   select new FixedSupplier {
                       ID = entity.ID,
                       CutoffTime = entity.CutoffTime,
                       DeliveryPlace = entity.DeliveryPlace,
                       DeliveryTime = entity.DeliveryTime,
                       QuoteMethod = (QuoteMethod)entity.QuoteMethod,
                       BatchMethod =entity.BatchMethod,
                       FreightPayer = (FreightPayer)entity.FreightPayer,
                       Mop = entity.Mop,
                       CompanyID = entity.CompanyID,
                       IsDelegatePay =entity.IsDelegatePay,
                       IsNotcieShiped = entity.IsNotcieShiped,
                       IsOriginPi = entity.IsOriginPi,
                       IsWaybillPi = entity.IsWaybillPi,
                       CreateDate = entity.CreateDate,
                       CreatorID = entity.CreatorID,
                       WaybillFrom=entity.WaybillFrom
                   };
        }
    }
}
