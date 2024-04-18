using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.Receipt.Store
{
    public partial class StoreReceiptDialog : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Model.MainOrderID = Request.QueryString["MainOrderID"];
            string financeReceiptId = Request.QueryString["FinanceReceiptId"];
            this.Model.FinanceReceiptId = financeReceiptId;

            var notice = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.MyReceiptNotices[financeReceiptId];

            this.Model.NoticeData = new
            {
                ClienName = notice.Client.Company.Name,
                Amount = notice.AvailableAmount.ToString("#0.00"),
                ClearAmount = notice.ClearAmount.ToString("#0.00"),
                ReceiptDate = notice.ReceiptDate.ToString("yyyy-MM-dd"),

                //UnReceiveListClientName = unReceiveListClientName,
            }.Json();
        }

        protected void StoreReceivableOrderReceipt()
        {
            string MainOrderID = Request.QueryString["MainOrderID"];

            List<LambdaExpression> lamdas = new List<LambdaExpression>();
            Expression<Func<Needs.Ccs.Services.Views.VouchersStatisticsViewModel, bool>> lambda1 = item => item.MainOrderID == MainOrderID;
            lamdas.Add(lambda1);

            var view = new Needs.Ccs.Services.Views.VouchersStatisticsView();
            var vouchers = view.GetList(lamdas.ToArray());

            Func<Needs.Ccs.Services.Views.VouchersStatisticsViewModel, object> convert = item => new
            {
                ReceivableID = item.ReceivableID,
                MainOrderID = item.MainOrderID,
                FeeTypeShowName = !string.IsNullOrEmpty(item.Subject) ? item.Subject : (item.Catalog ?? ""),
                ReceivableAmount = decimal.Round((item.LeftPrice - item.ReducePrice) * 1.06m - item.RightPrice, 2),
                ApplicationID = item.ApplicationID,
                IsFooter = false,
            };

            var resultRows = vouchers.Select(convert).ToList();

            decimal sum = 0;
            if (vouchers != null && vouchers.Any())
            {
                sum = vouchers.Sum(i => decimal.Round((i.LeftPrice - i.ReducePrice) * 1.06m - i.RightPrice, 2));
            }

            List<dynamic> listFooter = new List<dynamic>();
            listFooter.Add(new
            {
                ReceivableID = "",
                MainOrderID = "",
                FeeTypeShowName = "总计",
                ReceivableAmount = sum,
                ApplicationID = "",
                IsFooter = true,
            });

            Response.Write(new
            {
                rows = resultRows,
                footer = listFooter.ToArray(),
            }.Json());
        }


        protected void Submit()
        {
            try
            {
                var admin = new Needs.Ccs.Services.Views.AdminsTopView2().Where(t => t.OriginID == Needs.Wl.Admin.Plat.AdminPlat.Current.ID).FirstOrDefault();

                string financeReceiptId = Request.QueryString["FinanceReceiptId"];
                string ClienName = Request.Form["ClienName"];
                var sources = Request.Form["ReceiveData"].Replace("&quot;", "\'").Replace("amp;", "");
                var receiveDataModel = sources.JsonTo<dynamic>();

                foreach (var item in receiveDataModel)
                {
                    string strReceiveAmount = item.ReceiveAmount;
                    decimal decReceiveAmount = 0;
                    if (!decimal.TryParse(strReceiveAmount, out decReceiveAmount))
                    {
                        throw new Exception("提交了非数字");
                    }
                    if (decReceiveAmount < 0)
                    {
                        throw new Exception("提交了小于0的数字");
                    }
                }

                //foreach (var item in receiveDataModel)
                //{
                //    decimal decReceivableAmount = item.ReceivableAmount;
                //    decimal decReceiveAmount = item.ReceiveAmount;

                //    if (decReceivableAmount < decReceiveAmount)
                //    {
                //        throw new Exception("单个类别中，实收金额不能大于应收金额");
                //    }
                //}


                string oneMainOrderID = string.Empty;
                string applicationID = string.Empty;

                decimal increaceAmount = 0;

                List<Needs.Ccs.Services.Models.StoreReceiptToYahvAmountModel> listStoreReceipt = new List<Needs.Ccs.Services.Models.StoreReceiptToYahvAmountModel>();
                foreach (var item in receiveDataModel)
                {
                    decimal decReceiveAmount = item.ReceiveAmount;
                    if (0 == decReceiveAmount)
                    {
                        continue;
                    }

                    listStoreReceipt.Add(new Needs.Ccs.Services.Models.StoreReceiptToYahvAmountModel()
                    {
                        Amount = item.ReceiveAmount,
                        ReceivableID = item.ReceivableID,
                    });

                    oneMainOrderID = item.MainOrderID;
                    applicationID = item.ApplicationID;

                    increaceAmount += Convert.ToDecimal(item.ReceiveAmount);
                }

                //向Crm收取代仓储核销费用 Begin

                Needs.Ccs.Services.Models.StoreReceiptToYahv toYahv = new Needs.Ccs.Services.Models.StoreReceiptToYahv(
                    oneMainOrderID,
                    admin,
                    financeReceiptId,
                    ClienName,
                    applicationID,
                    listStoreReceipt.ToArray());
                toYahv.Execute();

                //向Crm收取代仓储核销费用 End

                //在 ReceiptNotices 表中增加 ClearAmount 数值 Begin

                var storeReceiptHandler = new Needs.Ccs.Services.Models.StoreReceiptHandler(financeReceiptId, increaceAmount);
                storeReceiptHandler.Execute();

                //在 ReceiptNotices 表中增加 ClearAmount 数值 End

                Response.Write((new { success = "true", message = "提交成功", clearAmount = storeReceiptHandler.NewClearAmount, }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = "false", message = "提交失败" + ex.Message }).Json());
            }
        }
    }
}