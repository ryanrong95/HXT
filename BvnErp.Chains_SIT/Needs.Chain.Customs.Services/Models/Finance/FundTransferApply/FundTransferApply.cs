using Needs.Ccs.Services.Hanlders;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class FundTransferApplies : IUnique, IPersistence, IFulError, IFulSuccess
    {
        public string ID { get; set; }
        /// <summary>
        /// 关联号码：流水号/承兑票号
        /// </summary>
        public string FromSeqNo { get; set; }
        /// <summary>
        /// 调出账户
        /// </summary>
        public FinanceAccount OutAccount { get; set; }
        /// <summary>
        /// 调出流水号
        /// </summary>
        public string OutSeqNo { get; set; }
        /// <summary>
        /// 调出金额
        /// </summary>
        public decimal OutAmount { get; set; }
        /// <summary>
        /// 调出币种
        /// </summary>
        public string OutCurrency { get; set; }
        /// <summary>
        /// 调入账户
        /// </summary>
        public FinanceAccount InAccount { get; set; }
        /// <summary>
        /// 调入流水号
        /// </summary>
        public string InSeqNo { get; set; }
        /// <summary>
        /// 调入金额
        /// </summary>
        public decimal InAmount { get; set; }
        /// <summary>
        /// 调入币种
        /// </summary>
        public string InCurrency { get; set; }
        /// <summary>
        /// 汇率
        /// </summary>
        public decimal Rate { get; set; }
        /// <summary>
        /// 贴现利息，银行承兑转出时记录
        /// </summary>
        public decimal? DiscountInterest { get; set; }
        /// <summary>
        /// 快捷支付手续费
        /// </summary>
        public decimal? QRCodeFee { get; set; }
        /// <summary>
        /// 手续费
        /// </summary>
        public decimal? Poundage { get; set; }
        /// <summary>
        /// 收付费流水号
        /// </summary>
        public string PoundageSeqNo { get; set; }
        /// <summary>
        /// 付款方式
        /// </summary>
        public Enums.PaymentType PaymentType { get; set; }
        /// <summary>
        /// 费用类型
        /// </summary>
        public Enums.FundTransferType FeeType { get; set; }
        /// <summary>
        /// 付款日期
        /// </summary>
        public DateTime? PaymentDate { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderID { get; set; }
        /// <summary>
        /// 审批状态
        /// </summary>
        public Enums.FundTransferApplyStatus ApplyStatus { get; set; }       
        public Admin Admin { get; set; }
        public Enums.Status Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Summary { get; set; }
        /// <summary>
        /// 付款人 贴现利息列表用
        /// </summary>
        public string Payer { get; set; }
        /// <summary>
        ///  客户 贴现利息列表用
        /// </summary>
        public Client Client { get; set; }
        public bool? FundTranCreSta { get; set; }
        public string FundTranWrod { get; set; }
        public string FundTranNo { get; set; }
        public FundTransferApplies()
        {
            this.ApplyStatus = Enums.FundTransferApplyStatus.Approving;
            this.Status = Enums.Status.Normal;
            this.UpdateDate = this.CreateDate = DateTime.Now;           
        }

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

        public event FinanceTransferApplyHanlder TransferCompleted;
        public event FinanceTransferApplyHanlder Post2Center;
        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FundTransferApplies>().Count(item => item.ID == this.ID);
                    if (count == 0)
                    {
                        //主键ID（FinanceAccount +8位年月日+6位流水号）
                        this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.FundTransferApply);
                        reponsitory.Insert(this.ToLinq());
                    }
                    else
                    {
                        reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
                    }
                }
                this.OnEnterSuccess();
                this.OnPost2Center();
                this.OnTransferCompleted();
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
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.FundTransferApplies>(
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

        public void Approve(Enums.FundTransferApplyStatus applyStatus)
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {                    
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.FundTransferApplies>(new { ApplyStatus = (int)applyStatus }, item => item.ID == this.ID);
                }
                this.OnEnterSuccess();
            }
            catch (Exception ex)
            {
                
            }
        }

        public void OnTransferCompleted()
        {
            if (this != null && this.TransferCompleted != null)
            {
                //成功后触发事件
                this.TransferCompleted(this, new FinanceTransferApplyEventArgs(this));
            }
        }

        public void OnPost2Center()
        {
            if (this != null && this.Post2Center != null)
            {
                this.Post2Center(this, new FinanceTransferApplyEventArgs(this));
            }
        }

        public void UpdateOrderID()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.FundTransferApplies>(new { OrderID = this.OrderID }, item => item.ID == this.ID);
                }                
            }
            catch (Exception ex)
            {

            }
        }       
    }
}
