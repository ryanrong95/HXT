using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// DecList 触发产品变更
    /// </summary>
    public class DecListTriggerProductChange
    {
        private Layer.Data.Sqls.ScCustomsReponsitory Reponsitory { get; set; }

        private Admin Admin { get; set; }

        private string OrderID { get; set; } = string.Empty;

        private string OrderItemID { get; set; } = string.Empty;

        private Models.DecList OldDecList { get; set; }

        private Models.DecList NewDecList { get; set; }

        public DecListTriggerProductChange(Admin admin, string orderID, string orderItemID,
            Models.DecList oldDecList, Models.DecList newDecList)
        {
            this.Reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory();
            this.Admin = admin;
            this.OrderID = orderID;
            this.OrderItemID = orderItemID;
            this.OldDecList = oldDecList;
            this.NewDecList = newDecList;
        }

        public DecListTriggerProductChange(Layer.Data.Sqls.ScCustomsReponsitory reponsitory, Admin admin, string orderID, string orderItemID,
            Models.DecList oldDecList, Models.DecList newDecList)
        {
            this.Reponsitory = reponsitory;
            this.Admin = admin;
            this.OrderID = orderID;
            this.OrderItemID = orderItemID;
            this.OldDecList = oldDecList;
            this.NewDecList = newDecList;
        }

        /// <summary>
        /// 执行触发
        /// </summary>
        public void DoTrigger()
        {
            Models.OrderItem orderItem = new Models.OrderItem()
            {
                ID = this.OrderItemID,
                OrderID = this.OrderID,
                Model = this.NewDecList.GoodsModel,
                Origin = this.NewDecList.OriginCountry,
                Manufacturer = this.NewDecList.GoodsBrand,
            };

            Models.OrderItemCategory orderItemCategory = new Models.OrderItemCategory()
            {
                ID = string.Concat(this.OrderItemID).MD5(),
                OrderItemID = this.OrderItemID,
                HSCode = this.NewDecList.CodeTS,
                Name = this.NewDecList.GName,
            };

            //1. 型号
            if (this.OldDecList.GoodsModel != this.NewDecList.GoodsModel)
            {
                this.UpdateOrderItem(Enums.OrderItemChangeType.ProductModelChange, orderItem);
            }

            //2. 产地
            if (this.OldDecList.OriginCountry != this.NewDecList.OriginCountry)
            {
                this.UpdateOrderItem(Enums.OrderItemChangeType.OriginChange, orderItem);
            }

            //3. 品牌
            if (this.OldDecList.GoodsBrand != this.NewDecList.GoodsBrand)
            {
                this.UpdateOrderItem(Enums.OrderItemChangeType.BrandChange, orderItem);
            }

            //4. 商品编码（海关编码）
            if (this.OldDecList.CodeTS != this.NewDecList.CodeTS)
            {
                this.UpdateOrderItemCategory(Enums.OrderItemChangeType.HSCodeChange, orderItemCategory);
            }

            //5. 商品名称（报关品名）
            if (this.OldDecList.GName != this.NewDecList.GName)
            {
                this.UpdateOrderItemCategory(Enums.OrderItemChangeType.TariffNameChange, orderItemCategory);
            }
        }

        #region 备选事件 可执行 A、B、E

        /// <summary>
        /// A 修改 OrderItem
        /// </summary>
        private void UpdateOrderItem(Enums.OrderItemChangeType orderItemChangeType, Models.OrderItem newOrderItemInfo)
        {
            var orderItemModel = (from orderItem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>()
                                  where orderItem.ID == newOrderItemInfo.ID
                                  select new Models.OrderItem()
                                  {
                                      ID = orderItem.ID,
                                      Model = orderItem.Model,
                                      Origin = orderItem.Origin,
                                      Manufacturer = orderItem.Manufacturer,
                                  }).FirstOrDefault();

            string summary = string.Empty;

            switch (orderItemChangeType)
            {
                case Enums.OrderItemChangeType.OriginChange:
                    summary = "报关员[" + this.Admin.ByName + "]做了产地变更操作，从[" + orderItemModel.Origin + "]变为[" + newOrderItemInfo.Origin + "]";
                    orderItemModel.Origin = newOrderItemInfo.Origin;
                    break;
                case Enums.OrderItemChangeType.BrandChange:
                    summary = "报关员[" + this.Admin.ByName + "]做了品牌变更操作，从[" + orderItemModel.Manufacturer + "]变为[" + newOrderItemInfo.Manufacturer + "]";
                    orderItemModel.Manufacturer = newOrderItemInfo.Manufacturer;
                    break;
                case Enums.OrderItemChangeType.ProductModelChange:
                    summary = "报关员[" + this.Admin.ByName + "]做了型号变更操作，从[" + orderItemModel.Model + "]变为[" + newOrderItemInfo.Model + "]";
                    orderItemModel.Model = newOrderItemInfo.Model;
                    break;
                default:
                    break;
            }

            this.Reponsitory.Update(new Layer.Data.Sqls.ScCustoms.OrderItems
            {
                Model = orderItemModel.Model,
                Origin = orderItemModel.Origin,
                Manufacturer = orderItemModel.Manufacturer,
            }, item => item.ID == orderItemModel.ID);

            if (!string.IsNullOrEmpty(summary))
            {
                this.OrderItemLog(orderItemChangeType, summary);
            }
        }

        /// <summary>
        /// B 修改 OrderItemCategory
        /// </summary>
        private void UpdateOrderItemCategory(Enums.OrderItemChangeType orderItemChangeType, Models.OrderItemCategory newOrderItemCategoryInfo)
        {
            var orderItemCategoryModel = (from OrderItemCategory in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemCategories>()
                                          where OrderItemCategory.ID == newOrderItemCategoryInfo.ID
                                          select new Models.OrderItemCategory()
                                          {
                                              ID = OrderItemCategory.ID,
                                              HSCode = OrderItemCategory.HSCode,
                                              Name = OrderItemCategory.Name,
                                          }).FirstOrDefault();

            string summary = string.Empty;

            switch (orderItemChangeType)
            {
                case Enums.OrderItemChangeType.HSCodeChange:
                    summary = "报关员[" + this.Admin.ByName + "]做了海关编码变更操作，从[" + orderItemCategoryModel.HSCode + "]变为[" + newOrderItemCategoryInfo.HSCode + "]";
                    orderItemCategoryModel.HSCode = newOrderItemCategoryInfo.HSCode;
                    break;
                case Enums.OrderItemChangeType.TariffNameChange:
                    summary = "报关员[" + this.Admin.ByName + "]做了报关品名变更操作，从[" + orderItemCategoryModel.Name + "]变为[" + newOrderItemCategoryInfo.Name + "]";
                    orderItemCategoryModel.Name = newOrderItemCategoryInfo.Name;
                    break;
                default:
                    break;
            }

            this.Reponsitory.Update(new Layer.Data.Sqls.ScCustoms.OrderItemCategories
            {
                HSCode = orderItemCategoryModel.HSCode,
                Name = orderItemCategoryModel.Name,
            }, item => item.ID == orderItemCategoryModel.ID);

            if (!string.IsNullOrEmpty(summary))
            {
                this.OrderItemLog(orderItemChangeType, summary);
            }
        }

        /// <summary>
        /// D OrderItem 日志
        /// 【放入到 A、B 中执行】
        /// </summary>
        private void OrderItemLog(Enums.OrderItemChangeType type, string summary)
        {
            OrderItemChangeLog log = new OrderItemChangeLog()
            {
                OrderItemID = this.OrderItemID,
                OrderID = this.OrderID,
                Type = type,
                Admin = this.Admin,
                Summary = summary,
            };
            log.Enter();
        }

        #endregion 备选事件
    }
}
