using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 用户确认收货操作
    /// </summary>
    public class UserConfirmReceipt
    {
        private string MainOrderID { get; set; } = string.Empty;

        public UserConfirmReceipt(string mainOrderID)
        {
            this.MainOrderID = mainOrderID;
        }

        public void Execute()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //查出该大订单下所有的小订单号
                string[] tinyOrderIDs = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                                            .Where(t => t.MainOrderId == this.MainOrderID
                                                     && t.Status == (int)Enums.Status.Normal)
                                            .Select(t => t.ID).ToArray();

                if (tinyOrderIDs == null || !tinyOrderIDs.Any())
                {
                    return;
                }

                //修改这小订单的状态
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new
                {
                    OrderStatus = (int)Enums.OrderStatus.Completed,
                }, item => tinyOrderIDs.Contains(item.ID));

                //插入OrderLogs
                foreach (var tinyOrderID in tinyOrderIDs)
                {
                    reponsitory.Insert<Layer.Data.Sqls.ScCustoms.OrderLogs>(new Layer.Data.Sqls.ScCustoms.OrderLogs
                    {
                        ID = Needs.Overall.PKeySigner.Pick(Needs.Ccs.Services.PKeyType.OrderLog),
                        OrderID = tinyOrderID,
                        OrderStatus = (int)Enums.OrderStatus.Completed,
                        CreateDate = DateTime.Now,
                        Summary = "该订单已出库完成，已将订单状态置为【完成】",
                    });

                    int targetCount = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderTraces>()
                                    .Where(t => t.OrderID == tinyOrderID
                                             && t.Step == (int)Enums.OrderTraceStep.Completed).Count();
                    if (targetCount <= 0)
                    {
                        // OrderTraces 表中插入订单轨迹
                        reponsitory.Insert<Layer.Data.Sqls.ScCustoms.OrderTraces>(new Layer.Data.Sqls.ScCustoms.OrderTraces()
                        {
                            ID = Needs.Overall.PKeySigner.Pick(Needs.Ccs.Services.PKeyType.OrderTrace),
                            OrderID = tinyOrderID,
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
