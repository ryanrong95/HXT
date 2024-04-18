using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Utils.Converters;
using NtErp.Crm.Services.Models;

namespace NtErp.Crm.Services.Views
{
    public class MyNoticeView : UniqueView<Notice, BvCrmReponsitory>, Needs.Underly.IFkoView<Notice>
    {
        Needs.Erp.Generic.IGenericAdmin Admin;

        /// <summary>
        /// 无参构造函数
        /// </summary>
        public MyNoticeView()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="admin">人员对象</param>
        public MyNoticeView(Needs.Erp.Generic.IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库实例</param>
        public MyNoticeView(BvCrmReponsitory reponsitory) : base(reponsitory)
        {

        }

        /// <summary>
        /// 查询数据集合
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Notice> GetIQueryable()
        {
            AdminTopView topview = new AdminTopView(this.Reponsitory); //人员视图

            return from notice in Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Notices>()
                   join admin in topview on notice.AdminID equals admin.ID
                   join map in Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsNotice>() on notice.ID equals map.NoticeID into maps
                   orderby notice.CreateDate descending
                   select new Notice
                   {
                       ID = notice.ID,
                       Name = notice.Name,
                       Context = notice.Context,
                       Admin = admin,
                       CreateDate = notice.CreateDate,
                       Status = maps.Count(item=>item.AdminID == Admin.ID) > 0 ? Enums.WarningStatus.read : Enums.WarningStatus.unread,
                   };
        }

        /// <summary>
        /// 已查看
        /// </summary>
        /// <param name="NoticeId"></param>
        public void Read(string NoticeId)
        {
            var Notice = this[NoticeId];
            var CurrentAdmin = Extends.AdminExtends.GetTop(this.Admin.ID);
            Reponsitory.Insert(new Layer.Data.Sqls.BvCrm.MapsNotice
            {
                ID = string.Concat(Notice.ID,CurrentAdmin.ID).MD5(),
                AdminID = CurrentAdmin.ID,
                NoticeID = Notice.ID,
            });
        }
    }
}
