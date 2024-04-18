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
    /// 北京总部管控的视图（Admin过滤）
    /// </summary>
    public class MyHQControlsView : HQControlsView
    {
        IGenericAdmin Admin;

        public MyHQControlsView(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.HQControl> GetIQueryable()
        {
            if (this.Admin.IsSa)
            {
                return base.GetIQueryable();
            }

            //TODO:暂时取消Admin过滤，通过配置菜单权限来控制
            return from control in base.GetIQueryable()
                   //where control.Admin.ID == this.Admin.ID &&
                   //     control.Status == Ccs.Services.Enums.Status.Normal
                   select control;
        }
    }
}
