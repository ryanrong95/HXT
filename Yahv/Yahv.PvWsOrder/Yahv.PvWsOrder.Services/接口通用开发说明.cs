//using Newtonsoft.Json.Linq;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Yahv.Utils.Serializers;

//namespace Yahv.PvWsOrder.Services
//{

//    class 调用类
//    {
//        public 调用类()
//        {
//            Yahv.Services.Models.PickingWaybill waybill = null;

//            var obj = new
//            {
//                Main = new Yahv.Services.Models.PickingWaybill { },
//                Enter = waybill.Notices,  // 根据Input.ID
//                Delete = "ItemID,InputID,NoticeID".Split(','),// 根据Input.ID //new paras[] { new paras { id=1,name="aa" }, new paras { id = 2, name = "bb" } }, // 根据Input.ID
//                Deleting = "ItemID,InputID,NoticeID".Split(','),// 根据Input.ID
//                Type = "1"   //枚举
//            };

//            //Post("url")
//        }
//    }






//    class 服务端接口通用开发说明
//    {
//        //调用端
//        public int Type { get; set; }

//        JObject json = null;
//        public 服务端接口通用开发说明(string context)
//        {
//            json = JObject.Parse(context);
//            this.Type = json["type"].Value<int>();
//        }

//        public void Enter()
//        {

//            //  处理各个业务



//            //工厂
//            switch (this.Type)
//            {
//                case 1:
//                    {

//                        var Main = json["Main"]?.Value<Yahv.Services.Models.PickingWaybill>();
//                        if (Main != null)
//                        {
//                            new Yahv.Services.Models.Waybills { ID = Main.ID, Summary = Main.Summary }.Enter();
//                        }
//                        var Enters = json["Enter"]?.Values<Yahv.Services.Models.Notice>();
//                        if (Enters != null)
//                        {
//                            foreach (var item in Enters)
//                            {
                                
//                            }
//                        }
                        


//                        //  处理各个业务
//                    }
//                    break;
//                case 2:
//                    {
//                        var Main = json["Main"]?.Value<Yahv.Services.Models.Sorting>();
//                        var Enters = json["Enter"]?.Values<Yahv.Services.Models.Sorting>();


//                        // 后续
//                        // 自动处理出库捡货的一些数据


//                    }
//                    break;
//                default:
//                    break;
//            }

//        }


//        public void Delete(string context)
//        {


//            //工厂
//            switch (this.Type)
//            {
//                case 1:
//                    {
//                        var Deletes = json["Delete"].Values<object>();
//                        var Deleting = json["Deleting"].Values<string>();


//                        //  处理各个业务
//                    }
//                    break;
//                case 2:
//                    {

//                        // 后续
//                        // 自动处理出库捡货的一些数据


//                    }
//                    break;
//                default:
//                    break;
//            }

//        }


//    }

//}

