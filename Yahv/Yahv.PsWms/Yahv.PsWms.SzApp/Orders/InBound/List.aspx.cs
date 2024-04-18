using System;
using System.Linq;
using System.Linq.Expressions;
using Yahv.Linq.Extends;
using Yahv.PsWms.SzMvc.Services.Enums;
using Yahv.PsWms.SzMvc.Services.Views.Roll;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PsWms.SzApp.Orders.InBound
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.LoadComboboxData();
            }
        }

        private void LoadComboboxData()
        {
            //币种数据
            this.Model.statusData = ExtendsEnum.ToArray(new OrderStatus[] { OrderStatus.Closed })
                .Select(item => new { Value = (int)item, Text = item.GetDescription() });
        }
        
        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <returns></returns>
        protected object data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);
            
            #region 查询参数
            
            var ID = Request.QueryString["ID"];
            var Partnumber = Request.QueryString["Partnumber"];
            var ClientName = Request.QueryString["ClientName"];
            var Status = Request.QueryString["Status"];
            var StartDate = Request.QueryString["StartDate"];
            var EndDate = Request.QueryString["EndDate"];

            var query = new OrdersIn_Show_View();
            if (!string.IsNullOrWhiteSpace(ID))
            {
                query = query.SearchByID(ID);
            }
            if (!string.IsNullOrWhiteSpace(ClientName))
            {
                query = query.SearchByClientName(ClientName);
            }
            if (!string.IsNullOrWhiteSpace(Status))
            {
                var status = (OrderStatus)Enum.Parse(typeof(OrderStatus), Status);
                query = query.SearchByStatus(status);
            }
            if (!(string.IsNullOrWhiteSpace(StartDate) && string.IsNullOrWhiteSpace(EndDate)))
            {
                DateTime? start = null;
                DateTime? end = null;
                if (!string.IsNullOrWhiteSpace(StartDate))
                {
                    start = Convert.ToDateTime(StartDate.Trim());
                }
                if (!string.IsNullOrWhiteSpace(EndDate))
                {
                    end = Convert.ToDateTime(EndDate.Trim());
                }
                query = query.SearchByDateTime(start, end);
            }
            if (!string.IsNullOrWhiteSpace(Partnumber))
            {
                query = query.SearchByPartNumber(Partnumber);
            }

            #endregion

            var linq = query.ToMyPage(page, rows);
            return new
            {
                rows = linq.Item1.Select(t => new
                {
                    t.ID,
                    t.ClientName,
                    t.TransportModeDec,
                    t.OrderStatusDec,
                    CreateDate = t.CreateDate.ToString("yyyy-MM-dd"),
                }),
                total = linq.Item2,
            }.Json();
        }
    }
}