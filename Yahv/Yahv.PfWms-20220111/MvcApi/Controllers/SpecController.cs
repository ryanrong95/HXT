using Yahv.Utils.EventExtend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Wms.Services.Models;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Underly.Attributes;
using Yahv.Usually;

namespace MvcApp.Controllers
{
    public class SpecController : Controller
    {
        public SpecController()
        {
            //Yahv.Utils.EventExtend.EventExtend
        }


        enum Message
        {
            [Description("成功")]
            Success = 0,
            [Description("失败")]
            Fail = 1,
            [Description("规格名称不能为空")]
            NameIsNull = 2,
            [Description("规格名称不能重复")]
            NameRepeated = 3,
        }
        Message message;

        /// <summary>
        /// 根据类型/ID分别获得库房、库区、货架、库位的规格的数据
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(/*string id = null*/)
        {
            //Expression<Func<Specs, bool>> exp = item => true;

            //if (!string.IsNullOrWhiteSpace(id))
            //{
            //    exp = PredicateExtends.And(exp, item => item.ID == id);
            //}
            //var returnData = new Wms.Services.Views.SpecsView().Where(exp);

            //if (returnData.ToArray() != null)
            //{
            return Json(new
            {
                obj = new Wms.Services.Views.SpecsView().Select(item => new
                {
                    item.ID,
                    item.Name
                })
            }, JsonRequestBehavior.AllowGet);
            //}

            //return null;
        }

        [HttpPost]
        public ActionResult Index(Specs datas)
        {
            try
            {
                //规格名称
                var name = datas.Name ?? "";
                if (string.IsNullOrWhiteSpace(name))
                {
                    return Json(new { val = (int)Message.NameIsNull, msg = message.GetDescription() });
                }
                datas.AddEvent("SpecsSuccess", new SuccessHanlder(Datas_SpecsSuccess))
                    .AddEvent("SpecsFailed", new SuccessHanlder(Datas_SpecsFailed))
                    .AddEvent("CheckNameRepeat", new SuccessHanlder(Datas_CheckNameRepeat))
                    .Enter();
                return Json(new { val = (int)message, msg = message.GetDescription() });
            }
            catch
            {
                return Json(new { val = (int)Message.Fail, msg = Message.Fail.GetDescription() });
            }
        }

        private void Datas_CheckNameRepeat(object sender, SuccessEventArgs e)
        {
            message = Message.NameRepeated;
        }

        private void Datas_SpecsFailed(object sender, SuccessEventArgs e)
        {
            message = Message.Fail;
        }

        private void Datas_SpecsSuccess(object sender, SuccessEventArgs e)
        {
            message = Message.Success;
        }
    }
}