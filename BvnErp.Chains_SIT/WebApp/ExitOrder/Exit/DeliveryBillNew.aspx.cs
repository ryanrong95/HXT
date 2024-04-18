using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.ExitOrder.Exit
{
    /// <summary>
    /// 出库通知-出库界面
    /// 深圳库房
    /// </summary>
    public partial class DeliveryBillNew : Uc.PageBase
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
                    ClientName = exitNotice.CompanyName,
                    Contactor = exitNotice.coeContact,
                    ContactTel = exitNotice.coePhone,
                    Address = exitNotice.coeAddress,//送货地址
                    DriverName = exitNotice.wldDriver,
                    DriverTel = exitNotice.wldTakingPhone,
                    License = exitNotice.wldCarNumber1,
                    DeliveryTime = exitNotice.AppointTime?.ToString("yyyy-MM-dd"),
                    SZPackingDate = exitNotice.CreateDate.ToString("yyyy-MM-dd"),//出库的装箱日期
                    PackNo = exitNotice.Quantity,
                    ExitType = (int)WaybillType.DeliveryToWarehouse,
                    Purchaser = PurchaserContext.Current.CompanyName,                   
                }.Json();

                this.Model.ExitNoticeItems = exitNotice.Items.Json();
            }
        }
    }
}