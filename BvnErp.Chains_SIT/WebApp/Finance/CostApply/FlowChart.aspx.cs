using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.CostApply
{
    public partial class FlowChart : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string CostApplyID = Request.QueryString["CostApplyID"];
            this.Model.CostApplyID = CostApplyID;
        }

        private ShowFlowInfo GetShowFlowInfoByLog(Needs.Ccs.Services.Views.CostApplyLogsViewModel item)
        {
            ShowFlowInfo showFlowInfo = new ShowFlowInfo();
            showFlowInfo.Color = "#6eda6e";

            string CostApplyFinanceStaffName = ConfigurationManager.AppSettings["CostApplyFinanceStaffName"];
            string CostApplyManagerName = ConfigurationManager.AppSettings["CostApplyManagerName"];

            switch (item.CurrentCostStatus)
            {
                case Needs.Ccs.Services.Enums.CostStatusEnum.Cancel:
                    break;
                case Needs.Ccs.Services.Enums.CostStatusEnum.UnSubmit:
                    showFlowInfo.Title = "申请人：";
                    showFlowInfo.AdminName = item.AdminName;
                    if (item.NextCostStatus == Needs.Ccs.Services.Enums.CostStatusEnum.Cancel)
                    {
                        showFlowInfo.ResultStr = "已撤销";
                        showFlowInfo.Color = "#6eda6e";
                    }
                    else if (item.NextCostStatus == Needs.Ccs.Services.Enums.CostStatusEnum.FinanceStaffUnApprove)
                    {
                        showFlowInfo.ResultStr = "已申请";
                        showFlowInfo.Color = "#6eda6e";
                    }
                    break;
                case Needs.Ccs.Services.Enums.CostStatusEnum.FinanceStaffUnApprove:
                    showFlowInfo.Title = "财务负责人：";
                    showFlowInfo.AdminName = CostApplyFinanceStaffName;
                    if (item.NextCostStatus == Needs.Ccs.Services.Enums.CostStatusEnum.UnSubmit)
                    {
                        showFlowInfo.ResultStr = "拒绝";
                        showFlowInfo.Color = "#ff4804";
                    }
                    else if (item.NextCostStatus == Needs.Ccs.Services.Enums.CostStatusEnum.ManagerUnApprove)
                    {
                        showFlowInfo.ResultStr = "通过";
                        showFlowInfo.Color = "#6eda6e";
                    }
                    break;
                case Needs.Ccs.Services.Enums.CostStatusEnum.ManagerUnApprove:
                    showFlowInfo.Title = "总经理：";
                    showFlowInfo.AdminName = CostApplyManagerName;
                    if (item.NextCostStatus == Needs.Ccs.Services.Enums.CostStatusEnum.UnSubmit)
                    {
                        showFlowInfo.ResultStr = "拒绝";
                        showFlowInfo.Color = "#ff4804";
                    }
                    else if (item.NextCostStatus == Needs.Ccs.Services.Enums.CostStatusEnum.UnPay)
                    {
                        showFlowInfo.ResultStr = "通过";
                        showFlowInfo.Color = "#6eda6e";
                    }
                    break;
                case Needs.Ccs.Services.Enums.CostStatusEnum.UnPay:
                    showFlowInfo.Title = "出纳：";
                    showFlowInfo.AdminName = item.AdminName;
                    if (item.NextCostStatus == Needs.Ccs.Services.Enums.CostStatusEnum.PaySuccess)
                    {
                        showFlowInfo.ResultStr = "通过";
                        showFlowInfo.Color = "#6eda6e";
                    }
                    break;
                case Needs.Ccs.Services.Enums.CostStatusEnum.PaySuccess:
                    break;
                default:
                    break;
            }

            return showFlowInfo;
        }

        private List<object> GetFutureSteps(bool isNew, Needs.Ccs.Services.Enums.CostStatusEnum nowCostStatus, string firstFlowAdminName)
        {
            //var currentAdmin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
            var currentAdmin = new Needs.Ccs.Services.Views.AdminsTopView2().Where(t => t.OriginID == Needs.Wl.Admin.Plat.AdminPlat.Current.ID).FirstOrDefault();

            string CostApplyFinanceStaffName = ConfigurationManager.AppSettings["CostApplyFinanceStaffName"];
            string CostApplyManagerName = ConfigurationManager.AppSettings["CostApplyManagerName"];

            List<object> futureSteps = new List<object>();

            switch (nowCostStatus)
            {
                case Needs.Ccs.Services.Enums.CostStatusEnum.Cancel:
                    break;
                case Needs.Ccs.Services.Enums.CostStatusEnum.UnSubmit:
                    futureSteps.Add(new
                    {
                        Title = "申请人：" + (isNew ? currentAdmin.RealName : firstFlowAdminName),
                        Operation = "",
                        Time = "",
                        Summary = "",
                        IsCurrent = true,
                        Color = "#bbffee",
                    });
                    futureSteps.Add(new
                    {
                        Title = "财务负责人：" + CostApplyFinanceStaffName,
                        Operation = "",
                        Time = "",
                        Summary = "",
                        IsCurrent = false,
                        Color = "#e4e3e3",
                    });
                    futureSteps.Add(new
                    {
                        Title = "总经理：" + CostApplyManagerName,
                        Operation = "",
                        Time = "",
                        Summary = "",
                        IsCurrent = false,
                        Color = "#e4e3e3",
                    });
                    futureSteps.Add(new
                    {
                        Title = "出纳",
                        Operation = "",
                        Time = "",
                        Summary = "",
                        IsCurrent = false,
                        Color = "#e4e3e3",
                    });
                    break;
                case Needs.Ccs.Services.Enums.CostStatusEnum.FinanceStaffUnApprove:
                    futureSteps.Add(new
                    {
                        Title = "财务负责人：" + CostApplyFinanceStaffName,
                        Operation = "",
                        Time = "",
                        Summary = "",
                        IsCurrent = true,
                        Color = "#bbffee",
                    });
                    futureSteps.Add(new
                    {
                        Title = "总经理：" + CostApplyManagerName,
                        Operation = "",
                        Time = "",
                        Summary = "",
                        IsCurrent = false,
                        Color = "#e4e3e3",
                    });
                    futureSteps.Add(new
                    {
                        Title = "出纳",
                        Operation = "",
                        Time = "",
                        Summary = "",
                        IsCurrent = false,
                        Color = "#e4e3e3",
                    });
                    break;
                case Needs.Ccs.Services.Enums.CostStatusEnum.ManagerUnApprove:
                    futureSteps.Add(new
                    {
                        Title = "总经理：" + CostApplyManagerName,
                        Operation = "",
                        Time = "",
                        Summary = "",
                        IsCurrent = true,
                        Color = "#bbffee",
                    });
                    futureSteps.Add(new
                    {
                        Title = "出纳",
                        Operation = "",
                        Time = "",
                        Summary = "",
                        IsCurrent = false,
                        Color = "#e4e3e3",
                    });
                    break;
                case Needs.Ccs.Services.Enums.CostStatusEnum.UnPay:
                    futureSteps.Add(new
                    {
                        Title = "出纳",
                        Operation = "",
                        Time = "",
                        Summary = "",
                        IsCurrent = true,
                        Color = "#bbffee",
                    });
                    break;
                case Needs.Ccs.Services.Enums.CostStatusEnum.PaySuccess:
                    break;
                default:
                    break;
            }

            return futureSteps;
        }

        /// <summary>
        /// 费用申请日志
        /// </summary>
        protected void CostApplyLogs()
        {
            string CostApplyID = Request.Form["CostApplyID"];

            var logs = new Needs.Ccs.Services.Views.CostApplyLogsView().GetResults(CostApplyID);

            Func<Needs.Ccs.Services.Views.CostApplyLogsViewModel, object> convert = item => new
            {
                Title = GetShowFlowInfoByLog(item).Title + GetShowFlowInfoByLog(item).AdminName,
                Operation = GetShowFlowInfoByLog(item).ResultStr,
                Time = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Summary = string.IsNullOrEmpty(item.Reason) ? string.Empty : "备注：" + item.Reason,
                IsCurrent = false,
                Color = GetShowFlowInfoByLog(item).Color,
            };

            var responseResult = logs.Select(convert).ToList();
            if (logs == null || !logs.Any())
            {
                var futureSteps = GetFutureSteps(true, Needs.Ccs.Services.Enums.CostStatusEnum.UnSubmit, string.Empty);
                responseResult.AddRange(futureSteps);
            }
            else
            {
                var futureSteps = GetFutureSteps(false, logs.LastOrDefault().NextCostStatus, logs.FirstOrDefault().AdminName);
                responseResult.AddRange(futureSteps);
            }
            

            Response.Write(new { rows = responseResult, }.Json());


            /*
            List<object> flows = new List<object>();
            flows.Add(new
            {
                Title = "申请人：荣检",
                Operation = "已申请",
                Time = "2020-04-15 10:10:20",
                Summary = "",
                IsCurrent = false,
                Color = "#6eda6e",
            });

            flows.Add(new
            {
                Title = "财务负责人：施思静",
                Operation = "通过",
                Time = "2020-04-15 11:25:32",
                Summary = "备注：同意。。。",
                IsCurrent = false,
                Color = "#6eda6e",
            });

            flows.Add(new
            {
                Title = "总经理：张庆永",
                Operation = "拒绝",
                Time = "2020-04-15 13:02:45",
                Summary = "备注：发票xxxxxxxxxx。请重新请重新请重新请重新请重新请重新请重新请重新请重新请重新",
                IsCurrent = false,
                Color = "#ff4804",
            });

            flows.Add(new
            {
                Title = "申请人：荣检",
                Operation = "已申请",
                Time = "2020-04-16 09:32:10",
                Summary = "",
                IsCurrent = false,
                Color = "#6eda6e",
            });

            flows.Add(new
            {
                Title = "财务负责人：施思静",
                Operation = "通过",
                Time = "2020-04-16 14:12:05",
                Summary = "备注：通过",
                IsCurrent = false,
                Color = "#6eda6e",
            });

            flows.Add(new
            {
                Title = "总经理：张庆永",
                Operation = "通过",
                Time = "2020-04-16 14:20:35",
                Summary = "备注：票据ok",
                IsCurrent = false,
                Color = "#6eda6e",
            });

            flows.Add(new
            {
                Title = "出纳：杨艳梦",
                Operation = "",
                Time = "",
                Summary = "",
                IsCurrent = true,
                Color = "#bbffee",
            });
            */

            /*
            List<object> flows = new List<object>();
            flows.Add(new
            {
                Title = "申请人：荣检",
                Operation = "",
                Time = "",
                Summary = "",
                IsCurrent = true,
                Color = "#bbffee",
            });
            flows.Add(new
            {
                Title = "财务负责人：施思静",
                Operation = "",
                Time = "",
                Summary = "",
                IsCurrent = false,
                Color = "#e4e3e3",
            });
            flows.Add(new
            {
                Title = "总经理：张庆永",
                Operation = "",
                Time = "",
                Summary = "",
                IsCurrent = false,
                Color = "#e4e3e3",
            });
            flows.Add(new
            {
                Title = "出纳：杨艳梦",
                Operation = "",
                Time = "",
                Summary = "",
                IsCurrent = false,
                Color = "#e4e3e3",
            });
            */

            //Response.Write(new { rows = flows.ToArray(), }.Json());
        }

    }

    public class ShowFlowInfo
    {
        /// <summary>
        /// 抬头
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 结果字符串：已申请/通过/拒绝
        /// </summary>
        public string ResultStr { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string Color { get; set; } = string.Empty;

        /// <summary>
        /// 操作人
        /// </summary>
        public string AdminName { get; set; } = string.Empty;
    }
}