using Needs.Ccs.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public static class PD_ClassifyProductExtends
    {
        /// <summary>
        /// 更新预处理一操作人
        /// </summary>
        /// <param name="classifyProduct"></param>
        /// <param name="reponsitory"></param>
        public static void UpdateFirstOperator(this PD_ClassifyProduct classifyProduct, Layer.Data.Sqls.ScCustomsReponsitory reponsitory)
        {
            reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderItemCategories>(new
            {
                ClassifyFirstOperator = classifyProduct.Admin.ID,
            }, item => item.ID == classifyProduct.Category.ID);
        }

        /// <summary>
        /// 更新预处理二操作人
        /// </summary>
        /// <param name="classifyProduct"></param>
        /// <param name="reponsitory"></param>
        public static void UpdateSecondOperator(this PD_ClassifyProduct classifyProduct, Layer.Data.Sqls.ScCustomsReponsitory reponsitory)
        {
            reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderItemCategories>(new
            {
                ClassifySecondOperator = classifyProduct.Admin.ID,
            }, item => item.ID == classifyProduct.Category.ID);
        }

        /// <summary>
        /// 更新归类状态
        /// </summary>
        /// <param name="classifyProduct">归类产品</param>
        /// <param name="status">归类状态</param>
        /// <param name="reponsitory">数据库连接</param>
        public static void UpdateClassifyStatus(this PD_ClassifyProduct classifyProduct, Enums.ClassifyStatus status, Layer.Data.Sqls.ScCustomsReponsitory reponsitory)
        {
            reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderItems>(new
            {
                ClassifyStatus = (int)status
            }, item => item.ID == classifyProduct.ID);
        }

        /// <summary>
        /// 同步订单特殊类型
        /// </summary>
        /// <param name="classifyProduct"></param>
        /// <param name="reponsitory"></param>
        public static void SetOrderVoyage(this PD_ClassifyProduct classifyProduct, Layer.Data.Sqls.ScCustomsReponsitory reponsitory)
        {
            var orderID = classifyProduct.OrderID;

            //该订单的型号归类完成后，判断该订单的特殊类型 高价值、商检、检疫、CCC Begin

            OrderItem[] orderItems = new Order() { ID = orderID, }.Items.ToArray();
            OrderItem[] orderItemsHasCategory = orderItems.Where(t => t.Category != null).ToArray();
            Dictionary<OrderSpecialType, ItemCategoryType> dicCheckOrderSpecialType = new Dictionary<OrderSpecialType, ItemCategoryType>();
            dicCheckOrderSpecialType.Add(OrderSpecialType.HighValue, ItemCategoryType.HighValue);
            dicCheckOrderSpecialType.Add(OrderSpecialType.Inspection, ItemCategoryType.Inspection);
            dicCheckOrderSpecialType.Add(OrderSpecialType.Quarantine, ItemCategoryType.Quarantine);
            dicCheckOrderSpecialType.Add(OrderSpecialType.CCC, ItemCategoryType.CCC);

            foreach (var dic in dicCheckOrderSpecialType)
            {
                if (orderItemsHasCategory != null && orderItemsHasCategory.Any())
                {
                    bool isTheType = orderItemsHasCategory.Any(t => (t.Category.Type.GetHashCode() & dic.Value.GetHashCode()) > 0);
                    var orderVoyage = new Needs.Ccs.Services.Models.OrderVoyage();
                    orderVoyage.Order = new Order() { ID = orderID, };
                    orderVoyage.Type = dic.Key;
                    if (isTheType)
                    {
                        orderVoyage.Enter();
                    }
                    else
                    {
                        orderVoyage.Abandon();
                    }
                }
            }

            //该订单的型号归类完成后，判断该订单的特殊类型 高价值、商检、检疫、CCC End
        }

        /// <summary>
        /// 更新订单信息
        /// </summary>
        /// <param name="classifyProduct"></param>
        /// <param name="reponsitory"></param>
        public static void UpdateOrder(this PD_ClassifyProduct classifyProduct, Layer.Data.Sqls.ScCustomsReponsitory reponsitory)
        {
            var orderID = classifyProduct.OrderID;
            var declarant = classifyProduct.Admin;

            var order = new Views.OrdersView(reponsitory)[orderID];
            //如果订单已经报价，不需要再做后续处理
            if (order.OrderStatus >= OrderStatus.Quoted)
                return;

            //未完成归类的产品数量
            int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>().Count(item => item.OrderID == orderID &&
                    (item.ClassifyStatus == (int)Enums.ClassifyStatus.Unclassified || item.ClassifyStatus == (int)Enums.ClassifyStatus.First) &&
                    item.Status == (int)Enums.Status.Normal);

            if (count == 0)
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new
                {
                    OrderStatus = (int)Enums.OrderStatus.Classified
                }, item => item.ID == orderID);
               
                if (declarant != null)
                {
                    order?.Log(declarant, "报关员【" + declarant.RealName + "】完成了订单归类，等待跟单员报价");                  
                    order.Trace(declarant, Enums.OrderTraceStep.Processing, "您的订单产品已归类完成");                     
                }

                order.SendNotice(SendNoticeType.ClassifyDone);
                //Icgoo和大赢家订单归类完成，系统自动完成报价和确认报价，进入待报关状态
                if (classifyProduct.OrderType != Enums.OrderType.Outside)
                {
                    var classifiedOrder = new Views.ClassifiedInsideOrdersView(reponsitory)[orderID];
                    classifiedOrder?.SetAdmin(classifiedOrder.Client.Merchandiser);
                    classifiedOrder?.GenerateBill();
                    classifiedOrder?.Quote();

                    var quotedOrder = new Views.QuotedInsideOrdersView(reponsitory)[orderID];
                    quotedOrder?.IcgooQuoteConfirm();
                    quotedOrder?.ToReceivables();
                }               
            }
        }

        /// <summary>
        /// 进行产品管控
        /// </summary>
        /// <param name="classifyProduct"></param>
        /// <param name="reponsitory"></param>
        public static void DoProductControl(this PD_ClassifyProduct classifyProduct, Layer.Data.Sqls.ScCustomsReponsitory reponsitory)
        {
            if (classifyProduct.IsCCC)
            {
                classifyProduct.OrderHangUp(Enums.OrderControlType.CCC);
            }
            else if (classifyProduct.IsSysCCC)
            {
                classifyProduct.OrderHangUp(Enums.OrderControlType.CCC, Enums.OrderControlStep.Headquarters);
            }

            if (classifyProduct.IsOriginProof)
            {
                classifyProduct.OrderHangUp(Enums.OrderControlType.OriginCertificate);
            }

            if (classifyProduct.IsSysForbid || classifyProduct.IsForbid)
            {
                classifyProduct.OrderHangUp(Enums.OrderControlType.Forbid, Enums.OrderControlStep.Headquarters);
            }
        }

        /// <summary>
        /// 更新产品变更通知
        /// </summary>
        /// <param name="classifyProduct"></param>
        /// <param name="reponsitory"></param>
        public static void UpdateOrderItemChangeNotice(this PD_ClassifyProduct classifyProduct, Layer.Data.Sqls.ScCustomsReponsitory reponsitory)
        {
            var changeNotice = new Needs.Ccs.Services.Views.OrderItemChangeNoticesView(reponsitory).GetTop(100, i => i.OrderItemID == classifyProduct.ID);
            foreach (var cn in changeNotice)
            {
                cn.ProcessState = Enums.ProcessState.Processed;
                cn.Enter();
            }
        }

        /// <summary>
        /// 获取产品变更日志
        /// </summary>
        /// <returns></returns>
        public static OrderItemChangeLog[] GetChangeLogs(this PD_ClassifyProduct classifyProduct)
        {
            return new Views.Origins.OrderItemChangeLogsOrigin().Where(t => t.OrderItemID == classifyProduct.ID)
                .OrderByDescending(t => t.CreateDate).ToArray();
        }
    }
}
