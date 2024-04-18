using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.Views;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.Models
{
    /// <summary>
    /// 归类产品
    /// </summary>
    public partial class OrderItem
    {
        #region 属性

        /// <summary>
        /// 下单日期
        /// </summary>
        public DateTime OrderedDate { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public CgOrderStatus OrderStatus { get; set; }

        /// <summary>
        /// 客户
        /// </summary>
        public string WsClientID { get; set; }
        public WsClient WsClient { get; set; }

        /// <summary>
        /// 税则归类
        /// </summary>
        public ClassifiedPartNumber ClassifiedPartNumber { get; set; }

        /// <summary>
        /// 当前归类是否已经锁定
        /// </summary>
        public bool IsLocked { get; set; }

        /// <summary>
        /// 锁定人
        /// </summary>
        public Admin Locker { get; set; }

        /// <summary>
        /// 锁定时间
        /// </summary>
        public DateTime? LockDate { get; set; }

        #endregion

        /// <summary>
        /// 归类锁定
        /// </summary>
        public JMessage Lock(string creatorId, string step)
        {
            var admin = new AdminsAll()[creatorId];
            var setting = new PvDataApiSetting();
            string apiurl = ConfigurationManager.AppSettings[setting.ApiName] + setting.Lock;

            return Yahv.Utils.Http.ApiHelper.Current.JPost<JMessage>(apiurl, new
            {
                itemId = this.ID,
                creatorId = creatorId,
                creatorName = admin.RealName,
                step = step
            });
        }
    }
}
