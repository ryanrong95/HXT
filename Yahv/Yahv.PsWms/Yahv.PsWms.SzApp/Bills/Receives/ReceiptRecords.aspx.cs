using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Linq.Extends;
using Yahv.PsWms.SzMvc.Services.Models.Origin;
using Yahv.PsWms.SzMvc.Services.Views.Origins;
using Yahv.PsWms.SzMvc.Services.Views.Roll;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PsWms.SzApp.Bills.Receives
{
    public partial class ReceiptRecords : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <returns></returns>
        protected object data()
        {
            var ID = Request.QueryString["ID"];
            var PayerID = Request.QueryString["PayerID"];
            var CutDateIndex = int.Parse(Request.QueryString["CutDateIndex"]);

            if (!string.IsNullOrEmpty(ID))
            {
                var query = new PayeeRightsOrigin().Where(t => t.LeftID == ID).ToArray();
                return new
                {
                    rows = query.Select(t => new
                    {
                        t.ID,
                        t.LeftID,
                        CreateDate = t.CreateDate.ToString("yyyy-MM-dd"),
                        Price = t.Price.ToString("f2"),
                        t.AdminID,
                        t.FlowFormCode,
                    }),
                }.Json();
            }
            else
            {
                //所有实收
                var payeeLefts = new PayeeLeftsOrigin().Where(t => t.PayerID == PayerID && t.CutDateIndex == CutDateIndex).Select(t => t.ID).ToArray();
                var query = new PayeeRightsOrigin().Where(t => payeeLefts.Contains(t.LeftID)).ToArray();
                return new
                {
                    rows = query.Select(t => new
                    {
                        t.ID,
                        t.LeftID,
                        CreateDate = t.CreateDate.ToString("yyyy-MM-dd"),
                        Price = t.Price.ToString("f2"),
                        t.AdminID,
                        t.FlowFormCode,
                    }),
                }.Json();
            }
        }
    }
}