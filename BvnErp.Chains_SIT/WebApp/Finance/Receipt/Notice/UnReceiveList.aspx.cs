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

namespace WebApp.Finance.Receipt.Notice
{
    /// <summary>
    /// 收款通知列表界面
    /// </summary>
    public partial class UnReceiveList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Model.ClientName = Server.UrlDecode(Request.QueryString["ClientName"]) ?? string.Empty;
        }

        protected void data()
        {
            string ClientName = Request.QueryString["ClientName"];
            string QuerenStatus = Request.QueryString["QuerenStatus"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];

            if (!string.IsNullOrEmpty(ClientName))
            {
                ClientName = ClientName.Trim(',');
            }
            if (!string.IsNullOrEmpty(QuerenStatus))
            {
                if (QuerenStatus.Contains(","))
                {
                    string[] arr = QuerenStatus.Split(',');
                    if (arr != null && arr.Any())
                    {
                        QuerenStatus = arr[arr.Length - 1];
                    }
                }
            }
            if (!string.IsNullOrEmpty(StartDate))
            {
                StartDate = StartDate.Trim(',');
            }
            if (!string.IsNullOrEmpty(EndDate))
            {
                EndDate = EndDate.Trim(',');
            }

            //var notices = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.MyReceiptNotices.Where(x => x.ClearAmount < x.Amount).AsQueryable();
            var notices = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.MyReceiptNotices.AsQueryable();

            if (string.IsNullOrEmpty(ClientName) == false)
            {
                notices = notices.Where(item => item.Client.Company.Name.Contains(ClientName));
            }
            if (!string.IsNullOrEmpty(QuerenStatus) && QuerenStatus != "0")
            {
                //0 - 全部, 1 - 未确认, 2 - 已确认
                if (QuerenStatus == "1")
                {
                    notices = notices.Where(x => x.ClearAmount < x.AvailableAmount);
                }
                else if (QuerenStatus == "2")
                {
                    notices = notices.Where(x => x.ClearAmount >= x.AvailableAmount);
                }
            }
            if (string.IsNullOrEmpty(StartDate) == false)
            {
                DateTime start;
                if (DateTime.TryParse(StartDate, out start))
                {
                    notices = notices.Where(item => item.ReceiptDate >= start);
                }
            }
            if (string.IsNullOrEmpty(EndDate) == false)
            {
                DateTime end;
                if (DateTime.TryParse(EndDate, out end))
                {
                    end = end.AddDays(1);
                    notices = notices.Where(item => item.ReceiptDate < end);
                }
            }

            notices = notices.OrderByDescending(item => item.ReceiptDate);

            //前台显示
            Func<ReceiptNotice, object> convert = item => new
            {
                item.ID,
                ClientID = item.Client?.ID, //用于传递到下一个页面
                item?.SeqNo,
                VaultName = item.Vault.Name,
                item?.Account.AccountName,
                ClientName = item.Client?.Company.Name,
                FinanceReceiptFeeType = item.FeeType.GetDescription(),
                Amount = item?.Amount.ToString("#0.00"),
                AvailableAmount = item.AvailableAmount,
                ClearAmount = item.ClearAmount.ToString("#0.00"),
                QuerenStatus = item.ClearAmount < item.AvailableAmount ? "未确认" : "已确认",
                ReceiptDate = item?.ReceiptDate.ToString("yyyy-MM-dd"),
                DyjID = item.DyjID
            };

            this.Paging(notices, convert);
        }

        protected void UnmackReceipt()
        {
            try
            {
                string financeReceiptId = Request.Form["FinanceReceiptId"];

                var targetUnmacks = from orderReceipt in Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.OrderReceiptsAllStatus
                                   where orderReceipt.FinanceReceiptID == financeReceiptId 
                                        && orderReceipt.Status == Needs.Ccs.Services.Enums.Status.Normal
                                        && orderReceipt.Type == Needs.Ccs.Services.Enums.OrderReceiptType.Received
                                   select orderReceipt;

                var Admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);

                foreach (var targetUnmack in targetUnmacks)
                {
                    var unmarkReceivedChongZhen = new UnmarkOrderReceipt();
                    unmarkReceivedChongZhen.ID = Needs.Overall.PKeySigner.Pick(PKeyType.OrderReceipt); //主键ID（OrderReceipt +8位年月日+6位流水号）
                    unmarkReceivedChongZhen.ReceiptNoticeID = financeReceiptId;
                    unmarkReceivedChongZhen.ClientID = targetUnmack.ClientID;
                    unmarkReceivedChongZhen.OrderID = targetUnmack.OrderID;
                    unmarkReceivedChongZhen.FeeSourceID = targetUnmack.FeeSourceID;
                    unmarkReceivedChongZhen.FeeType = targetUnmack.FeeType;
                    unmarkReceivedChongZhen.Amount = targetUnmack.Amount;
                    unmarkReceivedChongZhen.Admin = Admin;
                    unmarkReceivedChongZhen.Enter();

                    var unmarkReceivedOrigin = new OrderReceived(targetUnmack, Needs.Ccs.Services.Enums.Status.Delete);
                    unmarkReceivedOrigin.Enter();

                    //取消垫资记录
                    var advanceRecords = new Needs.Ccs.Services.Views.AdvanceMoneyRecordView().Where(t => t.ClientID == targetUnmack.ClientID
                        && t.OrderID == targetUnmack.OrderID && t.PayExchangeID == targetUnmack.FeeSourceID).ToList();
                    if (advanceRecords != null && advanceRecords.Any())
                    {
                        var orderReceiptSum = ((from orderReceipt in Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.OrderReceiptsAllStatus
                                                where orderReceipt.ID == unmarkReceivedChongZhen.ID || orderReceipt.ID == targetUnmack.ID
                                                     && orderReceipt.Status == Needs.Ccs.Services.Enums.Status.Delete
                                                     && orderReceipt.Type == Needs.Ccs.Services.Enums.OrderReceiptType.Received
                                                select orderReceipt).ToList()).Sum(item => item.Amount);
                        if (orderReceiptSum == 0)
                        {
                            var advanceRecordModel = new Needs.Ccs.Services.Models.AdvanceRecordModel();
                            advanceRecordModel.ClientID = targetUnmack.ClientID;
                            advanceRecordModel.OrderID = targetUnmack.OrderID;
                            advanceRecordModel.PayExchangeID = targetUnmack.FeeSourceID;
                            advanceRecordModel.AmountUsed = targetUnmack.Amount;
                            advanceRecordModel.AdvanceRecordUpdate();
                        }
                    }


                }

                //此处调用 Yahv Begin

                ReceiptToYahv toYahv = new ReceiptToYahv(financeReceiptId, Admin);
                toYahv.Unmack();

                //此处调用 Yahv End

                Response.Write((new { success = "true", message = "提交成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = "false", message = "提交失败" + ex.Message }).Json());
            }
        }

        protected void CheckReceive()
        {
            bool can = true;
            string ReceiptID = Request.Form["ReceiptID"];
            var Receipt = new Needs.Ccs.Services.Views.RefundApplyView().Where(x => x.FinanceReceiptID == ReceiptID).AsQueryable();
            if (Receipt.Any(t => t.ApplyStatus == Needs.Ccs.Services.Enums.RefundApplyStatus.Applied))
            {
                can = false;
            }
            Response.Write((new { success = can, message = "" }).Json());
        }
    }
}