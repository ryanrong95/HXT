using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.CrmPlus.WebApp.Crm.StandardBrand
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }


        protected object data()
        {
            using (var view = new Service.Views.StandardBrandView())
            {

                var query = view;
                var name = Request["s_name"];
                var pm = Request["s_admin"];
                var company = Request["company"];
                var supplier = Request["supplier"];
               
                if (!string.IsNullOrWhiteSpace(name))
                {
                    query = query.SearchByName(name);
                }

                if (!string.IsNullOrEmpty(company))
                {
                    query = query.SerchByCompany(company);
                }
                if (!string.IsNullOrEmpty(supplier))
                {
                    query = query.SerchBySupplier(supplier);
                }
                query = query.Search(this.GetPredicate());

                return this.Paging(query.ToMyArray().OrderByDescending(item => item.CreateDate), item => new
                {
                    item.ID,
                    item.Name,
                    item.Code,
                    IsAgent= item.IsAgent?"是":"否",
                    item.ChineseName,
                    PMs = string.Join(",", item.PMs),
                    PMAs = string.Join(",", item.PMAs),
                    FAEs = string.Join(",", item.FAEs),
                    Companys = string.Join(",", item.Companys),
                    Suppliers = string.Join(",", item.Suppliers),
                    CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    ModifyDate = item.ModifyDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    item.Summary,
                    Status = item.Status,
                    StatusDes = item.Status.GetDescription()
                });

            }
        }

        /// <summary>
        /// 获取条件
        /// </summary>
        /// <returns></returns>
        Expression<Func<Yahv.CrmPlus.Service.Models.Origins.StandardBrand, bool>> GetPredicate()
        {
            Expression<Func<Yahv.CrmPlus.Service.Models.Origins.StandardBrand, bool>> predicate = item => true;
            var isAgent = Request["isAgent"];

            if (isAgent == "true")
            {
                predicate = predicate.And(item => item.IsAgent == true);
            }

            return predicate;

        }


        protected void del()
        {
            var id = Request.Form["id"];

            var entity = Erp.Current.CrmPlus.StandardBrands[id];
            entity.AbandonSuccess += Entity_AbandonSuccess;
            entity.Abandon();
        }


        private void Entity_AbandonSuccess(object sender, Usually.SuccessEventArgs e)
        {
            Easyui.Window.Close("停用成功!", Web.Controls.Easyui.AutoSign.Success); ;
        }
    }
}
