using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Yahv.PvWsClient.WebAppNew.App_Utils
{
    // <summary>
    /// 地址扩展
    /// </summary>
    public static class AddressExtend
    {
        /// <summary>
        /// 获取省市区
        /// </summary>
        /// <param name="address">地址格式的字符 ，传入的字符格式为：江苏 苏州 工业园区 星湖街街328号国际科技园5期12栋901</param>
        /// <returns></returns>
        public static string[] ToAddress(this string address)
        {
            //验证格式是否正确，否则，一律返回 string.Empty
            //将转换的数组通过linq或更高效的方式返回 省市区，去掉详细地址部分。
            string[] result = null;
            if (!string.IsNullOrEmpty(address))
            {
                var fullAddress = address.Trim().Split(' ').ToArray();
                //北京 东城区 xx街道
                if (fullAddress.Count() > 2)
                {
                    result = fullAddress.Take(fullAddress.Length - 1).ToArray();
                }
            }
            return result;
        }

        /// <summary>
        /// 获取地址的详细地址部分
        /// 省 市 区 详细地址
        /// </summary>
        /// <param name="address">地址格式的字符 ，传入的字符格式为：江苏 苏州 工业园区 星湖街街328号国际科技园5期12栋901</param>
        /// <returns></returns>
        public static string ToDetailAddress(this string address)
        {
            var resultAddress = string.Empty;
            if (!string.IsNullOrEmpty(address))
            {
                var alAddress = address.Trim().Split(' ');
                if (alAddress.Count() > 2)
                {
                    resultAddress = alAddress.Last();
                }
            }
            return resultAddress;
        }
    }
}