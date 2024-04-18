using Layer.Data.Sqls;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Views;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;

namespace WebApi.Controllers
{
    public class ApiWorkWeeklyController : BaseController
    {
        /// <summary>
        /// 获取管理员的周报
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Get(string WeeklyID)
        {
            if (!IsAgree)
            {
                return Result(new { result = false });
            }
            using (BvCrmReponsitory reponsitory = new BvCrmReponsitory())
            {
                var worksweeeklys = new WorksWeeklyAlls(reponsitory).Where(item => item.Admin.ID == this.Admin.ID && item.Status != Status.Delete);
                if (!string.IsNullOrWhiteSpace(WeeklyID))
                {
                    worksweeeklys = worksweeeklys.Where(item => item.ID == WeeklyID);
                }
                var replys = new ReplyAlls(reponsitory);
                var data = from worksweekly in worksweeeklys
                           join reply in replys on worksweekly.ID equals reply.WorksWeeklyID into replies
                           select new
                           {
                               worksweekly.ID,
                               worksweekly.WeekOfYear,
                               worksweekly.Context,
                               worksweekly.CreateDate,
                               worksweekly.UpdateDate,
                               Replies = replies.Select(item => new
                               {
                                   AdminName = item.Admin.RealName,
                                   item.Context,
                                   item.UpdateDate,
                               }).ToArray(),
                           };
                return Result(data);
            }
        }

        /// <summary>
        /// 获取下属员工的周报
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetStaff(string WeeklyID)
        {
            if (!IsAgree)
            {
                return Result(new { result = false });
            }
            using (BvCrmReponsitory reponsitory = new BvCrmReponsitory())
            {
                //获取当前周
                int weekofyear = Get_WeekOfYear(DateTime.Now, new CultureInfo("zh-CN"));

                var mystaffid = new MyStaffsView(this.Admin, reponsitory).Select(item => item.ID).ToArray().Except(new string[] { this.Admin.ID });
                var worksweeeklys = new WorksWeeklyAlls(reponsitory).Where(item => mystaffid.Contains(item.Admin.ID) && item.Status != Status.Delete).
                    Where(item => item.WeekOfYear > weekofyear - 4);
                if (!string.IsNullOrWhiteSpace(WeeklyID))
                {
                    worksweeeklys = worksweeeklys.Where(item => item.ID == WeeklyID);
                }
                var replys = new ReplyAlls(reponsitory);
                var data = from worksweekly in worksweeeklys
                           join reply in replys on worksweekly.ID equals reply.WorksWeeklyID into replies
                           select new
                           {
                               worksweekly.ID,
                               worksweekly.WeekOfYear,
                               worksweekly.Context,
                               worksweekly.CreateDate,
                               worksweekly.UpdateDate,
                               AdminID = worksweekly.Admin.ID,
                               AdminName = worksweekly.Admin.RealName,
                               Replies = replies.Select(item => new
                               {
                                   AdminName = item.Admin.RealName,
                                   item.Context,
                                   item.UpdateDate,
                               }).ToArray(),
                           };
                return Result(data);
            }
        }

        private int Get_WeekOfYear(DateTime dt, CultureInfo ci)
        {
            return ci.Calendar.GetWeekOfYear(dt, ci.DateTimeFormat.CalendarWeekRule, ci.DateTimeFormat.FirstDayOfWeek);
        }

        /// <summary>
        /// 工作周报保存
        /// </summary>
        /// <param name="ID">主键</param>
        /// <param name="Context">内容</param>
        /// <param name="WeekOfYear">周</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Save(string ID, string Context, string WeekOfYear)
        {
            if (!IsAgree)
            {
                return Result(new { IsSuccess = false, Message = "没有授权！" });
            }
            using (BvCrmReponsitory reponsitory = new BvCrmReponsitory())
            {
                try
                {
                    var workweekly = new WorksWeeklyAlls(reponsitory)[ID] ?? new NtErp.Crm.Services.Models.WorksWeekly();
                    if (string.IsNullOrWhiteSpace(workweekly.ID))
                    {
                        workweekly.ID = Needs.Overall.PKeySigner.Pick(NtErp.Crm.Services.PKeyType.Weekly);
                        reponsitory.Insert(new Layer.Data.Sqls.BvCrm.WorksWeekly
                        {
                            ID = workweekly.ID,
                            WeekOfYear = int.Parse(WeekOfYear),
                            Context = Context,
                            AdminID = this.Admin.ID,
                            CreateDate = DateTime.Now,
                            UpdateDate = DateTime.Now,
                            Status = (int)Status.Normal,
                        });
                    }
                    else
                    {
                        reponsitory.Update<Layer.Data.Sqls.BvCrm.WorksWeekly>(new
                        {
                            WeekOfYear = int.Parse(WeekOfYear),
                            Context = Context,
                            AdminID = this.Admin.ID,
                            UpdateDate = DateTime.Now,
                        }, item => item.ID == workweekly.ID);
                    }
                    return Result(new { IsSuccess = true, Message = "保存成功！", Data = new { ID = workweekly.ID } });
                }
                catch (Exception ex)
                {
                    return Result(new { IsSuccess = false, Message = ex.Message });
                }
            }
        }

        /// <summary>
        /// 保存点评
        /// </summary>
        /// <param name="WeeklyID"></param>
        /// <param name="Context"></param>
        /// <returns></returns>
        public ActionResult SaveReply(string WeeklyID, string Context)
        {
            if (!IsAgree)
            {
                return Result(new { IsSuccess = false, Message = "没有授权！" });
            }
            using (BvCrmReponsitory reponsitory = new BvCrmReponsitory())
            {
                try
                {
                    var WorkWeekly = new WorksWeeklyAlls(reponsitory)[WeeklyID];
                    var id = Guid.NewGuid().ToString();
                    reponsitory.Insert(new Layer.Data.Sqls.BvCrm.Replies
                    {
                        ID = id,
                        WorksOtherID = null,
                        WorksWeeklyID = WorkWeekly.ID,
                        ReportID = null,
                        Context = Context,
                        AdminID = this.Admin.ID,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now
                    });
                    return Result(new { IsSuccess = true, Message = "保存成功！", Data = new { ID = id } });
                }
                catch (Exception ex)
                {
                    return Result(new { IsSuccess = false, Message = ex.Message });
                }
            }
        }
    }
}