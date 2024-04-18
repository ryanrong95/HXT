using Needs.Utils.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models.BalanceQueueRedis.CoreStep
{
    public class ContextParam
    {
        private RedisKey _redisKey;

        public RedisKey RedisKey
        {
            get
            {
                if (_redisKey == null)
                {
                    _redisKey = new RedisKey(this.BusinessType.ToString());
                }
                return _redisKey;
            }
        }

        public RedisHelper Redis { get; set; }

        /// <summary>
        /// 客户端机器MAC地址
        /// </summary>
        public string MacAddr { get; set; } = string.Empty;

        /// <summary>
        /// 哪个业务中的哪个过程，用一个字符串表示，比如：FailBox、InBox
        /// </summary>
        public string ProcessName { get; set; } = string.Empty;

        /// <summary>
        /// 所要平衡的各种类型(同一个BusinessID的不同类型)，比如：报关单、舱单
        /// </summary>
        public Enums.BalanceQueueBusinessType BusinessType { get; set; }

        /// <summary>
        /// Circle 中的最大个数
        /// </summary>
        public int MaxCountInCircle;

        /// <summary>
        /// 最大等待匹配时间
        /// </summary>
        public TimeSpan MaxWaitPairTimeSpan;

        /// <summary>
        /// 从 Circle 中取出最先 Pop 的数据
        /// </summary>
        public Models.BalanceQueueRedis.BalanceQueueInfo MinProcessIDModelInCircle { get; set; }

        /// <summary>
        /// 从 Circle 或 Queue 中找到的 Pair 的数据
        /// </summary>
        public Models.BalanceQueueRedis.BalanceQueueInfo PairModelInCircleAndQueue { get; set; }

        /// <summary>
        /// 是否成功配对
        /// </summary>
        public bool IsSuccessPaired { get; set; } = false;

        /// <summary>
        /// 是否不需要配对
        /// </summary>
        public bool IsUnNeedPair { get; set; } = false;

        /// <summary>
        /// 匹配后或不需要匹配处理方式
        /// </summary>
        public event PairedOrNotNeedPairHandler PairedOrNotNeedPair;

        /// <summary>
        /// 匹配后或不需要匹配 执行动作
        /// </summary>
        public void OnPairedOrNotNeedPair()
        {
            if (this != null && this.PairedOrNotNeedPair != null)
            {
                this.PairedOrNotNeedPair(this, new PairedOrNotNeedPairEventArgs(this));
            }
        }


        public Models.BalanceQueueRedis.BalanceQueueInfo RemindInfo { get; set; }

        /// <summary>
        /// 提醒通知处理方式
        /// </summary>
        public event RemindNotifyHandler RemindNotify;

        /// <summary>
        /// 提醒通知 执行动作
        /// </summary>
        public void OnRemindNotify()
        {
            if (this != null && this.RemindNotify != null)
            {
                this.RemindNotify(this, new RemindNotifyEventArgs(this.RemindInfo));
            }
        }

        /// <summary>
        /// 检查是否不需要配对
        /// </summary>
        public event CheckIsUnNeedPairHandler CheckIsUnNeedPair;

        /// <summary>
        /// 检查是否不需要配对 执行动作
        /// </summary>
        public bool OnCheckIsUnNeedPair()
        {
            if (this != null && this.CheckIsUnNeedPair != null)
            {
                return this.CheckIsUnNeedPair(this, new CheckIsUnNeedPairEventArgs(this));
            }

            return false;
        }
    }

    public delegate void PairedOrNotNeedPairHandler(object sender, PairedOrNotNeedPairEventArgs e);

    public class PairedOrNotNeedPairEventArgs : EventArgs
    {
        public ContextParam ContextParam;

        public PairedOrNotNeedPairEventArgs(ContextParam contextParam)
        {
            this.ContextParam = contextParam;
        }
    }


    public delegate void RemindNotifyHandler(object sender, RemindNotifyEventArgs e);

    public class RemindNotifyEventArgs : EventArgs
    {
        public BalanceQueueRedis.BalanceQueueInfo BalanceQueueInfo;

        public RemindNotifyEventArgs(BalanceQueueRedis.BalanceQueueInfo balanceQueueInfo)
        {
            this.BalanceQueueInfo = balanceQueueInfo;
        }
    }

    public delegate bool CheckIsUnNeedPairHandler(object sender, CheckIsUnNeedPairEventArgs e);
    
    public class CheckIsUnNeedPairEventArgs : EventArgs
    {
        public ContextParam ContextParam;

        public CheckIsUnNeedPairEventArgs(ContextParam contextParam)
        {
            this.ContextParam = contextParam;
        }
    }
}
