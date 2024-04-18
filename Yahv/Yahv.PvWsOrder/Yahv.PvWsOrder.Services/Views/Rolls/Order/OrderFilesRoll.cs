using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.PvWsOrder.Services.Models;

namespace Yahv.PvWsOrder.Services.Views
{
    /// <summary>
    /// 订单的文件视图
    /// </summary>
    public class OrderFilesRoll : UniqueView<Yahv.Services.Models.CenterFileDescription, PvCenterReponsitory>
    {
        string OrderID = string.Empty;

        public OrderFilesRoll(string orderID)
        {
            this.OrderID = orderID;
        }

        protected override IQueryable<Yahv.Services.Models.CenterFileDescription> GetIQueryable()
        {
            var files = new Yahv.Services.Views.CenterFilesTopView()
                .Where(t => t.WsOrderID == OrderID && t.Status == Yahv.Services.Models.FileDescriptionStatus.Normal);
            return files;
        }
    }

    /// <summary>
    /// 收付款申请的文件视图
    /// </summary>
    public class ApplicationFilesRoll : UniqueView<Yahv.Services.Models.CenterFileDescription, PvCenterReponsitory>
    {
        string ApplicationID = string.Empty;

        public ApplicationFilesRoll(string ApplicationID)
        {
            this.ApplicationID = ApplicationID;
        }

        protected override IQueryable<Yahv.Services.Models.CenterFileDescription> GetIQueryable()
        {
            var files = new Yahv.Services.Views.CenterFilesTopView().Where(t => t.ApplicationID == ApplicationID);
            return files;
        }
    }
}
