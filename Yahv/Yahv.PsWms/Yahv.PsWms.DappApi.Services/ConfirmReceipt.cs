using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Yahv.PsWms.DappApi.Services.Enums;
using Yahv.Underly;
using Yahv.Utils.Http;

namespace Yahv.PsWms.DappApi.Services
{
    public delegate void SqlErrorEventHandler(object sender, EventArgs e);
    public class ConfirmReceipt
    {

        Thread runner;

        object objectLock = new object();
        event SqlErrorEventHandler sqlError;

        /// <summary>
        /// sql错误事件
        /// </summary>
        public event SqlErrorEventHandler SqlError
        {
            add
            {
                lock (objectLock)
                {

                    System.Reflection.EventInfo eventInfo = this.GetType().GetEvent(nameof(this.SqlError));

                    if (sqlError != null)
                    {
                        foreach (Delegate dele in sqlError.GetInvocationList())
                        {
                            sqlError -= (dele as SqlErrorEventHandler);
                            eventInfo.RemoveEventHandler(this, dele);
                        }

                    }
                    sqlError += value;
                }
            }
            remove
            {
                lock (objectLock)
                {
                    sqlError -= value;
                }
            }
        }

        public ConfirmReceipt()
        {
            this.runner = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        this.ReceiptConfirm();
                    }
                    catch (SqlException ex)
                    {
                        //如果Console.WriteLine(); 显示错误需要增加事件
                        if (this != null && this.sqlError != null)
                        {
                            this.sqlError(ex.Message, new EventArgs());
                        }

                        Logs.Log(ex.InnerException ?? ex);

                        continue;
                    }
                    catch (DbException)
                    {
                        //如果Console.WriteLine(); 显示错误需要增加事件
                        continue;
                    }
                    catch (Exception ex)
                    {
                        //控制台输出错误
                        Console.WriteLine((ex.InnerException ?? ex).Message);
                        //然后记录日志并进行下次操作
                        Logs.Log(ex.InnerException ?? ex);
                        continue;
                    }
                    finally
                    {
                        Thread.Sleep(10);
                    }
                }
            });
        }

        public void Start()
        {
            this.runner.Start();
        }

        static ConfirmReceipt current;
        static object obj = new object();

        /// <summary>
        /// 单例
        /// </summary>
        static public ConfirmReceipt Current
        {
            get
            {
                if (current == null)
                {
                    lock (obj)
                    {
                        if (current == null)
                        {
                            current = new ConfirmReceipt();
                        }
                    }
                }
                return current;
            }
        }


        //确认收货的天数
        int confirmDaysCount = int.Parse(ConfigurationManager.AppSettings["confirmDaysCount"]);

        /// <summary>
        /// 收货确认
        /// </summary>
        public void ReceiptConfirm()
        {
            /*
             逻辑流程：
             1.取出出库的 等待收货 的是快递的订单；
             2.判断是7天后自动确认收货更改状态为已完成，并更新客户端订单状态为已完成
             */

            using (var pswms = new PsWmsRepository())
            {
                //出库找收货人信息
                var linq = from notice in pswms.ReadTable<Layers.Data.Sqls.PsWms.Notices>()
                           join consignee in pswms.ReadTable<Layers.Data.Sqls.PsWms.NoticeTransports>()
                           on notice.ConsigneeID equals consignee.ID
                           where notice.NoticeType == (int)NoticeType.Outbound
                                && notice.Status == (int)NoticeStatus.Arrivaling
                                && consignee.TransportMode == (int)TransportMode.Express
                           orderby notice.ID descending
                           select new
                           {
                               NoticeID = notice.ID,
                               CreateDate = notice.CreateDate,
                               ModifyDate = notice.ModifyDate,
                               OrderID=notice.FormID//对应的客户端的OrderID
                           };

                var notices = linq.Take(10).ToArray();
                foreach (var notice in notices)
                {
                    var lastDate = notice.ModifyDate == null ? notice.CreateDate : notice.ModifyDate;

#if DEBUG || TEST
                    if (lastDate.AddMinutes(1) <= DateTime.Now)
#else
                    //七天后该快递自动确认收货，状态为已完成
                    if (lastDate.AddDays(confirmDaysCount) <= DateTime.Now)
#endif
                    {
                        pswms.Update<Layers.Data.Sqls.PsWms.Notices>(new
                        {
                            Status = (int)NoticeStatus.Completed
                        }, item => item.ID == notice.NoticeID);

                        //调用客户端订单已完成接口
                        string url = Services.Enums.FromType.RealChangeOrder.GetDescription();
                        ApiHelper.Current.JPost(url, new { OrderID = notice.OrderID, OrderStatus = (int)NoticeStatus.Completed });

                    }

                }

            }
        }
    }
}
