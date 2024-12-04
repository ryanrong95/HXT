using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.vTaskers.Services
{
    public sealed class GlobalTimer
    {
        static GlobalTimer()
        {
        }

        GlobalTimer()
        {
        }

        /// <summary>
        /// 每十分钟执行一次
        /// </summary>
        public static void Do()
        {


            //每天工作期间执行 早8点至晚7点
            if (DateTime.Now.Hour >= 8 && DateTime.Now.Hour <= 19)
            {
                //string batchID = Guid.NewGuid().ToString("N");
                //Needs.Ccs.Services.Models.DeliveryNoticeApiLog apiLog = new Needs.Ccs.Services.Models.DeliveryNoticeApiLog()
                //{
                //    ID = Guid.NewGuid().ToString("N"),
                //    BatchID = batchID,
                //    // OrderID = applies.Items[0].VastOrderID,
                //    TinyOrderID = "1111",
                //    RequestContent = "1111",
                //    Status = Needs.Ccs.Services.Enums.Status.Normal,
                //    CreateDate = DateTime.Now,
                //    UpdateDate = DateTime.Now,
                //    Summary = "测试定时插入"
                //};
                //apiLog.Enter();


                //#region 获取开票结果

                //WaybillInfo waybillInfo = new WaybillInfo();
                //waybillInfo.doWaybill();

                //#endregion


                //#region 推送华芯通进价

                //PurPrice pushPrice = new PurPrice();
                //pushPrice.PushPurPrice();

                //#endregion


                //#region 获取大赢家凭证字 凭证号

                //WordNoTrigger wordNoTrigger = new WordNoTrigger();
                //wordNoTrigger.Credential();

                //#endregion


                //#region 从大赢家同步付款/付汇状态

                //DyjPayExchangeStatus dyjPayExchangeStatus = new DyjPayExchangeStatus();
                //dyjPayExchangeStatus.DYJStatusUpdate();

                //#endregion

            }
        }
    }
}
