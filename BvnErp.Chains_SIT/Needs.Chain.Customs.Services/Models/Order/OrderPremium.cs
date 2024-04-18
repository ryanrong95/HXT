using Needs.Linq;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 代理订单附加费用
    /// </summary>
    [Serializable]
    public class OrderPremium : IUnique, IPersistence, IFulError, IFulSuccess
    {
        #region 属性
        public string ID { get; set; }

        public string OrderID { get; set; }

        public string OrderItemID { get; set; }

        /// <summary>
        /// 添加人
        /// </summary>
        public Admin Admin { get; set; }
        public string  AdminID { get; set; }

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
        /// 汇率：选择币种时，默认带出实时汇率，允许跟单手动修改
        /// </summary>
        public decimal Rate { get; set; }

        public string StandardID { get; set; }

        public decimal? StandardPrice { get; set; }

        public string StandardCurrency { get; set; }

        public string StandardRemark { get; set; }

        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }

        #endregion

        #region 扩展属性

        /// <summary>
        /// 费用附件
        /// </summary>
        private OrderFiles files;
        public OrderFiles Files
        {
            get
            {
                if (files == null)
                {
                    using (var view = new Views.OrderFilesView())
                    {
                        var query = view.Where(item => item.OrderPremiumID == this.ID);
                        this.Files = new OrderFiles(query);
                    }
                }
                return this.files;
            }
            set
            {
                if (value == null)
                {
                    return;
                }

                this.files = new OrderFiles(value, new Action<OrderFile>(delegate (OrderFile item)
                {
                    item.OrderID = this.OrderID;
                    item.OrderPremiumID = this.ID;
                }));
            }
        }

        /// <summary>
        /// 杂费(非商检费)实收
        /// </summary>
        public IEnumerable<OrderReceived> OrderReceiveds { get; set; }

        /// <summary>
        /// 实收金额/客户实付金额
        /// </summary>
        public decimal PaidAmount
        {
            get
            {
                return this.OrderReceiveds.Sum(item => item.Amount * item.Rate);
            }
        }

        /// <summary>
        /// 实收日期/客户实付款日期
        /// </summary>
        public DateTime? PaymentDate
        {
            get
            {
                return this.OrderReceiveds.FirstOrDefault()?.CreateDate;
            }
        }

        #endregion

        public OrderPremium()
        {
            this.Status = Enums.Status.Normal;
            this.UpdateDate = this.CreateDate = DateTime.Now;

            this.AbandonSuccess += OrderPremium_Abandoned;
            this.EnterSuccess += OrderPremium_Successed;
        }

        public OrderPremium(OrderWhesPremium whesPremium, Enums.OrderPremiumType type) : this()
        {
            this.OrderID = whesPremium.OrderID;
            this.Type = type;
            this.Count = 1;
            this.UnitPrice = whesPremium.ApprovalPrice;
            this.Currency = MultiEnumUtils.ToCode<Enums.Currency>(Enums.Currency.CNY);
            this.Rate = 1M;
        }

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderPremiums>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    //主键ID（OrderPremium +8位年月日+6位流水号）
                    this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.OrderPremium);
                    reponsitory.Insert(this.ToLinq());
                }
                else
                {
                    reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
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
        }

        private void OrderPremium_Successed(object sender, SuccessEventArgs e)
        {
            var premium = (OrderPremium)e.Object;
            var order = new Views.OrdersView()[premium.OrderID];

            //记录日志
            var feeName = premium.Type == Enums.OrderPremiumType.OtherFee ? premium.Name : premium.Type.GetDescription();
            order.Log(premium.Admin, "跟单员【" + premium.Admin.RealName + "】新增了订单杂费【" + feeName +"】：数量【"+ premium.Count +"】，" +
                                     "单价【" + premium.UnitPrice +"】，币种【" + premium.Currency + "】，汇率【" + premium.Rate + "】");

            //保存费用附件
            foreach (var file in premium.Files)
            {
                file.Enter();
            }

            //跟单新增杂费或审核通过的库房费用插入到OrderReceipts表
            var orderReceivable = new OrderReceivable(order, premium);
            orderReceivable.Enter();
        }

        /// <summary>
        /// 删除
        /// </summary>
        public void Abandon()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderPremiums>(new { Status = Enums.Status.Delete }, item => item.ID == this.ID);
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

        private void OrderPremium_Abandoned(object sender, SuccessEventArgs e)
        {
            var premium = (OrderPremium)e.Object;
            var order = new Views.OrdersView()[premium.OrderID];

            //记录日志
            var feeName = premium.Type == Enums.OrderPremiumType.OtherFee ? premium.Name : premium.Type.GetDescription();
            order.Log(premium.Admin, "跟单员【" + premium.Admin.RealName + "】删除了订单杂费【" + feeName + "】：数量【" + premium.Count + "】，" +
                                     "单价【" + premium.UnitPrice + "】，币种【" + premium.Currency + "】，汇率【" + premium.Rate + "】");

            //删除费用附件及杂费应收
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderFiles>(new { Status = Enums.Status.Delete }, item => item.OrderPremiumID == premium.ID);
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderReceipts>(new { Status = Enums.Status.Delete }, item => item.FeeSourceID == premium.ID);
            }
        }

        //获取付款状态
        public Enums.OrderPremiumStatus GetPremiumStatus(decimal taxPoint)
        {
            if (this.PaidAmount == 0)
            {
                return Enums.OrderPremiumStatus.UnPay;
            }
            else
            {
                var receivableAmount = this.Count * this.UnitPrice * this.Rate * taxPoint;
                if (this.PaidAmount < receivableAmount)
                {
                    return Enums.OrderPremiumStatus.PartPaid;
                }
                else
                {
                    return Enums.OrderPremiumStatus.Paid;
                }
            }
        }
    }
}
