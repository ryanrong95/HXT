//using Newtonsoft.Json.Linq;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Yahv.Services.Enums;

//namespace Wms.Services.Models
//{
//    public class DataProcessing
//    {

//        public DataProcessing(string aa,string bb)
//        {

//        }
//        public ProcessingType Type { get; set; }
//        JObject json = null;
//        public DataProcessing(string context)
//        {
//            json = JObject.Parse(context);
//            this.Type = json["type"].Value<ProcessingType>();
//        }

//        public void Enter()
//        {
          
//            switch (this.Type)
//            {
//                //入库操作的Enter
//                case ProcessingType.Sorting:
//                    var sortingWaybill = json["Main"]?.Value<SortingWaybill>();
//                    if (sortingWaybill != null)
//                    {
//                        new WayBillServices().Submit(sortingWaybill);
//                    }
                  
//                    break;
//                //出库操作的Enter
//                case ProcessingType.Picking:
//                    var pickingWaybill = json["Main"]?.Value<PickingWaybill>();
//                    if (pickingWaybill != null)
//                    {
//                        pickingWaybill.Submit();
//                    }
//                    break;
//                //租赁操作的Enter
//                case ProcessingType.LsNotice:
//                    var lsNotices = json["Enter"]?.Values<Yahv.Services.Models.LsNotice>().ToArray();
//                    if (lsNotices != null)
//                    {
//                        new LsNoticeServices().Submit(lsNotices);
//                    }
//                    break;
//                default:
//                    throw new NotSupportedException("尚未实现");
//            }
//        }

//        public void Delete()
//        {
//            switch (this.Type)
//            {
//                case ProcessingType.Sorting:
//                    throw new NotSupportedException("尚未实现");
//                case ProcessingType.Picking:
//                    //delete一般为逻辑删除，具体情况具体分析
//                    var noticeIDs1 = json["Delete"].Values<string>();
//                    if (noticeIDs1 != null)
//                    {
//                        throw new NotSupportedException("尚未实现");
//                    }

//                    //删除未出库的出库通知（真删）
//                    var noticeIDs2 = json["Deleting"].Values<string>();
//                    if (noticeIDs2 != null)
//                    {
//                        foreach (var item in noticeIDs1)
//                        {
//                            new WayBillServices().DeleteOutStock(item);
//                        }
//                    }
//                    break;
//                case ProcessingType.LsNotice:
//                    throw new NotSupportedException("尚未实现");
//                default:
//                    throw new NotSupportedException("尚未实现");
//            }
//        }

//    }

//    public class InputData
//    {
//        public string InputID { get; set; }
//        public string ItemID { get; set; }
//        public string TinyOrderID { get; set; }
//    }
//}
