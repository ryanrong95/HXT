using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Views.Rolls;
using Yahv.Payments;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.Finance.WebApp.Payee.PayeeClaim
{
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var id = Request.QueryString["id"];

                if (string.IsNullOrEmpty(id))
                {
                    return;
                }

                this.Model.Data = new AccountWorksView().FirstOrDefault(item => item.ID == id);
            }
        }

        #region 提交保存
        protected object Submit()
        {
            var json = new JMessage() { success = true, data = "认领成功!" };
            AccountWork entity = null;
            try
            {
                var id = Request.QueryString["id"];
                using (var accountWorkView = new AccountWorksRoll())
                {
                    entity = accountWorkView.FirstOrDefault(item => item.ID == id);
                    if (entity == null || string.IsNullOrEmpty(entity.ID))
                    {
                        json.success = false;
                        json.data = "该认领信息不存在!";
                        return json;
                    }

                    if (!string.IsNullOrEmpty(entity.ClaimantID))
                    {
                        json.success = false;
                        json.data = "该收款已认领，不能重复认领!";
                        return json;
                    }

                    entity.ClaimantID = Erp.Current.ID;
                    entity.Company = Request.Form["Company"];
                    entity.UpdateSuccess += Entity_UpdateSuccess;
                    entity.Enter();

                    Services.Oplogs.Oplog(Erp.Current.ID, LogModular.收款认领, Services.Oplogs.GetMethodInfo(), "认领", entity.Json());
                }
            }
            catch (Exception ex)
            {
                json.success = false;
                json.data = $"认领失败!{ex.Message}";
                Services.Oplogs.Oplog(Erp.Current.ID, LogModular.收款认领, Services.Oplogs.GetMethodInfo(), $"认领异常!", ex.Message + entity.Json());
            }

            return json;
        }

        /// <summary>
        /// 同步crm 本地收款
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Entity_UpdateSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as AccountWork;

            try
            {
                using (var accountsWorkView = new AccountWorksView())
                {
                    var model = accountsWorkView.FirstOrDefault(item => item.PayeeLeftID == entity.PayeeLeftID);
                    if (model == null)
                    {
                        Services.Oplogs.Oplog(Erp.Current.ID, LogModular.收款认领_To_Crm, Services.Oplogs.GetMethodInfo(), $"收款认领同步Crm异常!", $"未找到数据!" + entity.Json());
                        return;
                    }
                    Yahv.Payments.PaymentManager.Erp(Erp.Current.ID)[model.Company, model.AccountEnterprise]
                        .Digital.AdvanceFromCustomers(model.Currency, model.Price, model.BankName,
                            model.AccountCode, model.FormCode, model.ReceiptDate);
                }

                Services.Oplogs.Oplog(Erp.Current.ID, LogModular.收款认领_To_Crm, Services.Oplogs.GetMethodInfo(), $"收款认领同步Crm!", entity.Json());
            }
            catch (Exception ex)
            {
                Services.Oplogs.Oplog(Erp.Current.ID, LogModular.收款认领_To_Crm, Services.Oplogs.GetMethodInfo(), $"收款认领同步Crm异常!", ex.Message + entity.Json());
            }
        }
        #endregion
    }
}