using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Yahv.PvWsClient.WebAppNew.App_Utils;
using Yahv.PvWsClient.WebAppNew.Controllers.Attribute;
using Yahv.PvWsClient.WebAppNew.Models;
using Yahv.PvWsOrder.Services.ClientModels.Client;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.PvWsOrder.Services.XDTClientView;
using Yahv.PvWsOrder.Services.XDTModels;
using Yahv.Underly;
using Yahv.Utils.Serializers;

namespace Yahv.PvWsClient.WebAppNew.Controllers
{
    /// <summary>
    /// 产品
    /// </summary>
    public class ProductsController : UserController
    {
        #region 我的产品

        /// <summary>
        /// 我的产品
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public ActionResult MyProducts()
        {
            var current = Yahv.Client.Current;
            var client = current.MyClients;

            ServiceType serviceType = client.ServiceType;
            bool isCustoms = (serviceType & ServiceType.Customs) == ServiceType.Customs;
            //bool isWarehouse = (serviceType & ServiceType.Warehouse) == ServiceType.Warehouse;
            bool isCustomsInfoOK = isCustoms & client.IsDeclaretion;
            //bool isWarehouseInfoOK = isWarehouse & client.IsStorageService;

            ViewBag.ClientIsValid = isCustomsInfoOK ? "true" : "false";
            return View();
        }

        /// <summary>
        /// 获取我的产品
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public JsonResult GetMyProducts()
        {
            var product = Client.Current.MyClientProducts.AsQueryable();

            //查询条件
            var paramlist = new
            {
                Name = Request.Form["Name"],
                Model = Request.Form["Model"],
                Manufacturer = Request.Form["Manufacturer"],
            };
            if(!string.IsNullOrWhiteSpace(paramlist.Name))
            {
                product = product.Where(item => item.Name.Contains(paramlist.Name));
            }
            if(!string.IsNullOrWhiteSpace(paramlist.Model))
            {
                product = product.Where(item => item.Model.Contains(paramlist.Model));
            }
            if(!string.IsNullOrWhiteSpace(paramlist.Manufacturer))
            {
                product = product.Where(item => item.Manufacturer.Contains(paramlist.Manufacturer));
            }

            //页面需要数据过滤
            Func<Services.Models.ClientProduct, object> convert = item => new
            {
                item.ID,
                item.Name,
                item.Model,
                item.Manufacturer,
                item.Batch
            };

            return this.Paging(product, convert);
        }

        /// <summary>
        /// 产品分部视图
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public ActionResult _PartialProduct()
        {
            var model = new MyProductsViewModel();
            return PartialView(model);
        }

        /// <summary>
        /// 提交产品
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [UserAuthorize(UserAuthorize = true, MobileLog = true)]
        public ActionResult ProductSubmit(MyProductsViewModel model)
        {
            try
            {
                var user = Client.Current;
                var product = user.MyClientProducts[model.ID] ?? new Services.Models.ClientProduct();
                product.ID = model.ID;
                product.ClientID = Client.Current.XDTClientID;
                product.Batch = model.Batch;
                product.Manufacturer = model.Manufacturer;
                product.Model = model.Models;
                product.Name = model.Name;
                user.MyClientProducts.Enter(product);
                return base.JsonResult(VueMsgType.success, "操作成功");
            }
            catch (Exception ex)
            {
                return base.JsonResult(VueMsgType.error, ex.Message);
            }
        }

        /// <summary>
        ///  获取产品信息
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public ActionResult GetProductInfo(string id)
        {
            var model = new MyProductsViewModel();
            var product = Client.Current.MyClientProducts[id];
            if (product != null)
            {
                model.ID = product.ID;
                model.Name = product.Name;
                model.Models = product.Model;
                model.Manufacturer = product.Manufacturer;
                model.Batch = product.Batch;
            }
            return base.JsonResult(VueMsgType.success, "", model.Json());
        }

        /// <summary>
        /// 删除产品
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, MobileLog = true)]
        public ActionResult DelProduct(string id)
        {
            try
            {
                var user = Client.Current;
                var product = user.MyClientProducts[id];
                if (product == null)
                {
                    return base.JsonResult(VueMsgType.error, "删除失败");
                }
                user.MyClientProducts.Abandon(product);
            }
            catch (Exception ex)
            {
                return base.JsonResult(VueMsgType.error, ex.Message);
            }

            return base.JsonResult(VueMsgType.success, "删除成功");
        }

        /// <summary>
        /// 加入预归类分部视图
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public ActionResult _PartialPreProduct()
        {
            var model = new PreProductInfoViewModel();
            var data = new
            {
                CurrencyOptions = ExtendsEnum.ToNameDictionary<Currency>().Where(item => item.Value != "Unknown").Select(item => new { value = item.Value, text = item.Name }).ToArray(),
            };
            ViewBag.Options = data;
            return PartialView(model);
        }

        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public ActionResult _PartialPreProductNew()
        {
            var model = new PreProductInfoViewModel();
            var data = new
            {
                CurrencyOptions = ExtendsEnum.ToNameDictionary<Currency>().Where(item => item.Value != "Unknown").Select(item => new { value = item.Value, text = item.Name }).ToArray(),
            };
            ViewBag.Options = data;
            return PartialView(model);
        }

        /// <summary>
        /// 提交预归类
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [UserAuthorize(UserAuthorize = true)]
        public JsonResult _PartialPreProduct(PreProductInfoViewModel model)
        {
            try
            {
                var client = Client.Current;
                var product = new PreProduct();
                product.ClientID = client.XDTClientID;
                product.Currency = model.Currency;
                product.ProductName = model.ProductName;
                product.Manufacturer = model.Manufacturer;
                product.Model = model.Models;
                product.ProductUnionCode = model.ProductUnionCode;
                product.CompanyType = client.XDTClientType == PvWsOrder.Services.Enums.ClientType.External ? CompanyTypeEnums.OutSide : CompanyTypeEnums.Icgoo;
                product.Price = model.Price.Value;
                product.Qty = model.Qty;
                product.DueDate = model.DueDate;
                product.UseType = PreProduct.PreProductUserType.Pre;
                client.MyPreProducts.Enter(product);
                return base.JsonResult(VueMsgType.success, "操作成功");
            }
            catch (Exception ex)
            {
                return base.JsonResult(VueMsgType.error, ex.Message);
            }
        }

        /// <summary>
        /// 验证产品识别是否重复
        /// </summary>
        /// <param name="ProductUnionCode"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public JsonResult CheckProductUnionCode(string ProductUnionCode, string ID)
        {
            ProductUnionCode = ProductUnionCode.InputText();
            ID = ID.InputText();
            var view = Client.Current.MyClassifiedPreProducts.ToList().Where(item => item.ClassifyStatus != PvWsOrder.Services.XDTClientView.ClassifyStatus.Anomaly && item.ProductUnionCode == ProductUnionCode && ((ID != "" && item.ID != ID) || (ID == "")));
            if (view.Count() > 0)
            {
                return base.JsonResult(VueMsgType.error, "物料号重复");
            }
            return base.JsonResult(VueMsgType.success, "");
        }

        /// <summary>
        /// 我的产品的分部视图
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public ActionResult _PartialMyPreProducts()
        {
            var product = Client.Current.MyClientProducts.ToList().Select(item => new
            {
                item.ID,
                item.Name,
                item.Model,
                item.Manufacturer,
                item.Batch,
            });
            return PartialView(product);
        }
        #endregion


        #region 我的预归类
        /// <summary>
        /// 我的预归类
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public ActionResult MyPreProducts()
        {
            var current = Yahv.Client.Current;
            var client = current.MyClients;

            ServiceType serviceType = client.ServiceType;
            bool isCustoms = (serviceType & ServiceType.Customs) == ServiceType.Customs;
            //bool isWarehouse = (serviceType & ServiceType.Warehouse) == ServiceType.Warehouse;
            bool isCustomsInfoOK = isCustoms & client.IsDeclaretion;
            //bool isWarehouseInfoOK = isWarehouse & client.IsStorageService;

            ViewBag.ClientIsValid = isCustomsInfoOK ? "true" : "false";

            ViewBag.ClassifyStatusOptions = ExtendsEnum.ToDictionary<Yahv.PvWsOrder.Services.XDTClientView.ClassifyStatus>().Select(item => new
            {
                value = item.Key,
                text = item.Value,
            }).ToArray();

            return View();
        }

        /// <summary>
        /// 产品预归类列表
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public JsonResult GetPreProductsList()
        {
            var ProductName = Request.Form["ProductName"];  //查询数据   
            var Model = Request.Form["Model"];  //查询数据    
            var Manufacturer = Request.Form["Manufacturer"];  //查询数据    
            var ClassifyStatus = Request.Form["ClassifyStatus"];
            var product = Client.Current.MyClassifiedPreProducts;
            var MultiField = Request.Form["MultiField"];

            List<LambdaExpression> lambdas = new List<LambdaExpression>();
            Expression<Func<ClassifiedPreProduct, bool>> lambda = item => true;
            if (!string.IsNullOrWhiteSpace(ProductName))
            {
                lambda = item => item.ProductName.ToUpper().Contains(ProductName.ToUpper());
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrWhiteSpace(Model))
            {
                lambda = item =>  item.Model.ToUpper().Contains(Model.ToUpper());
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrWhiteSpace(Manufacturer))
            {
                lambda = item => item.Manufacturer.ToUpper().Contains(Manufacturer.ToUpper());
                lambdas.Add(lambda);
            }

            if (!string.IsNullOrEmpty(ClassifyStatus) && int.TryParse(ClassifyStatus, out int classifyStatusInt_1))
            {
                lambda = item => item.ClassifyStatusInt == classifyStatusInt_1;
                lambdas.Add(lambda);
            }

            if (!string.IsNullOrEmpty(MultiField))
            {
                lambda = item => item.ProductName.ToUpper().Contains(MultiField.ToUpper())
                              || item.Model.ToUpper().Contains(MultiField.ToUpper())
                              || item.Manufacturer.ToUpper().Contains(MultiField.ToUpper());
                lambdas.Add(lambda);
            }

            #region 页面数据
            int page = int.Parse(Request.Form["page"]);
            int rows = int.Parse(Request.Form["rows"]);
            var list = product.GetPageList(lambdas.ToArray(), rows, page);
            Func<ClassifiedPreProduct, object> convert = item => new
            {
                item.ID,
                item.Manufacturer,
                item.Model,
                item.ProductName,
                ClassifyStatus = item.ClassifyStatus == null ? "" : item.ClassifyStatus.GetDescription(),
                item.Supplier,
                item.ProductUnionCode,
                Currency = ExtendsEnum.ToNameDictionary<Currency>().FirstOrDefault(a => a.Value == item.Currency)?.Name,
                CurrencyCode = item.Currency,
                DueDate=item.DueDate==null?"":item.DueDate.Value.ToString("yyyy-MM-dd"),
                DueDate1=item.DueDate,
                item.Qty,
                IsCheck = false,
                IsClassified = item.ClassifyStatus == PvWsOrder.Services.XDTClientView.ClassifyStatus.Done,
                IsUnClassified = item.ClassifyStatus == PvWsOrder.Services.XDTClientView.ClassifyStatus.Unclassified,
                IsNomarl = !((item.Type & ItemCategoryType.HKForbid) > 0 || (item.Type & ItemCategoryType.Forbid) > 0),
                Classified = item.Type == null ? "" : item.Type.GetFlagsDescriptions("|"),
                ClassifiedLabel = item.Type == null ? "" : item.Type.GetFlagsDescriptions("|"),
                UniqueCode = item.ProductUnionCode,
                item.CIQCode,
                item.Elements,
                item.HSCode,
                item.TaxCode,
                item.TariffRate,
                item.Unit1,
                item.Unit2,
                item.TaxName,
                item.AddedValueRate,
                item.Price,
                Isdetail=false,
            };
            #endregion
            return this.Paging(list, list.Total, convert);
        }

        /// <summary>
        /// 删除预归类产品
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, MobileLog = true)]
        public ActionResult DelPreProduct(string id)
        {
            try
            {
                var entity = Client.Current.MyPreProducts[id.InputText()];
                entity.Abandon();
            }
            catch (Exception ex)
            {
                return base.JsonResult(VueMsgType.error, "删除失败");
            }
            return base.JsonResult(VueMsgType.success, "删除成功");
        }

        #endregion
    }
}