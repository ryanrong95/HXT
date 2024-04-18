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
    ///// 转报关订单
    ///// </summary>
    //public class TransferEntryOrderExtends : OrderExtends
    //{
    //    #region 拓展属性
    //    /// <summary>
    //    /// 付汇供应商对象
    //    /// </summary>
    //    public WsSupplier[] PayExchangeSupplier { get; set; }

    //    #endregion


    //    public TransferEntryOrderExtends()
    //    {
    //        //this.ExecutionStatus = ExcuteStatus.香港待装箱;
    //        this.IsReturned = false;
    //        this.EnterSuccess += TransferEntryOrderExtends_EnterSuccess;
    //    }

    //    /// <summary>
    //    /// 订单项保存
    //    /// </summary>
    //    /// <param name="sender"></param>
    //    /// <param name="e"></param>
    //    private void TransferEntryOrderExtends_EnterSuccess(object sender, Usually.SuccessEventArgs e)
    //    {
    //        //订单项保存
    //        InsertOrderItems();

    //        //数据保存完后发给荣检
    //        Task.Run(() =>
    //        {
    //            if (this.MainStatus == CgOrderStatus.已提交)
    //            {
    //                this.SumbitToXDT();
    //            }
    //        });
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

    //            //文件保存
    //            Task.Run(() =>
    //            {
    //                new CenterFilesView().XDTUpload(this.OrderFiles.Select(item => new CenterFileDescription
    //                {
    //                    CustomName = item.CustomName,
    //                    Url = item.Url,
    //                    AdminID = item.AdminID,
    //                    WsOrderID = this.ID,
    //                    Type = item.Type,
    //                }).ToArray());
    //            });

    //            //付汇供应商和状态持久化
    //            this.MapsSupplierEnter(Reponsitory, this.PayExchangeSuppliers, this.ID);
    //            this.StatusLogEnter();
    //            if (this.MainStatus == CgOrderStatus.已提交)
    //            {
    //                this.EntrySubmit();
    //            }

    //            task1.Wait();//等待运单持久化结束
    //            OrderOutputEnter(Reponsitory);
    //        }
    //        this.OnEnterSuccess();
    //    }

    //}
}
