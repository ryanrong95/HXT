using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.PvWsOrder.Services.Models;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PvWsOrder.WebApp.Stocks.Adopt
{
    public partial class Adopt : ErpParticlePage
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
            this.Model.Client = Erp.Current.WsOrder.MyWsClients.Where(item => true).ToArray().Select(item => new
            {
                Value = item.EnterCode,
                Text = item.Name
            });  
        }

        protected void Submit()
        {
            try
            {
                string ID = Request.Form["ID"].Trim();
                string EnterCode = Request.Form["EnetrCode"].Trim();

                AdoptTmepStock adoptTmepStock = new AdoptTmepStock();
                adoptTmepStock.ID = ID;
                adoptTmepStock.EnterCode = EnterCode;
                adoptTmepStock.UpdateEnterCode();
                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            }
        }
    }
}