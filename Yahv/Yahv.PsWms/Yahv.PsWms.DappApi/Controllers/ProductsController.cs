using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yahv.Web.Mvc;

namespace Yahv.PsWms.DappApi.Controllers
{
    /// <summary>
    /// 产品接口
    /// </summary>
    public class ProductsController : Controller
    {
        [HttpPost]
        public ActionResult Enter(JPost jpost)
        {
            try
            {
                var view = new Services.Views.ProductsView();
                view.Enter(jpost);

                Response.StatusCode = 200;
                return Json(new { Success = true, Data = string.Empty });
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new { Success = false, Data = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult Delete(string id)
        {
            try
            {
                var view = new Services.Views.ProductsView();
                view.Delete(id);

                var noticeitems = new Services.Views.NoticeItemsView().ToArray();
                var notices = new  Services.Views.NoticesView().ToArray();

                var items = from entity in noticeitems
                            join notice in notices on entity.NoticeID equals notice.ID
                            where notice.NoticeType == Services.Enums.NoticeType.Inbound
                            select entity;

                foreach (var item in items)
                {
                    var storage = new Services.Models.Storage();
                    storage.ClientID = item.ClientID;
                    storage.NoticeID = item.NoticeID;
                    storage.NoticeItemID = item.ID;
                    storage.ProductID = item.ProductID;
                    storage.InputID = "";
                    storage.Type = Services.Enums.StorageType.Store;
                    storage.Islock = false;
                    storage.StocktakingType = item.StocktakingType;
                    storage.Mpq = item.Mpq;
                    storage.PackageNumber = item.PackageNumber;
                    storage.Total = item.Total;
                    storage.SorterID = item.SorterID;
                    storage.FormID = "";
                    storage.FormItemID = "";
                    storage.Currency = item.Currency;
                    storage.UnitPrice = item.UnitPrice;
                    storage.ShelveID = "Shelve20210111" + new Random().Next(2, 22).ToString("D4");
                    storage.Enter();
                }


                Response.StatusCode = 200;
                return Json(new { Success = true, Data = string.Empty });
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new { Success = false, Data = ex.Message });
            }
        }
    }
}