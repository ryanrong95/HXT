using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.PvData.WebApp.SysConfig.PresetExchangeRate
{
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string opt = Request.QueryString["opt"];
                string type = Request.QueryString["type"];
                District district;
                Currency from, to;

                Enum.TryParse(Request.QueryString["from"], out from);
                Enum.TryParse(Request.QueryString["to"], out to);
                Enum.TryParse(Request.QueryString["district"], out district);

                if (opt == "edit")
                {
                    var exchangeRate = Yahv.Erp.Current.PvData.ExchangeRates[type, district, from, to];
                    this.Model.ExchangeRate = new
                    {
                        exchangeRate.Type,
                        exchangeRate.District,
                        exchangeRate.From,
                        exchangeRate.To,
                        exchangeRate.Value,
                        StartDate = exchangeRate.StartDate?.ToShortDateString()
                    };
                }
                else if (opt == "create")
                {
                    this.Model.ExchangeRate = null;
                }

                this.Model.ExchangeCurrency = ExtendsEnum.ToDictionary<Currency>()
                    .Where(item => item.Key == "1" || item.Key == "2" || item.Key == "3").Select(item => new
                    {
                        value = item.Key,
                        text = item.Value,
                    });

                this.Model.ExchangeDistrict = ExtendsEnum.ToDictionary<District>()
                    .Where(item => item.Key == "1" || item.Key == "2").Select(item => new
                    {
                        value = item.Key,
                        text = item.Value,
                    });
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            //string type = Request["type"];
            string type = "Preset";
            string opt = Request["opt"];
            District district;
            Currency from, to;

            Enum.TryParse(Request["from"], out from);
            Enum.TryParse(Request["to"], out to);
            Enum.TryParse(Request["district"], out district);

            decimal rate = Convert.ToDecimal(Request["rate"]);
            DateTime startDate = Convert.ToDateTime(Request["startDate"]);

            var exchangeRate = new YaHv.PvData.Services.Models.ExchangeRate
            {
                District = district,
                From = from,
                To = to,
                Type = type,
                Value = rate,
                StartDate = startDate
            };

            exchangeRate.Enter();
            this.Model.ExchangeRate = exchangeRate;
            if (opt == "create")
            {
                Easyui.Dialog.Close("添加成功!", Web.Controls.Easyui.AutoSign.Success);
            }
            else if (opt == "edit")
            {
                Easyui.Dialog.Close("编辑成功!", Web.Controls.Easyui.AutoSign.Success);
            }
        }
    }
}