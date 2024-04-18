using Layer.Data.Sqls;
using Needs.Underly;
using Needs.Utils.Descriptions;
using NtErp.Crm.Services.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;


namespace WebApi.Controllers
{
    /// <summary>
    /// 销售机会api接口
    /// </summary>
    public class ApiProjectController : BaseController
    {

        /// <summary>
        /// 根据客户名称、型号、品牌获取销售机会状态和报备时间
        /// </summary>
        /// <param name="clientName"></param>
        /// <param name="partnumber"></param>
        /// <param name="mf"></param>
        /// <returns></returns>
        public ActionResult Get(string clientName, string partnumber, string mf)
        {
            try
            {
                // 状态时间，如果有申请，就返回申请的变更审批通过时间，如果没有申请，就返回型号的变更时间
                var data = new NtErp.Crm.Services.Api.Views.ProjectsView().Where(item => item.ClientName == clientName && item.Partnumber.ToUpper().StartsWith(partnumber.ToUpper()) && (item.Manufaturer.ToUpper().StartsWith(mf.ToUpper()) || item.ManufaturerShortName.ToUpper().StartsWith(mf.ToUpper())))
                .ToArray().OrderByDescending(item => item.Status).ThenByDescending(item => item.StatusDate).Select(item => new
                {
                    projectName = item.Name,
                    partnumber = item.Partnumber,
                    mf = item.Manufaturer,
                    status = item.Status.GetDescription(),
                    statusDate = item.Apply == null ? item.StatusDate.ToString("yyyy-MM-dd HH:mm:ss") : item.Apply.UpdateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    reportDate = item.Enquiries.OrderByDescending(t => t.ReportDate).FirstOrDefault()?.ReportDate.ToString("yyyy-MM-dd"),
                }).FirstOrDefault();

                return Json(new JSingle<object>
                {
                    success = true,
                    code = 200,
                    data = data
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JSingle<object>
                {
                    success = false,
                    code = 400,
                    data = ex.Message
                }, JsonRequestBehavior.AllowGet);

            }

        }
    }
}