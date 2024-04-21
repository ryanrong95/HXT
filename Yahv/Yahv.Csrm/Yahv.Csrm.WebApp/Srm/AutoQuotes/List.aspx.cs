using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Linq.Extends;
using YaHv.Csrm.Services.Extends;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApp.Srm.AutoQuotes
{
    public partial class List : BasePage
    {
        protected string SupplierID
        {
            get
            {
                return Request.QueryString["id"];
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model = new EnterprisesRoll()[SupplierID];
            }
        }
        protected object data()
        {
            var query = new SuppliersRoll()[SupplierID].AutoQuotes.Where(Predicate());
            return new
            {
                rows = query.OrderBy(item => item.CreateDate).ToArray().Select(item => new
                {
                    item.ID,
                    item.Name,
                    item.Supplier,
                    item.Packaging,
                    item.Manufacturer,
                    item.DateCode,
                    item.PackageCase,
                    item.Prices,
                    item.UnitPrice,
                    item.Quantity,
                    //Admin = item.Admin.RealName,
                    item.Deadline,
                    item.CreateDate
                })
            };
        }
        Expression<Func<AutoQuote, bool>> Predicate()
        {
            Expression<Func<AutoQuote, bool>> predicate = item => true;
            //string StartDate = Request.QueryString["StartDate"];
            //string EndDate = Request.QueryString["EndDate"];
            var partnumber = Request["partnumber"];
            var manufacturer = Request["manufacturer"];
            var datecode = Request["DateCode"];
            var currency = Request["currency"];

            //DateTime start;
            //DateTime end;
            //if (!string.IsNullOrWhiteSpace(StartDate) && DateTime.TryParse(StartDate, out start))//开始日期
            //{
            //    predicate = predicate.And(item => item.CreateDate >= start);
            //}
            //if (!string.IsNullOrWhiteSpace(EndDate) && DateTime.TryParse(EndDate, out end))//结束日期
            //{
            //    predicate = predicate.And(item => item.CreateDate < end.AddDays(1));
            //}
            if (!string.IsNullOrWhiteSpace(partnumber))
            {
                predicate = predicate.And(item => item.Name.Contains(partnumber));
            }
            if (!string.IsNullOrWhiteSpace(manufacturer))
            {
                predicate = predicate.And(item => item.Manufacturer.Contains(manufacturer));
            }
            if (!string.IsNullOrWhiteSpace(datecode))
            {
                predicate = predicate.And(item => item.DateCode .Contains(datecode));
            }
            return predicate;
        }
        protected void del()
        {
            var ids = Request["items"].Split(',');
            var entity = new SuppliersRoll()[Request["supplierid"]].AutoQuotes.Where(item => ids.Contains(item.ID));
            foreach (var it in entity)
            {
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                           nameof(Yahv.Systematic.Srm),
                                          "删除供应商库存", "删除供应商库存：" + it.ID, "");
            }
            entity.Delete();
        }
    }
}