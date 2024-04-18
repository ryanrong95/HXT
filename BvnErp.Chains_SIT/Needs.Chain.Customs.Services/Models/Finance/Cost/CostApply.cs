using Needs.Ccs.Services.Hanlders;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class CostApply : IUnique, IPersistence, IFulError, IFulSuccess
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; } = string.Empty;

        /// <summary>
        /// 收款方名称
        /// </summary>
        public string PayeeName { get; set; } = string.Empty;

        /// <summary>
        /// 收款方账号
        /// </summary>
        public string PayeeAccount { get; set; } = string.Empty;

        /// <summary>
        /// 收款方银行
        /// </summary>
        public string PayeeBank { get; set; } = string.Empty;
        /// <summary>
        /// 收款方账号ID
        /// </summary>
        public string PayeeAccountID { get; set; }


        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; } = string.Empty;

        /// <summary>
        /// 状态
        /// </summary>
        public Enums.CostStatusEnum CostStatus { get; set; }

        /// <summary>
        /// 申请提交人
        /// </summary>
        public string AdminID { get; set; } = string.Empty;

        /// <summary>
        /// 付款时间
        /// </summary>
        public DateTime? PayTime { get; set; }


        /// <summary>
        /// Status
        /// </summary>
        public Enums.Status Status { get; set; }

        /// <summary>
        /// CreateDate
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// UpdateDate
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { get; set; } = string.Empty;

        /// <summary>
        /// 区分费用申请类型（个人申请费用、银行自动扣款）
        /// </summary>
        public Enums.MoneyTypeEnum MoneyType { get; set; }

        /// <summary>
        /// 是否使用现金账户
        /// </summary>
        public Enums.CashTypeEnum CashType { get; set; }

        /// <summary>
        /// 订单项
        /// </summary>
        CostApplyItems items;
        public CostApplyItems Items
        {
            get
            {
                if (items == null)
                {
                    using (var view = new Views.CostApplyItemsView())
                    {
                        var query = view.Where(item => item.CostApplyID == this.ID && item.Status == Enums.Status.Normal);
                        this.Items = new CostApplyItems(query);
                    }
                }
                return this.items;
            }
            set
            {
                if (value == null)
                {
                    return;
                }

                this.items = new CostApplyItems(value, new Action<CostApplyItem>(delegate (CostApplyItem item)
                {
                    item.CostApplyID = this.ID;
                }));
            }
        }

        public event ErrorHanlder EnterError;
        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event SuccessHanlder EnterSuccess;

        public event DyjFeeApplyEnterHanlder FeeApplyEnter;

        public CostApply()
        {
            this.Status = Enums.Status.Normal;
            this.UpdateDate = this.CreateDate = DateTime.Now;

            this.FeeApplyEnter += CostApply_FeeApplyEnter;
        }

        public void Abandon()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.CostApplies>(
                    new
                    {
                        UpdateDate = DateTime.Now,
                        Status = (int)Enums.Status.Delete
                    }, item => item.ID == this.ID);
            }
            this.OnAbandonSuccess();
        }

        virtual protected void OnAbandonSuccess()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                //成功后触发事件
                this.AbandonSuccess(this, new SuccessEventArgs(this));
            }
        }

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.CostApplies>().Count(item => item.ID == this.ID);
                if (count == 0)
                {
                    reponsitory.Insert<Layer.Data.Sqls.ScCustoms.CostApplies>(new Layer.Data.Sqls.ScCustoms.CostApplies
                    {
                        ID = this.ID,
                        PayeeName = this.PayeeName,
                        PayeeAccount = this.PayeeAccount,
                        PayeeBank = this.PayeeBank,
                        PayeeAccountID = this.PayeeAccountID,
                        Amount = this.Amount,
                        Currency = this.Currency,
                        CostStatus = (int)this.CostStatus,
                        AdminID = this.AdminID,
                        PayTime = this.PayTime,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary,
                        MoneyType = (int)this.MoneyType,
                        CashType = (int)this.CashType,
                    });

                    //批量插入
                    reponsitory.Insert(this.Items.Select(item => new Layer.Data.Sqls.ScCustoms.CostApplyItems
                    {
                        ID = item.ID,
                        CostApplyID = this.ID,
                        FeeType = (int)item.FeeType,
                        FeeDesc = item.FeeDesc,
                        Amount = item.Amount,
                        Status = (int)item.Status,
                        CreateDate = item.CreateDate,
                        UpdateDate = item.UpdateDate,
                        Summary = item.Summary,
                        EmployeeID = item.EmployeeID

                    }).ToArray());
                }
                else
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.CostApplies>(new
                    {
                        PayeeName = this.PayeeName,
                        PayeeAccount = this.PayeeAccount,
                        PayeeBank = this.PayeeBank,
                        PayeeAccountID = this.PayeeAccountID,
                        Amount = this.Amount,
                        Currency = this.Currency,
                        CostStatus = (int)this.CostStatus,
                        AdminID = this.AdminID,
                        PayTime = this.PayTime,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary,
                        MoneyType = (int)this.MoneyType,
                        CashType = (int)this.CashType,
                    }, item => item.ID == this.ID);

                    //把原来的项置为删除状态
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.CostApplyItems>(new { Status = Enums.Status.Delete }, item => item.CostApplyID == this.ID);

                    //批量插入
                    reponsitory.Insert(this.Items.Select(item => new Layer.Data.Sqls.ScCustoms.CostApplyItems
                    {
                        ID = item.ID,
                        CostApplyID = this.ID,
                        FeeType = (int)item.FeeType,
                        FeeDesc = item.FeeDesc,
                        Amount = item.Amount,
                        Status = (int)item.Status,
                        CreateDate = item.CreateDate,
                        UpdateDate = item.UpdateDate,
                        Summary = item.Summary,

                    }).ToArray());
                }
            }
            this.OnEnterSuccess();
        }

        virtual public void OnEnterSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }

            //费用录入调用大赢家接口
            if (this != null && this.FeeApplyEnter != null)
            {
                //非银行自动扣款，并且银行账号真实（）
                if (this.MoneyType == Enums.MoneyTypeEnum.IndividualApply && this.PayeeAccount.Length > 5)
                {
                    this.FeeApplyEnter(this, new DyjFeeApplyEnterEventArgs(this));
                }
            }
        }

        private void CostApply_FeeApplyEnter(object sender, DyjFeeApplyEnterEventArgs e)
        {
            try
            {
                var result = new Finance.DyjFinance.DyjFeeApply(e.DyjFeeApply);
                var data = result.PostToDYJ();
                //TODO
                //收款接口返回值，保存进收款记录
                if (!string.IsNullOrEmpty(data))
                {
                    var DyjID = int.Parse(data.Split('.')[0]);
                    var DyjCheckID = int.Parse(data.Split('|')[1]);
                    using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                    {
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.CostApplies>(new
                        {
                            DyjID = DyjID,
                            DyjCheckID = DyjCheckID
                        }, item => item.ID == this.ID);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.CcsLog("付款调用大赢家错误");
            }

        }

        /// <summary>
        /// 根据 CostApplyID 删除
        /// </summary>
        public void AbandonFileFiles()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.CostApplyFiles>(new
                {
                    Status = (int)Enums.Status.Delete,
                }, item => item.CostApplyID == this.ID);
            }
        }

    }
}
