//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using WebApp.Services;
//using Yahv.Underly;
//using Yahv.Underly.Attributes;
//using Yahv.Utils.Serializers;
//using Wms.Services;
//using Layers.Data;
//using System.Transactions;
//using Wms.Services.Models;

//namespace MvcApi.Controllers
//{
//    public class NoticeEnterController : Controller
//    {
//        [HttpPost]
//        public ActionResult Index()
//        {

//            try
//            {

//                Byte[] tmpb = Request.BinaryRead(Request.TotalBytes);
//                var val = System.Text.Encoding.UTF8.GetString(tmpb);
//                var entity = val.JsonTo<WebApp.Services.NoticeEnter>();

//                using (TransactionScope trans = new TransactionScope())
//                {

//                    try
//                    {
//                        foreach (var item in entity.Notices)
//                        {


//                            //进项信息
//                            Inputs input = null;
//                            var inputID = "";
//                            if (item.Input != null)
//                            {
//                                var ipt = item.Input;
//                                input = new Inputs { EnterpriseID = ipt.EnterpriseID, Code = ipt.Code, OrderID = ipt.OrderID, Currency = ipt.Currency, DateCode = ipt.DateCode, ItemID = ipt.ItemID, Origin = ipt.Origin, ProductID = ipt.ProductID, PurchaserID = ipt.PurchaserID, SalerID = ipt.SalerID, TrackerID = ipt.TrackerID, UnitPrice = ipt.UnitPrice, CreateDate = DateTime.Now };
//                                input.Enter();
//                                inputID = input.ID;
//                            }


//                            Outputs output = null;
//                            var outputID = "";
//                            if (item.Output != null)
//                            {

//                                var opt = item.Output;
//                                output = new Outputs { InputID = inputID, ItemID = opt.ItemID, OrderID = opt.OrderID, CustomerServiceID = opt.CustomerServiceID, OwnerID = opt.OwnerID, Currency = opt.Currency, Price = opt.Price, PurchaserID = opt.PurchaserID, SalerID = opt.SalerID };
//                                output.Enter();
//                                outputID = output.ID;
//                            }

//                            var productID = "";
//                            if (item.Product != null)
//                            {
//                                var p = item.Product;
//                                var product = new StandardProduct { Catalog = p.Catalog, PartNumber = p.PartNumber, Manufacturer = p.Manufacturer, Packing = p.Packing, PackageCase = p.PackageCase, UnitGrossVolume = p.UnitGrossVolume, UnitGrossWeightBL = p.UnitGrossWeightBL, UnitGrossWeightTL = p.UnitGrossWeightTL };
//                                product.Enter();
//                                productID = product.ID;
//                            }

//                            var noticeid = "";
//                            var notice = new Notices { Type = item.Type, WareHouseID = item.WareHouseID, Conditions = item.Conditions, DateCode = item.DateCode, Quantity = item.Quantity, Source = item.Source, Supplier = item.Supplier, Target = item.Target, Volume = item.Volume, Weight = item.Weight, WaybillID = item.WaybillID, BoxCode = "", InputID = inputID, OutputID = inputID, ProductID = productID, ShelveID = "" };
//                            notice.Enter();
//                            noticeid = notice.ID;

//                            if (item.Files != null)
//                            {
//                                foreach (var tem in item.Files)
//                                {
//                                    var newFile = new FileInfo { NoticeID = noticeid, InputID = inputID, AdminID = tem.AdminID, ClientID = tem.ClientID, CustomName = tem.CustomName, StorageID = "", Type = tem.Type, Url = tem.Url };
//                                    newFile.Enter();

//                                }
//                            }


//                        }


//                        trans.Complete();

//                        return Json(new JMessage { success = true, code = 200, data = "提交成功！" }, JsonRequestBehavior.AllowGet);
//                    }
//                    catch
//                    {
//                        return Json(new JMessage { success = true, code = 300, data = "提交失败！" }, JsonRequestBehavior.AllowGet);
//                    }

//                }


//            }
//            catch(Exception ex)
//            {
//                return Json(new JMessage { success = true, code = 300, data = "提交失败！" }, JsonRequestBehavior.AllowGet);
//            }
//        }
//    }
//}