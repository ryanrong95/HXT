using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;

namespace Yahv.Payments.Views
{
    /// <summary>
    /// 应付视图
    /// </summary>
    public class PayablesView : Yahv.Linq.QueryView<Payable, PvbCrmReponsitory>
    {
        public PayablesView()
        {

        }

        public PayablesView(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Payable> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Payables>()
                   select new Payable
                   {
                       ID = entity.ID,
                       Payer = entity.Payer,
                       Payee = entity.Payee,
                       PayeeID = entity.PayeeID,
                       Business = entity.Business,
                       Subject = entity.Subject,
                       Currency = (Underly.Currency)entity.Currency,
                       Price = entity.Price,
                       OrderID = entity.OrderID,
                       CreateDate = entity.CreateDate,
                       Summay = entity.Summay,
                       AdminID = entity.AdminID,
                       PayerID = entity.PayerID,
                       WaybillID = entity.WaybillID,
                       Catalog = entity.Catalog,
                       ApplicationID = entity.ApplicationID,
                       VoucherID = entity.VoucherID,
                   };
        }

        public void Add(Payable entity)
        {
            this.Reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.Payables
            {
                AdminID = entity.AdminID,
                Currency = (int)entity.Currency,
                PayerID = entity.PayerID,
                ID = entity.ID,
                WaybillID = entity.WaybillID,
                Business = entity.Business,
                CreateDate = entity.CreateDate,
                OrderID = entity.OrderID,
                Payee = entity.Payee,
                PayeeID = entity.PayeeID,
                Payer = entity.Payer,
                Price = entity.Price,
                Subject = entity.Subject,
                Summay = entity.Summay,
                Catalog = entity.Catalog,
                VoucherID = entity.VoucherID,
            });
        }

        public Payable this[string id]
        {
            get { return this.SingleOrDefault(item => item.ID == id); }
        }

        /// <summary>
        /// 绑定VoucherID
        /// </summary>
        public void BindVoucherID(string[] payableIds, string voucherId)
        {
            foreach (var entity in this.Where(item => payableIds.Contains(item.ID)))
            {
                this.Reponsitory.Update<Layers.Data.Sqls.PvbCrm.Payables>(new
                {
                    VoucherID = voucherId
                }, item => item.ID == entity.ID);
            }

        }
    }
}
