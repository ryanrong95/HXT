using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.CustomsTool.WinForm.Models.Messages
{
    /// <summary>
    /// 业务回执原始数据
    /// </summary>
    public class DecReceiptOrigin
    {
        public string INTERFACE_VERSION { get; set; }

        public DEC_RESULT DecResult { get; set; }

        /// <summary>
        /// 处理结果文字信息
        /// </summary>
        public string RESULT_INFO { get; set; }
    }

    public class DEC_RESULT
    {
        /// <summary>
        /// 数据中心统一编号
        /// </summary>
        public string SEQ_NO { get; set; }

        /// <summary>
        /// 正式统一编号
        /// </summary>
        public string CUS_CIQ_NO { get; set; }


        public string CUSTOM_MASTER { get; set; }

        /// <summary>
        /// 海关编号
        /// </summary>
        public string ENTRY_ID { get; set; }

        /// <summary>
        /// 通知时间
        /// </summary>
        public string NOTICE_DATE { get; set; }

        /// <summary>
        /// 处理结果
        /// </summary>
        public string CHANNEL { get; set; }

        /// <summary>
        /// 审核备注
        /// </summary>
        public string NOTE { get; set; }

        /// <summary>
        /// 申报口岸
        /// </summary>
        public string DECL_PORT { get; set; }

        /// <summary>
        /// 申报单位
        /// </summary>
        public string AGENT_NAME { get; set; }

        /// <summary>
        /// 报关单员代码
        /// </summary>
        public string DECLARE_NO { get; set; }

        /// <summary>
        /// 境内收发货人代码
        /// </summary>
        public string TRADE_CO { get; set; }

        /// <summary>
        /// 货场代码
        /// </summary>
        public string CUSTOMS_FIELD { get; set; }

        /// <summary>
        /// 保税仓库代码
        /// </summary>
        public string BONDED_NO { get; set; }

        /// <summary>
        /// 进出口日期
        /// </summary>
        public string I_E_DATE { get; set; }

        /// <summary>
        /// 件数
        /// </summary>
        public string PACK_NO { get; set; }

        /// <summary>
        /// 提单号
        /// </summary>
        public string BILL_NO { get; set; }

        /// <summary>
        /// 运输方式
        /// </summary>
        public string TRAF_MODE { get; set; }

        /// <summary>
        /// 航班号
        /// </summary>
        public string VOYAGE_NO { get; set; }

        /// <summary>
        /// 净重
        /// </summary>
        public string NET_WT { get; set; }

        /// <summary>
        /// 毛重
        /// </summary>
        public string GROSS_WT { get; set; }

        /// <summary>
        /// 申报日期
        /// </summary>
        public string D_DATE { get; set; }

    }
}
