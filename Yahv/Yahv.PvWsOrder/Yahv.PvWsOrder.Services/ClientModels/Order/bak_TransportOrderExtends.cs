using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.PvWsOrder.Services.ClientViews;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.ClientModels
{
    ///// <summary>
    ///// 转运订单处理类
    ///// </summary>
    //public class TransportOrderExtends : OrderExtends
    //{
    //    /// <summary>
    //    /// 初始化
    //    /// </summary>
    //    public TransportOrderExtends()
    //    {
    //        //this.ExecutionStatus = ExcuteStatus.香港待入库;
    //        this.EnterSuccess += TransportOrderExtends_EnterSuccess;
    //    }


    //    /// <summary>
    //    /// 表头保存成功触发事件
    //    /// </summary>
    //    /// <param name="sender"></param>
    //    /// <param name="e"></param>
    //    private void TransportOrderExtends_EnterSuccess(object sender, Usually.SuccessEventArgs e)
    //    {
    //        Task task = new Task(() =>
    //        {
    //            foreach (var item in this.OrderItems)
    //            {
    //                Yahv.Services.Views.ProductsTopView<PvWsOrderReponsitory>.Enter(item.Product);
    //            }
    //        });
    //        task.Start();

    //        //订单项新增处理
    //        Task task1 = new Task(() => InsertOrderItems());
    //        task1.Start();
    //        //错误处理
    //        orderContinue(task1);

    //        if (this.IsModify)
    //        {
    //            using (PvWsOrderReponsitory Reponsitory = LinqFactory<PvWsOrderReponsitory>.Create())
    //            {
    //                //修改数据
    //                var UpdateItems = this.OrderItemCompare(this.OrderItems.Where(item => !string.IsNullOrWhiteSpace(item.ID)).ToArray(), Reponsitory);
    //                var logitems = new OrderItemAlls(Reponsitory).Where(item => UpdateItems.Select(a => a.ID).ToArray().Contains(item.ID)).ToArray();

    //                Task<OrderItem[]> task2 = new Task<OrderItem[]>(() => UpdateOrderItems(UpdateItems));
    //                task2.Start();
    //                //错误处理
    //                orderContinue(task2);
    //                //写入日志
    //                OrderItemsLogEnter(Reponsitory, logitems);

    //                var deleteitemids = this.OrderItems.Where(item => !string.IsNullOrWhiteSpace(item.ID)).Select(item => item.ID);
    //                //删除的数据
    //                Reponsitory.Update<Layers.Data.Sqls.PvWsOrder.OrderItems>(new
    //                {
    //                    ModifyDate = DateTime.Now,
    //                    Status = (int)GeneralStatus.Deleted,
    //                }, item => item.OrderID == this.ID && !deleteitemids.Contains(item.ID) && item.Status == (int)GeneralStatus.Normal);
    //                task2.Wait();
    //            }
    //        }
    //        Task.WaitAll(task, task1);
    //    }


    //    /// <summary>
    //    /// 表头保存成功触发事件
    //    /// </summary>
    //    /// <param name="sender"></param>
    //    /// <param name="e"></param>
    //    public void Enter()
    //    {
    //        using (PvWsOrderReponsitory Reponsitory = LinqFactory<PvWsOrderReponsitory>.Create())
    //        {
    //            this.ID = this.ID ?? Layers.Data.PKeySigner.Pick(PKeyType.Order, this.EnterCode);

    //            #region 保存运单
    //            Task task1 = new Task(() =>
    //            {
    //                this.OutWaybill.OrderID = this.InWaybill.OrderID = this.ID;
    //                this.OutWaybill.Enter();
    //                this.InWaybill.TransferID = this.OutWaybill.ID;
    //                this.InWaybill.Enter();
    //            });
    //            task1.Start();
    //            #endregion

    //            //订单表头保存
    //            this.OrderEnter(Reponsitory);
    //            this.StatusLogEnter();

    //            //文件保存
    //            Task.Run(() =>
    //            {
    //                new CenterFilesView().Upload(this.OrderFiles.Select(item => new CenterFileDescription
    //                {
    //                    CustomName = item.CustomName,
    //                    Url = item.Url,
    //                    AdminID = item.AdminID,
    //                    WsOrderID = this.ID,
    //                    Type = item.Type,
    //                }).ToArray());
    //            });

    //            task1.Wait();//等待运单持久化结束
    //            OrderInputEnter(Reponsitory);
    //            OrderOutputEnter(Reponsitory);
    //        }
    //        this.OnEnterSuccess();
    //    }
    //}
}
