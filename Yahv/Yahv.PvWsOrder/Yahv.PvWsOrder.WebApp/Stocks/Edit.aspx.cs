using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.PvWsOrder.Services;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PvOms.WebApp.Stocks
{
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 提交库位租赁价格配置
        /// </summary>
        protected void Submit()
        {
            //调用库房库存到货数量接口。
            try
            {
                string ID = Request.Form["ID"];
                string total = Request.Form["total"];
                string quantity = Request.Form["quantity"];

                //调用库房接口
                var apisetting = new PvWmsApiSetting();
                var apiurl = ConfigurationManager.AppSettings[apisetting.ApiName]
                    + apisetting.CgStorageUpdate;
                var result = Yahv.Utils.Http.ApiHelper.Current.JPost(apiurl,new { storageId= ID , quantity=quantity, total= total });
                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            }
        }
    }
}