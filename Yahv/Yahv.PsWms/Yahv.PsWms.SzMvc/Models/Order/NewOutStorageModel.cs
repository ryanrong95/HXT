using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Yahv.PsWms.SzMvc.Models
{
    public class NewOutStorageReturnModel
    {
        /// <summary>
        /// 从我的库存中转的订单项
        /// </summary>
        public StorageListModel[] OrderItemsFromMyStorage { get; set; }
    }
}