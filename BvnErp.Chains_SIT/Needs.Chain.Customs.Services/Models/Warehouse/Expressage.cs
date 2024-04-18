using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 快递信息
    /// </summary>
    public class Expressage : IUnique, IPersist, IFulError, IFulSuccess
    {
        public string ID { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public string Contact { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 送货地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 查询标记
        /// </summary>
        public string QueryMark { get; set; }

        /// <summary>
        /// 快递公司
        /// </summary>
        public ExpressCompany ExpressCompany { get; set; }

        /// <summary>
        /// 快递类型
        /// </summary>
        public ExpressType ExpressType { get; set; }

        /// <summary>
        /// 付费方式
        /// </summary>
        public Enums.PayType PayType { get; set; }
        
        /// <summary>
        /// 运单编号
        /// </summary>
        public string WaybillCode { get; set; }

        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public Expressage()
        {
            this.CreateDate = DateTime.Now;
            this.UpdateDate = DateTime.Now;
            this.Status = Enums.Status.Normal;
        }

        public event ErrorHanlder EnterError;
        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event SuccessHanlder EnterSuccess;

        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Expressages>().Count(item => item.ID == this.ID);
                    if (count == 0)
                    {
                       this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.Expressage);
                        reponsitory.Insert(this.ToLinq());
                    }
                    else
                    {
                        reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
                    }
                }
                this.OnEnterSuccess();
            }
            catch (Exception ex)
            {
                this.EnterError(this, new ErrorEventArgs(ex.Message));
            }
        }

        /// <summary>
        /// 去持久化
        /// </summary>
        public void Abandon()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Expressages>(new { Status = Enums.Status.Delete }, item => item.ID == this.ID);
            }
            this.OnAbandonSuccess();
        }

        virtual public void OnEnterSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
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

        /// <summary>
        /// 保存快递面单
        /// </summary>
        public void SaveKDD()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Expressages>(new
                {
                    WaybillCode = this.WaybillCode,
                    //HtmlTemplate = this.HtmlTemplate,
                }, item => item.ID == this.ID);
            }
        }
    }
}