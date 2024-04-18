using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.PvWsOrder.Services.Models;
using Yahv.PvWsOrder.Services.Views;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PvOms.WebApp.LsOrders
{
    public partial class ReNew: ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }
        protected void LoadData()
        {
            //续租订单开始日期
            var orderid = Request.QueryString["ID"];
            var enddate = new LsOrderItemRoll(orderid).AsEnumerable().FirstOrDefault().Lease.EndDate;
            this.Model.StartDate = enddate.AddDays(1);
        }
        protected object data()
        {
            var orderid = Request.QueryString["ID"];
            //订单项
            var query = new LsOrderItemRoll(orderid).AsEnumerable();
            var linq = query.Select(t => new
            {
                ProductID = t.ProductID,
                Name = t.Product.Name,
                SpecID = t.Product.SpecID,
                StartDate = t.Lease.StartDate.ToShortDateString(),
                EndDate = t.Lease.EndDate.ToShortDateString(),
                Quantity = t.Quantity,
                UnitPrice = t.UnitPrice,
                TotalPrice = t.UnitPrice * t.Quantity,
            });
            return linq;
        }

        /// <summary>
        /// 提交订单
        /// </summary>
        protected void SubmitOrder()
        {
            try
            {
                #region 界面数据
                //基本信息
                string clientID = Request.Form["clientId"];
                string company = Erp.Current.Leagues.Current?.EnterpriseID ?? "DBAEAB43B47EB4299DD1D62F764E6B6A";
                string beneficiary = "";
                string startDate = Request.Form["startDate"];
                string month = Request.Form["month"];
                string fatherid = Request.Form["fatherid"];
                //产品信息
                var products = Request.Form["products"].Replace("&quot;", "'").Replace("amp;", "");
                var productList = products.JsonTo<List<dynamic>>();
                #endregion

                #region 客户发票
                var invoice = Erp.Current.WsOrder.Invoices.Where(item => item.EnterpriseID == clientID).FirstOrDefault();//目前只有一个发票
                #endregion

                //续租订单
                LsOrder order = new LsOrder();
                order.Type = LsOrderType.Lease;
                order.Source = LsOrderSource.WarehouseServicing;
                order.ClientID = clientID;
                order.FatherID = fatherid;
                order.PayeeID = company;
                order.BeneficiaryID = beneficiary;
                order.Currency = Currency.CNY;
                order.InvoiceID = invoice?.ID;
                order.Creator = order.OperatorID = Erp.Current.ID;
                order.StartDate = Convert.ToDateTime(startDate);
                order.EndDate = order.StartDate?.AddMonths(int.Parse(month));
                order.OrderItems = productList.Select(item => new Services.Models.LsOrder.LsOrderItem
                {
                    Quantity = item.Quantity,
                    Currency = Currency.CNY,
                    UnitPrice = item.UnitPrice,
                    ProductID = item.ProductID,
                    CreateDate = DateTime.Now,
                    Lease = new Services.Models.LsOrder.OrderItemsLease
                    {
                        StartDate = (DateTime)order.StartDate,
                        EndDate = (DateTime)order.EndDate,
                        CreateDate = DateTime.Now,
                        ModifyDate = DateTime.Now,
                        Status = LsStatus.Subsist,
                    }
                }).ToArray();
                
                //续租订单不需扣库存，但要记账
                order.Enter();

                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            }
        }
    }
}