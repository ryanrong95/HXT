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

namespace Yahv.CrmPlus.WebApp.Srm.SupplierDetails.Specials
{
    public partial class Add : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var pn = Erp.Current.CrmPlus.PartNumbers.Where(item => item.Status == Underly.DataStatus.Normal).GroupBy(item => item.PartNumber);
                this.Model.PartNumber = pn.Select(item => new
                {
                    text = item.Key

                });
                //this.Model.nBrandType = ExtendsEnum.ToArray<Underly.nBrandType>().Select(item => new
                //{
                //    value = (int)item,
                //    text = item.GetDescription()
                //});
            }
        }
        protected object getbrands()
        {
            string pn = Request["Pn"];
            return Erp.Current.CrmPlus.PartNumbers.Where(item => item.Status == Underly.DataStatus.Normal && item.PartNumber == pn).Select(item => new
            {
                text = item.Brand
            });
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string enterpriseid = Request.QueryString["id"];
            var entity = new Special();
            entity.EnterpriseID = enterpriseid;
            //entity.Brand = Request["Brand"];
            string pnid = Request.Form["Pn"];
            var pn = new Service.Views.Rolls.StandardPartNumbersRoll()[pnid];
            var brand = new Service.Views.Rolls.BrandsRoll()[pn.BrandID];
            entity.PartNumber = pn.PartNumber;
            entity.Brand = brand.Name;
            entity.Type = (Underly.nBrandType)int.Parse(Request.Form["Type"]);
            entity.Summary = Request.Form["Summary"];
            entity.CreatorID = Erp.Current.ID;
            #region 名片
            var file = Request.Form["FileForJson"];
            entity.SpecialFiles = file == null ? null : file.JsonTo<List<CallFile>>();
            #endregion
            entity.Repeat += Entity_Repeat;
            entity.EnterSuccess += Entity_EnterSuccess;
            entity.Enter();

        }

        private void Entity_Repeat(object sender, Usually.ErrorEventArgs e)
        {
            Easyui.Dialog.Close("已存在!", Web.Controls.Easyui.AutoSign.Error);
        }

        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as Special;
            Service.LogsOperating.LogOperating(Erp.Current, entity.ID, $"新增特色型号:{entity.ID}");
            Easyui.Dialog.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }
    }
}