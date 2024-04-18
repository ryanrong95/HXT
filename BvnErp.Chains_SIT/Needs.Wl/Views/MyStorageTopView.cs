using Needs.Ccs.Services.Views;
using Needs.Erp.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Admin.Plat.Views
{
    /// <summary>
    /// 会员的视图(Admin过滤)
    /// </summary>
    public sealed class MyStorageTopView : StoragesTopOriginalView
    {
        IGenericAdmin Admin;

        public MyStorageTopView(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.StorageTop> GetIQueryable()
        {
            if (this.Admin.IsSa)
            {
                return base.GetIQueryable();
            }
           
            //过滤业务员、跟单员的数据
            return from client in base.GetIQueryable()
                   where client.MyClient.Merchandiser.ID == this.Admin.ID                  
                   select client;
        }
    }
}