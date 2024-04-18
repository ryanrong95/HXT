using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class ExpressKdd : IUnique, IPersist, IFulError, IFulSuccess
    {
        public string ID { get; set; }
        /// <summary>
        /// 寄件单位
        /// </summary>

        public string SenderComp { get; set; }
        /// <summary>
        ///寄件人
        /// </summary>
        public string Sender { get; set; }

        /// <summary>
        /// 寄件人电话
        /// </summary>
        public string SenderMobile { get; set; }

        /// <summary>
        /// 寄件地址
        /// </summary>
        public string SenderAddress { get; set; }

        /// <summary>
        /// 收件单位
        /// </summary>
        public string ReceiverComp { get; set; }

        /// <summary>
        /// 收件人
        /// </summary>
        public string Receiver { get; set; }

        /// <summary>
        /// 收件人电话
        /// </summary>
        public string ReveiveMobile { get; set; }

        /// <summary>
        /// 收获地址
        /// </summary>
        public string ReveiveAddress { get; set; }

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

        public ExpressKdd()
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
                    int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExpressKdds>().Count(item => item.ID == this.ID);
                    if (count == 0)
                    {
                        reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.ExpressKdds
                        {
                            ID = ChainsGuid.NewGuidUp(),
                            ExpressCompanyID = this.ExpressCompany.ID,
                            SenderComp = this.SenderComp,
                            Sender = this.Sender,
                            SenderMobile = this.SenderMobile,
                            SenderAddress = this.SenderAddress,
                            ReceiverComp=this.ReceiverComp,
                            Receiver = this.Receiver,
                            ReveiveMobile = this.ReveiveMobile,
                            ReveiveAddress = this.ReveiveAddress,
                            ExpressTypeID = this.ExpressType.ID,
                            PayType = (int)this.PayType,
                            Status = (int)this.Status,
                            CreateDate = this.CreateDate,
                            UpdateDate = this.UpdateDate,

                        });

                    }
                    else
                    {
                        reponsitory.Update(new Layer.Data.Sqls.ScCustoms.ExpressKdds
                        {
                            ID = this.ID,
                            ExpressCompanyID = this.ExpressCompany.ID,
                            SenderComp = this.SenderComp,
                            Sender = this.Sender,
                            SenderMobile = this.SenderMobile,
                            SenderAddress = this.SenderAddress,
                            ReceiverComp = this.ReceiverComp,
                            Receiver = this.Receiver,
                            ReveiveMobile = this.ReveiveMobile,
                            ReveiveAddress = this.ReveiveAddress,
                            ExpressTypeID = this.ExpressType.ID,
                            PayType = (int)this.PayType,
                            Status = (int)this.Status,
                            CreateDate = this.CreateDate,
                            UpdateDate = this.UpdateDate,
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

        /// <summary>
        /// 去持久化
        /// </summary>
        public void Abandon()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.ExpressKdds>(new { Status = Enums.Status.Delete }, item => item.ID == this.ID);
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
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.ExpressKdds>(new
                {
                    WaybillCode = this.WaybillCode,
                }, item => item.ID == this.ID);
            }
        }
    }
}
