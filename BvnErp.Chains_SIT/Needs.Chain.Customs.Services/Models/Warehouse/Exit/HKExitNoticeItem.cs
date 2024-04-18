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
    /// 香港出库通知Items
    /// </summary>
    [Serializable]
    public class HKExitNoticeItem : ExitNoticeItem
    {
        public override DecList DecList
        {
            get
            {
                return base.DecList;
            }
            set
            {
                base.DecList = value;
            }
        }
    }

    [Serializable]
    public class HKExitNoticeItems : BaseItems<HKExitNoticeItem>
    {
        internal HKExitNoticeItems(IEnumerable<HKExitNoticeItem> enums) : base(enums)
        {
        }

        internal HKExitNoticeItems(IEnumerable<HKExitNoticeItem> enums, Action<HKExitNoticeItem> action) : base(enums, action)
        {
        }

        public override void Add(HKExitNoticeItem item)
        {
            base.Add(item);
        }
    }

    [Serializable]
    public class HKExitProduct: HKExitNoticeItem
    {
        public DateTime PackingDate { get; set; }
        /// <summary>
        /// 香港库存信息
        /// </summary>
        public StoreStorage StoreStorage { get; set; }

    }
}