using Needs.Ccs.Services.Hanlders;
using Needs.Linq;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 预归类产品管控
    /// </summary>
    public class PreProductControl : IUnique, IPersist
    {
        #region 属性

        string id;
        /// <summary>
        /// 主键ID (PreProductID+Type).MD5
        /// </summary>
        public string ID
        {
            get
            {
                return this.id ?? string.Concat(this.PreProductID, this.Type).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// 预归类产品ID
        /// </summary>
        public string PreProductID { get; set; }

        /// <summary>
        /// 管控审批类型：3C、禁运
        /// </summary>
        public Enums.ItemCategoryType Type { get; set; }

        /// <summary>
        /// 管控审批状态：待审批、通过、否决
        /// </summary>
        public Enums.PreProductControlStatus Status { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 审批时间
        /// </summary>
        public DateTime ApproveDate { get; set; }

        /// <summary>
        /// 审批人
        /// </summary>
        public string ApproverID { get; set; }

        /// <summary>
        /// 摘要备注
        /// </summary>
        public string Summary { get; set; }

        #endregion

        #region 扩展属性

        /// <summary>
        /// 预归类产品
        /// </summary>
        public PreProduct PreProduct { get; set; }

        /// <summary>
        /// 归类信息
        /// </summary>
        public PreClassifyProduct Category { get; set; }

        #endregion

        public PreProductControl()
        {
            this.Status = Enums.PreProductControlStatus.Waiting;
            this.CreateDate = this.ApproveDate = DateTime.Now;
        }

        public event SuccessHanlder EnterSuccess;
        /// <summary>
        /// 审批通过时发生
        /// </summary>
        public event PreProductControledHanlder Approved;
        /// <summary>
        /// 审批否决时发生
        /// </summary>
        public event PreProductControledHanlder Vetoed;

        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PreProductControls>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.PreProductControls()
                    {
                        ID = this.ID,
                        PreProductID = this.PreProductID,
                        Type = (int)this.Type,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        ApproveDate = this.ApproveDate,
                    });
                }
                else
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.PreProductControls>(new
                    {
                        Status = (int)this.Status,
                        ApproveDate = DateTime.Now,
                        ApproverID = this.ApproverID,
                        Summary = this.Summary
                    }, item => item.ID == this.ID);
                }
            }

            this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this.ID));
        }

        /// <summary>
        /// 审批通过
        /// </summary>
        /// <param name="approverID">审批人</param>
        public void Approve(string approverID)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //更新审批状态为“通过”
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.PreProductControls>(new
                {
                    Status = Enums.PreProductControlStatus.Approved,
                    ApproveDate = DateTime.Now,
                    ApproverID = approverID,
                }, item => item.ID == this.ID);

                //生成推送通知
                this.ToApiNotice(reponsitory);
            }

            this.Approved?.Invoke(this, new PreProductControledEventArgs(this));
        }

        /// <summary>
        /// 审批否决
        /// </summary>
        /// <param name="approverID">审批人</param>
        public void Veto(string approverID)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //更新审批状态为“否决”
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.PreProductControls>(new
                {
                    Status = Enums.PreProductControlStatus.Vetoed,
                    ApproveDate = DateTime.Now,
                    ApproverID = approverID,
                }, item => item.ID == this.ID);

                //更新归类类型
                var Type = this.Category.Type | this.Type;
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.PreProductCategories>(new
                {
                    Type = (int)Type,
                    UpdateDate = DateTime.Now,
                }, item => item.ID == this.Category.ID);

                //生成推送通知
                this.ToApiNotice(reponsitory);
            }

            this.Vetoed?.Invoke(this, new PreProductControledEventArgs(this));
        }
    }
}
