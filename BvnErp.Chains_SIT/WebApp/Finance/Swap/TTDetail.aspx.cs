using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Underly;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance
{
    public partial class TTDetail : Uc.PageBase
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

        }

        protected void TTDetailData()
        {
            string txnRefId = Request.Form["txnRefId"];           
            var data = new Needs.Ccs.Services.Views.TTRequestView().Where(t => t.fxContractRef1 == txnRefId).FirstOrDefault();
            if (data != null)
            {
                Dictionary<string, string> chargeBearer = new Dictionary<string, string>();
                chargeBearer.Add("CRED", "收款方承担");
                chargeBearer.Add("DEBT", "汇款人承担");
                chargeBearer.Add("SHAR", "共同承担");

                Response.Write((new
                {
                    success = true,
                    senderName = data.senderName,
                    senderAccountNo = data.senderAccountNo,
                    senderSwiftBic = data.senderSwiftBic,
                    PartyNameEnglish = data.receivingName,
                    AccountNo = data.receivingAccountNo,
                    SwiftCode = data.receivingSwiftBic,
                    chargeBearer = chargeBearer[data.chargeBearer],
                }).Json());
            }
            else
            {
                Response.Write((new { success = false, message = "查询失败!" }).Json());
            }
        }

       
    }
}