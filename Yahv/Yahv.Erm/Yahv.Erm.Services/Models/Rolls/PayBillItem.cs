using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Erm.Services.Models.Origins;

namespace Yahv.Erm.Services.Models.Rolls
{
    public class PayBillItem
    {
        /// <summary>
        /// 唯一码 ：'员工编号'+ '-' + [DateIndex] 
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 员工ID
        /// </summary>
        public string StaffID { get; set; }

        /// <summary>
        /// 封账日期
        /// </summary>
        public DateTime? ClosedData { get; set; }

        /// <summary>
        /// 日期序数(例：201901 (表月份))
        /// </summary>
        public string DateIndex { get; set; }

        /// <summary>
        /// 所属公司ID
        /// </summary>
        public string EnterpriseID { get; set; }

        /// <summary>
        /// 岗位ID
        /// </summary>
        public string PostionID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreaetDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? UpdateDate { get; set; }

        /// <summary>
        /// 状态(考核,封账,已发放)
        /// </summary>
        public PayBillStatus Status { get; set; }

        /// <summary>
        /// 是否添加
        /// </summary>
        public bool IsInsert { get; set; }

        /// <summary>
        /// 是否修改
        /// </summary>
        public bool IsUpdate { get; set; }

        /// <summary>
        /// 工资数据
        /// </summary>
        public List<PayItem> PayItems { get; set; }
    }
}
