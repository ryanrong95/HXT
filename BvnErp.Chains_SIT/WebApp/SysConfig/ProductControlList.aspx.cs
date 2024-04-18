using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Needs.Utils.Linq;

namespace WebApp.SysConfig
{
    public partial class ProductControlList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //产品管控类型
            this.Model.ProductControlType = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.ProductControlType>().Select(item => new { ID = item.Key, Name = item.Value }).Json();
        }

        /// <summary>
        /// 初始化包装种类数据
        /// </summary>
        protected void data()
        {
            //品名 型号 管控类型进行筛选
            string model = Request.QueryString["Model"];
            string name = Request.QueryString["Name"];
            string Type = Request.QueryString["ControlType"];
            string sortField = Request.QueryString["sort"]; //ID
            string order = Request.QueryString["order"]; //DESC

            var products = Needs.Wl.Admin.Plat.AdminPlat.ProductControls.AsQueryable();
            if (string.IsNullOrEmpty(sortField) == false)
            {
                products = products.OrderBy(sortField, order == "desc");
            }
            if (!string.IsNullOrEmpty(Type))
            {
                products = products.Where(item => item.Type.ToString()==Type);
            }
            if (!string.IsNullOrEmpty(model))
            {
                products = products.Where(item => item.Model.Contains(model));
            }
            if (!string.IsNullOrEmpty(name))
            {
                products = products.Where(item => item.Name.Contains(name));
            }
            Func<Needs.Ccs.Services.Models.ProductControl, object> convert = head => new
            {
                ID = head.ID,
                Name = head.Name,
                Model = head.Model,
                Manufacturer = head.Manufacturer,
                Type = (head.Type== Needs.Ccs.Services.Enums.ProductControlType.CCC)? "CCC": "禁运",
                CreateDate = head.CreateDate.ToShortDateString(),
            };
            this.Paging(products, convert);
        }

        protected void Delete()
        {
            string id = Request.Form["ID"];
            var del = Needs.Wl.Admin.Plat.AdminPlat.ProductControls[id];
            if (del != null)
            {
                del.AbandonSuccess += Del_AbandonSuccess;
                del.Abandon();
            }
        }

        /// <summary>
        /// 删除成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Del_AbandonSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Alert("删除成功!");
        }
    }
}