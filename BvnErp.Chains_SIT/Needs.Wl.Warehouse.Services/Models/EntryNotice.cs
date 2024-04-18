using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Model;
using System;
using System.Linq;

namespace Needs.Wl.Warehouse.Services.Models
{
    /// <summary>
    /// 深圳库房入库通知
    /// </summary>
    public class SZEntryNotice : Needs.Wl.Models.EntryNotice
    {
        public SZEntryNotice()
        {
            this.WarehouseType = Wl.Models.Enums.WarehouseType.ShenZhen;
        }
    }
}