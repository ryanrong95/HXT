using System;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Erm.Services.Views.Rolls;
using Yahv.Underly;
using Yahv.Web.Erp;

namespace Yahv.Erm.WebApp.Erm.Leagues
{
    public partial class List : ErpParticlePage
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
            return new LeaguesTree(Category.Work).Json();
        }

        //删除节点
        protected void remove()
        {
            var id = Request["id"];
            if (!string.IsNullOrWhiteSpace(id))
            {
                new League().Subs.Delete(id);

                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "组织机构",
                    $"删除", id);
            }
        }
    }
}