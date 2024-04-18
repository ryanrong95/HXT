using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Model;
using System;
using System.Linq;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 代理订单附加费用
    /// </summary>
    [Serializable]
    public class OrderPremium : ModelBase<Layer.Data.Sqls.ScCustoms.Orders, ScCustomsReponsitory>, IUnique, IPersistence
    {
        #region 属性

        public string OrderID { get; set; }

        public string OrderItemID { get; set; }

        /// <summary>
        /// 添加人
        /// </summary>
        public Admin Admin { get; set; }

        /// <summary>
        /// 费用类型
        /// 代理费、商检费、送货费、快递费、清关费、提货费 、停车费... ... 杂费
        /// </summary>
        public Enums.OrderPremiumType Type { get; set; }

        /// <summary>
        /// 费用名称，费用类型为杂费时填写
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 汇率
        /// 选择币种时
        /// </summary>
        public decimal Rate { get; set; }

        #endregion

        public OrderPremium()
        {
            this.Status = (int)Enums.Status.Normal;
            this.UpdateDate = this.CreateDate = DateTime.Now;
        }

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

        /// <summary>
        /// 持久化
        /// </summary>
        public override void Enter()
        {
            int count = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderPremiums>().Count(item => item.ID == this.ID);

            if (count == 0)
            {
                //主键ID（OrderPremium +8位年月日+6位流水号）
                this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.OrderPremium);
                this.Reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.OrderPremiums
                {
                    ID = this.ID,
                    OrderID = this.OrderID,
                    OrderItemID = this.OrderItemID,
                    AdminID = this.Admin.ID,
                    Type = (int)this.Type,
                    Name = this.Name,
                    Count = this.Count,
                    UnitPrice = this.UnitPrice,
                    Currency = this.Currency,
                    Rate = this.Rate,
                    Status = (int)this.Status,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    Summary = this.Summary
                });
            }
            else
            {
                this.Reponsitory.Update(new Layer.Data.Sqls.ScCustoms.OrderPremiums
                {
                    ID = this.ID,
                    OrderID = this.OrderID,
                    OrderItemID = this.OrderItemID,
                    AdminID = this.Admin.ID,
                    Type = (int)this.Type,
                    Name = this.Name,
                    Count = this.Count,
                    UnitPrice = this.UnitPrice,
                    Currency = this.Currency,
                    Rate = this.Rate,
                    Status = (int)this.Status,
                    CreateDate = this.CreateDate,
                    UpdateDate = DateTime.Now,
                    Summary = this.Summary
                }, item => item.ID == this.ID);
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
        }

        /// <summary>
        /// 删除
        /// </summary>
        public override void Abandon()
        {
            this.Reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderPremiums>(new { Status = Enums.Status.Delete }, item => item.ID == this.ID);

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
    }
}