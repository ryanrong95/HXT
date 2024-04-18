using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class AdvanceMoneyApplyModel : IUnique
    {
        #region 属性

        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 申请ID
        /// </summary>
        public string ApplyID { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// 客户编号
        /// </summary>
        public string ClientCode { get; set; }

        /// <summary>
        /// 客户ID
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 已使用金额
        /// </summary>
        public decimal AmountUsed { get; set; }

        /// <summary>
        /// 垫款期限
        /// </summary>
        public int LimitDays { get; set; }

        /// <summary>
        /// 利率
        /// </summary>
        public Decimal InterestRate { get; set; }

        /// <summary>
        /// 逾期利率
        /// </summary>
        public Decimal OverdueInterestRate { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public Enums.AdvanceMoneyStatus Status { get; set; }
        public int IntStatus { get; set; }
        public string AdminID { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }

        public int UntieAdvance { get; set; }//待发货风控修改订单垫资状态 为1

        public string DepartmentCode { get; set; }

        #region 上传文件

        /// <summary>
        /// 文件名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 文件类型（单据等）
        /// </summary>
        public Enums.CostApplyFileTypeEnum FileType { get; set; }

        /// <summary>
        /// 文件格式
        /// </summary>
        public string FileFormat { get; set; } = string.Empty;

        /// <summary>
        /// URL
        /// </summary>
        public string URL { get; set; } = string.Empty;

        #endregion

        #endregion
        public virtual void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Insert<Layer.Data.Sqls.ScCustoms.AdvanceMoneyApplies>(new Layer.Data.Sqls.ScCustoms.AdvanceMoneyApplies
                {
                    ID = this.ID,
                    ClientID = this.ClientID,
                    Amount = Amount,
                    AmountUsed = AmountUsed,
                    Status = (int)this.Status,
                    LimitDays = this.LimitDays,
                    InterestRate = this.InterestRate,
                    OverdueInterestRate = this.OverdueInterestRate,
                    AdminID = this.AdminID,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    Summary = Summary,
                });
            }
        }
        /// <summary>
        /// 风控拒绝修改状态
        /// </summary>
        public virtual void Delete()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.AdvanceMoneyApplies>(new
                {
                    Status = (int)this.Status,
                    UpdateDate = DateTime.Now,
                    Summary = Summary,
                }, item => item.ID == this.ApplyID);
            }
        }

        /// <summary>
        /// 风控审核通过,修改状态
        /// </summary>
        public virtual void Update()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.AdvanceMoneyApplies>(new
                {
                    Amount = Amount,
                    LimitDays = LimitDays,
                    Status = (int)this.Status,
                    UpdateDate = DateTime.Now,
                    Summary = Summary,
                }, item => item.ID == this.ApplyID);
            }
        }

        /// <summary>
        /// 风控管控待发货订单垫资
        /// </summary>
        public virtual void UntieAdvanceUpdate()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.AdvanceRecords>(new
                {
                    UntieAdvance = this.UntieAdvance,
                    UpdateDate = DateTime.Now,
                }, item => item.ID == this.ID);
            }
        }

        /// <summary>
        /// 经理审核通过,修改状态
        /// </summary>
        public virtual void Audit()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.AdvanceMoneyApplies>(new
                {
                    Status = (int)this.Status,
                    UpdateDate = DateTime.Now,
                    Summary = Summary,
                }, item => item.ID == this.ApplyID);
            }
        }
    }
}
