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
    ///  提货水单提货人视图
    /// </summary>
    public class DeliveryPickerView : UniqueView<Models.DeliveryPicker, ScCustomsReponsitory>
    {
        public DeliveryPickerView()
        {
        }

        internal DeliveryPickerView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.DeliveryPicker> GetIQueryable()
        {
            return from deliveryPicker in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DeliveryPickers>()
                   where deliveryPicker.Status == (int)Enums.Status.Normal
                   select new Models.DeliveryPicker
                   {
                       ID = deliveryPicker.ID,
                       DeliveryNoticeID = deliveryPicker.DeliveryNoticeID,
                       Name=deliveryPicker.Name,
                       PhoneNumber=deliveryPicker.PhoneNumber,
                       IDType=(Enums.IDType)deliveryPicker.IDType,
                       IDNumber=deliveryPicker.IDNumber,
                       VehicleNO=deliveryPicker.VehicleNo,
                       Status =(Enums.Status)deliveryPicker.Status,
                       CreateDate=deliveryPicker.CreateDate
                   };

        }
    }

}
