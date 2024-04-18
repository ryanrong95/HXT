using Layer.Data.Sqls.ScCustoms;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 深圳出库通知Items
    /// </summary>
    public class SZExitNoticeItem : ExitNoticeItem
    {
        //public override SZSorting Sorting
        //{
        //    get
        //    {
        //        return base.Sorting;
        //    }

        //    set
        //    {
        //        base.Sorting = value;
        //    }
        //}

        /// <summary>
        /// 深圳库存信息
        /// </summary>
        public StoreStorage StoreStorage { get; set; }
    }

    public class SZExitNoticeItems : BaseItems<SZExitNoticeItem>
    {
        internal SZExitNoticeItems(IEnumerable<SZExitNoticeItem> enums) : base(enums)
        {
        }

        internal SZExitNoticeItems(IEnumerable<SZExitNoticeItem> enums, Action<SZExitNoticeItem> action) : base(enums, action)
        {
        }

        public override void Add(SZExitNoticeItem item)
        {
            base.Add(item);
        }
    }
}