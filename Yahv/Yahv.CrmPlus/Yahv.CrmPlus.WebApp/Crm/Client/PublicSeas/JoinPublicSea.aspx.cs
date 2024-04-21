using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.Underly;
using Yahv.Web.Erp;
using Yahv.Web.Forms;
using YaHv.CrmPlus.Services.Views.Rolls;

namespace Yahv.CrmPlus.WebApp.Crm.Client.PublicSeas
{
    public partial class JoinPublicSea : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        public void LoadData()
        {
            var enterpriseid = Request.QueryString["id"];
            var relation = new RelationsRoll().Where(x => x.ClientID == enterpriseid).ToArray();
            this.Model.ConductType = relation.Select(x => new
            {
                value = x.Type.GetHashCode(),
                text = x.Type.GetDescription()
            });
            this.Model.Companys = relation.Select(x => new { ID = x.CompanyID, Name = x.Company.Name });
            var ids = relation.Select(x => x.OwnerID).ToArray();
            this.Model.Owners = new AdminsAllRoll().Where(x => ids.Contains(x.ID)).Select(x => new { value = x.ID, text = x.RealName });

        }



        protected void btnSubmit_Click(object sender, EventArgs e)
        {

            var id = Request.QueryString["id"];
            var conductType = Request.Form["ConductType"];
            var company = Request.Form["Company"];
          //  var owner = Request.Form["Owner"];
            var summary = Request.Form["Summary"];
            var entity = Erp.Current.CrmPlus.Clients[id];

            #region  业务类型
            //添加业务类型
            entity.Conduct = new Service.Models.Origins.Conduct()
            {
                ConductType = (ConductType)int.Parse(conductType),
                EnterpriseID=id,
                IsPublic =true
            };
            #endregion

            #region   我方合作公司
            //我方合作公司
            entity.Relation = new Service.Models.Origins.Relation
            {
                Type = (ConductType)int.Parse(conductType),
               // OwnerID = owner,
                //CompanyID = company,
                ClientID = entity.ID,
                Summary=summary,
                Status= AuditStatus.Closed
        };
            #endregion
       
            entity.EnterSuccess += Entity_EnterSuccess;
            entity.JoinSea();
        }


        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            // var client = (Yahv.CrmPlus.Service.Models.Origins.Client)e.Object;
            var client = sender as Yahv.CrmPlus.Service.Models.Origins.Client;
            Service.LogsOperating.LogOperating(Erp.Current, client.ID, $"加入公海:{client.Name}");
          
            Easyui.Dialog.Close("操作成功!", Web.Controls.Easyui.AutoSign.Success); ;
        }

    }
}