using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Utils.Extends;

namespace Yahv.PsWms.PvRoute.Services.Express
{
    /// <summary>
    /// 快递鸟通讯接口
    /// </summary>
    public interface IKdAddress
    {
        /// <summary>
        /// 详细地址
        /// </summary>
        string Address { get; set; }

        string Company { get; set; }

        string Contact { get; set; }

        string Mobile { get; set; }

        string Tel { get; set; }

    }

    public class KdAddress

    {
        /// <summary>
        /// 省
        /// </summary>
        public string Province { get; set; }
        /// <summary>
        /// 市
        /// </summary>
        public string City { get; set; }


        /// <summary>
        /// 区
        /// </summary>
        public string Region { get; set; }

        string address;
        /// <summary>
        /// 详细地址
        /// </summary>
        public string Address
        {
            get { return this.address; }
            set
            {
                KdAddress kdAddress;
                KdAddressError error;

                if (value.TryAddress(out kdAddress, out error))
                {
                    this.Province = kdAddress.Province;
                    this.City = kdAddress.City;
                    this.Region = kdAddress.Region;
                    this.address = kdAddress.Address.ToKdnFullAngle();//防止特殊字符，作转全角处理
                }
                else
                {
                    this.address = value;
                }
            }
        }
        public KdAddress()
        {

        }
    }
}
