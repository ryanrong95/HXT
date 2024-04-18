using Layers.Data.Sqls;
using System;
using System.IO;
using System.Linq;

namespace Yahv.PvWsOrder.ScheduleTask
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Yahv.Services.Initializers.LsBoot();
                Yahv.Services.Initializers.OrderBoot();

                if (args.Length > 0)
                {
                    WriteLogs("Info", args[0]);
                    string param = args[0];
                    if (param == "LsOrderAutoCancel")
                    {
                        LsOrderAutoCancel();
                    }
                    if (param == "LsOrderAutoExpired")
                    {
                        LsOrderAutoExpired();
                    }
                }

                //Yahv.Services.Initializers.LsBoot();
                //Yahv.Services.Initializers.OrderBoot();
                //LsOrderAutoCancel();
            }
            catch (Exception ex)
            {
                WriteLogs("Error", args[0].ToString() + ex.ToString());
            }
            finally
            {
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }

        /// <summary>
        /// 订单订单24小时未支付自动取消
        /// </summary>
        private static void LsOrderAutoCancel()
        {
            var orders = new Services.Views.LsOrdersAll()
                .Where(item => item.Status == Underly.LsOrderStatus.Unpaid)
                .Where(item => (DateTime.Now - item.CreateDate).TotalHours > 24).ToArray();
            //取消订单
            foreach (var order in orders)
            {
                WriteLogs("Info", "系统设置租赁订单：" + order.ID + " 自动取消");
                order.OperatorID = "NPC";
                order.Abandon();
            }
        }

        /// <summary>
        /// 租赁订单自动到期
        /// </summary>
        private static void LsOrderAutoExpired()
        {
            var orders = new Services.Views.LsOrdersAll()
                .Where(item => item.Status == Underly.LsOrderStatus.Allocated);
            orders = orders.Where(item => item.EndDate < DateTime.Now);
            //订单到期
            foreach (var order in orders)
            {
                WriteLogs("Info", "系统设置租赁订单：" + order.ID + " 自动到期");
                order.OperatorID = "NPC";
                order.Expired();
            }
        }

        static object locker = new object();

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="content">内容</param>
        private static void WriteLogs(string type, string content)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            if (!string.IsNullOrEmpty(path))
            {
                lock (locker)
                {
                    path = AppDomain.CurrentDomain.BaseDirectory + "Logs";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    path = path + "\\" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                    if (!File.Exists(path))
                    {
                        FileStream fs = File.Create(path);
                        fs.Close();
                    }
                    if (File.Exists(path))
                    {
                        StreamWriter sw = new StreamWriter(path, true, System.Text.Encoding.Default);
                        sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " >>> " + type + " >>> " + content);
                        sw.WriteLine("-----------------华丽的分割线-----------------------");
                        sw.Close();
                    }
                }
            }
        }
    }
}
