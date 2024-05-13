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
using Yahv.Web.Mvc.Filters;
using static Wms.Services.chonggous.Views.CgTempStocksView;

namespace MvcApi.Controllers.chonggou
{
    public class cgTempStocksController : Controller
    {
        /// <summary>
        /// 已有暂存运单展示
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult Show(string id, int pageIndex = 1, int pageSize = 20)
        {
            var data = new CgTempStocksView().SearchByWareHouseID(id).ToMyPage(pageIndex, pageSize);
            return Json(new { obj = data}, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据前端的过滤条件,获取对应的已有暂存运单
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        [HttpPayload]
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
                pageIndex = jpost["PageIndex"]?.Value<int?>() ?? 1,
                pageSize = jpost["PageSize"]?.Value<int?>() ?? 20,
                partNumber = jpost["PartNumber"]?.Value<string>(),
            };

            var data = new CgTempStocksView();
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
            if (!string.IsNullOrEmpty(arguments.partNumber))
            {
                data = data.SearchByPartNumber(arguments.partNumber);
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
            var data = new CgTempStocksView().SearchByWaybillID(id).ToMyArray();
            return Json(data[0], JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 暂存录入及修改
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPayload]
        public ActionResult Enter(JPost jpost)
        {
            try
            {
                CgTempStocksView tempView = new CgTempStocksView();
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
        /// 新增及修改暂存
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EnterForPDA(JPost jpost)
        {
            try
            {
                CgTempStocksView tempView = new CgTempStocksView();
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
                CgTempStocksView view = new CgTempStocksView();               

                var file = Request.Files[0];
                string fileNameWithTimeStr = Path.GetFileNameWithoutExtension(file.FileName) + "_"+ DateTime.Now.ToString("yyyyMMdd_hhmmss");
                string strExtension = Path.GetExtension(file.FileName);
                string srcFileName = fileNameWithTimeStr + "_s" + strExtension;
                string destFileName = fileNameWithTimeStr + strExtension;
                string srcFullFileName = Path.Combine(di.FullName, srcFileName);
                string destFullFileName = Path.Combine(di.FullName, destFileName);

                FileInfo fi = new FileInfo(srcFullFileName);

                file.SaveAs(fi.FullName);
                view.CompressImage(srcFullFileName, destFullFileName);
                fi.Delete();

                string result = view.UploadFile(destFullFileName, adminID, null);
                var parseResult = result.JsonTo<List<FileDescrition>>()[0];
                return Json(new
                {
                    code = 200,
                    success = true,
                    data = new
                    {
                        ID = parseResult.FileID,
                        parseResult.FileName,
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
                CgTempStocksView view = new CgTempStocksView();
                for (int i=0; i<Request.Files.Count;i++)
                {
                    var file = Request.Files[i];
                    string fileNameWithTimeStr = Path.GetFileNameWithoutExtension(file.FileName) + "_" + DateTime.Now.ToString("yyyyMMdd_hhmmss");
                    string strExtension = Path.GetExtension(file.FileName);
                    string srcFileName = fileNameWithTimeStr + "_s" + strExtension;
                    string destFileName = fileNameWithTimeStr + strExtension;                    
                    string srcFullFileName = Path.Combine(di.FullName, srcFileName);
                    string destFullFileName = Path.Combine(di.FullName, destFileName);

                    FileInfo fi = new FileInfo(srcFullFileName);

                    file.SaveAs(fi.FullName);
                    view.CompressImage(srcFullFileName, destFullFileName);
                    fi.Delete();

                    string result = view.UploadFile(destFullFileName, adminID, null);
                    var parseResult = result.JsonTo<List<FileDescrition>>()[0];

                    results.Add(new {
                        ID = parseResult.FileID,
                        parseResult.FileName,
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
                    success =false,
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
            var data = new CgTempStocksView().GetWarehouseInfos();
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
            string WaybillID = new CgTempStocksView().GetWaybillID();
            return Json(new
            {
                code = 200,
                success = true,
                data = WaybillID,
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="fileID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteFile(string id)
        {
            var data = new CgTempStocksView().DeleteFile(id);
            return Json(data.JsonTo<JMessage>(), JsonRequestBehavior.DenyGet);
        }
    }
}