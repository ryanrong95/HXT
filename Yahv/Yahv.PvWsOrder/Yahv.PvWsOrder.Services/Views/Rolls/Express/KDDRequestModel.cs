using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvWsOrder.Services.Views
{
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

        //public KDDRequestModel(ExitNotice ExitNotice)
        //{
        //    this.OrderCode = ExitNotice.Order.ID + "-" + ExitNotice.ID.Substring(ExitNotice.ID.Length - 6, 6);
        //    this.ShipperCode = ExitNotice.ExitDeliver.Expressage.ExpressCompany.Code;
        //    this.CustomerName = ExitNotice.ExitDeliver.Expressage.ExpressCompany?.CustomerName;
        //    this.CustomerPwd = ExitNotice.ExitDeliver.Expressage.ExpressCompany?.CustomerPwd;
        //    this.MonthCode = ExitNotice.ExitDeliver.Expressage.ExpressCompany?.MonthCode;
        //    this.PayType = (int)ExitNotice.ExitDeliver.Expressage.PayType;
        //    this.ExpType = ExitNotice.ExitDeliver.Expressage.ExpressType.TypeValue;
        //    this.Cost = 0;
        //    this.OtherCost = 0;
        //    this.Sender = new Sender()
        //    {
        //        Company = PurchaserContext.Current.CompanyName,
        //        Name = ExitNotice.Order.Client.ServiceManager.RealName,
        //        Mobile = ExitNotice.Order.Client.ServiceManager.Mobile,
        //        ProvinceName = "广东省",
        //        CityName = "深圳市",
        //        ExpAreaName = "龙岗区",
        //        Address = "坂田吉华路393号英达丰科技园",
        //    };
        //    this.Receiver = this.GetReceiver(ExitNotice.ExitDeliver.Expressage.Address);
        //    this.Receiver.Company = ExitNotice.Order.Client.Company.Name;
        //    this.Receiver.Name = ExitNotice.ExitDeliver.Expressage.Contact;
        //    this.Receiver.Mobile = ExitNotice.ExitDeliver.Expressage.Mobile;
        //    this.Commodity = new Commodity[]{
        //        new Commodity()
        //        {
        //            GoodsName="电子元器件",
        //        }
        //    };
        //    this.Remark = "小心轻放";
        //    this.TemplateSize = "210";
        //    this.IsReturnPrintTemplate = "1";
        //    if (this.ShipperCode == "KYSY")
        //    {
        //        //跨域的默认数量都填1
        //        this.Quantity = 1;
        //    }
        //    else
        //    {
        //        this.Quantity = ExitNotice.ExitDeliver.PackNo;
        //    }
        //}

        public Receiver GetReceiver(string Address, string CompanyName)
        {
            var address = new Dictionary<string, string>();
            address = this.HandleAddress(Address);
            Receiver receiver = new Receiver();
            receiver.ProvinceName = address["Province"];
            receiver.CityName = address["City"];
            receiver.ExpAreaName = address["Area"];
            //寄件地址后面追加公司名称
            receiver.Address = address["DetailsAddress"] + "(" + CompanyName + ")";
            return receiver;
        }

        public Services.Models.Receiver GetSFReceiver(string Address, string CompanyName)
        {
            var address = new Dictionary<string, string>();
            address = this.HandleAddress(Address);
            Services.Models.Receiver receiver = new Services.Models.Receiver();
            receiver.ProvinceName = address["Province"];
            receiver.CityName = address["City"];
            receiver.ExpAreaName = address["Area"];
            //寄件地址后面追加公司名称
            receiver.Address = address["DetailsAddress"] + "(" + CompanyName + ")";
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

        public string Goodsquantity { get; set; }

        public string GoodsWeight { get; set; }
    }

}
