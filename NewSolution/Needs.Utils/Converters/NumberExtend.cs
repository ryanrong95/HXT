using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Utils.Converters
{
    public static class NumberExtend
    {
        /// <summary>
        /// 获取陈列价
        /// 格式化为4位小数,小数点4位后非0进1
        /// </summary>
        /// <param name="val">实际价格</param>
        /// <returns>陈列价</returns>
        public static string Fours(this decimal val)
        {
            return Fourh(val).ToString("0.0000");
        }

        /// <summary>
        /// 获取陈列价
        /// 取4位小数,小数点4位后非0进1
        /// </summary>
        /// <param name="val">实际价格</param>
        /// <returns>陈列价</returns>
        public static decimal Fourh(this decimal val)
        {
            return Fifh(val, 4);
        }

        /// <summary>
        /// 获取结算价
        /// 格式化为2位小数,小数点后两位进行四舍五入
        /// </summary>
        /// <param name="val">实际价格</param>
        /// <returns>结算价格</returns>
        public static string Twos(this decimal val)
        {
            return Twoh(val).ToString("0.00");
        }

        /// <summary>
        /// 获取结算价
        /// 取2位小数,小数点后两位进行四舍五入
        /// </summary>
        /// <param name="val">实际价格</param>
        /// <returns>结算价格</returns>
        public static decimal Twoh(this decimal val)
        {
            //按照张经理要求最后修改为小数点后两位进行四舍五入
            return Math.Round(val, 2);
            //return Fifh(val, 2);
        }


        /// <summary>
        /// 获取用户舍去价格
        /// 格式化为2位小数,小数点2位后全部舍去
        /// </summary>
        /// <param name="val">实际价格</param>
        /// <returns>舍去价格</returns>
        public static string TwoLs(this decimal val)
        {
            return TwoLh(val).ToString("0.00");
        }

        /// <summary>
        /// 获取用户舍去价格
        /// 取2位小数,小数点两位后全部舍去
        /// </summary>
        /// <param name="val">实际价格</param>
        /// <returns>舍去价格</returns>
        public static decimal TwoLh(this decimal val)
        {
            return FifLh(val, 2);
        }

        public static decimal FiveLh(this decimal val)
        {
            return FifLh(val, 5);
        }

        /// <summary>
        /// 五入四不舍
        /// </summary>
        /// <param name="val">计算值</param>
        /// <returns>五入四不舍值</returns>
        static decimal Fifh(this decimal val, int prec)
        {
            if (val == 0M)
            {
                return 0M;
            }

            bool isMinus = val < 0M;
            val = Math.Abs(val);

            decimal augend = (decimal)Math.Pow(10, -(prec + 1)) * 4M;
            var rtn = val + augend;
            rtn = Math.Round(rtn, prec, MidpointRounding.AwayFromZero);
            return isMinus ? -rtn : rtn;
        }

        /// <summary>
        /// 四舍五不近
        /// </summary>
        /// <param name="val">计算值</param>
        /// <returns>四舍五不近值</returns>
        static decimal FifLh(this decimal val, int prec)
        {
            if (val == 0M)
            {
                return 0M;
            }


            bool isMinus = val < 0M;
            val = Math.Abs(val);

            decimal augend = (decimal)Math.Pow(10, -(prec + 1)) * 5M;
            var rtn = val - augend;
            rtn = Math.Round(rtn, prec, MidpointRounding.AwayFromZero);
            return isMinus ? -rtn : rtn;
        }

        /// <summary>
        /// 百分比显示
        /// </summary>
        /// <param name="val">要显示的数值</param>
        /// <returns></returns>
        public static string P0(this decimal val)
        {
            return val.ToString("P0");
        }

    }
}
