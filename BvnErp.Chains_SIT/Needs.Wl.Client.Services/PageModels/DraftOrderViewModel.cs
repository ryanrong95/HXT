using System;
using System.Collections.Generic;

namespace Needs.Wl.Client.Services.PageModels
{
    public class DraftOrderViewModel : Needs.Wl.Models.Order
    {
        /// <summary>
        /// 供应商
        /// </summary>
        public Needs.Wl.Models.ClientSupplier Supplier { get; set; }
    }
}