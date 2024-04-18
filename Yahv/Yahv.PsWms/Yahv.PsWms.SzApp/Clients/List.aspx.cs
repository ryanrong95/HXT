using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Linq.Extends;
using Yahv.PsWms.SzMvc.Services.Models.Origin;
using Yahv.PsWms.SzMvc.Services.Views.Roll;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PsWms.SzApp.Clients
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <returns></returns>
        protected object data()
        {
            Expression<Func<ClientShow, bool>> expression = Predicate();
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            var query = new Clients_Show_View().GetPageList(page, rows, expression);
            
            return new
            {
                rows = query.OrderByDescending(t => t.CreateDate).Select(t => new
                {
                    t.ID,
                    t.Name,
                    t.Username,
                    Password = "******",
                    CreateDate = t.CreateDate.ToString("yyyy-MM-dd"),
                    LoginDate = t.LoginDate == null ? "" : t.LoginDate?.ToString("yyyy-MM-dd"),
                }),
                total = query.Total,
            }.Json();
        }

        Expression<Func<ClientShow, bool>> Predicate()
        {
            Expression<Func<ClientShow, bool>> predicate = item => true;
            //查询参数
            var Name = Request.QueryString["Name"];
            var StartDate = Request.QueryString["StartDate"];
            var EndDate = Request.QueryString["EndDate"];

            if (!string.IsNullOrWhiteSpace(Name))
            {
                predicate = predicate.And(item => item.Name.Contains(Name.Trim()));
            }
            if (!string.IsNullOrWhiteSpace(StartDate))
            {
                DateTime start = Convert.ToDateTime(StartDate.Trim());
                predicate = predicate.And(item => item.CreateDate >= start);
            }
            if (!string.IsNullOrWhiteSpace(EndDate))
            {
                DateTime end = Convert.ToDateTime(EndDate.Trim()).AddDays(1);
                predicate = predicate.And(item => item.CreateDate < end);
            }
            return predicate;
        }
    }
}