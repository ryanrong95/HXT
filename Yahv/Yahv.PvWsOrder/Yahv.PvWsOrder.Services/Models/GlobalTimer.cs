using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvWsOrder.Services.Models
{
    public sealed class GlobalTimer
    {

        static GlobalTimer() { }
        GlobalTimer() { }


        public static void DO()
        {


            //每天工作期间执行 早8点至晚7点
            if (DateTime.Now.Hour >= 8 && DateTime.Now.Hour <= 19)
            {
                //获取大赢家凭证字 凭证号
                WordNoTrigger wordNoTrigger = new WordNoTrigger();
                wordNoTrigger.Credential();
            }
        }
    }



}
