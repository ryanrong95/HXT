using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Underly.Erps;

namespace Yahv.Services.Views
{
    public class TradingInvoicesTopView<TReponsitory> : InvoicesTopView<TReponsitory>
        where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public TradingInvoicesTopView()
        {

        }
        public TradingInvoicesTopView(TReponsitory reponsitory) : base(reponsitory)
        {

        }
        override protected IQueryable<Layers.Data.Sqls.PvbCrm.MapsBEnterTopView> GetMapIQueryable()
        {
            return from map in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnterTopView>()
                   where map.Bussiness == (int)Business.Trading && map.Type == (int)MapsType.Invoice
                   select map;
        }
    }

    /// <summary>
    /// 当前登录人管理的发票
    /// </summary>
    /// <typeparam name="TReponsitory"></typeparam>

    public class MyTradingInvoicesTopView<TReponsitory> : InvoicesTopView<TReponsitory>
       where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        IErpAdmin admin;
        public MyTradingInvoicesTopView(IErpAdmin admin)
        {
            this.admin = admin;
        }
        public MyTradingInvoicesTopView(TReponsitory reponsitory, IErpAdmin admin) : base(reponsitory)
        {
            this.admin = admin;
        }
        override protected IQueryable<Layers.Data.Sqls.PvbCrm.MapsBEnterTopView> GetMapIQueryable()
        {
            return from map in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnterTopView>()
                   where map.Bussiness == (int)Business.Trading && map.Type == (int)MapsType.Invoice && map.CtreatorID == this.admin.ID
                   select map;
        }

    }
}
