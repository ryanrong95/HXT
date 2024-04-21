using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.PvRoute.Services.Models;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PvRoute.WebApp.LogisticsInfo.WaybillLogs
{
    public partial class EditConsignee : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var consigneeID = Request.QueryString["ConsigneeID"];
            this.Model.ConsigneeID = consigneeID;
            this.Model.Data = Yahv.Erp.Current.PvRoute.TransportConsignees.Where(t => t.ID == consigneeID).FirstOrDefault();
        }

        #region 提交保存

        protected void Submit()
        {
            var consigneeID = Request.QueryString["ConsigneeID"];
            TransportConsignee consignee = null;
            try
            {
                var Name = Request.Form["Name"];
                var Phone = Request.Form["Phone"];
                var Mobile = Request.Form["Mobile"];
                var Email = Request.Form["Email"];
                var Address = Request.Form["Address"];

                var consignees = Erp.Current.PvRoute.TransportConsignees;

                if (string.IsNullOrWhiteSpace(consigneeID))
                {
                    consignee = new TransportConsignee()
                    {
                        Name = Name,
                        Mobile=Mobile,
                        Phone=Phone,
                        Email=Email,
                        Address=Address
                    };

                    if (consignee != null && consignees.Any(item => item.Name == consignee.Name))
                    {
                        Response.Write((new { success = false, message = $"该收货人已经存在，请勿重复添加!", }).Json());
                        return;
                    }
                }
                else
                {
                    
                    consignee = consignees.Where(t => t.ID == consigneeID).FirstOrDefault();
                    consignee.Name = Name;
                    consignee.Phone = Phone;
                    consignee.Mobile = Mobile;
                    consignee.Email = Email;
                    consignee.Address = Address;

                    if (consignee != null && consignees.Any(item => item.ID != consigneeID && item.Name == consignee.Name))
                    {
                        Response.Write((new { success = false, message = $"该收货人已经存在，请勿重复添加!", }).Json());
                        return;
                    }
                }

                consignee.Enter();

                //后期增加物流管理的日志（需要增加日志表才行）
                //Services.Oplogs.Oplog(Erp.Current.ID, LogModular.银行管理, Services.Oplogs.GetMethodInfo(), string.IsNullOrWhiteSpace(BankID) ? "新增" : "修改", bank.Json());
                Response.Write((new { success = true, message = "提交成功", }).Json());
            }
            catch (Exception ex)
            {
                //后期增加物流管理的日志（需要增加日志表才行）
                //Services.Oplogs.Oplog(Erp.Current.ID, LogModular.银行管理, Services.Oplogs.GetMethodInfo(), string.IsNullOrWhiteSpace(BankID) ? "新增" : "修改", bank.Json());
                Response.Write((new { success = false, message = $"提交异常!{ex.Message}", }).Json());
            }
        }

        #endregion
    }
}