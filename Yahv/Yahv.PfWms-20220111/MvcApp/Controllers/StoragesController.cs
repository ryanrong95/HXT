using Needs.Linq;
using Needs.Linq.Extends;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Wms.Services.Models;
using Wms.Services.Views;

namespace MvcApp.Controllers
{
    public class StoragesController : Controller
    {
        #region 自定义枚举
        enum Message
        {
            [Description("成功")]
            Success = 0,
            [Description("传入数据有误")]
            DataWrong = 1,
            [Description("编辑数据出错")]
            EditWrong = 2,
            [Description("系统出现异常")]
            CatchException = 3
        }
        #endregion

        #region 私有变量
        Message message;
        #endregion

        #region 公共接口
        /// <summary>
        /// 库存查询
        /// </summary>
        /// <param name="id">库存编号</param>
        /// <param name="warehouseID">库房编号</param>
        /// <param name="sortingID">分拣编号</param>
        /// <param name="productID">产品编号</param>
        /// <param name="partNumber">型号</param>
        /// <param name="catalog">型号</param>
        /// <param name="manufacture">型号</param>
        /// <param name="orderID">订单编号</param>
        /// <param name="beginDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页大小</param>
        /// <returns></returns>
        /// <url>http://dev.pfwms.com/api/storages</url>
        // GET: Inputs
        public ActionResult Index(string id = null, string warehouseID = null, string sortingID = null, string productID = null,string partNumber=null, string catalog=null, string manufacture=null, string orderID = null, string beginDate = null, string endDate = null, int pageIndex = 1, int pageSize = 10)
        {
            try
            {


                Expression<Func<Storages, bool>> exp =item=>true;      

                if (!string.IsNullOrWhiteSpace(id))
                {
                    exp=exp.And(item => item.ID == id);
                }

                if (!string.IsNullOrWhiteSpace(warehouseID))
                {
                    exp = exp.And(item => item.WareHouseID.ToUpper().Contains(warehouseID.ToUpper()));
                }

                if (!string.IsNullOrWhiteSpace(sortingID))
                {
                    exp=exp.And(item => item.SortingID == sortingID);
                }

                if (!string.IsNullOrWhiteSpace(productID))
                {
                    exp=exp.And(item => item.ProductID == productID);
                }

                if (!string.IsNullOrWhiteSpace(orderID))
                {
                    exp=exp.And(item => item.OrderID == orderID);
                }
                if (!string.IsNullOrWhiteSpace(partNumber))
                {
                    exp=exp.And(item => item.Product.PartNumber.Contains(partNumber));
                    
                }

                if (!string.IsNullOrWhiteSpace(catalog))
                {
                    exp = exp.And(item => item.Product.Catalog.Contains(catalog));

                }

                if (!string.IsNullOrWhiteSpace(manufacture))
                {
                    exp = exp.And(item => item.Product.Manufacturer.Contains(manufacture));

                }




                DateTime? startDate, EndDate;
                DateExtend.DateHandle(beginDate, endDate, out startDate, out EndDate);

                if (startDate != null)
                {
                    exp = exp.And(item => item.CreateDate >= startDate);
                }

                if (EndDate != null)
                {
                    exp = exp.And(item => item.CreateDate < EndDate);
                }

                return Json(new { obj = new StoragesView().AsEnumerable().Where(exp.Compile()).Page(pageIndex,pageSize) }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                message = Message.CatchException;
                return Json(new { val = (int)message, msg = message.GetDescription() }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]        
        private ActionResult Index(Storages datas)
        {
            try
            {
                //保证数据正确及完整性
                if (!ModelState.IsValid)
                {
                    foreach (var key in ModelState.Keys)
                    {
                        var modelstate = ModelState[key];
                        if (modelstate.Errors.Any())
                        {
                            return Json(new { val = (int)Message.DataWrong, msg = modelstate.Errors.FirstOrDefault().ErrorMessage });
                        }
                    }
                }

                datas.EnterSuccess += Datas_EnterSuccess;
                datas.EnterError += Datas_EnterError;
                datas.Enter();
                return Json(new { val = (int)message, msg = message.GetDescription() });
            }
            catch
            {
                message = Message.CatchException;
                return Json(new { val = (int)message, msg = message.GetDescription() });
            }
        }

        private void Datas_EnterError(object sender, ErrorEventArgs e)
        {
            message = Message.EditWrong;
        }

        private void Datas_EnterSuccess(object sender, SuccessEventArgs e)
        {
            message = Message.Success;
        }
        #endregion

    }
}