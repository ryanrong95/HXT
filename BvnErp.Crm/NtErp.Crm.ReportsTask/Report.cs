using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.ReportsTask
{
    /// <summary>
    /// 客户拜访报告
    /// </summary>
    public class Report : IUnique
    {
        #region 属性
        /// <summary>
        /// 主键
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 报告创建人
        /// </summary>
        public string AdminID { get; set; }

        /// <summary>
        /// 更新日期
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Context { get; set; }

        /// <summary>
        /// 客户
        /// </summary>
        public string ClientID { get; set; }
        #endregion

        #region 扩展属性

        ReportContext reportContext;
        public ReportContext ReportContext
        {
            get
            {
                if (this.reportContext == null && this.Context != null)
                {
                    this.reportContext = Newtonsoft.Json.JsonConvert.DeserializeObject<ReportContext>(this.Context);
                }

                return this.reportContext;
            }
        }

        int dateIndex;
        public int DateIndex
        {
            get
            {
                if (this.dateIndex == 0 && this.reportContext != null)
                {
                    var date = this.reportContext.Date;
                    this.dateIndex = int.Parse($"{date.Year}" + $"{date.Month}".PadLeft(2, '0'));
                }

                return this.dateIndex;
            }
        }
        #endregion
    }

    public class ReportContext
    {
        public ActionMethord Type { get; set; }

        public DateTime Date { get; set; }

        public DateTime NextDate { get; set; }

        public string Content { get; set; }
    }
}
