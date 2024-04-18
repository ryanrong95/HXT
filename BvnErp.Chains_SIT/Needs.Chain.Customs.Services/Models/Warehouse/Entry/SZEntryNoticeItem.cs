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
    /// 深圳入库通知Item
    /// </summary>
    public class SZEntryNoticeItem : EntryNoticeItem
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
        /// <summary>
        /// 添加时间：20190524
        /// </summary>
        public  EntryNotice EntryNotice { get; set; }
        public SZEntryNoticeItem() : base()
        {

        }

        public override void Enter()
        {
            base.Enter();
        }

        public override void Abandon()
        {
            base.Abandon();
        }
    }

    public class SZEntryNoticeItems : BaseItems<SZEntryNoticeItem>
    {
        internal SZEntryNoticeItems(IEnumerable<SZEntryNoticeItem> enums) : base(enums)
        {
        }

        internal SZEntryNoticeItems(IEnumerable<SZEntryNoticeItem> enums, Action<SZEntryNoticeItem> action) : base(enums, action)
        {
        }

        public override void Add(SZEntryNoticeItem item)
        {
            base.Add(item);
        }
    }
}