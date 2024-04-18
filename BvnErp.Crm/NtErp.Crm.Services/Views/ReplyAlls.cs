using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Utils.Converters;
using NtErp.Crm.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Views
{
    /// <summary>
    /// 获取所有点评内容
    /// </summary>
    public class ReplyAlls : UniqueView<Reply, BvCrmReponsitory>
    {
        public ReplyAlls()
        {

        }

        public ReplyAlls(BvCrmReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Reply> GetIQueryable()
        {
            AdminTopView adminTops = new AdminTopView(this.Reponsitory);

            return from reply in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Replies>()
                   join admin in adminTops on reply.AdminID equals admin.ID
                   select new Reply
                   {
                       ID = reply.ID,
                       WorksOtherID = reply.WorksOtherID,
                       WorksWeeklyID = reply.WorksWeeklyID,
                       ReportID = reply.ReportID,
                       Context = reply.Context,
                       Admin = admin,
                       CreateDate = reply.CreateDate,
                       UpdateDate = reply.UpdateDate,
                   };
        }

        /// <summary>
        /// 绑定指定阅读人
        /// </summary>
        /// <param name="replyid"></param>
        /// <param name="Reader"></param>
        public void BindingReader(Reply reply, AdminTop reader)
        {
            Reponsitory.Insert(new Layer.Data.Sqls.BvCrm.MapsReply()
            {
                ID = string.Concat(reply.ID, reader.ID).MD5(),
                ReplyID = reply.ID,
                ReadAdminID = reader.ID,
            });
        }

        /// <summary>
        /// 删除阅读人绑定
        /// </summary>
        /// <param name="reply"></param>
        public void DeleteBinding(Reply reply)
        {
            this.Reponsitory.Delete<Layer.Data.Sqls.BvCrm.MapsReply>(item => item.ReplyID == reply.ID);
        }
    }
}
