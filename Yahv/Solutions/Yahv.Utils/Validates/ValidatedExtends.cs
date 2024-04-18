using System.Text.RegularExpressions;

namespace Yahv.Utils.Validates
{
    /// <summary>
    /// 验证 助手类
    /// </summary>
    static public class ValidatesExtends
    {
        #region 检测是否符合email格式
        /// <summary>
        /// 检测是否符合email格式
        /// </summary>
        /// <param name="strEmail">要判断的email字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsValidEmail(string strEmail)
        {
            return Regex.IsMatch(strEmail, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }
        #endregion

        /// <summary>
        /// 是否为数字
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNumber(this string value)
        {
            return Regex.IsMatch(value, @"^\d+(\.\d+)?$");
        }

        #region 检测是否是正确的Url
        /// <summary>
        /// 检测是否是正确的Url
        /// </summary>
        /// <param name="value">要验证的Url</param>
        /// <returns>判断结果</returns>
        public static bool IsUrl(this string value)
        {
            return Regex.IsMatch(value, @"^(http|https)\://([a-zA-Z0-9\.\-]+(\:[a-zA-Z0-9\.&%\$\-]+)*@)*((25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])|localhost|([a-zA-Z0-9\-]+\.)*[a-zA-Z0-9\-]+\.(com|edu|gov|int|mil|net|org|biz|arpa|info|name|pro|aero|coop|museum|[a-zA-Z]{1,10}))(\:[0-9]+)*(/($|[a-zA-Z0-9\.\,\?\'\\\+&%\$#\=~_\-]+))*$");
        }
        #endregion

        #region 是否为ip
        /// <summary>
        /// 是否为ip
        /// </summary>
        /// <param name="value">IP 字符串</param>
        /// <returns>bool</returns>
        public static bool IsIP(this string value)
        {
            return Regex.IsMatch(value, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");

        }

        /// <summary>
        /// 是否为ip
        /// </summary>
        /// <param name="value">IPSec 字符串</param>
        /// <returns>bool</returns>
        public static bool IsIPSec(this string value)
        {
            return Regex.IsMatch(value, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){2}((2[0-4]\d|25[0-5]|[01]?\d\d?|\*)\.)(2[0-4]\d|25[0-5]|[01]?\d\d?|\*)$");

        }
        #endregion

        #region 检测是否有Sql危险字符
        /// <summary>
        /// 检测是否有Sql危险字符
        /// </summary>
        /// <param name="sql">要判断字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsSafeSql(this string sql)
        {
            return !Regex.IsMatch(sql, @"[-|;|,|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']");
        }
        #endregion

        #region 检测用户名称是否有危险
        /// <summary>
        /// 检测用户名称是否有危险
        /// </summary>
        /// <param name="value">要判断字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsSafeUsername(this string value)
        {
            return !Regex.IsMatch(value, @"^\s*$|^c:\\con\\con$|[%,\*" + "\"" + @"\s\t\<\>\&]|游客|^Guest");
        }
        #endregion
    }
}
