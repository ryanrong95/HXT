using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Linq.Extends;
using Yahv.PvWsOrder.Services.Models;
using Yahv.PvWsOrder.Services.Views;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PvOms.WebApp.Clients
{
    public partial class BillAdd : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        public void LoadData()
        {
            this.Model.ClientData = Erp.Current.WsOrder.MyWsClients.Select(t => new
            {
                value = t.Name,
                text = t.Name,
            });

            this.Model.Currency = ExtendsEnum.ToArray<Currency>().Select(item => new
            {
                value = (int)item,
                text = item.GetDescription()
            }).Where(t => t.value == 1 || t.value == 3);
        }

        protected object data()
        {
            Expression<Func<Order_Show_AddBill, bool>> expression = Predicate();

            var enterpriseid = Erp.Current.Leagues?.Current?.EnterpriseID;
            var query = Erp.Current.WsOrder.MyCnyOrders(enterpriseid).Where(expression).ToArray();

            return new
            {
                rows = query.Select(item => new
                {
                    item.ID,
                    ClientID = item.ClientID,
                    Type = item.Type.GetDescription(),
                    ClientName = item.ClientName,
                    ClientCode = item.EnterCode,
                    Currency = Currency.CNY.GetDescription(),
                    Price = item.TotalPrice,
                    MainStatus = item.MainStatus.GetDescription(),
                    PaymentStatus = item.PaymentStatus.GetDescription(),
                    CreateDate = item.CreateDate.ToString("yyyy-MM-dd"),
                }).ToArray(),
                total = query.Count(),
            }.Json();
        }

        Expression<Func<Order_Show_AddBill, bool>> Predicate()
        {
            Expression<Func<Order_Show_AddBill, bool>> predicate = item => item.Type != OrderType.Declare && item.Type != OrderType.TransferDeclare
            && item.PaymentStatus != OrderPaymentStatus.Waiting && item.PaymentStatus != OrderPaymentStatus.Confirm;

            //查询参数
            var companyName = Request.QueryString["ClientName"];
            var clientCode = Request.QueryString["ClientCode"];

            var billItems = new PvWsOrder.Services.Views.Alls.BillItemsAll().Select(t => t.OrderID).ToArray();
            if (billItems.Count() > 0)
            {
                predicate = predicate.And(item => !billItems.Contains(item.ID));
            }
            if (!string.IsNullOrWhiteSpace(companyName))
            {
                companyName = companyName.Trim();
                predicate = predicate.And(item => item.ClientName.Contains(companyName));
            }
            if (!string.IsNullOrWhiteSpace(clientCode))
            {
                clientCode = clientCode.Trim();
                predicate = predicate.And(item => item.EnterCode.Contains(clientCode));
            }
            return predicate;
        }

        /// <summary>
        /// 新增账单
        /// </summary>
        protected void Submit()
        {
            try
            {
                string orders = Request.Form["orders"];
                string currency = Request.Form["currency"];
                string clientID = Request.Form["clientID"];

                var bill = new Bill();
                bill.ClientID = clientID;
                bill.AdminID = Erp.Current.ID;
                bill.Currency = (Currency)Enum.Parse(typeof(Currency), currency);
                bill.Enter();

                string[] orderids = orders.Split(',');
                foreach (var orderid in orderids)
                {
                    var billItem = new BillItem();
                    billItem.BillID = bill.ID;
                    billItem.OrderID = orderid;
                    billItem.Enter();
                }

                Response.Write((new { success = true, message = "新增成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "新增失败：" + ex.Message }).Json());
            }
        }
    }
}