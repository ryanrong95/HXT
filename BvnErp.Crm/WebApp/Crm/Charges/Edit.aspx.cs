using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.Charges
{
    /// <summary>
    /// 客户费用编辑页面
    /// </summary>
    public partial class Edit : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }
        }


        /// <summary>
        /// 费用数据保存
        /// </summary>
        protected void SavePost()
        {
            string ClientID = Request.Form["ClientID"];
            string ActionID = Request.QueryString["ActionID"];        
            string data = Request.Form["data"];
            var s = data.Replace("&quot;", "\"");
          
            string message = "保存成功";
            var charges = JsonSerializerExtend.JsonTo<List<NtErp.Crm.Services.Models.Charge>>(s);
       
            foreach (var fee in charges)
            {
                try
                {
                    fee.Clients = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.Client>.Create(ClientID);
                    //fee.Actions = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.Plan>.Create(ActionID);
                    fee.ClientID = ClientID;
                    fee.ActionID = ActionID;
                    fee.AdminID = Needs.Erp.ErpPlot.Current.ID;
                    fee.Enter();
                }
                catch (Exception e)
                {
                    message += e.Message;
                    continue;
                }
            }
            Charges_EnterSuccess();
        }

        /// <summary>
        /// 保存成功触发事件
        /// </summary>
        private void Charges_EnterSuccess()
        {
            var url = Request.UrlReferrer ?? Request.Url;
            this.Alert("保存成功", url, true);
        }
    }
}