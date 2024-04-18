using System;
using System.Linq;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.PvWsOrder.Services.ClientViews;
using Yahv.PvWsOrder.Services.Extends;
using Yahv.Services.Models;
using Yahv.Underly;


namespace Yahv.PvWsOrder.Services.ClientModels
{
    public class DeclareOrder : OrderExtends
    {
        #region 拓展属性
        /// <summary>
        /// 付汇供应商对象
        /// </summary>
        public wsnSuppliers[] PayExchangeSupplier { get; set; }

        #endregion

        /// <summary>
        /// 初始化
        /// </summary>
        public DeclareOrder()
        {
            this.IsReturned = false;
            this.EnterSuccess += EntryOrderExtends_EnterSuccess;
        }

        /// <summary>
        /// 表头保存成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EntryOrderExtends_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            Task task = new Task(() =>
            {
                foreach (var item in this.OrderItems)
                {
                    Yahv.Services.Views.ProductsTopView<PvWsOrderReponsitory>.Enter(item.Product);
                }
            });
            task.Start();

            //订单项新增处理
            Task task1 = new Task(() => InsertOrderItems());
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

                    //数据库中存在,页面不存在数据
                    var deleteitemids = currentids.Except(this.OrderItems.Select(item => item.ID).ToArray());
                    //删除的数据
                    reponsitory.Update<Layers.Data.Sqls.PvWsOrder.OrderItems>(new
                    {
                        ModifyDate = DateTime.Now,
                        Status = (int)Enums.OrderItemStatus.Deleted,
                    }, item => deleteitemids.Contains(item.ID));
                    task2.Wait();
                }
            }
            Task.WaitAll(task, task1);

            //数据保存完后发给荣检
            Task.Run(() =>
            {
                if (this.MainStatus == CgOrderStatus.待审核)
                {
                    this.SumbitToXdt();
                }
            });
        }

        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            using (PvWsOrderReponsitory reponsitory = LinqFactory<PvWsOrderReponsitory>.Create())
            {
                #region 主键设置
                this.ID = this.ID ?? Layers.Data.PKeySigner.Pick(PKeyType.Order, this.EnterCode);
                this.OutWaybill.ID = this.OutWaybill.ID ?? Layers.Data.PKeySigner.Pick(PKeyType.Waybill);
                if (this.InWaybill != null)
                {
                    this.InWaybill.ID = this.InWaybill.ID ?? Layers.Data.PKeySigner.Pick(PKeyType.Waybill);
                }
                #endregion

                #region 保存运单
                Task task1 = new Task(() =>
                {
                    this.OutWaybill.OrderID = this.ID;
                    this.OutWaybill.Enter();
                    if (this.InWaybill != null)
                    {
                        this.InWaybill.OrderID = this.ID;
                        this.InWaybill.Enter();
                    }
                });
                task1.Start();
                #endregion

                //订单表头保存
                this.OrderEnter(reponsitory);
                //订单拓展表插入
                OrderInputEnter(reponsitory);
                OrderOutputEnter(reponsitory);
                //付汇供应商和状态持久化
                this.MapsSupplierEnter(reponsitory, this.PayExchangeSuppliers, this.ID);
                this.StatusLogEnter();


                //文件保存
                Task.Run(() =>
                {
                    var centerfilesView = new CenterFilesView();
                    centerfilesView.DeleteByLambda(item => item.WsOrderID == this.ID);
                    centerfilesView.XDTUpload(this.OrderFiles.Select(item => new CenterFileDescription
                    {
                        CustomName = item.CustomName,
                        Url = item.Url,
                        AdminID = item.AdminID,
                        WsOrderID = this.ID,
                        Type = item.Type,
                    }).ToArray());
                });

                task1.Wait();
            }
            this.OnEnterSuccess();
        }


        /// <summary>
        /// 退回订单持久化
        /// </summary>
        public void ReturnEnter()
        {
            //只能修改订单项数据,订单数据不可以修改
            this.IsModify = true;
            this.MainStatus = CgOrderStatus.待审核;
            this.StatusLogEnter();

            //订单项编辑处理
            using (PvWsOrderReponsitory reponsitory = new PvWsOrderReponsitory(false))
            {
                //获取当前修改的订单项
                var modifyitems = this.OrderItems.Where(item => item.Status == Enums.OrderItemStatus.Returned).ToArray();
                var modifyids = modifyitems.Where(c => c.TinyOrderID != null).Select(item => item.TinyOrderID).Distinct();

                #region 小订单退回数据删除
                //先删除改小订单所有数据
                var currentids = new OrderItemAlls(reponsitory).Where(item => modifyids.Contains(item.TinyOrderID)).Select(item => item.ID).ToArray();
                reponsitory.Delete<Layers.Data.Sqls.PvWsOrder.OrderItemsChcd>(item => currentids.Contains(item.ID));
                reponsitory.Delete<Layers.Data.Sqls.PvWsOrder.OrderItemsTerm>(item => currentids.Contains(item.ID));
                reponsitory.Delete<Layers.Data.Sqls.PvWsOrder.OrderItems>(item => currentids.Contains(item.ID));
                #endregion

                #region 数据持久化
                //产品数据持久化
                foreach(var item in this.OrderItems)
                {
                    Yahv.Services.Views.ProductsTopView<PvWsOrderReponsitory>.Enter(item.Product);
                    item.ProductID = item.Product.ID;
                    if(string.IsNullOrWhiteSpace(item.ID))
                    {
                        item.ID = Layers.Data.PKeySigner.Pick(PKeyType.OrderItem);
                        item.InputID = Layers.Data.PKeySigner.Pick(PKeyType.Input);
                        item.OutputID = Layers.Data.PKeySigner.Pick(PKeyType.Output);
                        item.TinyOrderID = modifyids.FirstOrDefault();
                    }
                }

                //订单项数据持久化
                var linq = modifyitems.Select(item => new Layers.Data.Sqls.PvWsOrder.OrderItems
                {
                    ID = item.ID,
                    InputID = item.InputID,
                    OutputID = item.OutputID,
                    OrderID = this.ID,
                    TinyOrderID = item.TinyOrderID,
                    ProductID = item.ProductID,
                    CustomName = item.Name,
                    Origin = item.Origin,
                    Quantity = item.Quantity,
                    Currency = (int)item.Currency,
                    UnitPrice = item.UnitPrice,
                    Unit = (int)item.Unit,
                    TotalPrice = item.TotalPrice,
                    CreateDate = item.CreateDate,
                    ModifyDate = DateTime.Now,
                    GrossWeight = item.GrossWeight,
                    Volume = item.Volume,
                    Conditions = item.Conditions,
                    Status = (int)Enums.OrderItemStatus.Normal,
                    IsAuto = item.IsAuto,
                    WayBillID = item.WayBillID,
                    Type = (int)Enums.OrderItemType.Modified,
                    StorageID = item.StorageID,
                }).ToArray();

                reponsitory.Insert(linq);
                #endregion

                reponsitory.Submit();
            }


            //数据保存完后发给荣检
            Task.Run(() =>
            {
                this.OrderItems = this.OrderItems.Where(item => item.Status == Enums.OrderItemStatus.Returned).ToArray();
                this.PayExchangeSuppliers = this.PayExchangeSupplier.Select(item => item.ID).ToArray();
                this.IsReturned = true;

                var result = this.XDTOrderNotice();
                //调用结果日志
                Logger.log(this.CreatorID, new OperatingLog
                {
                    MainID = this.ID,
                    Operation = "芯达通订单退回修改后对接结果日志！",
                    Summary = result,
                });
            });
        }
    }
}
