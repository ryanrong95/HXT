using Needs.Erp.Generic;
using NtErp.Crm.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Views
{
    public class MyClientReportsView : ReportsAlls
    {
        //人员对象
        AdminTop Admin;

        /// <summary>
        /// 无参构造函数
        /// </summary>
        private MyClientReportsView()
        {

        }

        /// <summary>
        /// 带参数的构造函数
        /// </summary>
        /// <param name="admin"></param>
        public MyClientReportsView(IGenericAdmin admin)
        {
            this.Admin = Extends.AdminExtends.GetTop(admin.ID); ;
        }

        /// <summary>
        /// 获取数据集合
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Report> GetIQueryable()
        {
            var clients = new MyClientBaseView(this.Admin, this.Reponsitory).Where(item => item.Status == Enums.ActionStatus.Complete
                || item.Status == Enums.ActionStatus.Auditing);

            var reports = base.GetIQueryable().Where(item => item.Client != null);

            return from report in reports
                   join client in clients on report.Client.ID equals client.ID
                   where report.Status == Enums.Status.Normal ||
                   (report.Status == Enums.Status.Temporary && report.Admin.ID == Admin.ID)
                   select report;
        }
    }
}
