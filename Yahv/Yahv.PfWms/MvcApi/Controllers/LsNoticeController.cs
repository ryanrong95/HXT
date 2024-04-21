using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wms.Services.Models;
using System.Linq.Expressions;
using Wms.Services;
using Yahv.Usually;
using Yahv.Underly.Attributes;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Mvc;
using Yahv.Services.Models;
using Wms.Services.chonggous;
using Wms.Services.chonggous.Views;

namespace MvcApp.Controllers
{
    /// <summary>
    /// 租赁通知
    /// </summary>
    public class LsNoticeController : Controller
    {

        #region 已废弃
        //enum Message
        //{
        //    [Description("成功")]
        //    Success = 0,
        //    [Description("失败")]
        //    Failed = 1,
        //    //[Description("合同编号重复，请重新添加")]
        //    //CodeRepeated = 2,
        //    [Description("所选择库位已经被使用，请重新添加")]
        //    ShelveNoUse = 2,
        //    [Description("库位添加重复")]
        //    ShevleRepeated = 3
        //}
        //Message message;

        //// GET: FileInfo
        ///// <summary>
        ///// 获取租赁显示页面
        ///// </summary>
        ///// <param name="key">订单编号/客户编号</param>
        ///// <param name="status">状态</param>
        ///// <param name="pageIndex">当前页码</param>
        ///// <param name="pageSize">每页记录数</param>
        ///// <returns></returns>
        //public ActionResult Index(/*string StartTime = null, string EndTime = null*/string key = null, string status = null, int pageIndex = 1, int pageSize = 10)
        //{
        //    try
        //    {
        //        return Json(new { obj= new Wms.Services.LsNoticeServices().GetLsNotice(key, status, pageIndex, pageSize) }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch
        //    {
        //        return Json(null, JsonRequestBehavior.AllowGet);
        //    }
        //}

        ///// <summary>
        ///// 获取租赁的状态值
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult GetStatus()
        //{
        //    var data = Enum.GetValues(typeof(LsOrderStatus)).Cast<LsOrderStatus>();
        //    return Json(new { obj = data.Select(item => new { Status = ((int)item).ToString(), StatusDes = item.GetDescription() }) }, JsonRequestBehavior.AllowGet);
        //}

        ///// <summary>
        ///// 详情页展示
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult Detail(string warehouseID,string orderID)
        //{
        //    try
        //    {
        //        return Json(new { obj = new Wms.Services.LsNoticeServices().Detail(warehouseID, orderID) }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch 
        //    {

        //        return Json(null, JsonRequestBehavior.AllowGet);
        //    }

        //}

        ////[HttpPost]
        ////public ActionResult Index(Contract datas)
        ////{
        ////    try
        ////    {
        ////        //保证数据的一致性
        ////        if (datas.OwnerID == null)
        ////        {
        ////            message = Message.OwnerIsNull;
        ////            return Json(new { obj = (int)message, msg = message.GetDescription() });
        ////        }
        ////        datas.EnterSuccess += Datas_EnterSuccess;
        ////        datas.EnterError += Datas_EnterError;
        ////        datas.Enter();
        ////        return Json(new { obj = (int)message, msg = message.GetDescription() });
        ////    }
        ////    catch
        ////    {
        ////        message = Message.Fail;
        ////        return Json(new { obj = (int)message, msg = message.GetDescription() }, JsonRequestBehavior.AllowGet);
        ////    }
        ////}

        ////private void Datas_EnterError(object sender, ErrorEventArgs e)
        ////{
        ////    message = Message.Fail;
        ////}

        ////private void Datas_EnterSuccess(object sender, SuccessEventArgs e)
        ////{
        ////    message = Message.Success;
        ////}

        //[HttpPost]
        //public ActionResult Enter(string orderID,LsNotice[] lsnotices)
        //{
        //    try
        //    {

        //        //((string[])obj)[0].JsonTo<Contract>():postman测试用这个，不支持下面的结构
        //        //var lsnotice = obj.ToString().JsonTo<LsNotice>();
        //        var warehouse = Yahv.Erp.Current.WareHourse;
        //        warehouse.LsnoticeSuccess += Warehouse_ContractSuccess;
        //        warehouse.LsnoticeFailed += Warehouse_ContractFailed;
        //        //warehouse.CodeRepeated += Warehouse_CodeRepeated;
        //        warehouse.ShelveNoUse += Warehouse_ShelveNoUse;
        //        warehouse.ShelveRepeated += Warehouse_ShelveRepeated;

        //        warehouse.LsNoticeEnter(orderID,lsnotices);

        //        return Json(new { val = message, msg = message.GetDescription() }); ;
        //        //new Wms.Services.ContractServices().ContractEnter(contract/*obj.ToString().JsonTo<Contract>()*/);
        //    }
        //    catch
        //    {
        //        return Json(new { val = Message.Failed, msg = Message.Failed.GetDescription() });
        //    }

        //}
        //private void Warehouse_ShelveRepeated(object sender, ErrorEventArgs e)
        //{
        //    message = Message.ShevleRepeated;
        //}

        //private void Warehouse_ShelveNoUse(object sender, ErrorEventArgs e)
        //{
        //    message = Message.ShelveNoUse;
        //}

        ////private void Warehouse_CodeRepeated(object sender, ErrorEventArgs e)
        ////{
        ////    message = Message.CodeRepeated;
        ////}

        //private void Warehouse_ContractFailed(object sender, ErrorEventArgs e)
        //{
        //    message = Message.Failed;
        //}

        //private void Warehouse_ContractSuccess(object sender, SuccessEventArgs e)
        //{
        //    message = Message.Success;
        //}
        #endregion

        /// <summary>
        /// 租赁通知接口
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Submit(JPost jpost)
        {
            var manage = new LsNoticeManage();
            //var service = new LsNoticeServices();
            try
            {
                var lsNotics = jpost.ToObject<LsNoticeSubmit>();
                //var warehouse = Yahv.Erp.Current.WareHourse;
                //warehouse.LsNoticeSubmit(lsNotics.List);
                manage.Submit(lsNotics.List);

                var result = Json(new
                {
                    Success = true,
                    Data = "调用成功"
                });
                LitTools.Current["调用租赁通知结果"].Log(result.Data.ToString());
                return result;
            }
            catch (Exception ex)
            {
                var result = Json(new
                {
                    Success = false,
                    Data = ex.Message
                });
                LitTools.Current["调用租赁通知结果"].Log(result.Data.ToString());
                return result;
            }
        }


    }
}