using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms.Services.Enums;
using Yahv.Underly;

namespace Wms.Services.Models
{
    public class InvoiceNoticeForWin
    {

        /// <summary>
        /// 开票通知ID
        /// </summary>
        public string InvoiceNoticeID { get; set; }

        /// <summary>
        /// 客户编号
        /// </summary>
        public string ClientCode { get; set; }

        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 开票类型
        /// </summary>
        public Enums.InvoiceType InvoiceType { get; set; }

        public string InvoiceTypeDes
        {
            get
            {
                return this.InvoiceType.GetDescription();
            }
        }

        /// <summary>
        /// 申请日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 含税金额
        /// </summary>
        public decimal? Amount { get; set; }

        /// <summary>
        /// 交付方式
        /// </summary>
        public Enums.InvoiceDeliveryType DeliveryType { get; set; }

        public string DeliveryTypeDes
        {
            get
            {
                return this.DeliveryType.GetDescription();
            }
        }
        /// <summary>
        /// 开票状态
        /// </summary>
        public InvoiceNoticeStatus InvoiceNoticeStatus { get; set; }

        public string InvoiceNoticeStatusDes
        {
            get
            {
                return this.InvoiceNoticeStatus.GetDescription();
            }
        }

        /// <summary>
        /// 申请人
        /// </summary>
        public string ApplyName { get; set; }

        public InvoiceNoticeFiles[] InvoiceNoticeFilesViews { get; set; }
    }

    public class InvoiceNoticeFiles
    {
       
        /// <summary>
        /// 文件ID
        /// </summary>
        public string InvoiceNoticeFileID { get; set; }

        /// <summary>
        /// 开票通知ID
        /// </summary>
        public string InvoiceNoticeID { get; set; }

        /// <summary>
        /// AdminID
        /// </summary>
        public string ErmAdminID { get; set; }

        /// <summary>
        /// AdminName
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        public int FileType { get; set; }

        /// <summary>
        /// 文件格式
        /// </summary>
        public string FileFormat { get; set; }

        /// <summary>
        /// url路径
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

    }
}
