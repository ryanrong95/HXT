using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.CrmPlus.WebApp.Srm.SupplierDetails.FixedSuppliers
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.Origin = ExtendsEnum.ToArray<Underly.Origin>(Underly.Origin.Unknown, Underly.Origin.NG).Select(item => new
                {
                    value = item.GetDescription()
                });
                this.Model.EnterpriseID = Request.QueryString["enterpriseid"];
            }
            var supplier = Erp.Current.CrmPlus.Suppliers[Request.QueryString["enterpriseid"]];
            this.Model.Entity = supplier.FiexedSupplier;
            //this.Model.PricingRules = new FilesDescriptionRoll()[supplier.ID, CrmFileType.PricingRules].ToArray();

        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string id = Request.QueryString["enterpriseid"];
            string cutoffTime = Request.Form["CutoffTime"];
            QuoteMethod QuoteMethod = (QuoteMethod)int.Parse(Request.Form["QuoteMethod"]);
            string DeliveryPlace = Request.Form["DeliveryPlace"];
            FreightPayer FreightPayer = (FreightPayer)int.Parse(Request.Form["FreightPayer"]);
            decimal Mop = decimal.Parse(Request.Form["Mop"]);
            string WaybillFrom = Request.Form["WaybillFrom"];
            string DeliveryTime = Request.Form["DeliveryTime"];
            string BatchMethod = Request.Form["BatchMethod"];
            bool IsOriginPi = Request.Form["IsOriginPi"] != null;
            bool IsDelegatePay = Request.Form["IsDelegatePay"] != null;
            bool IsWaybillPi = Request.Form["IsWaybillPi"] != null;
            bool IsNotcieShiped = Request.Form["IsNotcieShiped"] != null;
            FixedSupplier entity = new FixedSupplier();
            entity.ID = id;
            //entity.CutoffTime = TimeSpan.Parse(cutoffTime);

            if (!string.IsNullOrWhiteSpace(cutoffTime))
            {
                entity.CutoffTime = TimeSpan.Parse(cutoffTime);
            }
            entity.QuoteMethod = QuoteMethod;
            entity.DeliveryPlace = DeliveryPlace;
            entity.DeliveryTime = DeliveryTime;
            entity.BatchMethod = BatchMethod;
            entity.FreightPayer = FreightPayer;
            entity.Mop = Mop;
            //entity.CompanyID = CompanyID;
            entity.IsDelegatePay = IsDelegatePay;
            entity.IsNotcieShiped = IsNotcieShiped;
            entity.IsOriginPi = IsOriginPi;
            entity.IsWaybillPi = IsWaybillPi;
            entity.WaybillFrom = WaybillFrom;
            entity.CreatorID = Erp.Current.ID;
            #region 文件
            var priceRules = Request.Form["PriceRulesForJson"];
            entity.PriceRules = priceRules == null ? null : priceRules.JsonTo<List<CallFile>>();
            #endregion
            entity.EnterSuccess += Entity_EnterSuccess;
            entity.Enter();
        }

        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as FixedSupplier;
            Service.LogsOperating.LogOperating(Erp.Current, entity.ID, $"FixedSupplier信息:{entity.Json()}");
            Easyui.Reload("", "保存成功!", Web.Controls.Easyui.Sign.Info);
        }

        protected object data()
        {
            string enterpriseid = Request.QueryString["enterpriseid"];
            var query = new Service.Views.Rolls.nFixedBrandsRoll().Where(item => item.EnterpriseID == enterpriseid);
            return this.Paging(query.ToArray().Select(item => new
            {
                item.ID,
                EnterpriseID = item.EnterpriseID,
                Brand = item.Brand,
                IsProhibited = item.IsProhibited,
                IsPromoted = item.IsPromoted,
                IsDiscounted = item.IsDiscounted,
                IsAdvantaged = item.IsAdvantaged,
                IsSpecial = item.IsSpecial,
                Summary = item.Summary,
                CreatorID = item.CreatorID,

            }));
        }
        bool success = false;
        protected bool del()
        {
            string id = Request["id"];
            var entity = new nFixedBrandsRoll()[id];
            entity.AbandonSuccess += Entity_AbandonSuccess;
            entity.Abandon();
            return success;
        }

        private void Entity_AbandonSuccess(object sender, Usually.AbandonedEventArgs e)
        {
            success = true;
            var entity = sender as nFixedBrand;
            Service.LogsOperating.LogOperating(Erp.Current, entity.ID, $"删除nFixedBrands:{entity.Json()}");
        }

    }
}