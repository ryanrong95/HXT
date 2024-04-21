using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Underly;
using Yahv.Underly.CrmPlus;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.CrmPlus.WebApp.Crm.Client.Specials
{
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Model.Enterpriseid = Request.QueryString["enterpriseid"];
            this.Model.SpecialType = ExtendsEnum.ToDictionary<SpecialType>().Select(item => new
            {
                value =int.Parse(item.Key),
                text = item.Value
            });
        }

        /// <summary>
        ///加载数据
        /// </summary>
        public void LoadData()
        {
            var id = Request.QueryString["id"];
            if (string.IsNullOrEmpty(id))
            {


            }
        }


        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var ID = Request.QueryString["id"];
            var enterpseId = Request.QueryString["enterpriseid"];
            var specialType = Request.Form["SpecialType"];
            var content = Request.Form["Content"];
            #region 附件
            var file = Request.Form["fileForJson"];
            var entity = new Yahv.CrmPlus.Service.Views.Rolls.RequiremensRoll()[ID] ?? new Requirement();
            entity.Content = content;
            entity.EnterpriseID = enterpseId;
            entity.SpecialType = (SpecialType)int.Parse(specialType);
            entity.CreatorID = Erp.Current.ID;
            entity.Files = file == null ? null : file.JsonTo<List<CallFile>>();
            entity.EnterSuccess += Entity_EnterSuccess;
            entity.Enter();
            #endregion

        }

        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as Requirement;
            LogsOperating.LogOperating(Erp.Current, entity.ID, $"新增特殊要求:{ entity.ID}");
            Easyui.Dialog.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }
    }
}