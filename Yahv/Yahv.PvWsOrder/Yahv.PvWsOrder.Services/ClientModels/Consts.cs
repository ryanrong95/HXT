using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvWsOrder.Services.ClientModels
{
    public class Consts
    {
        /// <summary>
        /// 手机端token过期小时数
        /// </summary>
        public static double TokenOverdueHour = 24 * 5;

        /// <summary>
        /// 手机端token临时有效分钟数
        /// </summary>
        public static double TokenTempValidMinute = 5;

        /// <summary>
        /// 手机端token更换小时数
        /// </summary>
        public static double TokenChangeHour = 2;
    }
}
