using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 提货水单提货人扩展方法
    /// </summary>
    public static class DeliveryPickerExtends
    {
        public static Layer.Data.Sqls.ScCustoms.DeliveryPickers ToLinq(this Models.DeliveryPicker entity)
        {

            return new Layer.Data.Sqls.ScCustoms.DeliveryPickers
            {
                ID = entity.ID,
                DeliveryNoticeID=entity.DeliveryNoticeID,
                Name=entity.Name,
                PhoneNumber=entity.PhoneNumber,
                IDType=(int)entity.IDType,
                IDNumber=entity.IDNumber,
                VehicleNo=entity.VehicleNO,
                Status=(int)entity.Status,
                CreateDate = entity.CreateDate,
            };

        }
    }
}
