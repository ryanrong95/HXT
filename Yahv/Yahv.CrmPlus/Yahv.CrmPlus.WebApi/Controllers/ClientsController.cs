using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yahv.CrmPlus.Service.Views.Plugins;
using Yahv.Underly;
using Yahv.Web.Mvc;

namespace Yahv.CrmPlus.WebApi.Controllers
{
    public class ClientsController : ClientController
    {
        /// <summary>
        /// 获取新注册的客户
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult NewRegistered()
        {
            //TODO:暂时使用该视图
            //var client = Erp.Current.CrmPlus.MyClients.OrderByDescending(item => item.ModifyDate)
            //    .Select(item => new { item.ID, item.Name, item.Grade }).FirstOrDefault();

            var client = new { ID = "Ep202103290001", Name = "测试", Grade = 1 };

            return Json(new { code = 200, success = true, data = client }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 客户地址
        /// </summary>
        /// <param name="id">客户ID</param>
        /// <param name="relationType">业务类型</param>
        /// <param name="addressType">地址类型</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Addresses(string id, RelationType relationType = RelationType.Trade, AddressType addressType = AddressType.Consignee)
        {
            try
            {
                if (Erp.Current == null)
                {
                    throw new Exception("请先登录");
                }

                if (string.IsNullOrEmpty(id))
                {
                    throw new Exception("客户ID不能为空");
                }

                using (var view = new ClientAddressesView())
                {
                    var data = view.SearchByClientID(id)
                        .SearchByRelationType(relationType)
                        .SearchByAddressType(addressType);

                    if (!Erp.Current.IsSuper)
                    {
                        data = data.SearchByCreatorID(Erp.Current.ID);
                    }

                    var result = data.ToMyArray().Take(20);
                    return Json(new { success = true, code = 200, data = result }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, code = 500, data = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 客户发票
        /// </summary>
        /// <param name="id">客户ID</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Invoices(string id, RelationType relationType = RelationType.Trade)
        {
            try
            {
                if (Erp.Current == null)
                {
                    throw new Exception("请先登录");
                }

                if (string.IsNullOrEmpty(id))
                {
                    throw new Exception("客户ID不能为空");
                }

                using (var view = new ClientInvoicesView())
                {
                    var data = view.SearchByClientID(id)
                        .SearchByRelationType(relationType);

                    var result = data.ToMyArray();
                    return Json(new { success = true, code = 200, data = result }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, code = 500, data = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 客户付款人
        /// </summary>
        /// <param name="id">客户ID</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Payers(string id, RelationType relationType = RelationType.Trade)
        {
            try
            {
                if (Erp.Current == null)
                {
                    throw new Exception("请先登录");
                }

                if (string.IsNullOrEmpty(id))
                {
                    throw new Exception("客户ID不能为空");
                }

                using (var view = new ClientPayersView())
                {
                    var data = view.SearchByClientID(id)
                        .SearchByRelationType(relationType);

                    var result = data.ToMyArray();
                    return Json(new { success = true, code = 200, data = result }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, code = 500, data = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 客户联系人
        /// </summary>
        /// <param name="id">客户ID</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Contacts(string id, RelationType relationType = RelationType.Trade)
        {
            try
            {
                if (Erp.Current == null)
                {
                    throw new Exception("请先登录");
                }

                if (string.IsNullOrEmpty(id))
                {
                    throw new Exception("客户ID不能为空");
                }

                using (var view = new ClientContactsView())
                {
                    var data = view.SearchByClientID(id)
                        .SearchByRelationType(relationType);

                    if (!Erp.Current.IsSuper)
                    {
                        data = data.SearchByOwnerID(Erp.Current.ID);
                    }

                    var result = data.ToMyArray().Take(20);
                    return Json(new { success = true, code = 200, data = result }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, code = 500, data = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}