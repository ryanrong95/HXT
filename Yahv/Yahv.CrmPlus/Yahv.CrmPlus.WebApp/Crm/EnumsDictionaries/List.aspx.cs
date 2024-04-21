using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.CrmPlus.WebApp.Crm.EnumsDictionaries
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.Enum = Request.QueryString["Enum"]; 
            }
        }
        protected object data()
        {
            string enumname = Request["Enum"];
            var query = new EnumsDictionariesRoll().Where(item => item.Enum == enumname);
            return this.Paging(query, item => new
            {
                item.ID,
                item.Enum,
                item.IsFixed,
                item.Description,
                item.Field,
                item.Value,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd"),
            });
        }
        bool delsuccess = false;
        protected bool del()
        {
            try
            {
                string id = Request["id"];
                var entity = new CrmPlus.Service.Views.Rolls.EnumsDictionariesRoll()[id];
                entity.AbandonSuccess += Entity_AbandonSuccess;
                entity.Abandon();
                return delsuccess;
            }
            catch
            {
                return false;
            }

        }

        private void Entity_AbandonSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as Service.Models.Origins.EnumsDictionary;
            Service.LogsOperating.LogOperating(Erp.Current, entity.ID, $"Enum删除:{entity.Json()}");
            delsuccess = true;
        }

        #region 初始化固定数据
        protected void InitialEnum()
        {
            Service.InitialEnum.Execute();
        }
        #endregion
    }
}