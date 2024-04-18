using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models.BalanceQueueRedis
{
    public class RedisKey
    {
        private string _businessType = string.Empty;

        public RedisKey(string businessType)
        {
            this._businessType = businessType;
        }

        /// <summary>
        /// SyncDBBalanceInfoList
        /// </summary>
        public string SyncDBBalanceInfoList
        {
            get { return this._businessType + "_SyncDBBalanceInfoList"; }
        }

        /// <summary>
        /// SyncDBBalanceRecordList
        /// </summary>
        public string SyncDBBalanceRecordList
        {
            get { return this._businessType + "_SyncDBBalanceRecordList"; }
        }

        /// <summary>
        /// CircleList
        /// </summary>
        public string CircleList
        {
            get { return this._businessType + "_CircleList"; }
        }

        /// <summary>
        /// QueueList
        /// </summary>
        public string QueueList
        {
            get { return this._businessType + "_QueueList"; }
        }

        /// <summary>
        /// XmlSet
        /// </summary>
        public string XmlSet
        {
            get { return this._businessType + "_XmlSet"; }
        }

        /// <summary>
        /// FailBoxSet
        /// </summary>
        public string FailBoxSet
        {
            get { return this._businessType + "_FailBoxSet"; }
        }

        /// <summary>
        /// AlonePairSet
        /// </summary>
        public string AlonePairSet
        {
            get { return this._businessType + "_AlonePairSet"; }
        }

        /// <summary>
        /// PreSendEmailList
        /// </summary>
        public string PreSendEmailList
        {
            //get { return this._businessType + "_PreSendEmailList"; }
            get { return "_PreSendEmailList"; }
        }

        /// <summary>
        /// PreRestartCustomsList
        /// </summary>
        public string PreRestartCustomsList
        {
            //get { return this._businessType + "_PreRestartCustomsList"; }
            get { return "_PreRestartCustomsList"; }
        }

        /// <summary>
        /// RemindHimList
        /// </summary>
        public string RemindHimList
        {
            get { return this._businessType + "_RemindHimList"; }
        }

        /// <summary>
        /// RemindDBSet
        /// </summary>
        public string RemindDBSet
        {
            get { return this._businessType + "_RemindDBSet"; }
        }
    }
}
