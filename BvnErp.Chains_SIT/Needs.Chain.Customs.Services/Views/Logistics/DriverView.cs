using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 司机信息视图
    /// </summary>
    public class DriverView : UniqueView<Models.Driver, ScCustomsReponsitory>
    {
        public DriverView()
        {
        }

        internal DriverView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Driver> GetIQueryable()
        {
            var carrierView= new CarriersView(this.Reponsitory);
            return from driver in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Drivers>()
                   join carrier in carrierView on driver.CarrierID equals carrier.ID
                   //where driver.Status == (int)Enums.Status.Normal
                   select new Models.Driver
                   {
                       ID = driver.ID,
                       Carrier = carrier,
                       Name = driver.Name,
                       Code = driver.Code,
                       Mobile = driver.Mobile,
                       License = driver.License,
                       CreateDate = driver.CreateDate,
                       Status = (Enums.Status)driver.Status,
                       UpdateDate = driver.UpdateDate,
                       HSCode=driver.HSCode,
                       DriverCardNo=driver.DriverCardNo,
                       HKMobile=driver.HKMobile,
                       PortElecNo=driver.PortElecNo,
                       LaoPaoCode=driver.LaoPaoCode,
                       IsChcd=driver.IsChcd
                   };
        }
    }
}
