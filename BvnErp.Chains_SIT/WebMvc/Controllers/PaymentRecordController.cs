using Needs.Linq;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using Needs.Wl.Web.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebMvc.Models;

namespace WebMvc.Controllers
{
    /// <summary>
    /// 我的付款记录
    /// 付款的账单明细
    /// </summary>
    [UserHandleError(ExceptionType = typeof(Exception))]
    public class PaymentRecordController : UserController
    {
        /// <summary>
        /// 我的付款记录
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public async Task<ActionResult> MyPayment()
        {
            var myPaidTotalAmount = await Needs.Wl.User.Plat.UserPlat.Current.MyPaymentRecords.PaidTotalAmount();//已付款总金额
            var myRecordedTotalAmount = await Needs.Wl.User.Plat.UserPlat.Current.MyPaymentRecords.RecordedTotalAmount();//已经入账总金额

            PaymentViewModel model = new PaymentViewModel
            {
                ClearAmount = myRecordedTotalAmount.ToString("0.00"),
                TotalAmount = myPaidTotalAmount.ToString("0.00"),
                UnClearAmount = (myPaidTotalAmount - myRecordedTotalAmount).ToString("0.00")
            };

            return View(model);
        }

        /// <summary>
        /// 获取我的付款记录
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public async Task<JsonResult> GetMyPaymentList()
        {
            int page = int.Parse(Request.Form["page"]);
            int rows = int.Parse(Request.Form["rows"]);

            var dateNav = Request.Form["dateNav"];  //日期
            var startDate = Request.Form["startDate"];  //日期选择
            var endDate = Request.Form["endDate"];  //日期选择

            var view = Needs.Wl.User.Plat.UserPlat.Current.MyPaymentRecords;
            view.PageIndex = page;
            view.PageSize = rows;
            var predicate = PredicateBuilder.Create<Needs.Wl.Client.Services.Models.ClientPaymentRecord>();

            if ((!string.IsNullOrWhiteSpace(dateNav)) && dateNav != "All")
            {
                if (dateNav == "Month") //当月订单
                {
                    predicate = predicate.And(item => item.ReceiptDate >= DateTime.Now.AddDays(-30));
                }
                else if (dateNav == "Week")  //最近一周
                {
                    predicate = predicate.And(item => item.ReceiptDate >= DateTime.Now.AddDays(-7));
                }
            }

            if ((!string.IsNullOrWhiteSpace(startDate)) && (!string.IsNullOrWhiteSpace(endDate)))
            {
                predicate = predicate.And(item => item.ReceiptDate >= DateTime.Parse(startDate) && item.ReceiptDate <= DateTime.Parse(endDate));
            }

            view.Predicate = predicate;
            int total = view.RecordCount;

            var viewList = await view.ToListAsync();
            var list = viewList.Select(item => new
            {
                item.ID,
                item.ClientID,
                item.SeqNo,
                item.AccountBankName,
                item.Amount,
                item.ClearAmount,
                ReceiptDate = item.ReceiptDate.ToString("yyyy-MM-dd HH:mm"),
            });

            return JsonResult(VueMsgType.success, "", new { list = list.ToArray(), total }.Json());
        }

        /// <summary>
        /// 我的付款明细
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult Details(string id)
        {
            var receipt = Needs.Wl.User.Plat.UserPlat.Current.MyPaymentRecords[id];
            if (receipt == null)
            {
                return View("Error");
            }

            OrderRecievedViewModel model = new OrderRecievedViewModel
            {
                ID = id,
                SeqNo = receipt.SeqNo,
                AccountName = receipt.AccountName,
                AccountBankName = receipt.AccountBankName,
                AccountBankAccount = receipt.AccountBankAccount,
                Amount = receipt.Amount.ToString("0.00"),
                Currency = receipt.Currency,
                ClearAmount = receipt.ClearAmount.ToString("0.00"),
                CreateTime = receipt.ReceiptDate.ToString("yyyy-MM-dd HH:mm")
            };
            return View(model);
        }

        /// <summary>
        /// 获取我的付款明细
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public async Task<ActionResult> GetDetailsList()
        {
            var id = Request.Form["id"];
            int page = int.Parse(Request.Form["page"]);
            int rows = int.Parse(Request.Form["rows"]);
            var view = Needs.Wl.User.Plat.UserPlat.Current.PaymentRecordContext[id].Receiveds;
            view.PageSize = rows;
            view.PageIndex = page;
            int total = view.RecordCount;

            var viewList = await view.ToListAsync();
            var list = viewList.Select(item => new
            {
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                item.OrderID,
                FeeType = item.FeeType.GetDescription(),
                Amount = 0 - item.Amount,
            });

            return JsonResult(VueMsgType.success, "", new { list = list.ToArray(), total }.Json());
        }
    }
}