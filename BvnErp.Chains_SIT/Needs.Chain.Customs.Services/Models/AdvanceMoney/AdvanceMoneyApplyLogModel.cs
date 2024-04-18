using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class AdvanceMoneyApplyLogModel : IUnique
    {
        #region 属性
        public string ID { get; set; }

        public string ApplyID { get; set; }
        /// <summary>
        /// 后台管理员
        /// </summary>
        public string AdminID { get; set; }
        public Admin Admin { get; set; }

        public Enums.AdvanceMoneyStatus Status { get; set; }

        public DateTime CreateDate { get; set; }

        public string Summary { get; set; }

        #endregion
        public virtual void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Insert<Layer.Data.Sqls.ScCustoms.AdvanceMoneyApplyLogs>(new Layer.Data.Sqls.ScCustoms.AdvanceMoneyApplyLogs
                {
                    ID = this.ID,
                    ApplyID = this.ApplyID,
                    Status = (int)this.Status,
                    AdminID = this.AdminID,
                    CreateDate = DateTime.Now,
                    Summary = Summary,
                });
            }
        }
    }
}
