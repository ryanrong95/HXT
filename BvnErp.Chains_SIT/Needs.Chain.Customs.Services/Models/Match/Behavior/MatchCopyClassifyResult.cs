using Needs.Ccs.Services.Enums;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
   public class MatchCopyClassifyResult
    {
        public List<Models.OrderItemAssitant> OrderItems { get; private set; }
        public Order OriginOrder { get; set; }
        public string CurrentOrderID { get; set; }
        public MatchCopyClassifyResult(List<Models.OrderItemAssitant> orderItems, Order originOrder,string currentOrderID)
        {
            this.OrderItems = orderItems;
            this.OriginOrder = originOrder;
            this.CurrentOrderID = currentOrderID;
        }

        public void Copy()
        {
            var orderItems = this.OrderItems.Where(t => t.ChangeType != Enums.MatchChangeType.OrderChange && t.PersistenceType == PersistenceType.Insert).ToList();
            foreach (var t in orderItems)
            {
                try
                {
                    var sourceClassifyResult = OriginOrder.Items.Where(m => m.ID == t.MatchedOrderItemID).FirstOrDefault();

                    #region OrderItemCategory
                    OrderItemCategory category = new OrderItemCategory();
                    category.ID = string.Concat(t.ID).MD5();
                    category.OrderItemID = t.ID;
                    category.TaxCode = sourceClassifyResult.Category.TaxCode;
                    category.TaxName = sourceClassifyResult.Category.TaxName;
                    category.HSCode = sourceClassifyResult.Category.HSCode;
                    category.Name = sourceClassifyResult.Category.Name;
                    category.Elements = sourceClassifyResult.Category.Elements;
                    category.Unit1 = sourceClassifyResult.Category.Unit1;
                    category.Unit2 = sourceClassifyResult.Category.Unit2;
                    category.CIQCode = sourceClassifyResult.Category.CIQCode;
                    category.ClassifyFirstOperatorID = Icgoo.DefaultCreator;
                    category.ClassifySecondOperatorID = Icgoo.DefaultCreator;
                    category.Status = Enums.Status.Normal;
                    category.CreateDate = sourceClassifyResult.Category.CreateDate;
                    category.UpdateDate = sourceClassifyResult.Category.UpdateDate;

                    category.Type = sourceClassifyResult.Category.Type;

                    category.Enter();
                    #endregion

                    #region
                    //5.添加订单项关税、增值税、消费税记录
                    OrderItemTax itemTax = new OrderItemTax();
                    itemTax.ID = string.Concat(t.ID, CustomsRateType.ImportTax).MD5();
                    itemTax.OrderItemID = t.ID;
                    itemTax.Type = CustomsRateType.ImportTax;
                    itemTax.Rate = sourceClassifyResult.ImportTax.Rate;
                    itemTax.ReceiptRate = sourceClassifyResult.ImportTax.ReceiptRate;
                    itemTax.Value = Math.Round(t.TotalPrice * itemTax.Rate, 2, MidpointRounding.AwayFromZero);
                    itemTax.Status = Status.Normal;
                    itemTax.CreateDate = DateTime.Now;
                    itemTax.UpdateDate = DateTime.Now;
                    itemTax.Enter();

                    OrderItemTax itemAddedValue = new OrderItemTax();
                    itemAddedValue.ID = string.Concat(t.ID, CustomsRateType.AddedValueTax).MD5();
                    itemAddedValue.OrderItemID = t.ID;
                    itemAddedValue.Type = CustomsRateType.AddedValueTax;
                    itemAddedValue.Rate = sourceClassifyResult.AddedValueTax.Rate;
                    itemAddedValue.ReceiptRate = sourceClassifyResult.AddedValueTax.ReceiptRate;
                    itemAddedValue.Value = Math.Round(t.TotalPrice * itemAddedValue.Rate, 2, MidpointRounding.AwayFromZero);
                    itemAddedValue.Status = Status.Normal;
                    itemAddedValue.CreateDate = DateTime.Now;
                    itemAddedValue.UpdateDate = DateTime.Now;
                    itemAddedValue.Enter();

                    if (sourceClassifyResult.ExciseTax != null)
                    {
                        OrderItemTax exciseTax = new OrderItemTax();
                        exciseTax.ID = string.Concat(t.ID, CustomsRateType.ConsumeTax).MD5();
                        exciseTax.OrderItemID = t.ID;
                        exciseTax.Type = CustomsRateType.ConsumeTax;
                        exciseTax.Rate = sourceClassifyResult.ExciseTax.Rate;
                        exciseTax.ReceiptRate = sourceClassifyResult.ExciseTax.ReceiptRate;
                        exciseTax.Value = Math.Round(t.TotalPrice * exciseTax.Rate, 2, MidpointRounding.AwayFromZero);
                        exciseTax.Status = Status.Normal;
                        exciseTax.CreateDate = DateTime.Now;
                        exciseTax.UpdateDate = DateTime.Now;
                        exciseTax.Enter();
                    }
                    #endregion

                    #region 商检
                    //6.商检
                    if ((category.Type & ItemCategoryType.Inspection) > 0)
                    {
                        var InspectionFee = OriginOrder.Premiums.Where(m => m.Type == OrderPremiumType.InspectionFee && m.OrderItemID == t.ID).FirstOrDefault();
                        if (InspectionFee != null)
                        {
                            var premium = new OrderPremium
                            {
                                ID = Needs.Overall.PKeySigner.Pick(PKeyType.OrderPremium),
                                OrderID = this.CurrentOrderID,
                                OrderItemID = t.ID,
                                Type = Enums.OrderPremiumType.InspectionFee,
                                Count = 1,
                                UnitPrice = InspectionFee.UnitPrice,
                                Currency = MultiEnumUtils.ToCode<Enums.Currency>(Enums.Currency.CNY),
                                Rate = 1,
                                Admin = new Admin
                                {
                                    ID = Icgoo.DefaultCreator
                                }
                            };
                            premium.Enter();
                        }
                    }
                    #endregion

                    t.ClassifyStatus = ClassifyStatus.Done;
                }
                catch (Exception ex)
                {
                    continue;
                }
            }

            UpdataOrderItemClassifyStatus();
        }


        private void UpdataOrderItemClassifyStatus()
        {
            var items = this.OrderItems.Where(t => t.ClassifyStatus == Enums.ClassifyStatus.Done);
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                foreach (var item in items)
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderItems>(new
                    {
                        ClassifyStatus = (int)Enums.ClassifyStatus.Done
                    }, t => t.ID == item.ID);
                }
            }
        }
    }
}
