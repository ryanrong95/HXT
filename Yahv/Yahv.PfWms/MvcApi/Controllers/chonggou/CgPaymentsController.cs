using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Wms.Services.Models;
using Wms.Services.Views;
using Yahv.Payments;
using Yahv.Payments.Tools;
using Yahv.Services;
using Yahv.Services.Enums;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Mvc;

namespace MvcApi.Controllers
{
    public class CgPaymentsController : Controller
    {
        /// <summary>
        /// 逾期接口
        /// </summary>
        /// <param name="enterCode">入仓号</param>
        /// <param name="source">业务来源(代报关[30,35,60] 代仓储[other])</param>
        /// <returns></returns>
        public ActionResult Overdue(string enterCode, int source)
        {
            var json = new OverdueResults<OverdueResult>() { code = 200, success = true };

            var sources = new[] {
                CgNoticeSource.AgentBreakCustoms,
                CgNoticeSource.AgentBreakCustomsForIns ,
                CgNoticeSource.AgentCustomsFromStorage
            };

            string conduct = sources.Contains((CgNoticeSource)source) ? ConductConsts.代报关 : ConductConsts.代仓储;

            using (var clientsView = new ClientsView())
            {
                var client = clientsView.SingleOrDefault(item => item.EnterCode == enterCode);

                if (client == null || string.IsNullOrWhiteSpace(client.ID))
                {
                    json.code = 500;
                    json.success = false;

                    return Json(json, JsonRequestBehavior.AllowGet);
                }

                var list = new List<OverdueResult>();
                foreach (var currency in ExtendsEnum.ToArray<Currency>().Where(item => item != Currency.Unknown))
                {
                    list.AddRange(WhSettings.SZ.Doors.Select(item => new OverdueResult()
                    {
                        EnterpriseID = item.Enterprise.ID,
                        EnterpriseName = item.Enterprise.Name,
                        Currency = currency,
                        Price = PaymentManager.Npc[client.ID, item.Enterprise.ID][conduct].DebtTerm[DateTime.Now, currency].Overdue
                    }));

                    list.AddRange(WhSettings.HK.Doors.Select(item => new OverdueResult()
                    {
                        EnterpriseID = item.Enterprise.ID,
                        EnterpriseName = item.Enterprise.Name,
                        Currency = currency,
                        Price = PaymentManager.Npc[client.ID, item.Enterprise.ID][conduct].DebtTerm[DateTime.Now, currency].Overdue
                    }));
                }


                json.isOverdue = list.Sum(item => item.Price) > 0;
                json.data = list;
            }

            return Json(json, JsonRequestBehavior.AllowGet);
        }

        #region 帮助类

        class OverdueResult
        {
            public string EnterpriseID { get; set; }
            public string EnterpriseName { get; set; }
            public Currency Currency { get; set; }
            public decimal Price { get; set; }
        }

        class OverdueResults<T> : JList<T>
        {
            public bool isOverdue { get; set; }
        }

        #endregion
    }
}