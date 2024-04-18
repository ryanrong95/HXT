using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.PvData.WebApp.SysConfig.Tariff
{
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string id = Request.QueryString["id"];

                if (string.IsNullOrEmpty(id))
                {
                    this.Model.Tariff = null;
                }
                else
                {
                    var tariff = Yahv.Erp.Current.PvData.Tariffs[id];
                    this.Model.Tariff = new
                    {
                        tariff.ID,
                        tariff.HSCode,
                        tariff.Name,
                        tariff.LegalUnit1,
                        tariff.LegalUnit2,
                        tariff.ImportPreferentialTaxRate,
                        tariff.ImportControlTaxRate,
                        tariff.ImportGeneralTaxRate,
                        tariff.VATRate,
                        tariff.ExciseTaxRate,
                        tariff.SupervisionRequirements,
                        tariff.CIQC,
                        tariff.CIQCode,
                        tariff.DeclareElements,
                    };
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string id = Request["id"];
            string hsCode = Request["hsCode"];
            string name = Request["name"];
            string legalUnit1 = Request["legalUnit1"];
            string legalUnit2 = Request["legalUnit2"];
            string supervisionRequirements = Request["supervisionRequirements"];
            string ciqC = Request["ciqC"];
            string ciqCode = Request["ciqCode"];
            string declareElements = Request["declareElements"];

            decimal importPreferentialTaxRate = decimal.Parse(Request["importPreferentialTaxRate"]);
            decimal? importControlTaxRate = null;
            if (!string.IsNullOrWhiteSpace(Request["importControlTaxRate"]))
                importControlTaxRate = decimal.Parse(Request["importControlTaxRate"]);
            decimal importGeneralTaxRate = decimal.Parse(Request["importGeneralTaxRate"]);
            decimal vatRate = decimal.Parse(Request["vatRate"]);
            decimal exciseTaxRate = decimal.Parse(Request["exciseTaxRate"]);

            var tariff = new YaHv.PvData.Services.Models.Tariff()
            {
                HSCode = hsCode,
                Name = name,
                LegalUnit1 = legalUnit1,
                LegalUnit2 = legalUnit2,
                ImportPreferentialTaxRate = importPreferentialTaxRate,
                ImportControlTaxRate = importControlTaxRate,
                ImportGeneralTaxRate = importGeneralTaxRate,
                VATRate = vatRate,
                ExciseTaxRate = exciseTaxRate,
                DeclareElements = declareElements,
                SupervisionRequirements = supervisionRequirements,
                CIQC = ciqC,
                CIQCode = ciqCode
            };
            tariff.Enter();

            if (string.IsNullOrEmpty(id))
            {
                Easyui.Dialog.Close("添加成功!", Web.Controls.Easyui.AutoSign.Success);
            }
            else
            {
                Easyui.Dialog.Close("编辑成功!", Web.Controls.Easyui.AutoSign.Success);
            }
        }
    }
}