using Needs.Web.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Erp.Languages
{
    public partial class List : Needs.Web.Sso.Forms.ErpPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 列表数据
        /// </summary>
        /// <returns></returns>
        protected object data()
        {
            var list = new NtErp.Services.Views.LanguagesView();

            return list;
        }
        /// <summary>
        /// 删除项
        /// </summary>
        /// <returns></returns>
        protected bool del()
        {
            try
            {
                var id = Request["id"];

                var entity = new NtErp.Services.Views.LanguagesView().SingleOrDefault(item => item.ShortName == id);
                if (entity != null)
                {
                    entity.AbandonSuccess += Entity_AbandonSuccess;
                    entity.Abandon();
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// 删除成功事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Entity_AbandonSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Alert("删除成功");
        }
    }
}