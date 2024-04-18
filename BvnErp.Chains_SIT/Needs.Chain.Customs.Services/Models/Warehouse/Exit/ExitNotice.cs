using Needs.Linq;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    [Serializable]
    public class ExitNotice : IUnique, IPersist, IFulError, IFulSuccess
    {
        public string ID { get; set; }

        public Admin Admin { get; set; }

        public string AdminID { get; set; } = string.Empty;

        public Interfaces.IOrder Order { get; set; }

        public string OrderId { get; set; }
        /// <summary>
        /// 报关单
        /// </summary>
        public virtual DecHead DecHead { get; set; }

        public string DecHeadID { get; set; } = string.Empty;

        public Enums.WarehouseType WarehouseType { get; set; }

        public Enums.ExitType ExitType { get; set; }

        public Enums.ExitNoticeStatus ExitNoticeStatus
        {
            get; set;
        }

        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }
        /// <summary>
        ///打印状态
        /// </summary>
        public Enums.IsPrint? IsPrint { get; set; }


        public int? IsPrintInt { get; set; }


        public DateTime? OutStockTime { get; set; }


        ExitNoticeItems items;
        public ExitNoticeItems Items
        {
            get
            {
                if (items == null)
                {
                    using (var view = new Views.ExitNoticeItemView())
                    {
                        var query = view.Where(item => item.ExitNoticeID == this.ID);
                        this.Items = new ExitNoticeItems(query);
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

                this.items = new ExitNoticeItems(value, new Action<ExitNoticeItem>(delegate (ExitNoticeItem item)
                {
                    item.ExitNoticeID = this.ID;
                }));
            }
        }

        /// <summary>
        /// 出库交货信息
        /// </summary>
        public ExitDeliver ExitDeliver { get; set; }

        /// <summary>
        /// 提货单
        /// </summary>
        public LadingBill LadingBill
        {
            get
            {
                return new LadingBill(this);
            }
        }

        /// <summary>
        /// 送货单
        /// </summary>
        public DeliveryBill DeliveryBill
        {
            get
            {
                return new DeliveryBill(this);
            }
        }

        /// <summary>
        /// 快递单
        /// </summary>
        public ExpressBill ExpressBill
        {
            get
            {
                return new ExpressBill(this);
            }
        }

        public KDDRequestModel KDDRequestModel
        {
            get
            {
                return new KDDRequestModel(this);
            }
        }

        public ExitNotice()
        {
            this.CreateDate = this.UpdateDate = DateTime.Now;
            this.Status = Enums.Status.Normal;
            this.ExitNoticeStatus = Enums.ExitNoticeStatus.UnExited;
            this.IsPrint = Enums.IsPrint.UnPrint;
        }

        public event ErrorHanlder EnterError;
        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event SuccessHanlder EnterSuccess;

        /// <summary>
        /// 持久化
        /// </summary>
        virtual public void Enter()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExitNotices>().Count(item => item.ID == this.ID);
                    if (count == 0)
                    {
                        this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.ExitNotice);
                        reponsitory.Insert(this.ToLinq());
                    }
                    else
                    {
                        this.UpdateDate = DateTime.Now;
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
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.ExitNotices>(new { Status = Enums.Status.Delete }, item => item.ID == this.ID);
                }
                this.OnAbandonSuccess(); ;
            }
            catch (Exception ex)
            {
                this.AbandonError(this, new ErrorEventArgs(ex.Message));
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

        virtual protected void OnAbandonSuccess()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                //成功后触发事件
                this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
            }
        }
    }

    /// <summary>
    /// 提货单（深圳出库）
    /// </summary>
    public class LadingBill
    {
        public string ExitNoticeID { get; set; }

        public string OrderID { get; set; }

        /// <summary>
        /// 提货单号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// 提货人
        /// </summary>
        public string DeliveryName { get; set; }

        /// <summary>
        /// 提货人电话
        /// </summary>
        public string DeliveryTel { get; set; }

        /// <summary>
        /// 证件类型
        /// </summary>
        public string IDType { get; set; }

        /// <summary>
        /// 证件号码
        /// </summary>
        public string IDCard { get; set; }

        /// <summary>
        /// 提货件数
        /// </summary>
        public int PackNo { get; set; }

        /// <summary>
        /// 提货时间
        /// </summary>
        public DateTime DeliveryTime { get; set; }

        public LadingBill(ExitNotice ExitNotice)
        {
            this.ExitNoticeID = ExitNotice.ID;
            this.OrderID = ExitNotice.Order.ID;
            this.Code = ExitNotice.ExitDeliver.Code;
            this.ClientName = ExitNotice.ExitDeliver.Name;
            this.DeliveryName = ExitNotice.ExitDeliver.Consignee.Name;
            this.DeliveryTel = ExitNotice.ExitDeliver.Consignee.Mobile;
            this.IDType = ExitNotice.ExitDeliver.Consignee.IDType.GetDescription();
            this.IDCard = ExitNotice.ExitDeliver.Consignee.IDNumber;
            this.PackNo = ExitNotice.ExitDeliver.PackNo;
            this.DeliveryTime = ExitNotice.ExitDeliver.DeliverDate;
        }
    }

    /// <summary>
    /// 送货单（深圳出库）
    /// </summary>
    public class DeliveryBill
    {
        public string ExitNoticeID { get; set; }

        public string OrderID { get; set; }

        /// <summary>
        /// 送货单号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public string Contactor { get; set; }

        /// <summary>
        /// 联系人电话
        /// </summary>
        public string ContactTel { get; set; }

        /// <summary>
        /// 送货地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 司机名称
        /// </summary>
        public string DriverName { get; set; }

        /// <summary>
        /// 司机电话
        /// </summary>
        public string DriverTel { get; set; }

        /// <summary>
        /// 车牌号
        /// </summary>
        public string License { get; set; }

        /// <summary>
        /// 送货件数
        /// </summary>
        public int PackNo { get; set; }

        /// <summary>
        /// 送货时间
        /// </summary>
        public DateTime DeliveryTime { get; set; }

        public DeliveryBill(ExitNotice ExitNotice)
        {
            this.ExitNoticeID = ExitNotice.ID;
            this.OrderID = ExitNotice.Order.ID;
            this.Code = ExitNotice.ExitDeliver.Code;
            this.ClientName = ExitNotice.ExitDeliver.Name;
            this.Contactor = ExitNotice.ExitDeliver?.Deliver.Contact;
            this.ContactTel = ExitNotice.ExitDeliver?.Deliver.Mobile;
            this.Address = ExitNotice.ExitDeliver?.Deliver.Address;
            this.DriverName = ExitNotice.ExitDeliver?.Deliver?.Driver?.Name;
            this.DriverTel = ExitNotice.ExitDeliver?.Deliver?.Driver?.Mobile;
            this.License = ExitNotice.ExitDeliver?.Deliver?.Vehicle?.License;
            this.PackNo = ExitNotice.ExitDeliver.PackNo;
            this.DeliveryTime = ExitNotice.ExitDeliver.DeliverDate;
        }
    }

    /// <summary>
    /// 快递单（深圳出库）
    /// </summary>
    public class ExpressBill
    {
        public string ExitNoticeID { get; set; }

        public string OrderID { get; set; }

        /// <summary>
        /// 运单号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 查询标记
        /// </summary>
        public string QueryMark { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public string Contactor { get; set; }

        /// <summary>
        /// 联系人电话
        /// </summary>
        public string ContactTel { get; set; }

        /// <summary>
        /// 送货地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 快递公司
        /// </summary>
        public string ExpressComp { get; set; }

        /// <summary>
        /// 快递方式
        /// </summary>
        public string ExpressTy { get; set; }

        /// <summary>
        /// 付费方式
        /// </summary>
        public Enums.PayType? ExpressPayType { get; set; }

        /// <summary>
        /// 送货件数
        /// </summary>
        public int PackNo { get; set; }

        /// <summary>
        /// 送货时间
        /// </summary>
        public DateTime DeliveryTime { get; set; }

        public ExpressBill(ExitNotice ExitNotice)
        {
            this.ExitNoticeID = ExitNotice.ID;
            this.OrderID = ExitNotice.Order.ID;
            //this.Code = ExitNotice.ExitDeliver.Code;
            this.ClientName = ExitNotice.ExitDeliver.Name;
            this.QueryMark = ExitNotice.ExitDeliver?.Expressage.QueryMark;
            this.Contactor = ExitNotice.ExitDeliver?.Expressage.Contact;
            this.ContactTel = ExitNotice.ExitDeliver?.Expressage.Mobile;
            this.Address = ExitNotice.ExitDeliver?.Expressage.Address;
            this.ExpressComp = ExitNotice.ExitDeliver?.Expressage?.ExpressCompany.Name;
            this.ExpressTy = ExitNotice.ExitDeliver?.Expressage?.ExpressType.TypeName;
            this.ExpressPayType = ExitNotice.ExitDeliver?.Expressage?.PayType;
            this.PackNo = ExitNotice.ExitDeliver.PackNo;
            this.DeliveryTime = ExitNotice.ExitDeliver.DeliverDate;
            this.Code = ExitNotice.ExitDeliver?.Expressage.WaybillCode;
        }
    }

    /// <summary>
    /// 快递返回结果
    /// </summary>
    public class KDDResultModel
    {
        public string EBusinessID { get; set; }
        /// <summary>
        ///面单打印模板
        /// </summary>
        public string PrintTemplate { get; set; }
        /// <summary>
        /// 子模板个数
        /// </summary>
        public int SubCount { get; set; }
        /// <summary>
        ///子模板单号
        /// </summary>
        public string[] SubOrders { get; set; }
        /// <summary>
        /// 子模板面单
        /// </summary>
        public string[] SubPrintTemplates { get; set; }
        public bool Success { get; set; }
        public string ResultCode { get; set; }
        public string Reason { get; set; }
        public OrderModel Order { get; set; }
        public class OrderModel
        {
            /// <summary>
            /// 订单编号
            /// </summary>
            public string OrderCode { get; set; }
            /// <summary>
            /// 快递公司编号
            /// </summary>
            public string ShipperCode { get; set; }
            /// <summary>
            /// 快递单号
            /// </summary>
            public string LogisticCode { get; set; }
            public string DestinatioCode { get; set; }
            public string KDNOrderCode { get; set; }
            public string OriginCode { get; set; }
        }
    }

    /// <summary>
    /// 快递请求数据
    /// </summary>
    public class KDDRequestModel
    {
        public string OrderCode { get; set; }

        public string ShipperCode { get; set; }

        public string CustomerName { get; set; }

        public string CustomerPwd { get; set; }

        public string MonthCode { get; set; }

        public int PayType { get; set; }

        public int ExpType { get; set; }

        public double Cost { get; set; }

        public double OtherCost { get; set; }

        public Sender Sender { get; set; }

        public Receiver Receiver { get; set; }

        public Commodity[] Commodity { get; set; }

        public string Weight { get; set; }

        public int Quantity { get; set; }

        public string Volume { get; set; }

        public string Remark { get; set; }

        public string TemplateSize { get; set; }

        public string IsReturnPrintTemplate { get; set; }

        public KDDRequestModel()
        {

        }

        public KDDRequestModel(ExitNotice ExitNotice)
        {
            this.OrderCode = ExitNotice.Order.ID + "-" + ExitNotice.ID.Substring(ExitNotice.ID.Length - 6, 6);
            this.ShipperCode = ExitNotice.ExitDeliver.Expressage.ExpressCompany.Code;
            this.CustomerName = ExitNotice.ExitDeliver.Expressage.ExpressCompany?.CustomerName;
            this.CustomerPwd = ExitNotice.ExitDeliver.Expressage.ExpressCompany?.CustomerPwd;
            this.MonthCode = ExitNotice.ExitDeliver.Expressage.ExpressCompany?.MonthCode;
            this.PayType = (int)ExitNotice.ExitDeliver.Expressage.PayType;
            this.ExpType = ExitNotice.ExitDeliver.Expressage.ExpressType.TypeValue;
            this.Cost = 0;
            this.OtherCost = 0;
            this.Sender = new Sender()
            {
                Company = PurchaserContext.Current.CompanyName,
                Name = ExitNotice.Order.Client.ServiceManager.RealName,
                Mobile = ExitNotice.Order.Client.ServiceManager.Mobile,
                ProvinceName = "广东省",
                CityName = "深圳市",
                ExpAreaName = "龙岗区",
                Address = "坂田吉华路393号英达丰科技园",
            };
            this.Receiver = this.GetReceiver(ExitNotice.ExitDeliver.Expressage.Address);
            this.Receiver.Company = ExitNotice.Order.Client.Company.Name;
            this.Receiver.Name = ExitNotice.ExitDeliver.Expressage.Contact;
            this.Receiver.Mobile = ExitNotice.ExitDeliver.Expressage.Mobile;
            this.Commodity = new Commodity[]{
                new Commodity()
                {
                    GoodsName="电子元器件",
                }
            };
            this.Remark = "小心轻放";
            this.TemplateSize = "210";
            this.IsReturnPrintTemplate = "1";
            if (this.ShipperCode == "KYSY")
            {
                //跨域的默认数量都填1
                this.Quantity = 1;
            }
            else
            {
                this.Quantity = ExitNotice.ExitDeliver.PackNo;
            }
        }

        public Receiver GetReceiver(string Address,string CompanyName)
        {
            var address = new Dictionary<string, string>();
            address = this.HandleAddress(Address);
            Receiver receiver = new Receiver();
            receiver.ProvinceName = address["Province"];
            receiver.CityName = address["City"];
            receiver.ExpAreaName = address["Area"];
            //寄件地址后面追加公司名称
            receiver.Address = address["DetailsAddress"]+"("+CompanyName+ ")";
            return receiver;
        }

        public Receiver GetReceiver(string Address)
        {
            var address = new Dictionary<string, string>();
            address = this.HandleAddress(Address);
            Receiver receiver = new Receiver();
            receiver.ProvinceName = address["Province"];
            receiver.CityName = address["City"];
            receiver.ExpAreaName = address["Area"];
            receiver.Address = address["DetailsAddress"];
            return receiver;
        }
        public Sender GetSender(string Address)
        {
            var address = new Dictionary<string, string>();
            address = this.HandleAddress(Address);
            Sender sender = new Sender();
            sender.ProvinceName = address["Province"];
            sender.CityName = address["City"];
            sender.ExpAreaName = address["Area"];
            sender.Address = address["DetailsAddress"];
            return sender;
        }


        private Dictionary<string, string> HandleAddress(string Address)
        {
            var Province = "";
            var City = "";
            var Area = "";
            var DetailsAddress = "";
            if (Address.Split(' ').Length == 3)
            {
                Province = Address.Split(' ')[0].Trim();
                City = Address.Split(' ')[0].Trim() + "市";
                Area = Address.Split(' ')[1].Trim();
                DetailsAddress = Address.Split(' ')[2].Trim();
            }
            else
            {
                Province = Address.Split(' ')[0].Trim();
                if (Province == "内蒙古" || Province == "西藏")
                    Province = Address.Split(' ')[0] + "自治区";
                if (Province == "新疆")
                    Province = Address.Split(' ')[0] + "维吾尔自治区";
                if (Province == "广西")
                    Province = Address.Split(' ')[0] + "壮族自治区";
                if (Province == "宁夏")
                    Province = Address.Split(' ')[0] + "回族自治区";
                else
                {
                    Province = Address.Split(' ')[0] + "省";
                }
                City = Address.Split(' ')[1].Trim();
                Area = Address.Split(' ')[2].Trim();
                DetailsAddress = Address.Split(' ')[3].Trim();
            }
            var DicAddres = new Dictionary<string, string>();
            DicAddres.Add("Province", Province);
            DicAddres.Add("City", City);
            DicAddres.Add("Area", Area);
            DicAddres.Add("DetailsAddress", DetailsAddress);
            return DicAddres;
        }
    }

    /// <summary>
    /// 发件人
    /// </summary>
    public class Sender
    {
        public string Company { get; set; }

        public string Name { get; set; }

        public string Mobile { get; set; }

        public string ProvinceName { get; set; }

        public string CityName { get; set; }

        public string ExpAreaName { get; set; }

        public string Address { get; set; }
    }

    /// <summary>
    /// 收件人
    /// </summary>
    public class Receiver
    {
        public string Company { get; set; }

        public string Name { get; set; }

        public string Mobile { get; set; }

        public string ProvinceName { get; set; }

        public string CityName { get; set; }

        public string ExpAreaName { get; set; }

        public string Address { get; set; }
    }

    /// <summary>
    /// 商品
    /// </summary>
    public class Commodity
    {
        public string GoodsName { get; set; }
        public string GoodsCode { get; set; }
        public long Goodsquantity { get; set; }
        public double GoodsPrice { get; set; }
        public double GoodsWeight { get; set; }
        public string GoodsDesc { get; set; }
        public double GoodsVol { get; set; }
    }
}