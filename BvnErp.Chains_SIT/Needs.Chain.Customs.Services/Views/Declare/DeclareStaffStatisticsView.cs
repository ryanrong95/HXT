using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class DeclareStaffStatisticsView : QueryView<Models.DecHead, ScCustomsReponsitory>
    {
        public DeclareStaffStatisticsView()
        {
        }

        protected DeclareStaffStatisticsView(ScCustomsReponsitory reponsitory, IQueryable<Models.DecHead> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<Models.DecHead> GetIQueryable()
        {
            var decheadViews = new DecHeadsView(this.Reponsitory);
            var IQuery = from head in decheadViews
                         where head.IsSuccess
                         select head;
            return IQuery;

        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<Models.DecHead> iquery = this.IQueryable.Cast<Models.DecHead>().OrderByDescending(item => item.DDate);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_myDecHeads = iquery.ToArray();

            var listHeads = new List<DeclareStaffStatisticsModel>();
            foreach (var inputer in from declare in ienum_myDecHeads
                                     select new DeclareStaffStatisticsModel
                                     {
                                         StaffName = "制单员",
                                         AdminName = declare.Inputer.ID
                                     })
            {
                listHeads.Add(inputer);
            }

            foreach (var doubleChecker in from declare in ienum_myDecHeads
                                    select new DeclareStaffStatisticsModel
                                    {
                                        StaffName = "复核员",
                                        AdminName = declare.DoubleCheckerAdminID
                                    })
            {
                listHeads.Add(doubleChecker);
            }

            foreach (var submitCustom in from declare in ienum_myDecHeads
                                    select new DeclareStaffStatisticsModel
                                    {
                                        StaffName = "录入及申报员",
                                        AdminName = declare.SubmitCustomAdminID
                                    })
            {
                listHeads.Add(submitCustom);
            }




            var adminView = new AdminsTopView(this.Reponsitory);
            var results = from head in listHeads
                          join admin in adminView on head.AdminName equals admin.ID into s
                          from temp in s.DefaultIfEmpty()
                          group new { head.StaffName, temp.RealName } by new { head.StaffName, temp.RealName } into head_g
                          select new DeclareStaffStatisticsModel {
                              StaffName = head_g.Key.StaffName,
                              AdminName = head_g.Key.RealName,
                              Quantity = head_g.Count()
                          };

            if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                return results.Select(item =>
                {
                    object o = item;
                    return o;
                }).ToArray();
            }

            Func<DeclareStaffStatisticsModel, object> convert = declareStaff => new
            {
                StaffName = declareStaff.StaffName,
                AdminName = declareStaff.AdminName,
                Quantity = declareStaff.Quantity
            };

            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = results.Select(convert).ToArray(),
            };
        }

        public DeclareStaffStatisticsView SearchByFrom(DateTime fromtime)
        {
            var linq = from query in this.IQueryable
                       where query.CreateTime >= fromtime
                       select query;

            var view = new DeclareStaffStatisticsView(this.Reponsitory, linq);
            return view;
        }

        public DeclareStaffStatisticsView SearchByTo(DateTime totime)
        {
            var linq = from query in this.IQueryable
                       where query.CreateTime <= totime
                       select query;

            var view = new DeclareStaffStatisticsView(this.Reponsitory, linq);
            return view;
        }
    }

    /// <summary>
    /// 统计查询容器
    /// </summary>
    public class DeclareStaffStatisticsModel
    {
        public string StaffName { get; set; } = string.Empty;

        public string AdminName { get; set; } = string.Empty;

        public int Quantity { get; set; }
    }


}
