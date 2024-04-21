using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Views.Rolls.Samples;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.CrmPlus.WebApp.Crm.Client.Approvals.Samples
{
    public partial class Edit : ErpParticlePage
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
            this.Model.Entity = new SampleRoll()[id];
            //  this.Model.Clients = Erp.Current.CrmPlus.MyClients.Where(x => x.Status == Underly.AuditStatus.Normal && x.Enterprise.IsDraft == false).Select(x => new { value = x.ID, text = x.Enterprise.Name });
            //this.Model.StandardPartNumber = new CrmPlus.Service.Views.Rolls.StandardPartNumbersRoll().Select(x => new { PartNumber = x.PartNumber, SpnID = x.ID, Brand = x.Brand });
            //this.Model.SampleTypes = ExtendsEnum.ToDictionary<SampleType>().Select(item => new
            //{
            //    value = int.Parse(item.Key),
            //    text = item.Value.ToString()
            //});
        }

        //protected object getInfoByClient()
        //{
        //    var id = Request.Form["ID"];
        //    var projects = Erp.Current.CrmPlus.MyProjects.Where(x => x.EndClientID == id).Select(x => new { value = x.ID, text = x.Name });
        //    var contacts = Erp.Current.CrmPlus.MyContacts.Where(x => x.EnterpriseID == id).Select(x => new { value = x.ID, text = $"{x.Name}-{x.Mobile}" });
        //    var address = Erp.Current.CrmPlus.Addresses[id, RelationType.Trade].Select(x => new { value = x.ID, text = x.Context });
        //    var data = new { projects, address, contacts };

        //    return data;
        //}

        protected object data()
        {

            string id = Request.QueryString["ID"];
            var query = new SampleItemRoll().Where(x => x.SampleID == id);
            var result = this.Paging(query.OrderByDescending(item => item.CreateDate).ToArray().Select(item => new
            {
                item.ID,
                item.SampleID,
                item.SpnName,
                item.Brand,
                item.Price,
                item.Quantity,
                Total = (item.Price) * (Convert.ToDecimal(item.Quantity)),
                item.SpnID,
                item.SampleType,
                SampleTypeDes = item.SampleType.GetDescription(),
                CreteDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                item.AuditStatus,
            }));

            return result;

        }


        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            bool result = Convert.ToBoolean(this.Result.Value);
            var id = Request.QueryString["id"];
            var entity = new SampleRoll()[id];
            entity.ApproverID = Erp.Current.ID;
            if (result)
            {
                entity.AuditStatus = AuditStatus.Normal;
            }
            else
            {
                entity.AuditStatus = AuditStatus.Voted;
            }
            var products = new SampleItemRoll().Where(item => item.SampleID == id);
            if (products != null)
            {
                entity.lstSampleItem = new List<SampleItem>();
                foreach (var item in products)
                {
                    var product = new SampleItem();
                    product.SpnID = item.SpnID;
                    product.SampleType = item.SampleType;
                    product.Quantity = item.Quantity;
                    product.Price = item.Price;
                    product.AuditStatus = entity.AuditStatus;
                    entity.lstSampleItem.Add(product);
                }
            }
            entity.EnterSuccess += Entity_EnterSuccess;
            entity.Approve();
        }


        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var sample = sender as Sample;
            Service.LogsOperating.LogOperating(Erp.Current, sample.ID, $"审批了送样申请:ID:{sample.ID}");
            if (sample.AuditStatus == AuditStatus.Normal)
            {
                (new ApplyTask
                {
                    MainID = sample.ID,
                    MainType = Underly.MainType.Clients,
                    ApproverID = sample.ApproverID,
                    ApproveDate = DateTime.Now,
                    ApplyTaskType = Underly.ApplyTaskType.ClientSample,
                    Status = Underly.ApplyStatus.Allowed

                }).Approve();
            }
            else
            {

                (new ApplyTask
                {
                    MainID = sample.ID,
                    MainType = Underly.MainType.Clients,
                    ApproverID = sample.ApproverID,
                    ApproveDate = DateTime.Now,
                    ApplyTaskType = Underly.ApplyTaskType.ClientSample,
                    Status = Underly.ApplyStatus.Voted

                }).Approve();

            }

            Easyui.Window.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }
    }
}