using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PsWms.DappApi.Services.Models;
using Yahv.PsWms.DappApi.Services.Enums;

namespace Yahv.PsWms.DappApi.Services.Views
{
    public class ReportsView : UniqueView<Report, PsWmsRepository>
    {
        #region 构造函数
        public ReportsView()
        {
        }

        public ReportsView(PsWmsRepository reponsitory) : base(reponsitory)
        {
        }

        public ReportsView(PsWmsRepository reponsitory, IQueryable<Report> iQueryable) : base(reponsitory, iQueryable)
        {

        }

        #endregion

        protected override IQueryable<Report> GetIQueryable()
        {
            var view = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.Reports>()
                       select new Report
                       {
                           ID = entity.ID,
                           ReportType = (ReportType)entity.ReportType,
                           ReviewDateTime = entity.ReviewDateTime,
                           ReviewerID = entity.ReviewerID,
                           CreateDate = entity.CreateDate,
                           ModifyDate = entity.ModifyDate,
                           Status = entity.Status,
                       };
            return view;
        }
    }

    public class Reports_Show_View : UniqueView<Report, PsWmsRepository>
    {
        #region 构造函数

        public Reports_Show_View()
        {

        }

        public Reports_Show_View(PsWmsRepository repository) : base(repository)
        {

        }

        public Reports_Show_View(PsWmsRepository reponsitory, IQueryable<Report> iQueryable) : base(reponsitory, iQueryable)
        {

        }

        #endregion

        protected override IQueryable<Report> GetIQueryable()
        {
            var reports = Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.Reports>().OrderByDescending(t => t.CreateDate);
            var linq = from entity in reports
                       select new Report()
                       {
                           ID = entity.ID,
                           ReportType = (ReportType)entity.ReportType,
                           ReviewDateTime = entity.ReviewDateTime,
                           ReviewerID = entity.ReviewerID,
                           CreateDate = entity.CreateDate,
                           ModifyDate = entity.ModifyDate,
                           Status = entity.Status,
                       };
            return linq;
        }

        /// <summary>
        /// 分页视图
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public object ToMyPage(int pageIndex = 1, int pageSize = 20)
        {
            var iquery = this.IQueryable.Cast<Report>();
            int total = iquery.Count();

            iquery = iquery.Skip(pageSize * (pageIndex - 1)).Take(pageSize);
            var ienum_iquery = iquery.ToArray();

            var linq = from entity in ienum_iquery
                       select new Report()
                       {
                           ID = entity.ID,
                           ReportType = (ReportType)entity.ReportType,
                           ReviewDateTime = entity.ReviewDateTime,
                           ReviewerID = entity.ReviewerID,
                           CreateDate = entity.CreateDate,
                           ModifyDate = entity.ModifyDate,
                           Status = entity.Status,
                       };
            return new
            {
                Total = total,
                PageSize = pageSize,
                PageIndex = pageIndex,
                data = linq.ToArray(),
            };
        }

        #region 查询方法

        /// <summary>
        /// 根据Type搜索
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Reports_Show_View SearchByType(ReportType type)
        {
            var reports = this.IQueryable.Cast<Report>();
            var linq = from report in reports
                       where report.ReportType == type
                       select report;

            var view = new Reports_Show_View(this.Reponsitory, linq)
            {
            };
            return view;
        }

        #endregion
    }
}
