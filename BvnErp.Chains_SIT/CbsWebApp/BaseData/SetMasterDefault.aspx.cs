using Needs.Cbs.Services.Models.Origins;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Needs.Cbs.WebApp.BaseData
{
    public partial class SetMasterDefault : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        /// <summary>
        /// 初始化海关申报地默认关联信息
        /// </summary>
        protected void LoadData()
        {
            string code = Request.QueryString["Code"];
            var masterDefault = Needs.Wl.Admin.Plat.AdminPlat.Current.Cbs.MasterDefaults.FirstOrDefault(ms=>ms.Code == code);

            if (masterDefault != null)
            {
                this.Model.MasterDefault = new
                {
                    masterDefault.ID,
                    masterDefault.Code,
                    masterDefault.IEPortCode,
                    masterDefault.EntyPortCode,
                    masterDefault.OrgCode,
                    masterDefault.VsaOrgCode,
                    masterDefault.InspOrgCode,
                    masterDefault.PurpOrgCode
                }.Json();
            }
            else
            {
                this.Model.MasterDefault = new
                {
                    Code = code
                }.Json();
            }
        }

        protected void Save()
        {
            string id = Request.Form["ID"];
            var masterDefault = Needs.Wl.Admin.Plat.AdminPlat.Current.Cbs.MasterDefaults[id] as CustomsMasterDefault ?? new CustomsMasterDefault();

            masterDefault.Code = Request.Form["Code"];
            masterDefault.IEPortCode = Request.Form["IEPortCode"];
            masterDefault.EntyPortCode = Request.Form["EntyPortCode"];
            masterDefault.OrgCode = Request.Form["OrgCode"];
            masterDefault.VsaOrgCode = Request.Form["VsaOrgCode"];
            masterDefault.InspOrgCode = Request.Form["InspOrgCode"];
            masterDefault.PurpOrgCode = Request.Form["PurpOrgCode"];

            masterDefault.EnterError += MasterDefault_EnterError;
            masterDefault.EnterSuccess += MasterDefault_EnterSuccess;
            masterDefault.Enter();
        }

        /// <summary>
        /// 保存成功
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MasterDefault_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "保存成功" }).Json());
        }

        /// <summary>
        /// 保存失败
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MasterDefault_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Response.Write((new { success = false, message = "保存失败" }).Json());
        }
    }
}