using System;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Views.Rolls;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.Finance.WebApp.BasicInfo.AccountCatalogsGrant
{
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.ID = Request.QueryString["id"];
                this.Model.Name = Request.QueryString["name"];
            }
        }

        /// <summary>
        /// 保存按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string id = Request.QueryString["id"];
            var arry = Request.Form["menustree"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            Erp.Current.Finance.AccountCatalogs.Map(id, arry);
            Services.Oplogs.Oplog(Erp.Current.ID, LogModular.账款类型分配, Services.Oplogs.GetMethodInfo(), "分配", arry.Json(), url: Request.Url.ToString());
            Easyui.Dialog.Close("操作成功!", Web.Controls.Easyui.AutoSign.Success);
        }

        protected string menustree()
        {
            return new AccountCatalogsGrantTree(Request.QueryString["id"]).Json();
        }
    }
}