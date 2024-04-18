using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using Needs.Wl.Logs.Services;
using Needs.Wl.Web.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMvc.Models;

namespace WebMvc.Controllers
{
    /// <summary>
    /// 会员的产品
    /// </summary>
    [UserHandleError(ExceptionType = typeof(Exception))]
    public class ProductsController : UserController
    {
        #region 我的产品

        /// <summary>
        /// 我的产品
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult MyProducts()
        {
            return View();
        }

        /// <summary>
        /// 获取我的产品
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult GetMyProducts()
        {
            var query = Request.Form["query"];  //查询数据          
            int page = int.Parse(Request.Form["page"]);
            int rows = int.Parse(Request.Form["rows"]);

            var productView = Needs.Wl.User.Plat.UserPlat.Current.MyProducts;

            if (!string.IsNullOrWhiteSpace(query))
            {
                productView.Predicate = item => item.Name.ToUpper().Contains(query.ToUpper()) || item.Model.ToUpper().Contains(query.ToUpper()) || item.Manufacturer.ToUpper().Contains(query.ToUpper());
            }

            productView.PageIndex = page;
            productView.PageSize = rows;
            int total = productView.RecordCount;

            var list = productView.ToList().Select(item => new
            {
                item.ID,
                item.Name,
                item.Model,
                item.Manufacturer,
                item.Batch
            }).ToArray();

            return JsonResult(VueMsgType.success, "", new { list, total }.Json());
        }

        /// <summary>
        /// 产品分部视图
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
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
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult _PartialProduct(MyProductsViewModel model)
        {
            try
            {
                var user = Needs.Wl.User.Plat.UserPlat.Current;
                var product = user.MyProducts[model.ID] ?? new Needs.Wl.Models.ClientProducts();
                product.ClientID = user.ClientID;
                product.Batch = model.Batch;
                product.Manufacturer = model.Manufacturer;
                product.Model = model.Models;
                product.Name = model.Name;
                product.Enter();
                return base.JsonResult(VueMsgType.success, "操作成功");
            }
            catch (Exception ex)
            {
                ex.Log();
                return base.JsonResult(VueMsgType.error, ex.Message);
            }
        }

        /// <summary>
        ///  获取产品信息
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult GetProductInfo(string id)
        {
            var model = new MyProductsViewModel();
            var product = Needs.Wl.User.Plat.UserPlat.Current.MyProducts[id];
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
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult DelProduct(string id)
        {
            try
            {
                var pro = Needs.Wl.User.Plat.UserPlat.Current.MyProducts[id];
                pro.Abandon();
            }
            catch (Exception ex)
            {
                ex.Log();
                return base.JsonResult(VueMsgType.error, "删除失败");
            }

            return base.JsonResult(VueMsgType.success, "删除成功");
        }

        /// <summary>
        /// 加入预归类分部视图
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult _PartialPreProduct()
        {
            var model = new PreProductInfoViewModel();
            var view = Needs.Wl.User.Plat.UserPlat.Currencies;
            view.AllowPaging = false;

            model.CurrencyOptions = view.ToArray().Select(item => new
            {
                value = item.Code,
                text = item.Code + " " + item.Name
            }).Json();
            return PartialView(model);
        }

        /// <summary>
        /// 提交预归类
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult _PartialPreProduct(PreProductInfoViewModel model)
        {
            try
            {
                var client = Needs.Wl.User.Plat.UserPlat.Current.Client;
                string unionCode = model.ProductUnionCode.InputText();
                var preProduct = Needs.Wl.User.Plat.UserPlat.Current.MyPreProducts.FindByUnionCode(unionCode);

                var product = new PreProduct();
                if (preProduct == null)
                {
                    product.ID = Guid.NewGuid().ToString("N").ToUpper();
                }

                product.ClientID = client.ID;
                product.Currency = model.Currency;
                product.Manufacturer = model.Manufacturer;
                product.Model = model.Models;
                product.ProductUnionCode = model.ProductUnionCode;
                product.CompanyType = client.ClientType == Needs.Wl.Models.Enums.ClientType.External ? CompanyTypeEnums.OutSide : CompanyTypeEnums.Icgoo;
                product.Price = model.Price.Value;
                product.Enter();

                return base.JsonResult(VueMsgType.success, "操作成功");
            }
            catch (Exception ex)
            {
                ex.Log();
                return base.JsonResult(VueMsgType.error, ex.Message);
            }
        }

        /// <summary>
        /// 验证产品识别是否重复
        /// </summary>
        /// <param name="ProductUnionCode"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult CheckProductUnionCode(string ProductUnionCode, string ID)
        {
            ProductUnionCode = ProductUnionCode.InputText();
            ID = ID.InputText();
            var view = Needs.Wl.User.Plat.UserPlat.Current.MyClassifiedPreProducts;
            view.Predicate = item => item.ClassifyStatus != Needs.Wl.Models.Enums.ClassifyStatus.Anomaly && item.ProductUnionCode == ProductUnionCode && ((ID != "" && item.ID != ID) || (ID == ""));

            if (view.RecordCount > 0)
            {
                return base.JsonResult(VueMsgType.error, "物料号重复");
            }
            return base.JsonResult(VueMsgType.success, "");
        }

        #endregion

        #region 自定义产品税号

        /// <summary>
        /// 自定义产品税号列表
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult ProductTaxCategories()
        {
            return View();
        }

        /// <summary>
        /// 获取自定义产品税号列表
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult GetProductTaxCategories()
        {
            int page = int.Parse(Request.Form["page"]);
            int rows = int.Parse(Request.Form["rows"]);

            var query = Request.Form["query"];  //查询数据
            var productView = Needs.Wl.User.Plat.UserPlat.Current.MyProductTaxCategories;
            if (!string.IsNullOrWhiteSpace(query))
            {
                productView.Predicate = item => item.Name.ToUpper().Contains(query.ToUpper()) || item.Model.ToUpper().Contains(query.ToUpper());
            }
            productView.PageSize = rows;
            productView.PageIndex = page;

            int total = productView.RecordCount;
            var list = productView.ToList().Select(item => new
            {
                item.ID,
                item.Name,
                item.Model,
                item.TaxCode,
                item.TaxName,
                TaxStatus = item.TaxStatus.GetDescription(),
                isExamine = item.TaxStatus == Needs.Wl.Models.Enums.ProductTaxStatus.Audited
            });

            return JsonResult(VueMsgType.success, "", new { list = list.ToArray(), total }.Json());
        }

        /// <summary>
        /// 自定义税务分部视图
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult _PartialProductTax()
        {
            var model = new ProductTaxViewModel();
            return PartialView(model);
        }

        /// <summary>
        /// 提交自定义税务
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult _partialproTaxInfo(ProductTaxViewModel model)
        {
            try
            {
                var user = Needs.Wl.User.Plat.UserPlat.Current;

                var tax = user.MyProductTaxCategories[model.ID] ?? new Needs.Wl.Models.ClientProductTaxCategory();
                tax.ClientID = user.Client.ID;
                tax.Name = model.Name;
                tax.Model = model.Models;
                tax.TaxCode = model.TaxCode;
                tax.TaxName = model.TaxName;
                tax.TaxStatus = Needs.Wl.Models.Enums.ProductTaxStatus.Auditing;
                tax.Enter();

                return base.JsonResult(VueMsgType.success, "操作成功");
            }
            catch (Exception ex)
            {
                ex.Log();
                return base.JsonResult(VueMsgType.error, "操作失败");
            }
        }

        /// <summary>
        /// 验证供品名是否重复
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult CheckProductName(string Name, string ID)
        {
            Name = Name.InputText();
            ID = ID.InputText();
            var view = Needs.Wl.User.Plat.UserPlat.Current.MyProductTaxCategories;
            view.Predicate = item => item.Name == Name && ((ID != "" && item.ID != ID) || (ID == "")) && item.TaxStatus != Needs.Wl.Models.Enums.ProductTaxStatus.NotPass;
            if (view.RecordCount > 0)
            {
                return base.JsonResult(VueMsgType.error, "该品名已存在！");
            }

            return base.JsonResult(VueMsgType.success, "");
        }

        /// <summary>
        ///  
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult GetProductTaxInfo(string id)
        {
            var model = new ProductTaxViewModel();
            var entity = Needs.Wl.User.Plat.UserPlat.Current.MyProductTaxCategories[id];
            if (entity != null)
            {
                model.ID = entity.ID;
                model.Name = entity.Name;
                model.Models = entity.Model;
                model.TaxCode = entity.TaxCode;
                model.TaxName = entity.TaxName;
                model.Summary = entity.Summary;
            }
            return base.JsonResult(VueMsgType.success, "", model.Json());
        }

        /// <summary>
        /// 删除自定义税务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult DelProductTax(string id)
        {
            try
            {
                var tax = Needs.Wl.User.Plat.UserPlat.Current.MyProductTaxCategories[id];
                tax.Abandon();
            }
            catch (Exception ex)
            {
                ex.Log();
                return base.JsonResult(VueMsgType.error, "删除失败");
            }

            return base.JsonResult(VueMsgType.success, "删除成功");
        }

        #endregion

        #region 产品预归类

        [UserActionFilter(UserAuthorize = true)]
        public ActionResult MyPreProducts()
        {
            return View();
        }

        /// <summary>
        /// 产品列表
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult GetPreProductsList()
        {
            var query = Request.Form["query"];  //查询数据          
            int page = int.Parse(Request.Form["page"]);
            int rows = int.Parse(Request.Form["rows"]);

            query = query.InputText();

            var productView = Needs.Wl.User.Plat.UserPlat.Current.MyClassifiedPreProducts;
            if (!string.IsNullOrWhiteSpace(query))
            {
                productView.Predicate = item => item.ProductName.ToUpper().Contains(query.ToUpper()) || item.Model.ToUpper().Contains(query.ToUpper()) || item.Manufacturer.ToUpper().Contains(query.ToUpper());
            }

            productView.PageIndex = page;
            productView.PageSize = rows;
            int total = productView.RecordCount;
            var list = productView.ToList().Select(item => new
            {
                item.ID,
                item.Manufacturer,
                item.Model,
                item.ProductName,
                ClassifyStatus = item.ClassifyStatus == null ? "" : item.ClassifyStatus.GetDescription(),
                item.Supplier,
                item.ProductUnionCode,
                Currency = item.Currency,
                CurrencyCode = item.Currency,
                IsCheck = false,
                IsClassified = item.ClassifyStatus == Needs.Wl.Models.Enums.ClassifyStatus.Done,
                IsNomarl = true,
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
                item.ExciseTaxRate,
                item.Price
            });

            return JsonResult(VueMsgType.success, "", new { list = list.ToArray(), total }.Json());
        }

        /// <summary>
        /// 删除预归类产品
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult DelPreProduct(string id)
        {
            try
            {
                var entity = Needs.Wl.User.Plat.UserPlat.Current.MyPreProducts[id.InputText()];
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
        /// 上传产品预归类
        /// </summary>
        /// <returns></returns>
        public JsonResult UploadPreProduct(HttpPostedFileBase file)
        {
            try
            {
                //拿到上传来的文件
                file = Request.Files["file"];
                string ext = Path.GetExtension(file.FileName).ToLower();
                string[] exts = { ".xls", ".xlsx" };
                if (exts.Contains(ext) == false)
                {
                    return base.JsonResult(VueMsgType.error, "文件格式错误，仅限.xls或.xlsx格式的文件。");
                }

                System.Data.DataTable dt = Needs.Wl.Web.Mvc.Utils.NPOIHelper.ExcelToDataTable(ext, file.InputStream, true);

                List<PreProcuctsData> list = new List<PreProcuctsData>();

                if (dt.Rows.Count == 0)
                {
                    return base.JsonResult(VueMsgType.error, "上传的文件错误，请下载模板，按模板要求填入正确的数据后上传。");
                }

                var currenciesView = Needs.Wl.User.Plat.UserPlat.Currencies;
                currenciesView.AllowPaging = false;
                var currencies = currenciesView.ToArray();

                for (var i = 0; i < dt.Rows.Count; i++)
                {
                    var array = dt.Rows[i].ItemArray;

                    PreProcuctsData preData = new PreProcuctsData
                    {
                        Name = array[0].ToString(),
                        Batch = array[1].ToString(),
                        Model = array[2].ToString(),
                        Manufacturer = array[3].ToString(),
                        UnitPrice = array[4].ToString(),
                        Currency = array[5].ToString(),
                        UniqueCode = array[6].ToString(),
                        DueDate = array[7].ToString(),
                        Qty = array[8].ToString(),
                    };

                    if (string.IsNullOrWhiteSpace(preData.Name))
                    {
                        return base.JsonResult(VueMsgType.error, string.Format("第{0}行数据产品名称不能为空", i + 3));
                    }
                    if (string.IsNullOrWhiteSpace(preData.Model))
                    {
                        return base.JsonResult(VueMsgType.error, string.Format("第{0}行数据型号不能为空", i + 3));
                    }
                    if (string.IsNullOrWhiteSpace(preData.Manufacturer))
                    {
                        return base.JsonResult(VueMsgType.error, string.Format("第{0}行数据品牌不能为空", i + 3));
                    }
                    if (string.IsNullOrWhiteSpace(preData.UnitPrice))
                    {
                        return base.JsonResult(VueMsgType.error, string.Format("第{0}行数据单价不能为空", i + 3));
                    }
                    if (string.IsNullOrWhiteSpace(preData.Currency))
                    {
                        return base.JsonResult(VueMsgType.error, string.Format("第{0}行数据币种不能为空", i + 3));
                    }

                    if (string.IsNullOrWhiteSpace(preData.UniqueCode))
                    {
                        return base.JsonResult(VueMsgType.error, string.Format("第{0}行数据物料号(产品唯一编码)不能为空", i + 3));
                    }

                    preData.Currency = preData.Currency.ToUpper();

                    var cyrrency = currencies.Where(item => item.ID == preData.Currency || item.Code == preData.Currency || item.Name == preData.Currency).FirstOrDefault();
                    if (cyrrency == null)
                    {
                        return base.JsonResult(VueMsgType.error, string.Format("第{0}行数据币种错误", i + 3));
                    }
                    else
                    {
                        preData.Currency = cyrrency.Code;
                    }

                    DateTime dtTry;
                    if (preData.DueDate != "")
                    {
                        if (!DateTime.TryParse(preData.DueDate, out dtTry))
                        {
                            return base.JsonResult(VueMsgType.error, string.Format("第{0}行预计到货日期格式错误", i + 3));
                        }
                    }

                    decimal intTry;
                    if (preData.Qty != "")
                    {
                        if (!decimal.TryParse(preData.Qty, out intTry))
                        {
                            return base.JsonResult(VueMsgType.error, string.Format("第{0}行数据格式错误", i + 3));
                        }
                    }

                    list.Add(preData);
                }

                list.Reverse();

                var client = Needs.Wl.User.Plat.UserPlat.Current.Client;
                var userID = Needs.Wl.User.Plat.UserPlat.Current.ID;
                foreach (var item in list)
                {
                    var product = new Needs.Wl.Models.ClientProducts();
                    product.Batch = item.Batch;
                    product.Manufacturer = item.Manufacturer;
                    product.Model = item.Model;
                    product.Name = item.Name;
                    product.UnitPrice = decimal.Parse(item.UnitPrice.Trim());
                    product.ClientID = client.ID;
                    product.Enter();

                    var preProduct = new PreProduct();
                    preProduct.ClientID = client.ID;
                    preProduct.ProductUnionCode = item.UniqueCode;
                    preProduct.Model = item.Model;
                    preProduct.Manufacturer = item.Manufacturer;
                    if (item.Qty != "")
                    {
                        preProduct.Qty = Convert.ToDecimal(item.Qty);
                    }
                    preProduct.Price = decimal.Parse(item.UnitPrice.Trim());
                    preProduct.Currency = item.Currency;
                    preProduct.CompanyType = client.ClientType == Needs.Wl.Models.Enums.ClientType.External ? CompanyTypeEnums.OutSide : CompanyTypeEnums.Icgoo;
                    if (item.DueDate != "")
                    {
                        preProduct.DueDate = Convert.ToDateTime(item.DueDate);
                    }

                    preProduct.UseType = PreProductUserType.Pre;

                    preProduct.Enter();
                }
            }
            catch (Exception ex)
            {
                ex.Log();
                return base.JsonResult(VueMsgType.error, ex.Message);
            }
            return base.JsonResult(VueMsgType.success, "导入成功");
        }

        #endregion
    }
}