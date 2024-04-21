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

namespace Yahv.CrmPlus.WebApp.Crm.Samples
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
            this.Model.Entity = Erp.Current.CrmPlus.MySamples[id];
            this.Model.Clients = Erp.Current.CrmPlus.MyClients.Where(x => x.Status == Underly.AuditStatus.Normal && x.IsDraft == false).Select(x => new { value = x.ID, text = x.Name });
            this.Model.StandardPartNumber = new CrmPlus.Service.Views.Rolls.StandardPartNumbersRoll().Select(x => new { PartNumber = x.PartNumber, SpnID = x.ID, Brand = x.Brand });
            this.Model.SampleTypes = ExtendsEnum.ToDictionary<SampleType>().Select(item => new
            {
                value = int.Parse(item.Key),
                text = item.Value.ToString()
            });
        }

        protected object getInfoByClient()
        {
            var id = Request.Form["ID"];
            var projects = Erp.Current.CrmPlus.MyProjects.Where(x => x.EndClientID == id).Select(x => new { value = x.ID, text = x.Name });
            var contacts = Erp.Current.CrmPlus.MyContacts.Where(x => x.EnterpriseID == id).Select(x => new { value = x.ID, text = $"{x.Name}-{x.Mobile}" });
            var address = Erp.Current.CrmPlus.Addresses[id, RelationType.Trade].Select(x => new { value = x.ID, text = x.Context });
            var data = new { projects, address, contacts };

            return data;
        }

        protected object data()
        {

            string id = Request.QueryString["id"];
            var query = new SampleItemRoll().Where(x => x.SampleID == id);
            var result = this.Paging(query.OrderByDescending(item => item.CreateDate).ToArray().Select(item => new
            {
                item.ID,
                item.SampleID,
                item.Price,
                item.Quantity,
                item.Total,
                item.SpnID,
                item.SampleType,
                SampleTypeDes = item.SampleType.GetDescription(),
                CreteDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                item.AuditStatus,
            }));

            return result;

        }


        protected void Save()
        {
            var id = Request.Form["ID"];
            var deliveryDate = Request.Form["DeliveryDate"];
            //var clientName = Request.Form["ClientName"];
            var name = Request.Form["Name"];
            var contact = Request.Form["Contact"];
            var address = Request.Form["Address"];
            string strProducts = Request.Form["products"].Replace("&quot;", "'");
            var products = strProducts.JsonTo<List<dynamic>>();
            var entity = Erp.Current.CrmPlus.MySamples[id];
            entity.DeliveryDate = Convert.ToDateTime(deliveryDate);
            entity.ProjectID = name;
            entity.AddressID = address;
            entity.ContactID = contact;
            entity.ApplierID = Erp.Current.ID;
            entity.AuditStatus = AuditStatus.Waiting;
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
                    product.AuditStatus = AuditStatus.Waiting;
                    entity.lstSampleItem.Add(product);
                }
            }
            entity.EnterSuccess += Entity_EnterSuccess;
            entity.Enter();
        }


        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var sample = sender as Yahv.CrmPlus.Service.Models.Origins.Sample;
            Service.LogsOperating.LogOperating(Erp.Current, sample.ID, $"编辑了送样申请:ID:{sample.ID}");
            Response.Write((new { success = true, message = "保存成功！" }).Json());
        }
    }
}