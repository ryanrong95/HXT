using Layers.Data.Sqls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yahv.Services.Models;
using Yahv.Services.Views;
using Yahv.Web.Mvc;

namespace MvcApi.Controllers.chonggou
{
    public class cgStoragesController : Controller
    {
        public ActionResult Show(JPost jpost)
        {
            var arguments = new
            {
                whid = jpost["WhID"]?.Value<string>(),
                partnumber = jpost["PartNumber"]?.Value<string>(),
                manufacturer = jpost["Manufacturer"]?.Value<string>(),
                shelveid = jpost["ShelveID"]?.Value<string>(),
                starttime = jpost["StartTime"]?.Value<DateTime?>(),
                endtime = jpost["EndTime"]?.Value<DateTime?>(),
                pageIndex = jpost["PageIndex"]?.Value<int?>() ?? 1,
                pageSize = jpost["PageSize"]?.Value<int?>() ?? 20,
            };

            CgStoragesTopView<PvWmsRepository> storageView = new CgStoragesTopView<PvWmsRepository>();

            if (!string.IsNullOrEmpty(arguments.whid))
            {
                storageView = storageView.SearchByWareHouseID(arguments.whid);
            }

            if (!string.IsNullOrEmpty(arguments.partnumber))
            {
                storageView = storageView.SearchByPartNumber(arguments.partnumber);
            }

            if (!string.IsNullOrEmpty(arguments.manufacturer))
            {
                storageView = storageView.SearchByManufacturer(arguments.manufacturer);
            }

            if (!string.IsNullOrEmpty(arguments.shelveid))
            {
                var shelveid = arguments.whid.StartsWith("SZ") ? ("PSZ" + arguments.shelveid) : arguments.shelveid;
                storageView = storageView.SearchByShelveID(shelveid);
            }

            if (arguments.starttime.HasValue || arguments.endtime.HasValue)
            {
                storageView = storageView.SearchByDate(arguments.starttime.Value, arguments.endtime.Value);
            }

            var data = storageView.ToMyPage(arguments.pageIndex, arguments.pageSize);

            return Json(new { obj = data }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 修改到货数量
        /// </summary>
        /// <param name="storageId">库存ID</param>
        /// <param name="quantity">现存数量</param>
        /// <param name="total">总数量</param>
        /// <returns></returns>
        public ActionResult UpdateDeliveredQty(JPost jpost)
        {
            try
            {

                var storageId = jpost["storageId"].Value<string>();
                var quantity = jpost["quantity"]?.Value<decimal>();
                var total = jpost["total"]?.Value<decimal>();
                if (quantity < 0)
                {
                    throw new NotImplementedException("数量不能为负数！");
                }
                using (var repository = new PvWmsRepository())
                {
                    var storage = repository.ReadTable<Layers.Data.Sqls.PvWms.Storages>().FirstOrDefault(item => item.ID == storageId);
                    if (storage == null)
                    {
                        throw new NotImplementedException("库存ID不存在！");
                    }
                    else
                    {
                        //Total的数量不能低于quantity
                        if (!(total < quantity))
                        {
                        repository.Update<Layers.Data.Sqls.PvWms.Storages>(new
                        {
                            Total = total,
                            Quantity =quantity,
                        }, item => item.ID == storageId);

                        repository.Update<Layers.Data.Sqls.PvWms.Sortings>(new
                        {
                            Quantity = quantity,
                        }, item => item.ID == storage.SortingID);
                        }
                    }
                }
                var result = Json(new
                {
                    Success = true,
                    Data = "修改成功！",
                });
                return result;

            }
            catch (Exception ex)
            {

                var result = Json(new
                {
                    Success = false,
                    Data = ex.Message,
                });
                return result;
            }

        }
    }
}