using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Temp
{
    public partial class DataImport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ClassifyProduct[] cps = { new ClassifyProduct { ID = "XDTOrderItem202009260000000058" },
            new ClassifyProduct { ID = "XDTOrderItem202009260000000095" },
            new ClassifyProduct { ID = "XDTOrderItem202009260000000097" },
            new ClassifyProduct { ID = "XDTOrderItem202009260000000098" },
            new ClassifyProduct { ID = "XDTOrderItem202009260000000099" },
            new ClassifyProduct { ID = "XDTOrderItem202009260000000100" }};
            SyncManager.Current.ClassifyCompensation.For(cps).DoSync();

            //PayExchangeDataSync();
            //ReceivableDataSync();
        }

        void PayExchangeDataSync()
        {
            DateTime dueDate = DateTime.Parse($"{2020}-{8}-{1}").Date;
            var applies = new AdminPayExchangeApplyView()
                .Where(item => item.CreateDate >= dueDate && item.PayExchangeApplyStatus != PayExchangeApplyStatus.Auditing && item.PayExchangeApplyStatus != PayExchangeApplyStatus.Cancled)
                .Select(item => new { item.ID, item.Client }).ToArray();
            foreach (var apply in applies)
            {
                var admin = apply.Client.Merchandiser;
                var ermAdmin = new AdminsTopView2().FirstOrDefault(item => item.OriginID == admin.ID);
                var toYahv = new PayExchangeToYahvReceivable_Temp(apply.ID, ermAdmin);
                toYahv.Execute();
            }
        }

        void ReceivableDataSync()
        {
            DateTime dueDate = DateTime.Parse($"{2020}-{6}-{6}").Date;
            var orders = new OrdersViewBase<Order_Temp>()
                .Where(item => item.CreateDate >= dueDate)
                .Select(item => new Order_Temp()
                {
                    ID = item.ID,
                    Type = item.Type,
                    Client = item.Client,
                    ClientAgreement = item.ClientAgreement,
                    Currency = item.Currency,
                    CustomsExchangeRate = item.CustomsExchangeRate,
                    RealExchangeRate = item.RealExchangeRate,
                    DeclarePrice = item.DeclarePrice,
                    CreateDate = item.CreateDate,
                    UpdateDate = item.UpdateDate,
                    MainOrderID = item.MainOrderID,
                    OrderBillType = item.OrderBillType,
                }).ToArray();

            string[] excludeOrderIDs = { "WL86620200607002-01", "WL17120200610002-01", "XL00220200619501-01" };
            string[] excludeMainIDs = { "WL86620200609001", "WL86620200609002", "WL86620200609002", "WL86620200609008", "WL86620200609010", "WL53820200616002" };

            orders = orders.Where(item => !excludeOrderIDs.Contains(item.ID) && !excludeMainIDs.Contains(item.MainOrderID)).ToArray();

            int count = 0;
            foreach (var order in orders)
            {
                string id = order.ID;
                order.GenerateBill();
                count++;
            }
        }
    }
}