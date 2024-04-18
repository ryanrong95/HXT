using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.PvData.WebApp.SysConfig.OriginDisinfection
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
                    this.Model.OriginDisinfection = null;
                }
                else
                {
                    var originRate = Yahv.Erp.Current.PvData.OriginsDisinfection[id];
                    this.Model.OriginDisinfection = new
                    {
                        originRate.ID,
                        Origin = Enum.Parse(typeof(Origin), originRate.Origin),
                        originRate.StartDate,
                        originRate.EndDate
                    };
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string id = Request["id"];
            string origin = Request["origin"];

            DateTime startDate;
            DateTime? endDate = null;

            DateTime.TryParse(Request["startDate"], out startDate);
            if (!string.IsNullOrEmpty(Request["endDate"]))
            {
                endDate = DateTime.Parse(Request["endDate"]);
            }

            var originRate = Yahv.Erp.Current.PvData.OriginsDisinfection[id] ?? new YaHv.PvData.Services.Models.OriginDisinfection();
            originRate.Origin = ((Origin)Enum.Parse(typeof(Origin), origin)).GetOrigin().Code;
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