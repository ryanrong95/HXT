using Needs.Ccs.Services.Hanlders;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.PayExchange.PrepaymentApplyRecord
{
    public partial class Edit : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }
        public void LoadData()
        {
            string payID = Request.QueryString["PaidID"];
            string orderID = Request.QueryString["orderID"];
            string remainingAmount = Request.QueryString["amount"]; //预付金额
            string PaidExchangeAmount = Request.QueryString["PaidExchangeAmount"];//可用金额
            string DeclarePrice = Request.QueryString["DeclarePrice"];//可用金额
            string OldOrderID = Request.QueryString["OldOrderID"];
            
            if (!string.IsNullOrEmpty(payID))
            {
                this.Model.PayID = payID;
            }
            else
            {
                this.Model.PayID = "";
            }
            if (!string.IsNullOrEmpty(orderID))
            {
                this.Model.OrderID = orderID;
            }
            else
            {
                this.Model.OrderID = "";
            }
            if (!string.IsNullOrEmpty(remainingAmount))
            {
                this.Model.RemainingAmount = remainingAmount;
            }
            else
            {
                this.Model.RemainingAmount = "";
            }
            if (!string.IsNullOrEmpty(PaidExchangeAmount))
            {
                this.Model.PaidExchangeAmount = PaidExchangeAmount;
            }
            else
            {
                this.Model.PaidExchangeAmount = "";
            }
            if (!string.IsNullOrEmpty(DeclarePrice))
            {
                this.Model.DeclarePrice = DeclarePrice;
            }
            else
            {
                this.Model.DeclarePrice = "";
            }
            if (!string.IsNullOrEmpty(OldOrderID))
            {
                this.Model.OldOrderID = OldOrderID;
            }
            else
            {
                this.Model.OldOrderID = "";
            }
        }
        /// <summary>
        /// 修改申请预付金额
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Save()
        {
            string payID = Request.Form["PayID"];
            string orderID = Request.Form["orderID"];
            string oldOrderID = Request.Form["OldOrderID"];
            decimal amount = Convert.ToDecimal(Request.Form["Amount"].ToString());
            decimal remainingAmount = Convert.ToDecimal(Request.Form["RemainingAmount"].ToString()); ;//预付金额
            try
            {
                //订单
                var orderItem = new Needs.Ccs.Services.Views.Origins.OrdersOrigin().FirstOrDefault(t => (t.OrderStatus != Needs.Ccs.Services.Enums.OrderStatus.Returned && t.OrderStatus != Needs.Ccs.Services.Enums.OrderStatus.Canceled) && t.ID == orderID);

                //去掉订单状态的验证，此验证让不少订单无法匹配；不知当初为何验证状态
                //if (orderItem == null)
                //{
                //    Response.Write((new { success = false, message = "操作失败：订单状态错误，已确认未报关订单才能匹配" }).Json());
                //    return;
                //}

                //是否已申请付汇(订单编号查询时，存在多笔付汇申请，默认取付汇详细表里的第一笔，按创建时间排序)
                var payOrder = new Needs.Ccs.Services.Views.PrepaymentApplyListView().OrderBy(item => item.CreateDate).FirstOrDefault(item => item.OrderID == orderID && item.ID == payID);

                if (payOrder == null)
                {
                    var payExchangeApplyItem = new Needs.Ccs.Services.Models.PayExchangeApplyItem();
                    payExchangeApplyItem.ID = payID;
                    payExchangeApplyItem.OrderID = orderID;
                    payExchangeApplyItem.NewOrderID = oldOrderID;
                    payExchangeApplyItem.Amount = amount;
                    payExchangeApplyItem.PaidExchangeAmount = orderItem.PaidExchangeAmount;//订单申请预付金额
                    payExchangeApplyItem.UserID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
                    payExchangeApplyItem.RealName = Needs.Wl.Admin.Plat.AdminPlat.Current.RealName;
                    payExchangeApplyItem.NewPayEnter();
                    Response.Write((new { success = true, message = "" }).Json());

                }
                else
                {
                    //付汇申请ID不变，金额修改
                    var payExchangeApplyItem = new Needs.Ccs.Services.Models.PayExchangeApplyItem();
                    payExchangeApplyItem.ID = payOrder.ID;
                    payExchangeApplyItem.Amount = amount;
                    payExchangeApplyItem.PaidExchangeAmount = payOrder.TotalAmount;//申请编号对应订单编号，已申请付汇金额
                    payExchangeApplyItem.OrderID = payOrder.OrderID;
                    payExchangeApplyItem.UserID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
                    payExchangeApplyItem.RealName = Needs.Wl.Admin.Plat.AdminPlat.Current.RealName;
                    payExchangeApplyItem.PidEnter();

                    //剩余预付汇记录金额
                    var payItem = new Needs.Ccs.Services.Models.PayExchangeApplyItem();
                    payExchangeApplyItem.PayExchangeApplyID = payID;
                    payExchangeApplyItem.OrderID = orderID;
                    payExchangeApplyItem.Amount = amount;
                    payExchangeApplyItem.PreItemEnter();

                    //更正核销金额
                    var orderReceipt = new Needs.Ccs.Services.Models.OrderReceipt();
                    orderReceipt.OrderID = payOrder.OrderID;
                    orderReceipt.FeeSourceID = payOrder.ID;
                    orderReceipt.OrderReceiptEnter();

                    Response.Write((new { success = true, message = "" }).Json());
                }


            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "操作失败：" + ex.Message }).Json());
            }

        }
    }
}