using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using Yahv.Underly.Erps;

namespace Yahv.Services.Views
{
    public class TradingContactsTopView<IReponsitory> : ContactsTopView<IReponsitory>
        where IReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public TradingContactsTopView()
        {

        }
        public TradingContactsTopView(IReponsitory reponsitory) : base(reponsitory)
        {

        }
        override protected IQueryable<Layers.Data.Sqls.PvbCrm.MapsBEnterTopView> GetMapIQueryable()
        {
            return from map in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnterTopView>()
                   where map.Bussiness == (int)Business.Trading && map.Type == (int)MapsType.Contact
                   select map;
        }
       
    }

    /// <summary>
    /// 当前登录人管理的所有联系人
    /// </summary>
    /// <typeparam name="IReponsitory"></typeparam>
    public class MyTradingContactsTopView<IReponsitory> : ContactsTopView<IReponsitory>
       where IReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        IErpAdmin admin;
        public MyTradingContactsTopView(IErpAdmin admin)
        {
            this.admin = admin;
        }
        public MyTradingContactsTopView(IReponsitory reponsitory, IErpAdmin admin) : base(reponsitory)
        {
            this.admin = admin;
        }
        override protected IQueryable<Layers.Data.Sqls.PvbCrm.MapsBEnterTopView> GetMapIQueryable()
        {
            return from map in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnterTopView>()
                   where map.Bussiness == (int)Business.Trading && map.Type == (int)MapsType.Contact && map.CtreatorID == this.admin.ID
                   select map;
        }
    }
}
