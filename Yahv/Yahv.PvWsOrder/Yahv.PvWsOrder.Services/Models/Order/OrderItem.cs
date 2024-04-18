using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.PvWsOrder.Services.Extends;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Usually;
using Yahv.Utils.Serializers;

namespace Yahv.PvWsOrder.Services.Models
{
    /// <summary>
    /// 订单项
    /// </summary>
    public partial class OrderItem : OrderItemOrigin
    {
        public OrderItem()
        {
            this.CreateDate = this.ModifyDate = DateTime.Now;
            this.Status = OrderItemStatus.Normal;
            this.Type = OrderItemType.Normal;

            this.EnterSuccess += OrderItem_EnterSuccess;
        }

        #region 扩展属性

        /// <summary>
        /// 产品对象
        /// </summary>
        public CenterProduct Product { get; set; }

        public OrderItemCondition OrderItemCondition
        {
            get
            {
                return this.Conditions.JsonTo<OrderItemCondition>();
            }
        }

        /// <summary>
        /// 归类信息
        /// </summary>
        public OrderItemsChcd OrderItemsChcd { get; set; }

        /// <summary>
        /// 归类信息
        /// </summary>
        public OrderItemsTerm OrderItemsTerm { get; set; }

        /// <summary>
        /// 产品归类结果
        /// </summary>
        public string Terms
        {
            get
            {
                if (this.OrderItemsTerm == null)
                {
                    return "";
                }
                StringBuilder terms = new StringBuilder();
                if (OrderItemsTerm.Ccc)
                {
                    terms.Append("3C/");
                }
                if (OrderItemsTerm.CIQ)
                {
                    terms.Append("商检/");
                }
                if (OrderItemsTerm.Embargo)
                {
                    terms.Append("禁运/");
                }
                if (OrderItemsTerm.HkControl)
                {
                    terms.Append("香港管控/");
                }
                if (OrderItemsTerm.IsHighPrice)
                {
                    terms.Append("高价值/");
                }
                if (OrderItemsTerm.IsDisinfected)
                {
                    terms.Append("消毒/");
                }
                if (OrderItemsTerm.Coo)
                {
                    terms.Append("原产地证明/");
                }
                return terms.ToString().TrimEnd('/');
            }
        }

        /// <summary>
        /// 原产地描述
        /// </summary>
        public string OriginGetDescription
        {
            get
            {
                return this.Origin.GetDescription();
            }
        }

        /// <summary>
        /// 原产地描述
        /// </summary>
        public string OriginGetCode
        {
            get
            {
                return this.Origin.GetOrigin().Code;
            }
        }

        public string StorageID { get; set; }
        /// <summary>
        /// 物料编号
        /// </summary>
        public string ProductUniqueCode { get; set; }
        #endregion

        #region 事件

        public event SuccessHanlder EnterSuccess;

        public void OnEnterSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }

        private void OrderItem_EnterSuccess(object sender, SuccessEventArgs e)
        {
            var item = (OrderItem)e.Object;
            //自动归类
            //AutoClassify();
        }

        #endregion

        #region 持久化

        public void Enter()
        {
            using (Layers.Data.Sqls.PvWsOrderReponsitory reponsitory = new Layers.Data.Sqls.PvWsOrderReponsitory())
            {
                Yahv.Services.Views.ProductsTopView<Layers.Data.Sqls.PvWsOrderReponsitory>.Enter(this.Product);
                //保存订单项
                var entity = new Views.OrderItemsAlls(reponsitory).Where(item => item.ID == this.ID).FirstOrDefault();
                if (entity == null)
                {
                    this.ID = Layers.Data.PKeySigner.Pick(PKeyType.OrderItem);
                    reponsitory.Insert(this.ToLinq());
                }
                else
                {
                    //判断是否需要更新
                    if (!IsUpdateItem(this, entity))
                    {
                        //添加日志Log_OrderItems
                        reponsitory.Insert(entity.ToLinqLog());
                        //更新数据
                        reponsitory.Update<Layers.Data.Sqls.PvWsOrder.OrderItems>(new
                        {
                            ProductID = this.Product.ID,
                            CustomName = this.CustomName,
                            Origin = this.Origin.GetOrigin().Code,
                            DateCode = this.DateCode,
                            Quantity = this.Quantity,
                            Currency = (int)this.Currency,
                            UnitPrice = this.UnitPrice,
                            Unit = (int)this.Unit,
                            TotalPrice = this.TotalPrice,
                            ModifyDate = DateTime.Now,
                            GrossWeight = this.GrossWeight,
                            Volume = this.Volume,
                            Conditions = this.Conditions,
                            Status = (int)this.Status,
                            IsAuto = this.IsAuto,
                            WayBillID = this.WayBillID,
                        }, item => item.ID == this.ID);
                    }
                }
                //新增事件
                this.OnEnterSuccess();
            }
        }

        public void Abandon()
        {
            using (Layers.Data.Sqls.PvWsOrderReponsitory reponsitory = new Layers.Data.Sqls.PvWsOrderReponsitory())
            {
                reponsitory.Update<Layers.Data.Sqls.PvWsOrder.OrderItems>(new
                {
                    ModifyDate = DateTime.Now,
                    Status = (int)OrderItemStatus.Deleted,
                }, item => item.ID == this.ID);
            }
        }

        #endregion

        #region 其它方法

        /// <summary>
        /// 人工归类
        /// </summary>
        /// <param name="result"></param>
        public void AdminClassify(ClassifiedResult result)
        {
            using (Layers.Data.Sqls.PvWsOrderReponsitory reponsitory = new Layers.Data.Sqls.PvWsOrderReponsitory())
            {
                if (this.Status != OrderItemStatus.Normal)
                {
                    throw new Exception("订单项" + result.ItemID + "状态为删除");
                }
                //判断产品项是否变更
                if (this.ProductID != result.ProductID)
                {
                    #region 更新订单项和日志
                    reponsitory.Insert(this.ToLinqLog());
                    reponsitory.Update<Layers.Data.Sqls.PvWsOrder.OrderItems>(new
                    {
                        ProductID = result.ProductID,
                        ModifyDate = DateTime.Now,
                    }, item => item.ID == this.ID);
                    #endregion                   
                }

                #region 变更Chcd归类信息 
                if (this.OrderItemsChcd == null)
                {
                    this.OrderItemsChcd = new OrderItemsChcd();
                }
                this.OrderItemsChcd.ID = this.ID;
                this.OrderItemsChcd.ModifyDate = DateTime.Now;
                if (int.Parse(result.Step) == (int)ClassifyStep.Step1)
                {
                    this.OrderItemsChcd.FirstHSCodeID = result.HSCodeID;
                    this.OrderItemsChcd.FirstAdminID = result.CreatorID;
                    this.OrderItemsChcd.FirstDate = DateTime.Now;
                }
                if (int.Parse(result.Step) == (int)ClassifyStep.Step2)
                {
                    this.OrderItemsChcd.SecondHSCodeID = result.HSCodeID;
                    this.OrderItemsChcd.SecondAdminID = result.CreatorID;
                    this.OrderItemsChcd.SecondDate = DateTime.Now;
                }
                this.OrderItemsChcd.Enter();
                #endregion

                #region 变更Term归类信息                            
                if (this.OrderItemsTerm == null)
                {
                    this.OrderItemsTerm = new OrderItemsTerm();
                }
                this.OrderItemsTerm.ID = this.ID;
                this.OrderItemsTerm.OriginRate = result.OriginRate;
                this.OrderItemsTerm.FVARate = result.FVARate;
                this.OrderItemsTerm.Ccc = result.Ccc;
                this.OrderItemsTerm.Embargo = result.Embargo;
                this.OrderItemsTerm.HkControl = result.HkControl;
                this.OrderItemsTerm.Coo = result.Coo;
                this.OrderItemsTerm.CIQ = result.CIQ;
                this.OrderItemsTerm.CIQprice = result.CIQprice;
                this.OrderItemsTerm.IsHighPrice = result.IsHighPrice;
                this.OrderItemsTerm.IsDisinfected = result.IsDisinfected;
                this.OrderItemsTerm.Enter();
                #endregion

                #region 判断是否变更订单状态  
                //TODO:是否等到二次归类后再挂起
                if (this.OrderItemsTerm.Embargo || this.OrderItemsTerm.HkControl)
                {
                    //更新状态日志
                    var order = new Views.OrderAlls().Where(item => item.ID == this.OrderID).FirstOrDefault();
                    order.OperatorID = result.CreatorID;
                    order.MainStatus = CgOrderStatus.挂起;
                    order.StatusLogUpdate();
                }
                #endregion
            }
        }

        /// <summary>
        /// 持久化(异步)
        /// </summary>
        /// <param name="state"></param>
        public void SaveOrderItem(object state)
        {
            using (Layers.Data.Sqls.PvWsOrderReponsitory reponsitory = new Layers.Data.Sqls.PvWsOrderReponsitory())
            {
                reponsitory.Insert(this.ToLinq());
            }
        }

        /// <summary>
        /// 是否需要更新Item
        /// </summary>
        /// <param name="newItem"></param>
        /// <param name="oldItem"></param>
        /// <returns></returns>
        private bool IsUpdateItem(OrderItem newItem, OrderItem oldItem)
        {
            var newEntity = new
            {
                ProductID = newItem.Product.ID,
                CustomName = newItem.CustomName,
                Origin = newItem.Origin,
                DateCode = newItem.DateCode,
                Quantity = newItem.Quantity.ToString("0.0000000"),
                Currency = newItem.Currency,
                UnitPrice = newItem.UnitPrice.ToString("0.0000000"),
                Unit = newItem.Unit,
                TotalPrice = newItem.TotalPrice.ToString("0.0000000"),
                GrossWeight = newItem.GrossWeight?.ToString("0.0000000"),
                Volume = newItem.Volume?.ToString("0.0000000"),
                Conditions = newItem.Conditions,
                Status = newItem.Status,
                IsAuto = newItem.IsAuto,
                WayBillID = newItem.WayBillID,
            };
            var oldEntity = new
            {
                ProductID = oldItem.Product.ID,
                CustomName = oldItem.CustomName,
                Origin = oldItem.Origin,
                DateCode = oldItem.DateCode,
                Quantity = oldItem.Quantity.ToString("0.0000000"),
                Currency = oldItem.Currency,
                UnitPrice = oldItem.UnitPrice.ToString("0.0000000"),
                Unit = oldItem.Unit,
                TotalPrice = oldItem.TotalPrice.ToString("0.0000000"),
                GrossWeight = oldItem.GrossWeight?.ToString("0.0000000"),
                Volume = oldItem.Volume?.ToString("0.0000000"),
                Conditions = oldItem.Conditions,
                Status = oldItem.Status,
                IsAuto = oldItem.IsAuto,
                WayBillID = oldItem.WayBillID,
            };
            if (newEntity == oldEntity)
            {
                return true;
            }
            return false;
        }

        #endregion

    }

    public class OrderItemOrigin : IUnique
    {
        public string ID { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        public string OrderID { get; set; }

        public string TinyOrderID { get; set; }

        /// <summary>
        /// 规则生成:Ipt+yyyyMMdd+count
        /// </summary>
        public string InputID { get; set; }

        /// <summary>
        /// 规则生成:Ipt+yyyyMMdd+count
        /// </summary>
        public string OutputID { get; set; }

        /// <summary>
        /// 产品ID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 海关品名（客户特殊要求）
        /// </summary>
        public string CustomName { get; set; }

        /// <summary>
        /// 原产地
        /// </summary>
        public Origin Origin { get; set; }

        /// <summary>
        /// 批次号
        /// </summary>
        public string DateCode { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public Currency? Currency { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public LegalUnit Unit { get; set; }

        /// <summary>
        /// 总价
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// 毛重
        /// </summary>
        public decimal? GrossWeight { get; set; }

        /// <summary>
        /// 体积
        /// </summary>
        public decimal? Volume { get; set; }

        /// <summary>
        /// 出入库条件
        /// </summary>
        public string Conditions { get; set; }

        public OrderItemStatus Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime ModifyDate { get; set; }

        public bool IsAuto { get; set; }

        public string WayBillID { get; set; }

        public OrderItemType Type { get; set; }
    }
}
