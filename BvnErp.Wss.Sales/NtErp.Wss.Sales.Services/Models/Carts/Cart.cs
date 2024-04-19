using NtErp.Wss.Sales.Services.Extends;
using NtErp.Wss.Sales.Services.Underly;
using NtErp.Wss.Sales.Services.Underly.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace NtErp.Wss.Sales.Services.Models.Carts
{
    /// <summary>
    /// 购物车项
    /// </summary>
    public class Cart
    {
        public Cart()
        {
            this.CreateDate = this.UpdateDate = DateTime.Now;
        }

        public Cart(XElement xml)
        {
            this.CreateDate = DateTime.Now;
            var entity = xml.XmlEleTo<Cart>();
            this.Product = entity.Product;
            this.District = entity.District;
            this.Price = entity.Price;
            this.Currency = entity.Currency;
        }

        //Document properties;
        //public Document Properties
        //{
        //    get
        //    {
        //        return this.properties;
        //    }
        //    set
        //    {
        //        this.properties = value;
        //    }
        //}

        internal Cart(string id)
        {
            this.ServiceOutputID = id;
        }

        #region 属性
        [XmlIgnore]
        public string ID
        {
            get
            {
                return this.ServiceOutputID;
            }
        }

        /// <summary>
        /// 购买人
        /// </summary>
        [XmlIgnore]
        public string UserID { get; set; }

        string serviceInputID;
        /// <summary>
        /// 进项服务ID
        /// </summary>
        [XmlIgnore]
        public string ServiceInputID
        {
            get
            {
                if (string.IsNullOrWhiteSpace(serviceInputID))
                {
                    if (string.IsNullOrWhiteSpace(this.Product.ServiceInputID)) // 产品没有进项ID，就赋值等于销项ID
                    {
                        this.serviceInputID = this.ServiceOutputID;
                    }
                    else
                    {
                        this.serviceInputID = this.Product.ServiceInputID;
                    }

                }
                return this.serviceInputID;
            }
            set
            {
                this.serviceInputID = value;
            }
        }
        /// <summary>
        /// 销项服务ID
        /// </summary>
        [XmlIgnore]
        public string ServiceOutputID { get; set; }

        CartProduct product;
        /// <summary>
        /// 产品
        /// </summary>
        public CartProduct Product
        {
            get
            {
                if (this.product == null)
                {
                    this.product = new CartProduct();
                }
                return this.product;
            }
            set
            {
                this.product = value;
            }
        }

        /// <summary>
        /// 产品Sign
        /// </summary>
        [XmlIgnore]
        public string ProductSign
        {
            get
            {
                return this.Product.ProductSign;
            }
            set
            {
                this.Product.ProductSign = value;
            }
        }
        /// <summary>
        /// 数量
        /// </summary>
        [XmlIgnore]
        public int Quantity { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        [XmlIgnore]
        public string Supplier
        {
            get
            {
                return this.Product.Supplier;
            }
        }

        /// <summary>
        /// 客户编号
        /// </summary>
        [XmlIgnore]
        public string CustomerCode
        {
            get
            {
                return this.Product.CustomerCode;
            }
            set
            {
                this.Product.CustomerCode = value;
            }
        }
        [XmlIgnore]
        public DateTime CreateDate { get; set; }
        [XmlIgnore]
        public DateTime UpdateDate { get; set; }

        #endregion

        #region 后台虚拟产品

        /// <summary>
        /// 交货地
        /// </summary>
        public District District { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { get; set; }

        public decimal Total
        {
            get
            {
                return this.Quantity * this.Price;
            }
        }

        List<Log> logs;
        public List<Log> Logs
        {
            get
            {
                if (logs == null)
                {
                    this.logs = new List<Log>();
                }
                return this.logs;
            }
            set
            {
                if (this.logs == null || this.logs.Count == 0)
                {
                    this.logs = value;
                    return;
                }

                throw new NotSupportedException("Do not support multiple assignment!");
            }
        }

        #endregion

        #region 持久化

        public event ErrorHanlder Error;
        public event EnterSuccessHanlder EnterSuccess;
        public event AbandonSuccessHanlder AbandonSuccess;

        public void Enter()
        {
            using (var repository = new Layer.Data.Sqls.BvOrdersReponsitory())
            {
                if (string.IsNullOrWhiteSpace(this.ServiceOutputID))
                {
                    this.ServiceOutputID = Needs.Overall.PKeySigner.Pick(Services.PKeyType.Cart);
                    if (string.IsNullOrWhiteSpace(this.ServiceInputID))
                    {
                        this.ServiceInputID = this.ServiceOutputID;
                    }
                    repository.Insert(this.ToLinq());
                }
                else
                {
                    this.UpdateDate = DateTime.Now;
                    repository.Update(this.ToLinq(), item => item.ServiceOutputID == this.ServiceOutputID);
                }
            }
            if (this.EnterSuccess != null)
            {
                this.EnterSuccess(this, new EnterSuccessEventArgs(this.ID));
            }
        }

        public void Abandon()
        {
            using (var repository = new Layer.Data.Sqls.BvOrdersReponsitory())
            {
                repository.Delete<Layer.Data.Sqls.BvOrders.Carts>(item => item.ServiceOutputID == this.ServiceOutputID);
                if (this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new AbandonSuccessEventArgs(this.ID));
                }
            }
        }

        #endregion

        public class Log
        {
            public Log()
            {
                this.UpdateDate = DateTime.Now;
            }
            public string AdminID { get; set; }
            /// <summary>
            /// 操作类型 1 add  2 update
            /// </summary>
            public int Type { get; set; }
            public DateTime UpdateDate { get; set; }

        }
    }
}
