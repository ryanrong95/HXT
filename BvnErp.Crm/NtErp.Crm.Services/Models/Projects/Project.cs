using Needs.Linq;
using NtErp.Crm.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Models.Projects
{
    /// <summary>
    /// 销售机会
    /// </summary>
    public class Project : IUnique, IPersistence
    {

        /// <summary>
        /// 构造函数
        /// </summary>
        public Project()
        {
            this.CreateDate = this.UpdateDate = DateTime.Now;
            this.Status = ActionStatus.Normal;
        }

        #region 属性

        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public ProjectType Type { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 产品全称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 客户ID
        /// </summary>
        public string ClientID { get; set; }
        /// <summary>
        /// 公司
        /// </summary>
        public string CompanyID { get; set; }
        /// <summary>
        /// 货币
        /// </summary>
        public CurrencyType Currency { get; set; }
        /// <summary>
        /// 项目估值
        /// </summary>
        public decimal? Valuation { get; set; }

        /// <summary>
        /// 人员对象
        /// </summary>
        public string AdminID { get; set; }
        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// 原型日期
        /// </summary>
        public DateTime? ModelDate { get; set; }

        /// <summary>
        /// 量产日期
        /// </summary>
        public DateTime? ProductDate { get; set; }

        /// <summary>
        /// 月产量
        /// </summary>
        public int? MonthYield { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public string Contactor { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 联系地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 更新日期
        /// </summary>
        public DateTime UpdateDate { get; set; }
        /// <summary>
        /// 行动状态
        /// </summary>
        public ActionStatus Status { get; set; }
        /// <summary>
        /// 项目摘要描述
        /// </summary>
        public string Summary { get; set; }

        #endregion

        #region 扩展属性

        /// <summary>
        /// 客户
        /// </summary>
        public Client Client { get; set; }

        /// <summary>
        /// 公司
        /// </summary>
        public Company Company { get; set; }

        /// <summary>
        /// 人员对象
        /// </summary>
        public AdminTop Admin { get; set; }

        /// <summary>
        /// 所属行业
        /// </summary>
        public Industry Industry { get; set; }
        /// <summary>
        /// 预计成交总额
        /// </summary>
        public decimal ExpectTotal
        {
            get; set;
        }


        #endregion

        #region 持久化

        public void Abandon()
        {
            throw new NotImplementedException();
        }

        public void Enter()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
