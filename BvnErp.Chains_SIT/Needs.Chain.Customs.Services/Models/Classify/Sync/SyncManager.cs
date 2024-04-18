using Needs.Ccs.Services.ApiSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 数据同步
    /// </summary>
    public class SyncManager
    {
        static readonly object locker = new object();
        static SyncManager current;

        PvDataApiSetting apisetting;
        PvWsOrderApiSetting wsorderApisetting;

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

        SyncManager()
        {
            this.apisetting = new PvDataApiSetting();
            this.wsorderApisetting = new PvWsOrderApiSetting();
        }

        public SyncHandlerBase Classify
        {
            get
            {
                return new Classify_SyncHandler(this.apisetting);
            }
        }

        public SyncHandlerBase TaxChange
        {
            get
            {
                return new TaxChange_SyncHandler(this.apisetting);
            }
        }

        public SyncHandlerBase ClassifyCompensation
        {
            get
            {
                return new Classify_Compensation(this.apisetting);
            }
        }

        public SyncHandlerBase CccControl
        {
            get
            {
                return new CccControl_SyncHandler(this.wsorderApisetting);
            }
        }
    }
}
