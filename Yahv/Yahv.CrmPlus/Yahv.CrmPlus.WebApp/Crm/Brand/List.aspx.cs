using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Underly;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.CrmPlus.WebApp.Crm.Brand
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }


        protected object data()
        {
            using (var view = new Service.Views.Rolls.BrandsRoll())
            {

                var query = view;
                var name = Request["s_name"];
                if (!string.IsNullOrWhiteSpace(name))
                {
                    query = query.SearchByName(name);
                }
                var iquery = query.Search(this.GetPredicate());

                return this.Paging(iquery.OrderByDescending(item => item.CreateDate), item => new
                {
                    item.ID,
                    item.Name,
                    item.Code,
                    item.IsAgent,
                    item.ChineseName,
                    Creator = item.Admin?.RealName,
                    CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    ModifyDate= item.ModifyDate.ToString("yyyy-MM-dd HH:mm:ss"),
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
