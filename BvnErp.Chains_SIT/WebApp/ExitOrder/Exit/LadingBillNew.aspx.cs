using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using YuanDa_Logistics.Utility;

namespace WebApp.ExitOrder.Exit
{
    /// <summary>
    /// 待出库信息显示
    /// 深圳库房
    /// </summary>
    public partial class LadingBillNew : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        protected void LoadData()
        {
            this.Model.ExitNotice = "";
            string ID = Request["ExitNoticeID"];
            if (string.IsNullOrEmpty(ID) == true)
            {
                return;
            }

            var exitNotice = new DeliveryOrderOriginView().Where(item => item.ID == ID).FirstOrDefault();

            if (exitNotice != null)
            {
                this.Model.ExitNotice = new
                {
                    ExitNoticeID = exitNotice.ID,
                    OrderID = exitNotice.OrderID,
                    PackNo = exitNotice.Quantity,
                    ClientName = exitNotice.CompanyName,
                    DeliveryName = exitNotice.coeContact,//提货人
                    DeliveryTel = exitNotice.coePhone,
                    IDType = "身份证",
                    IDCard = exitNotice.coeIDNumber,                   
                    ExitType = (int)WaybillType.PickUp,
                    DeliveryTime = exitNotice.AppointTime?.ToString("yyyy-MM-dd"),
                    SZPackingDate = exitNotice.CreateDate.ToString("yyyy-MM-dd"),//出库的装箱日期
                    Purchaser = PurchaserContext.Current.CompanyName
                }.Json();

                this.Model.ExitNoticeItems = exitNotice.Items.Json();
            }
        }
    }
}