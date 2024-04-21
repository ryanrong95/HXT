using System;
using Yahv.CrmPlus.Service;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Web.Erp;
using Yahv.Web.Forms;
using System.Linq;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using System.Collections.Generic;
using Yahv.CrmPlus.Service.Models;

namespace Yahv.CrmPlus.WebApp.Crm.StandardPartNumbers
{
    public partial class Add : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //this.Model.Brands = new Service.Views.Rolls.BrandsRoll().Where(item => item.Status == Underly.DataStatus.Normal).Select(item => new
                //{
                //    ID = item.ID,
                //    text = item.Name
                //});
                this.Model.SpnCatalog = ExtendsEnum.ToArray<SpnCatalog>().Select(item => new
                {
                    value = item.GetDescription(),
                });
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string PartNumber = Request["PartNumber"];
            string BrandID = Request["Brand"];
            string ProductName = Request["ProductName"];
            string DateCode = Request["DateCode"];
            string PackageCase = Request["PackageCase"];
            string Packaging = Request["Packaging"];
            string Moq = Request["Moq"];
            string Mpq = Request["Mpq"];
            string TaxCode = Request["TaxCode"];
            string TariffRate = Request["TariffRate"];
            string ECCN = Request["ECCN"];
            string Summary = Request["Summary"];
            var entity = new Yahv.CrmPlus.Service.Models.Origins.StandardPartNumber();
            entity.PartNumber = PartNumber;
            entity.BrandID = BrandID;
            entity.ProductName = ProductName;
            entity.Catalog = Request["Catalog"];
            entity.PackageCase = PackageCase;
            entity.Packaging = Packaging;
            entity.Moq = int.Parse(Moq);
            entity.Mpq = int.Parse(Mpq);
            entity.TaxCode = TaxCode;
            entity.TariffRate = decimal.Parse(TariffRate);
            entity.Eccn = ECCN;
            entity.Ccc = Request["Ccc"] != null;
            entity.Summary = Summary;
            entity.CreatorID = Erp.Current.ID;

            #region 文件
            var pcns = Request.Form["PcnForJson"];
            var datasheet = Request.Form["DataSheetForJson"];
            List<PcFile> list = new List<PcFile>();
            if (!string.IsNullOrEmpty(pcns))
            {
                var callfile = pcns.JsonTo<List<CallFile>>().First();
                list.Add(new PcFile()
                {
                    CreatorID = Erp.Current.ID,
                    Type = CrmFileType.Pcn,
                    Url = callfile.CallUrl,
                    CustomName = callfile.FileName,
                });
            }
            if (!string.IsNullOrEmpty(datasheet))
            {
                var callfile = datasheet.JsonTo<List<CallFile>>().First();
                list.Add(new PcFile()
                {
                    CreatorID = Erp.Current.ID,
                    Type = CrmFileType.DataSheet,
                    Url = callfile.CallUrl,
                    CustomName = callfile.FileName,
                });
            }
            entity.files = list.ToArray();
            #endregion
            entity.EnterError += Entity_EnterError; ;
            entity.EnterSuccess += Entity_EnterSuccess;
            entity.Repeat += Entity_Repeat;
            entity.Enter();
        }

        private void Entity_Repeat(object sender, Usually.ErrorEventArgs e)
        {
            Easyui.Dialog.Close("型号已存在!", Web.Controls.Easyui.AutoSign.Error);
        }

        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as StandardPartNumber;
            LogsOperating.LogOperating(Erp.Current, entity.ID, $"新增标准型号:{ entity.ID}");
            Easyui.Dialog.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }

        private void Entity_EnterError(object sender, Usually.ErrorEventArgs e)
        {
            Easyui.Dialog.Close("保存失败!", Web.Controls.Easyui.AutoSign.Error);
        }
    }
}