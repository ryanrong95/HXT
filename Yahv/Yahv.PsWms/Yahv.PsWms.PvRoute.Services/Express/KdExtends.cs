using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PsWms.PvRoute.Services.Express
{

    public class KdAddressError
    {
        public string Error { get; set; }

        public string[] Values { get; set; }
    }

    /// <summary>
    /// 快递拓展
    /// </summary>
    static public class KdExtends
    {
        /// <summary>
        /// 验证地址
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns>验证是否成功</returns>
        static public bool TryAddress(this string address, out KdAddressError error)
        {
            KdAddress kda;
            return TryAddress(address, out kda, out error);
        }

        /// <summary>
        /// 验证地址
        /// </summary>
        /// <typeparam name="T">输出对象类型</typeparam>
        /// <param name="address">地址</param>
        /// <param name="kdAddress">输出对象</param>
        /// <param name="error">缺少提示</param>
        /// <returns>验证是否成功</returns>
        static public bool TryAddress<T>(this string address, out T kdAddress, out KdAddressError error) where T : KdAddress, new()
        {
            var china = pccAreas.Current["中国"];
            var province = GetPcc(address, china);

            kdAddress = null;
            error = null;

            if (province == null)
            {
                error = new KdAddressError
                {
                    Error = "省/直辖市",
                    Values = china.s.Select(item => item.n).ToArray()
                };
                return false;
            }



            var isZxs = province.StartsWith("北京")
                || province.StartsWith("天津")
                || province.StartsWith("上海")
                || province.StartsWith("重庆");

            string city;
            string county;

            if (isZxs)
            {
                city = province + "市";
                county = GetPcc(address, china[province]);
            }
            else
            {
                city = GetPcc(address, china[province]);
                county = GetPcc(address, china[province][city]);
            }

            if (city == null)
            {
                error = new KdAddressError
                {
                    Error = "市",
                    Values = china[province].s.Select(item => item.n).ToArray()
                };
                return false;
            }

            if (county == null)
            {
                error = new KdAddressError
                {
                    Error = "区/县",
                    Values = china[province][city]?.s.Select(item => item.n).ToArray()
                };
                return false;
            }

            if (province == "内蒙古" || province == "西藏")
            {
                province = province + "自治区";
            }
            else if (province == "新疆")
            {
                province = province + "维吾尔自治区";
            }
            else if (province == "广西")
            {
                province = province + "壮族自治区";
            }
            else if (province == "宁夏")
            {
                province = province + "回族自治区";
            }
            else if (!isZxs)
            {
                province = province + "省";
            }

            kdAddress = new T
            {
                Province = province,
                City = city,
                Region = county,
                Address = address.Substring(address.IndexOf(county) + county.Length).TrimStart('区'),
            };

            return true;
        }

        static Func<string, pccArea, string> GetPcc = (address, areas) =>
        {
            if (areas == null)
            {
                return null;
            }

            var selects = areas.s.Select(province => new
            {
                Name = province.n,
                level = address.IndexOf(province.n)
            }).Where(item => item.level != -1).OrderBy(item => item.level).Select(item => item.Name);

            return selects.FirstOrDefault();
        };
    }
}
