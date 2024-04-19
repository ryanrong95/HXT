
using NtErp.Wss.Sales.Services.Models;
using NtErp.Wss.Sales.Services.Models.Orders;
using NtErp.Wss.Sales.Services.Models.Orders.Commodity;
using NtErp.Wss.Sales.Services.Underly;
using NtErp.Wss.Sales.Services.Underly.Collections;
using NtErp.Wss.Sales.Services.Views;
using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NtErp.Wss.Sales.Services.Model.Orders
{
    /// <summary>
    /// 服务详情
    /// </summary>
    public sealed class ServiceDetail : PremiumBase<SaleProduct>, IAlter
    {
        Order father;
        public ServiceDetail()
        {
            this.Status = AlterStatus.Normal;
            this.AlterDate = DateTime.Now;
            this.Waybills = new List<Waybill>();
        }

        #region 属性
        /// <summary>
        /// 名称
        /// </summary>
        [XmlIgnore]
        public string Name
        {
            get
            {
                return this.Product.Name;
            }
            set
            {
                this.Product.Name = value;
            }
        }

        /// <summary>
        /// 进项服务ID
        /// </summary>
        public string ServiceInputID { get; set; }
        /// <summary>
        /// 销项服务ID
        /// </summary>
        public string ServiceOutputID { get; set; }

        /// <summary>
        /// 采购商标识
        /// </summary>
        [XmlIgnore]
        public string PurchaserSign { get { return this.Product.PurchaserSign; } }

        public DateTime AlterDate { get; set; }

        public AlterStatus Status { get; set; }

        /// <summary>
        /// 如果客户有询问要求可以添加在这里
        /// </summary>
        public string Summary { get; set; }

        District district;
        /// <summary>
        /// 交货地
        /// </summary>
        [XmlIgnore]
        override public District District
        {
            get
            {
                if (father != null)
                {
                    this.district = father.District;
                }
                return this.district;
            }
            set
            {
                this.district = value;
            }
        }

        Currency currency;
        /// <summary>
        /// 交易货币
        /// </summary>
        [XmlIgnore]
        override public Currency Currency
        {
            get
            {
                if (father != null)
                {
                    this.currency = father.Currency;
                }
                return this.currency;
            }
            set
            {
                this.currency = value;
            }
        }

        public override int GetHashCode()
        {
            return this.ServiceOutputID.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is ServiceDetail ? obj.GetHashCode() == this.GetHashCode() : false;
        }

        /// <summary>
        /// 已重写  可以相应Order的变更事件
        /// </summary>
        public override int Quantity
        {
            get
            {
                return base.Quantity;
            }
            set
            {
                var old = this.SubTotal;

                if (base.Quantity == value)
                {
                    return;
                }

                base.Quantity = value;
            }
        }

        public override decimal Price
        {
            get
            {
                if (this.Product != null && base.Price == 0)
                {
                    base.Price = this.Product.GetPrice(this.Quantity);
                }
                return base.Price;
            }

            set
            {
                var old = this.SubTotal;

                if (base.Price == value)
                {
                    return;
                }

                base.Price = value;
            }
        }

        /// <summary>
        /// 操作人
        /// </summary>
        public string AdminID { get; set; }

        /// <summary>
        /// 运单
        /// </summary>
        public List<Waybill> Waybills { get; set; }

        /// <summary>
        /// 商品帐
        /// </summary>
        public Commodity Commodity
        {
            get
            {
                return new Commodity(this);
            }
        }
        /// <summary>
        /// 是否已发货
        /// </summary>
        public bool IsSend { get; set; }

        #endregion

        public void AddWaybill(Waybill waybill)
        {
            this.Waybills.Add(waybill);
            father.Enter();
        }

        /// <summary>
        /// 发货
        /// </summary>
        /// <param name="count">数量</param>
        /// <param name="waybill">运单</param>
        public void Sent(int count, Waybill waybill)
        {
            // 商品帐
            if (this.Commodity.Unsent >= count)
            {
                var input = new CommodityInputsView().Where(t => t.ServiceOuputID == this.ServiceOutputID).OrderByDescending(t => t.CreateDate).First();
                new CommodityOutput
                {
                    InputID = input.ID,
                    OrderID = father.ID,
                    ServiceInputID = this.ServiceInputID,
                    Count = count
                }.Enter();
                this.IsSend = true;
                father.IsSend = true;
                this.Waybills.Add(waybill);
                father.Enter();
            }
            else
            {
                throw new Exception("The delivery count is not correct!");
            }
        }


        // decimal changeValue;

        //public bool ReadChange(out Receipt receipt)
        //{
        //    //一大套在这里做
        //    if (changeValue == 0)
        //    {
        //        receipt = null;
        //        return false;
        //    }
        //    else
        //    {
        //        this.father.Details.Log(this);
        //        changeValue = 0;
        //        receipt = new Receipt
        //        {
        //            Amount = changeValue,
        //            CreateTime = DateTime.Now,
        //            Drawee = "",
        //            PaymentMethod = PaymentMethod.Wallet,
        //            Summary = "",
        //        };
        //        return true;
        //    }
        //}
    }
}
