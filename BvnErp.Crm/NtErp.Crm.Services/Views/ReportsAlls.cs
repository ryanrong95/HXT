using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Utils.Converters;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Views
{
    public class ReportsAlls : UniqueView<Report, BvCrmReponsitory>, Needs.Underly.IFkoView<Report>
    {
        /// <summary>
        /// 无参构造函数
        /// </summary>
        public ReportsAlls()
        {

        }

        public ReportsAlls(BvCrmReponsitory reponsitory) : base(reponsitory)
        {

        }

        /// <summary>
        /// 获取追踪记录数据集合
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Report> GetIQueryable()
        {
            ClientAlls clientview = new ClientAlls(this.Reponsitory); //客户视图
            ProjectAlls projectalls = new ProjectAlls(this.Reponsitory);
            AdminTopView adminview = new AdminTopView(this.Reponsitory); //人员视图

            return from report in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Reports>()
                   join admin in adminview on report.AdminID equals admin.ID
                   join client in clientview on report.ClientID equals client.ID into clients
                   from _client in clients.DefaultIfEmpty()
                   join project in projectalls on report.ProjectID equals project.ID into projects
                   from _project in projects.DefaultIfEmpty()
                   select new Report
                   {
                       ID = report.ID,
                       Admin = admin,
                       CreateDate = report.CreateDate,
                       UpdateDate = report.UpdateDate,
                       Context = report.Context,
                       Client = _client,
                       Project = _project,
                       Status = (Status)report.Status,
                   };
        }

        /// <summary>
        /// 附件删除
        /// </summary>
        /// <param name="Reportid">跟踪记录ID</param>
        virtual public void DeleteFiles(string Reportid)
        {
            var report = this[Reportid];
            if (report != null)
            {
                Reponsitory.Update<Layer.Data.Sqls.BvCrm.Files>(new
                {
                    Status = Status.Delete
                }, item => item.ReportID == report.ID);
            }
        }

        /// <summary>
        /// 绑定指定阅读人
        /// </summary>
        /// <param name="report">报告对象</param>
        /// <param name="Reader">阅读人</param>
        virtual public void BindingReader(Report report, AdminTop reader)
        {
            if (reader == null)
            {
                return;
            }
            Reponsitory.Insert(new Layer.Data.Sqls.BvCrm.MapsReport()
            {
                ID = string.Concat(report.ID, reader.ID).MD5(),
                ReportID = report.ID,
                ReadAdminID = reader.ID,
            });
        }

        /// <summary>
        /// 删除阅读人绑定
        /// </summary>
        /// <param name="report">报告对象</param>
        virtual public void DeleteBinding(Report report)
        {
            this.Reponsitory.Delete<Layer.Data.Sqls.BvCrm.MapsReport>(item => item.ReportID == report.ID);
        }
    }
}
