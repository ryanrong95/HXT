using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms.Services.Enums;
using Yahv.Underly;

namespace Wms.Services.Extends
{
    public static class WaybillExtend
    {
        //public static ExcuteStatus? ToOrderExcuteStaus(this SortingExcuteStatus status)
        //{

        //    switch (status)
        //    {
        //        case SortingExcuteStatus.WaitTake:
        //            return ExcuteStatus.香港待提货;
        //        case SortingExcuteStatus.Stocked:
        //            return ExcuteStatus.香港入库完成;
        //        case SortingExcuteStatus.Anomalous:
        //            return ExcuteStatus.香港分拣异常;
        //        //case SortingExcuteStatus.Taking:
        //        //    return ExcuteStatus.;
        //        //case SortingExcuteStatus.Sorting:
        //        //    break;
        //        case SortingExcuteStatus.PartStocked:
        //            return ExcuteStatus.香港部分到货;
        //        //case SortingExcuteStatus.Testing:
        //        //    break;
        //        default:
        //            return null;
        //    }
        //}

        //public static ExcuteStatus? ToOrderExcuteStaus(this CgLoadingExcuteStauts status)
        //{

        //    switch (status)
        //    {
        //        case CgLoadingExcuteStauts.Waiting:
        //            return ExcuteStatus.香港待提货;
        //        case CgLoadingExcuteStauts.Taking:
        //        case CgLoadingExcuteStauts.Completed:
        //        default:
        //            throw new Exception("not supports your type ");
        //    }
        //}

        //public static ExcuteStatus? ToOrderExcuteStaus(this Enum status)
        //{
        //    if (status.GetType() == typeof(CgLoadingExcuteStauts))
        //    {
        //        return ((CgLoadingExcuteStauts)status).ToOrderExcuteStaus();
        //    }
        //    if (status.GetType() == typeof(CgSortingExcuteStatus))
        //    {
        //        return ((CgSortingExcuteStatus)status).ToOrderExcuteStaus();
        //    }

        //    throw new Exception("not supports your type ");
        //}

        public static ExcuteStatus? ToOrderExcuteStaus(this CgSortingExcuteStatus status)
        {

            switch (status)
            {
                case CgSortingExcuteStatus.Completed:
                    return ExcuteStatus.香港入库完成;
                case CgSortingExcuteStatus.Anomalous:
                    return ExcuteStatus.香港分拣异常;
                case CgSortingExcuteStatus.PartStocked:
                    return ExcuteStatus.香港部分到货;
                default:
                    return null;
            }
        }

        //public static ExcuteStatus? ToOrderExcuteStaus(this PickingExcuteStatus status)
        //{

        //    switch (status)
        //    {
        //        case PickingExcuteStatus.Waiting:
        //            return ExcuteStatus.香港待出库;
        //        //case PickingExcuteStatus.Picking:
        //        //    return ExcuteStatus.;
        //        //case PickingExcuteStatus.WaitDelivery:
        //        //    return ExcuteStatus.;
        //        case PickingExcuteStatus.OutStock:
        //            return ExcuteStatus.香港已出库;
        //        case PickingExcuteStatus.Anomalous:
        //            return ExcuteStatus.香港分拣异常;
        //        //case SortingExcuteStatus.Testing:
        //        //    break;
        //        default:
        //            return null;
        //    }
        //}
    }
}
