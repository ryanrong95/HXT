using Needs.Ccs.Services;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.Swap
{
    public partial class EditAddDecHead : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string DecHeadIDFromAddDecHeadInEditInfo = Request.QueryString["DecHeadIDFromAddDecHeadInEditInfo"];
            string BankName = Request.QueryString["BankName"];
            string Currency = Request.QueryString["Currency"];
            string SwapNoticeID = Request.QueryString["SwapNoticeID"];

            this.Model.DecHeadIDFromAddDecHeadInEditInfo = DecHeadIDFromAddDecHeadInEditInfo;
            this.Model.BankName1 = BankName;
            this.Model.Currency1 = Currency;
            this.Model.SwapNoticeID1 = SwapNoticeID;
        }

        protected void data()
        {
            string CleanIDs = Request.QueryString["CleanIDs"];

            string[] CleanIDs_Array = { };
            if (!string.IsNullOrEmpty(CleanIDs))
            {
                CleanIDs_Array = CleanIDs.Split(',');
            }

            List<LambdaExpression> lamdas = new List<LambdaExpression>();
            lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.UnSwapDecHeadListView.UnSwapDecHeadListModel, bool>>)(t => CleanIDs_Array.Contains(t.DecHeadID)));
            var selectedUnSwapDecHeadList = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.UnSwapDecHeadListView.GetAll(lamdas.ToArray()).ToList();

            Func<Needs.Ccs.Services.Views.UnSwapDecHeadListView.UnSwapDecHeadListModel, object> convert = unSwapDecHead => new
            {
                ID = unSwapDecHead.DecHeadID,
                ContrNo = unSwapDecHead.ContrNo,
                OrderID = unSwapDecHead.OrderID,
                EntryID = unSwapDecHead.EntryId,
                OwnerName = unSwapDecHead.OwnerName,
                Currency = unSwapDecHead.Currency,
                SwapAmount = unSwapDecHead.SwapAmount,
                DDate = unSwapDecHead.DDate?.ToString("yyyy-MM-dd HH:mm:ss"),
                SwapStatus = unSwapDecHead.SwapStatus.GetDescription(),
                URL = Needs.Utils.FileDirectory.Current.FileServerUrl + @"/" + unSwapDecHead?.Url.ToUrl(),
                DecHeadStatus = MultiEnumUtils.ToText<Needs.Ccs.Services.Enums.CusDecStatus>(
                    (Needs.Ccs.Services.Enums.CusDecStatus)Enum.Parse(typeof(Needs.Ccs.Services.Enums.CusDecStatus), unSwapDecHead.CusDecStatus)),
                SwapedAmount = unSwapDecHead.SwapedAmount,
                MaxInputSwapAmount = unSwapDecHead.SwapAmount - unSwapDecHead.SwapedAmount - unSwapDecHead.UserCurrentPayApply.ToRound(2),
                UserCurrentPayApply = unSwapDecHead.UserCurrentPayApply.ToRound(2),
            };

            Response.Write(new
            {
                rows = selectedUnSwapDecHeadList.Select(convert).ToArray(),
            }.Json());
        }

        /// <summary>
        /// 申请换汇-此处已过滤掉含有黑名单国家的报关单
        /// </summary>
        protected void SubmitApply()
        {
            try
            {
                //string BankID = Request.Form["BankID"];
                string BankName = Request.Form["BankName"];
                string TheInputAmounts = Request.Form["TheInputAmounts"];
                string UserPayExchangeApplyAmounts = Request.Form["UserPayExchangeApplyAmounts"];

                string oldSwapNoticeID = Request.Form["SwapNoticeID"];

                //输入换汇金额、自定义换汇金额
                string[] TheInputAmounts_Array = TheInputAmounts.Split(',');
                string[] UserPayExchangeApplyAmounts_Array = UserPayExchangeApplyAmounts.Split(',');

                List<string> CleanIDs = new List<string>();

                decimal TheInputAmountsTotal = 0;
                decimal UserPayExchangeApplyAmountsTotal = 0;

                foreach (var theInputAmount in TheInputAmounts_Array)
                {
                    string decHeadID = theInputAmount.Split('|')[0];

                    CleanIDs.Add(decHeadID);
                    TheInputAmountsTotal += decimal.Parse(theInputAmount.Split('|')[1]);

                    string oneUserPayExchangeApplyAmount = UserPayExchangeApplyAmounts_Array.Where(t => t.StartsWith(decHeadID)).FirstOrDefault();
                    if (string.IsNullOrEmpty(oneUserPayExchangeApplyAmount))
                    {
                        throw new Exception("输入金额列表中的 DecHeadID 在用户本次申请列表中查询不到");
                    }

                    UserPayExchangeApplyAmountsTotal += decimal.Parse(oneUserPayExchangeApplyAmount.Split('|')[1]);
                }



                List<LambdaExpression> lamdas = new List<LambdaExpression>();
                lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.UnSwapDecHeadListView.UnSwapDecHeadListModel, bool>>)(t => CleanIDs.ToArray().Contains(t.DecHeadID)));
                var selectedUnSwapDecHeadList = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.UnSwapDecHeadListView.GetAll(lamdas.ToArray()).ToList();

                // ============================= "客户本次申请金额"和"自定义换汇金额"在服务器端要进行校验  Begin =============================

                StringBuilder sbCheckError = new StringBuilder();

                foreach (var theInputAmount in TheInputAmounts_Array)
                {
                    string decHeadID = theInputAmount.Split('|')[0];

                    var theSelectedUnSwapDecHead = selectedUnSwapDecHeadList.Where(t => t.DecHeadID == decHeadID).FirstOrDefault();
                    if (theSelectedUnSwapDecHead == null)
                    {
                        sbCheckError.Append("报关单 <label style=\"color:green\">" + theSelectedUnSwapDecHead.ContrNo + "</label>在未换汇完成列表中已经查询不到<br>");
                        continue;
                    }

                    decimal maxInputSwapAmount = theSelectedUnSwapDecHead.SwapAmount - theSelectedUnSwapDecHead.SwapedAmount - theSelectedUnSwapDecHead.UserCurrentPayApply.ToRound(2);
                    if (decimal.Parse(theInputAmount.Split('|')[1]) > maxInputSwapAmount)
                    {
                        sbCheckError.Append("报关单 <label style=\"color:green\">" + theSelectedUnSwapDecHead.ContrNo + "</label>自定义换汇金额 "
                            + theInputAmount.Split('|')[1] + " 超过最大自定义换汇金额 " + maxInputSwapAmount + "<br>");
                    }

                    string oneUserPayExchangeApplyAmount = UserPayExchangeApplyAmounts_Array.Where(t => t.StartsWith(decHeadID)).FirstOrDefault();
                    if (decimal.Parse(oneUserPayExchangeApplyAmount.Split('|')[1]) != theSelectedUnSwapDecHead.UserCurrentPayApply.ToRound(2))
                    {
                        sbCheckError.Append("报关单 <label style=\"color:green\">" + theSelectedUnSwapDecHead.ContrNo + "</label>界面中用户本次申请换汇金额 "
                            + oneUserPayExchangeApplyAmount.Split('|')[1] + " 已变为 " + theSelectedUnSwapDecHead.UserCurrentPayApply.ToRound(2) + "<br>");
                    }


                }

                if (!string.IsNullOrEmpty(sbCheckError.ToString()))
                {
                    Response.Write((new { success = false, message = "申请失败：" + sbCheckError.ToString(), }).Json());
                }

                // ============================= "客户本次申请金额"和"自定义换汇金额"在服务器端要进行校验  End =============================


                //string newSwapNoticeID = Needs.Overall.PKeySigner.Pick(PKeyType.InvoiceNotice);

                //Needs.Wl.Models.SwapNotice swapNotice = new Needs.Wl.Models.SwapNotice();
                //swapNotice.ID = newSwapNoticeID;
                //swapNotice.AdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
                //swapNotice.Currency = selectedUnSwapDecHeadList.FirstOrDefault().Currency;
                //swapNotice.TotalAmount = TheInputAmountsTotal + UserPayExchangeApplyAmountsTotal;
                //swapNotice.BankName = BankName;
                //swapNotice.CreateDate = DateTime.Now;
                //swapNotice.UpdateDate = DateTime.Now;
                //swapNotice.Status = Needs.Wl.Models.Enums.SwapStatus.Auditing;



                Needs.Ccs.Services.Views.UserCurrentPayApplyListByDecHeeadIDView userCurrentPayApplyListByDecHeeadIDView =
                    new Needs.Ccs.Services.Views.UserCurrentPayApplyListByDecHeeadIDView(CleanIDs.ToArray());
                var userCurrentPayApplyList = userCurrentPayApplyListByDecHeeadIDView.ToList();


                List<Needs.Wl.Models.SwapNoticeItem> swapNoticeItems = new List<Needs.Wl.Models.SwapNoticeItem>();
                List<Needs.Wl.Models.PayApplySwapNoticeItemRelation> payApplySwapNoticeItemRelations = new List<Needs.Wl.Models.PayApplySwapNoticeItemRelation>();
                foreach (var theInputAmount in TheInputAmounts_Array)
                {
                    string decHeadID = theInputAmount.Split('|')[0];
                    string oneUserPayExchangeApplyAmount = UserPayExchangeApplyAmounts_Array.Where(t => t.StartsWith(decHeadID)).FirstOrDefault();

                    decimal amountForOneDecHead = decimal.Parse(theInputAmount.Split('|')[1]) + decimal.Parse(oneUserPayExchangeApplyAmount.Split('|')[1]);
                    if (amountForOneDecHead != 0)
                    {
                        //某一个报关单的 SwapNoticeItem
                        string newSwapNoticeItemID = Needs.Overall.PKeySigner.Pick(PKeyType.InvoiceNoticeItem);

                        swapNoticeItems.Add(new Needs.Wl.Models.SwapNoticeItem()
                        {
                            ID = newSwapNoticeItemID,
                            SwapNoticeID = oldSwapNoticeID,
                            DecHeadID = decHeadID,
                            CreateDate = DateTime.Now,
                            Amount = amountForOneDecHead,
                            Status = Needs.Wl.Models.Enums.Status.Normal,
                            CustomizeAmount = decimal.Parse(theInputAmount.Split('|')[1]),
                        });

                        //PayExchangeApplyItems 和 SwapNoticeItems 的对应关系
                        var thisDecHeadUserCurrentPayApplyList = userCurrentPayApplyList.Where(t => t.DecHeadID == decHeadID).ToList();
                        decimal sumForThisDecHeadUserCurrentPayApplyList = thisDecHeadUserCurrentPayApplyList.Sum(t => t.CurrentPayApplyAmount).ToRound(2);
                        if (sumForThisDecHeadUserCurrentPayApplyList != decimal.Parse(oneUserPayExchangeApplyAmount.Split('|')[1]))
                        {
                            throw new Exception("通过 DecHeadID：" + decHeadID + " 查询到的用户当前申请金额之和与发送置服务器的用户当前申请金额不一致");
                        }

                        foreach (var item in thisDecHeadUserCurrentPayApplyList)
                        {
                            payApplySwapNoticeItemRelations.Add(new Needs.Wl.Models.PayApplySwapNoticeItemRelation()
                            {
                                ID = Guid.NewGuid().ToString("N"),
                                PayExchangeApplyItemID = item.PayExchangeApplyItemID,
                                SwapNoticeItemID = newSwapNoticeItemID,
                                Status = Needs.Wl.Models.Enums.Status.Normal,
                                CreateDate = DateTime.Now,
                                UpdateDate = DateTime.Now,
                            });
                        }

                    }

                }


                var decHeadTotalSwapAmounts = selectedUnSwapDecHeadList.Select(t => new Needs.Wl.Finance.Services.Models.DecHeadTotalSwapAmount()
                {
                    DecHeadID = t.DecHeadID,
                    TotalSwapAmount = t.SwapAmount,
                }).ToArray();


                //Needs.Wl.Finance.Services.Models.SwapApplyHandler swapApplyHandler = new Needs.Wl.Finance.Services.Models.SwapApplyHandler(
                //    swapNotice, swapNoticeItems.ToArray(), decHeadTotalSwapAmounts, payApplySwapNoticeItemRelations.ToArray());

                //swapApplyHandler.DoApplay();

                Needs.Wl.Finance.Services.Models.AddSwapApplyHandler addSwapApplyHandler
                    = new Needs.Wl.Finance.Services.Models.AddSwapApplyHandler(
                        oldSwapNoticeID, swapNoticeItems.ToArray(), decHeadTotalSwapAmounts, payApplySwapNoticeItemRelations.ToArray());

                addSwapApplyHandler.Do();

                Response.Write((new { success = true, message = "申请成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "申请失败：" + ex.Message }).Json());
            }
        }



    }
}