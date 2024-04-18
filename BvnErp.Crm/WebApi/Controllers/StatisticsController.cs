using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Models.Statistics;
using NtErp.Crm.Services.Views.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace WebApi.Controllers
{
    public class StatisticsController : BaseController
    {
        /// <summary>
        /// 统计客户拜访数
        /// </summary>
        /// <param name="name">创建人</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult ClientVisits(string name)
        {
            try
            {
                RateLimits.Current[StatisticsApi.ClientVisits].Verify();

                if (string.IsNullOrEmpty(name))
                {
                    throw new ArgumentNullException("创建人不能为空");
                }

                using (var view = new ClientVisitsView(name.Trim()))
                {
                    var result = view.Select(item => new { item.DateIndex, item.Count }).ToArray();
                    return Json(new { success = true, code = 200, data = result }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, code = 500, data = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 统计DI个数
        /// </summary>
        /// <param name="name">人名</param>
        /// <param name="position">职位</param>
        /// <param name="year">指定年</param>
        /// <param name="month">指定月</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DI(string name, string position, int year, int month)
        {
            try
            {
                RateLimits.Current[StatisticsApi.DI].Verify();

                if (string.IsNullOrEmpty(name))
                {
                    throw new ArgumentNullException("人名不能为空");
                }
                if (string.IsNullOrEmpty(position))
                {
                    throw new ArgumentNullException("职位不能为空");
                }

                JobType jobType = (JobType)Enum.Parse(typeof(JobType), position);
                using (var view = new ProductItemsView())
                {
                    var result = view.SearchByAdmin(name.Trim(), jobType)
                        .SearchByApply(ApplyType.DIApply, ProductStatus.DI, year, month)
                        .ToMyObject(name, jobType, ApplyType.DIApply);

                    return Json(new { success = true, code = 200, data = result }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, code = 500, data = ex.Message });
            }
        }

        /// <summary>
        /// 统计DW个数
        /// </summary>
        /// <param name="name">人名</param>
        /// <param name="position">职位</param>
        /// <param name="year">指定年</param>
        /// <param name="month">指定月</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DW(string name, string position, int year, int month)
        {
            try
            {
                RateLimits.Current[StatisticsApi.DW].Verify();

                if (string.IsNullOrEmpty(name))
                {
                    throw new ArgumentNullException("人名不能为空");
                }
                if (string.IsNullOrEmpty(position))
                {
                    throw new ArgumentNullException("职位不能为空");
                }

                JobType jobType = (JobType)Enum.Parse(typeof(JobType), position);
                using (var view = new ProductItemsView())
                {
                    var result = view.SearchByAdmin(name.Trim(), jobType)
                        .SearchByApply(ApplyType.DWApply, ProductStatus.DW, year, month)
                        .ToMyObject(name, jobType, ApplyType.DWApply);

                    return Json(new { success = true, code = 200, data = result }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, code = 500, data = ex.Message });
            }
        }

        /// <summary>
        /// 统计新增客户数
        /// </summary>
        /// <param name="manufacturer">厂商</param>
        /// <param name="year">指定年</param>
        /// <param name="month">指定月</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult NewClients(string manufacturer, int year, int month)
        {
            try
            {
                RateLimits.Current[StatisticsApi.NewClients].Verify();

                if (string.IsNullOrEmpty(manufacturer))
                {
                    throw new ArgumentNullException("厂商不能为空");
                }

                int count = 0, dicount = 0;
                object details = null;

                //2021-03-24 新需求: 额外返回厂商的新增DI数
                var task = Task.Run(() =>
                {
                    using (var diView = new ProductItemsView())
                    {
                        var di = diView.SearchByManufacturer(manufacturer.Trim())
                        .SearchByApply(ApplyType.DIApply, ProductStatus.DI, year, month);

                        dicount = di.Count();
                    }
                });
                using (var clientsView = new ClientProjectsView())
                {
                    var clients = clientsView.SearchByMfrAndDate(manufacturer.Trim(), year, month);

                    count = clients.Count();
                    details = clients.Details();
                }

                task.Wait();

                return Json(new
                {
                    success = true,
                    code = 200,
                    data = new
                    {
                        Count = count,
                        DICount = dicount,
                        Details = details
                    }
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, code = 500, data = ex.InnerException?.Message ?? ex.Message });
            }
        }
    }
}