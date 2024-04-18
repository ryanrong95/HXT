using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 报关员（制单员、发单员、核对人）候选
    /// </summary>
    public class DeclarantCandidate : IUnique, IPersist
    {
        #region 数据库字段

        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// AdminID
        /// </summary>
        public string AdminID { get; set; }

        /// <summary>
        /// 类型（制单员、发单员、核对人）
        /// </summary>
        public Enums.DeclarantCandidateType Type { get; set; }

        /// <summary>
        /// 数据状态
        /// </summary>
        public Enums.Status Status { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { get; set; }

        #endregion

        /// <summary>
        /// AdminName
        /// </summary>
        public string AdminName { get; set; }

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DeclarantCandidates>().Where(item => item.ID == this.ID);
                if (count.Count() == 0)
                {
                    reponsitory.Insert<Layer.Data.Sqls.ScCustoms.DeclarantCandidates>(new Layer.Data.Sqls.ScCustoms.DeclarantCandidates
                    {
                        ID = this.ID,
                        AdminID = this.AdminID,
                        Type = (int)this.Type,
                        Status = (int)this.Status,
                        CreateTime = this.CreateTime,
                        UpdateTime = this.UpdateTime,
                        Summary = this.Summary,
                    });
                }
                else
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.DeclarantCandidates>(new
                    {
                        AdminID = this.AdminID,
                        Type = (int)this.Type,
                        Status = (int)this.Status,
                        CreateTime = this.CreateTime,
                        UpdateTime = this.UpdateTime,
                        Summary = this.Summary,
                    }, t => t.ID == this.ID);
                }
            }
        }

        public void Abandon()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.DeclarantCandidates>(new
                {
                    Status = (int)Enums.Status.Delete,
                    UpdateTime = DateTime.Now,
                }, t => t.ID == this.ID);
            }
        }

    }
}
