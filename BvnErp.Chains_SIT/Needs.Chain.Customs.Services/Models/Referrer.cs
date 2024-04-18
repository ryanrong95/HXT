using Needs.Ccs.Services.Enums;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 引荐人设置
    /// </summary>
    public class Referrer : IUnique
    {
        #region 
        public string ID { get; set; }
        /// <summary>
        /// 引荐人名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 添加人
        /// </summary>
        public string Creator { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public Needs.Ccs.Services.Enums.Status Status { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { get; set; }


        public Admin CreatorName { get; set; }

        #endregion
        public Referrer()
        {
            this.Status = Status.Normal;
            this.CreateDate = this.UpdateDate = DateTime.Now;
        }

        #region     持久化

        public event ErrorHanlder EnterError;
        public event ErrorHanlder AbandonError;
        /// <summary>
        /// 当删除成功后发生
        /// </summary>
        public event SuccessHanlder AbandonSuccessed;
        public event SuccessHanlder EnterSuccess;

        public void Enter()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Referrers>().Count(item => item.Name == this.Name && item.Status == (int)Status.Normal);

                    if (count == 0)
                    {
                        reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.Referrers
                        {
                            ID = ChainsGuid.NewGuidUp(),
                            Name = this.Name,
                            Creator = this.Creator,
                            CreateDate = this.CreateDate,
                            UpdateDate = this.UpdateDate,
                            Status = (int)this.Status,
                            Summary = this.Summary
                        });
                    }
                    else
                    {
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.Referrers>(new
                        {
                            Name = this.Name,
                            Creator = this.Creator,
                            CreateDate = this.CreateDate,
                            UpdateDate = this.UpdateDate,
                            Status = (int)this.Status,
                            Summary = this.Summary
                        }, item => item.ID == this.ID);
                    }
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }

            this.OnEnterSuccess();
        }

        virtual public void OnEnterSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        public void Abandon()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                if (reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>().Any(item => item.Referrer == this.Name))
                {
                    return;
                }
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Referrers>(new { Status = Enums.Status.Delete }, item => item.ID == this.ID);
            }

            this.OnAbandonSuccess();
        }

        virtual protected void OnAbandonSuccess()
        {
            if (this != null && this.AbandonSuccessed != null)
            {
                //成功后触发事件
                this.AbandonSuccessed(this, new SuccessEventArgs(this.ID));
            }
        }

        #endregion


    }
}
