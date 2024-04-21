using System;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.Erm.WebApp.Erm.PersonalRates
{
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var id = Request.QueryString["id"];
                var model = Alls.Current.PersonalRates[id];
                if (model != null)
                {
                    model.Rate = model.Rate * 100;
                    this.Model.Data = model;
                }
            }
        }
        /// <summary>
        /// 数据保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            var id = Request.QueryString["id"];
            var rate = Alls.Current.PersonalRates[id] ?? new Services.Models.Origins.PersonalRate();
            rate.Levels = int.Parse(Request.Form["Levels"].Trim());
            rate.AdminID = Erp.Current.ID;
            rate.BeginAmount = decimal.Parse(Request.Form["BeginAmount"].Trim());
            rate.EndAmount = decimal.Parse(Request.Form["EndAmount"].Trim());
            rate.Rate = decimal.Parse(Request.Form["Rate"].Trim()) / 100;
            rate.Deduction = decimal.Parse(Request.Form["Deduction"].Trim());
            rate.Description = Request.Form["Description"];
            rate.EnterError += Rate_EnterError;
            rate.EnterSuccess += Rate_EnterSuccess;
            if (string.IsNullOrWhiteSpace(id))
            {
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "个税税率表",
                    $"添加", rate.Json());
            }
            else
            {
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "个税税率表",
                    $"修改", rate.Json());
            }
            rate.Enter();
        }

        /// <summary>
        /// 保存成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Rate_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            Easyui.Dialog.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }

        /// <summary>
        /// 保存失败触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Rate_EnterError(object sender, Usually.ErrorEventArgs e)
        {
            Easyui.Alert("操作提示", e.Message, Web.Controls.Easyui.Sign.Error);
        }
    }
}