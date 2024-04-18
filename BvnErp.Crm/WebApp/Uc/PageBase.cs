using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace WebApp.Uc
{
    abstract public class PageBase : Needs.Web.Sso.Forms.ErpPage
    {
        protected PageBase()
        {
            this.Init += PageBase_Init;
        }

        private void PageBase_Init(object sender, EventArgs e)
        {
            //判断  erpplot.current 在 AdminsProject中是否有数据，没有数据就添加
            string id = Needs.Erp.ErpPlot.Current.ID;
            var adminsproject = new NtErp.Crm.Services.Views.AdminTopView()[id];
            if(adminsproject == null)
            {
                var project = new NtErp.Crm.Services.Models.AdminProject();
                project.AdminID = id;
                project.JobType = NtErp.Crm.Services.Enums.JobType.Sales;
                project.Company = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.Company>.Create("001");
                project.Summary = "默认销售";
                project.Enter();
            }

        }

        /// <summary>
        /// 数据转化为datetable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="linq"></param>
        /// <returns></returns>
        protected DataTable ToDataTable<T>(IEnumerable<T> linq)
        {
            var dt = new DataTable();

            var props = typeof(T).GetProperties();

            dt.Columns.AddRange(props.Select(p => new DataColumn(p.Name, p.PropertyType)).ToArray());

            linq.ToList().ForEach(
                i => dt.Rows.Add(props.Select(item => item.GetValue(i)).ToArray())
            );

            return dt;
        }

        protected void Paging<T>(IEnumerable<T> queryable, Func<T, object> converter = null)
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            int total = queryable.Count();
            var query = queryable.Skip(rows * (page - 1)).Take(rows);

            if (converter == null)
            {
                Response.Write(new
                {
                    rows = query.ToArray(),
                    total = total
                }.Json());
            }
            else
            {
                Response.Write(new
                {
                    rows = query.ToArray().Select(converter).ToArray(),
                    total = total
                }.Json());
            }
        }
    }
}