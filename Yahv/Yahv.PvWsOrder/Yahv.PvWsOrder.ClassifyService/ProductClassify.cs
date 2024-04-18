using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.PvWsOrder.Services;
using Yahv.PvWsOrder.Services.ClientModels;
using Yahv.PvWsOrder.Services.ClientViews;
using Yahv.PvWsOrder.Services.Warehouse;
using Yahv.Services.Enums;
using Yahv.Services.Models;
using Yahv.Services.Views;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.PvWsOrder.Services.Enums;
using SortingNotice = Yahv.PvWsOrder.Services.Warehouse.SortingNotice;

namespace Yahv.PvWsOrder.ClassifyService
{
    public class ProductClassify
    {
        /// <summary>
        /// 收货订单归类
        /// </summary>
        public bool RecievedClassify
        {
            get
            {
                return this.OrderItemsHandle(OrderType.Recieved);
            }
        }

        /// <summary>
        /// 即收即发订单归类
        /// </summary>
        public bool TransportClassify
        {
            get
            {
                return this.OrderItemsHandle(OrderType.Transport);
            }
        }

        /// <summary>
        /// 订单归类处理
        /// </summary>
        private bool OrderItemsHandle(OrderType type)
        {
            string[] hangupids;    //挂起订单项ID
            string[] submitids;    //归类成功订单项ID

            using (PvWsOrderReponsitory Reponsitory = new PvWsOrderReponsitory(false))
            {
                //获取待提交订单数据
                var orders = new Services.Views.Origins.OrderOrigin(Reponsitory).Where(item => item.Type == type)
                    .Where(item => ((int)item.MainStatus == (int)CgOrderStatus.已提交)
                                || ((int)item.MainStatus == (int)CgOrderStatus.待审核)).Take(10);

                //没数据就返回
                if (orders.Count() == 0)
                {
                    return true;
                }

                //获取订单项数据
                var orderitems = from order in orders
                                 join item in new OrderItemAlls(Reponsitory) on order.ID equals item.OrderID
                                 select item;

                //订单项归类
                this.AutoClassify(orderitems.ToArray(), Reponsitory);

                //查询是否存在禁运和管制数据
                var linq = from item in orderitems
                           join term in Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItemsTerm>() on item.ID equals term.ID
                           where term.HkControl || term.Embargo
                           select item.OrderID;

                //挂起的订单ID
                hangupids = linq.Distinct().ToArray();
                //归类成功的订单ID
                submitids = orders.Select(item => item.ID).ToArray().Except(hangupids).ToArray();
            }

            //归类无异常则直接提交
            UpdateOrderStatus(CgOrderStatus.待交货, submitids.ToArray());
            //归类出禁运和管制数据则挂起，等待人工处理
            UpdateOrderStatus(CgOrderStatus.挂起, hangupids.ToArray());

            //操作日志记录
            var log = new Logger();
            log.Log(submitids.Select(item => new OperatingLog
            {
                MainID = item,
                Operation = "订单自动归类成功,等待入库!",
                Summary = "订单自动归类成功",
            }).ToArray());

            //即收即发订单/待收货订单生成入库通知发给库房
            CgEntryNotice(submitids, type);

            return true;
        }

        #region 更新订单状态
        /// <summary>
        /// 更新订单状态表
        /// </summary>
        /// <param name="status">主状态</param>
        /// <param name="ordersid">订单ID数组</param>
        /// <param name="Reponsitory">数据库实体</param>
        private void UpdateOrderStatus(CgOrderStatus status, string[] ordersid)
        {
            using (PvCenterReponsitory reponsitory = new PvCenterReponsitory())
            {
                //更新日志表状态
                reponsitory.Update<Layers.Data.Sqls.PvCenter.Logs_PvWsOrder>(new
                {
                    IsCurrent = false,
                }, item => ordersid.Contains(item.MainID) && item.IsCurrent == true && item.Type == (int)OrderStatusType.MainStatus);

                reponsitory.Insert(ordersid.Select(item => new Layers.Data.Sqls.PvCenter.Logs_PvWsOrder
                {
                    ID = Guid.NewGuid().ToString(),
                    MainID = item,
                    Type = (int)OrderStatusType.MainStatus,
                    Status = (int)status,
                    CreateDate = DateTime.Now,
                    CreatorID = "NPC", //系统自动
                    IsCurrent = true,
                }).ToArray());
            }
        }
        #endregion

        #region 生产入库通知

        /// <summary>
        /// 入库通知重构后
        /// </summary>
        /// <param name="orderids"></param>
        /// <param name="type"></param>
        private void CgEntryNotice(string[] orderids, OrderType type)
        {
            using (PvWsOrderReponsitory Reponsitory = LinqFactory<PvWsOrderReponsitory>.Create())
            {
                var orders = new WsOrdersTopView<PvWsOrderReponsitory>(Reponsitory).Where(item => orderids.Contains(item.ID)).ToArray();
                //根据订单生成入库通知
                foreach (var order in orders)
                {
                    try
                    {
                        #region 查询出需要的订单数据
                        var waybill = new WayBillAlls(Reponsitory)[order.Input.WayBillID];

                        var items = Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItems>()
                            .Where(item => item.OrderID == order.ID && item.Type == (int)OrderItemType.Normal && item.Status != (int)OrderItemStatus.Deleted);
                        //查询出订单项数据
                        var orderitems = from item in items
                                         join product in new ProductsTopView<PvWsOrderReponsitory>(Reponsitory) on item.ProductID equals product.ID
                                         where item.OrderID == order.ID
                                         select new OrderItem
                                         {
                                             ID = item.ID,
                                             OrderID = item.OrderID,
                                             InputID = item.InputID,
                                             ProductID = item.ProductID,
                                             TinyOrderID = item.TinyOrderID,
                                             Origin = item.Origin,
                                             DateCode = item.DateCode,
                                             Quantity = item.Quantity,
                                             Currency = (Currency)item.Currency,
                                             UnitPrice = item.UnitPrice,
                                             Unit = (LegalUnit)item.Unit,
                                             TotalPrice = item.TotalPrice,
                                             CreateDate = item.CreateDate,
                                             ModifyDate = item.ModifyDate,
                                             GrossWeight = item.GrossWeight,
                                             Volume = item.Volume,
                                             Conditions = item.Conditions,
                                             Status = (OrderItemStatus)item.Status,
                                             IsAuto = item.IsAuto,
                                             WayBillID = item.WayBillID,
                                             Product = product,
                                         };

                        #endregion

                        #region 拼凑入库通知对象
                        var noticeSource = type == OrderType.Recieved ? CgNoticeSource.AgentEnter : CgNoticeSource.Transfer;
                        var Notices = orderitems.Select(item => new
                        {
                            Type = CgNoticeType.Enter,
                            WareHouseID = Yahv.Services.WhSettings.HK["HK01"].ID,//代仓储默认香港万路通库房
                            WaybillID = waybill.ID,
                            item.InputID,
                            item.OutputID,
                            item.CustomName,
                            item.DateCode,
                            item.Origin,
                            item.Quantity,
                            Source = noticeSource,
                            Target = NoticesTarget.Default,
                            Weight = item.GrossWeight,
                            item.Volume,
                            Conditions = new
                            {
                                DevanningCheck = waybill.WayCondition.UnBoxed,//是否拆箱验货 ????
                                Weigh = item.OrderItemCondition.IsWeighing,//是否称重
                                CheckNumber = item.OrderItemCondition.CheckNumber,//是否点数
                                OnlineDetection = item.OrderItemCondition.IsDetection,//是否上机检测
                                AttachLabel = item.OrderItemCondition.IsCustomLabel,//是否贴标签
                                PaintLabel = item.OrderItemCondition.IsDaubLotCode,//是否涂抹标签
                                Repacking = item.OrderItemCondition.IsRepackaging,//是否重新标签
                            }.Json(),
                            Product = new
                            {
                                item.Product.PartNumber,
                                item.Product.Manufacturer,
                                item.Product.PackageCase,
                                item.Product.Packaging,
                            },
                            Input = new
                            {
                                ID = item.InputID,
                                Code = item.InputID,
                                item.OrderID,
                                item.TinyOrderID,
                                ItemID = item.ID,
                                ClientID = order.ClientID,
                                PayeeID = order.PayeeID,
                                //ThirdID = Yahv.Services.WhSettings.HK["HK01"].Enterprise.ID,//代仓储默认香港万路通公司
                                Currency = item.Currency,
                                UnitPrice = item.UnitPrice,
                            },
                        }).ToArray();

                        if(type == OrderType.Transport)
                        {
                            //获取订单特殊要求
                            var Requirements = Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderRequirements>().Where(item => item.OrderID == order.ID);
                            //获取收款申请
                            var receive = (from apply in Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.Applications>()
                                           join item in Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.ApplicationItems>()
                                           on apply.ID equals item.ApplicationID
                                           where item.OrderID == order.ID && apply.Type == (int)ApplicationType.Receival
                                           select apply).FirstOrDefault();
                            var Waybill = new
                            {
                                WaybillID = waybill.ID,
                                Supplier = waybill.Supplier,
                                Type = waybill.Type,
                                Requirement = Requirements.Select(item => new {
                                    item.ID,
                                    item.OrderID,
                                    item.Type,
                                    item.Name,
                                    item.Quantity,
                                    item.UnitPrice,
                                    item.TotalPrice,
                                    item.Requirement,
                                }).ToArray(),
                                CheckRequirement = new
                                {
                                    order.Input.IsPayCharge,
                                    //order.Output.IsReciveCharge,
                                    ApplicationID = receive?.ID,
                                    DelivaryOpportunity = receive?.DelivaryOpportunity,
                                }
                            };
                            var data = new { Waybill, Enter = new { Notices } };
                            //调用库房接口
                            var apisetting = new PvWmsApiSetting();
                            var apiurl = ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.CgNoticeEnter;
                            var result = Yahv.Utils.Http.ApiHelper.Current.JPost(apiurl, data);
                            //返回日志
                            this.Logger("入库通知结果:" + result, order.ID);
                        }
                        else
                        {
                            var Waybill = new
                            {
                                WaybillID = waybill.ID,
                                Supplier = waybill.Supplier,
                                Type = waybill.Type,
                                CheckRequirement = new
                                {
                                    order.Input.IsPayCharge,
                                }
                            };
                            var data = new { Waybill, Enter = new { Notices } };
                            //调用库房接口
                            var apisetting = new PvWmsApiSetting();
                            var apiurl = ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.CgNoticeEnter;
                            var result = Yahv.Utils.Http.ApiHelper.Current.JPost(apiurl, data);
                            //返回日志
                            this.Logger("入库通知结果:" + result, order.ID);
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        //报错数据,该归类状态,记错误日志
                        this.Logger("生成入库通知报错信息:" + ex.Message, order.ID);
                        continue;
                    }
                }
            }
        }

        #endregion

        #region 调用接口获取归类数据

        /// <summary>
        /// 获取归类数据
        /// </summary>
        /// <param name="item"></param>
        private void AutoClassify(OrderItem[] items, PvWsOrderReponsitory Reponsitory)
        {
            //获取归类接口地址
            var apisetting = new PvDataApiSetting();
            var apiurl = ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.AutoClassify;

            foreach (var item in items)
            {
                try
                {
                    var product = Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.ProductsTopView>().SingleOrDefault(a => a.ID == item.ProductID);
                    //调用Api接口自动归类
                    var result = Yahv.Utils.Http.ApiHelper.Current.JPost<JSingle<dynamic>>(apiurl, new
                    {
                        product.PartNumber,
                        product.Manufacturer,
                        item.UnitPrice,
                        item.Origin,
                    });
                    //有归类数据
                    if (result.code == 200)
                    {
                        ItemChcdEnter(result.data, item, Reponsitory);
                        ItemTermEnter(result.data, item, Reponsitory);
                        //更新订单项归类状态,完成自动归类
                        Reponsitory.Update<Layers.Data.Sqls.PvWsOrder.OrderItems>(new
                        {
                            ModifyDate = DateTime.Now,
                            IsAuto = true,
                        }, orderitem => orderitem.ID == item.ID);
                    }
                }
                catch (Exception ex)
                {
                    //报错数据,该归类状态,记错误日志
                    this.Logger(ex.Message, item.ID);
                    Reponsitory.Update<Layers.Data.Sqls.PvWsOrder.OrderItems>(new
                    {
                        ModifyDate = DateTime.Now,
                        IsAuto = true,
                    }, orderitem => orderitem.ID == item.ID);
                    continue;
                }
            }

            //提交
            Reponsitory.Submit();
        }

        /// <summary>
        /// 自动归类数据持久化
        /// </summary>
        /// <param name="Classifydata"></param>
        private void ItemChcdEnter(dynamic Classifydata, OrderItem item, PvWsOrderReponsitory reponsitory)
        {
            //自动归类新需求：如果该产品的自动归类结果包含特殊类型，则自动归类后到预处理二，由报关员在归类界面最终确认是否属于特殊类型；
            //             如果该产品的自动归类结果为普通型号，自动归类后直接到归类完成，报关员无需再做界面归类确认。
            bool isSpecialType = Classifydata.IsSpecialType;
            var itemsChcd = reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItemsChcd>().SingleOrDefault(a => a.ID == item.ID);
            if (itemsChcd == null)
            {
                if (isSpecialType)
                {
                    reponsitory.Insert(new Layers.Data.Sqls.PvWsOrder.OrderItemsChcd()
                    {
                        ID = item.ID,
                        AutoHSCodeID = Classifydata.AutoHSCodeID,
                        AutoDate = DateTime.Now,
                        FirstAdminID = "NPC",
                        FirstHSCodeID = (string)Classifydata.AutoHSCodeID,
                        FirstDate = DateTime.Now,
                        CreateDate = DateTime.Now,
                        ModifyDate = DateTime.Now,
                    });
                }
                else
                {
                    reponsitory.Insert(new Layers.Data.Sqls.PvWsOrder.OrderItemsChcd()
                    {
                        ID = item.ID,
                        AutoHSCodeID = Classifydata.AutoHSCodeID,
                        AutoDate = DateTime.Now,
                        SecondAdminID = "NPC",
                        SecondHSCodeID = (string)Classifydata.AutoHSCodeID,
                        SecondDate = DateTime.Now,
                        CreateDate = DateTime.Now,
                        ModifyDate = DateTime.Now,
                    });
                }
            }
            else
            {
                if (isSpecialType)
                {
                    reponsitory.Update<Layers.Data.Sqls.PvWsOrder.OrderItemsChcd>(new
                    {
                        AutoHSCodeID = (string)Classifydata.AutoHSCodeID,
                        AutoDate = DateTime.Now,
                        FirstAdminID = "NPC",
                        FirstHSCodeID = (string)Classifydata.AutoHSCodeID,
                        FirstDate = DateTime.Now,
                        ModifyDate = DateTime.Now,
                    }, a => a.ID == item.ID);
                }
                else
                {
                    reponsitory.Update<Layers.Data.Sqls.PvWsOrder.OrderItemsChcd>(new
                    {
                        AutoHSCodeID = (string)Classifydata.AutoHSCodeID,
                        AutoDate = DateTime.Now,
                        SecondAdminID = "NPC",
                        SecondHSCodeID = (string)Classifydata.AutoHSCodeID,
                        SecondDate = DateTime.Now,
                        ModifyDate = DateTime.Now,
                    }, a => a.ID == item.ID);
                }
                //原来的数据插入历史日志表
                reponsitory.Insert(new Layers.Data.Sqls.PvWsOrder.Logs_OrderItemsChcd
                {
                    ID = Guid.NewGuid().ToString(),
                    OrderItemID = itemsChcd.ID,
                    AutoHSCodeID = itemsChcd.AutoHSCodeID,
                    AutoDate = itemsChcd.AutoDate,
                    FirstAdminID = itemsChcd.FirstAdminID,
                    FirstHSCodeID = itemsChcd.FirstHSCodeID,
                    FirstDate = itemsChcd.FirstDate,
                    SecondAdminID = itemsChcd.SecondAdminID,
                    SecondHSCodeID = itemsChcd.SecondHSCodeID,
                    SecondDate = itemsChcd.SecondDate,
                    CreateDate = itemsChcd.CreateDate,
                    ModifyDate = itemsChcd.ModifyDate,
                });
            }
        }


        /// <summary>
        /// 报关特殊要求持久化
        /// </summary>
        private void ItemTermEnter(dynamic Classifydata, OrderItem item, PvWsOrderReponsitory reponsitory)
        {
            var itemsTerm = reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItemsTerm>().SingleOrDefault(a => a.ID == item.ID);
            if (itemsTerm == null)
            {
                reponsitory.Insert(new Layers.Data.Sqls.PvWsOrder.OrderItemsTerm()
                {
                    ID = item.ID,
                    OriginRate = Classifydata.OriginRate,
                    FVARate = Classifydata.FVARate,
                    Ccc = Classifydata.Ccc,
                    Embargo = Classifydata.Embargo,
                    HkControl = Classifydata.HkControl,
                    Coo = Classifydata.Coo,
                    CIQ = Classifydata.CIQ,
                    CIQprice = Classifydata.CIQprice,
                    IsHighPrice = Classifydata.IsHighPrice,
                    IsDisinfected = Classifydata.IsDisinfected,
                });
            }
            else
            {
                //更新表数据
                reponsitory.Update<Layers.Data.Sqls.PvWsOrder.OrderItemsTerm>(new
                {
                    OriginRate = (decimal)Classifydata.OriginRate,
                    FVARate = (decimal)Classifydata.FVARate,
                    Ccc = (bool)Classifydata.Ccc,
                    Embargo = (bool)Classifydata.Embargo,
                    HkControl = (bool)Classifydata.HkControl,
                    Coo = (bool)Classifydata.Coo,
                    CIQ = (bool)Classifydata.CIQ,
                    CIQprice = (decimal)Classifydata.CIQprice,
                    IsHighPrice = (bool)Classifydata.IsHighPrice,
                    IsDisinfected = (bool)Classifydata.IsDisinfected,
                }, a => a.ID == item.ID);

                //原来的数据插入历史日志表
                reponsitory.Insert(new Layers.Data.Sqls.PvWsOrder.Logs_OrderItemsTerm
                {
                    ID = Guid.NewGuid().ToString(),
                    OrderItemID = itemsTerm.ID,
                    OriginRate = itemsTerm.OriginRate,
                    FVARate = itemsTerm.FVARate,
                    Ccc = itemsTerm.Ccc,
                    Embargo = itemsTerm.Embargo,
                    HkControl = itemsTerm.HkControl,
                    Coo = itemsTerm.Coo,
                    CIQ = itemsTerm.CIQ,
                    CIQprice = itemsTerm.CIQprice,
                    IsHighPrice = itemsTerm.IsHighPrice,
                    IsDisinfected = itemsTerm.IsDisinfected,
                });
            }
        }

        #endregion

        #region 错误日志记录
        /// <summary>
        /// 记录错误日志
        /// </summary>
        /// <param name="ErrorMessage"></param>
        private void Logger(string ErrorMessage, string ID)
        {
            var logger = new Logger();
            logger[LogType.Error].Log(new OperatingLog
            {
                MainID = ID,
                Operation = ErrorMessage,
                Summary = "自动归类服务",
            });
        }

        #endregion 

    }


    /// <summary>
    /// 归类服务错误日志类
    /// </summary>
    public class Logger : Yahv.Services.OperatingLogger
    {
        private static string AdminID = "NPC";

        internal Logger() : base(AdminID)
        {

        }
    }
}
