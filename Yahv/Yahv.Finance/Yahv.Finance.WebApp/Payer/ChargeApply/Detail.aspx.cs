using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.Finance.WebApp.Payer.ChargeApply
{
    public partial class Detail : ErpParticlePage
    {
        #region 加载数据
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitData();
            }
        }

        public void InitData()
        {
            var id = Request.QueryString["id"];
            if (!string.IsNullOrWhiteSpace(id))
            {
                var model = Erp.Current.Finance.ChargeApplies.GetApply(id);

                if (string.IsNullOrEmpty(model.PayerAccountName))
                {
                    model.PayerAccountName = model.PayerName;
                    model.PayerAccountCurrencyDes = model.Currency.GetDescription();
                }

                this.Model.Data = model;
            }
        }
        #endregion

        #region 功能函数
        /// <summary>
        /// 加载附件
        /// </summary>
        protected void filedata()
        {
            var files = Erp.Current.Finance.FilesDescriptionView
                .SearchByFilesMapValue(FilesMapName.ChargeApplyID.ToString(), Request.QueryString["id"]);
            string fileWebUrlPrefix = ConfigurationManager.AppSettings["FileWebUrlPrefix"];
            Func<FilesDescription, object> convert = item => new
            {
                ID = item.ID,
                CustomName = item.CustomName,
                FileFormat = "",
                Url = item.Url,    //数据库相对路径
                WebUrl = string.Concat(fileWebUrlPrefix, "/" + item.Url),
            };
            Response.Write(new
            {
                rows = files.Select(convert).ToArray(),
                total = files.Count(),
            }.Json());
        }

        /// <summary>
        /// 审批日志
        /// </summary>
        /// <returns></returns>
        protected object getLogs()
        {
            string applyId = Request.QueryString["id"];

            return Erp.Current.Finance.LogsApplyStepView.Where(item => item.ApplyID == applyId)
                .OrderByDescending(item => item.CreateDate).ToArray().ToArray()
                .Select(item => new
                {
                    item.ID,
                    CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    ApproverName = item.Approver?.RealName,
                    Status = item.Status.GetDescription(),
                    Summary = item.Summary,
                });
        }

        /// <summary>
        /// 加载申请项
        /// </summary>
        /// <returns></returns>
        protected object data()
        {
            var id = Request.QueryString["id"];
            if (string.IsNullOrWhiteSpace(id)) return null;
            var applyItems = Erp.Current.Finance.ChargeApplyItems.Where(item => item.ApplyID == id).ToArray();
            var catalogs = Erp.Current.Finance.AccountCatalogs.ToArray();

            return from apply in applyItems
                   join catalog in catalogs on apply.AccountCatalogID equals catalog.ID
                   select new
                   {
                       AccountCatalogName = catalog.Name,
                       Price = apply.Price,
                       CreateDate = apply.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                       apply.Summary,
                   };
        }
        #endregion
    }
}