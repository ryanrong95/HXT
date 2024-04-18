using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yahv.PsWms.SzMvc.App_Utils;
using Yahv.PsWms.SzMvc.Models;
using Yahv.PsWms.SzMvc.Services.ClientModels;
using Yahv.PsWms.SzMvc.Services.ClientViews;
using Yahv.PsWms.SzMvc.Services.Enums;
using Yahv.Underly;

namespace Yahv.PsWms.SzMvc.Controllers
{
    public partial class ClientController : BaseController
    {
        #region 页面

        /// <summary>
        /// 客户收货/提货地址列表
        /// </summary>
        /// <returns></returns>
        public ActionResult MyAddress() { return View(); }

        /// <summary>
        /// 编辑收货/提货地址页面
        /// </summary>
        /// <returns></returns>
        public ActionResult _PartialAddress() { return PartialView(); }

        #endregion

        /// <summary>
        /// 刷新收货地址列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetMyConsigneesList()
        {
            var siteuser = Yahv.PsWms.SzMvc.SiteCoreInfo.Current;
            //收货地址列表
            var linq = new SzMvc.Services.ClientViews.AddressView(siteuser.TheClientID).Where(a => a.Type == Services.Enums.AddressType.Consignee && a.Status == Underly.GeneralStatus.Normal).ToArray().Select(item => new
            {
                ID = item.ID,
                Title = item.Title,
                Address = item.ClientAddress,
                Contact = item.Contact,
                Phone = item.Phone,
                Email = item.Email,
                IsDefault = item.IsDefault ? "是" : "否",
            });

            var result = Json(new { consignees = Newtonsoft.Json.JsonConvert.SerializeObject(linq.ToArray(), Newtonsoft.Json.Formatting.None) }, JsonRequestBehavior.AllowGet);
            return result;
        }

        /// <summary>
        /// 刷新提货地址列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetMyConsignorsList()
        {
            var siteuser = Yahv.PsWms.SzMvc.SiteCoreInfo.Current;
            //提货地址列表
            var linq = new SzMvc.Services.ClientViews.AddressView(siteuser.TheClientID).Where(a => a.Type == Services.Enums.AddressType.Consignor && a.Status == Underly.GeneralStatus.Normal).ToArray().Select(item => new
            {
                ID = item.ID,
                Title = item.Title,
                Address = item.ClientAddress,
                Contact = item.Contact,
                Phone = item.Phone,
                Email = item.Email,
                IsDefault = item.IsDefault ? "是" : "否",
            });

            var result = Json(new { consignors = Newtonsoft.Json.JsonConvert.SerializeObject(linq.ToArray(), Newtonsoft.Json.Formatting.None) }, JsonRequestBehavior.AllowGet);
            return result;
        }

        /// <summary>
        /// 新增/编辑收货地址
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult ConsigneeSubmit(ConsigneeAddressViewModel model)
        {
            try
            {
                var user = SiteCoreInfo.Current;
                var consignee = new Address
                {
                    ID = model.ID,
                    Title = model.Title?.Trim(),
                    ClientID = user.TheClientID,
                    //”省\s市\s县\sAddress” 
                    //  ClientAddress = string.Concat(model.ClientAddress,model.AddressDetail),
                    ClientAddress = string.Join(" ", model.ClientAddress?.Concat(new string[] { model.AddressDetail.Trim() }) ?? Array.Empty<string>()),
                    Contact = model.Contact.Trim(),
                    Phone = model.Phone,
                    Email = model.Email ?? string.Empty,
                    Type = Services.Enums.AddressType.Consignee,
                    IsDefault = model.IsDefaultVal == "1" ? true : false,
                };
                //数据持久化
                consignee.Enter();
                //设置默认地址
                if (consignee.IsDefault)
                {
                    consignee.SetDefaultAddress();
                }

                return Json(new { type = "success", msg = "提交成功" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { type = "error", msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 新增/编辑提货地址
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult ConsignorSubmit(ConsigneeAddressViewModel model)
        {
            try
            {
                var user = SiteCoreInfo.Current;
                var consignor = new Address
                {
                    ID = model.ID,
                    Title = model.Title?.Trim(),
                    ClientID = user.TheClientID,
                    //”省\s市\s县\sAddress” 
                    //  ClientAddress = string.Concat(model.ClientAddress,model.AddressDetail),
                    ClientAddress = string.Join(" ", model.ClientAddress?.Concat(new string[] { model.AddressDetail.Trim() }) ?? Array.Empty<string>()),
                    Contact = model.Contact.Trim(),
                    Phone = model.Phone,
                    Email = model.Email ?? string.Empty,
                    Type = Services.Enums.AddressType.Consignor,
                    IsDefault = model.IsDefaultVal == "1" ? true : false,
                };
                //数据持久化
                consignor.Enter();
                //设置默认地址
                if (consignor.IsDefault)
                {
                    consignor.SetDefaultAddress();
                }

                return Json(new { type = "success", msg = "提交成功" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { type = "error", msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 获取单个地址信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult GetAddressInfo(string id)
        {
            var model = new ConsigneeAddressViewModel();
            var list = new AddressView(SiteCoreInfo.Current.TheClientID).FirstOrDefault(a => a.ID == id);
            if (list != null)
            {
                model.ID = list.ID;
                model.Title = list.Title;

                model.ClientAddress = list.ClientAddress.ToAddress();
                model.AddressDetail = list.ClientAddress.ToDetailAddress();

                model.Contact = list.Contact;
                model.Phone = list.Phone;
                model.Email = list.Email;
                model.IsDefaultVal = list.IsDefault ? "1" : "2";
            }
            return Json(new { data = Newtonsoft.Json.JsonConvert.SerializeObject(model, Newtonsoft.Json.Formatting.None) }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除地址
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult DeleteAddress(string id)
        {
            var model = new Address();
            model.Delete(id);
            return Json(new { type = "success", msg = "删除成功" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 设置默认地址
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult SetDefault(string id)
        {
            var siteuser = SiteCoreInfo.Current;
            var model = new AddressView(siteuser.TheClientID).FirstOrDefault(a => a.ID == id);
            model.IsDefault = true;
            model.Enter();
            return Json(new { type = "success", msg = "设置成功" }, JsonRequestBehavior.AllowGet);
        }
    }
}