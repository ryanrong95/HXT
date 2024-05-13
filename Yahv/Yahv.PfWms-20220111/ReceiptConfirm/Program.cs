using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Yahv.Underly;
using Yahv.Underly.Enums;
using Yahv.Utils.Converters.Contents;

namespace ReceiptConfirm
{
    class Program
    {
        static void Main(string[] args)
        {

            AutoResetEvent autoEvent = new AutoResetEvent(false);
            StatusChecker statusChecker = new StatusChecker(10);

            // Create the delegate that invokes methods for the timer.
            TimerCallback timerDelegate =
                new TimerCallback(statusChecker.CheckStatus);

            TimeSpan delayTime = new TimeSpan(0, 0, 1);
            TimeSpan intervalTime = new TimeSpan(0, 0, 0, 0, 250);

            // Create a timer that signals the delegate to invoke 
            // CheckStatus after one second, and every 1/4 second 
            // thereafter.
            Console.WriteLine("{0} Creating timer.\n",
                DateTime.Now.ToString("h:mm:ss.fff"));
            Timer stateTimer = new Timer(
                timerDelegate, autoEvent, delayTime, intervalTime);

            // When autoEvent signals, change the period to every 
            // 1/2 second.
            autoEvent.WaitOne(5000, false);
            stateTimer.Change(new TimeSpan(0),
                intervalTime + intervalTime);
            Console.WriteLine("\nChanging period.\n");

            // When autoEvent signals the second time, dispose of 
            // the timer.
            autoEvent.WaitOne(5000, false);
            stateTimer.Dispose();
            Console.WriteLine("\nDestroying timer.");
            Console.ReadKey();


            #region 箱号思路
            ////确定连续箱号总共多少个
            //// NL0001-NL0020

            ////确定连续箱号多少个
            ////000100002-000100010

            //string boxcode1 = "NL0001-NL0020";
            //string boxcode2 = "000100002-000100010";
            //string boxcode3 = "0001000hang-0001000hang";
            //string boxcode4 = "WL00050";

            ////箱号的正则表达式
            //Regex reg = new Regex(@"^(\D*)(\d*)[-](\D*)(\d*)$");

            ////匹配正则
            //var matches = reg.Matches(boxcode1);

            ////是否满足正则
            //if (reg.IsMatch(boxcode1))
            //{
            //    foreach (Match match in matches)
            //    {
            //        //获得前缀1
            //        var pre1 = match.Groups[1].ToString();
            //        //获得前缀2
            //        var pre2 = match.Groups[3].ToString();
            //        //前缀不相等就报错
            //        if (pre1 != pre2)
            //        {
            //            throw new Exception("箱号错误");
            //        }

            //        //获得第一个箱号下标值
            //        var index1 = int.Parse(match.Groups[2].ToString());
            //        //获得第二个箱号下标值
            //        var index2 = int.Parse(match.Groups[4].ToString());

            //        //获得箱号的总长度
            //        var length = match.Groups[4].ToString().Length;

            //        List<string> list = new List<string>();
            //        for (int index = index1; index <= index2; index++)
            //        {
            //            //添加到数组（添加的时候注意数据库是否存在，存在就抛异常（“箱号中已经有被使用的箱号”））
            //            list.Add(string.Concat(pre1, index.ToString().PadLeft((length - 1), '0')));
            //        }

            //        //1.把数组的数据添加到数据库里；
            //        //2.把连续箱号也存到数据库里
            //        //
            //        //var boxCode1 = string.Concat(pre1, match.Groups[2].ToString());
            //        //var boxCode2 = string.Concat(pre1, match.Groups[4].ToString());
            //    }
            //}
            #endregion

            #region 废弃代码
            //快递的7天后自动改为已确认收货。??是否是运单所对应的订单主状态修改成 "已完成" 状态就行?

            //using (var rep = new PvWmsRepository())
            //{
            //    //获取已出库的类型是快递的运单
            //    var waybills = new Yahv.Services.Views.WaybillsTopView<PvWmsRepository>().
            //        Where(item => item.ExcuteStatus == (int)PickingExcuteStatus.OutStock && (item.Type == WaybillType.LocalExpress || item.Type == WaybillType.InternationalExpress)).ToArray();

            //    var waybillIDs = waybills.Select(item => item.ID).ToArray();

            //    var date = DateTime.Now;

            //    foreach (var waybill in waybills)
            //    {
            //        //是否是快递的7天后，是则修改
            //        if (waybill.CreateDate.AddDays(7) > date)
            //        {

            //            //rep.Update<Layers.Data.Sqls.PvWms.WaybillsTopView>(new Layers.Data.Sqls.PvWms.WaybillsTopView { ConfirmReceiptStatus = 200 }, item => item.wbID == waybill.ID); 
            //            //修改对应运单状态
            //            rep.Command($"update {nameof(Layers.Data.Sqls.PvWms.WaybillsTopView)} set {nameof(Layers.Data.Sqls.PvWms.WaybillsTopView.ConfirmReceiptStatus)}=200 where {nameof(Layers.Data.Sqls.PvWms.WaybillsTopView.wbID)}='{waybill.ID}'");

            //            rep.Command($"insert into {nameof(Layers.Data.Sqls.PvWms.Logs_WaybillsTopView)} values('{Guid.NewGuid().ToString().MD5()}','{waybill.ID}',{(int)WaybillStatusType.ConfirmReceiptStatus},'200',getdate(),'NPC',1)");

            //        }
            //    }

            //}
            #endregion
        }


        void SeperateBoxCode(string boxCode)
        {
            string[] boxCodes = boxCode.Split('-');

            var first = boxCodes[0];
            var last = boxCodes[1];


        }
    }
    class StatusChecker
    {
        int invokeCount, maxCount;

        public StatusChecker(int count)
        {
            invokeCount = 0;
            maxCount = count;
        }

        // This method is called by the timer delegate.
        public void CheckStatus(Object stateInfo)
        {
            AutoResetEvent autoEvent = (AutoResetEvent)stateInfo;
            Console.WriteLine("{0} Checking status {1,2}.",DateTime.Now.ToString("h:mm:ss.fff"),
(++invokeCount).ToString());

            if (invokeCount == maxCount)
            {
                // Reset the counter and signal Main.
                invokeCount = 0;
                autoEvent.Set();
            }
        }
    }

}
