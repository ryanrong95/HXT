using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models.BalanceQueue.CoreStep
{
    public class NodeParam
    {
        public NodeParam()
        {
            SearchPairParam = new SearchPairParam();
            PairPopParam = new PairPopParam();
            PopOrRepushParam = new PopOrRepushParam();
        }

        public Layer.Data.Sqls.ScCustomsReponsitory Reponsitory { get; set; }

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

        public SearchPairParam SearchPairParam { get; set; }

        public PairPopParam PairPopParam { get; set; }

        public PopOrRepushParam PopOrRepushParam { get; set; }
    }

    public class SearchPairParam
    {
        public string InfoID { get; set; } = string.Empty;

        public string BusinessID { get; set; } = string.Empty;
    }

    public class PairPopParam
    {
        public string PairCode { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public Models.BalanceQueue.BalanceQueueInfo MinProcessIDModelInCircle { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Models.BalanceQueue.BalanceQueueInfo PairModelInCircleAndQueue { get; set; }
    }

    public class PopOrRepushParam
    {
        /// <summary>
        /// 
        /// </summary>
        public Models.BalanceQueue.BalanceQueueInfo MinProcessIDModelInCircle { get; set; }
    }
}
