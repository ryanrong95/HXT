using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service.Views;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.CrmPlus.Service.Views.Rolls.SalesChances;
using Yahv.Underly;
using Yahv.Web.Controls.Easyui;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.CrmPlus.WebApp.Crm.Projects
{
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            loadData();
        }

        public void loadData()
        {
            var id = Request.QueryString["ID"];
            var entity = new ProjectRoll()[id];
            this.Model.Entity = entity;
        }

       /// <summary>
       /// 项目明细
       /// </summary>
       /// <returns></returns>
        protected object data()
        {

            string id = Request.QueryString["id"];
            var query = new ProjectProductView(id).ToMyArray();
            var result = query.Select(item => new
            {
                item.ID,
                item.ProjectID,
                item.UnitProduceQuantity,
                item.SpnName,
                item.BrandName,
                item.ProduceQuantity,
                item.ExpectQuantity,
                item.Currency,
                item.ExpectUnitPrice,
                item.ProjectStatus,
                ProjectStatusDes = item.ProjectStatus.GetDescription(),
                item.Status,
                StatusDes= item.Status.GetDescription(),
                CurrencyDes = item.Currency.GetDescription(),
                CreteDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss")
            });

            return result;
        }

       /// <summary>
       /// 编辑提交
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var traceid = Request.QueryString["traceid"];

            var name = Request.Form["ProjectName"];
            var establishDate = Request.Form["EstablishDate"];
            var rDDate = Request.Form["RDDate"];
            var productDate = Request.Form["ProductDate"];
            var contact = Request.Form["Contact"];
            var orderClient = Request.Form["OrderClient"];
            var summary = Request.Form["Summary"];
            var id = Request.QueryString["ID"];
            //项目名称不允许重复
            if (!string.IsNullOrEmpty(id))
            {
                bool result = new ProjectRoll().Count(item => item.Name == name.Trim() &&item.ID!=id) >0;
                if (result) {
                Easyui.Alert("提示", "项目名称已存在", Sign.Error);
                return;
                }
            }
            var entity = new ProjectRoll()[id];
            entity.Name = name;
            entity.ClientContactID = contact;
            entity.OwnerID = Erp.Current.ID;
            entity.Status = DataStatus.Normal;
            entity.Summary = summary;
            if (!string.IsNullOrWhiteSpace(establishDate))
            {
                entity.EstablishDate = Convert.ToDateTime(establishDate);
            }
            if (!string.IsNullOrWhiteSpace(rDDate))
            {
                entity.RDDate = Convert.ToDateTime(rDDate);
            }
            if (!string.IsNullOrWhiteSpace(productDate))
            {
                entity.ProductDate = Convert.ToDateTime(productDate);
            }
            if (!string.IsNullOrEmpty(orderClient) && orderClient!="null")
            {
                MapsEnterprisesView.Binding(entity.EndClientID, orderClient, Underly.BusinessRelationType.Place);
            }
            entity.TraceId = traceid;
            entity.EnterSuccess += Entity_EnterSuccess;
            entity.Enter();
        }
        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as Yahv.CrmPlus.Service.Models.Origins.Project;
            Easyui.AutoRedirect(this.hideSuccess.Value, Request.Url.AbsolutePath + "?ID=" + entity.ID,
                Web.Controls.Easyui.AutoSign.Success);
            //Easyui.Window.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }
    }
}