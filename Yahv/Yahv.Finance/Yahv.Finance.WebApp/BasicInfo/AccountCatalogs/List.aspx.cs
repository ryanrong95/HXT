using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Views.Rolls;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Underly.Enums;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.Finance.WebApp.BasicInfo.AccountCatalogs
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
            return new AccountCatalogsTree().Json();
        }

        protected void remove()
        {
            var id = Request["id"];
            if (!string.IsNullOrWhiteSpace(id))
            {
                new AccountCatalog().Abandon(id);
                Services.Oplogs.Oplog(Erp.Current.ID, LogModular.账款类型管理, Services.Oplogs.GetMethodInfo(), "删除", id);
            }
        }
    }
}