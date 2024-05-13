//using Layers.Data.Sqls;
//using System;
//using System.Collections.Generic;
//using System.Data.Common;
//using System.Data.SqlClient;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using Yahv.Services;
//using Yahv.Underly;

//namespace ConsoleApp.vTaskers.Services
//{
//    /// <summary>
//    /// 箱号删除
//    /// </summary>
//    /// <remarks>
//    /// 把昨日产生的箱号删除掉
//    /// </remarks>
//    public class DeleteBoxes
//    {
//        Thread runner;

//        object objectLock = new object();
//        event SqlErrorEventHandler sqlError;

//        /// <summary>
//        /// sql错误事件
//        /// </summary>
//        public event SqlErrorEventHandler SqlError
//        {
//            add
//            {
//                lock (objectLock)
//                {

//                    System.Reflection.EventInfo eventInfo = this.GetType().GetEvent(nameof(this.SqlError));

//                    if (sqlError != null)
//                    {
//                        foreach (Delegate dele in sqlError.GetInvocationList())
//                        {
//                            sqlError -= (dele as SqlErrorEventHandler);
//                            eventInfo.RemoveEventHandler(this, dele);
//                        }

//                    }
//                    sqlError += value;
//                }
//            }
//            remove
//            {
//                lock (objectLock)
//                {
//                    sqlError -= value;
//                }
//            }
//        }
//        public DeleteBoxes()
//        {
//            this.runner = new Thread(() =>
//            {
//                //while (true)
//                //{
//                try
//                {
//                    this.Delete();
//                }
//                catch (SqlException ex)
//                {
//                    //如果Console.WriteLine(); 显示错误需要增加事件
//                    if (this != null && this.sqlError != null)
//                    {
//                        this.sqlError(ex.Message, new EventArgs());
//                    }

//                    return;
//                }
//                catch (DbException)
//                {
//                    //如果Console.WriteLine(); 显示错误需要增加事件
//                    //continue;
//                }
//                catch (Exception ex)
//                {
//                    throw ex;
//                }
//                finally
//                {
//                    Thread.Sleep(10);
//                    //任务计划加上这句，任务完成后kill当前进程
//                    System.Diagnostics.Process.GetCurrentProcess().Kill();
//                }
//                //}
//            });
//        }
//        public void Start()
//        {
//            this.runner.Start();
//        }

//        static DeleteBoxes current;
//        static object obj = new object();

//        /// <summary>
//        /// 单例
//        /// </summary>
//        static public DeleteBoxes Current
//        {
//            get
//            {
//                if (current == null)
//                {
//                    lock (obj)
//                    {
//                        if (current == null)
//                        {
//                            current = new DeleteBoxes();
//                        }
//                    }
//                }
//                return current;
//            }
//        }

//        /// <summary>
//        /// 把昨日产生的箱号删除掉
//        /// </summary>
//        /// <param name="waybillID"></param>
//        internal void Delete()
//        {
//            using (var repository = new PvWmsRepository())
//            {
               
//                //第一种处理：删除所有的昨天的箱号
//                repository.Delete<Layers.Data.Sqls.PvWms.Boxes>(item => item.CreateDate >= DateTime.Now.AddDays(-1) && item.CreateDate < DateTime.Now);


//                //第二种处理：查出所有昨天的已经出库的箱号并且删除
//                var boxcodes = repository.ReadTable<Layers.Data.Sqls.PvWms.Pickings>().Where(item => item.CreateDate >= DateTime.Now.AddDays(-1) && item.CreateDate < DateTime.Now).Select(item => item.BoxCode).Distinct();
//                repository.Delete<Layers.Data.Sqls.PvWms.Boxes>(item => boxcodes.Contains(item.Series)&&item.CreateDate >= DateTime.Now.AddDays(-1) && item.CreateDate < DateTime.Now);
//            }
//        }
//    }
//}
