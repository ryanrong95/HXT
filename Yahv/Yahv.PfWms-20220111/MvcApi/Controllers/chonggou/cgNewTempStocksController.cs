using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using Wms.Services.chonggous.Views;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Mvc;
using static Wms.Services.chonggous.Views.CgNewTempStocksView;

namespace MvcApi.Controllers.chonggou
{
    public class cgNewTempStocksController : Controller
    {
        /// <summary>
        /// 根据前端的过滤条件,获取对应的已有暂存运单
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Show(JPost jpost)
        {
            var arguments = new
            {
                whid = jpost["WhID"]?.Value<string>(),
                code = jpost["Code"]?.Value<string>(),
                carrierid = jpost["CarrierID"]?.Value<string>(),
                shelveid = jpost["ShelveID"]?.Value<string>(),
                starttime = jpost["StartTime"]?.Value<string>(),
                endtime = jpost["EndTime"]?.Value<string>(),
                status = jpost["Status"]?.Value<int?>(),
                pageIndex = jpost["PageIndex"]?.Value<int?>() ?? 1,
                pageSize = jpost["PageSize"]?.Value<int?>() ?? 20,
            };

            var data = new CgNewTempStocksView();
            if (!string.IsNullOrEmpty(arguments.whid))
            {
                data = data.SearchByWareHouseID(arguments.whid);
            }
            if (!string.IsNullOrEmpty(arguments.code))
            {
                data = data.SearchByWaybillCode(arguments.code);
            }
            if (!string.IsNullOrEmpty(arguments.carrierid))
            {
                data = data.SearchByCarrier(arguments.carrierid);
            }
            if (!string.IsNullOrEmpty(arguments.shelveid))
            {
                data = data.SearchByShelveID(arguments.shelveid);
            }

            if (!string.IsNullOrEmpty(arguments.starttime))
            {
                if (!string.IsNullOrEmpty(arguments.endtime))
                {
                    data = data.SearchByCreateTime(DateTime.Parse(arguments.starttime), DateTime.Parse(arguments.endtime));
                }
                else
                {
                    var time = DateTime.Now;
                    data = data.SearchByCreateTime(DateTime.Parse(arguments.starttime), new DateTime(time.Year, time.Month, time.Day));
                }
            }

            if (arguments.status.HasValue)
            {
                data = data.SearchByStatus((TempStockStatus)arguments.status.Value);
            }

            var result = data.ToMyPage(arguments.pageIndex, arguments.pageSize);
            return Json(new { obj = result }, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 获取具体的已有暂存运单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Detail(string id)
        {
            var data = new CgNewTempStocksView().SearchByWaybillID(id).ToMyArray();
            return Json(data[0], JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 新增及修改暂存
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EnterForPDA(JPost jpost)
        {
            try
            {
                CgNewTempStocksView tempView = new CgNewTempStocksView();
                tempView.Enter(jpost);

                return Json(new
                {
                    Success = true,
                    Data = string.Empty
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Success = false,
                    Data = ex.Message,
                });
            }
        }

        /// <summary>
        /// 接收上传过来的单个文件
        /// </summary>
        /// <param name="forms"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EnterFileForPDA(FormCollection forms)
        {
            try
            {
                var query = Request.QueryString;
                var adminID = query["AdminID"];
                string uploadPath = HostingEnvironment.MapPath(@"~/Upload");
                DirectoryInfo di = new DirectoryInfo(uploadPath);
                if (!di.Exists)
                {
                    di.Create();
                }
                CgNewTempStocksView view = new CgNewTempStocksView();

                var file = Request.Files[0];
                string fileNameWithTimeStr = Path.GetFileNameWithoutExtension(file.FileName) + "_" + DateTime.Now.ToString("yyyyMMdd_hhmmss");
                string strExtension = Path.GetExtension(file.FileName);
                //string srcFileName = fileNameWithTimeStr + "_s" + strExtension;
                //string destFileName = fileNameWithTimeStr + strExtension;
                //string srcFullFileName = Path.Combine(di.FullName, srcFileName);
                //string destFullFileName = Path.Combine(di.FullName, destFileName);

                //FileInfo fi = new FileInfo(srcFullFileName);

                //file.SaveAs(fi.FullName);
                //view.CompressImage(srcFullFileName, destFullFileName);
                //fi.Delete();
                
                
                string destFileName = fileNameWithTimeStr + strExtension;
                string destFullFileName = Path.Combine(di.FullName, destFileName);

                FileInfo fi = new FileInfo(destFullFileName);
                file.SaveAs(fi.FullName);                

                string result = view.UploadFile(destFullFileName, adminID, null);
                var parseResult = result.JsonTo<List<FileDescrition>>()[0];
                return Json(new
                {
                    code = 200,
                    success = true,
                    data = new
                    {
                        ID = parseResult.FileID,
                        //parseResult.FileName,
                        file.FileName,
                        parseResult.SessionID,
                        parseResult.Url,
                    }
                }, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Json(new
                {
                    code = 400,
                    success = false,
                    data = msg
                });
            }
        }

        /// <summary>
        /// 接收上传过来的多个文件
        /// </summary>
        /// <param name="forms"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EnterFilesForPDA(FormCollection forms)
        {
            try
            {
                var query = Request.QueryString;
                var adminID = query["AdminID"];
                var results = new List<object>();
                string uploadPath = HostingEnvironment.MapPath(@"~/Upload");
                DirectoryInfo di = new DirectoryInfo(uploadPath);
                if (!di.Exists)
                {
                    di.Create();
                }
                CgNewTempStocksView view = new CgNewTempStocksView();
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var file = Request.Files[i];
                    string fileNameWithTimeStr = Path.GetFileNameWithoutExtension(file.FileName) + "_" + DateTime.Now.ToString("yyyyMMdd_hhmmss");
                    string strExtension = Path.GetExtension(file.FileName);
                    //string srcFileName = fileNameWithTimeStr + "_s" + strExtension;
                    //string destFileName = fileNameWithTimeStr + strExtension;
                    //string srcFullFileName = Path.Combine(di.FullName, srcFileName);
                    //string destFullFileName = Path.Combine(di.FullName, destFileName);

                    //FileInfo fi = new FileInfo(srcFullFileName);

                    //file.SaveAs(fi.FullName);
                    //view.CompressImage(srcFullFileName, destFullFileName);
                    //fi.Delete();
                      
                    string destFileName = fileNameWithTimeStr + strExtension;
                    string destFullFileName = Path.Combine(di.FullName, destFileName);

                    FileInfo fi = new FileInfo(destFullFileName);
                    file.SaveAs(fi.FullName);
                    //view.CompressImage(destFullFileName, destFullFileName);

                    string result = view.UploadFile(destFullFileName, adminID, null);
                    var parseResult = result.JsonTo<List<FileDescrition>>()[0];

                    results.Add(new
                    {
                        ID = parseResult.FileID,
                        //parseResult.FileName,
                        file.FileName,
                        parseResult.SessionID,
                        parseResult.Url
                    });
                }

                return Json(new
                {
                    code = 200,
                    success = true,
                    data = results.ToArray(),
                }, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Json(new
                {
                    code = 400,
                    success = false,
                    data = msg,
                });
            }
        }

        /// <summary>
        /// 获取库房信息
        /// </summary>
        /// <returns></returns>
        public ActionResult GetWarehouseInfos()
        {
            var data = new CgNewTempStocksView().GetWarehouseInfos();
            return Json(new
            {
                code = 200,
                success = true,
                data = data,
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 新建时获取WaybillID
        /// </summary>
        /// <returns></returns>
        public ActionResult GetWaybillID()
        {
            string WaybillID = new CgNewTempStocksView().GetWaybillID();
            return Json(new
            {
                code = 200,
                success = true,
                data = WaybillID,
            }, JsonRequestBehavior.AllowGet);
        }
                
        /// <summary>
        /// 同时删除多个文件
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteFile(JPost jpost)
        {
            var ids = jpost["ID"].Values<string>();            
            try
            {
                new CgNewTempStocksView().DeleteFiles(ids.ToArray());
                return Json(new JMessage
                {
                    success = true,
                    code = 200,
                    data = "",
                }, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                return Json(new JMessage
                {
                    success = false,
                    code = 400,
                    data = ex.Message,
                }, JsonRequestBehavior.DenyGet);
            }            
        }

        /// <summary>
        /// 更新暂存状态及OrderID
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateTWaybills(JPost jpost)
        {
            var arguments = new
            {
                orderID = jpost["OrderID"]?.Value<string>(),
                waybillID = jpost["WaybillID"]?.Value<string>(),
                adminID = jpost["AdminID"]?.Value<string>(),
            };

            var view = new CgNewTempStocksView();

            if (string.IsNullOrEmpty(arguments.orderID) || string.IsNullOrEmpty(arguments.waybillID))
            {
                return Json(new
                {
                    code = 400,
                    success = false,
                    data = "OrderID 及 WaybillID不能为空"

                }, JsonRequestBehavior.DenyGet);
            }

            view.UpdateTWaybills(arguments.waybillID, arguments.orderID, arguments.adminID);

            return Json(new
            {
                code = 200,
                success = true,
                data = "",
            }, JsonRequestBehavior.DenyGet);
        }
    }
}