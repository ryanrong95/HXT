//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Layers.Data.Sqls.PvbCrm;
//using Yahv.Underly;

//namespace Yahv.Services.Views
//{
//    /// <summary>
//    /// 已被 wsnSupplierPayeesTopView代替
//    /// 代仓储业务受益人
//    /// </summary>
//    /// <typeparam name="TReponsitory"></typeparam>
//    [Obsolete]
//    public class WsBeneficiariesTopView<TReponsitory> : BeneficiariesTopView<TReponsitory>
//         where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
//    {
//        public WsBeneficiariesTopView()
//        {

//        }
//        public WsBeneficiariesTopView(TReponsitory reponsitory) : base(reponsitory)
//        {
//        }
//        protected override IQueryable<MapsBEnterTopView> GetMapIQueryable()
//        {
//            return from map in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnterTopView>()
//                   where map.Bussiness == (int)Business.WarehouseServicing && map.Type == (int)MapsType.Beneficiary
//                   select map;
//        }
     
//    }
//}
