using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using Needs.Wl.Admin.Plat;
using System;
using System.Linq;

namespace WebApp.ExitOrder.Exit
{
    /// <summary>
    /// 已出库报关订单
    /// 深圳库房
    /// </summary>
    public partial class Exited : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        protected void LoadData()
        {
            this.Model.ExitType = EnumUtils.ToDictionary<Needs.Ccs.Services.Models.WaybillType>()
                .Select(item => new { item.Key, item.Value }).Json();
        }

        /// <summary>
        /// 已出库通知列表
        /// </summary>
        protected void data()
        {
            //查询条件
            string OrderID = Request.QueryString["OrderID"];
            string CustomerCode = Request.QueryString["EntryNumber"];
            string ExitType = Request.QueryString["ExitType"];

            var exitNotices = AdminPlat.Current.Warehouse.MyCenterSZExitNotice
                             .Where(x =>x.CenterExeStatus == CgPickingExcuteStatus.Completed).AsQueryable();
            if (!string.IsNullOrEmpty(OrderID))
            {
                exitNotices = exitNotices.Where(x => x.Order.ID.Contains(OrderID));
            }
            if (!string.IsNullOrEmpty(ExitType))
            {
                exitNotices = exitNotices.Where(x => x.CenterExitType == (WaybillType)int.Parse(ExitType));
            }
            if (!string.IsNullOrEmpty(CustomerCode))
            {
                exitNotices = exitNotices.Where(x => x.Order.Client.ClientCode.Contains(CustomerCode));
            }

            exitNotices = exitNotices.OrderByDescending(x => x.CreateDate);

            // 返回数据列表
            Func<SZExitNotice, object> convert = item => new
            {
                ID = item.ID,
                OrderID = item.Order.ID,//订单编号
                ClientCode = item.Order.Client.ClientCode,
                ClientName = item.Order.Client.Company.Name,//客户
                PackNo = item.CenterPackNo,
                AdminName = item.Admin.RealName,//制单人
                ExitType = item.CenterExitType.GetDescription(),
                CreateDate = item.CreateDate,
                NoticeStatus = item.CenterExeStatus.GetDescription(),
            };
            this.Paging(exitNotices, convert);
        }
    }
}