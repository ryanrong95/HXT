using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Yahv.PsWms.DappApi.Controllers
{

    public enum SenderType
    {
        Sz,
        Sh,
        Bj,
        Hk

    }

    /// <summary>
    /// 费用文件
    /// </summary>
    public class FixedController : FilesController
    {
        /// <summary>
        /// 深圳库房
        /// </summary>
        /// <returns></returns>
        public ActionResult SzSender()
        {
            return this.Sender(SenderType.Sz);
        }

        public ActionResult Sender(SenderType type)
        {

            SenderModel model;

            switch (type)
            {
                case SenderType.Sz:
                    {
                        model = new SenderModel
                        {
                            Company = "深圳市芯达通供应链管理有限公司",
                            Address = "吉华路393号英达丰科技园A栋101",
                            Contact = "张景鸿",
                            Mobile = "15234030881",
                            Tel = "15234030881",
                            Province = "广东省",
                            City = "深圳市",
                            Region = "龙岗区",
                        };
                    }
                    break;
                case SenderType.Sh:
                case SenderType.Bj:
                case SenderType.Hk:
                default:
                    throw new NotImplementedException("暂时不支持指定的类型:" + type);
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Sender(string id)
        {
            throw new NotImplementedException("暂时不支持指定的ID调用");
        }
    }
}