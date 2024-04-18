using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class NoticeBoardModel : IUnique
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 公告标题
        /// </summary>
        public string NoticeTitle { get; set; }

        /// <summary>
        /// 公告内容
        /// </summary>
        public string NoticeContent { get; set; }
        /// <summary>
        /// 哪些角色可见
        /// </summary>
        public string IsVisible { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public Enums.Status Status { get; set; }
        /// <summary>
        /// admin
        /// </summary>
        public string AdminID { get; set; }

        /// <summary>
        /// RoleID
        /// </summary>
        public string RoleID { get; set; }

        /// <summary>
        /// RoleName
        /// </summary>
        public string RoleName { get; set; }

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.NoticeBoard>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    reponsitory.Insert<Layer.Data.Sqls.ScCustoms.NoticeBoard>(new Layer.Data.Sqls.ScCustoms.NoticeBoard
                    {
                        ID = this.ID,
                        NoticeTitle = this.NoticeTitle,
                        NoticeContent = this.NoticeContent,
                        AdminID = this.AdminID,
                        RoleID = this.RoleID,
                        RoleName = this.RoleName,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = DateTime.Now,
                    });
                }

            }
        }
    }
}
