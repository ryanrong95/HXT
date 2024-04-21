using System;
using System.Linq;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.Erm.WebApp.Erm.PersonalRates
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 数据查询
        /// </summary>
        /// <returns></returns>
        protected object data()
        {
            var rates = Alls.Current.PersonalRates.Where(item => true);

            return new
            {
                rows = rates.OrderBy(t => t.Levels).ToArray().Select(item => new
                {
                    item.ID,
                    item.Levels,
                    item.BeginAmount,
                    item.EndAmount,
                    Rate = item.Rate * 100,
                    item.Deduction,
                    CreateDate = item.CreateDate.ToString("yyyy-MM-dd"),
                    item.AdminID,
                })
            };
        }
        /// <summary>
        /// 删除
        /// </summary>
        protected void Delete()
        {
            string id = Request.Form["ID"];
            var del = Alls.Current.PersonalRates[id];
            if (del != null)
            {
                del.Abandon();
            }

            Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "个税税率表",
                    $"删除", del.Json());
        }
    }
}