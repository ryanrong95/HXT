using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Views.Rolls;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Web.Erp;

namespace Yahv.Finance.WebApp.Log.Operation
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.Modules = Enum.GetValues(typeof(LogModular)).Cast<LogModular>().Select(item => new { text = item.ToString(), value = item.ToString() });
            }
        }

        #region 功能函数
        protected object data()
        {
            using (var logsView = new LogsOperationRoll())
            {
                var query = logsView.Where(GetExpression()).ToArray();
                return this.Paging(query.OrderByDescending(item => item.CreateDate), item => new
                {
                    item.ID,
                    CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    item.CreatorName,
                    item.Modular,
                    item.Operation,
                    Remark = item?.Remark.Length > 80 ? item?.Remark.Substring(0, 80) + "..." : item?.Remark,
                    item.Type,
                });
            }
        }
        #endregion

        #region 私有函数
        private Expression<Func<Logs_Operation, bool>> GetExpression()
        {
            Expression<Func<Logs_Operation, bool>> predicate = item => true;

            string module = Request.QueryString["s_module"];
            string operation = Request.QueryString["s_operation"];
            string startTime = Request.QueryString["s_opstarttime"];
            string endTime = Request.QueryString["s_opendtime"];

            //模块
            if (!string.IsNullOrWhiteSpace(module))
            {
                predicate = predicate.And(item => item.Modular == module);
            }
            //操作
            if (!string.IsNullOrWhiteSpace(operation))
            {
                predicate = predicate.And(item => item.Operation.Contains(operation));
            }
            //开始时间
            if (!string.IsNullOrWhiteSpace(startTime))
            {
                predicate = predicate.And(item => item.CreateDate >= DateTime.Parse(startTime));
            }
            //结束时间
            if (!string.IsNullOrWhiteSpace(endTime))
            {
                DateTime dtEnd = DateTime.Parse(DateTime.Parse(endTime).AddDays(1).ToString("yyyy-MM-dd"));

                predicate = predicate.And(item => item.CreateDate < dtEnd);
            }

            return predicate;
        }
        #endregion
    }
}