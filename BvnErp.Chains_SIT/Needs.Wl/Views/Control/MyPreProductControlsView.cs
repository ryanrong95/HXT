using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views.Alls;
using Needs.Erp.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Admin.Plat.Views
{
    /// <summary>
    /// 待审批预归类产品
    /// </summary>
    public class MyPreProductControlsView : PreProductControlsAll
    {
        IGenericAdmin Admin;

        public MyPreProductControlsView(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        protected override IQueryable<PreProductControl> GetIQueryable(Expression<Func<PreProductControl, bool>> expression, params LambdaExpression[] expressions)
        {
            //取消Admin过滤，通过配置菜单权限来控制
            return from entity in base.GetIQueryable(expression, expressions)
                   where entity.Status == Ccs.Services.Enums.PreProductControlStatus.Waiting
                   select entity;
        }
    }
}
