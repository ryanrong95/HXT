using Needs.Erp.Generic;
using NtErp.Crm.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Views
{
    public class MyReportsView : ReportsAlls
    {
        IGenericAdmin Admin;

        /// <summary>
        /// 无参构造函数
        /// </summary>
        private MyReportsView()
        {

        }

        /// <summary>
        /// 带参数的构造函数
        /// </summary>
        /// <param name="admin"></param>
        public MyReportsView(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        /// <summary>
        /// 获取数据集合
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Report> GetIQueryable()
        {
            return from report in base.GetIQueryable()
                   where report.Status == Enums.Status.Normal ||
                   (report.Status == Enums.Status.Temporary && report.Admin.ID == Admin.ID)
                   select report;
        }

        /// <summary>
        /// 绑定指定阅读人
        /// </summary>
        /// <param name="report">报告对象</param>
        /// <param name="Reader">阅读人</param>
        public override void BindingReader(Report report, AdminTop reader)
        {
            base.BindingReader(report, reader);
        }
        /// <summary>
        /// 附件删除
        /// </summary>
        /// <param name="Reportid">跟踪记录ID</param>
        public override void DeleteFiles(string Reportid)
        {
            base.DeleteFiles(Reportid);
        }
        /// <summary>
        /// 删除阅读人绑定
        /// </summary>
        /// <param name="report">报告对象</param>
        public override void DeleteBinding(Report report)
        {
            base.DeleteBinding(report);
        }
    }
}
