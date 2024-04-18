using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Yahv.PvWsOrder.Services.Models;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PvOms.WebApp.LsOrders
{
    public partial class Add : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected object data()
        {
            var query = Erp.Current.WsOrder.LsProduct.Where(t => t.Quantity > 0).ToList();
            var linq = query.Select(t => new
            {
                ID = t.ID,
                Name = t.Name,
                SpecID = t.SpecID,
                Load = t.Load,
                Volume = t.Volume,
                Quantity = t.Quantity,
                ApplyQty = 0,
                UnitPrice = 0,
                TotalPrice = 0,
            });
            return linq;
        }

        protected void Calculate()
        {
            try
            {
                int Month = int.Parse(Request.Form["Month"]);
                var linq = Erp.Current.WsOrder.LsProductPrice;
                var ids = linq.Select(item => item.ProductID).Distinct();
                List<Services.Models.LsOrder.LsProductPrices> list = new List<Services.Models.LsOrder.LsProductPrices>();
                foreach (var id in ids)
                {
                    var product = linq.Where(item => item.ProductID == id && item.Month <= Month).OrderByDescending(item => item.Month).FirstOrDefault();
                    if (product != null)
                    {
                        list.Add(product);
                    }
                }

                var data = list.Select(item => new
                {
                    ProductID = item.ProductID,
                    Month = item.Month,
                    Price = item.Price,
                });
                Response.Write((new { success = true, data = data }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, data = ex }).Json());
            }
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
                string clientID = Request.Form["clientID"];
                //主体   TODO：hard code 主体 深圳市芯达通供应链管理有限公司
                string company = Erp.Current.Leagues.Current?.EnterpriseID ?? "DBAEAB43B47EB4299DD1D62F764E6B6A";
                string beneficiary = "";
                string startDate = Request.Form["startDate"];
                string month = Request.Form["month"];
                //产品信息
                var products = Request.Form["products"].Replace("&quot;", "'").Replace("amp;", "");
                var productList = products.JsonTo<List<dynamic>>();
                #endregion

                #region 客户发票
                var invoice = Erp.Current.WsOrder.Invoices.Where(item => item.EnterpriseID == clientID).FirstOrDefault();//目前只有一个发票
                #endregion

                LsOrder order = new LsOrder();
                order.Type = LsOrderType.Lease;
                order.Source = LsOrderSource.WarehouseServicing;
                order.ClientID = clientID;
                order.PayeeID = company;
                order.BeneficiaryID = beneficiary;
                order.Currency = Currency.CNY;
                order.InvoiceID = invoice?.ID;
                order.Creator = order.OperatorID = Erp.Current.ID;
                order.StartDate = Convert.ToDateTime(startDate);
                order.EndDate = order.StartDate?.AddMonths(int.Parse(month));
                order.OrderItems = productList.Select(item => new Services.Models.LsOrder.LsOrderItem
                {
                    Quantity = item.ApplyQty,
                    Currency = Currency.CNY,
                    UnitPrice = item.UnitPrice,
                    ProductID = item.ID,
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