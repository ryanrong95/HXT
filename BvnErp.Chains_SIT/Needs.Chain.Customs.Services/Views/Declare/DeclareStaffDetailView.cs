using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class DeclareStaffDetailView : QueryView<Models.DecHead, ScCustomsReponsitory>
    {
        public DeclareStaffDetailView()
        {
        }

        protected DeclareStaffDetailView(ScCustomsReponsitory reponsitory, IQueryable<Models.DecHead> iQueryable) : base(reponsitory, iQueryable)
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
            var adminView = new AdminsTopView(this.Reponsitory);

            var results = from head in ienum_myDecHeads
                          join admin in adminView on head.SubmitCustomAdminID equals admin.ID into s
                          from s_admin in s.DefaultIfEmpty()
                          join admin in adminView on head.DoubleCheckerAdminID equals admin.ID into d
                          from d_admin in d.DefaultIfEmpty()
                          select new DeclareStaffDetailModel
                          {
                              OrderID = head.OrderID,
                              EntryId = head.EntryId,
                              ContrNo = head.ContrNo,
                              OwnerName = head.OwnerName,
                              CusDecStatus = head.CusDecStatus,
                              DDate = head.DDate.Value,
                              InputerID = head.Inputer.ID,
                              InputerName = head.Inputer.RealName,
                              SubmitCustomAdminID = head.SubmitCustomAdminID,
                              SubmitCustomAdminName = s_admin.RealName,
                              DoubleCheckerAdminID = head.DoubleCheckerAdminID,
                              DoubleCheckerAdminName = d_admin.RealName
                          };

            if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                return results.Select(item =>
                {
                    object o = item;
                    return o;
                }).ToArray();
            }

            Func<DeclareStaffDetailModel, object> convert = head => new
            {
                OrderID = head.OrderID,
                EntryId = head.EntryId,
                ContrNo = head.ContrNo,
                OwnerName = head.OwnerName,
                CusDecStatus = head.CusDecStatus,
                StatusName = head.StatusName,
                DDate = head.DDate.ToString("yyyy-MM-dd"),
                InputerID = head.InputerID,
                InputerName = head.InputerName,
                SubmitCustomAdminID = head.SubmitCustomAdminID,
                SubmitCustomAdminName = head.SubmitCustomAdminName,
                DoubleCheckerAdminID = head.DoubleCheckerAdminID,
                DoubleCheckerAdminName = head.DoubleCheckerAdminName
            };

            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = results.Select(convert).ToArray(),
            };
        }

        public DeclareStaffDetailView SearchByFrom(DateTime fromtime)
        {
            var linq = from query in this.IQueryable
                       where query.CreateTime >= fromtime
                       select query;

            var view = new DeclareStaffDetailView(this.Reponsitory, linq);
            return view;
        }

        public DeclareStaffDetailView SearchByTo(DateTime totime)
        {
            var linq = from query in this.IQueryable
                       where query.CreateTime <= totime
                       select query;

            var view = new DeclareStaffDetailView(this.Reponsitory, linq);
            return view;
        }
    }

    /// <summary>
    /// 统计查询容器
    /// </summary>
    public class DeclareStaffDetailModel
    {
        /// <summary>
        /// 数据中心统一编号
        /// </summary>
        public string SeqNo { get; set; } = string.Empty;

        public string OrderID { get; set; } = string.Empty;

        /// <summary>
        /// 预录入编号
        /// </summary>
        public string PreEntryId { get; set; } = string.Empty;

        /// <summary>
        /// 报关单状态
        /// </summary>
        public string CusDecStatus { get; set; } = string.Empty;

        public string StatusName
        {
            get
            {
                return MultiEnumUtils.ToText<Enums.CusDecStatus>(this.CusDecStatus);
            }
        }

        /// <summary>
        /// 报关单号/海关编号
        /// </summary>
        public string EntryId { get; set; } = string.Empty;

        public string ContrNo { get; set; } = string.Empty;

        /// <summary>
        /// 进口日期
        /// </summary>
        public string IEDate { get; set; } = string.Empty;

        /// <summary>
        /// 申报日期
        /// </summary>
        public DateTime DDate { get; set; }

        /// <summary>
        /// 消费使用/生产销售单位名称
        /// </summary>
        public string OwnerName { get; set; } = string.Empty;

        /// <summary>
        /// 制单员ID
        /// </summary>
        public string InputerID { get; set; } = string.Empty;

        /// <summary>
        /// 制单员名称
        /// </summary>
        public string InputerName { get; set; } = string.Empty;

        /// <summary>
        /// 录入及申报员ID
        /// </summary>
        public string SubmitCustomAdminID { get; set; } = string.Empty;

        /// <summary>
        /// 录入及申报员
        /// </summary>
        public string SubmitCustomAdminName { get; set; } = string.Empty;

        /// <summary>
        /// 复核员ID
        /// </summary>
        public string DoubleCheckerAdminID { get; set; } = string.Empty;

        /// <summary>
        /// 复核员名称
        /// </summary>
        public string DoubleCheckerAdminName { get; set; } = string.Empty;
    }
}
