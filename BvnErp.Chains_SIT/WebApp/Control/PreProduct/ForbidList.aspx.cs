using Needs.Ccs.Services.ApiSettings;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
using Needs.Ccs.Services.Models;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Control.PreProduct
{
    /// <summary>
    /// 预归类产品禁运管控审批查询界面
    /// </summary>
    public partial class ForbidList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 初始化待审批数据
        /// </summary>
        protected void data()
        {
            string model = Request.QueryString["Model"];
            string clientCode = Request.QueryString["ClientCode"];

            List<LambdaExpression> lamdas = new List<LambdaExpression>();
            Expression<Func<PreProductControl, bool>> expression = item => item.Type == ItemCategoryType.Forbid;

            #region 页面查询条件
            if (!string.IsNullOrWhiteSpace(model))
            {
                lamdas.Add((Expression<Func<PreProductControl, bool>>)(item => item.PreProduct.Model.Contains(model.Trim())));
            }
            if (!string.IsNullOrEmpty(clientCode))
            {
                lamdas.Add((Expression<Func<PreProductControl, bool>>)(item => item.PreProduct.Client.ClientCode.Contains(clientCode.Trim())));
            }
            #endregion

            #region 页面需要数据
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            var records = Needs.Wl.Admin.Plat.AdminPlat.Current.Control.MyPreProductControls.GetPageList(page, rows, expression, lamdas.ToArray());

            Response.Write(new
            {
                rows = records.Select(
                       item => new
                       {
                           item.ID,
                           item.PreProduct.Client.ClientCode,
                           ClientName = item.PreProduct.Client.Company.Name,

                           item.PreProduct.ProductUnionCode,
                           item.PreProduct.Model,
                           item.PreProduct.Manufacturer,
                           item.Category.HSCode,
                           item.Category.ProductName,
                           item.Category.ClassifyFirstOperatorName,
                           item.Category.ClassifySecondOperatorName,
                           Approver = Needs.Wl.Admin.Plat.AdminPlat.Current.RealName,
                           CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                       }
                    ).ToArray(),
                total = records.Total,
            }.Json());
            #endregion
        }

        /// <summary>
        /// 审批通过
        /// </summary>
        protected void Approve()
        {
            try
            {
                string id = Request.Form["ID"];
                var control = Needs.Wl.Admin.Plat.AdminPlat.Current.Control.MyPreProductControls[id];
                var approverID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
                control.Approved += Control_Approved;
                control.Approve(approverID);

                Response.Write((new { success = true, message = "审批成功！" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "审批失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 审批否决
        /// </summary>
        protected void Veto()
        {
            try
            {
                string id = Request.Form["ID"];
                var control = Needs.Wl.Admin.Plat.AdminPlat.Current.Control.MyPreProductControls[id];
                var approverID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
                control.Vetoed += Control_Vetoed;
                control.Veto(approverID);

                Response.Write((new { success = true, message = "审批成功！" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "审批失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 一键通过
        /// </summary>
        protected void BatchApprove()
        {
            try
            {
                string[] ids = Request.Form["IDs"].Split(',');
                var controls = Needs.Wl.Admin.Plat.AdminPlat.Current.Control.MyPreProductControls.GetTop(ids.Length, item => ids.Contains(item.ID));
                var approverID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;

                Parallel.ForEach(controls, (control) =>
                {
                    control.Approved += Control_Approved;
                    control.Approve(approverID);
                });

                Response.Write((new { success = true, message = "审批成功！" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "审批失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 一键否决
        /// </summary>
        protected void BatchVeto()
        {
            try
            {
                string[] ids = Request.Form["IDs"].Split(',');
                var controls = Needs.Wl.Admin.Plat.AdminPlat.Current.Control.MyPreProductControls.GetTop(ids.Length, item => ids.Contains(item.ID));
                var approverID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;

                Parallel.ForEach(controls, (control) =>
                {
                    control.Vetoed += Control_Vetoed;
                    control.Veto(approverID);
                });

                Response.Write((new { success = true, message = "审批成功！" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "审批失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 审批通过触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Control_Approved(object sender, PreProductControledEventArgs e)
        {
            var preProduct = e.PreProductControl.PreProduct;

            //调用中心数据的接口，删除该型号的禁运管控信息
            var pvdataApi = new PvDataApiSetting();
            var url = ConfigurationManager.AppSettings[pvdataApi.ApiName] + pvdataApi.DeleteSysControl;
            var result = Needs.Utils.Http.ApiHelper.Current.JPost<Needs.Underly.JMessage>(url, new
            {
                partNumber = preProduct.Model,
                type = ProductControlType.Forbid.GetHashCode()
            });
        }

        /// <summary>
        /// 审批否决触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Control_Vetoed(object sender, PreProductControledEventArgs e)
        {
            var preProduct = e.PreProductControl.PreProduct;

            //调用中心数据的接口，更新禁运管控历史记录
            var pvdataApi = new PvDataApiSetting();
            var url = ConfigurationManager.AppSettings[pvdataApi.ApiName] + pvdataApi.UpdateEmbargoControl;
            var result = Needs.Utils.Http.ApiHelper.Current.JPost<Needs.Underly.JMessage>(url, new
            {
                partNumber = preProduct.Model,
                manufacturer = preProduct.Manufacturer,
                isEmbargo = true
            });
        }
    }
}