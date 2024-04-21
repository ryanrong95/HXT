using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Erm.Services;
using Yahv.Erm.Services.Models.Rolls;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using Application = Yahv.Erm.Services.Models.Origins.Application;
using ApplicationType = Yahv.Erm.Services.ApplicationType;

namespace Yahv.Erm.WebApp.Erm_KQ.Application_Resignation
{
    public partial class ListOfMy : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitData();
            }
        }

        #region 加载数据
        private void InitData()
        {
            this.Model.Status =
                ExtendsEnum.ToDictionary<Services.ApplicationStatus>().Select(item => new { text = item.Value, value = item.Key });
        }
        #endregion

        #region 获取数据
        /// <summary>
        /// 列表数据
        /// </summary>
        /// <returns></returns>
        protected object data()
        {
            var query = Erp.Current.Erm.ApplicationsRoll.Where(GetExpression()).ToArray();
            return this.Paging(query.OrderByDescending(item => item.CreateDate), item => new
            {
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd"),
                item.Title,
                StatusName = item.ApplicationStatus.GetDescription(),
                item.ApplicationStatus,
                item.ApplicantID,
                item.CreatorID,
                item.ID,
                Entity = item.Context.JsonTo<Resignation>(),
                StepName = item.ApplicationStatus == Services.ApplicationStatus.UnderApproval ? item.CurrentVoteStep?.VoteStep?.Name : "--",
                AdminName = item.ApplicationStatus == Services.ApplicationStatus.UnderApproval ? item.CurrentVoteStep?.Admin?.RealName : "--",
            });
        }
        #endregion

        #region 功能函数

        #region 功能函数
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
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "离职申请",
                    $"删除", del.Json());
            }
        }
        #endregion

        /// <summary>
        /// 下载交接表模板
        /// </summary>
        protected void btn_handover_Click(object sender, EventArgs e)
        {
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Content\") + "Template\\files\\员工离职交接表.docx";
            DownLoadFile(fileName);
        }

        /// <summary>
        /// 下载申请表模板
        /// </summary>
        protected void btn_apply_Click(object sender, EventArgs e)
        {
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Content\") + "Template\\files\\员工离职申请表.xlsx";
            DownLoadFile(fileName);
        }
        #endregion

        #region 私有函数
        /// <summary>
        /// 获取查询条件
        /// </summary>
        /// <returns></returns>
        private Expression<Func<Application, bool>> GetExpression()
        {
            Expression<Func<Application, bool>> predicate = item => item.ApplicationType == ApplicationType.Leave;
            predicate = predicate.And(item => item.ApplicantID == Erp.Current.ID);

            string keyword = Request.QueryString["s_keyword"];
            string status = Request.QueryString["s_status"];

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                predicate = predicate.And(item => item.Title.Contains(keyword));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                predicate = predicate.And(item => (int)item.ApplicationStatus == int.Parse(status));
            }

            return predicate;
        }
        #endregion
    }
}