using Needs.Ccs.Services.Views;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Classify
{
    /// <summary>
    /// 用于展示产品归类变更日志
    /// </summary>
    public partial class Log : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Model.ClassifyProduct = new
            {
                ID = Request.QueryString["ID"],
                Model = Request.QueryString["Model"]
            }.Json();
        }

        /// <summary>
        /// 产品归类日志
        /// </summary>
        /// <returns></returns>
        protected object GetProductClassifyLogs()
        {
            string id = Request.Form["ID"];
            var data = new ProductClassifyLogsView().Where(log => log.ClassifyProductID == id)
                                                    .OrderByDescending(item => item.CreateDate)
                                                     .Select(log => new
                                                     {
                                                         log.ID,
                                                         log.CreateDate,
                                                         Summary = log.OperationLog
                                                     });

            return data;
        }

        /// <summary>
        /// 产品归类变更日志
        /// </summary>
        /// <returns></returns>
        protected object GetProductClassifyChangeLogs()
        {
            string model = Request.Form["Model"];
            var data = new ProductClassifyChangeLogsView().Where(log => log.Model == model)
                                                          .OrderByDescending(item => item.CreateDate)
                                                            .Select(log => new
                                                            {
                                                                log.ID,
                                                                log.CreateDate,
                                                                log.Summary
                                                            });

            return data;
        }
    }
}