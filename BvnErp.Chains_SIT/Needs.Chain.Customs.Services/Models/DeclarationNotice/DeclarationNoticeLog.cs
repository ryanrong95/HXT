using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 报关通知日志
    /// </summary>
    public class DeclarationNoticeLog : IUnique, IPersist
    {
        #region 属性
        public string ID { get; set; }

        public string DeclarationNoticeID { get; set; }

        public string DeclarationNoticeItemID { get; set; }

        public Admin Admin { get; set; }

        public DateTime CreateDate { get; set; }

        public string Summary { get; set; }
        #endregion

        public DeclarationNoticeLog()
        {
            this.CreateDate = DateTime.Now;
        }

        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DeclarationNoticeLogs>().Where(item => item.ID == this.ID).Count();
                if (count == 0)
                {
                    this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.DeclareNoticeLog);
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.DeclarationNoticeLogs
                    {
                        ID = this.ID,
                        DeclarationNoticeID = this.DeclarationNoticeID,
                        DeclarationNoticeItemID = this.DeclarationNoticeItemID,
                        AdminID = this.Admin.ID,
                        CreateDate = this.CreateDate,
                        Summary = this.Summary,
                    });
                }
                else
                {
                    reponsitory.Update(new Layer.Data.Sqls.ScCustoms.DeclarationNoticeLogs
                    {
                        DeclarationNoticeID = this.DeclarationNoticeID,
                        DeclarationNoticeItemID = this.DeclarationNoticeItemID,
                        AdminID = this.Admin.ID,
                        CreateDate = this.CreateDate,
                        Summary = this.Summary,
                    }, item => item.ID == this.ID);
                }
            }
        }
    }
}
