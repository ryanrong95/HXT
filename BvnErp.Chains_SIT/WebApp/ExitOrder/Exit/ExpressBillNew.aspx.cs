using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using YuanDa_Logistics.Utility;

namespace WebApp.ExitOrder.Exit
{
    /// <summary>
    /// 出库-快递单
    /// 深圳库房
    /// </summary>
    public partial class ExpressBillNew : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {          
            LoadData();
        }

        private string getExpressType(string ExpressCompany,int EXEValue)
        {
            string ExtName = "";
            switch (ExpressCompany)
            {
                case "SF":
                case "顺丰":
                    Type tSF = typeof(SfExpType);
                    var SFins = Activator.CreateInstance(tSF) as CodeType;
                    ExtName = SFins.Where(t2 => t2.Value == EXEValue).Select(item => item.Name).FirstOrDefault();                   
                    break;

                case "KY":
                case "跨越":
                case "跨越速运":
                    Type tKY = typeof(KysyExpType);
                    var KYins = Activator.CreateInstance(tKY) as CodeType;
                    ExtName = KYins.Where(t2 => t2.Value == EXEValue).Select(item => item.Name).FirstOrDefault();
                    break;
            }

            return ExtName;  
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
                    Contactor = exitNotice.coeContact,//提货人
                    ContactTel = exitNotice.coePhone,
                    Address = exitNotice.coeAddress,
                    PackNo = exitNotice.Quantity,
                    ExpressComp = exitNotice.CarrierName,
                    ExpressCode = exitNotice.Code,
                    ExpressTy = getExpressType(exitNotice.CarrierName,exitNotice.ExpressTy!=null? exitNotice.ExpressTy.Value:1),
                    ExpressPayType = exitNotice.ExpressPayType?.GetDescription(),
                    ExitType = (int)WaybillType.LocalExpress,
                    DeliveryTime = exitNotice.AppointTime?.ToString("yyyy-MM-dd"),
                    SZPackingDate = exitNotice.CreateDate.ToString("yyyy-MM-dd"),//出库的装箱日期
                    Purchaser = PurchaserContext.Current.CompanyName,
                }.Json();

                this.Model.ExitNoticeItems = exitNotice.Items.Json();
            }
        }
    }
}