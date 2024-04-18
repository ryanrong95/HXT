using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.HKWarehouse.Entry
{
    /// <summary>
    /// 装箱界面
    /// 香港操作
    /// </summary>
    public partial class Packing : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        protected void LoadData()
        {
            string id = Request.QueryString["ID"];
            var OrderID = Request.QueryString["OrderID"];
            var EntryNumber = Request.QueryString["EntryNumber"];
            this.Model.ID = id ?? "";
            this.Model.OrderID = OrderID;
            this.Model.EntryNumber = EntryNumber;          
        }

    }
}