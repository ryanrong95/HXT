using Needs.Ccs.Services;
using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.Payment.PayExchange
{
    public partial class Confirm : Uc.PageBase
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
            //付款人
            this.Model.PayerData = Needs.Wl.Admin.Plat.AdminPlat.Current.Permissions.AdminRoles
                .Where(manager => manager.Role.Name == "集团财务出纳").Select(item => new { item.Admin.ID, item.Admin.ByName }).Json();

            //协议、实收RMB、应收RMB等信息
            string ClientID = Request.QueryString["ClientID"];

            //协议
            string periodType = "";
            var agreement = new Needs.Wl.Models.Views.ClientAgreementsView(ClientID).FirstOrDefault();
            if (agreement != null && agreement.ProductFeeClause != null)
            {
                periodType = agreement.ProductFeeClause.PeriodType.GetDescription();
            }
            this.Model.PeriodType = periodType;

            //应收总额
            this.Model.ReceivableAmountTotal = Request.QueryString["ReceivableAmountTotal"];
            //实收总额
            string ReceivedAmountTotal = Request.QueryString["ReceivedAmountTotal"];
            this.Model.ReceivedAmountTotal = ReceivedAmountTotal;
            //外币, 客户要付
            string WaiBiPrice = Request.QueryString["WaiBiPrice"];
            this.Model.WaiBiPrice = WaiBiPrice;

            //人民币
            string RmbPrice = Request.QueryString["RmbPrice"];
            this.Model.RmbPrice = RmbPrice;

            //汇率
            decimal WaiBiPriceDecimal = Convert.ToDecimal(WaiBiPrice);
            if (WaiBiPriceDecimal != 0)
            {
                this.Model.TheExchangeRate = (Convert.ToDecimal(ReceivedAmountTotal) / WaiBiPriceDecimal).ToRound(4);
            }
            else
            {
                this.Model.TheExchangeRate = "";
            }

            //查出货款可用垫款
            var availableProductFeeView = new Needs.Ccs.Services.Views.AvailableProductFeeView(ClientID);
            //decimal availableProductFee = (availableProductFeeView.GetProductUpperLimit() - availableProductFeeView.GetProductPayable()).ToRound(2);
            decimal availableProductFee = (availableProductFeeView.GetProductAdvanceMoneyApply()); //- availableProductFeeView.GetProductPayable()).ToRound(2);//by yess 2020-12-29
            this.Model.AvailableProductFee = availableProductFee;
        }

        /// <summary>
        /// 审批通过
        /// </summary>
        protected void Save()
        {
            try
            {
                string ID = Request.Form["ID"];
                string Payer = Request.Form["Payer"];
                string Summary = Request.Form["Summary"];
                string receivableAmountTotal = Request.Form["ReceivableAmountTotal"];
                string receivedAmountTotal = Request.Form["ReceivedAmountTotal"];
                string IsAdvanceMoney = Request.Form["IsAdvanceMoney"];
                string RmbPrice = Request.Form["RmbPrice"];
                //付款人
                var payer = Needs.Underly.FkoFactory<Admin>.Create(Payer);
                //操作人
                var admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);

                var apply = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.UnApprovalPayExchangeApply.Where(item => item.ID == ID).FirstOrDefault();
                apply.SetOperator(admin);
                apply.SetPayer(payer);
                //审批
                apply.Approval(Summary);

                //生成垫款记录   by 2020-12-28 yess 
                if (IsAdvanceMoney == "0")
                {
                    AdvanceRecordModel advanceRecord = new AdvanceRecordModel();
                    //  advanceRecord.ID = Guid.NewGuid().ToString("N");
                    advanceRecord.ClientID = apply.ClientID;
                    advanceRecord.PayExchangeID = ID;
                    advanceRecord.AmountUsed = Convert.ToDecimal(RmbPrice);//Convert.ToDecimal(receivableAmountTotal) - Convert.ToDecimal(receivedAmountTotal);
                    advanceRecord.Enter();
                }

                NoticeLog noticeLog = new NoticeLog();
                noticeLog.MainID = apply.ID;
                noticeLog.NoticeType = Needs.Ccs.Services.Enums.SendNoticeType.PayExChangeApprove;
                noticeLog.OrderID = apply.PayExchangeApplyItems.FirstOrDefault().OrderID;
                noticeLog.Readed = true;
                noticeLog.SendNotice();

                Response.Write((new { success = true, message = "审批成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "审批失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 审批退回
        /// </summary>
        protected void Cancel()
        {
            try
            {
                string Summary = Request.Form["Summary"];
                var admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);

                string ApplyID = Request.Form["ID"];
                var apply = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.UnApprovalPayExchangeApply
                    .Where(item => item.ID == ApplyID).FirstOrDefault();
                apply.SetOperator(admin);
                apply.ApprovalCancel(Summary);

                NoticeLog noticeLog = new NoticeLog();
                noticeLog.MainID = apply.ID;
                noticeLog.NoticeType = Needs.Ccs.Services.Enums.SendNoticeType.PayExChangeApprove;
                noticeLog.OrderID = apply.PayExchangeApplyItems.FirstOrDefault().OrderID;
                noticeLog.Readed = true;
                noticeLog.SendNotice();

                Response.Write((new { success = true, message = "审批成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "审批失败" + ex.Message }).Json());
            }
        }
    }
}
