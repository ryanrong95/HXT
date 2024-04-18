using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using Needs.Overall;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class NoticeBoardView : QueryView<NoticeBoardModel, ScCustomsReponsitory>
    {
        public NoticeBoardView()
        {
        }

        protected NoticeBoardView(ScCustomsReponsitory reponsitory, IQueryable<NoticeBoardModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<NoticeBoardModel> GetIQueryable()
        {
            var iQuery = from noticeBoard in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.NoticeBoard>()
                         where noticeBoard.Status == (int)Enums.Status.Normal
                         select new NoticeBoardModel
                         {
                             ID = noticeBoard.ID,
                             CreateDate = noticeBoard.CreateDate,
                             NoticeContent = noticeBoard.NoticeContent,
                             NoticeTitle = noticeBoard.NoticeTitle,
                             RoleID = noticeBoard.RoleID,
                             RoleName = noticeBoard.RoleName,
                             Status = (Enums.Status)noticeBoard.Status,
                             AdminID = noticeBoard.AdminID

                         };
            return iQuery;
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<NoticeBoardModel> iquery = this.IQueryable.Cast<NoticeBoardModel>().OrderByDescending(item => item.CreateDate);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_noticeboard = iquery.ToArray();

            var ienums_linq = from noticeboard in ienum_noticeboard
                              select new NoticeBoardModel
                              {
                                  ID = noticeboard.ID,
                                  CreateDate = noticeboard.CreateDate,
                                  NoticeContent = noticeboard.NoticeContent,
                                  NoticeTitle = noticeboard.NoticeTitle,
                                  RoleID = noticeboard.RoleID,
                                  RoleName = noticeboard.RoleName,
                                  //Status = (Enums.Status)noticeboard.Status
                              };

            var results = ienums_linq;

            if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                return results.Select(item =>
                {
                    object o = item;
                    return o;
                }).ToArray();
            }

            Func<NoticeBoardModel, object> convert = item => new
            {
                ID = item.ID,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd"),
                NoticeContent = item.NoticeContent.Replace("&amp;", "&"),
                NoticeTitle = item.NoticeTitle.Replace("&amp;", "&"),
                RoleID = item.RoleID,
                RoleName = item.RoleName,
                //Status = item.Status.GetDescriptions()
            };

            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = results.Select(convert).ToArray(),
            };
        }

        /// <summary>
        /// 根据发布时间开始时间查询
        /// </summary>
        /// <param name="begin"></param>
        /// <returns></returns>
        public NoticeBoardView SearchByStartDate(DateTime begin)
        {
            var linq = from query in this.IQueryable
                       where query.CreateDate >= begin
                       select query;

            var view = new NoticeBoardView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据发布时间结束时间查询
        /// </summary>
        /// <param name="end"></param>
        /// <returns></returns>
        public NoticeBoardView SearchByEndDate(DateTime end)
        {
            var linq = from query in this.IQueryable
                       where query.CreateDate < end
                       select query;

            var view = new NoticeBoardView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 查询所有
        /// </summary>
        /// <param name="adminID"></param>
        /// <returns></returns>
        public NoticeBoardView SearchByClientAdmin(string adminID)
        {
            var roleIDs = new AdminsRoleView().Where(t => t.OriginID == adminID).Select(t => t.ID).ToList();
            var linq = from query in this.IQueryable
                       where roleIDs.Contains(query.RoleID) || query.AdminID == adminID || query.RoleID == "ALL"
                       select query;

            var view = new NoticeBoardView(this.Reponsitory, linq);
            return view;
        }
        /// <summary>
        /// 查询自己
        /// </summary>
        /// <param name="adminID"></param>
        /// <returns></returns>
        public NoticeBoardView SearchByClientAdminID(string adminID)
        {
            var linq = from query in this.IQueryable
                       where query.AdminID.Contains(adminID) 
                       select query;

            var view = new NoticeBoardView(this.Reponsitory, linq);
            return view;
        }
    }
    public class AdminsRoleView : UniqueView<Admin, ScCustomsReponsitory>, IFkoView<Admin>
    {
        public AdminsRoleView()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库实体</param>
        public AdminsRoleView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        /// <summary>
        /// 获取人员集合
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Admin> GetIQueryable()
        {
            return from admin in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView2>()
                   select new Models.Admin
                   {
                       OriginID = admin.OriginID,
                       ID = admin.RoleID
                   };
        }
    }
}
