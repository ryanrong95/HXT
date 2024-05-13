using Layers.Data.Sqls.HvRFQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
//using WebApp.Services;
using Wms.Services.Enums;
using Wms.Services.Models;
using Yahv;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Underly.Attributes;

namespace MvcApp.Controllers
{
    public class WarehouseController : Controller
    {

      
        enum Message
        {
            [Description("成功")]
            Success = 0,
            [Description("失败")]
            Fail = 1,
            [Description("库房名称不能为空")]
            NameIsNull = 2,
            [Description("库房名字不能重复")]
            NameRepeated = 3,
            [Description("类型有误，请重新选择物理库房或逻辑库房")]
            TypeError = 4,
        }

        Message message=Message.Success;
        // GET: Warehouse
        /// <summary>
        /// 显示库房信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name">库房名称</param>
        /// <param name="ownerID">所有人</param>
        /// <param name="managerID">负责人</param>
        /// <param name="address">库房地址</param>
        /// <returns></returns>
        //DateTime? starttime = null, DateTime? endtime = null,
        public ActionResult Index(string id = null, string name = null, string ownerID = null, string managerID = null, string address = null)
        {
            Expression<Func<Warehouse, bool>> exp = item => item.Type == ShelvesType.Warehouse;

            if (!string.IsNullOrWhiteSpace(name))
            {
                exp = PredicateExtends.And(exp, item => item.Name == name);
            }
            if (!string.IsNullOrWhiteSpace(id))
            {
                exp = PredicateExtends.And(exp, item => item.ID == id);
            }
            if (!string.IsNullOrWhiteSpace(address))
            {
                exp = PredicateExtends.And(exp, item => item.Address.Contains(address));
            }

            var returnData = new Wms.Services.Views.WarehousesView().Where(exp)/*.OrderBy(item=>item.CreateDate)*/;

            if (returnData.ToArray() != null)
            {
                return Json(new { obj = returnData.Paging(1, 1000000) }, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        [HttpPost]
        public ActionResult Index(WareHouses datas)
        {
            try
            {

                //if (!Enum.IsDefined(typeof(Wms.Services.Enums.WarehouseType), datas.Type))
                //{
                //    return Json(new { val = (int)Message.TypeError, msg = Message.TypeError.GetDescription() });
                //}

                //库房名称
                var name = datas.Name ?? "";

                if (string.IsNullOrWhiteSpace(name))
                {
                    return Json(new { val = (int)Message.NameIsNull, msg = Message.NameIsNull.GetDescription() });
                }

                //datas.FatherID = datas.FatherID == null ? null : datas.FatherID;
                //datas.FatherID = null ?? "";


                //datas.FatherID = datas.FatherID ?? "";
                //datas.Summary = datas.Summary ?? "";

                //datas.WarehouseSuccess += Datas_WarehouseSuccess;
                //datas.CheckNameRepeat += Datas_NameRepeat;
                //datas.WarehouseFailed += Datas_WarehouseFailed;
                //datas.Enter();
                return Json(new { val = (int)message, msg = message.GetDescription() });
            }
            catch
            {
                return Json(new { val = (int)Message.Fail, msg = Message.Fail.GetDescription() });

            }

        }

        //private void Datas_WarehouseFailed(object sender, Yahv.Linq.ErrorEventArgs e)
        //{
        //    message = Message.Fail;
        //}

        //private void Datas_NameRepeat(object sender, Yahv.Linq.ErrorEventArgs e)
        //{
        //    message = Message.NameRepeated;
        //}

        //private void Datas_WarehouseSuccess(object sender, Yahv.Linq.SuccessEventArgs e)
        //{
        //    message = Message.Success;
        //}




    }
}