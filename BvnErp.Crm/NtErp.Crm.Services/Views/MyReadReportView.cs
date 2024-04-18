using Needs.Erp.Generic;
using NtErp.Crm.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Views
{
    public class MyReadReportView : ReportsAlls
    {
        IGenericAdmin Admin;

        /// <summary>
        /// 无参构造函数
        /// </summary>
        private MyReadReportView()
        {

        }

        /// <summary>
        /// 带参数的构造函数
        /// </summary>
        /// <param name="admin"></param>
        public MyReadReportView(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        /// <summary>
        /// 获取数据集合
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Report> GetIQueryable()
        {
            var reports = base.GetIQueryable().Where(item => item.Status == Enums.Status.Normal);

            //获取指定阅读人的点评关联的跟踪记录
            var myreplyreport = from map in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsReply>()
                                join reply in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Replies>() on map.ReplyID equals reply.ID
                                join report in reports on reply.ReportID equals report.ID
                                where map.ReadAdminID == this.Admin.ID
                                select report;

            //获取指定阅读人的跟踪记录
            var myreport = from map in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsReport>()
                           join report in reports on map.ReportID equals report.ID
                           where map.ReadAdminID == this.Admin.ID
                           select report;

            return myreplyreport.Union(myreport).OrderByDescending(item => item.UpdateDate);
        }
    }
}
