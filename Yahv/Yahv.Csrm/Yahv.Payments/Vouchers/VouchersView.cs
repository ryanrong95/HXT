using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbCrm;
using Layers.Linq;
using Yahv.Linq;
using Yahv.Payments.Models.Origins;
using Yahv.Payments.Models.Rolls;
using Yahv.Underly;

namespace Yahv.Payments
{
    /// <summary>
    /// 财务确认单
    /// </summary>
    public class VouchersView : UniqueView<Voucher, PvbCrmReponsitory>
    {
        public VouchersView() { }

        public VouchersView(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Voucher> GetIQueryable()
        {
            return from v in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Vouchers>()
                   select new Voucher()
                   {
                       Currency = (Currency)v.Currency,
                       Payee = v.Payee,
                       Payer = v.Payer,
                       Type = (VoucherType)v.Type,
                       OrderID = v.OrderID,
                       CreateDate = v.CreateDate,
                       ID = v.ID,
                       CreatorID = v.CreatorID,
                       ApplicationID = v.ApplicationID,
                       IsSettlement = v.IsSettlement.Value,
                       DateIndex = v.DateIndex,
                       Status = (VoucherStatus)v.Status,
                   };
        }
    }
}
