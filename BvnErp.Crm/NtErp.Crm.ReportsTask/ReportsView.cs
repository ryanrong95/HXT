using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NtErp.Crm.ReportsTask
{
    public class ReportsView : UniqueView<Report, BvCrmReponsitory>
    {
        #region 构造函数

        public ReportsView()
        {

        }

        internal ReportsView(BvCrmReponsitory reponsitory) : base(reponsitory)
        {

        }

        #endregion

        protected override IQueryable<Report> GetIQueryable()
        {
            return from report in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Reports>()
                   where report.Status == (int)Status.Normal
                   select new Report
                   {
                       ID = report.ID,
                       AdminID = report.AdminID,
                       UpdateDate = report.UpdateDate,
                       Context = report.Context,
                       ClientID = report.ClientID
                   };
        }

        /// <summary>
        /// 按照每月有效拜访数进行统计：
        /// 1.电话方式除外
        /// 2.拜访报告内容中有效汉字不少于30个
        /// </summary>
        public void StatisticByMonth()
        {
            //删除之前的统计数据
            this.Reponsitory.Command("DELETE FROM [bv3crm].[dbo].[ClientVisits];");

            //排除电话拜访
            string communication = "{\"Type\":" + $"{ActionMethord.communication.GetHashCode()}";
            var iquery = from report in this.IQueryable
                         where report.ClientID != null && !report.Context.StartsWith(communication)
                         select report;

            //查询报告创建人
            var adminIDs = iquery.Select(item => item.AdminID).Distinct().ToArray();
            Console.WriteLine($"总共需要处理[{adminIDs.Length}]个报告创建人的数据 >>> \r\n");

            //自增主键
            int pk = 0;
            //数据处理进度
            int processed = 0;

            //依次统计每个报告创建人的数据
            foreach (var adminID in adminIDs)
            {
                try
                {
                    processed++;
                    Console.WriteLine($"[{adminID}]的报告数据_开始处理 ...");

                    //当前创建人的报告
                    var ienum_iquery = (from report in iquery
                                        where report.AdminID == adminID
                                        select new Report
                                        {
                                            UpdateDate = report.UpdateDate,
                                            Context = report.Context
                                        }).ToArray();

                    //统计有效汉字不少于30个的报告
                    var validReports = new List<Report>();
                    foreach (var report in ienum_iquery)
                    {
                        var content = report.ReportContext.Content;
                        if (string.IsNullOrEmpty(content))
                        {
                            continue;
                        }

                        int count = GetChineseCount(content);
                        if (count >= 30)
                        {
                            validReports.Add(report);
                        }
                    }

                    //按月统计客户拜访数
                    var clientVisits = validReports.GroupBy(item => item.DateIndex)
                        .Select(item => new Layer.Data.Sqls.BvCrm.ClientVisits()
                        {
                            ID = $"{++pk}",
                            AdminID = adminID,
                            DateIndex = item.Key,
                            Count = item.Count(),
                            CreateDate = DateTime.Now
                        }).ToArray();

                    //数据插入
                    this.Reponsitory.Insert(clientVisits);

                    Console.WriteLine($"[{adminID}]的报告数据_处理成功 ...");
                    Console.WriteLine($"当前处理进度[{processed}/{adminIDs.Length}] ... \r\n");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[{adminID}]的报告数据_处理异常 ...");
                    Console.WriteLine($"异常信息{ex.Message}");
                    Console.WriteLine($"堆栈信息{ex.StackTrace}");
                    Console.WriteLine($"当前处理进度[{processed}/{adminID.Length}] ... \r\n");
                }
            }

            Console.WriteLine($">>> 数据处理完成 \r\n");
        }

        /// <summary>
        /// 统计字符串中汉字的数量
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        int GetChineseCount(string context)
        {
            int count = 0;
            Regex regex = new Regex(@"^[\u4E00-\u9FA5]{0,}$");

            for (int i = 0; i < context.Length; i++)
            {
                if (regex.IsMatch(context[i].ToString()))
                {
                    count++;
                }
            }

            return count;
        }
    }
}
