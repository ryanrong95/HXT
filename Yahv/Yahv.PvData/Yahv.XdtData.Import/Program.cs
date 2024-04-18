using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Yahv.XdtData.Import
{
    class Program
    {
        static void Main(string[] args)
        {
            //Services.UpdateService.Current.Start();
            //Services.UpdateService.Current.AddMainOrderIDs("NL02020191101001", "NL02020191113001");

            //Test();
            //DataImport();
            //PaysImport();
            //FilesDownLoad();

            Console.ReadKey();
        }

        /// <summary>
        /// 测试
        /// </summary>
        static void Test()
        {
            /*
            string[] excludedOrderIDs = { "NL02020190815010", "WL47020190628001", "NL02020190524001" };
            string[] xdtMainOrderIDs, pvWsOrderIDs;

            using (var reponsitory = new PvWsOrderReponsitory())
            {
                //芯达通主订单
                xdtMainOrderIDs = reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.XdtMainOrdersTopView>()
                    .Where(item => !excludedOrderIDs.Contains(item.ID) && item.Status == 200)
                    .Take(10).Select(item => item.ID).ToArray();

                //代仓储订单
                pvWsOrderIDs = reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.Orders>()
                    .Select(item => item.ID).ToArray();
            }

            //尚未导入代仓储系统的芯达通订单
            xdtMainOrderIDs = xdtMainOrderIDs.Except(pvWsOrderIDs).ToArray();
            */

            string[] xdtMainOrderIDs = { "NL02020191019006" };

            Stopwatch watch = new Stopwatch();
            watch.Start();

            //new Services.OrderService(xdtMainOrderIDs).Query().Encapsule().Enter();

            watch.Stop();
            Console.WriteLine("订单插入完成，耗时：" + watch.ElapsedMilliseconds + "毫秒");

            watch.Restart();

            //new Services.WayChcdService(xdtMainOrderIDs).Query().Encapsule().Enter();

            watch.Stop();
            Console.WriteLine("中港报关运单插入完成，耗时：" + watch.ElapsedMilliseconds + "毫秒");

            watch.Restart();

            var szOutWaybillService = new Services.SzOutWaybillService(xdtMainOrderIDs);
            szOutWaybillService.Query().Encapsule().Enter();

            watch.Stop();
            Console.WriteLine("深圳出库运单插入完成，耗时：" + watch.ElapsedMilliseconds + "毫秒");

            watch.Restart();

            new Services.PvWmsDataService(szOutWaybillService.MapsWaybill, xdtMainOrderIDs).Enter();

            watch.Stop();
            Console.WriteLine("库房插入完成，耗时：" + watch.ElapsedMilliseconds + "毫秒");
        }

        /// <summary>
        /// 数据导入
        /// </summary>
        static void DataImport()
        {
            int processedCount = 0;
            int exceptionCount = 0;

            //NL02020190815010与WL47020190628001 没有报关，应该属于废弃订单，不需要导入
            //NL02020190524001中同一个orderitem有多个sorting，应该有人工干预过，后期单独处理
            string[] excludedOrderIDs = { "NL02020190815010", "WL47020190628001", "NL02020190524001" };
            string[] xdtMainOrderIDs, pvWsOrderIDs;

            using (var reponsitory = new PvWsOrderReponsitory())
            {
                //芯达通主订单
                xdtMainOrderIDs = reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.XdtTinyOrdersTopView>()
                    .Where(item => !excludedOrderIDs.Contains(item.VastOrderID) && item.OrderStatus > 4 && item.OrderStatus < 8)
                    .Select(item => item.VastOrderID).Distinct().ToArray();

                //代仓储订单
                pvWsOrderIDs = reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.Orders>()
                    .Select(item => item.ID).ToArray();
            }

            //尚未导入代仓储系统的芯达通订单
            xdtMainOrderIDs = xdtMainOrderIDs.Except(pvWsOrderIDs).ToArray();

            //订单导入
            foreach (var xdtMainOrderID in xdtMainOrderIDs)
            {
                try
                {
                    Stopwatch watch = new Stopwatch();
                    watch.Start();

                    new Services.OrderService(xdtMainOrderID).Query().Encapsule().Enter();
                    new Services.WayChcdService(xdtMainOrderID).Query().Encapsule().Enter();

                    var szOutWaybillService = new Services.SzOutWaybillService(xdtMainOrderID);
                    szOutWaybillService.Query().Encapsule().Enter();
                    new Services.PvWmsDataService(szOutWaybillService.MapsWaybill, xdtMainOrderID).Enter();

                    watch.Stop();
                    Console.WriteLine($"订单{xdtMainOrderID}导入完成, 耗时: " + watch.ElapsedMilliseconds + "毫秒");

                    processedCount++;
                    Console.WriteLine($"已处理订单数: {processedCount}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"订单{xdtMainOrderID}导入发生异常");
                    exceptionCount++;
                    Console.WriteLine($"异常订单数: {exceptionCount}");

                    string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "data.exceptions.log");
                    using (var file = new StreamWriter(path, true))
                    {
                        file.WriteLine($"订单: {xdtMainOrderID}");
                        file.WriteLine($"   异常时间: {DateTime.Now}");
                        file.WriteLine($"   异常信息: {ex.Message}, 堆栈信息: { ex.StackTrace}");
                        file.Flush();
                    }

                    continue;
                }
            }

            Console.WriteLine($"订单处理完成");
        }

        static void PaysImport()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            new Services.PaysDataService("1B1E77CC051207C4A472C5EF91C68A02").Query().Encapsule().Enter();       //杭州比一比电子科技有限公司
            //new Services.PaysDataService("9E467CBBE0C4D78C856C58041E055580").Query().Encapsule().Enter();       //北京创新在线电子产品销售有限公司杭州分公司

            watch.Stop();
            Console.WriteLine("费用插入完成，耗时：" + watch.ElapsedMilliseconds + "毫秒");
        }

        static void FilesDownLoad()
        {
            new Services.FileService().DownLoad();
        }
    }
}
