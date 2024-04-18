using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
   public static class SpecialCharacter
    {
        ///任意字符串
        ///全角字符串
        ///全角空格为12288，半角空格为32
        ///其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248 
        ///<summary>
        /// 快递鸟转全角的函数
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        static public string ToKdnFullAngle(this string input)
        {
            // 半角转全角：
            char[] c = input.ToCharArray();
            int[] number = { 37, 38, 43, 60, 62 };// % & + < >这五个字符的特殊处理

            for (int i = 0; i < c.Length; i++)
            {
                if (number.Contains(c[i]))
                {
                    c[i] = (char)(c[i] + 65248);
                }
            }
            return new string(c);
        }
    }
}
