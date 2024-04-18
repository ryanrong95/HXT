using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Linq.Extends;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using YaHv.PvData.Services;
using YaHv.PvData.Services.Utils;

namespace Yahv.PvData.WebApp.SysConfig.ClassifyHistory
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                init();
            }
        }

        void init()
        {
            this.Model.Admin = new
            {
                ID = Yahv.Erp.Current.ID,
                UserName = Yahv.Erp.Current.UserName,
                RealName = Yahv.Erp.Current.RealName
            }.Json();

            var pvdataApi = new PvDataApiSetting();

            this.Model.DomainUrls = new
            {
                PvDataApiUrl = ConfigurationManager.AppSettings[pvdataApi.ApiName],
            }.Json();
        }

        protected object data()
        {
            int pageIndex = 1; int.TryParse(Request.QueryString["page"], out pageIndex);
            int pageSize = 20; int.TryParse(Request.QueryString["rows"], out pageSize);
            string partNumber = Request["partNumber"];
            string manufacturer = Request["manufacturer"];
            string hsCode = Request["hsCode"];
            string name = Request["name"];

            int total = 0;
            var task = Task.Run(() => total = SqlView.GetClassifiedHistoriesCount(partNumber, manufacturer, hsCode, name));
            var data = SqlView.ClassifiedHistories(pageIndex, pageSize, partNumber, manufacturer, hsCode, name);
            var rows = data.Select(item => new
            {
                item.PartNumber,
                item.Manufacturer,
                item.HSCode,
                item.TariffName,
                item.TaxCode,
                item.TaxName,
                item.LegalUnit1,
                item.LegalUnit2,
                ImportPreferentialTaxRate = item.ImportTaxRate,
                item.VATRate,
                item.ExciseTaxRate,
                Elements = item.Elements.FixSpecialChars(),
                item.CIQCode,
                item.SpecialTypes,

                item.ID,
                item.Ccc,
                item.Embargo,
                item.HkControl,
                item.Coo,
                item.CIQ,
                item.CIQprice,
                item.Summary,
            }).ToArray();

            task.Wait();

            return new
            {
                rows = data,
                total = total
            };
        }
    }
}