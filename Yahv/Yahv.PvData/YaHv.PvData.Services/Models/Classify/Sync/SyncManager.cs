using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YaHv.PvData.Services.Models
{
    /// <summary>
    /// 数据同步
    /// </summary>
    public class SyncManager
    {
        static readonly object locker = new object();
        static SyncManager current;

        public static SyncManager Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        current = new SyncManager();
                    }
                }

                return current;
            }
        }

        /// <summary>
        /// 归类同步
        /// </summary>
        public SyncHandlerBase Classify
        {
            get
            {
                return new ClassifySyncHandler();
            }
        }

        /// <summary>
        /// 税费变更同步
        /// </summary>
        public SyncHandlerBase TaxChange
        {
            get
            {
                return new TaxChangeSyncHandler();
            }
        }

        [Obsolete]
        public SyncHandler this[OrderedProduct op]
        {
            get
            {
                return new SyncHandler(op);
            }
        }
    }
}
