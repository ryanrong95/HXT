using Needs.Utils.Serializers;
using NtErp.Wss.Sales.Services.Model.Orders;
using NtErp.Wss.Sales.Services.Models.Carts;
using NtErp.Wss.Sales.Services.Models.Orders.Commodity;
using NtErp.Wss.Sales.Services.Models.SsoUsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Extends
{
    static public class OrdersExtends
    {
        static internal Layer.Data.Sqls.BvOrders.Orders ToLinq(this Order entity)
        {
            return new Layer.Data.Sqls.BvOrders.Orders
            {
                ID = entity.ID,
                UserID = entity.UserID,
                SiteUserName = entity.SiteUserName,
                CompanyName = entity.CompanyName,
                Currency = (int)entity.Currency,
                District = (int)entity.District,
                Transport = (int)entity.Transport,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                Status = (int)entity.Status,
                DeliveryRatio = entity.DeliveryRatio,
                PaidRatio = entity.PaidRatio,
                Summary = entity.Summary
            };
        }

        static internal Layer.Data.Sqls.BvOrders.Orders ToLinq(this OrderMain entity)
        {
            return new Layer.Data.Sqls.BvOrders.Orders
            {
                ID = entity.ID,
                UserID = entity.UserID,
                SiteUserName = entity.SiteUserName,
                CompanyName = entity.CompanyName,
                Currency = (int)entity.Currency,
                District = (int)entity.District,
                Transport = (int)entity.Transport,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                Status = (int)entity.Status,
                DeliveryRatio = entity.DeliveryRatio,
                PaidRatio = entity.PaidRatio,
                Summary = entity.Summary
            };
        }

        static internal UserAccountType ToAccountType(this PaymentMethod type)
        {
            UserAccountType atype = UserAccountType.Unknown;
            switch (type)
            {
                case PaymentMethod.Wallet:
                    atype = UserAccountType.Cash;
                    break;
                case PaymentMethod.Credit:
                    atype = UserAccountType.Credit;
                    break;
                default:
                    break;
            }
            return atype;
        }

        static internal Layer.Data.Sqls.BvOrders.Carts ToLinq(this Cart entity)
        {
            return new Layer.Data.Sqls.BvOrders.Carts
            {
                ServiceOutputID = entity.ServiceOutputID,
                ServiceInputID = entity.ServiceInputID,
                UserID = entity.UserID,
                ProductSign = entity.ProductSign,
                Quantity = entity.Quantity,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                CustomerCode = entity.CustomerCode,
                Xml = entity.XmlEle()
            };
        }

        static internal Layer.Data.Sqls.BvOrders.CommodityInputs ToLinq(this CommodityInput entity)
        {
            return new Layer.Data.Sqls.BvOrders.CommodityInputs
            {
                ID = entity.ID,
                OrderID = entity.OrderID,
                ServiceOuputID = entity.ServiceOuputID,
                UserID = entity.UserID,
                Count = entity.Count,
                CreateDate = entity.CreateDate
            };
        }
        static internal Layer.Data.Sqls.BvOrders.CommodityOutputs ToLinq(this CommodityOutput entity)
        {
            return new Layer.Data.Sqls.BvOrders.CommodityOutputs
            {
                ID = entity.ID,
                InputID = entity.InputID,
                OrderID = entity.OrderID,
                ServiceInputID = entity.ServiceInputID,
                Count = entity.Count,
                CreateDate = entity.CreateDate
            };
        }
    }
}
