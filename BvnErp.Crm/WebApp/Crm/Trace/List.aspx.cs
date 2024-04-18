using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.Trace
{

    /// <summary>
    /// 跟踪记录列表页面
    /// </summary>
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                data();
            }
        }

        /// <summary>
        /// 数据加载页面
        /// </summary>
        protected void data()
        {
            string ClientID = Request.QueryString["ClientID"];
            var IsRead = bool.Parse(Request.QueryString["IsRead"] ?? "false");
            this.Model.ClientName = "";
            var client = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.Client>.Create(ClientID);
            if (client!=null)
            {
                this.Model.ClientName = client.Name;
            }
            IEnumerable<NtErp.Crm.Services.Models.Report> report;
            //是否为指定阅读人
            if (IsRead)
            {
                report = Needs.Erp.ErpPlot.Current.ClientSolutions.MyReadReports.Where(item => item.Client.ID == ClientID).ToList();
            }
            else
            {
                report = Needs.Erp.ErpPlot.Current.ClientSolutions.MyReports.Where(c => c.Client.ID == ClientID).ToList();
            }
            this.Model.AllData = report.Select(c => new
            {
                JsonSerializerExtend.JsonTo<NtErp.Crm.Services.Models.Report>(c.Context).Date,
                JsonSerializerExtend.JsonTo<NtErp.Crm.Services.Models.Report>(c.Context).NextDate,
                OriginalStaffs = JsonSerializerExtend.JsonTo<NtErp.Crm.Services.Models.Report>(c.Context).OriginalStaffs ?? string.Empty,
                AdminName = c.Admin.RealName,
                TypeName = JsonSerializerExtend.JsonTo<NtErp.Crm.Services.Models.Report>(c.Context).Type == null ? 
                    string.Empty : JsonSerializerExtend.JsonTo<NtErp.Crm.Services.Models.Report>(c.Context).Type.GetDescription(),
                Context = JsonSerializerExtend.JsonTo<NtErp.Crm.Services.Models.Report>(c.Context).Content,
                IsEdit = (DateTime.Now - c.CreateDate).Days <= 7 && c.Admin.ID == Needs.Erp.ErpPlot.Current.ID,
                c.ID,
                IsOwner = c.Admin.ID == Needs.Erp.ErpPlot.Current.ID,
                Reply = new NtErp.Crm.Services.Views.ReplyAlls().Where(item => item.ReportID == c.ID).OrderBy(a => a.UpdateDate).Select(item => new
                {
                    RealName = item.Admin.RealName,
                    UpdateDate = item.UpdateDate,
                    Context = item.Context
                }).ToArray(),
                File = new NtErp.Crm.Services.Views.FileAlls().Where(item => item.ReportID == c.ID && item.Status == Status.Normal).
                Select(item => new {
                    URL = item.Url,
                    Name = item.Name
                }).ToArray(),
                c.Status,
                c.CreateDate,
                IsLight = JsonSerializerExtend.JsonTo<NtErp.Crm.Services.Models.Report>(c.Context).Date == null ? false : 
                (c.CreateDate - JsonSerializerExtend.JsonTo<NtErp.Crm.Services.Models.Report>(c.Context).Date).Value.Days > 3
                //isLeader = GetStaffs(Needs.Erp.ErpPlot.Current.ID).Contains(c.Admin.ID) //是否为上级人员
            }).Json();
        }

        ///// <summary>
        ///// 获取员工集合
        ///// </summary>
        ///// <param name="AdminID">人员ID</param>
        ///// <returns></returns>
        //private IEnumerable<string> GetStaffs(string AdminID)
        //{
        //    List<string> list = new List<string>() { AdminID };

        //    var admin = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.AdminTop>.Create(AdminID);
        //    if(admin.JobType == NtErp.Crm.Services.Enums.JobType.TPM)
        //    {
        //        return new string[0];
        //    }
        //    else
        //    {
        //        //获取所有员工
        //        var Mystaffids = Needs.Erp.ErpPlot.Current.ClientSolutions.MyStaffs.Select(item => item.ID).ToArray();
        //        return Mystaffids.Except(list);
        //    }
        //}
    }
}