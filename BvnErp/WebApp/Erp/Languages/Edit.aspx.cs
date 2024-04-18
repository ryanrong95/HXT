using Needs.Web.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Erp.Languages
{
    public partial class Edit : Needs.Web.Sso.Forms.ErpPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var id = Request["id"];
                if (!string.IsNullOrWhiteSpace(id))
                {
                    this.Model = new NtErp.Services.Views.LanguagesView().SingleOrDefault(item => item.ShortName == id);
                }
            }
        }

        /// <summary>
        /// Enter成功事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Entity_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Alert("保存成功", Request.Url, true);
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {

            var shortName = Request["shortName"];
            var displayName = Request["displayName"];
            var englishName = Request["englishName"];
            var dataName = Request["dataName"];

            if (string.IsNullOrWhiteSpace(shortName) || string.IsNullOrWhiteSpace(displayName) || string.IsNullOrWhiteSpace(englishName) || string.IsNullOrWhiteSpace(dataName))
            {
                Alert("必填项不能为空", Request.Url);
                return;
            }

            var entity = new NtErp.Services.Models.Language
            {
                ShortName = shortName,
                DisplayName = displayName,
                EnglishName = englishName,
                DataName = dataName
            };
            entity.EnterSuccess += Entity_EnterSuccess;
            entity.Enter();
        }
    }
}