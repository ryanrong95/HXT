using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApp.Crm.FinanceSubjects
{
    public partial class List : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 获取组织机构
        /// </summary>
        /// <returns></returns>
        protected object tree()
        {
            return new SubjectsTree().Json();
        }

        //删除节点
        protected void remove()
        {
            var id = Request["id"];
            if (!string.IsNullOrWhiteSpace(id))
            {
                new _Subject().Subs.Delete(id);
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Crm), "财务科目",
                    $"删除", id);
            }
        }

    }
}