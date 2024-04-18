using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Models;

namespace Yahv.Services.Views
{
    public class DriversTopView<TReponsitory> : UniqueView<Driver, TReponsitory>
            where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public DriversTopView()
        {

        }
        public DriversTopView(TReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Driver> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.DriversTopView>()
                   select new Driver
                   {
                       ID = entity.ID,
                       EnterpriseID = entity.EnterpriseID,
                       Name = entity.Name,
                       IDCard = entity.IDCard,
                       Mobile = entity.Mobile,//大陆手机号
                       Status = (Yahv.Underly.GeneralStatus)entity.Status,
                       Mobile2 = entity.Mobile2,//其他手机号，香港或其他
                       CustomsCode = entity.CustomsCode,
                       CardCode = entity.CardCode,
                       PortCode = entity.PortCode,
                       LBPassword = entity.LBPassword
                   };

        }
    }

    /// <summary>
    /// 默认司机视图
    /// </summary>
    public class DriversTopView : DriversTopView<Layers.Data.Sqls.PvbCrmReponsitory>
    {

    }

}
