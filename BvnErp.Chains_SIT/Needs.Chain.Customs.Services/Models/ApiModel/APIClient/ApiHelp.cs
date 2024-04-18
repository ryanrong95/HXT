using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 联系人
    /// </summary>
    //public class ApiContact
    //{
    //    /// <summary>
    //    /// 联系人类型(1 Online线上、2 Offline线下、3 Sales销售、4 Pruchaser 采购)
    //    /// </summary>
    //    public int Type { get; set; }
    //    /// <summary>
    //    /// 联系人姓名
    //    /// </summary>
    //    public string Name { get; set; }
    //    /// <summary>
    //    /// 联系人电话
    //    /// </summary>
    //    public string Tel { get; set; }
    //    /// <summary>
    //    /// 联系人手机号
    //    /// </summary>
    //    public string Mobile { get; set; }
    //    /// <summary>
    //    /// 联系人邮箱
    //    /// </summary>
    //    public string Email { get; set; }
    //    /// <summary>
    //    /// 传真
    //    /// </summary>
    //    public string Fax { get; set; }
    //   /// <summary>
    //   /// 
    //   /// </summary>
    //    public int Status { get; set; }
    //   /// <summary>
    //   /// 
    //   /// </summary>
    //    public string CreateDate { get; set; }
    //    public string UpdateDate { get; set; }

    //}

    public class ApiAddressHelp {

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
    }

  
}