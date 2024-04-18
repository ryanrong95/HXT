using Needs.Ccs.Services;
using Needs.Ccs.Services.Models;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.Declare
{
    public partial class SelectBank : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            this.Model.BankData = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.SwapBanks.Select(item => new { value = item.ID, text = item.Name }).Json();
        }

        /// <summary>
        /// 校验是否存在黑名单，存在：提示消息，不存在，申请成功
        /// </summary>
        protected void CheckLimitCountryOld()
        {
            try
            {
                string BankID = Request.Form["BankID"];
                string BankName = Request.Form["BankName"];
                string IDs = Request.Form["IDs"];
                string[] arrId = IDs.Replace("[", "").Replace("]", "").Replace("&quot;", "").Split(',');
                var decheads = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.SwapDecHead
                        .Where(item => arrId.Contains(item.ID)).AsEnumerable();

                var countryList = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.SwapLimitCountries.AsQueryable()
                        .Where(item => item.Status == Needs.Ccs.Services.Enums.Status.Normal)
                        .Where(item => item.BankID == BankID).ToList();
                // .Select(item => item.Code);

                //验证黑名单国家
                var cleanIDs = string.Empty;
                var message = string.Empty;
                var total = 0M;
                if (countryList.Count() > 0)
                {
                    foreach (var head in decheads)
                    {
                        if (head.Lists.Any(t => countryList.Select(c => c.Code).Contains(t.OriginCountry)))
                        {
                            //此单存在限制地区
                            message += string.Concat("报关单<label style=\"color:green\">", head.ContrNo, "</label> 有", BankName, "换汇黑名单国家 <label style=\"color:red\">",
                                string.Join("，", countryList.Where(t => head.Lists.Select(c => c.OriginCountry).Contains(t.Code)).Select(t => t.Name).ToArray()),
                                "</label><br/>");
                        }
                        else
                        {
                            total += head.Lists.Sum(t => t.DeclTotal);
                            cleanIDs += head.ID + ",";
                        }
                    }
                }

                //无黑名单
                if (string.IsNullOrEmpty(message))
                {
                    var swapContext = new SwapContext();
                    swapContext.Creater = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                    swapContext.SwapDecHeads = decheads;
                    swapContext.BankName = BankName;
                    swapContext.SubmitApply();
                    Response.Write((new { success = true, total, message = "申请成功" }).Json());
                }
                else
                {
                    Response.Write((new { success = false, message, total, ids = cleanIDs.Trim(',') }).Json());
                }
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "申请失败：" + ex.Message }).Json());
            }


        }

        /// <summary>
        /// 校验是否存在黑名单
        /// </summary>
        protected void CheckLimitCountry()
        {
            try
            {
                string BankID = Request.Form["BankID"];
                string BankName = Request.Form["BankName"];
                string IDs = Request.Form["IDs"];
                string[] arrDecHeadId = IDs.Replace("[", "").Replace("]", "").Replace("&quot;", "").Split(',');

                List<LambdaExpression> lamdas = new List<LambdaExpression>();
                lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.UnSwapDecHeadListView.UnSwapDecHeadListModel, bool>>)(t => arrDecHeadId.Contains(t.DecHeadID)));
                var selectedUnSwapDecHeadList = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.UnSwapDecHeadListView.GetAll(lamdas.ToArray()).ToList();

                var countryList = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.SwapLimitCountries.AsQueryable()
                            .Where(item => item.BankID == BankID && item.Status == Needs.Ccs.Services.Enums.Status.Normal)
                            .ToList();

                var cleanIDs = string.Empty;
                StringBuilder sbMessage = new StringBuilder();
                var total = 0M;

                if (countryList != null && countryList.Any())
                {
                    //该银行有黑名单国家

                    var allDecList = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DecOriginList.Where(t => arrDecHeadId.Contains(t.DeclarationID)).ToList();

                    bool isReach = false;

                    foreach (var selectedUnSwapDecHead in selectedUnSwapDecHeadList)
                    {
                        var thisDecHeadDecList = allDecList.Where(t => t.DeclarationID == selectedUnSwapDecHead.DecHeadID);
                        if (thisDecHeadDecList.Any(t => countryList.Select(c => c.Code).Contains(t.OriginCountry)))
                        {
                            isReach = true;

                            sbMessage.Append(string.Concat("报关单<label style=\"color:green\">", selectedUnSwapDecHead.ContrNo, "</label> 有", BankName, "换汇黑名单国家 <label style=\"color:red\">"));
                            sbMessage.Append(string.Join("，", countryList.Where(t => thisDecHeadDecList.Select(c => c.OriginCountry).Contains(t.Code)).Select(t => t.Name).ToArray()));
                            sbMessage.Append("</label>，已移除<br/>");
                        }
                        else
                        {
                            total += (selectedUnSwapDecHead.SwapAmount - selectedUnSwapDecHead.SwapedAmount).ToRound(2);
                            cleanIDs += selectedUnSwapDecHead.DecHeadID + ",";
                        }
                    }

                    if (false == isReach)
                    {
                        sbMessage.Append("所选报关单中，无" + BankName + "换汇黑名单国家<br/>");
                    }

                    sbMessage.Append("过滤后金额：" + total);
                }
                else
                {
                    //该银行无黑名单国家

                    sbMessage.Append("所选报关单中，无" + BankName + "换汇黑名单国家<br/>");
                    sbMessage.Append("过滤后金额：" + selectedUnSwapDecHeadList.Sum(t => (t.SwapAmount - t.SwapedAmount).ToRound(2)));

                    cleanIDs = string.Join(",", selectedUnSwapDecHeadList.Select(t => t.DecHeadID).ToArray());
                }

                Response.Write((new { success = true, message = sbMessage.ToString(), ids = cleanIDs.Trim(','), }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "错误：" + ex.Message, }).Json());
            }
        }

        /// <summary>
        /// 申请换汇-此处已过滤掉含有黑名单国家的报关单
        /// </summary>
        protected void SubmitApply()
        {
            try
            {
                string BankID = Request.Form["BankID"];
                string BankName = Request.Form["BankName"];
                string IDs = Request.Form["IDs"];
                string[] arrId = IDs.Replace("[", "").Replace("]", "").Replace("&quot;", "").Split(',');

                var decheads = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.SwapDecHead
                    .Where(item => arrId.Contains(item.ID)).AsEnumerable();

                ////管控国家
                //var bank = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.SwapBanks.AsQueryable().Where(item => item.ID == BankID).FirstOrDefault();
                //var list = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.SwapLimitCountries.AsQueryable()
                //    .Where(item => item.Status == Needs.Ccs.Services.Enums.Status.Normal)
                //    .Where(item => item.BankID == BankID);
                //if (list.Count() != 0)
                //{
                //    //判断是否有管控区域
                //    foreach (var dechead in decheads)
                //    {
                //        foreach (var item in dechead.Lists)
                //        {
                //            if (list.Where(t => t.Code == item.OriginCountry).Count() > 0)
                //            {
                //                Response.Write((new
                //                {
                //                    success = false,
                //                    message = "申请失败：报关单" + dechead.ContrNo + "有管控地区：" + item.OriginCountry
                //                }).Json());
                //                return;
                //            }
                //        }
                //    }
                //}

                var swapContext = new SwapContext();
                swapContext.Creater = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                swapContext.SwapDecHeads = decheads;
                swapContext.BankName = BankName;
                swapContext.SubmitApply();
                Response.Write((new { success = true, message = "申请成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "申请失败：" + ex.Message }).Json());
            }
        }
    }
}