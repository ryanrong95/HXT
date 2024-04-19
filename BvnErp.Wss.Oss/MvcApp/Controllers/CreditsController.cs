using NtErp.Wss.Oss.Services.Extends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcApp.Controllers
{

    enum MyEnum
    {
        RepayError = 400,
        RepaySuccess = 200
    }

    public class CreditsController : UserController
    {

        /// <summary>
        /// 账期列表
        /// </summary>
        /// <returns></returns>
        public ActionResult DateIndexes()
        {
            Needs.Underly.Currency currency;
            Enum.TryParse(Request["Currency"], out currency);

            NtErp.Wss.Oss.Services.Models.ClientTop client;
            using (var view = new NtErp.Wss.Oss.Services.Views.ClientsTopView())
            {
                client = view[this.Client.ID];
            };

            return Json(Paging(client.GetDebts(currency)), JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 信用还款统计
        /// </summary>
        /// <param name="dateindex"></param>
        /// <param name="currency"></param>
        /// <returns></returns>
        public ActionResult DebtStat(string dateindex,string currency)
        {
            Needs.Underly.Currency cy;
            Enum.TryParse(currency, out cy);
            int dateIndex = int.Parse(dateindex);

            NtErp.Wss.Oss.Services.Models.ClientTop client;
            using (var view = new NtErp.Wss.Oss.Services.Views.ClientsTopView())
            {
                client = view[this.Client.ID];
                return Json(client.GetDebtStat(cy,dateIndex), JsonRequestBehavior.AllowGet);
            }

           
        }


        /// <summary>
        /// 还款详情
        /// </summary>
        /// <returns></returns>
        public ActionResult Repaids(string Currency,string DateIndex)
        {
            Needs.Underly.Currency currency;
            Enum.TryParse(Currency, out currency);
            int dateindex = int.Parse(DateIndex);

            NtErp.Wss.Oss.Services.Models.ClientTop client;
            using (var view = new NtErp.Wss.Oss.Services.Views.ClientsTopView())
            {
                client = view[this.Client.ID];
            };

            return Json(Paging(client.GetRepaids(currency, dateindex)), JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 信息账期详情列表
        /// </summary>
        /// <param name="dateIndex"></param>
        /// <returns></returns>
        public ActionResult Index(int dateIndex)
        {
            Needs.Underly.Currency currency;
            Enum.TryParse(Request["Currency"], out currency);

            NtErp.Wss.Oss.Services.Models.ClientTop client;
            using (var view = new NtErp.Wss.Oss.Services.Views.ClientsTopView())
            {
                client = view[this.Client.ID];
            };
            var credits = client.GetDebts(dateIndex).Where(item => item.Currency == currency);
            if (credits != null && credits.Count() > 0)
            {
                return Json(Paging(credits.First().Items), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// 返回所有期号
        /// </summary>
        /// <returns></returns>
        public ActionResult Indexes()
        {
            NtErp.Wss.Oss.Services.Models.ClientTop client;
            using (var view = new NtErp.Wss.Oss.Services.Views.ClientsTopView())
            {
                client = view[this.Client.ID];
            };

            return Json(client.GetDebtIndexes(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 信用还款
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Repays(int dateIndex,string currency,decimal price)
        {
            NtErp.Wss.Oss.Services.Models.ClientTop client;
            using (var view = new NtErp.Wss.Oss.Services.Views.ClientsTopView())
            {
                client = view[this.Client.ID];
            }

            client.NotDebt += Client_NotDebt;
            client.NotEnough += Client_NotEnough;
            client.RepayError += Client_RepayError;
            client.RepaySuccess += Client_RepaySuccess;

            Needs.Underly.Currency outer;
            Enum.TryParse(currency, out outer);


            client.Repay(outer, dateIndex, price);

            return this.CurrentResult as ActionResult ?? Json(new
            {
                status = (int)MyEnum.RepaySuccess,
            }) as ActionResult;
        }

        private void Client_RepaySuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            eJson(new
            {
                status = 200,
            });
        }

        private void Client_RepayError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            eJson(new
            {
                status = 400,
                message = "RepayError",
            });
        }

        private void Client_NotEnough(object sender, Needs.Linq.ErrorEventArgs e)
        {
            eJson(new
            {
                status = 401,
                message = "NotEnough",
            });
        }

        private void Client_NotDebt(object sender, Needs.Linq.ErrorEventArgs e)
        {
            eJson(new
            {
                status = 402,
                message = "NotDebt",
            });
        }
    }
}