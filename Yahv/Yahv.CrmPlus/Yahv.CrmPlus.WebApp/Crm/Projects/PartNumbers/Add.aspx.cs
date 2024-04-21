using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Views;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.CrmPlus.Service.Views.Rolls.SalesChances;
using Yahv.Underly;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.CrmPlus.WebApp.Crm.Projects.PartNumbers
{
    public partial class Add : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            loadData();
        }

        public void loadData()
        {
            var id = Request.QueryString["ID"];
            var entity = new Service.Views.Rolls.SalesChances.ProjectProductRoll()[id];
            this.Model.Entity = entity;
            //标准型号
            //    this.Model.StandardPartNumber = new CrmPlus.Service.Views.Rolls.StandardPartNumbersRoll().Select(x => new { PartNumber = x.PartNumber, SpnID = x.ID, Brand = x.Brand });
            //this.Model.Currencys = ExtendsEnum.ToDictionary<Currency>().Select(item => new
            //{
            //    value = int.Parse(item.Key),
            //    text = item.Value.ToString()
            //});
            this.Model.ProjectStatus = ExtendsEnum.ToDictionary<ProductStatus>().Select(item => new
            {
                value = int.Parse(item.Key),
                text = item.Value.ToString()
            });

        }



        #region 删除竞品
        bool disableSuccess = false;
        protected bool disable()
        {
            var id = Request.Form["id"];
            var entity = new Service.Views.Rolls.SalesChances.ProjectCompeletetRoll()[id];
            if (entity == null)
            {
                return false;
            }
            entity.AbandonSuccess += Entity_AbandonSuccess; ;
            entity.Abandon();
            return disableSuccess;
        }


        private void Entity_AbandonSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var id = Request.Form["id"];
            LogsOperating.LogOperating(Erp.Current, id, $"删除该竞品:{ id}");
            this.disableSuccess = true;
        }

        #endregion

        protected object data()
        {
            string id = Request.QueryString["id"];
            var query = new ProjectCompeleteView(id);
            var result = query.ToMyArray().Select(item => new
            {
                item.ID,
                item.ProjectID,
                item.ProjectProductID,
                item.Brand,
                item.SpnName,
                item.UnitPrice,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss")
            });

            return result;

        }


        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var projectID = Request.QueryString["ProjectID"];
            var clientID = Request.QueryString["ClientID"];
            var spnID = Request.Form["PartNumber"];
            var unitProduceQuantity = Request.Form["UnitProduceQuantity"];
            var produceQuantity = Request.Form["ProduceQuantity"];
            var currency = Request.Form["Currency"];
            var projectStatus = Request.Form["ProjectStatus"];
            var expectUnitPrice = Request.Form["ExpectUnitPrice"];
            var summary = Request.Form["Summary"];
            var expectQuantity = Request.Form["ExpectQuantity"];
            var id = Request.QueryString["ID"];
            // var entity = new ProjectProductRoll()[id] ?? new ProjectProduct();
            var entity = new ProjectProduct();
            entity.SpnID = spnID;
            if (!string.IsNullOrEmpty(projectID))
            {
                entity.ProjectID = projectID;
            }
            entity.Summary = summary;
            if (!string.IsNullOrEmpty(clientID))
            {
                entity.AssignClientID = clientID;
            }
            else
            {
                var project = new ProjectRoll()[projectID];
                entity.AssignClientID = project.EndClientID;
            }
            entity.UnitProduceQuantity = int.Parse(unitProduceQuantity);
            if (!string.IsNullOrEmpty(produceQuantity))
            {
                entity.ProduceQuantity = int.Parse(produceQuantity);
            }
            //预计成交量
            if (!string.IsNullOrEmpty(expectQuantity))
            {
                entity.ExpectQuantity = int.Parse(expectQuantity);
            }
            entity.Currency = string.IsNullOrEmpty(currency) ? Currency.Unknown : (Currency)int.Parse(currency);
            entity.ExpectUnitPrice = Convert.ToDecimal(expectUnitPrice);
            entity.ProjectStatus = entity.ProjectStatus;
            entity.Reporter = Erp.Current.ID;
            entity.Status = AuditStatus.Normal;
            entity.EnterSuccess += Entity_EnterSuccess;
            entity.Enter();
        }
        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as Yahv.CrmPlus.Service.Models.Origins.ProjectProduct;
            Service.LogsOperating.LogOperating(Erp.Current, entity.ID, $"新增型号:ID:{entity.ID}");
            Easyui.AutoRedirect(this.hideSuccess.Value, Request.Url.AbsolutePath.Replace(nameof(Add) + ".aspx", "Edit.aspx") + "?ID=" + entity.ID,
               Web.Controls.Easyui.AutoSign.Success);
        }
    }
}