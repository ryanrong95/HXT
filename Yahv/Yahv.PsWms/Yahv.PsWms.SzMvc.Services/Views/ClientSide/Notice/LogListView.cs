using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace Yahv.PsWms.SzMvc.Services.Views
{
    public class LogListView : QueryView<LogListViewModel, PsOrderRepository>
    {
        public LogListView()
        {
        }

        protected LogListView(PsOrderRepository reponsitory, IQueryable<LogListViewModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<LogListViewModel> GetIQueryable()
        {
            var logs = this.Reponsitory.ReadTable<Layers.Data.Sqls.PsOrder.Logs>();

            var iQuery = from log in logs
                         orderby log.CreateDate descending
                         select new LogListViewModel
                         {
                             LogID = log.ID,
                             ActionName = log.ActionName,
                             MainID = log.MainID,
                             Url = log.Url,
                             Content = log.Content,
                             CreateDate = log.CreateDate,
                         };

            return iQuery;
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public Tuple<T[], int> ToMyPage<T>(Func<LogListViewModel, T> convert, int? pageIndex = null, int? pageSize = null) where T : class
        {
            IQueryable<LogListViewModel> iquery = this.IQueryable.Cast<LogListViewModel>();
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_logs = iquery.ToArray();

            var ienums_linq = from log in ienum_logs
                              orderby log.CreateDate descending
                              select new LogListViewModel
                              {
                                  LogID = log.LogID,
                                  ActionName = log.ActionName,
                                  MainID = log.MainID,
                                  Url = log.Url,
                                  Content = log.Content,
                                  CreateDate = log.CreateDate,
                              };

            var results = ienums_linq;

            return new Tuple<T[], int>(results.Select(convert).ToArray(), total);
        }

        /// <summary>
        /// 根据 ActionName 查询
        /// </summary>
        /// <param name="actionName"></param>
        /// <returns></returns>
        public LogListView SearchByActionNames(string[] actionNames)
        {
            var linq = from query in this.IQueryable
                       where actionNames.Contains(query.ActionName)
                       select query;

            var view = new LogListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据 MainID 查询
        /// </summary>
        /// <param name="mainID"></param>
        /// <returns></returns>
        public LogListView SearchByMainID(string mainID)
        {
            var linq = from query in this.IQueryable
                       where query.MainID.Contains(mainID)
                       select query;

            var view = new LogListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据 CreateDate 开始时间查询
        /// </summary>
        /// <param name="begin"></param>
        /// <returns></returns>
        public LogListView SearchByCreateDateBegin(DateTime begin)
        {
            var linq = from query in this.IQueryable
                       where query.CreateDate >= begin
                       select query;

            var view = new LogListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据 CreateDate 结束时间查询
        /// </summary>
        /// <param name="end"></param>
        /// <returns></returns>
        public LogListView SearchByCreateDateEnd(DateTime end)
        {
            var linq = from query in this.IQueryable
                       where query.CreateDate < end
                       select query;

            var view = new LogListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据 LogID 查询
        /// </summary>
        /// <param name="logID"></param>
        /// <returns></returns>
        public LogListView SearchByLogID(string logID)
        {
            var linq = from query in this.IQueryable
                       where query.LogID == logID
                       select query;

            var view = new LogListView(this.Reponsitory, linq);
            return view;
        }
    }

    public class LogListViewModel
    {
        /// <summary>
        /// LogID
        /// </summary>
        public string LogID { get; set; }

        /// <summary>
        /// ActionName
        /// </summary>
        public string ActionName { get; set; }

        /// <summary>
        /// MainID
        /// </summary>
        public string MainID { get; set; }

        /// <summary>
        /// Url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Content
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// CreateDate
        /// </summary>
        public DateTime CreateDate { get; set; }
    }
}
