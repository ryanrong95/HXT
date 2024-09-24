using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Wms.Services.chonggous.Views;
using Yahv.Payments;
using Yahv.Underly;
using Yahv.Utils.Serializers;

namespace ConsoleApp.vTaskers.Services
{
    /// <summary>
    /// 库房费用的处理
    /// </summary>
    public class HKExitTaskers
    {
        Thread runner;

        public HKExitTaskers()
        {
            this.runner = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        this.HKExit();
                        this.SZCustomInsExit();
                        this.AfterHKExitAutoVouchers();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        Thread.Sleep(20);
                    }
                }
            });
        }
        public void Start()
        {
            //using (CgDelcareShipView service = new CgDelcareShipView())
            //    service.AutoHkExit("1100352206115");
            this.runner.Start();

            //调试深圳报关内单出库时使用
            //using (CgSzInsidePickingsView service = new CgSzInsidePickingsView())
            //    service.SZCustomInsExit("Waybill202008040149");
        }

        static HKExitTaskers current;
        static object obj = new object();

        /// <summary>
        /// 单例
        /// </summary>
        static public HKExitTaskers Current
        {
            get
            {
                if (current == null)
                {
                    lock (obj)
                    {
                        if (current == null)
                        {
                            current = new HKExitTaskers();
                        }
                    }
                }
                return current;
            }
        }

        /// <summary>
        /// 香港报关出库
        /// </summary>
        public void HKExit()
        {
            using (var pvcenter = new PvCenterReponsitory())
            {
                var taskspoll = pvcenter.ReadTable<Layers.Data.Sqls.PvCenter.TasksPool>()
                    .Where(item => item.Name == TaskSettgins.PvCenter.单一窗口申报后深圳出库)
                    .Take(3).ToArray();
                var ids = taskspoll.Select(item => item.ID).ToArray();
                //删除任务
                pvcenter.Delete<Layers.Data.Sqls.PvCenter.TasksPool>(item => ids.Contains(item.ID));

                using (CgDelcareShipView service = new CgDelcareShipView())
                {
                    foreach (var tast in taskspoll)
                    {
                        #region 香港库房报关重构后不在执行，香港报关出库
                        //if (tast.Name == TaskSettgins.PvCenter.香港报关出库)
                        //{
                            
                            //try
                            //{
                            //    Console.WriteLine($"正在处理{TaskSettgins.PvCenter.香港报关出库}-执行库房：AutoHkExit({tast.MainID}),{DateTime.Now}");
                            //    service.AutoHkExit(tast.MainID);
                            //    Console.WriteLine($"处理结束{TaskSettgins.PvCenter.香港报关出库}-执行库房：AutoHkExit({tast.MainID}),{DateTime.Now}");
                            //}
                            //catch (Exception ex)
                            //{
                            //    Console.WriteLine($"{TaskSettgins.PvCenter.香港报关出库}-执行库房-异常：AutoHkExit({tast.MainID}),{DateTime.Now})");
                            //    Console.WriteLine(ex.Message);
                            //    Console.WriteLine(ex.StackTrace);
                            //    LitTools.Current.Log($"{TaskSettgins.PvCenter.香港报关出库}-执行库房-异常：AutoHkExit({tast.MainID}),{DateTime.Now}",
                            //        ex.Message,
                            //        ex.StackTrace);
                            //}

                            //Console.WriteLine();
                            
                            //try
                            //{
                            //    Console.WriteLine($"正在处理{TaskSettgins.PvCenter.香港报关出库}-执行财务：AutoVouchers({tast.MainID}),{DateTime.Now}");
                            //    service.AutoVouchers(tast.MainID);
                            //    Console.WriteLine($"处理结束{TaskSettgins.PvCenter.香港报关出库}-执行财务：AutoVouchers({tast.MainID}),{DateTime.Now}");
                            //}
                            //catch (Exception ex)
                            //{
                            //    Console.WriteLine($"{TaskSettgins.PvCenter.香港报关出库}-执行财务-异常：AutoVouchers({tast.MainID}),{DateTime.Now})");
                            //    Console.WriteLine(ex.Message);
                            //    Console.WriteLine(ex.StackTrace);
                            //    LitTools.Current.Log($"{TaskSettgins.PvCenter.香港报关出库}-执行财务-异常：AutoVouchers({tast.MainID}),{DateTime.Now}",
                            //        ex.Message,
                            //        ex.StackTrace);
                            //}

                            //Console.WriteLine();
                            
                        //}
                        #endregion

                        #region 单一窗口申报后深圳出库
                        if (tast.Name == TaskSettgins.PvCenter.单一窗口申报后深圳出库)
                        {
                            try
                            {
                                LitTools.Current.Record(tast.MainID, tast.Context);
                                Console.WriteLine($"正在处理{TaskSettgins.PvCenter.单一窗口申报后深圳出库}-执行库房：AutoHkExitNoticeForTask({tast.MainID}),{DateTime.Now}");

                                var cgDelcare = tast.Context.JsonTo<Wms.Services.chonggous.Models.CgDelcare>();
                                // service.AutoHkExitNoticeForTask(cgDelcare);
                                // 香港库房报关重构调用
                                service.AutoHkExitNoticeForTaskNew(cgDelcare);

                                Console.WriteLine($"处理结束{TaskSettgins.PvCenter.单一窗口申报后深圳出库}-执行库房：AutoHkExitNoticeForTask({tast.MainID}),{DateTime.Now}");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"{TaskSettgins.PvCenter.单一窗口申报后深圳出库}-执行库房-异常：AutoHkExitNoticeForTask({tast.MainID}),{DateTime.Now})");
                                Console.WriteLine(ex.Message);
                                Console.WriteLine(ex.StackTrace);
                                LitTools.Current.Log($"{TaskSettgins.PvCenter.单一窗口申报后深圳出库}-执行库房-异常：AutoHkExitNoticeForTask({tast.MainID}),{DateTime.Now}",
                                    ex.Message,
                                    ex.StackTrace);

                                LitTools.Current.RecordFailed(tast.MainID, tast.Context);
                            }

                            Console.WriteLine();
                        }
                        #endregion
                    }
                }
            }
        }

        /// <summary>
        /// 深圳报关内单出库
        /// </summary>
        public void SZCustomInsExit()
        {
            using (var pvcenter = new PvCenterReponsitory())
            {
                var taskspoll = pvcenter.ReadTable<Layers.Data.Sqls.PvCenter.TasksPool>()
                    .Where(item => item.Name == TaskSettgins.PvCenter.深圳报关内单出库)
                    .Take(3).ToArray();
                var ids = taskspoll.Select(item => item.ID).ToArray();
                //删除任务
                pvcenter.Delete<Layers.Data.Sqls.PvCenter.TasksPool>(item => ids.Contains(item.ID));

                using (CgSzInsidePickingsView service = new CgSzInsidePickingsView())
                {
                    foreach (var tast in taskspoll)
                    {
                        if (tast.Name == TaskSettgins.PvCenter.深圳报关内单出库)
                        {
                            try
                            {
                                Console.WriteLine($"正在处理{TaskSettgins.PvCenter.深圳报关内单出库}:SZCustomInsExit({tast.MainID}),{DateTime.Now}");
                                service.SZCustomInsExit(tast.MainID);
                                Console.WriteLine($"处理结束{TaskSettgins.PvCenter.深圳报关内单出库}:SZCustomInsExit({tast.MainID}),{DateTime.Now}");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"{TaskSettgins.PvCenter.深圳报关内单出库}-异常：SZCustomInsExit({tast.MainID}),{DateTime.Now})");
                                Console.WriteLine(ex.Message);
                                Console.WriteLine(ex.StackTrace);
                                LitTools.Current.Log($"{TaskSettgins.PvCenter.深圳报关内单出库}-异常：SZCustomInsExit({tast.MainID}),{DateTime.Now}",
                                      ex.Message,
                                      ex.StackTrace);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 华芯通费用同步, 香港库房报关重构后仅供深圳出库时深圳库房费用同步
        /// </summary>
        /// <remarks>香港出库后，再添加的费用需要同步到华芯通</remarks>
        public void AfterHKExitAutoVouchers()
        {
            using (var pvcenter = new PvCenterReponsitory())
            {
                var taskspoll = pvcenter.ReadTable<Layers.Data.Sqls.PvCenter.TasksPool>()
                    .Where(item => item.Name == TaskSettgins.PvCenter.华芯通费用同步)
                    .Take(3).ToArray();
                var ids = taskspoll.Select(item => item.ID).ToArray();
                //删除任务
                pvcenter.Delete<Layers.Data.Sqls.PvCenter.TasksPool>(item => ids.Contains(item.ID));

                using (CgDelcareShipView service = new CgDelcareShipView())
                {
                    foreach (var tast in taskspoll)
                    {
                        if (tast.Name == TaskSettgins.PvCenter.华芯通费用同步)
                        {
                            try
                            {
                                Console.WriteLine($"正在处理{TaskSettgins.PvCenter.华芯通费用同步}-执行财务同步：AfterHKExitAutoVouchers({tast.MainID}),{DateTime.Now}");
                                service.AutoVouchers(service.GetLotNumberByOrderID(tast.MainID), tast.MainID);
                                Console.WriteLine($"处理结束{TaskSettgins.PvCenter.华芯通费用同步}-执行财务同步：AfterHKExitAutoVouchers({tast.MainID}),{DateTime.Now}");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"{TaskSettgins.PvCenter.华芯通费用同步}-执行财务同步-异常：AfterHKExitAutoVouchers({tast.MainID}),{DateTime.Now})");
                                Console.WriteLine(ex.Message);
                                Console.WriteLine(ex.StackTrace);
                                LitTools.Current.Log($"{TaskSettgins.PvCenter.华芯通费用同步}-执行财务同步-异常：AfterHKExitAutoVouchers({tast.MainID}),{DateTime.Now}",
                                    ex.Message,
                                    ex.StackTrace);
                            }

                            Console.WriteLine();
                        }
                    }
                }
            }
        }
    }
}
