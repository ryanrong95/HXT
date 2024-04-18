using Layer.Data.Sqls;
using Needs.Linq;
using System.Linq;

namespace Needs.Wl.Models.Views
{
    /// <summary>
    /// 
    /// </summary>
    public class OrderReceiptsView : View<Models.OrderReceivables, ScCustomsReponsitory>
    {
        private string OrderID;

        public OrderReceiptsView(string orderID)
        {
            this.OrderID = orderID;
        }

        internal OrderReceiptsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Models.OrderReceivables> GetIQueryable()
        {
            return from receipt in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderReceipts>()
                   join admin in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView>() on receipt.AdminID equals admin.ID
                   where receipt.Status == (int)Needs.Wl.Models.Enums.Status.Normal && receipt.Type == (int)Needs.Wl.Models.Enums.OrderReceiptType.Receivable
                   && receipt.OrderID == this.OrderID
                   select new Models.OrderReceivables
                   {
                       ID = receipt.ID,
                       ClientID = receipt.ClientID,
                       OrderID = receipt.OrderID,
                       FeeType = (Needs.Wl.Models.Enums.OrderFeeType)receipt.FeeType,
                       Currency = receipt.Currency,
                       Rate = receipt.Rate,
                       Amount = receipt.Amount,
                       Admin = new Needs.Wl.Models.Admin
                       {
                           ID = admin.ID,
                           RealName = admin.RealName,
                           UserName = admin.UserName
                       },
                       Status = receipt.Status,
                       CreateDate = receipt.CreateDate,
                       UpdateDate = receipt.UpdateDate,
                       Summary = receipt.Summary
                   };
        }
    }
}