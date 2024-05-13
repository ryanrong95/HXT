using Layers.Data;
using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Wms.Services;
using Yahv.Underly;

namespace ConsoleApp.vTaskers.Services
{
    /// <summary>
    /// 每天生成库房统计信息
    /// </summary>
    public class StatisticStorageTasker
    {
        Thread runner;

        protected StatisticStorageTasker()
        {
            this.runner = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        this.StatisticsStorage();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    finally
                    {
                        Thread.Sleep(60 * 1000 * 15);
                    }
                }
            });
        }

        public void Start()
        {
            this.runner.Start();
        }

        static StatisticStorageTasker current;
        static Object obj = new object();

        public static StatisticStorageTasker Current
        {
            get
            {
                if (current == null)
                {
                    lock (obj)
                    {
                        if (current == null)
                        {
                            current = new StatisticStorageTasker();
                        }
                    }
                }
                return current;
            }
        }

        /// <summary>
        /// 统计相同客户在本地库房是否有货物并生成StatisticsStorage对象记录
        /// </summary>
        protected void StatisticsStorage()
        {
            DateTime dateTime = DateTime.Now;
            DateTime today = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);

            string todayDateIndex = dateTime.ToString("yyyyMMdd");
            string yesterdayDateIndex = dateTime.AddDays(-1).ToString("yyyyMMdd");


            using (var pvwms = new PvWmsRepository())
            using (var pvbcrm = new PvbCrmReponsitory())
            {
                var statisticStorageView = from statisticStorage in pvwms.ReadTable<Layers.Data.Sqls.PvWms.StatisticsStorage>()
                                           select new
                                           {
                                               statisticStorage.EnterCode,
                                               statisticStorage.DateIndex,
                                               statisticStorage.CreateDate,
                                               statisticStorage.WarehouseID,
                                               statisticStorage.Quantity,
                                               statisticStorage.Box1Quantity,
                                               statisticStorage.Box2Quantity,
                                               statisticStorage.Box3Quantity,
                                               statisticStorage.AdminID,
                                           };

                var yesterdayView = statisticStorageView.Where(item => item.DateIndex.ToString() == yesterdayDateIndex);
                var todayView = statisticStorageView.Where(item => item.DateIndex.ToString() == todayDateIndex);


                if (yesterdayView != null)
                {
                    foreach (var item in yesterdayView)
                    {
                        if (!todayView.Any(storage => storage.EnterCode == item.EnterCode))
                        {
                            pvwms.Insert(new Layers.Data.Sqls.PvWms.StatisticsStorage
                            {
                                EnterCode = item.EnterCode,
                                Box1Quantity = item.Box1Quantity,
                                Box2Quantity = item.Box2Quantity,
                                Box3Quantity = item.Box3Quantity,
                                CreateDate = DateTime.Now,
                                DateIndex = int.Parse(todayDateIndex),
                                Quantity = item.Quantity,
                                WarehouseID = item.WarehouseID,
                                AdminID = Npc.Robot.Obtain(),
                                ID = PKeySigner.Pick(PkeyType.StatisticStorage),
                            });
                        }
                    }
                }

                var linq_storages = from storage in pvwms.ReadTable<Layers.Data.Sqls.PvWms.Storages>()
                                    join input in pvwms.ReadTable<Layers.Data.Sqls.PvWms.Inputs>()
                                    on storage.InputID equals input.ID
                                    join client in pvwms.ReadTable<Layers.Data.Sqls.PvWms.ClientsTopView>()
                                    on input.ClientID equals client.ID
                                    where storage.Quantity > 0 && storage.WareHouseID.StartsWith("HK") && storage.InputID != null && input.ClientID != null && storage.CreateDate >= today && storage.CreateDate < today.AddDays(1)
                                    // 第一次运行时,需要用下面的条件, 其他的情况需要用上面的条件
                                    //where storage.Quantity > 0 && storage.WareHouseID.StartsWith("HK") && storage.InputID != null && input.ClientID != null
                                    select new
                                    {
                                        storage.ShelveID,
                                        storage.InputID,
                                        input.ClientID,
                                        client.EnterCode,
                                        dateIndex = todayDateIndex,
                                        WareHouseID = storage.WareHouseID.Substring(0, 2),
                                    };

                var linq_storage_group = from storage in linq_storages
                                         group storage by new { storage.EnterCode, storage.ClientID, storage.dateIndex, storage.WareHouseID } into storages
                                         select new
                                         {
                                             storages.Key.EnterCode,
                                             storages.Key.dateIndex,
                                             storages.Key.ClientID,
                                             storages.Key.WareHouseID,
                                         };

                var enum_storage_group = linq_storage_group.ToArray();

                if (enum_storage_group.Count() > 0)
                {
                    foreach (var item in enum_storage_group)
                    {
                        if (!todayView.Any(storage => storage.EnterCode == item.EnterCode))
                        {
                            pvwms.Insert(new Layers.Data.Sqls.PvWms.StatisticsStorage
                            {
                                ID = PKeySigner.Pick(PkeyType.StatisticStorage),
                                Box1Quantity = 0,
                                Box2Quantity = 0,
                                Box3Quantity = 0,
                                CreateDate = DateTime.Now,
                                DateIndex = int.Parse(todayDateIndex),
                                EnterCode = item.EnterCode,
                                Quantity = 0,
                                WarehouseID = item.WareHouseID,
                                AdminID = Npc.Robot.Obtain(),
                            });
                        }
                    }
                }
            }
        }


    }
}
