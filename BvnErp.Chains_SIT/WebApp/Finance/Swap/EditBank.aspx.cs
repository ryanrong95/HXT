using Needs.Ccs.Services;
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
    public partial class EditBank : Uc.PageBase
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
            string BankName = Request.QueryString["CurrentBankName"];
            string SwapNoticeID = Request.QueryString["SwapNoticeID"];

            this.Model.BankData = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.SwapBanks
                .Where(t => t.Name != BankName)
                .Select(item => new { value = item.ID, text = item.Name }).Json();

            this.Model.SwapNoticeID = SwapNoticeID;

            //var swapNoticeItems = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.SwapNoticeItem.Where(item => item.SwapNoticeID == SwapNoticeID);
            //string[] currentDecHeadIDs = swapNoticeItems.Select(t => t.SwapDecHead.ID).ToArray();

            //this.Model.CurrentDecHeadIDs = currentDecHeadIDs.Json();
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
                string SwapNoticeID = Request.Form["SwapNoticeID"];

                var swapNoticeItems = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.SwapNoticeItem.Where(item => item.SwapNoticeID == SwapNoticeID);
                string[] currentDecHeadIDs = swapNoticeItems.Select(t => t.SwapDecHead.ID).ToArray();


                List<LambdaExpression> lamdas = new List<LambdaExpression>();
                lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.PreSwapDecHeadListViewModel, bool>>)(t => currentDecHeadIDs.Contains(t.DecHeadID)));
                //var selectedDecHeadList = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.UnSwapDecHeadListView.GetAll(lamdas.ToArray()).ToList();
                var selectedDecHeadList = new Needs.Ccs.Services.Views.PreSwapDecHeadListView().GetResults(lamdas.ToArray(), SwapNoticeID);

                var countryList = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.SwapLimitCountries.AsQueryable()
                            .Where(item => item.BankID == BankID && item.Status == Needs.Ccs.Services.Enums.Status.Normal)
                            .ToList();

                var cleanIDs = string.Empty;
                StringBuilder sbMessage = new StringBuilder();
                var total = 0M;

                if (countryList != null && countryList.Any())
                {
                    //该银行有黑名单国家

                    var allDecList = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DecOriginList.Where(t => currentDecHeadIDs.Contains(t.DeclarationID)).ToList();

                    bool isReach = false;

                    foreach (var selectedDecHead in selectedDecHeadList)
                    {
                        var thisDecHeadDecList = allDecList.Where(t => t.DeclarationID == selectedDecHead.DecHeadID);
                        if (thisDecHeadDecList.Any(t => countryList.Select(c => c.Code).Contains(t.OriginCountry)))
                        {
                            isReach = true;

                            sbMessage.Append(string.Concat("报关单<label style=\"color:green\">", selectedDecHead.ContrNo, "</label> 有", BankName, "换汇黑名单国家 <label style=\"color:red\">"));
                            sbMessage.Append(string.Join("，", countryList.Where(t => thisDecHeadDecList.Select(c => c.OriginCountry).Contains(t.Code)).Select(t => t.Name).ToArray()));
                            sbMessage.Append("</label>，将被移除<br/>");
                        }
                        else
                        {
                            total += selectedDecHead.SwapedAmount.ToRound(2);   //(selectedDecHead.SwapAmount - selectedDecHead.SwapedAmount).ToRound(2);
                            cleanIDs += selectedDecHead.DecHeadID + ",";
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
                    //sbMessage.Append("过滤后金额：" + selectedDecHeadList.Sum(t => (t.SwapAmount - t.SwapedAmount).ToRound(2)));
                    sbMessage.Append("过滤后金额：" + selectedDecHeadList.Sum(t => t.SwapedAmount.ToRound(2)));

                    cleanIDs = string.Join(",", selectedDecHeadList.Select(t => t.DecHeadID).ToArray());
                }

                Response.Write((new { success = true, message = sbMessage.ToString(), ids = cleanIDs.Trim(','), }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "错误：" + ex.Message, }).Json());
            }
        }


        protected void ChangeBank()
        {
            try
            {
                string CleanDecHeadIDs = Request.Form["CleanDecHeadIDs"];
                string SwapNoticeID = Request.Form["SwapNoticeID"];
                string BankName = Request.Form["BankName"];

                Needs.Wl.Finance.Services.Models.EditBankHandler editBankHandler = 
                    new Needs.Wl.Finance.Services.Models.EditBankHandler(SwapNoticeID, CleanDecHeadIDs, BankName);

                editBankHandler.Execute();

                Response.Write((new { success = true, message = "", }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "错误：" + ex.Message, }).Json());
            }
        }


    }
}