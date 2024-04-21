using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Views;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.CrmPlus.Service.Views.Rolls.SalesChances;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Controls.Easyui;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.CrmPlus.WebApp.Crm.Projects
{
    public partial class Add : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                loadData();
            }
        }
        public void loadData()
        {
            var id = Request.QueryString["ID"];
            var entity = new ProjectRoll()[id];
            this.Model.Entity = entity;
            //销售机会状态
            this.Model.ProjectStatus = ExtendsEnum.ToDictionary<ProductStatus>().Select(item => new
            {
                value = int.Parse(item.Key),
                text = item.Value.ToString()
            });

            //跟踪记录？
            this.Model.TraceID = Request.QueryString["traceid"];
        }

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
                CurrencyDes = item.Currency.GetDescription(),
                CreteDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss")
            });

            return result;

        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var traceid = Request.QueryString["traceid"];

            var clientName = Request.Form["ClientName"];
            var name = Request.Form["ProjectName"];
            var establishDate = Request.Form["EstablishDate"];
            var rDDate = Request.Form["RDDate"];
            var productDate = Request.Form["ProductDate"];
            var contact = Request.Form["Contact"];
            var orderClient = Request.Form["OrderClient"];
            var summary = Request.Form["Summary"];
            var id = Request.QueryString["ID"];
            if (string.IsNullOrEmpty(id))
            {
                bool result = new ProjectRoll().Count(item => item.Name == name.Trim()) > 0;
                if(result)
                {
                    Easyui.Alert("提示", "项目名称已存在", Sign.Error);
                    return;
                }

            }
            var entity = new ProjectRoll()[id] ?? new Project();
            entity.EndClientID = clientName;
            entity.Name = name;
            entity.ClientContactID = contact;
            entity.OwnerID = Erp.Current.ID;
            entity.Status = DataStatus.Normal;
            entity.Summary = summary;
            if (!string.IsNullOrEmpty(orderClient))
                entity.AssignClientID = orderClient;
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
            //如果填了下单公司要生成关联关系
            if (!string.IsNullOrEmpty(orderClient) && clientName != orderClient)
            {
                entity.AssignClientID = orderClient;
                MapsEnterprisesView.Binding(entity.EndClientID, orderClient, Underly.BusinessRelationType.Place);
            }
            //else
            //{
            //    entity.AssignClientID = entity.EndClientID;
            //}
            entity.TraceId = traceid;
            entity.EnterSuccess += Entity_EnterSuccess;
            entity.Enter();
        }
        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as Yahv.CrmPlus.Service.Models.Origins.Project;
            if (entity.AssignClientID != null)
            {
                (new ApplyTask
                {
                    MainID = entity.AssignClientID,
                    MainType = Underly.MainType.Clients,
                    ApplierID = entity.MapsEnterprise.CreatorID,
                    ApplyTaskType = Underly.ApplyTaskType.ClientBusinessRelation,
                }).Enter();
            }
            Service.LogsOperating.LogOperating(Erp.Current, entity.ID, $"新增销售机会:ID:{entity.ID}");




            Easyui.AutoRedirect(this.hideSuccess.Value, Request.Url.AbsolutePath.Replace(nameof(Add) + ".aspx", "Edit.aspx") + "?ID=" + entity.ID,
                 Web.Controls.Easyui.AutoSign.Success);
        }
    }
}