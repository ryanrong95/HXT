using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using Yahv.Erm.Services;
using Yahv.Web.Erp;
using Yahv.Underly;
using Yahv.Linq.Extends;
using Yahv.Web.Controls.Easyui;
using Yahv.Web.Forms;
using Yahv.Erm.Services.Models.Origins;
using Layers.Data;
using Yahv.Utils;
using System.Data;
using Yahv.Utils.Serializers;
using System.Linq.Expressions;

namespace Yahv.Erm.WebApp.Erm_KQ.Application_ArchiveLending
{
    public partial class ListOfMy : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadComboBoxData();
            }
        }

        protected void LoadComboBoxData()
        {
            this.Model.Status = ExtendsEnum.ToDictionary<Services.ApplicationStatus>().Select(item => new { Value = item.Key, Text = item.Value });

            this.Model.CurrentAdmin = new
            {
                ID = Erp.Current.IsSuper ? "" : Erp.Current.ID,
            };
        }

        /// <summary>
        /// 数据查询
        /// </summary>
        /// <returns></returns>
        protected object data()
        {
            Expression<Func<Application, bool>> expression = Predicate();
            int page = int.Parse(Request.QueryString["page"]);
            int rows = int.Parse(Request.QueryString["rows"]);

            var applications = Erp.Current.Erm.ApplicationsRoll
                .Where(item => item.ApplicationType == Services.ApplicationType.ArchiveLending)
                .Where(expression);
            if (!Erp.Current.IsSuper)
            {
                applications = applications.Where(item => item.ApplicantID == Erp.Current.ID);
            }

            var data = applications.OrderByDescending(item => item.CreateDate).ToArray().Select(item => new
            {
                item.ID,
                item.ApplicantID,
                ApplicantName = item.Applicant.RealName,
                DepartmentType = item.ArchiveLendingContext.DepartmentName,
                Manager = item.ArchiveLendingContext.ApproveName,
                BorrowDate = item.ArchiveLendingContext.BorrowDate.ToString("yyyy-MM-dd"),
                ReturnDate = item.ArchiveLendingContext.ReturnDate.ToString("yyyy-MM-dd"),
                Reason = item.ArchiveLendingContext.Reason,
                Count = item.ArchiveLendingContext.Count,
                ArchiveName = item.ArchiveLendingContext.ArchiveName,
                Status = item.ApplicationStatus,
                StatusDec = item.ApplicationStatus.GetDescription(),
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd"),
                StepName = item.ApplicationStatus == Services.ApplicationStatus.UnderApproval ? item.CurrentVoteStep?.VoteStep?.Name : "--",
                AdminName = item.ApplicationStatus == Services.ApplicationStatus.UnderApproval ? item.CurrentVoteStep?.Admin?.RealName : "--",
            });

            return new
            {
                rows = data.Skip((page - 1) * rows).Take(rows),
                total = data.Count(),
            };
        }
        Expression<Func<Application, bool>> Predicate()
        {
            Expression<Func<Application, bool>> predicate = item => true;

            //查询参数
            var Status = Request.QueryString["Status"];

            if (!string.IsNullOrWhiteSpace(Status))
            {
                var status = ((Services.ApplicationStatus)Enum.Parse(typeof(Services.ApplicationStatus), Status));
                predicate = predicate.And(item => item.ApplicationStatus == status);
            }
            return predicate;
        }

        /// <summary>
        /// 删除
        /// </summary>
        protected void Delete()
        {
            string id = Request.Form["ID"];
            var del = Erp.Current.Erm.Applications[id];
            if (del != null)
            {
                del.Abandon();
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "加班申请",
                    $"删除", del.Json());
            }
        }    

    }
}