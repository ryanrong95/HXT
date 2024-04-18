using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.SysConfig.DomesticExpress.Type
{
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            PageInit();
        }
        private void PageInit()
        {
            string ID = Request.QueryString["ID"];
            this.Model.IDdate = ID;
        }
        /// <summary>
        /// 加载
        /// </summary>
        protected void data()
        {
            string ID = Request.QueryString["ID"];
            string Name = Request.QueryString["Name"];
            var list = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.ExpressTypes.Where(item => item.ExpressCompany.ID == ID)
                .AsQueryable().Where(item => item.Status == Needs.Ccs.Services.Enums.Status.Normal);
            if (!string.IsNullOrEmpty(Name))
            {
                Name = Name.Trim();
                list = list.Where(t => t.TypeName.Contains(Name));
            }
            Func<Needs.Ccs.Services.Models.ExpressType, object> convert = item => new
            {
                ID = item.ID,
                Name = item.ExpressCompany.Name,
                Code = item.ExpressCompany.Code,
                TypeName = item.TypeName,
                TypeValue = item.TypeValue,
            };
            this.Paging(list, convert);
        }

        /// <summary>
        /// 数据删除
        /// </summary>
        protected void Delete()
        {
            string id = Request.Form["ID"];
            var del = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.ExpressTypes[id];
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