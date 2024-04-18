using Layer.Data.Sqls;
using Needs.Linq;
using NtErp.Crm.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Models
{
    public class Score : IUnique
    {
        #region 属性
        /// <summary>
        /// 主键
        /// </summary>
        public string ID
        {
            get; set;
        }

        /// <summary>
        /// 考核类型
        /// </summary>
        public ScoreType ScoreType
        {
            get; set;
        }

        /// <summary>
        /// 客户拜访数量
        /// </summary>
        public int ReportScore
        {
            get; set;
        }

        /// <summary>
        /// 新客户数量(Sales)
        /// </summary>
        public int ClientScore
        {
            get; set;
        }

        /// <summary>
        /// 新销售机会数量(PM/FAE)
        /// </summary>
        public int ProjectScore
        {
            get; set;
        }

        /// <summary>
        /// DI数量
        /// </summary>
        public int DIScore
        {
            get; set;
        }

        /// <summary>
        /// DW数量
        /// </summary>
        public int DWScore
        {
            get; set;
        }

        /// <summary>
        /// 管理员
        /// </summary>
        public AdminTop Admin
        {
            get; set;
        }

        /// <summary>
        /// 年份
        /// </summary>
        public string Year
        {
            get; set;
        }

        /// <summary>
        /// 月份
        /// </summary>
        public string Month
        {
            get; set;
        }

        /// <summary>
        /// 绩效分
        /// </summary>
        public decimal TotalScore
        {
            get; set;
        }

        /// <summary>
        /// 绩效工资
        /// </summary>
        public decimal? Bonus
        {
            get; set;
        }

        /// <summary>
        /// 所属地区
        /// </summary>
        public string[] DistrictNames
        {
            get;set;
        }
        #endregion
    }
}
