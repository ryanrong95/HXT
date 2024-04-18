using Needs.Ccs.Services.Models;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Order.IcgooOrder
{
    public partial class CreateOrder : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //GenerateDecheadPDF();
        }

        protected void UpdateEntryID()
        {
            try
            {
                string DeclarationID = Request.Form["ID"];
                CenterCreateDyjOrderWeb centerCreateDyjOrderWeb = new CenterCreateDyjOrderWeb("深圳市芯达通供应链管理有限公司", "杭州比一比电子科技有限公司");
                centerCreateDyjOrderWeb.CreateNew(DeclarationID);

                Response.Write(new { result = true, info = "保存成功" }.Json());
            }
            catch (Exception ex)
            {
                Response.Write(new { result = false, info = "保存错误" + ex.ToString() }.Json());
            }
        }

    }
}