using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NtErp.Crm.Services.Enums;
using Needs.Utils.Descriptions;
using Needs.Erp.Generic;

namespace NtErp.Crm.Services.Models
{
    
    public class _AdminProject
    {
        /// <summary>
        /// 公司
        /// </summary>
        public Company Company
        {
            get; set;
        }

        /// <summary>
        /// 公司主键ID
        /// </summary>
        public string CompanyID
        {
            get
            {
                return this.Company?.ID;
            }
            set
            {
                this.Company.ID = value;
            }
        }

        /// <summary>
        /// 职位
        /// </summary>
        public JobType JobType
        {
            get; set;
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate
        {
            get; set;
        }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateDate
        {
            get; set;
        }

        /// <summary>
        /// 微信ID
        /// </summary>
        public string WXID
        {
            get;set;
        }

        /// <summary>
        /// 是否授权微信
        /// </summary>
        public bool IsAgree
        {
            get;set;
        }

        /// <summary>
        /// 登录Token
        /// </summary>
        public string Token
        {
            get;set;
        }

        /// <summary>
        /// 考核类型
        /// </summary>
        public ScoreType? ScoreType
        {
            get;set;
        }

        /// <summary>
        /// 绩效基数
        /// </summary>
        public decimal? SalaryBase
        {
            get; set;
        }

        /// <summary>
        /// 大赢家ID
        /// </summary>
        public string DyjID
        {
            get;set;
        }

        /// <summary>
        /// 描述
        /// </summary>
        public string Summary
        {
            get; set;
        }
    }

    [Needs.Underly.FactoryView(typeof(Views.AdminTopView))]
    public class AdminTop : _AdminProject, Needs.Linq.IUnique
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public string ID
        {
            get; set;
        }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName
        {
            get; set;
        }

        /// <summary>
        /// 真实名称
        /// </summary>
        public string RealName
        {
            get; set;
        }
    }
}
