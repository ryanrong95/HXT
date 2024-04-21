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
    public partial class BindConsignee : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var ID = Request.QueryString["ID"];
            this.Model.TransportLogID = ID;
            this.Model.Consignees = Yahv.Erp.Current.PvRoute.TransportConsignees.Select(item => new
            {
                value = item.ID,
                text = item.Name
            });
        }

        #region 提交保存

        protected void Submit()
        {
            var transportLogID = Request.Form["TransportLogID"];
            Logs_Transport transportLog = null;

            /* 
             推送货物动态，物流等信息给收货人
             业务逻辑：1.每条物流日志信息绑定不同的收货人然后推送消息
                       2.查出每条物流日志信息的物流订单，统一改掉该物流订单下的收货人，理论上一个物流订单只有一个实际收货人（决定采用这种）
             */

            try
            {
                var ConsigneeID = Request.Form["ConsigneeID"];

                var transportLogs = Erp.Current.PvRoute.TransportLogs;

                transportLog= transportLogs.Where(t => t.ID == transportLogID).FirstOrDefault();

                //获得物流订单ID
                var faceOrderID = transportLog.FaceOrderID;
                //查出每条物流日志信息的物流订单
                var transportLogs1 = transportLogs.Where(t => t.FaceOrderID == faceOrderID);
                foreach (var item in transportLogs1)
                {
                    item.ConsigneeID = ConsigneeID;
                    item.Enter();
                }

                //补上流程给手机号发邮件，没有手机号给邮箱发邮件，告知货物动态

                //transportLog.ConsigneeID = ConsigneeID;
                //transportLog.Enter();


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