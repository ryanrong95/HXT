using Layer.Data.Sqls;
using NtErp.Crm.Services.Enums;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using NtErp.Crm.Services;
using NtErp.Crm.Services.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;

namespace WebApi.Controllers
{
    public class ApiClientReportController : BaseController
    {
        /// <summary>
        /// 获取跟踪记录
        /// </summary>
        /// <param name="ClientID">客户主键</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Get(string ClientID)
        {
            if (!IsAgree)
            {
                return Result(new { result = false });
            }
            using (BvCrmReponsitory reponsitory = new BvCrmReponsitory())
            {
                var reports = new ReportsAlls(reponsitory).Where(item=>item.Status == Status.Normal);
                if (string.IsNullOrWhiteSpace(ClientID))
                {
                    var clients = new MyClientBaseView(this.Admin, reponsitory).Where(item => item.Status == ActionStatus.Complete 
                        || item.Status == ActionStatus.Auditing);
                    reports = from report in reports
                              join client in clients on report.Client.ID equals client.ID
                              select report;
                }
                else
                {
                    reports = reports.Where(item => item.Client.ID == ClientID);
                }
                var data = reports.Select(item => new
                {
                    item.ID,
                    ClientID = item.Client.ID,
                    AdminName = item.Admin.RealName,
                    Type = JsonSerializerExtend.JsonTo<NtErp.Crm.Services.Models.Report>(item.Context).Type.GetDescription(),
                    JsonSerializerExtend.JsonTo<NtErp.Crm.Services.Models.Report>(item.Context).Date,
                    JsonSerializerExtend.JsonTo<NtErp.Crm.Services.Models.Report>(item.Context).NextDate,
                    Context = JsonSerializerExtend.JsonTo<NtErp.Crm.Services.Models.Report>(item.Context).Content,
                    item.CreateDate,
                    item.UpdateDate,
                });
                return Result(data);
            }
        }

        /// <summary>
        /// 跟踪记录保存
        /// </summary>
        /// <param name="ID">主键ID</param>
        /// <param name="ClientID">客户ID</param>
        /// <param name="Context">记录内容</param>
        /// <param name="Type">跟进方式</param>
        /// <param name="Date">跟踪时间</param>
        /// <param name="NextDate">下一次跟进时间</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Save(string ID, string ClientID, string Context, string Type, string Date, string NextDate)
        {
            if (!IsAgree)
            {
                return Result(new { IsSuccess = false, Message = "没有授权！" });
            }
            using (BvCrmReponsitory reponsitory = new BvCrmReponsitory())
            {
                try
                {

                    var report = new ReportsAlls(reponsitory)[ID] ?? new NtErp.Crm.Services.Models.Report();
                    if (string.IsNullOrWhiteSpace(report.ID))
                    {
                        report.ID = Needs.Overall.PKeySigner.Pick(PKeyType.Report);
                        reponsitory.Insert(new Layer.Data.Sqls.BvCrm.Reports
                        {
                            ID = report.ID,
                            ClientID = ClientID,
                            AdminID = this.Admin.ID,
                            CreateDate = DateTime.Now,
                            UpdateDate = DateTime.Now,
                            Status = (int)Status.Normal,
                            Context = (new
                            {
                                Type = int.Parse(Type),
                                Date = DateTime.Parse(Date),
                                NextDate = DateTime.Parse(NextDate),
                                Content = Context,
                            }).Json(),
                        });
                    }
                    else
                    {
                        reponsitory.Update<Layer.Data.Sqls.BvCrm.Reports>(new
                        {
                            ClientID = ClientID,
                            AdminID = this.Admin.ID,
                            UpdateDate = DateTime.Now,
                            Context = (new
                            {
                                Type = int.Parse(Type),
                                Date = DateTime.Parse(Date),
                                NextDate = DateTime.Parse(NextDate),
                                Content = Context,
                            }).Json(),
                        }, item => item.ID == report.ID);
                    }
                    return Result(new { IsSuccess = true, Message = "保存成功！", Data = new { ID = report.ID } });
                }
                catch (Exception ex)
                {
                    return Result(new { IsSuccess = false, Message = ex.Message });
                }
            }
        }
    }
}