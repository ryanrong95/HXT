using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Yahv.Erm.Services;
using Yahv.Web.Erp;

namespace Yahv.Erm.WebApp.Erm.Staffs
{
    public partial class PayBill : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var id = Request.QueryString["ID"];
                var staffPostion = Alls.Current.Staffs[id].PostionID;
                this.Model.PositionItems = Alls.Current.WageItems.Where(item => Alls.Current.Postions.PositionWages(staffPostion).Contains(item.ID));
            }
        }
        /// <summary>
        /// 数据查询
        /// </summary>
        /// <returns></returns>
        protected object data()
        {
            var id = Request.QueryString["ID"];
            var staffPostion = Alls.Current.Staffs[id].PostionID;
            var mapwageitems = Alls.Current.MyWageItems
                .Where(item => Alls.Current.Postions.PositionWages(staffPostion).Contains(item.WageItemID))
                .Where(item => item.ID == id).Select(item => new
                {
                    item.ID,
                    item.WageItemID,
                    item.DefaultValue
                });
            List<string> dynColumns;
            if (!mapwageitems.Any())
            {
                return null;
            }
            return ExportWages.Current.DynamicLinq(mapwageitems.ToList(), GetFixedColumns(), "WageItemID", "DefaultValue", out dynColumns); ;
        }
        /// <summary>
        /// 获取固定列
        /// </summary>
        /// <returns></returns>
        private List<string> GetFixedColumns()
        {
            return new List<string>()
                    {
                         "ID",
                    };
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {

        }
    }
}