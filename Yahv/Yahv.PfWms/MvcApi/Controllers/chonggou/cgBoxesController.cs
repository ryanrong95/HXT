using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Wms.Services.chonggous;
using Wms.Services.Enums;
using Wms.Services.Models;
using Yahv.Services.Enums;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Mvc;
using Yahv.Web.Mvc.Filters;

namespace MvcApi.Controllers
{
    public class cgBoxesController : ClientController
    {
        static cgBoxesController()
        {
            BoxManage.HkDeclare.BoxSelected += Current_BoxSelected;
            BoxManage.HkDeclare.NotSupported += HkDeclare_NotSupported;
            BoxManage.Current.SZBoxSelected += Current_BoxSelected;

            CgBoxManage.Current.BoxSelected += Current_BoxSelected1;
        }

        private static void Current_BoxSelected1(object sender, EventArgs e)
        {
            Current.JsonResult(new JMessage
            {
                success = false,
                code = 400,
                data = "箱号已经被选择！"
            });
        }

        private static void HkDeclare_NotSupported(object sender, EventArgs e)
        {
            Current.JsonResult(new JMessage
            {
                success = false,
                code = 400,
                data = "不支持此箱号格式的手动输入！"
            });
        }

        static void Current_BoxSelected(object sender, EventArgs e)
        {
            Current.JsonResult(new JMessage
            {
                success = false,
                code = 400,
                data = "箱号已经被选择！"
            });
        }

        // GET: Boxes
        /// <summary>
        /// 获取箱号标识
        /// </summary>
        /// <param name="enterCode">入仓单号,入仓号以wl开头的就代表外单,其它为内单</param>
        /// <param name="whCode">库房标识</param>
        /// <param name="date">箱号日期</param>
        /// <returns></returns>
        [Obsolete("荣检要求：形式废弃")]
        public ActionResult Show1(string enterCode, string orderID, string whCode = null, DateTime? date = null)
        {
            IEnumerable<BoxCoder> data = null;
            if (whCode.StartsWith(nameof(Yahv.Services.WhSettings.HK)))
            {
                data = BoxManage.HkDeclare[enterCode, orderID, whCode, date];
            }
            else if (whCode.StartsWith(nameof(Yahv.Services.WhSettings.SZ)))
            {
                data = BoxManage.Current[enterCode, orderID, whCode, date];
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Show(string enterCode, DateTime? date = null)
        {
            var data = BoxManage.HkDeclare[enterCode, date];
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取箱号包装种类
        /// </summary>
        /// <returns></returns>
        public ActionResult GetPackageType()
        {
            var data = Wms.Services.PackageTypes.Current;
            return Json(data.Select(item => new
            {
                GBCode = item.GBCode,
                ChineseName = item.ChineseName
            }), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 保存已经选择的箱号
        /// </summary>
        /// <param name="enterCode">入仓号</param>
        /// <param name="code">箱号</param>
        /// <param name="tinyOrderID">小订单编号</param>
        /// <param name="orderID">订单ID</param>
        /// <param name="date">时间</param>
        /// <param name="adminID">操作人</param>
        /// <param name="storageID">分拣ID</param>
        /// <returns></returns>
        [HttpPost]
        [Obsolete("荣检要求：形式废弃")]
        public ActionResult Enter1(string enterCode, string code, string tinyOrderID, string orderID, DateTime? date, string adminID)
        {
            //try
            //{
            //BoxManage.HkDeclare.Enter(enterCode, code, tinyOrderID, date, adminID);

            Regex regex = new Regex(@"(WL|WLSZ|NL|NLSZ)\d{9}", RegexOptions.None);
            Regex regex1 = new Regex(@"^(NL|WL|NLSZ|WLSZ)\d{9}\-(NL|WL|NLSZ|WLSZ)\d{9}$", RegexOptions.None);
            if (!regex.IsMatch(code) || regex1.IsMatch(code))
            {
                return Json(null);
            }

            BoxManage.HkDeclare.Enter(enterCode, code.ToUpper(), tinyOrderID, orderID, date, adminID);
            return Json(new
            {
                success = true,
                code = 200,
                data = "操作成功"
            });
            //}
            //catch (Exception ex)
            //{
            //    return Json(new JMessage
            //    {
            //        success = false,
            //        code = 400,
            //        data = ex.Message
            //    });
            //}
        }


        /// <summary>
        /// 序列箱号
        /// </summary>
        /// <param name="enterCode">入仓号</param>
        /// <param name="quantity">箱号个数</param>
        /// <param name="date">日期</param>
        /// <param name="adminID">操作人</param>
        /// <returns></returns>
        [HttpPost]
        [Obsolete("荣检要求：形式废弃")]
        public ActionResult EnterSeries(string enterCode, string orderID, string tinyOrderID, int quantity, DateTime? date, string adminID)
        {
            //try
            //{
            string boxCode = BoxManage.HkDeclare.EnterSeries(enterCode, orderID, tinyOrderID, quantity, date, adminID);
            return Json(new
            {
                success = true,
                code = 200,
                data = boxCode
            });
            //}
            //catch (Exception ex)
            //{
            //    return Json(new
            //    {
            //        success = true,
            //        code = 400,
            //        data = ex.Message
            //    });
            //}
        }

        [HttpPayload]
        [Obsolete("废弃，采用NewEnter")]
        public ActionResult Enter(JPost jpost)
        {
            var args = new
            {
                storageID = jpost["storageID"]?.Value<string>(),
                boxCode = jpost["boxCode"]?.Value<string>(),
                date = jpost["date"]?.Value<DateTime?>(),
                adminID = jpost["adminID"]?.Value<string>(),
                quantity = jpost["quantity"]?.Value<int?>(),
                enterCode = jpost["enterCode"]?.Value<string>(),
                oldBoxCode = jpost["oldBoxCode"]?.Value<string>(),
                newBoxCode = jpost["newBoxCode"]?.Value<string>(),
            };


            if (string.IsNullOrWhiteSpace(args.storageID))
            {      //专用于返回箱号
                string series = null;

                //录入连续
                if (args.quantity.HasValue)
                {
                    series = BoxManage.HkDeclare.EnterSeries(args.enterCode, args.quantity.Value, args.date ?? null, args.adminID);
                }

                // 录入箱号
                if (!string.IsNullOrWhiteSpace(args.boxCode))
                {
                    BoxManage.HkDeclare.Enter(args.enterCode, args.boxCode, args.date ?? null, args.adminID);
                }

                return Json(new
                {
                    success = true,
                    code = 200,
                    data = series
                });
            }
            else
            {
                //调用苏亮的方法
                if (!string.IsNullOrWhiteSpace(args.storageID))
                {
                    using (var view = new Wms.Services.chonggous.Views.CgSortingsView())
                    {
                        view.ModifyBoxCode(args.storageID, args.newBoxCode, args.adminID);
                    }
                }

                BoxManage.HkDeclare.ModifyBoxCode(args.adminID, args.oldBoxCode, args.newBoxCode);

                return Json(new
                {
                    code = 200
                });
            }
        }

        [HttpPayload]
        public ActionResult NewEnter(JPost jpost)
        {
            var args = new
            {
                storageID = jpost["storageID"]?.Value<string>(),
                source = jpost["source"]?.Value<int>(),
                enterCode = jpost["enterCode"]?.Value<string>(),
                code = jpost["code"]?.Value<string>().Trim().ToUpper(),
                date = jpost["date"]?.Value<DateTime?>(),
                adminID = jpost["adminID"]?.Value<string>(),
                oldBoxCode = jpost["oldBoxCode"]?.Value<string>(),
                newBoxCode = jpost["newBoxCode"]?.Value<string>().Trim().ToUpper(),
            };

            if (args.code == "WL"||args.newBoxCode=="WL")
            {
                return Json(new
                {
                    success = false,
                    code = 300,
                    data = "请输入正确的箱号！"
                });
            }

            //没有库存ID是添加箱号
            if (string.IsNullOrWhiteSpace(args.storageID))
            {
                string boxCode = null;
                // 箱号不为空时录入箱号
                if (!string.IsNullOrWhiteSpace(args.code))
                {
                    boxCode = CgBoxManage.Current.Enter(args.enterCode, args.code, args.date, args.adminID);
                }

                return Json(new
                {
                    code = 200,
                    boxCode = boxCode
                });
            }
            //修改历史到货中的箱号
            else
            {
                // 当来源是代报关时调用代报关的修改箱号
                if (args.source == (int)CgNoticeSource.AgentBreakCustoms)
                {
                    //调用苏亮的方法
                    if (!string.IsNullOrWhiteSpace(args.storageID))
                    {
                        using (var view = new Wms.Services.chonggous.Views.CgSortingsView())
                        {
                            view.ModifyBoxCode(args.storageID, args.newBoxCode, args.adminID);
                        }
                    }
                }

                // 当来源是转报关时调用转报关的修改箱号
                if (args.source == (int)CgNoticeSource.AgentCustomsFromStorage)
                {
                    using (var view = new Wms.Services.chonggous.Views.CgCustomsStorageView())
                    {
                        view.ModifyBoxCode(args.storageID, args.newBoxCode, args.adminID);
                    }
                }                

                CgBoxManage.Current.ModifyCode(args.adminID, args.oldBoxCode, args.newBoxCode, args.date);

                return Json(new
                {
                    code = 200
                });
            }
        }

        //[HttpPayload]
        //测试多选箱号变连续箱号
        //public ActionResult  Test(JPost jpost)
        //{
        //    BoxManage.HkDeclare.TestEnter(jpost);
        //    return Json(null);
        //}

        [HttpPayload]
        [Obsolete("废弃，采用NewDelete")]
        public ActionResult Delete(JPost jpost)
        {
            var boxCode = jpost["boxCode"]?.Value<string>();
            // 删除箱号
            if (!string.IsNullOrWhiteSpace(boxCode))
            {
                BoxManage.HkDeclare.Delete(boxCode);
            }
            return Json(new
            {
                code = 200
            });

        }

        [HttpPayload]
        public ActionResult NewDelete(JPost jpost)
        {
            var boxCode = jpost["boxCode"]?.Value<string>();
            //var date = jpost["date"]?.Value<DateTime>();
            // 删除箱号
            if (!string.IsNullOrWhiteSpace(boxCode))
            {
                CgBoxManage.Current.Delete(boxCode);
            }
            return Json(new
            {
                code = 200
            });

        }

        /// <summary>
        /// 修改箱号的包装种类
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        [HttpPayload]
        public ActionResult EnterPackageType(JPost jpost)
        {
            var boxCode = jpost["boxCode"].Value<string>();
            var packageType = jpost["packageType"].Value<string>();
            var adminID = jpost["adminID"].Value<string>();
            BoxManage.HkDeclare.EnterPackageType(boxCode, packageType, adminID);
            return Json(new
            {
                success = true,
                code = 200,
                data = "操作成功"
            });
        }


        /// <summary>
        /// 传递过来的箱号和对应的重量/件数进行处理后返回前台
        /// </summary>
        /// <param name="jpost">参考 箱签打印.json 参数</param>
        /// <returns></returns>
        [HttpPayload]
        public ActionResult GetPrintInfo(JPost jpost)
        {
            JToken datas = jpost["Enter"];
            var source = jpost["Source"].Value<int>();
            return Json(BoxManage.Current.GetPrintInfo(datas, source));
        }

        [HttpPayload]
        public ActionResult GetPrintInfo1(JPost jpost)
        {
            var source = jpost["Source"].Value<int>();
            var waybillID = jpost["waybillID"].Value<string>();
            return Json(BoxManage.Current.GetPrintInfo(source, waybillID));

        }

        //public ActionResult 

        ///// <summary>
        ///// 对使用过的箱号进行临时保存
        ///// </summary>
        ///// <param name="jpost"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public ActionResult Used(JPost jpost)
        //{
        //    // 把从乔霞出提交过来的Json文件内容存储到内存中.

        //    BoxManage.HkDeclare.Enter(new BoxManage.MyTempBox
        //    {
        //        SortingID = jpost["SortingID"].Value<string>(),
        //        BoxCode = jpost["BoxCode"].Value<string>(),
        //        EnterCode = jpost["EnterCode"].Value<string>(),
        //        AdminID = jpost["AdminID"].Value<string>(),
        //        TinyOrderID = jpost["TinyOrderID"].Value<string>()
        //    });

        //    return eJson(new JMessage
        //    {
        //        success = true,
        //        code = 200,
        //        data = "保存成功"
        //    });
        //}
    }
}