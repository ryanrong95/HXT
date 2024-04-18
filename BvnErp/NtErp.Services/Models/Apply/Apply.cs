//using Needs.Linq;
//using Needs.Overall;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using NtErp.Services.Extends;
//using NtErp.Wss.Sales.Services.Underly;
//using System.Xml.Linq;
//using System.Xml.Serialization;
//using Needs.Underly;
//using NtErp.Wss.Sales.Services.Model.Orders;
//using Needs.Utils.Serializers;
//using Needs.Model;

//namespace NtErp.Services.Models
//{
//    public class Apply :IApplies, IPersist
//    {
//        public Apply()
//        {
            
//            this.Properties = new Needs.Underly.Document();
//            this.CreateDate = this.UpdateDate = DateTime.Now;
//            this.Status = Needs.Underly.SelfStatus.Auditing;
//            this.Images = new NetFile[] { };
//        }
//        public Apply(XElement xml)
//        {
//            var entity = xml.XmlEleTo<Apply>();
//            this.UpdateDate = DateTime.Now;
//            this.Detail = entity.Detail;
//            this.Properties = entity.Properties;
//            this.Images = entity.Images;
//        }
//        public string ID { get; internal set; }
//        public string UserID { get; set; }
//        public ApplySource Source { get; set; }
//        public Needs.Underly.SelfStatus Status { get; set; }
//        public string Summary { get; set; }
//        public string AdminID { get; set; }
//        public string AdminName { get; set; }
//        public string Opinion { get; set; }
//        public DateTime CreateDate { get; set; }
//        public DateTime UpdateDate { get; set; }

//        Needs.Underly.Document properties;
//        public Needs.Underly.Document Properties
//        {
//            get
//            {
//                return this.properties;
//            }
//            set
//            {
//                this.properties = value;
//            }
//        }



//        /// <summary>
//        /// 订单
//        /// </summary>
//        [XmlIgnore]
//        public string OrderID
//        {
//            get
//            {
//                return this[nameof(this.OrderID)];
//            }
//            set
//            {
//                this[nameof(this.OrderID)] = value;
//            }
//        }
//        /// <summary>
//        /// 产品服务号
//        /// </summary>
//        [XmlIgnore]
//        public string ServiceOutputID
//        {
//            get
//            {
//                return this[nameof(this.ServiceOutputID)];
//            }
//            set
//            {
//                this[nameof(this.ServiceOutputID)] = value;
//            }
//        }

//        Needs.Model.Orders.ServiceDetail detail;
//        /// <summary>
//        /// 产品项详情
//        /// </summary>
//        public Needs.Model.Orders.ServiceDetail Detail
//        {
//            get
//            {
//                return this.detail;
//            }
//            set
//            {
//                if (this.detail != null)
//                {
//                    return;
//                }
//                this.detail = value;
//            }
//        }


//        /// <summary>
//        /// 退货数量
//        /// </summary>
//        [XmlIgnore]
//        public int Quantity
//        {
//            get
//            {
//                int outs;
//                if (!int.TryParse(this[nameof(this.Quantity)].ToString(), out outs))
//                {
//                    return 0;
//                }
//                return outs;
//            }
//            set
//            {
//                this[nameof(this.Quantity)] = value.ToString();
//            }
//        }

//        /// <summary>
//        /// 换货原因
//        /// </summary>
//        [XmlIgnore]
//        public string RepalceType
//        {
//            get
//            {
//                return this[nameof(this.RepalceType)];
//            }
//            set
//            {
//                this[nameof(this.RepalceType)] = value;
//            }
//        }
//        /// <summary>
//        /// 申请凭证
//        /// </summary>
//        [XmlIgnore]
//        public string Voucher
//        {
//            get
//            {
//                return this[nameof(this.Voucher)];
//            }
//            set
//            {
//                this[nameof(this.Voucher)] = value;
//            }
//        }
//        /// <summary>
//        /// 图片信息
//        /// </summary>
//        public NetFile[] Images { get; set; }

//        /// <summary>
//        /// 商品返回类型
//        /// </summary>
//        [XmlIgnore]
//        public string BackType
//        {
//            get
//            {
//                return this[nameof(this.BackType)];
//            }
//            set
//            {
//                this[nameof(this.BackType)] = value;
//            }
//        }

//        /// <summary>
//        /// 寄回地址
//        /// </summary>
//        [XmlIgnore]
//        public string BackAddress
//        {
//            get
//            {
//                return this[nameof(this.BackAddress)];
//            }
//            set
//            {
//                this[nameof(this.BackAddress)] = value;
//            }
//        }

//        /// <summary>
//        /// 联系人
//        /// </summary>
//        [XmlIgnore]
//        public string Contacts
//        {
//            get
//            {
//                return this[nameof(this.Contacts)];
//            }
//            set
//            {
//                this[nameof(this.Contacts)] = value;
//            }
//        }
//        /// <summary>
//        /// 手机号
//        /// </summary>
//        [XmlIgnore]
//        public string Mobile
//        {
//            get
//            {
//                return this[nameof(this.Mobile)];
//            }
//            set
//            {
//                this[nameof(this.Mobile)] = value;
//            }
//        }

//        /// <summary>
//        /// 邮箱
//        /// </summary>
//        [XmlIgnore]
//        public string Email
//        {
//            get
//            {
//                return this[nameof(this.Email)];
//            }
//            set
//            {
//                this[nameof(this.Email)] = value;
//            }
//        }

//        /// <summary>
//        /// QQ
//        /// </summary>
//        [XmlIgnore]
//        public string QQ
//        {
//            get
//            {
//                return this[nameof(this.QQ)];
//            }
//            set
//            {
//                this[nameof(this.QQ)] = value;
//            }
//        }

//        /// <summary>
//        /// 买家发货承运方式
//        /// </summary>
//        [XmlIgnore]
//        public string TransportTerm
//        {
//            get
//            {
//                return this[nameof(this.TransportTerm)];
//            }
//            set
//            {
//                this[nameof(this.TransportTerm)] = value;
//            }
//        }

//        /// <summary>
//        ///  买家户发货承运编号
//        /// </summary>
//        [XmlIgnore]
//        public string TransportCode
//        {
//            get
//            {
//                return this[nameof(this.TransportCode)];
//            }
//            set
//            {
//                this[nameof(this.TransportCode)] = value;
//            }
//        }


//        /// <summary>
//        /// 卖家发货承运方式
//        /// </summary>
//        [XmlIgnore]
//        public string SellerTransportTerm
//        {
//            get
//            {
//                return this[nameof(this.SellerTransportTerm)];
//            }
//            set
//            {
//                this[nameof(this.SellerTransportTerm)] = value;
//            }
//        }

//        /// <summary>
//        ///  卖家发货承运编号
//        /// </summary>
//        [XmlIgnore]
//        public string SellerTransportCode
//        {
//            get
//            {
//                return this[nameof(this.SellerTransportCode)];
//            }
//            set
//            {
//                this[nameof(this.SellerTransportCode)] = value;
//            }
//        }


//        public Elements this[string index]
//        {
//            get { return this.properties[index]; }
//            set { this.properties[index] = value; }
//        }


//        public event EnterSuccessHanlder EnterSuccess;

//        public void Enter()
//        {
//            using (var repository = new Layer.Data.Sqls.BvSsoReponsitory())
//            {
//                if (string.IsNullOrWhiteSpace(this.ID))
//                {
//                    this.ID = PKeySigner.Pick(PKeyType.Apply);

//                    repository.Insert(this.ToLinq());
//                }
//                else
//                {
//                    repository.Update(this.ToLinq(), item => item.ID == this.ID);
//                    repository.Delete<Layer.Data.Sqls.BvSso.ApplySearchers>(item => item.MainID == this.ID);
//                }
//                if (this.EnterSuccess != null)
//                {
//                    this.EnterSuccess(this, new EnterSuccessEventArgs(this.ID));
//                }
//            }
//        }
//    }
//}
