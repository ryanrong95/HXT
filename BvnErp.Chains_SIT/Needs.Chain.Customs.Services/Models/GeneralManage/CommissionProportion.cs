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
    /// 提成比例信息
    /// </summary>
    public class CommissionProportion : IUnique, IPersist
    {
        public string ID { get; set; }

        /// <summary>
        /// 注册月数
        /// </summary>
        public int RegeisterMonth { get; set; }

        /// <summary>
        /// 提成比例
        /// </summary>
        public decimal Proportion { get; set; }


        public Enums.Status Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// 摘要
        /// </summary>
        public string Summary { get; set; }

        public CommissionProportion()
        {
            this.Status = Enums.Status.Normal;
            this.UpdateDate = this.CreateDate = DateTime.Now;
        }

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

        public void Enter()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.CommissionProportions>().Count(item => item.ID == this.ID);
                    if (count == 0)
                    {
                        //主键ID（FinanceVault +8位年月日+6位流水号）
                        this.ID = ChainsGuid.NewGuidUp();
                        reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.CommissionProportions
                        {
                            ID = this.ID,
                            Proportion = this.Proportion,
                            RegeisterMonth = this.RegeisterMonth,
                            Status = (int)this.Status,
                            UpdateDate = DateTime.Now,
                            Summary = this.Summary,
                            CreateDate = DateTime.Now
                        });
                    }
                    else
                    {
                        reponsitory.Update(new Layer.Data.Sqls.ScCustoms.CommissionProportions
                        {
                            ID = this.ID,
                            Proportion = this.Proportion,
                            RegeisterMonth = this.RegeisterMonth,
                            Status = (int)this.Status,
                            UpdateDate = DateTime.Now,
                            CreateDate = this.CreateDate,
                            Summary = this.Summary
                        }, item => item.ID == this.ID);
                    }
                }
                this.OnEnterSuccess();
            }
            catch (Exception ex)
            {
                this.EnterError(this, new ErrorEventArgs(ex.Message));
            }
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
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.CommissionProportions>(
                        new
                        {
                            Status = Enums.Status.Delete
                        }, item => item.ID == this.ID);
                }
                this.OnAbandonSuccess(); ;
            }
            catch (Exception ex)
            {
                this.AbandonError(this, new ErrorEventArgs(ex.Message));
            }
        }
        virtual protected void OnAbandonSuccess()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                //成功后触发事件
                this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
            }
        }
    }
}
