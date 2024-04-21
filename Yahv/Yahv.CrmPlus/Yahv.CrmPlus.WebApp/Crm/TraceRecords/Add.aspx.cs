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

namespace Yahv.CrmPlus.WebApp.Crm.TraceRecords
{
    public partial class Add : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }


        public void LoadData()
        {

            //this.Model.Clients = Erp.Current.CrmPlus.MyClients.Where(x => x.Status == Underly.AuditStatus.Normal && x.IsDraft == false).Select(x => new { value = x.ID, text = x.Name });
            //this.Model.FollowWay = ExtendsEnum.ToDictionary<FollowWay>().Select(item => new
            //{
            //    value = item.Key,
            //    text = item.Value
            //});

            this.Model.Readers = new YaHv.CrmPlus.Services.Views.Rolls.AdminsAllRoll().Where(item => item.RoleID == FixedRole.SaleManager.GetFixedID() || item.RoleID == FixedRole.PM.GetFixedID()).Select(item => new
            {
                value = item.ID,
                text = $"{item.RealName}-{item.RoleName}"
            });
        }

        /// <summary>
        /// 
        /// </summary>
        protected object getContacts()
        {
            var id = Request.Form["ID"];
            return Erp.Current.CrmPlus.MyContacts.Where(x => x.EnterpriseID == id).Select(x => new { value = x.ID, text = $"{x.Name}-{x.Mobile}" });

        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            #region  参数
            var name = Request.Form["Name"];
            var followWay = Request.Form["FollowWay"];
            var TraceDate = Request.Form["TraceDate"];
            var NextDate = Request.Form["NextDate"];
            var SupplierStaffs = Request.Form["SupplierStaffs"];
            var CompanyStaffs = Request.Form["CompanyStaffs"];
            var Contact = Request.Form["Contact"];
            var Context = Request.Form["Context"];
            var nextPlan = Request.Form["NextPlan"];
            var file = Request.Form["fileForJson"];
            var contantid = Request.Form["Contact"];
            var readers = Request.Form["Readers"];
            #endregion

            #region  对象数据填充
            var entity = new Service.Models.Origins.TraceRecord();
            entity.ClientID = name.Trim();
            entity.FollowWay = (FollowWay)int.Parse(followWay);
            entity.TraceDate = Convert.ToDateTime(TraceDate);
            entity.NextDate = Convert.ToDateTime(NextDate);
            entity.SupplierStaffs = SupplierStaffs;
            entity.CompanyStaffs = CompanyStaffs;
            entity.Context = Context;
            entity.NextPlan = nextPlan;
            entity.ClientContactID = contantid;
            entity.OwnerID = Erp.Current.ID;
            if (!string.IsNullOrEmpty(readers))
            {
            entity.ReadIDs = readers;
            }

            #region 附件
            entity.Files = file == null ? null : file.JsonTo<List<CallFile>>();
            #endregion
            entity.EnterSuccess += Entity_EnterSuccess;
            entity.Enter();
            #endregion

        }

        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var traceRecord = sender as Yahv.CrmPlus.Service.Models.Origins.TraceRecord;
            Service.LogsOperating.LogOperating(Erp.Current, traceRecord.ID, $"新增跟踪记录:ID:{traceRecord.ID}");
            Easyui.Dialog.Close("提交成功!", Web.Controls.Easyui.AutoSign.Success); ;
        }

    }
}