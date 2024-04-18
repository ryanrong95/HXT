using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Linq.Extends;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.PvWsOrder.Services.Models;
using Yahv.PvWsOrder.Services.Views;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PvWsOrder.WebApp.Orders.Clients
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadComboBoxData();
            }
        }

        protected void LoadComboBoxData()
        {

        }

        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <returns></returns>
        protected object data()
        {
            //查询参数
            var CompanyName = Request.QueryString["CompanyName"];
            var ClientCode = Request.QueryString["ClientCode"];
            var StartDate = Request.QueryString["StartDate"];
            var EndDate = Request.QueryString["EndDate"];
            #region 查询表达式
            Expression<Func<WsClient, bool>> predicate = null;
            if (!string.IsNullOrWhiteSpace(CompanyName))
            {
                CompanyName = CompanyName.Trim();
                predicate = predicate.And(item => item.Name.Contains(CompanyName));
            }
            if (!string.IsNullOrWhiteSpace(ClientCode))
            {
                ClientCode = ClientCode.Trim();
                predicate = predicate.And(item => item.EnterCode.Contains(ClientCode));
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
            #endregion
            var query = Erp.Current.WsOrder.MyWsClients.Where(item => true);
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            return this.Paging(query.AsEnumerable().OrderByDescending(item => item.CreateDate), t => new
            {
                ID = t.ID,
                CompanyName = t.Name,
                CompanyCode = t.Uscc,
                Grade = t.Grade.GetDescription(),
                EnterCode = t.EnterCode,
                ContactName = t.Contact?.Name,
                ContactTel = string.IsNullOrEmpty(t.Contact?.Tel) ? t.Contact?.Mobile : t.Contact?.Tel,
                CreateDate = t.CreateDate.ToString("yyyy-MM-dd"),
                Status = t.Status.GetDescription(),
            });
        }
    }
}