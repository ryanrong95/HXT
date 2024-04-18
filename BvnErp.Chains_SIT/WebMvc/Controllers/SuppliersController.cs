using Needs.Utils;
using Needs.Utils.Serializers;
using Needs.Wl.Client.Services;
using Needs.Wl.Logs.Services;
using Needs.Wl.Models;
using Needs.Wl.Web.Mvc;
using Needs.Wl.Web.Mvc.Utils;
using System;
using System.Linq;
using System.Web.Mvc;
using WebMvc.Models;

namespace WebMvc.Controllers
{
    /// <summary>
    /// 客户供应商 
    /// 供应商信息、供应商银行账号、提货地址
    /// </summary>
    [UserHandleError(ExceptionType = typeof(Exception))]
    public class SuppliersController : UserController
    {
        // GET: 会员供应商列表
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult MySuppliers()
        {
            var pickSupplier = new Needs.Ccs.Services.Views.OrderConsigneesView().Select(item => item.ClientSupplierID).ToArray();
            var paySupplier = new Needs.Ccs.Services.Views.OrderPayExchangeSuppliersView().Select(item => item.ClientSupplier.ID).ToArray();
            var ids = pickSupplier.Concat(paySupplier).Distinct();

            var supplerView = Needs.Wl.User.Plat.UserPlat.Current.MySuppliers;
            supplerView.AllowPaging = false;

            var list = supplerView.ToArray().Select(item => new
            {
                item.ID,
                item.Name,
                item.ChineseName,
                isShowBtn = !ids.Contains(item.ID)
            });

            return View(list);
        }

        /// <summary>
        /// GET: 供应商信息
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult _PartialSupplierInfo()
        {
            var model = new SupplierInfoViewModel();
            return PartialView(model);
        }

        /// <summary>
        /// 供应商银行账户
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult SupplierBank(string id)
        {
            string supplerID = id.InputText();
            var supplier = Needs.Wl.User.Plat.UserPlat.Current.MySuppliers[supplerID];
            if (supplier == null || supplier.ID == "" || supplier.ClientID != Needs.Wl.User.Plat.UserPlat.Current.ClientID)
            {
                //尝试攻击或在URL中输入错误的ID
                return View("Error");
            }

            var view = supplier.Banks();
            view.AllowPaging = false;
            var list = view.ToArray().Select(item => new
            {
                item.ID,
                item.ClientSupplierID,
                item.BankAccount,
                item.BankName,
                item.BankAddress,
                item.SwiftCode,
                item.Summary,
            });

            return View(list);
        }

        /// <summary>
        /// 供应商提货地址
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult SupplierAddress(string id)
        {
            string supplerID = id.InputText();
            var supplier = Needs.Wl.User.Plat.UserPlat.Current.MySuppliers[supplerID];
            if (supplier == null || supplier.ID == "" || supplier.ClientID != Needs.Wl.User.Plat.UserPlat.Current.ClientID)
            {
                //尝试攻击或在URL中输入错误的ID，返回错误
                return View("Error");
            }

            var list = supplier.Addresses().ToArray().Select(item => new
            {
                item.ID,
                item.ClientSupplierID,
                IsDefault = item.IsDefault ? "是" : "否",
                item.Address,
                item.Contact.Name,
                item.Contact.Mobile,
            });
            return View(list);
        }

        /// <summary>
        /// GET: 供应商银行地址
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult _PartialBeneficiarieInfo()
        {
            BeneficiarieInfoViewModel model = new BeneficiarieInfoViewModel();
            return PartialView(model);
        }

        /// <summary>
        /// GET: 供应商提货地址
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult _PartialSupplierAddressInfo()
        {
            SupplierAddressesViewModel model = new SupplierAddressesViewModel();
            return PartialView(model);
        }

        /// <summary>
        /// POST: 新增修改供应商
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult _partialSupplierInfo(SupplierInfoViewModel model)
        {
            var supplier = Needs.Wl.User.Plat.UserPlat.Current.MySuppliers[model.ID] ?? new Needs.Wl.Models.ClientSupplier();

            if (!string.IsNullOrWhiteSpace(supplier.ID))
            {
                //香港交货方式存在订单不能修改
                if (SupplierUtils.SupplierIsUesedByOrder(supplier.ID))
                {
                    return base.JsonResult(VueMsgType.error, "已经使用该供应商下单，不能修改");
                }
                //付汇供应商存在订单不能删除
                if (SupplierUtils.SupplierIsUesedByOrderPayExchange(supplier.ID))
                {
                    return base.JsonResult(VueMsgType.error, "已经使用该供应商下单，不能修改");
                }
            }
            supplier.Name = model.Name;
            supplier.ChineseName = model.ChineseName;
            supplier.Summary = model.Summary;
            supplier.ClientID = Needs.Wl.User.Plat.UserPlat.Current.Client.ID;
            supplier.Enter();
            return base.JsonResult(VueMsgType.success, "操作成功");
        }

        /// <summary>
        /// POST: 新增修改供应商银行
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult _PartialBeneficiarieInfo(BeneficiarieInfoViewModel data)
        {
            try
            {
                var bankView = Needs.Wl.User.Plat.UserPlat.Current.MySuppliers[data.ClientSupplierID].Banks();
                bankView.Predicate = item => (item.BankAccount == data.BankAccount && item.SwiftCode == data.SwiftCode)
                                    && ((data.ID != "" && item.ID != data.ID) || (data.ID == ""));

                if (bankView.RecordCount > 0)
                {
                    return base.JsonResult(VueMsgType.error, "不可添加重复的供应商银行账号");
                }

                bankView.Predicate = null;//TODO:完成框架的查询条件Clear()
                var bank = bankView[data.ID] ?? new Needs.Wl.Models.ClientSupplierBank();
                bank.ClientSupplierID = data.ClientSupplierID;
                bank.BankAccount = data.BankAccount;
                bank.BankName = data.BankName;
                bank.BankAddress = data.BankAddress;
                bank.Summary = data.Summary;
                bank.SwiftCode = data.SwiftCode;
                bank.Enter();
            }
            catch (Exception ex)
            {
                ex.Log();
                return base.JsonResult(VueMsgType.success, "编辑提货地址失败，请稍后重试或联系您的业务经理。");
            }

            return base.JsonResult(VueMsgType.success, "操作成功");
        }

        /// <summary>
        /// POST: 新增修改供应商提货地址
        /// </summary>
        /// <returns>
        [HttpPost]
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult _PartialSupplierAddressInfo(SupplierAddressesViewModel data)
        {
            try
            {
                var contact = new Needs.Wl.Models.Contact
                {
                    Name = data.Name,
                    Mobile = data.Mobile
                };

                var addressView = Needs.Wl.User.Plat.UserPlat.Current.MySuppliers[data.ClientSupplierID].Addresses();
                var address = addressView[data.ID] ?? new Needs.Wl.Models.ClientSupplierAddress();
                address.ClientSupplierID = data.ClientSupplierID;
                address.Contact = contact;
                address.IsDefault = data.IsDefault;
                address.Address = string.Join(" ", data.Address) + " " + data.DetailAddress.Replace(" ", "");
                address.Summary = data.Summary;
                address.Enter();
            }
            catch (Exception ex)
            {
                ex.Log();
                return base.JsonResult(VueMsgType.success, "编辑提货地址失败，请稍后重试或联系您的业务经理。");
            }

            return base.JsonResult(VueMsgType.success, "操作成功");
        }

        /// <summary>
        /// POST: 删除供应商
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult DelSupplier(string id)
        {
            try
            {
                var supplier = Needs.Wl.User.Plat.UserPlat.Current.MySuppliers[id];
                if (supplier.ClientID != Needs.Wl.User.Plat.UserPlat.Current.ClientID)
                {
                    //是否是本客户的供应商
                    return base.JsonResult(VueMsgType.error, "删除失败");
                }

                supplier.Abandon();
            }
            catch (Exception ex)
            {
                ex.Log();
                return base.JsonResult(VueMsgType.success, ex.Message);
            }

            return base.JsonResult(VueMsgType.success, "删除成功");
        }

        /// <summary>
        /// 删除供应商银行账号
        /// </summary>
        /// <param name="bankid">银行账号ID</param>
        /// <param name="supplierid">供应商ID</param>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult DelSupplierBank(string bankid, string supplierid)
        {
            try
            {
                bankid = bankid.InputText();
                supplierid = supplierid.InputText();
                var supplier = Needs.Wl.User.Plat.UserPlat.Current.MySuppliers[supplierid];
                if (supplier.ClientID != Needs.Wl.User.Plat.UserPlat.Current.ClientID)
                {
                    //尝试攻击或在URL中输入错误的ID，返回错误
                    return base.JsonResult(VueMsgType.error, "删除失败");
                }

                var banksView = supplier.Banks();
                var entity = banksView[bankid];
                entity.Abandon();
            }
            catch (Exception ex)
            {
                ex.Log();
                return base.JsonResult(VueMsgType.error, "删除失败");
            }
            return base.JsonResult(VueMsgType.success, "删除成功");
        }

        /// <summary>
        /// 删除供应商提货地址
        /// </summary>
        /// <param name="addressid">提货地址ID</param>
        /// <param name="supplierid">供应商ID</param>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult DelSupplierAddress(string addressid, string supplierid)
        {
            try
            {
                addressid = addressid.InputText();
                supplierid = supplierid.InputText();
                var supplier = Needs.Wl.User.Plat.UserPlat.Current.MySuppliers[supplierid];
                if (supplier.ClientID != Needs.Wl.User.Plat.UserPlat.Current.ClientID)
                {
                    //尝试攻击或在URL中输入错误的ID，返回错误
                    return base.JsonResult(VueMsgType.error, "删除失败");
                }

                var addressesView = supplier.Addresses();
                var addresses = addressesView[addressid];
                addresses.Abandon();
            }
            catch (Exception ex)
            {
                ex.Log();
                return base.JsonResult(VueMsgType.error, "删除失败！");
            }

            return base.JsonResult(VueMsgType.success, "删除成功");
        }

        #region  数据绑定

        /// <summary>
        /// 获取供应商列表
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult GetSuppliersList()
        {
            var pickSupplier = new Needs.Ccs.Services.Views.OrderConsigneesView().Select(item => item.ClientSupplier.ID).ToArray();
            var paySupplier = new Needs.Ccs.Services.Views.OrderPayExchangeSuppliersView().Select(item => item.ClientSupplier.ID).ToArray();
            var ids = pickSupplier.Concat(paySupplier);

            var view = Needs.Wl.User.Plat.UserPlat.Current.MySuppliers;
            view.AllowPaging = false;

            var list = view.ToArray().Select(item => new
            {
                item.ID,
                item.Name,
                item.ChineseName,
                isShowBtn = !ids.Contains(item.ID)
            });

            return base.JsonResult(VueMsgType.success, "", list.Json());
        }

        /// <summary>
        /// 获取供应商银行列表
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult GetSuppliersBankList(string ID)
        {
            var view = Needs.Wl.User.Plat.UserPlat.Current.MySuppliers[ID].Banks();
            view.AllowPaging = false;

            var list = view.ToArray().Select(item => new
            {
                item.ID,
                item.ClientSupplierID,
                item.BankAccount,
                item.BankName,
                item.BankAddress,
                item.SwiftCode,
                item.Summary,
            });
            return base.JsonResult(VueMsgType.success, "", list.Json());
        }

        /// <summary>
        /// 获取供应商地址列表
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult GetSuppliersaddressList(string ID)
        {
            var view = Needs.Wl.User.Plat.UserPlat.Current.MySuppliers[ID].Addresses();
            view.AllowPaging = false;

            var list = view.ToArray().Select(item => new
            {
                item.ID,
                item.ClientSupplierID,
                IsDefault = item.IsDefault ? "是" : "否",
                item.Address,
                item.Contact.Name,
                item.Contact.Mobile,
            });

            return base.JsonResult(VueMsgType.success, "", list.Json());
        }

        /// <summary>
        ///  获取供应商信息
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult GetSupplierInfo(string id)
        {
            var model = new SupplierInfoViewModel();
            var list = Needs.Wl.User.Plat.UserPlat.Current.MySuppliers[id];
            if (list != null)
            {
                model.ID = list.ID;
                model.Name = list.Name;
                model.ChineseName = list.ChineseName;
                model.Summary = list.Summary;
            }
            return base.JsonResult(VueMsgType.success, "", model.Json());
        }

        /// <summary>
        ///  获取供应商银行信息
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult GetBeneficiarieInfo(string bankid, string supplierid)
        {
            bankid = bankid.InputText();
            supplierid = supplierid.InputText();
            var model = new BeneficiarieInfoViewModel();
            var list = Needs.Wl.User.Plat.UserPlat.Current.MySuppliers[supplierid].Banks()[bankid];
            if (list == null)
            {
                return base.JsonResult(VueMsgType.error, "该银行账号不存在！");
            }
            else
            {
                model.ID = list.ID;
                model.ClientSupplierID = list.ClientSupplierID;
                model.BankAccount = list.BankAccount;
                model.BankName = list.BankName;
                model.BankAddress = list.BankAddress;
                model.Summary = list.Summary;
                model.SwiftCode = list.SwiftCode;
            }
            return base.JsonResult(VueMsgType.success, "", model.Json());
        }

        /// <summary>
        ///  获取供应商提货地址信息
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult GetSupplierAddressInfo(string addressid, string supplierid)
        {
            addressid = addressid.InputText();
            supplierid = supplierid.InputText();
            var model = new SupplierAddressesViewModel();
            var list = Needs.Wl.User.Plat.UserPlat.Current.MySuppliers[supplierid].Addresses()[addressid];
            if (list == null)
            {
                return base.JsonResult(VueMsgType.error, "该提货地址不存在！");
            }
            else
            {
                model.ID = list.ID;
                model.ClientSupplierID = list.ClientSupplierID;
                model.IsDefault = list.IsDefault;
                model.Name = list.Contact.Name;
                model.Mobile = list.Contact.Mobile;
                model.Address = list.Address.ToAddress();
                model.DetailAddress = list.Address.ToDetailAddress();
                model.AllAddress = list.Address;
                model.Summary = list.Summary;
            }
            return base.JsonResult(VueMsgType.success, "", model.Json());
        }

        #endregion

        #region 数据处理

        /// <summary>
        /// 验证供应商中文名是否重复
        /// </summary>
        /// <param name="CheckSupplierChineseName"></param>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult CheckSupplierChineseName(string ChineseName, string ID)
        {
            ChineseName = ChineseName.InputText();
            var view = Needs.Wl.User.Plat.UserPlat.Current.MySuppliers;
            view.Predicate = item => item.ChineseName == ChineseName && ((ID != null && item.ID != ID) || (ID == null));
            if (view.RecordCount > 0)
            {
                return base.JsonResult(VueMsgType.error, "该供应商名称已存在！");
            }

            return base.JsonResult(VueMsgType.success, "");
        }

        /// <summary>
        /// 验证供应商英文名是否重复
        /// </summary>
        /// <param name="CheckSupplierChineseName"></param>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult CheckSupplierName(string Name, string ID)
        {
            Name = Name.InputText();
            ID = ID.InputText();
            var view = Needs.Wl.User.Plat.UserPlat.Current.MySuppliers;
            view.Predicate = item => item.Name == Name && ((ID != null && item.ID != ID) || (ID == null));
            if (view.RecordCount > 0)
            {
                return base.JsonResult(VueMsgType.error, "该供应商英文名称已存在！");
            }
            return base.JsonResult(VueMsgType.success, "");
        }

        /// <summary>
        /// 验证供应商的银行账号是否重复
        /// </summary>
        /// <param name="BankAccount">账号</param>
        /// <param name="ClientSupplierID">供应商ID</param>
        /// <param name="ID">账号ID</param>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult CheckSupplierBank(string BankAccount, string Code, string ClientSupplierID, string ID)
        {
            BankAccount = BankAccount.InputText();
            ClientSupplierID = ClientSupplierID.InputText();
            ID = ID.InputText();
            var view = Needs.Wl.User.Plat.UserPlat.Current.MySuppliers[ClientSupplierID].Banks();
            view.Predicate = item => (item.BankAccount == BankAccount && item.SwiftCode == Code) && ((ID != "" && item.ID != ID) || (ID == ""));

            if (view.RecordCount > 0)
            {
                return base.JsonResult(VueMsgType.error, "该银行账号已存在！");
            }
            return base.JsonResult(VueMsgType.success, "");
        }

        #endregion
    }
}