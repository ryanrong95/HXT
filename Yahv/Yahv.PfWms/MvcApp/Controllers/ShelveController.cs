using Needs.Linq;
using Needs.Utils.Descriptions;
using Needs.Utils.EventExtend;
using Needs.Utils.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Wms.Services.Models;

namespace MvcApp.Controllers
{
    public class ShelveController : PageController
    {

        enum Message
        {
            [Description("成功")]
            Success = 0,
            [Description("失败")]
            Fail = 1,
            [Description("名称不能为空")]
            NameIsNull = 2,
            [Description("名称不能重复")]
            NameRepeated = 3,
            [Description("名称不支持修改")]
            IDNotSupported = 4,
            [Description("父ID不能为空")]
            FatherIDIsNull = 5,
            [Description("所要修改的数据不存在")]
            DataIsNull = 6
        }

        Message message;

        public class ShelveMessage
        {

            public string FatherID { get; set; }

            private string managerID = null;
            public string ManagerID
            {
                get
                {
                    return this.managerID;
                }
                set
                {
                    this.managerID = value;
                }
            }

            private int pageindex = 1;

            public int PageIndex
            {
                get
                {
                    return this.pageindex;
                }
                set
                {
                    this.pageindex = value;
                }
            }
            private int pagesize = 10;

            public int PageSize
            {
                get
                {
                    return this.pagesize;
                }
                set
                {
                    this.pagesize = value;
                }
            }
        }


        /// <summary>
        /// 根据父亲ID分别获得库区、货架、库位的数据
        /// </summary>
        /// <param name="fatherID">父亲ID</param>
        /// <param name="managerID">负责人，在库人员</param>
        /// <param name="pageindex">当前页码</param>
        /// <param name="pagesize">每页记录数</param>
        /// <returns></returns>
        public ActionResult Index(ShelveMessage datas)
        {
            Expression<Func<Shelves, bool>> exp = item => item.Status != Wms.Services.Enums.ShelvesStatus.Deleted;

            if (string.IsNullOrWhiteSpace(datas.FatherID))
            {
                return Json(new { val = (int)Message.FatherIDIsNull, msg = Message.FatherIDIsNull.GetDescription() }, JsonRequestBehavior.AllowGet);
            }
            exp = PredicateBuilder.And(exp, item => item.FatherID == datas.FatherID.ToUpper());

            if (!string.IsNullOrWhiteSpace(datas.ManagerID))
            {
                exp = PredicateBuilder.And(exp, item => item.ManagerID == datas.ManagerID);
            }

            var returnData = new Wms.Services.Views.ShelvesView().Where(exp);

            if (returnData.ToArray() != null)
            {
                return Json(new { obj = new ResponsePageList<Shelves>(returnData, datas.PageIndex, datas.PageSize) }, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        [HttpPost]
        public ActionResult Index(Shelves datas)
        {
            try
            {
                var name = datas.Name ?? "";
                if (string.IsNullOrWhiteSpace(name))
                {
                    return Json(new { val = (int)Message.NameIsNull, msg = Message.NameIsNull.GetDescription() });
                }
                datas.AddEvent("ShelvesSuccess", new SuccessHandler(Datas_ShelvesSuccess))
                    .AddEvent("ShelvesFailed", new ErrorHandler(Datas_ShelvesFailed))
                    .AddEvent("CheckNameRepeated", new ErrorHandler(Datas_CheckNameRepeated))
                    .AddEvent("IDNotSupportModify", new ErrorHandler(Datas_IDNotSupportModify))
                    .AddEvent("NotSupportedUpdate", new ErrorHandler(Datas_NotSupportedUpdate))
                    .Enter();

                return Json(new { val = (int)message, msg = message.GetDescription() });

            }
            catch
            {
                return Json(new { val = (int)Message.Fail, msg = Message.Fail.GetDescription() });
            }
        }

        private void Datas_NotSupportedUpdate(object sender, ErrorEventArgs e)
        {
            message = Message.DataIsNull;
        }

        private void Datas_IDNotSupportModify(object sender, ErrorEventArgs e)
        {
            message = Message.IDNotSupported;
        }

        private void Datas_CheckNameRepeated(object sender, ErrorEventArgs e)
        {
            message = Message.NameRepeated;
        }

        private void Datas_ShelvesFailed(object sender, ErrorEventArgs e)
        {
            message = Message.Fail;
        }

        private void Datas_ShelvesSuccess(object sender, SuccessEventArgs e)
        {
            message = Message.Success;
        }
    }
}