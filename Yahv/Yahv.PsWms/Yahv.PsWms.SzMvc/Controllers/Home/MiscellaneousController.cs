using Layers.Data.Sqls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yahv.PsWms.SzMvc.Models;
using Yahv.PsWms.SzMvc.Services.Enums;
using Yahv.Underly;

namespace Yahv.PsWms.SzMvc.Controllers
{
    public class MiscellaneousController : BaseController
    {
        /// <summary>
        /// 包装类型
        /// </summary>
        /// <returns></returns>
        public JsonResult StocktakingTypes()
        {
            var stocktakingTypes = ExtendsEnum.ToDictionary<StocktakingType>().Select(item => new { value = item.Key, text = item.Value }).ToArray();
            return Json(new { success = 200, result = stocktakingTypes }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 货运类型
        /// </summary>
        /// <returns></returns>
        public JsonResult TransportModes()
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();
            dic.Add(TransportMode.Express.GetHashCode(), TransportMode.Express.GetDescription());
            dic.Add(TransportMode.Dtd.GetHashCode(), TransportMode.Dtd.GetDescription());
            dic.Add(TransportMode.PickUp.GetHashCode(), TransportMode.PickUp.GetDescription());

            var transportModes = dic.Select(item => new { value = item.Key, text = item.Value }).ToArray();
            return Json(new { success = 200, result = transportModes }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 快递公司
        /// </summary>
        /// <returns></returns>
        public JsonResult ExpressCompanies()
        {
            Type type = typeof(Express);
            Dictionary<string, string> dic = new Dictionary<string, string>();
            foreach (var item in Enum.GetValues(type).Cast<Enum>())
            {
                var key = item.ToString();
                var value = item.GetDescription();
                dic[key] = value;
            }
            var expressCompanies = dic.Select(item => new { value = item.Key, text = item.Value }).ToArray();
            return Json(new { success = 200, result = expressCompanies }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 库房信息
        /// </summary>
        /// <returns></returns>
        public JsonResult WareHouseInfo()
        {
            string WareHouseName = ConfigurationManager.AppSettings["WareHouseName"];
            string WareHouseAddress = ConfigurationManager.AppSettings["WareHouseAddress"];
            string WareHouseManName = ConfigurationManager.AppSettings["WareHouseManName"];
            string WareHouseTel = ConfigurationManager.AppSettings["WareHouseTel"];
            string WareHouseInfoForCopy = "收货公司：" + WareHouseName + "\r\n" +
                                          "收货地址：" + WareHouseAddress + "\r\n" +
                                          "联系人：" + WareHouseManName + "\r\n" +
                                          "联系电话：" + WareHouseTel;
            var wareHouseInfo = new
            {
                WareHouseName,
                WareHouseAddress,
                WareHouseManName,
                WareHouseTel,
                WareHouseInfoForCopy
            };
            return Json(new { success = 200, result = wareHouseInfo }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 交货联系人信息
        /// </summary>
        /// <returns></returns>
        public JsonResult ConsigneeMans()
        {
            var siteuser = Yahv.PsWms.SzMvc.SiteCoreInfo.Current;
            string theClientID = siteuser.TheClientID;

            using (PsOrderRepository repository = new PsOrderRepository())
            {
                var addresses = repository.ReadTable<Layers.Data.Sqls.PsOrder.Addresses>()
                    .Where(t => t.ClientID == theClientID && t.Type == (int)Services.Enums.AddressType.Consignor && t.Status == (int)GeneralStatus.Normal)
                    .OrderByDescending(t => t.CreateDate)
                    .Select(item => new
                    {
                        ConsigneeManID = item.ID,
                        ConsigneeManName = item.Contact,
                        ConsigneeManTel = item.Phone,
                        ConsigneeManAddress = item.Address,
                    }).ToArray();

                return Json(new { success = 200, result = addresses }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 送货地址信息
        /// </summary>
        /// <returns></returns>
        public JsonResult DeliverTargets()
        {
            var siteuser = Yahv.PsWms.SzMvc.SiteCoreInfo.Current;
            string theClientID = siteuser.TheClientID;

            using (PsOrderRepository repository = new PsOrderRepository())
            {
                var addresses = repository.ReadTable<Layers.Data.Sqls.PsOrder.Addresses>()
                    .Where(t => t.ClientID == theClientID && t.Type == (int)Services.Enums.AddressType.Consignee && t.Status == (int)GeneralStatus.Normal)
                    .OrderByDescending(t => t.CreateDate)
                    .Select(item => new
                    {
                        DeliverTargetID = item.ID,
                        DeliverTargetMan = item.Contact,
                        DeliverTargetTel = item.Phone,
                        DeliverTargetAddress = item.Address,
                        DeliverTargetTitle = item.Title,
                    }).ToArray();

                return Json(new { success = 200, result = addresses }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 提货人信息
        /// </summary>
        /// <returns></returns>
        public JsonResult TakingInfos()
        {
            var siteuser = Yahv.PsWms.SzMvc.SiteCoreInfo.Current;
            string theClientID = siteuser.TheClientID;

            using (PsOrderRepository repository = new PsOrderRepository())
            {
                var pickers = repository.ReadTable<Layers.Data.Sqls.PsOrder.Pickers>()
                    .Where(t => t.ClientID == theClientID && t.Status == (int)GeneralStatus.Normal)
                    .OrderByDescending(t => t.CreateDate)
                    .Select(item => new TakingInfosReturnModel
                    {
                        TakingID = item.ID,
                        TakingMan = item.Contact,
                        TakingTel = item.Phone,
                        ProofTypeValue = Convert.ToString(item.IDType),
                        ProofNumber = item.IDCode,
                    }).ToArray();

                for (int i = 0; i < pickers.Length; i++)
                {
                    pickers[i].ProofTypeDes = ((Services.Enums.IDType)Convert.ToInt32(pickers[i].ProofTypeValue)).GetDescription();
                }

                return Json(new { success = 200, result = pickers }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 快递方式信息
        /// </summary>
        /// <returns></returns>
        public JsonResult ExpressMethods(ExpressMethodsRequestModel model)
        {
            List<string> allExpresses = new List<string>();
            Type type = typeof(Express);
            foreach (var item in Enum.GetValues(type).Cast<Enum>())
            {
                allExpresses.Add(item.ToString());
            }

            if (model.ExpressCompanies == null || model.ExpressCompanies.Length == 0)
            {
                model.ExpressCompanies = allExpresses.ToArray();
            }

            List<ExpressMethodsReturnModel.ExpressMethodSingle> expressMethods = new List<ExpressMethodsReturnModel.ExpressMethodSingle>();

            foreach (var expressCompany in model.ExpressCompanies)
            {
                if (expressCompany == Express.SF.ToString())
                {
                    var expressMethodSF = ExtendsEnum.ToDictionary<Services.Enums.ExpressMethodSF>()
                        .Select(item => new ExpressMethodsReturnModel.Content { value = item.Key, text = item.Value }).ToArray();

                    expressMethods.Add(new ExpressMethodsReturnModel.ExpressMethodSingle
                    {
                        ExpressName = expressCompany,
                        Values = expressMethodSF,
                    });
                }

                if (expressCompany == Express.KY.ToString())
                {
                    var expressMethodKY = ExtendsEnum.ToDictionary<Services.Enums.ExpressMethodKY>()
                        .Select(item => new ExpressMethodsReturnModel.Content { value = item.Key, text = item.Value }).ToArray();

                    expressMethods.Add(new ExpressMethodsReturnModel.ExpressMethodSingle
                    {
                        ExpressName = expressCompany,
                        Values = expressMethodKY,
                    });
                }

                if (expressCompany == Express.DB.ToString())
                {
                    var expressMethodDB = ExtendsEnum.ToDictionary<Services.Enums.ExpressMethodDB>()
                        .Select(item => new ExpressMethodsReturnModel.Content { value = item.Key, text = item.Value }).ToArray();

                    expressMethods.Add(new ExpressMethodsReturnModel.ExpressMethodSingle
                    {
                        ExpressName = expressCompany,
                        Values = expressMethodDB,
                    });
                }
            }

            return Json(new { success = 200, result = expressMethods }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 运费支付信息
        /// </summary>
        /// <returns></returns>
        public JsonResult FreightPays()
        {
            var freightPayers = ExtendsEnum.ToDictionary<Services.Enums.FreightPayer>().Select(item => new { value = item.Key, text = item.Value }).ToArray();
            return Json(new { success = 200, result = freightPayers }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 证件类型信息
        /// </summary>
        /// <returns></returns>
        public JsonResult ProofTypes()
        {
            var IDTypes = ExtendsEnum.ToDictionary<Services.Enums.IDType>().Select(item => new { value = item.Key, text = item.Value }).ToArray();
            return Json(new { success = 200, result = IDTypes }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 入库订单特殊要求信息
        /// </summary>
        /// <returns></returns>
        public JsonResult StorageInSpecialRequires()
        {
            var storageInSpecialRequires = ExtendsEnum.ToDictionary<StorageInSpecialRequire>().Select(item => new { value = item.Key, text = item.Value }).ToArray();
            return Json(new { success = 200, result = storageInSpecialRequires }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 出库订单特殊要求信息
        /// </summary>
        /// <returns></returns>
        public JsonResult StorageOutSpecialRequires()
        {
            var storageOutSpecialRequires = ExtendsEnum.ToDictionary<StorageOutSpecialRequire>().Select(item => new { value = item.Key, text = item.Value }).ToArray();
            return Json(new { success = 200, result = storageOutSpecialRequires }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 订单状态信息(入库订单)
        /// </summary>
        /// <returns></returns>
        public JsonResult OrderStatusesForInStorage()
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();
            dic.Add(OrderStatus.Waiting.GetHashCode(), OrderStatus.Waiting.GetDescription());
            dic.Add(OrderStatus.Storaged.GetHashCode(), OrderStatus.Storaged.GetDescription());
            dic.Add(OrderStatus.Rejected.GetHashCode(), OrderStatus.Rejected.GetDescription());
            dic.Add(OrderStatus.Closed.GetHashCode(), OrderStatus.Closed.GetDescription());
            dic.Add(OrderStatus.Completed.GetHashCode(), OrderStatus.Completed.GetDescription());

            var orderStatuses = dic.Select(item => new { value = item.Key, text = item.Value }).ToArray();
            return Json(new { success = 200, result = orderStatuses }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 订单状态信息(出库订单)
        /// </summary>
        /// <returns></returns>
        public JsonResult OrderStatusesForOutStorage()
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();
            dic.Add(OrderStatus.Waiting.GetHashCode(), OrderStatus.Waiting.GetDescription());
            dic.Add(OrderStatus.Storaged.GetHashCode(), OrderStatus.Storaged.GetDescription());
            dic.Add(OrderStatus.Rejected.GetHashCode(), OrderStatus.Rejected.GetDescription());
            dic.Add(OrderStatus.Closed.GetHashCode(), OrderStatus.Closed.GetDescription());
            dic.Add(OrderStatus.Completed.GetHashCode(), OrderStatus.Completed.GetDescription());

            var orderStatuses = dic.Select(item => new { value = item.Key, text = item.Value }).ToArray();
            return Json(new { success = 200, result = orderStatuses }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 发票交付方式
        /// </summary>
        /// <returns></returns>
        public JsonResult InvoiceDeliveryType()
        {
            var invoiceDeliveryTypes = ExtendsEnum.ToDictionary<Underly.InvoiceDeliveryType>().Select(item => new { value = item.Key, text = item.Value }).ToArray();
            return Json(new { success = 200, result = invoiceDeliveryTypes }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取我们公司的账户信息
        /// </summary>
        /// <returns></returns>
        public JsonResult OurCompanyAccount()
        {
            string OurBankName = ConfigurationManager.AppSettings["OurBankName"];
            string OurBankAccount = ConfigurationManager.AppSettings["OurBankAccount"];
            string OurAccountName = ConfigurationManager.AppSettings["OurAccountName"];

            var result = new
            {
                OurBankName = OurBankName,
                OurBankAccount = OurBankAccount,
                OurAccountName = OurAccountName,
            };

            return Json(new { success = 200, result = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 日志 ActionName
        /// </summary>
        /// <returns></returns>
        public JsonResult LogActionNames()
        {
            var actionNames = ExtendsEnum.ToDictionary<Services.Enums.LogAction>().Select(item => new { value = item.Key, text = item.Value }).ToArray();
            return Json(new { success = 200, result = actionNames }, JsonRequestBehavior.AllowGet);
        }
    }
}