using Needs.Ccs.Services;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.Declare
{
    public partial class AddDecHead : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string CleanIDs = Request.QueryString["CleanIDs"];
            this.Model.CleanIDs = CleanIDs;
        }

        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string CleanIDs = Request.QueryString["CleanIDs"];
            string strIsExpssive = Request.QueryString["IsExpssive"];
            string strIsInExpssive = Request.QueryString["IsInExpensive"];

            string[] CleanIDs_Array = { };
            if (!string.IsNullOrEmpty(CleanIDs))
            {
                CleanIDs_Array = CleanIDs.Split(',');
            }

            List<LambdaExpression> lamdas = new List<LambdaExpression>();
            lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.UnSwapDecHeadListView.UnSwapDecHeadListModel, bool>>)(t => !CleanIDs_Array.Contains(t.DecHeadID)));

            int DecHeadType = int.Parse(Request.QueryString["DecHeadType"]);
            bool 是至少一种特殊报关单 = false;
            if (DecHeadType == 0) //全部
            {

            }
            else if (DecHeadType == 1) //特殊报关单
            {
                是至少一种特殊报关单 = true;
            }
            else if (DecHeadType == 2) //普通报关单
            {
                lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.UnSwapDecHeadListView.UnSwapDecHeadListModel, bool>>)(
                                        t => t.IsOverDate == false && t.IsHasLimitArea == false));
            }

            int ClientType = int.Parse(Request.QueryString["ClientType"]);

            if (ClientType != 0)
            {
                Expression<Func<Needs.Ccs.Services.Views.UnSwapDecHeadListView.UnSwapDecHeadListModel, bool>> lambda = item => item.ClientType == (Needs.Ccs.Services.Enums.ClientType)ClientType;
                lamdas.Add(lambda);
            }

            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];
            string OwnerName = Request.QueryString["OwnerName"];

            if (!string.IsNullOrEmpty(StartDate))
            {
                DateTime start = Convert.ToDateTime(StartDate);
                lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.UnSwapDecHeadListView.UnSwapDecHeadListModel, bool>>)(t => t.DDate >= start));
            }
            if (!string.IsNullOrEmpty(EndDate))
            {
                DateTime end = Convert.ToDateTime(EndDate).AddDays(1);
                lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.UnSwapDecHeadListView.UnSwapDecHeadListModel, bool>>)(t => t.DDate < end));
            }
            if (!string.IsNullOrEmpty(OwnerName))
            {
                OwnerName = OwnerName.Trim();
                //lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.UnSwapDecHeadListView.UnSwapDecHeadListModel, bool>>)(t => t.OwnerName.Contains(OwnerName)));
                //使用精确查找
                lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.UnSwapDecHeadListView.UnSwapDecHeadListModel, bool>>)(t => t.OwnerName == OwnerName));
            }

            if ((strIsInExpssive == "true" && strIsExpssive == "true") || ((strIsInExpssive == "false" && strIsExpssive == "false")))
            {
                lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.UnSwapDecHeadListView.UnSwapDecHeadListModel, bool>>)(t => t.IsExcessive == true || t.IsExcessive == false));

            }
            else
            {
                bool IsExpssive;
                if ((!string.IsNullOrEmpty(strIsExpssive)))
                {
                    bool.TryParse(strIsExpssive, out IsExpssive);
                    if (IsExpssive)
                        lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.UnSwapDecHeadListView.UnSwapDecHeadListModel, bool>>)(t => t.IsExcessive == true));
                }

                bool IsInExpssive;
                if (!string.IsNullOrEmpty(strIsInExpssive))
                {
                    bool.TryParse(strIsInExpssive, out IsInExpssive);
                    if (IsInExpssive)
                        lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.UnSwapDecHeadListView.UnSwapDecHeadListModel, bool>>)(t => t.IsExcessive == false));

                }
            }

            int totalCount = 0;
            var unSwapDecHeadList = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.UnSwapDecHeadListView.GetResult(out totalCount, page, rows, lamdas.ToArray(), 是至少一种特殊报关单);

            Func<Needs.Ccs.Services.Views.UnSwapDecHeadListView.UnSwapDecHeadListModel, object> convert = unSwapDecHead => new
            {
                ID = unSwapDecHead.DecHeadID,
                ContrNo = unSwapDecHead.ContrNo,
                OrderID = unSwapDecHead.OrderID,
                OwnerName = unSwapDecHead.OwnerName,
                EntryID = unSwapDecHead.EntryId,
                Currency = unSwapDecHead.Currency,
                SwapAmount = unSwapDecHead.SwapAmount,
                DDate = unSwapDecHead.DDate?.ToString("yyyy-MM-dd HH:mm:ss"),
                SwapStatus = unSwapDecHead.SwapStatus.GetDescription(),
                URL = Needs.Utils.FileDirectory.Current.FileServerUrl + @"/" + unSwapDecHead?.Url.ToUrl(),
                DecHeadStatus = MultiEnumUtils.ToText<Needs.Ccs.Services.Enums.CusDecStatus>(
                    (Needs.Ccs.Services.Enums.CusDecStatus)Enum.Parse(typeof(Needs.Ccs.Services.Enums.CusDecStatus), unSwapDecHead.CusDecStatus)),
                SwapedAmount = unSwapDecHead.SwapedAmount,
                UnSwapedAmount = unSwapDecHead.SwapAmount - unSwapDecHead.SwapedAmount,
                UserCurrentPayApply = unSwapDecHead.UserCurrentPayApply.ToRound(2),
                SwapSpecialInfo = GetSwapSpecialInfo(unSwapDecHead),
            };

            Response.Write(new
            {
                rows = unSwapDecHeadList.Select(convert).ToArray(),
                total = totalCount,
            }.Json());
        }

        /// <summary>
        /// 获取特殊报关单信息
        /// </summary>
        /// <param name="unSwapDecHead"></param>
        /// <returns></returns>
        private string GetSwapSpecialInfo(Needs.Ccs.Services.Views.UnSwapDecHeadListView.UnSwapDecHeadListModel unSwapDecHead)
        {
            List<string> listSpecialInfo = new List<string>();

            //if (unSwapDecHead.DDate != null && (DateTime.Now - unSwapDecHead.DDate.Value).Days > 90)
            if (unSwapDecHead.IsOverDate)
            {
                listSpecialInfo.Add("超过90天");
            }

            if (unSwapDecHead.IsHasLimitArea)
            {
                listSpecialInfo.Add("有受限地区（" + string.Join("、", unSwapDecHead.LimitBankNames) + "）");
            }

            if (listSpecialInfo == null || !listSpecialInfo.Any())
            {
                return string.Empty;
            }

            return string.Join("、", listSpecialInfo.ToArray());
        }


    }
}
