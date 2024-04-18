using Layer.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class StoreReceiptReceivedReceivableIDView
    {
        private ScCustomsReponsitory Reponsitory { get; set; }

        public StoreReceiptReceivedReceivableIDView()
        {
            this.Reponsitory = new ScCustomsReponsitory();
        }

        public StoreReceiptReceivedReceivableIDView(ScCustomsReponsitory reponsitory)
        {
            this.Reponsitory = reponsitory;
        }

        public List<StoreReceiptReceivedReceivableIDViewModel> GetList(string receivableID)
        {
            var receivedsStatisticsView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ReceivedsStatisticsView>();
            var adminsTopView2 = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView2>();

            var results = from received in receivedsStatisticsView
                          join admin in adminsTopView2 on received.AdminID equals admin.ID into adminsTopView2T
                          from admin in adminsTopView2T.DefaultIfEmpty()
                          where received.ReceivableID == receivableID
                          orderby received.CreateDate descending
                          select new StoreReceiptReceivedReceivableIDViewModel
                          {
                              ReceivedID = received.ReceivedID,
                              CreateDate = received.CreateDate,
                              AdminID = received.AdminID,
                              RealName = admin.RealName,
                              Catalog = received.Catalog,
                              Subject = received.Subject,
                              Price = received.Price,
                              AccountType = received.AccountType,
                          };

            return results.ToList();
        }

    }

    public class StoreReceiptReceivedReceivableIDViewModel
    {
        public string ReceivedID { get; set; }

        public DateTime CreateDate { get; set; }

        public string AdminID { get; set; }

        public string RealName { get; set; }

        public string Catalog { get; set; }

        public string Subject { get; set; }

        public decimal Price { get; set; }

        public int? AccountType { get; set; }
    }

}
