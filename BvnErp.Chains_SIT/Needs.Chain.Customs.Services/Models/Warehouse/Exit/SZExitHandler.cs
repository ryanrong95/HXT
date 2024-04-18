using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 深证库房出库，完成操作处理
    /// </summary>
    public class SZExitCompleteHandler
    {
        private string _exitNoticeID = string.Empty;

        private string _adminID = string.Empty;

        public SZExitCompleteHandler(string exitNoticeID, string adminID)
        {
            this._exitNoticeID = exitNoticeID;
            this._adminID = adminID;
        }

        public void Execute()
        {
            //如果出库通知的状态是未出库，则先执行一次已出库 Begin
            var ExitNotice = new Views.SZExitNoticeView().Where(t => t.ID == this._exitNoticeID).FirstOrDefault();

            if (ExitNotice.ExitNoticeStatus == Enums.ExitNoticeStatus.UnExited)
            {
                ExitNotice.Admin = Needs.Underly.FkoFactory<Admin>.Create(this._adminID);
                ExitNotice.OutStock();
            }
            //如果出库通知的状态是未出库，则先执行一次已出库 End

            //开始将出库通知从“已出库”变为“已完成”，并且看订单状态是否可以变为“已完成”和是否需要插入订单日志和订单轨迹
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //更新NoticeItems表状态
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.ExitNoticeItems>(new
                {
                    ExitNoticeStatus = (int)Enums.ExitNoticeStatus.Completed,
                }, item => item.ExitNoticeID == this._exitNoticeID);

                //更新ExitNotice主表的状态
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.ExitNotices>(new
                {
                    ExitNoticeStatus = (int)Enums.ExitNoticeStatus.Completed,
                }, item => item.ID == this._exitNoticeID);
                //}

                string mainorderid = ExitNotice.Order.ID;

                //查询主订单的所有型号的数量
                var orderQty = (from c in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                                join d in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>()
                                on c.ID equals d.OrderID
                                where c.MainOrderId == mainorderid && c.OrderStatus != (int)Enums.OrderStatus.Canceled && c.OrderStatus != (int)Enums.OrderStatus.Returned
                                select d.Quantity).Sum();

                //查询主订单下所有出库通知的数量
                var exitNoticeQty = (from c in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExitNotices>()
                                     join d in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExitNoticeItems>()
                                     on c.ID equals d.ExitNoticeID
                                     where c.OrderID == mainorderid && c.Status == (int)Enums.Status.Normal
                                     select d.Quantity).Sum();

                //如果两个数量相等，则继续，如果数量不等则只改出库通知的状态
                if (orderQty == exitNoticeQty)
                {
                    //判断所有的出库通知是否都已经变成已完成
                    var status = (from c in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExitNotices>()
                                  where c.OrderID == mainorderid && c.Status == (int)Enums.Status.Normal
                                  select new { ExitNoticeStatus = c.ExitNoticeStatus }).ToList();
                    bool isAny = status.Any(item => (item.ExitNoticeStatus != (int)Enums.ExitNoticeStatus.Completed));

                    if (!isAny)
                    {
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(
                            new { OrderStatus = (int)Needs.Ccs.Services.Enums.OrderStatus.Completed }, 
                            item => item.MainOrderId == mainorderid
                                    &&item.OrderStatus != (int)Enums.OrderStatus.Canceled
                                    &&item.OrderStatus!=(int)Enums.OrderStatus.Returned);

                        //写日志
                        var OrderIDs = (from c in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                                        where c.MainOrderId == mainorderid && c.OrderStatus != (int)Enums.OrderStatus.Canceled && c.OrderStatus != (int)Enums.OrderStatus.Returned
                                        select c.ID).ToList();

                        foreach (var orderid in OrderIDs)
                        {
                            string thisOrderID = orderid;

                            // OrderLogs 表插入日志
                            reponsitory.Insert<Layer.Data.Sqls.ScCustoms.OrderLogs>(new Layer.Data.Sqls.ScCustoms.OrderLogs()
                            {
                                ID = Needs.Overall.PKeySigner.Pick(Needs.Ccs.Services.PKeyType.OrderLog),
                                OrderID = thisOrderID,
                                AdminID = this._adminID,
                                OrderStatus = (int)Enums.OrderStatus.Completed,
                                CreateDate = DateTime.Now,
                                Summary = "该订单已出库完成，已将订单状态置为【完成】",
                            });

                            int targetCount = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderTraces>()
                                .Where(t => t.OrderID == thisOrderID
                                         && t.Step == (int)Enums.OrderTraceStep.Completed).Count();
                            if (targetCount <= 0)
                            {
                                // OrderTraces 表中插入订单轨迹
                                reponsitory.Insert<Layer.Data.Sqls.ScCustoms.OrderTraces>(new Layer.Data.Sqls.ScCustoms.OrderTraces()
                                {
                                    ID = Needs.Overall.PKeySigner.Pick(Needs.Ccs.Services.PKeyType.OrderTrace),
                                    OrderID = thisOrderID,
                                    AdminID = this._adminID,
                                    Step = (int)Enums.OrderTraceStep.Completed,
                                    CreateDate = DateTime.Now,
                                    Summary = "您的订单已完成，感谢使用一站式报关服务，期待与您的下次合作",
                                });
                            }

                        }
                    }
                }




            }

        }

    }

    /// <summary>
    /// 深证库房出库，插入文件处理
    /// </summary>
    public class SZExitInsertFileHandler
    {
        private ExitNoticeFile _exitNoticeFile;

        private string _exitNoticeID = string.Empty;

        private string _adminID = string.Empty;

        public SZExitInsertFileHandler(ExitNoticeFile exitNoticeFile, string exitNoticeID, string adminID)
        {
            this._exitNoticeFile = exitNoticeFile;
            this._exitNoticeID = exitNoticeID;
            this._adminID = adminID;
        }

        public void Execute()
        {
            //执行文件插入操作
            SZExitOnlyInsertFileHandler sZExitOnlyInsertFileHandler = new SZExitOnlyInsertFileHandler(this._exitNoticeFile, this._exitNoticeID);
            sZExitOnlyInsertFileHandler.Execute();

            //如果出库通知的状态是已完成，则不执行
            var exitNotice = new Views.Origins.ExitNoticesOrigin()
                .Where(t => t.ID == this._exitNoticeID
                         && t.Status == Enums.Status.Normal
                         && t.WarehouseType == Enums.WarehouseType.ShenZhen)
                .FirstOrDefault();
            if (exitNotice != null && exitNotice.ExitNoticeStatus != Enums.ExitNoticeStatus.Completed)
            {
                SZExitCompleteHandler sZExitCompleteHandler = new SZExitCompleteHandler(this._exitNoticeID, this._adminID);
                sZExitCompleteHandler.Execute();
            }

        }
    }

    /// <summary>
    /// 只上传文件
    /// </summary>
    public class SZExitOnlyInsertFileHandler
    {
        private ExitNoticeFile _exitNoticeFile;

        private string _exitNoticeID = string.Empty;

        public SZExitOnlyInsertFileHandler(ExitNoticeFile exitNoticeFile, string exitNoticeID)
        {
            this._exitNoticeFile = exitNoticeFile;
            this._exitNoticeID = exitNoticeID;
        }

        public void Execute()
        {
            //将 ExitNoticeFiles 表中旧的该通知对应的文件数据置为 400
            //然后再插入新的文件数据
            string[] oldNormalExitNoticeFilesID = new Views.Origins.ExitNoticeFilesOrigin()
                .Where(t => t.ExitNoticeID == this._exitNoticeID
                         && t.Status == Enums.Status.Normal)
                .Select(t => t.ID).ToArray();

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                if (oldNormalExitNoticeFilesID != null && oldNormalExitNoticeFilesID.Length > 0)
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.ExitNoticeFiles>(new
                    {
                        Status = (int)Enums.Status.Delete,
                    }, item => oldNormalExitNoticeFilesID.Contains(item.ID));
                }

                this._exitNoticeFile.Enter();
            }
        }
    }
}
