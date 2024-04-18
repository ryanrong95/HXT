using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Warehouse.Services.Models
{
//    public class MyExitNotice : ExitNotice
//    {
//        void TTT() { }

//        string TTED;
//    }

//    /// <summary>
//    /// 类的名称为 签收单上传 SignReceiptUpload
//    /// 完成出库通知的文件上传、订单文件的上传，订单完成
//    /// </summary>
//    public class SignReceiptUpload
//    {
//        private MyExitNotice ExitNotice;

//        private Order Order;

//        public SignReceiptUpload()
//        {

//            ExitNotice.CLII
//        }

//        /// <summary>
//        /// TODO:这里的构造函数，也可以是入库通知ID，在构造函数中，进行出库通知的实例化
//        /// 同时，签收单也要同步上传到订单的附件中。
//        /// 这里需要根据实际的业务逻辑进行设计构造函数
//        /// </summary>
//        /// <param name="exitNotice"></param>
//        public SignReceiptUpload(Needs.Wl.Models.ExitNotice exitNotice) : this()
//        {
//            this.ExitNotice = exitNotice;
//            this.Order = OrderView[exitNotice.OrderID];
//            //this.Order.Completed+=OrderCompleted;
//        }

//        public void SetFile(string url)
//        {

//        }

//        public void Upload()
//        {
//            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
//            {
//                //trans
//                this.ExitNotice.reponsitory = reponsitory;
//                this.ExitNotice.Completed();


//                this.Order.reponsitory = reponsitory;

//                this.Order.Completed();
//                //trans.commit();
               
//            }

//            ExitNoticeFile file = new ExitNoticeFile();
//            file.Enter();
          
//            //this.
//            this.ExitNotice.Files.Add(url);
//                //this.Order.Files.Add(url)

//        }

//        private void OrderCompleted()
//        {
//            //订单完成
//            //日志：
//        }
//    }

    //public class ExitNoticeFile : Needs.Wl.Models.ExitNoticeFile
    //{
    //    public ExitNoticeFile() : base()
    //    {

    //    }

    //    public void InsertUniqueFileForOneExitNotice(string exitNoticeID)
    //    {
    //        //将 ExitNoticeFiles 表中旧的该通知对应的文件数据置为 400
    //        //然后再插入新的文件数据
    //        string[] oldNormalExitNoticeFilesID = new Needs.Wl.Models.Views.ExitNoticeFilesView()
    //            .Where(t => t.ExitNoticeID == exitNoticeID
    //                     && t.Status == Needs.Wl.Models.Enums.Status.Normal)
    //            .Select(t => t.ID).ToArray();

    //        using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
    //        {
    //            if (oldNormalExitNoticeFilesID != null && oldNormalExitNoticeFilesID.Length > 0)
    //            {
    //                reponsitory.Update<Layer.Data.Sqls.ScCustoms.ExitNoticeFiles>(new
    //                {
    //                    Status = (int)Needs.Wl.Models.Enums.Status.Delete,
    //                }, item => oldNormalExitNoticeFilesID.Contains(item.ID));
    //            }

    //            this.EnterSuccess += InsertUniqueFileForOneExitNotice_Successed;

    //            //插入新的文件数据
    //            this.Enter();
    //        }
    //    }

    //    /// <summary>
    //    /// 插入出库通知文件成功后事件
    //    /// </summary>
    //    /// <param name="sender"></param>
    //    /// <param name="e"></param>
    //    private void InsertUniqueFileForOneExitNotice_Successed(object sender, SuccessEventArgs e)
    //    {
    //        //修改该出库通知状态置为“已完成”
    //        Needs.Wl.Models.ExitNoticeFile exitNoticeFile = (Needs.Wl.Models.ExitNoticeFile)e.Object;

    //        using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
    //        {
    //            reponsitory.Update<Layer.Data.Sqls.ScCustoms.ExitNotices>(new
    //            {
    //                ExitNoticeStatus = (int)Needs.Wl.Models.Enums.ExitNoticeStatus.Completed,
    //            }, item => item.ID == exitNoticeFile.ExitNoticeID);

    //            // ExitNoticeLogs 表中插入日志
    //            reponsitory.Insert<Layer.Data.Sqls.ScCustoms.ExitNoticeLogs>(new Layer.Data.Sqls.ScCustoms.ExitNoticeLogs()
    //            {
    //                ID = Guid.NewGuid().ToString("N"),
    //                ExitNoticeID = exitNoticeFile.ExitNoticeID,
    //                AdminID = exitNoticeFile.AdminID,
    //                OperType = (int)Needs.Wl.Models.Enums.ExitOperType.Completed,
    //                CreateDate = DateTime.Now,
    //                Summary = "客户收货确认单已上传，出库通知状态已置为【送货完成】",
    //            });

    //            //该出库通知对应的订单中，如果所有出库通知都为已完成，则将订单的状态置为“已完成”

    //            var currentExitNotice = new Needs.Wl.Models.Views.ExitNoticesView(reponsitory).Where(t => t.ID == exitNoticeFile.ExitNoticeID).FirstOrDefault();
    //            if (currentExitNotice != null)
    //            {
    //                int SZ_NotCompleted_ForThisOrder_Count = new Needs.Wl.Models.Views.ExitNoticesView(reponsitory)
    //                    .Where(t => t.WarehouseType == Wl.Models.Enums.WarehouseType.ShenZhen
    //                             && t.OrderID == currentExitNotice.OrderID
    //                             && t.Status == Wl.Models.Enums.Status.Normal
    //                             && t.ExitNoticeStatus != Wl.Models.Enums.ExitNoticeStatus.Completed).Count();

    //                if (SZ_NotCompleted_ForThisOrder_Count <= 0)
    //                {
    //                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new Layer.Data.Sqls.ScCustoms.Orders
    //                    {
    //                        OrderStatus = (int)Needs.Wl.Models.Enums.OrderStatus.Completed,
    //                    }, item => item.ID == currentExitNotice.OrderID);

    //                    // OrderLogs 表插入日志
    //                    reponsitory.Insert<Layer.Data.Sqls.ScCustoms.OrderLogs>(new Layer.Data.Sqls.ScCustoms.OrderLogs()
    //                    {
    //                        ID = Needs.Overall.PKeySigner.Pick(Needs.Wl.Models.PKeyType.OrderLog),
    //                        OrderID = currentExitNotice.OrderID,
    //                        AdminID = exitNoticeFile.AdminID,
    //                        OrderStatus = (int)Needs.Wl.Models.Enums.OrderStatus.Completed,
    //                        CreateDate = DateTime.Now,
    //                        Summary = "该订单已出库完成，已将订单状态置为【完成】",
    //                    });

    //                    int targetCount = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderTraces>()
    //                        .Where(t => t.OrderID == currentExitNotice.OrderID
    //                                 && t.Step == (int)Needs.Wl.Models.Enums.OrderTraceStep.Completed).Count();
    //                    if (targetCount <= 0)
    //                    {
    //                        // OrderTraces 表中插入订单轨迹
    //                        reponsitory.Insert<Layer.Data.Sqls.ScCustoms.OrderTraces>(new Layer.Data.Sqls.ScCustoms.OrderTraces()
    //                        {
    //                            ID = Needs.Overall.PKeySigner.Pick(Needs.Wl.Models.PKeyType.OrderTrace),
    //                            OrderID = currentExitNotice.OrderID,
    //                            AdminID = exitNoticeFile.AdminID,
    //                            Step = (int)Needs.Wl.Models.Enums.OrderTraceStep.Completed,
    //                            CreateDate = DateTime.Now,
    //                            Summary = "您的订单已完成，感谢使用一站式报关服务，期待与您的下次合作",
    //                        });
    //                    }
    //                }
    //            }

    //        }

    //    }

    //}
}
