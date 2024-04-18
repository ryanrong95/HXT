using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Declaration.Notice
{
    public partial class PalletsList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var MarkList = getWarehouse();
            this.Model.Warehouse = MarkList.Select(item => new { Value = item.Key, Text = item.Value }).OrderBy(item => item.Value).Json();
        }

       

        /// <summary>
        /// 初始化订单数据
        /// </summary>
        protected void data()
        {
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];
            string Warehouse = Request.QueryString["Warehouse"];

            var list = new Needs.Ccs.Services.Views.PalletsOrigin().Where(item => item.Status == Needs.Ccs.Services.Enums.Status.Normal);


            if (!string.IsNullOrEmpty(StartDate))
            {
                StartDate = StartDate.Trim();
                var from = DateTime.Parse(StartDate);
                list = list.Where(t => t.NoticeTime >= from);
            }

            if (!string.IsNullOrEmpty(EndDate))
            {
                EndDate = EndDate.Trim();
                var to = DateTime.Parse(EndDate).AddDays(1);
                list = list.Where(t => t.NoticeTime < to);
            }

            
            if (!string.IsNullOrEmpty(Warehouse))
            {
                list = list.Where(x => x.Stock == Warehouse.Trim());

            }

            list = list.OrderByDescending(t => t.CreateDate);

            Func<Needs.Ccs.Services.Models.PalletNumber, object> convert = carrier => new
            {
                ID = carrier.ID,
                Stock = carrier.Stock,
                Pallet = carrier.Pallet,
                NoticeTime = carrier.NoticeTime.ToShortDateString(),
                CreateDate = carrier.CreateDate.ToString("yyyy-MM-dd HH:mm"),
                Summary = carrier.Summary,                             
            };
            this.Paging(list, convert);
        }

        private Dictionary<string, string> getWarehouse()
        {
            Dictionary<string, string> mark = new Dictionary<string, string>();
            mark.Add("yisheng", "怡生");
            mark.Add("zhongmei", "中美");
            //mark.Add("zhicheng", "志成");

            return mark;
        }
    }
}