using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Needs.Ccs.Services
{
    public static class DecimalExtends
    {
        /// <summary>
        /// 四舍五入  IEEE规范
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="length">保留位数</param>
        /// <returns></returns>
        public static decimal ToRound(this decimal value, int length)
        {
            return Math.Round((decimal)value, length, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// 将数字金额转换为人民币大写金额
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static string ToChineseAmount(this decimal value)
        {
            // 大写数字数组
            string[] num = { "零", "壹", "贰", "叁", "肆", "伍", "陆", "柒", "捌", "玖" };
            // 数量单位数组，个位数为空
            string[] unit = { "", "拾", "佰", "仟", "万", "拾", "佰", "仟", "亿", "拾", "佰", "仟", "兆" };
            string d = value.ToString();
            string zs = string.Empty;// 整数
            string xs = string.Empty;// 小数
            int i = d.IndexOf(".");
            string str = string.Empty;
            if (i > -1)
            {
                // 仅考虑两位小数
                zs = d.Substring(0, i);
                xs = d.Substring(i + 1, d.Length - i - 1);
                str = "元";
                if (xs.Length == 1)
                    str = str + xs + "角";
                else if (xs.Length == 2)
                    str = str + xs.Substring(0, 1) + "角" + xs.Substring(1, 1) + "分";
            }
            else
            {
                zs = d;
                str = "元整";
            }
            // 处理整数部分
            if (!string.IsNullOrEmpty(zs))
            {
                i = 0;
                // 从整数部分个位数起逐一添加单位
                foreach (char s in zs.Reverse())
                {
                    str = s.ToString() + unit[i] + str;
                    i++;
                }
            }
            // 将阿拉伯数字替换成中文大写数字
            for (int m = 0; m < 10; m++)
            {
                str = str.Replace(m.ToString(), num[m]);
            }
            // 替换零佰、零仟、零拾之类的字符
            str = Regex.Replace(str, "[零]+仟", "零");
            str = Regex.Replace(str, "[零]+佰", "零");
            str = Regex.Replace(str, "[零]+拾", "零");
            str = Regex.Replace(str, "[零]+亿", "亿");
            str = Regex.Replace(str, "[零]+万", "万");
            str = Regex.Replace(str, "[零]+", "零");
            str = Regex.Replace(str, "亿[万|仟|佰|拾]+", "亿");
            str = Regex.Replace(str, "万[仟|佰|拾]+", "万");
            str = Regex.Replace(str, "仟[佰|拾]+", "仟");
            str = Regex.Replace(str, "佰拾", "佰");
            str = Regex.Replace(str, "[零]+元整", "元整");
            return str;
        }
    }
}
