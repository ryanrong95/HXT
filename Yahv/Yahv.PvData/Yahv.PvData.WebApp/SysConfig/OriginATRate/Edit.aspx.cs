using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.PvData.WebApp.SysConfig.OriginATRate
{
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.Origin = ExtendsEnum.ToDictionary<Origin>().Select(item => new
                {
                    value = item.Key,
                    text = Enum.Parse(typeof(Origin), item.Key) + "-" + item.Value,
                });

                string id = Request.QueryString["id"];
                if (string.IsNullOrEmpty(id))
                {
                    this.Model.OriginATRate = null;
                }
                else
                {
                    var originRate = Yahv.Erp.Current.PvData.OriginsATRate[id];
                    this.Model.OriginATRate = new
                    {
                        originRate.ID,
                        originRate.Tariff.HSCode,
                        originRate.Tariff.Name,
                        Origin = Enum.Parse(typeof(Origin), originRate.Origin),
                        originRate.Rate,
                        originRate.StartDate,
                        originRate.EndDate
                    };
                }
            }
        }

        protected object GetHSCodes()
        {
            var hsCode = Request.Form["hsCode"];
            return Yahv.Erp.Current.PvData.Tariffs.Where(t => t.HSCode.StartsWith(hsCode.Trim()))
                .Take(10).Select(t => new
                {
                    value = t.ID,
                    text = t.HSCode
                });
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string id = Request["id"];
            string hsCode = Request["hsCode"];
            string origin = Request["origin"];

            decimal rate;
            DateTime startDate;
            DateTime? endDate = null;

            decimal.TryParse(Request["rate"], out rate);
            DateTime.TryParse(Request["startDate"], out startDate);
            if (!string.IsNullOrEmpty(Request["endDate"]))
            {
                endDate = DateTime.Parse(Request["endDate"]);
            }

            var originRate = Yahv.Erp.Current.PvData.OriginsATRate[id] ?? new YaHv.PvData.Services.Models.OriginATRate();
            originRate.TariffID = hsCode;
            originRate.Origin = ((Origin)Enum.Parse(typeof(Origin), origin)).GetOrigin().Code;
            originRate.Rate = rate;
            originRate.StartDate = startDate;
            originRate.EndDate = endDate;
            originRate.Enter();

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