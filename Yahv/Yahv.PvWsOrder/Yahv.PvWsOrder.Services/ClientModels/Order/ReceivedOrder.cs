using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Payments;
using Yahv.PvWsOrder.Services.ClientViews;
using Yahv.Services.Models;
using Yahv.Underly;


namespace Yahv.PvWsOrder.Services.ClientModels
{
    /// <summary>
    /// 收货订单，包含即收即发，代收货
    /// </summary>
    public class ReceivedOrder : OrderExtends
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public ReceivedOrder()
        {
            this.EnterSuccess += RecievedOrderExtends_EnterSuccess;
        }

        /// <summary>
        /// 表头保存成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RecievedOrderExtends_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var task = new Task(() =>
            {
                foreach (var item in this.OrderItems)
                {
                    Yahv.Services.Views.ProductsTopView<PvWsOrderReponsitory>.Enter(item.Product);
                }
            });
            task.Start();

            //订单项新增处理
            var task1 = new Task(InsertOrderItems);
            task1.Start();
            //错误处理
            OrderContinue(task1);

            if (this.IsModify)
            {
                using (PvWsOrderReponsitory reponsitory = LinqFactory<PvWsOrderReponsitory>.Create())
                {
                    var currentids = new OrderItemAlls(reponsitory).Where(item => item.OrderID == this.ID).Select(item => item.ID).ToArray();
                    //修改数据
                    var updateItems = this.OrderItemCompare(this.OrderItems.Where(item => !string.IsNullOrWhiteSpace(item.ID)).ToArray(), reponsitory);
                    Task<OrderItem[]> task2 = new Task<OrderItem[]>(() => UpdateOrderItems(updateItems));
                    task2.Start();
                    //错误处理
                    OrderContinue(task2);


                    //排除页面提交的数据
                    var deleteitemids = currentids.Except(this.OrderItems.Select(item => item.ID).ToArray());
                    //删除的数据
                    reponsitory.Update<Layers.Data.Sqls.PvWsOrder.OrderItems>(new
                    {
                        ModifyDate = DateTime.Now,
                        Status = (int)Enums.OrderItemStatus.Deleted,
                    }, item => deleteitemids.Contains(item.ID));

                    //删除代收代付要求
                    //reponsitory.Delete<Layers.Data.Sqls.PvWsOrder.Applications>(item => item.OrderID == this.ID);

                    task2.Wait();
                }
            }
            //待收货保存成功后，记录代理费
            //Task.Run(() =>
            //{
            //    var fee = PaymentTools.Receivables[ConductConsts.供应链, CatalogConsts.代理费, CatalogConsts.代理费];
            //    PaymentManager.Npc[this.ClientID, PvClientConfig.CompanyID][ConductConsts.供应链].Receivable[CatalogConsts.代理费].Record(fee.Quotes.Currency,
            //      fee.Quotes.Price, this.ID, this.InWaybill.ID);
            //});

            Task.WaitAll(task, task1);
        }


        /// <summary>
        /// 表头保存成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Enter()
        {
            using (PvWsOrderReponsitory Reponsitory = LinqFactory<PvWsOrderReponsitory>.Create())
            {
                #region 主键设置
                this.ID = this.ID ?? Layers.Data.PKeySigner.Pick(PKeyType.Order, this.EnterCode);
                this.InWaybill.ID = this.InWaybill.ID ?? Layers.Data.PKeySigner.Pick(PKeyType.Waybill);
                if (this.OutWaybill != null)
                {
                    this.OutWaybill.ID = this.OutWaybill.ID ?? Layers.Data.PKeySigner.Pick(PKeyType.Waybill);
                }
                #endregion

                #region 保存运单
                Task task1 = new Task(() =>
                {
                    this.InWaybill.OrderID = this.ID;
                    if (this.OutWaybill != null)
                    {
                        this.OutWaybill.OrderID = this.ID;
                        this.OutWaybill.Enter();
                        this.InWaybill.TransferID = this.OutWaybill.ID;
                    }
                    this.InWaybill.Enter();
                });
                task1.Start();
                #endregion

                //订单信息保存
                this.OrderEnter(Reponsitory);
                OrderInputEnter(Reponsitory);
                OrderOutputEnter(Reponsitory);
                this.StatusLogEnter();
                //货物特殊要求插入
                this.OrderRequirementEnter(Reponsitory);

                //文件保存
                Task.Run(() =>
                {
                    //订单文件
                    new CenterFilesView().XDTUpload(this.OrderFiles.Select(item => new CenterFileDescription
                    {
                        CustomName = item.CustomName,
                        Url = item.Url,
                        AdminID = item.AdminID,
                        WsOrderID = this.ID,
                        Type = item.Type,
                    }).ToArray());
                    //货物特殊文件
                    if (Requirements != null)
                    {
                        new CenterFilesView().Upload(Requirements.Where(item => item.RequireFiles != null).Select(item => new CenterFileDescription
                        {
                            CustomName = item.RequireFiles.CustomName,
                            Url = item.RequireFiles.Url,
                            AdminID = item.RequireFiles.AdminID,
                            WsOrderID = item.ID,
                            Type = item.RequireFiles.Type,
                        }).ToArray());
                    }
                });
                task1.Wait();//等待运单持久化结束
            }
            this.OnEnterSuccess();
        }
    }
}
