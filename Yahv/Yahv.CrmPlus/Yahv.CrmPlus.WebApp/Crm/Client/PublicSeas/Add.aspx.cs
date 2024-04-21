using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.CrmPlus.WebApp.Crm.Client.PublicSeas
{
    /// <summary>
    /// 新增公海客户
    /// </summary>
    public partial class Add : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }


        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            #region  参数
            var name = Request.Form["name"].Trim();
            #endregion

            var entity = new Yahv.CrmPlus.Service.Models.Origins.PublicClient();
            #region  客户信息
            entity.ClientType = Yahv.Underly.CrmPlus.ClientType.Terminals;
            entity.Vip = Underly.VIPLevel.NonVIP;
            //这s三个字段值不确定
            entity.IsMajor = false;
            entity.IsSpecial = false;
            entity.IsSupplier = false;
            #endregion

            #region   企业

            entity.Name = name;
            entity.Status = AuditStatus.Waiting;
            entity.IsDraft = false;
            //entity.Enterprise = new Service.Models.Origins.Enterprise()
            //{

            //    IsDraft = false,
            //    Name = name,
            //    Status = AuditStatus.Waiting
            //};
            #endregion

            entity.ClientType = Yahv.Underly.CrmPlus.ClientType.Trader;
            entity.Vip = VIPLevel.NonVIP;
            //这s三个字段值不确定
            entity.IsMajor = false;
            entity.IsSpecial = false;
            entity.IsSupplier = false;
            // entity.ID = Request["ID"];
            entity.EnterSuccess += Entity_EnterSuccess;
            entity.Add();


        }
        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var client = sender as Yahv.CrmPlus.Service.Models.Origins.PublicClient;
            Service.LogsOperating.LogOperating(Erp.Current, client.ID, $"新增公海客户:{client.Name}");
            Easyui.Dialog.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success); ;
        }

    }
}