//using Newtonsoft.Json;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.IO;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Reflection;
//using System.Web;
//using System.Web.Mvc;
//using Yahv.PvWsClient.WebApp.App_Utils;
//using Yahv.PvWsClient.WebApp.Models;
//using Yahv.PvWsOrder.Services.ClientModels;
//using Yahv.PvWsOrder.Services.ClientModels.Client;
//using Yahv.PvWsOrder.Services.Enums;
//using Yahv.PvWsOrder.Services.Extends;
//using Yahv.PvWsOrder.Services.Views;
//using Yahv.PvWsOrder.Services.XDTClientView;
//using Yahv.Services.Models.LsOrder;
//using Yahv.Services.Views;
//using Yahv.Underly;
//using Yahv.Utils.Serializers;


//namespace Yahv.PvWsClient.WebApp.Controllers
//{
//    /// <summary>
//    /// 产品
//    /// </summary>
//    public class ProductsController : UserController
//    {
//        #region 我的产品

//        /// <summary>
//        /// 我的产品
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult MyProducts()
//        {
//            return View();
//        }

//        /// <summary>
//        /// 获取我的产品
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult GetMyProducts()
//        {
//            var query = Request.Form["query"];  //查询数据          
//            var product = Client.Current.MyClientProducts.AsQueryable();
//            if (!string.IsNullOrWhiteSpace(query))
//            {
//                product= product.Where(item => item.Name.ToUpper().Contains(query.ToUpper()) || item.Model.ToUpper().Contains(query.ToUpper()) || item.Manufacturer.ToUpper().Contains(query.ToUpper()));
//            }
//            #region 页面数据
//            Func<Services.Models.ClientProduct, object> convert = item => new 
//            {
//                item.ID,
//                item.Name,
//                item.Model,
//                item.Manufacturer,
//                item.Batch
//            };
//            #endregion
//            return this.Paging(product, convert);
//        }

//        /// <summary>
//        /// 产品分部视图
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult _PartialProduct()
//        {
//            var model = new MyProductsViewModel();
//            return PartialView(model);
//        }

//        /// <summary>
//        /// 提交产品
//        /// </summary>
//        /// <returns></returns>
//        [HttpPost]
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult _PartialProduct(MyProductsViewModel model)
//        {
//            try
//            {
//                var user = Client.Current;
//                var product =user.MyClientProducts[model.ID]?? new Services.Models.ClientProduct();
//                product.ID = model.ID;
//                product.ClientID = Client.Current.XDTClientID;
//                product.Batch = model.Batch;
//                product.Manufacturer = model.Manufacturer;
//                product.Model = model.Models;
//                product.Name = model.Name;
//                user.MyClientProducts.Enter(product);
//                return base.JsonResult(VueMsgType.success, "操作成功");
//            }
//            catch (Exception ex)
//            {
//                return base.JsonResult(VueMsgType.error, ex.Message);
//            }
//        }

//        /// <summary>
//        ///  获取产品信息
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult GetProductInfo(string id)
//        {
//            var model = new MyProductsViewModel();
//            var product =Client.Current.MyClientProducts[id];
//            if (product != null)
//            {
//                model.ID = product.ID;
//                model.Name = product.Name;
//                model.Models = product.Model;
//                model.Manufacturer = product.Manufacturer;
//                model.Batch = product.Batch;
//            }
//            return base.JsonResult(VueMsgType.success, "", model.Json());
//        }

//        /// <summary>
//        /// 删除产品
//        /// </summary>
//        /// <param name="id"></param>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult DelProduct(string id)
//        {
//            try
//            {
//                var user = Client.Current;
//                var pro = user.MyClientProducts[id];
//                if (pro == null)
//                {
//                    return base.JsonResult(VueMsgType.error, "删除失败");
//                }
//                user.MyClientProducts.Abandon(pro);
//            }
//            catch (Exception ex)
//            {
//                return base.JsonResult(VueMsgType.error, "删除失败");
//            }

//            return base.JsonResult(VueMsgType.success, "删除成功");
//        }

//        /// <summary>
//        /// 加入预归类分部视图
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult _PartialPreProduct()
//        {
//            var model = new PreProductInfoViewModel();
//            var data = new
//            {
//                CurrencyOptions = ExtendsEnum.ToDictionary<Currency>().Where(item => item.Key != "0" ).Select(item => new { value = item.Key, text = item.Value }).ToArray(),
//            };
//            ViewBag.Options = data;
//            return PartialView(model);
//        }

//        /// <summary>
//        /// 提交预归类
//        /// </summary>
//        /// <returns></returns>
//        [HttpPost]
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult _PartialPreProduct(PreProductInfoViewModel model)
//        {
//            try
//            {
//                var client = Client.Current;
//                var product = new PreProduct();
//                product.ClientID = client.XDTClientID;
//                product.Currency = model.Currency;
//                product.ProductName = model.ProductName;
//                product.Manufacturer = model.Manufacturer;
//                product.Model = model.Models;
//                product.ProductUnionCode = model.ProductUnionCode;
//                product.CompanyType = client.XDTClientType == PvWsOrder.Services.Enums.ClientType.External ? CompanyTypeEnums.OutSide : CompanyTypeEnums.Icgoo;
//                product.Price = model.Price.Value;
//                client.MyPreProducts.Enter(product);
//                //product.Enter();

//                return base.JsonResult(VueMsgType.success, "操作成功");
//            }
//            catch (Exception ex)
//            {
//                return base.JsonResult(VueMsgType.error, ex.Message);
//            }
//        }

//        /// <summary>
//        /// 验证产品识别是否重复
//        /// </summary>
//        /// <param name="ProductUnionCode"></param>
//        /// <param name="ID"></param>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult CheckProductUnionCode(string ProductUnionCode, string ID)
//        {
//            ProductUnionCode = ProductUnionCode.InputText();
//            ID = ID.InputText();
//            var view = Client.Current.MyClassifiedPreProducts.ToList().Where(item => item.ClassifyStatus != ClassifyStatus.Anomaly && item.ProductUnionCode == ProductUnionCode && ((ID != "" && item.ID != ID) || (ID == "")));
//            if (view.Count() > 0)
//            {
//                return base.JsonResult(VueMsgType.error, "物料号重复");
//            }
//            return base.JsonResult(VueMsgType.success, "");
//        }

//        /// <summary>
//        /// 我的产品的分部视图
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult _PartialMyPreProducts()
//        {
//            var product =Client.Current.MyClientProducts.ToList().Select(item => new
//            {
//                item.ID,
//                item.Name,
//                item.Model,
//                item.Manufacturer,
//                item.Batch,
//            });
//            return PartialView(product);
//        }
//        #endregion

//        #region 我的预归类
//        /// <summary>
//        /// 我的预归类
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult MyPreProducts()
//        {
//            return View();
//        }

//        /// <summary>
//        /// 产品预归类列表
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult GetPreProductsList()
//        {
//            var query = Request.Form["query"];  //查询数据          
//            var product = Client.Current.MyClassifiedPreProducts;

//            List<LambdaExpression> lambdas = new List<LambdaExpression>();
//            Expression<Func<ClassifiedPreProduct, bool>> lambda = item => true;
//            if (!string.IsNullOrWhiteSpace(query))
//            {
//                lambda = item => item.ProductName.ToUpper().Contains(query.ToUpper()) || item.Model.ToUpper().Contains(query.ToUpper()) || item.Manufacturer.ToUpper().Contains(query.ToUpper());
//                lambdas.Add(lambda);
//            }
//            #region 页面数据
//            int page = int.Parse(Request.Form["page"]);
//            int rows = int.Parse(Request.Form["rows"]);
//            var list = product.GetPageList(lambdas.ToArray(), rows, page);
//            Func<ClassifiedPreProduct, object> convert = item => new
//            {
//                item.ID,
//                item.Manufacturer,
//                item.Model,
//                item.ProductName,
//                ClassifyStatus = item.ClassifyStatus == null ? "" : item.ClassifyStatus.GetDescription(),
//                item.Supplier,
//                item.ProductUnionCode,
//                Currency = item.Currency,
//                CurrencyCode = item.Currency,
//                IsCheck = false,
//                IsClassified = item.ClassifyStatus == ClassifyStatus.Done,
//                IsUnClassified= item.ClassifyStatus == ClassifyStatus.Unclassified,
//                IsNomarl = !((item.Type & ItemCategoryType.HKForbid) > 0 || (item.Type & ItemCategoryType.Forbid) > 0),
//                Classified = item.Type == null ? "" : item.Type.GetFlagsDescriptions("|"),
//                ClassifiedLabel = item.Type == null ? "" : item.Type.GetFlagsDescriptions("|"),
//                UniqueCode = item.ProductUnionCode,
//                item.CIQCode,
//                item.Elements,
//                item.HSCode,
//                item.TaxCode,
//                item.TariffRate,
//                item.Unit1,
//                item.Unit2,
//                item.TaxName,
//                item.AddedValueRate,
//                item.Price
//            };
//            #endregion
//            return this.Paging(list, list.Total, convert);
//        }

//        /// <summary>
//        /// 删除预归类产品
//        /// </summary>
//        /// <param name="id"></param>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult DelPreProduct(string id)
//        {
//            try
//            {
//                var entity = Client.Current.MyPreProducts[id.InputText()];
//                entity.Abandon();
//            }
//            catch (Exception ex)
//            {
//                return base.JsonResult(VueMsgType.error, "删除失败");
//            }
//            return base.JsonResult(VueMsgType.success, "删除成功");
//        }

//        /// <summary>
//        /// 上传产品预归类
//        /// </summary>
//        /// <returns></returns>
//        public JsonResult UploadPreProduct(HttpPostedFileBase file)
//        {
//            try
//            {
//                var client = Client.Current;
//                //拿到上传来的文件
//                file = Request.Files["file"];
//                string ext = Path.GetExtension(file.FileName).ToLower();
//                string[] exts = { ".xls", ".xlsx" };
//                if (exts.Contains(ext) == false)
//                {
//                    return base.JsonResult(VueMsgType.error, "文件格式错误，仅限.xls或.xlsx格式的文件。");
//                }

//                System.Data.DataTable dt = NPOIHelper.ExcelToDataTable(ext, file.InputStream, true);

//                List<PreProcuctsData> list = new List<PreProcuctsData>();

//                if (dt.Rows.Count == 0)
//                {
//                    return base.JsonResult(VueMsgType.error, "上传的文件错误，请下载模板，按模板要求填入正确的数据后上传。");
//                }

//                var currenciesView = Alls.Current.Currency;
//                for (var i = 0; i < dt.Rows.Count; i++)
//                {
//                    var array = dt.Rows[i].ItemArray;

//                    PreProcuctsData preData = new PreProcuctsData
//                    {
//                        Name = array[0].ToString(),
//                        Batch = array[1].ToString(),
//                        Model = array[2].ToString(),
//                        Manufacturer = array[3].ToString(),
//                        UnitPrice = array[4].ToString(),
//                        Currency = array[5].ToString(),
//                        UniqueCode = array[6].ToString(),
//                        DueDate = array[7].ToString(),
//                        Qty = array[8].ToString(),
//                    };

//                    if (string.IsNullOrWhiteSpace(preData.Name))
//                    {
//                        return base.JsonResult(VueMsgType.error, string.Format("第{0}行数据产品名称不能为空", i + 3));
//                    }
//                    if (string.IsNullOrWhiteSpace(preData.Model))
//                    {
//                        return base.JsonResult(VueMsgType.error, string.Format("第{0}行数据型号不能为空", i + 3));
//                    }
//                    if (string.IsNullOrWhiteSpace(preData.Manufacturer))
//                    {
//                        return base.JsonResult(VueMsgType.error, string.Format("第{0}行数据品牌不能为空", i + 3));
//                    }
//                    if (string.IsNullOrWhiteSpace(preData.UnitPrice))
//                    {
//                        return base.JsonResult(VueMsgType.error, string.Format("第{0}行数据单价不能为空", i + 3));
//                    }
//                    if (string.IsNullOrWhiteSpace(preData.Currency))
//                    {
//                        return base.JsonResult(VueMsgType.error, string.Format("第{0}行数据币种不能为空", i + 3));
//                    }

//                    if (string.IsNullOrWhiteSpace(preData.UniqueCode))
//                    {
//                        return base.JsonResult(VueMsgType.error, string.Format("第{0}行数据物料号(产品唯一编码)不能为空", i + 3));
//                    }

//                    preData.Currency = preData.Currency.ToUpper();

//                    var cyrrency = currenciesView.Where(item => item.ID == preData.Currency || item.Code == preData.Currency || item.Name == preData.Currency).FirstOrDefault();
//                    if (cyrrency == null)
//                    {
//                        return base.JsonResult(VueMsgType.error, string.Format("第{0}行数据币种错误", i + 3));
//                    }
//                    else
//                    {
//                        preData.Currency = cyrrency.Code;
//                    }

//                    DateTime dtTry;
//                    if (preData.DueDate != "")
//                    {
//                        if (!DateTime.TryParse(preData.DueDate, out dtTry))
//                        {
//                            return base.JsonResult(VueMsgType.error, string.Format("第{0}行预计到货日期格式错误", i + 3));
//                        }
//                    }

//                    decimal intTry;
//                    if (preData.Qty != "")
//                    {
//                        if (!decimal.TryParse(preData.Qty, out intTry))
//                        {
//                            return base.JsonResult(VueMsgType.error, string.Format("第{0}行数据格式错误", i + 3));
//                        }
//                    }

//                    list.Add(preData);
//                }

//                list.Reverse();
//                List<PreProduct> preProducts = new List<PreProduct>();
//                foreach (var item in list)
//                {
//                    var product = new Services.Models.ClientProduct();
//                    product.Batch = item.Batch;
//                    product.Manufacturer = item.Manufacturer;
//                    product.Model = item.Model;
//                    product.Name = item.Name;
//                    product.ClientID = client.XDTClientID;
//                    client.MyClientProducts.Enter(product);

//                    var preProduct = new PreProduct();
//                    preProduct.ProductName = item.Name;
//                    preProduct.ClientID = client.XDTClientID;
//                    preProduct.ProductUnionCode = item.UniqueCode;
//                    preProduct.Model = item.Model;
//                    preProduct.Manufacturer = item.Manufacturer;
//                    if (item.Qty != "")
//                    {
//                        preProduct.Qty = Convert.ToDecimal(item.Qty);
//                    }
//                    preProduct.Price = decimal.Parse(item.UnitPrice.Trim());
//                    preProduct.Currency = item.Currency;
//                    preProduct.CompanyType = client.XDTClientType == PvWsOrder.Services.Enums.ClientType.External ? CompanyTypeEnums.OutSide : CompanyTypeEnums.Icgoo;
//                    if (item.DueDate != "")
//                    {
//                        preProduct.DueDate = Convert.ToDateTime(item.DueDate);
//                    }
//                    preProducts.Add(preProduct);
//                    //preProduct.Enter();
//                }
//                client.MyPreProducts.ExcelEnter(preProducts.ToArray());
//            }
//            catch (Exception ex)
//            {
//                return base.JsonResult(VueMsgType.error, ex.Message);
//            }
//            return base.JsonResult(VueMsgType.success, "导入成功");
//        }
//        #endregion
//    }
//}