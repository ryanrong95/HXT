using Needs.Utils;
using Needs.Utils.Npoi;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class ClassifiedStatisticExcel
    {

        public List<ClassifiedStatistic> Query(DateTime startDate, DateTime endDate)
        {
            using (var reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //归类明细
                var cdList = GetClassifiedDetails(startDate, endDate);
                //归类统计
                var csList = GetClassifiedStatistics(cdList);
               
                #region 归类统计
                var csData = csList.OrderByDescending(cs => cs.TotalCount)
                    .Select(cs => new ClassifiedStatistic
                    {
                        AdminName = cs.AdminName,
                        Pre1Count = cs.Pre1Count,
                        Pre2Count = cs.Pre2Count,
                        Consult1Count = cs.Consult1Count,
                        Consult2Count = cs.Consult2Count,
                        Classify1Count = cs.Classify1Count,
                        Classify2Count = cs.Classify2Count,
                        //TotalCount = cs.TotalCount,
                        Percent = cs.Percent,
                        Summary = cs.Summary ?? string.Empty
                    }).ToList();

                csData.Add(new ClassifiedStatistic
                {
                    AdminName = "<h3>合计：</h3>",
                    Pre1Count = csData.Sum(t=>t.Pre1Count),
                    Pre2Count = csData.Sum(t => t.Pre2Count),
                    Consult1Count = csData.Sum(t => t.Consult1Count),
                    Consult2Count = csData.Sum(t => t.Consult2Count),
                    Classify1Count = csData.Sum(t => t.Classify1Count),
                    Classify2Count = csData.Sum(t => t.Classify2Count),
                    Percent = "-",
                    Summary = string.Empty
                });
                #endregion

                return csData;
            }
        }


        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">截止时间</param>
        /// <returns></returns>
        public string Export(DateTime startDate, DateTime endDate)
        {
            using (var reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //归类明细
                var cdList = GetClassifiedDetails(startDate, endDate);
                //归类统计
                var csList = GetClassifiedStatistics(cdList);
                //税号修改日志
                var logsView = new Views.Alls.Logs_ClassifyModifiedAll();
                var logs = (from log in logsView
                            where log.Summary.Contains("海关编码") && log.CreateDate >= startDate && log.CreateDate <= endDate
                            select new { log.PartNumber, log.Manufacturer, log.Summary }).ToArray();

                #region 归类统计
                var csData = csList.OrderByDescending(cs => cs.TotalCount)
                    .Select(cs => new
                    {
                        归类人员 = cs.AdminName,
                        产品预归类_预处理一 = cs.Pre1Count,
                        产品预归类_预处理二 = cs.Pre2Count,
                        咨询归类_预处理一 = cs.Consult1Count,
                        咨询归类_预处理二 = cs.Consult2Count,
                        产品归类_预处理一 = cs.Classify1Count,
                        产品归类_预处理二 = cs.Classify2Count,
                        归类条数 = cs.TotalCount,
                        归类占比 = cs.Percent,
                        备注 = cs.Summary ?? string.Empty
                    });
                #endregion

                #region 归类统计合计行
                DataTable sumTable = new DataTable("SumTable");
                for (int index = 1; index <= 10; index++)
                    sumTable.Columns.Add($"column{index}");

                DataRow row = sumTable.NewRow();
                row["column1"] = "合计";
                row["column2"] = csList.Sum(cs => cs.Pre1Count);
                row["column3"] = csList.Sum(cs => cs.Pre2Count);
                row["column4"] = csList.Sum(cs => cs.Consult1Count);
                row["column5"] = csList.Sum(cs => cs.Consult2Count);
                row["column6"] = csList.Sum(cs => cs.Classify1Count);
                row["column7"] = csList.Sum(cs => cs.Classify2Count);
                row["column8"] = csList.Sum(cs => cs.TotalCount);
                sumTable.Rows.Add(row);
                #endregion

                #region 归类税号不一致
                var cdData = cdList.Where(cd => logs.Any(log => log.PartNumber == cd.Model && log.Manufacturer == cd.Manufacturer))
                    .OrderByDescending(cd => cd.UpdateDate)
                    .Select(cd => new
                    {
                        归类模块 = cd.UseType == null ? "产品归类" : cd.UseType == Enums.PreProductUserType.Pre ? "产品预归类" : "咨询归类",
                        预处理一归类人员 = cd.FirstOperator,
                        预处理一税号 = string.Empty,
                        预处理二归类人员 = cd.SecondOperator,
                        预处理二税号 = cd.HSCode,
                        不一致原因 = "归类人员认知",
                        归类完成时间 = cd.UpdateDate,
                        产品型号 = cd.Model,
                        产品品牌 = cd.Manufacturer,
                        归类税号修改日志 = string.Join(",", logs.Where(log => log.PartNumber == cd.Model && log.Manufacturer == cd.Manufacturer).Select(log => log.Summary))
                    });
                #endregion

                //文件
                string filename = "归类任务统计明细表" + DateTime.Now.Ticks + ".xlsx";
                FileDirectory fileDic5 = new FileDirectory(filename);
                fileDic5.SetChildFolder(Needs.Ccs.Services.SysConfig.Export);
                fileDic5.CreateDataDirectory();

                var templatePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Needs.Ccs.Services.SysConfig.ExportClassifiedStatistics);
                using (FileStream file = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
                {
                    XSSFWorkbook workbook = new XSSFWorkbook(file);
                    NPOIHelper npoi = new NPOIHelper(workbook);

                    npoi.SetSheet("归类统计表");
                    npoi.SetCellValue(0, 0, $"归类统计: {startDate.ToString("yyyy年MM月dd日")} - {endDate.ToString("yyyy年MM月dd日")}");
                    npoi.SetCellValue(1, 1, PurchaserContext.Current.ShortName);
                    npoi.GenerateExcelByTemplate(csData, 4, sumTable);

                    npoi.SetSheet("归类税号不一致");
                    npoi.SetCellValue(0, 0, $"归类税号不一致: {startDate.ToString("yyyy年MM月dd日")} - {endDate.ToString("yyyy年MM月dd日")}");
                    npoi.GenerateExcelByTemplate(cdData, 2);

                    npoi.SaveAs(fileDic5.FilePath);
                }

                return fileDic5.FileUrl;
            }
        }

        /// <summary>
        /// 获取归类统计信息
        /// </summary>
        /// <param name="cdList">归类明细</param>
        /// <returns></returns>
        private List<ClassifiedStatistic> GetClassifiedStatistics(List<ClassifiedDetail> cdList)
        {
            //归类总条数
            var totalClassified = cdList.Count() * 2;
            //所有的归类人员
            var adminNames = cdList.Select(item => item.FirstOperator)
                .Union(cdList.Select(item => item.SecondOperator)).Distinct();
            //归类统计
            var csList = new List<ClassifiedStatistic>();
            foreach (var adminName in adminNames)
            {
                var cs = new ClassifiedStatistic();
                cs.AdminName = adminName;
                cs.Pre1Count = cdList.Where(item => item.UseType == Enums.PreProductUserType.Pre && item.FirstOperator == adminName).Count();
                cs.Pre2Count = cdList.Where(item => item.UseType == Enums.PreProductUserType.Pre && item.SecondOperator == adminName).Count();
                cs.Consult1Count = cdList.Where(item => item.UseType == Enums.PreProductUserType.Consult && item.FirstOperator == adminName).Count();
                cs.Consult2Count = cdList.Where(item => item.UseType == Enums.PreProductUserType.Consult && item.SecondOperator == adminName).Count();
                cs.Classify1Count = cdList.Where(item => item.UseType == null && item.FirstOperator == adminName).Count();
                cs.Classify2Count = cdList.Where(item => item.UseType == null && item.SecondOperator == adminName).Count();
                cs.Percent = $"{((cs.TotalCount * 1.0m / totalClassified) * 100).ToRound(2)}%";

                csList.Add(cs);
            }

            return csList;
        }

        /// <summary>
        /// 获取归类明细
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        private List<ClassifiedDetail> GetClassifiedDetails(DateTime startDate, DateTime endDate)
        {
            using (var reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var cdList = new List<ClassifiedDetail>();
                var preCategories = (from entity in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PreProductCategories>()
                                     join pp in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PreProducts>() on entity.PreProductID equals pp.ID
                                     join admin1 in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView>() on entity.ClassifyFirstOperator equals admin1.ID
                                     join admin2 in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView>() on entity.ClassifySecondOperator equals admin2.ID
                                     where entity.UpdateDate >= startDate && entity.UpdateDate <= endDate && entity.ClassifyStatus == (int)Enums.ClassifyStatus.Done
                                     select new ClassifiedDetail
                                     {
                                         ID = entity.ID,
                                         Model = entity.Model,
                                         Manufacturer = entity.Manufacture,
                                         HSCode = entity.HSCode,
                                         UpdateDate = entity.UpdateDate,
                                         FirstOperator = admin1.RealName,
                                         SecondOperator = admin2.RealName,
                                         UseType = (Enums.PreProductUserType)pp.UseType
                                     }).ToArray();
                //产品预归类
                var pcArr = preCategories.Where(item => item.UseType == Enums.PreProductUserType.Pre).ToArray();
                //咨询归类
                var ccArr = preCategories.Where(item => item.UseType == Enums.PreProductUserType.Consult).ToArray();
                //产品归类
                var oicArr = (from entity in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemCategories>()
                              join item in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>() on entity.OrderItemID equals item.ID
                              join admin1 in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView>() on entity.ClassifyFirstOperator equals admin1.ID
                              join admin2 in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView>() on entity.ClassifySecondOperator equals admin2.ID
                              where entity.UpdateDate >= startDate && entity.UpdateDate <= endDate && item.ClassifyStatus == (int)Enums.ClassifyStatus.Done
                              select new ClassifiedDetail
                              {
                                  ID = entity.ID,
                                  Model = item.Model,
                                  Manufacturer = item.Manufacturer,
                                  HSCode = entity.HSCode,
                                  UpdateDate = entity.UpdateDate,
                                  FirstOperator = admin1.RealName,
                                  SecondOperator = admin2.RealName,
                              }).ToArray();

                cdList.AddRange(pcArr);
                cdList.AddRange(ccArr);
                cdList.AddRange(oicArr);

                return cdList;
            }
        }
    }

    /// <summary>
    /// 归类任务统计
    /// </summary>
    public class ClassifiedStatistic
    {
        /// <summary>
        /// 归类人员
        /// </summary>
        public string AdminName { get; set; }

        /// <summary>
        /// 产品预归类 - 预处理一(条数)
        /// </summary>
        public int Pre1Count { get; set; }

        /// <summary>
        /// 产品预归类 - 预处理二(条数)
        /// </summary>
        public int Pre2Count { get; set; }

        /// <summary>
        /// 咨询归类 - 预处理一(条数)
        /// </summary>
        public int Consult1Count { get; set; }

        /// <summary>
        /// 咨询归类 - 预处理二(条数)
        /// </summary>
        public int Consult2Count { get; set; }

        /// <summary>
        /// 产品归类 - 预处理一(条数)
        /// </summary>
        public int Classify1Count { get; set; }

        /// <summary>
        /// 产品归类 - 预处理二(条数)
        /// </summary>
        public int Classify2Count { get; set; }

        /// <summary>
        /// 归类(条数)
        /// </summary>
        public int TotalCount
        {
            get
            {
                return this.Pre1Count + this.Pre2Count + this.Consult1Count + this.Consult2Count + this.Classify1Count + this.Classify2Count;
            }
        }

        /// <summary>
        /// 归类占比
        /// </summary>
        public string Percent { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { get; set; }
    }

    /// <summary>
    /// 归类任务明细
    /// </summary>
    public class ClassifiedDetail
    {
        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// 海关编码
        /// </summary>
        public string HSCode { get; set; }

        /// <summary>
        /// 归类时间
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// 预处理一归类人员
        /// </summary>
        public string FirstOperator { get; set; }

        /// <summary>
        /// 预处理二归类人员
        /// </summary>
        public string SecondOperator { get; set; }

        /// <summary>
        /// null : 产品归类 Pre : 产品预归类 Consult : 咨询归类
        /// </summary>
        public Enums.PreProductUserType? UseType { get; set; }
    }
}
