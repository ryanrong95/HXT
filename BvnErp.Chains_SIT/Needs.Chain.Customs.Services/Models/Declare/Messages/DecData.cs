using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 报关单回执报文-海关回执格式
    /// </summary>
    public class DecData
    {
        /// <summary>
        /// 版本号
        /// </summary>
        public string INTERFACE_VERSION { get; set; }

        public DEC_RESULT DecResult { get; set; }

        /// <summary>
        /// 处理结果文字信息
        /// </summary>
        public string RESULT_INFO { get; set; }

        /// <summary>
        /// 拓展-回执文件名称
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 拓展-回执文件路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 拓展-回执文件备份路径
        /// </summary>
        public string BackupUrl { get; set; }

        //
        [XmlIgnore]
        public DecHead DecHead { get; private set; }

        public DecData()
        {
            
        }

        public void SetHead()
        {
            this.DecHead = new Views.DecHeadsView().First(t => t.SeqNo == this.DecResult.CUS_CIQ_NO);
        }

        public void SaveAs()
        {
            switch (DecResult.CHANNEL)
            {
                case "0":                   
                case "1":
                case "7":
                    ReceiptNormal();
                    //生成香港仓库的出库通知
                    HKOut();
                    IcgooInsideAutoOut();
                    break;
                case "a":
                case "b":
                case "A":
                case "B":
                case "C":
                case "D":
                case "E":
                case "F":
                case "G":
                case "H":
                case "I":
                    ReceiptNormal();
                    break;
                case "K":
                    ReceiptNormal();
                    break;
                case "L":
                    ReceiptNormal();
                    ReceiptL();
                    break;
                case "M":
                case "N":
                    ReceiptNormal();
                    break;
                case "P":
                    ReceiptNormal();
                    break;
                case "R":
                case "S":
                case "T":
                case "W":
                case "X":
                case "Y":
                case "Z":
                case "f0":
                case "F1":
                case "S0":
                    ReceiptNormal();
                    break;
                case "J":
                    //改状态
                    ReceiptNormal();
                    break;

                default:
                    ReceiptNoChannel();
                    break;
            }
        }

        public void ReceiptNormal()
        {
            this.DecHead.CusDecStatus = this.DecResult.CHANNEL;

            //更新报关单状态
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecHeads>(new { this.DecHead.CusDecStatus, }, item => item.ID == this.DecHead.ID);
            }

            this.DecHead.Trace(this.DecResult.NOTE, DateTime.Parse(this.DecResult.NOTICE_DATE), this.FileName, this.FilePath, this.BackupUrl);
        }

        public void ReceiptNoChannel()
        {
            this.DecHead.Trace(this.DecResult.NOTE, DateTime.Parse(this.DecResult.NOTICE_DATE), this.FileName, this.FilePath, this.BackupUrl);
        }

        public void ReceiptL()
        {
            //更新报关单相关字段
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var ddate = DateTime.Parse(this.DecResult.D_DATE);
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecHeads>(new { EntryId = this.DecResult.ENTRY_ID, IEDate = this.DecResult.I_E_DATE, DDate = ddate }, item => item.ID == this.DecHead.ID);
            }
        }

        public void HKOut()
        {
            this.DecHead.DeclareSucceess();
        }

        public void IcgooInsideAutoOut()
        {
            this.DecHead.SZWarehouseAutoOut();
        }

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
