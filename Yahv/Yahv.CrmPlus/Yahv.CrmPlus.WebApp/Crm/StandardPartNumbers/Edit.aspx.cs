using System;
using System.Collections.Generic;
using System.Linq;
using Yahv.CrmPlus.Service;
using Yahv.CrmPlus.Service.Models;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.CrmPlus.WebApp.Crm.StandardPartNumbers
{
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.Entity = new Yahv.CrmPlus.Service.Views.Rolls.StandardPartNumbersRoll()[Request.QueryString["id"]];
                this.Model.SpnCatalog = ExtendsEnum.ToArray<SpnCatalog>().Select(item => new
                {
                    value = item.GetDescription(),
                });
                //var files = new Service.Views.PcFilesView().Search(item => item.MainID == Request.QueryString["id"]).ToMyArray();

                //this.Model.Files = files;

            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var entity = new Yahv.CrmPlus.Service.Views.Rolls.StandardPartNumbersRoll()[Request.QueryString["id"]];
            if (entity == null)
            {
                Easyui.Alert("提示", "型号不存在!", Web.Controls.Easyui.Sign.Error);
            }
            //string PartNumber = Request["PartNumber"].Trim();
            //string Brand = Request["Brand"].Trim();
            string ProductName = Request["ProductName"].Trim();
            string DateCode = Request["DateCode"];
            string PackageCase = Request["PackageCase"];
            string Packaging = Request["Packaging"];
            string Moq = Request["Moq"];
            string Mpq = Request["Mpq"];
            string TaxCode = Request["TaxCode"];
            string TariffRate = Request["TariffRate"];
            string ECCN = Request["ECCN"].Trim();
            string Summary = Request["Summary"];

            //entity.PartNumber = PartNumber;
            //entity.Brand = Brand;
            entity.ProductName = ProductName;
            entity.Catalog = Request["Catalog"];
            //entity.DateCode = int.Parse(DateCode);
            entity.PackageCase = PackageCase;
            entity.Packaging = ProductName;
            entity.Moq = string.IsNullOrWhiteSpace(Moq) ? 0 : int.Parse(Moq);
            entity.Mpq = string.IsNullOrWhiteSpace(Mpq) ? 0 : int.Parse(Mpq);
            entity.TaxCode = TaxCode;
            entity.TariffRate = string.IsNullOrWhiteSpace(TariffRate) ? 0 : decimal.Parse(TariffRate);
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
            LogsOperating.LogOperating(Erp.Current, entity.ID, $"修改标准型号:{ entity.ID}");
            Easyui.Dialog.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }

        private void Entity_EnterError(object sender, Usually.ErrorEventArgs e)
        {
            //Easyui.Alert("提示", "保存失败!", Web.Controls.Easyui.Sign.Error);
            Easyui.Dialog.Close("保存失败!", Web.Controls.Easyui.AutoSign.Error);
        }

     
    }
}