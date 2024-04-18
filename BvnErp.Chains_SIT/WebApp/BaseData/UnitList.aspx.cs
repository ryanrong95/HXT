using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.BaseData
{
    public partial class UnitList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }
        }

        /// <summary>
        /// 初始化计量单位数据
        /// </summary>
        protected void data()
        {
            string code = Request.QueryString["Code"];
            string name = Request.QueryString["Name"];

            var units = Needs.Wl.Admin.Plat.AdminPlat.Units.AsQueryable();
            if (!string.IsNullOrEmpty(code))
            {
                units = units.Where(item => item.Code.Contains(code));
            }
            if (!string.IsNullOrEmpty(name))
            {
                units = units.Where(item => item.Name.Contains(name));
            }

            //对象转化
            Func<Needs.Ccs.Services.Models.Unit, object> convert = unit => new
            {
                unit.Code,
                unit.Name
            };

            this.Paging(units, convert);
        }
    }
}