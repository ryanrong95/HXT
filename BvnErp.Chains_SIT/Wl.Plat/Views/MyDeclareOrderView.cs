using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Views;
using Needs.Wl.User.Plat.Models;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Needs.Wl.User.Plat.Views
{
    /// <summary>
    /// 报关单
    /// </summary>
    public class MyDeclareOrderView : Needs.Ccs.Services.Views.ClientDecHeadView
    {
        IPlatUser User;

        internal MyDeclareOrderView(IPlatUser user)
        {
            this.User = user;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.ClientDecHead> GetIQueryable()
        {
            var linq = from entity in base.GetIQueryable()
                       join client in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>() on entity.ClientID equals client.ID
                       where client.ClientType == (int)ClientType.Internal && entity.IsSuccess
                       select entity;
            if (this.User.IsMain)
            {
                return from entity in linq
                       where entity.ClientID == this.User.Client.ID
                       orderby entity.CreateTime descending
                       select entity;
            }
            else
            {
                return from entity in linq
                       where entity.UserID == this.User.ID
                       orderby entity.CreateTime descending
                       select entity;
            }
        }
    }

    /// <summary>
    /// 报关单数据
    /// </summary>
    public class ClientOrderDataView : Needs.Ccs.Services.Views.SZInput1View
    {
        IPlatUser User;

        internal ClientOrderDataView(IPlatUser user)
        {
            this.User = user;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.SZInput> GetIQueryable(Expression<Func<Needs.Ccs.Services.Models.SZInput, bool>> expression, params LambdaExpression[] expressions)
        {
            var linq = from entity in base.GetIQueryable(expression, expressions)
                       join client in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>() on entity.ClientID equals client.ID
                       where client.ClientType == (int)ClientType.Internal
                       select entity;
            if (this.User.IsMain)
            {
                return linq.Where(item => item.ClientID == this.User.ClientID).OrderByDescending(item => item.CreateDate);
            }
            return linq.Where(item => item.UserID == this.User.ID).OrderByDescending(item => item.CreateDate);
        }
    }

    /// <summary>
    /// 用于excel导出
    /// </summary>
    public class ClientOrderDataExportView: SZInputView
    {
        IPlatUser User;

        internal ClientOrderDataExportView(IPlatUser user)
        {
            this.User = user;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.SZInput> GetIQueryable()
        {
            var linq = from entity in base.GetIQueryable()
                       join client in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>() on entity.ClientID equals client.ID
                       where client.ClientType == (int)ClientType.Internal 
                       select entity;
            if (this.User.IsMain)
            {
                return from entity in linq
                       where entity.ClientID == this.User.Client.ID
                       orderby entity.CreateDate descending
                       select entity;
            }
            else
            {
                return from entity in linq
                       where entity.UserID == this.User.ID
                       orderby entity.CreateDate descending
                       select entity;
            }
        }
    }
}
