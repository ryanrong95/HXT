using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Models.Rolls;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.CrmPlus.WebApp.Srm.SupplierDetails.BusinessRelations
{
    public partial class Add : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.ID = Request.QueryString["id"]; ;
                //string enterpriseid = Request.QueryString["id"];
                //this.Model.Suppliers = Erp.Current.CrmPlus.Suppliers.Where(item => item.Status == Underly.AuditStatus.Normal && item.ID != enterpriseid).Select(item => new
                //{
                //    ID = item.ID,
                //    Name = item.Name
                //});
                //this.Model.Type = ExtendsEnum.ToDictionary<Underly.BusinessRelationType>().Select(item => new
                //{
                //    value = int.Parse(item.Key),
                //    text = item.Value
                //});
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string id = Request.QueryString["id"];
            string subid = Request.Form["SubID"];
            BusinessRelation entity = new BusinessRelation();
            entity.MainID = id;
            entity.SubID = subid;
            entity.BusinessRelationType = (Underly.BusinessRelationType)int.Parse(Request.Form["Type"]);
            entity.CreatorID = Erp.Current.ID;
            entity.MainType = Underly.MainType.Suppliers;
            entity.TaskType = Underly.ApplyTaskType.SupplierBusinessRelation;
            #region 文件
            var files = Request.Form["RelationFileForJson"];
            entity.RelationFiles = files == null ? null : files.JsonTo<List<CallFile>>();
            #endregion
            entity.Repeat += Entity_Repeat;
            entity.EnterSuccess += Entity_EnterSuccess;
            entity.Enter();
        }

        private void Entity_Repeat(object sender, Usually.ErrorEventArgs e)
        {
            Easyui.Dialog.Close("关联关系已存在!", Web.Controls.Easyui.AutoSign.Error);
        }

        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as BusinessRelation;
            Service.LogsOperating.LogOperating(Erp.Current, entity.MainID, $"新增关联关系:{entity.ID}");
            Easyui.Dialog.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }
    }
}