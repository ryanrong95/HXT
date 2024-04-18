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
    ///// 代发货订单处理类
    ///// </summary>
    //public class DeliveryOrderExtends : OrderExtends
    //{
    //    /// <summary>
    //    /// 初始化
    //    /// </summary>
    //    public DeliveryOrderExtends()
    //    {
    //        //this.ExecutionStatus = ExcuteStatus.香港待出库;
    //        this.EnterSuccess += DeliveryOrderExtends_EnterSuccess;
    //    }

    //    /// <summary>
    //    /// 表头保存成功触发事件
    //    /// </summary>
    //    /// <param name="sender"></param>
    //    /// <param name="e"></param>
    //    private void DeliveryOrderExtends_EnterSuccess(object sender, Usually.SuccessEventArgs e)
    //    {
    //        //订单项保存
    //        InsertOrderItems();

    //        //生成出库通知
    //        //var result = Extends.NoticeExtends.OutNotice(this as OrderExtends);
    //        //this.HandleOutNotice(result);
    //    }


    //    /// <summary>
    //    /// 持久化
    //    /// </summary>
    //    public void Enter()
    //    {
    //        using (PvWsOrderReponsitory Reponsitory = LinqFactory<PvWsOrderReponsitory>.Create())
    //        {
    //            this.ID = this.ID ?? Layers.Data.PKeySigner.Pick(PKeyType.Order, this.EnterCode);
    //            #region 保存运单
    //            Task task1 = new Task(() =>
    //            {
    //                this.OutWaybill.OrderID = this.ID;
    //                this.OutWaybill.Enter();
    //            });
    //            task1.Start();
    //            #endregion

    //            this.OrderEnter(Reponsitory);

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

    //            this.StatusLogEnter();

    //            task1.Wait();//等待运单持久化结束
    //            OrderOutputEnter(Reponsitory);
    //        }
    //        this.OnEnterSuccess();
    //    }
    //}
}
