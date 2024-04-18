using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Hanlders
{
    /// <summary>
    /// 产品深圳入库
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">状态改变事件参数</param>
    public delegate void WarehouseEntriedEventHanlder(object sender, WarehouseEntriedEventArgs e);

    /// <summary>
    /// 事件参数
    /// </summary>
    public class WarehouseEntriedEventArgs : EventArgs
    {
        public Models.EntryNotice EntryNotice { get; private set; }

        public WarehouseEntriedEventArgs(Models.EntryNotice item)
        {
            this.EntryNotice = item;
        }

        public WarehouseEntriedEventArgs() { }
    }

    //#region 深圳库房上架后事件

    //public delegate void SZWarehouseOnStockedEventHanlder(object sender, SZWarehouseOnStockedEventArgs e);

    //public class SZWarehouseOnStockedEventArgs : EventArgs
    //{
    //    public string EntryNoticeID { get; private set; }

    //    public Layer.Data.Sqls.ScCustomsReponsitory Reponsitory { get; private set; }

    //    public SZWarehouseOnStockedEventArgs()
    //    {

    //    }

    //    public SZWarehouseOnStockedEventArgs(Layer.Data.Sqls.ScCustomsReponsitory reponsitory, string entryNoticeID)
    //    {
    //        this.Reponsitory = reponsitory;
    //        this.EntryNoticeID = entryNoticeID;
    //    }
    //}

    //#endregion
}
