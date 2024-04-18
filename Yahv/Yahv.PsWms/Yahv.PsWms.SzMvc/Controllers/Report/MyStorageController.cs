using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Yahv.PsWms.SzMvc.Models;
using Yahv.PsWms.SzMvc.Services.Views;
using Yahv.Underly;

namespace Yahv.PsWms.SzMvc.Controllers
{
    public partial class ReportController : BaseController
    {
        #region 页面

        /// <summary>
        /// 我的库存
        /// </summary>
        /// <returns></returns>
        public ActionResult MyStorage() { return View(); }

        #endregion

        /// <summary>
        /// 获取我的库存列表数据
        /// </summary>
        /// <returns></returns>
        public JsonResult GetMyStorageList(GetMyStorageListSearchModel searchModel)
        {
            var siteuser = Yahv.PsWms.SzMvc.SiteCoreInfo.Current;
            string theClientID = siteuser.TheClientID;

            using (var query = new StorageListView())
            {
                var view = query;

                view = view.SearchByClientID(theClientID);

                if (!string.IsNullOrEmpty(searchModel.PartNumber))
                {
                    view = view.SearchByPartnumber(searchModel.PartNumber);
                }

                if (!string.IsNullOrEmpty(searchModel.Brand))
                {
                    view = view.SearchByBrand(searchModel.Brand);
                }

                if (!string.IsNullOrEmpty(searchModel.Code))
                {
                    view = view.SearchByCode(searchModel.Code);
                }

                view = view.SearchByPackageNumberIsNotZero();

                Func<StorageListViewModel, StorageListModel> convert = item => new StorageListModel
                {
                    StorageID = item.StorageID,
                    CustomCode = item.CustomCode,
                    PartNumber = item.Partnumber,
                    Brand = item.Brand,
                    Package = item.Package,
                    DateCode = item.DateCode,
                    StocktakingTypeInt = (int)item.StocktakingType,
                    StocktakingTypeName = item.StocktakingType.GetDescription(),
                    Mpq = item.Mpq,
                    PackageNumber = item.PackageNumber - (item.Ex_PackageNumber ?? 0),
                    LocationNo = item.Code,

                    ItemTotal = item.Mpq * (item.PackageNumber - (item.Ex_PackageNumber ?? 0)),
                    IsCheck = false,
                    NeedPackageNumber = item.PackageNumber - (item.Ex_PackageNumber ?? 0),
                };

                var viewData = view.ToMyPage(convert, searchModel.page, searchModel.rows);

                return Json(new { type = "success", msg = "", data = new { list = viewData.Item1, total = viewData.Item2 } }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}