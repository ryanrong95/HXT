using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.Services.Views
{
    /// <summary>
    /// 财务实际收付款统计视图
    /// </summary>
    /// <typeparam name="TReponsitory"></typeparam>
    public class VouchersTopView<TReponsitory> : QueryView<Voucher, TReponsitory>
         where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public VouchersTopView()
        {

        }

        public VouchersTopView(TReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Voucher> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.VouchersTopView>()
                   select new Voucher()
                   {
                       ID = entity.ID,
                       OrderID = entity.OrderID,
                       Type = (VoucherType)entity.Type,
                       Payer = entity.Payer,
                       Payee = entity.Payee,
                       Currency = (Currency)entity.Currency,
                       CreatorID = entity.CreatorID,
                       CreateDate = entity.CreateDate,
                       Status = (VoucherStatus)entity.Status,
                       ApplicationID = entity.ApplicationID,
                       IsSettlement = entity.IsSettlement ?? false,
                       DateIndex = entity.DateIndex,
                   };
        }
    }
}
