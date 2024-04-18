using Layer.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class StoreReceiptReceivedClientView
    {
        private ScCustomsReponsitory Reponsitory { get; set; }

        public StoreReceiptReceivedClientView()
        {
            this.Reponsitory = new ScCustomsReponsitory();
        }

        public StoreReceiptReceivedClientView(ScCustomsReponsitory reponsitory)
        {
            this.Reponsitory = reponsitory;
        }

        private IQueryable<StoreReceiptReceivedClientViewModel> GetAll(string clientName)
        {
            var receivedsStatisticsView = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.ReceivedsStatisticsView>();
            var adminsTopView2 = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.AdminsTopView2>();

            var results = from received in receivedsStatisticsView
                          join admin in adminsTopView2 on received.AdminID equals admin.ID into adminsTopView2T
                          from admin in adminsTopView2T.DefaultIfEmpty()
                          where received.Business == "代仓储"
                             && received.PayerName == clientName
                          orderby received.CreateDate descending
                          select new StoreReceiptReceivedClientViewModel
                          {
                              CreateDate = received.CreateDate,
                              AdminID = received.AdminID,
                              RealName = admin.RealName,
                              OrderID = received.OrderID,
                              Catalog = received.Catalog,
                              Subject = received.Subject,
                              Price = received.Price,
                              AccountType = received.AccountType,
                          };

            return results;
        }

        public List<StoreReceiptReceivedClientViewModel> GetResult(out int totalCount, int pageIndex, int pageSize, string clientName)
        {
            var alls = GetAll(clientName);

            totalCount = alls.Count();

            var pagedLists = alls.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();

            return pagedLists.ToList();
        }



    }

    public class StoreReceiptReceivedClientViewModel
    {
        public DateTime CreateDate { get; set; }
        public string AdminID { get; set; }
        public string RealName { get; set; }
        public string OrderID { get; set; }
        public string Catalog { get; set; }
        public string Subject { get; set; }
        public decimal Price { get; set; }
        public int? AccountType { get; set; }
    }
}
