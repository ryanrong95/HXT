using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Linq.Extends;
using Yahv.Underly;
using YaHv.Csrm.Services;
using YaHv.Csrm.Services.Models.Origins;

namespace Yahv.Csrm.WebApp.Crm.FlowAccounts
{
    public partial class List : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 加载列表
        /// </summary>
        /// <returns></returns>
        //protected object data()
        //{
        //    try
        //    {
        //        Expression<Func<FlowAccount, bool>> predicate = item => item.PayerName == Request.QueryString["com"] && item.Business == Request.QueryString["bus"] && item.Payee == Request.QueryString["cid"];
        //        var subject = Request.QueryString["proj"];
        //        var currency = Request.QueryString["cur"];

        //        SubjectType type = SubjectType.Business;        //默认值

        //        if (!string.IsNullOrWhiteSpace(subject))
        //        {
        //            type = Erp.Current.Crm.FinanceSubjects.GetSubjectType(subject);
        //        }

        //        if (type == SubjectType.Catalog)
        //        {
        //            predicate = predicate.And(item => item.Catalog == subject);
        //        }
        //        else if (type == SubjectType.Subject)
        //        {
        //            predicate = predicate.And(item => item.Subject == subject);
        //        }

        //        if (!string.IsNullOrWhiteSpace(currency))
        //        {
        //            predicate = predicate.And(item => item.Currency == (Currency)(int.Parse(currency)));
        //        }

        //        var query = Erp.Current.Crm.FlowAccounts.Where(predicate).ToList();

        //        var result = from entity in query
        //                     where entity.Type == FlowAccountType.CreditRecharge
        //                     orderby entity.CreateDate descending
        //                     select new
        //                     {
        //                         CreateDate = entity.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
        //                         entity.FormCode,
        //                         entity.Price,
        //                         CurrencyName = entity.Currency.GetDescription(),
        //                         entity.Business,
        //                         Project = entity.Catalog ?? entity.Subject,
        //                         entity.Subject,
        //                         AdminName = entity.Admin.RealName,
        //                         entity.PayeeName,
        //                         entity.PayerName,
        //                         entity.ID,
        //                         entity.Currency,
        //                     };


        //        return new
        //        {
        //            rows = result,
        //            footer = new[]
        //            {
        //            new { AdminName= "总计(人民币)：",CreateDate=result.Where(item=>item.Currency==Currency.CNY).Sum(item=>item.Price)},
        //            new { AdminName= "总计(美元)：",CreateDate=result.Where(item=>item.Currency==Currency.USD).Sum(item=>item.Price)}
        //        }
        //        };
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //}
    }
}