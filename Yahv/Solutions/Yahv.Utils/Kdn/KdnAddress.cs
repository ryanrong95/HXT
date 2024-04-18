using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Utils.Extends;

namespace Yahv.Utils.Kdn
{
    /// <summary>
    /// 快递鸟通讯接口
    /// </summary>
    public interface IKdnAddress
    {
        /// <summary>
        /// 详细地址
        /// </summary>
        string Address { get; set; }

        string Company { get; set; }

        string Name { get; set; }

        string Mobile { get; set; }

        string Tel { get; set; }

    }

    /// <summary>
    /// 快递鸟基类拓展
    /// </summary>
    public class KdnAddress
    {
        /// <summary>
        /// 省
        /// </summary>
        public string ProvinceName { get; set; }
        /// <summary>
        /// 市
        /// </summary>
        public string CityName { get; set; }


        /// <summary>
        /// 区
        /// </summary>
        public string ExpAreaName { get; set; }

        string address;
        /// <summary>
        /// 详细地址
        /// </summary>
        public string Address
        {
            get { return this.address; }
            set
            {
                KdnAddress kdnAddress;
                KdnAddressError error;

                if (value.TryAddress(out kdnAddress, out error))
                {
                    this.ProvinceName = kdnAddress.ProvinceName;
                    this.CityName = kdnAddress.CityName;
                    this.ExpAreaName = kdnAddress.ExpAreaName;
                    this.address = kdnAddress.Address.ToKdnFullAngle();//防止特殊字符，作转全角处理
                }
                else
                {
                    this.address = value;
                }
            }
        }
        public KdnAddress()
        {

        }
    }
}
