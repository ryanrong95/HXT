using System;
using System.Collections.Generic;

namespace Needs.Wl.Client.Services.PageModels
{
    public class UnReceivedOrder : Needs.Wl.Models.Order
    {
        /// <summary>
        /// 深圳交货方式
        /// </summary>
        public Wl.Models.OrderConsignor Consignor { get; set; }
    }
}