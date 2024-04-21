using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbErm;
using Yahv.Erm.Services.Models.Rolls;
using Yahv.Linq;
using Yahv.Underly;
using Layers.Linq;
using Yahv.Utils.Validates;

namespace Yahv.Erm.Services.Views.Rolls
{
    /// <summary>
    /// 员工工资
    /// </summary>
    public class StaffPayItems : QueryView<StaffPayItem, PvbErmReponsitory>
    {
        public StaffPayItems()
        {
        }

        /// <summary>
        /// 可查询集
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<StaffPayItem> GetIQueryable()
        {
            var result = from pb in Reponsitory.ReadTable<PayBills>()
                         join pi in Reponsitory.ReadTable<PayItems>() on pb.ID equals pi.PayID
                         join s in Reponsitory.ReadTable<Staffs>() on pb.StaffID equals s.ID
                         join lg in Reponsitory.ReadTable<Leagues>() on s.WorkCity equals lg.ID
                         join lb in Reponsitory.ReadTable<Labours>() on s.ID equals lb.ID
                         join p in Reponsitory.ReadTable<Postions>() on s.PostionID equals p.ID
                         join po in Reponsitory.ReadTable<Postions>() on pb.PostionID equals po.ID into po_join
                         from _po in po_join.DefaultIfEmpty()
                         join pl in Reponsitory.ReadTable<Personals>() on s.ID equals pl.ID
                         join c in Reponsitory.ReadTable<CompaniesTopView>() on pb.EnterpriseID equals c.ID
                         join b in Reponsitory.ReadTable<BankCards>() on pb.StaffID equals b.ID into b_view
                         from b in b_view.DefaultIfEmpty()
                         join wi in Reponsitory.ReadTable<WageItems>() on pi.Name equals wi.Name
                         select new StaffPayItem()
                         {
                             Status = pb.Status,
                             StaffStatus = s.Status,
                             StaffStatusName = s.Status == (int)StaffStatus.Normal ? StaffStatus.Normal.GetDescription() :
                             (s.Status == (int)StaffStatus.Departure ? StaffStatus.Departure.GetDescription() :
                             (s.Status == (int)StaffStatus.Period ? StaffStatus.Period.GetDescription() :
                             (s.Status == (int)StaffStatus.Delete ? StaffStatus.Delete.GetDescription() :
                             (s.Status == (int)StaffStatus.Cancel ? StaffStatus.Cancel.GetDescription() :
                                    "")))),
                             Name = pi.Name,
                             Value = pi.Value,
                             PayID = pb.ID,
                             DateIndex = pb.DateIndex,
                             StaffID = pb.StaffID,
                             StaffCode = s.Code,
                             StaffName = s.Name,
                             IDCard = pl.IDCard,
                             DyjDepartmentCode = s.DyjDepartmentCode,
                             DyjCompanyCode = s.DyjCompanyCode,
                             City = lg.Name,
                             CompanyName = c.Name,
                             PostionName = _po.Name ?? p.Name,
                             StaffSelCode = s.SelCode,
                             BankAccount = b.Account,
                             DyjCode = s.DyjCode,
                             WageType = (WageItemType)wi.Type,
                         };

            return result;


            //string sql = @"SELECT pi.PayID,pb.StaffID,s.Name StaffName,s.Code StaffCode,pb.DateIndex,pb.Status,pi.Name,pi.Value,
            //                lg.Name City,c.Name CompanyName,s.DyjCompanyCode,s.DyjDepartmentCode,p.Name PostionName,pl.IDCard,
            //                s.SelCode StaffSelCode,bc.Account BankAccount
            //                FROM dbo.PayBills pb 
            //                LEFT JOIN PayItems pi ON pb.ID=pi.PayID
            //                LEFT JOIN dbo.Staffs s ON s.ID=pb.StaffID
            //                LEFT JOIN dbo.Leagues lg ON lg.ID=s.WorkCity
            //                LEFT JOIN dbo.Labours lb ON lb.ID=s.ID
            //                LEFT JOIN PvbCrm.dbo.CompaniesTopView c ON c.ID=lb.EnterpriseID
            //                LEFT JOIN dbo.Postions p ON p.ID =s.PostionID
            //                LEFT JOIN dbo.Personals pl ON pl.ID=s.ID
            //                LEFT JOIN dbo.BankCards bc ON s.ID=bc.ID";
            //var result = this.Reponsitory.Query<StaffPayItem>(sql).ToList().AsQueryable();
            //return result;
        }

        /// <summary>
        /// 导入数据
        /// </summary>
        /// <param name="json">json数据格式</param>
        /// <param name="dateIndex">工资月</param>
        /// <param name="adminId">adminId</param>
        /// <returns></returns>
        public string Import(string json, string dateIndex, string adminId)
        {
            string error_msg = string.Empty;

            try
            {
                if (string.IsNullOrWhiteSpace(json))
                {
                    return "json 数据不能为空!";
                }

                if (string.IsNullOrWhiteSpace(dateIndex) || string.IsNullOrWhiteSpace(adminId))
                {
                    return "工资月和录入人不能为空!";
                }

                string sql = @"EXEC	[dbo].[P_ImportWageData]
		                            @json={0},
                                    @dateIndex = {1},
		                            @adminId = {2}";

                error_msg = Reponsitory.Query<string>(sql, json, dateIndex, adminId).Single();
            }
            catch (System.Exception ex)
            {
                error_msg = ex.Message;
            }

            return error_msg;
        }

        /// <summary>
        /// 计算工资项
        /// </summary>
        /// <param name="payId"></param>
        /// <param name="adminId"></param>
        public void CalcWageByPayId(string payId, string adminId)
        {
            if (string.IsNullOrWhiteSpace(payId) || string.IsNullOrWhiteSpace(adminId))
                return;

            string sql = @"EXEC	[dbo].[P_CalcWageByPayId]
		                            @payId={0},
                                    @adminId = {1}";
            Reponsitory.Query<string>(sql, payId, adminId);
        }

        /// <summary>
        /// 根据表机构动态生成表
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        public string CreateTable(DataColumnCollection columns)
        {
            string tableName = "temp_" + DateTime.Now.ToString("yyyyMMddHHmmss");

            if (columns == null)
            {
                return string.Empty;
            }

            string column = string.Empty;

            foreach (var col in columns)
            {
                column += $"[{col}] varchar(50),";
            }

            string sql = @"
                    IF  (OBJECT_ID('{0}') IS NOT NULL ) DROP TABLE {0}
                    CREATE TABLE {0}(
                                {1}
                    );
            ";

            sql = string.Format(sql, tableName, column.Trim(','));

            Reponsitory.Query<string>(sql);

            return tableName;
        }

        /// <summary>
        /// 根据DataTable插入数据库
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dt"></param>
        public void InsertDataByTable(string tableName, DataTable dt)
        {
            this.Reponsitory.SqlBulkCopyByDatatable(tableName, dt);
        }

        /// <summary>
        /// 获取导入工资项目的状态
        /// </summary>
        /// <param name="dateIndex">工资月</param>
        /// <param name="adminId">adminId</param>
        public List<PayItemInputer> GetImportPayItemsStatus(string dateIndex, string adminId = "")
        {
            List<PayItemInputer> result = new List<PayItemInputer>();

            //获取需要导入的项目
            var wageItems = (from wi in
                     this.Reponsitory.ReadTable<WageItems>()
                         .Where(item => item.Type == (int)WageItemType.Normal && item.InputerId != "" && item.Status == (int)Status.Normal)
                             join _a in this.Reponsitory.ReadTable<Admins>() on wi.InputerId equals _a.ID into joinA
                             from a in joinA.DefaultIfEmpty()
                             select new
                             {
                                 wi.Name,
                                 a.RealName,
                                 wi.InputerId,
                             }).ToList();

            if (!string.IsNullOrWhiteSpace(adminId))
            {

                wageItems = wageItems.Where(item => item.InputerId == adminId).ToList();
            }


            //获取已经导入的项目
            var payItems = this.Reponsitory.ReadTable<PayItems>()
                        .Where(item => item.DateIndex == dateIndex && wageItems.Select(t => t.Name).Contains(item.Name) && item.Status == (int)PayItemStatus.Submit).Select(item => item.Name).ToList();

            foreach (var wageItem in wageItems)
            {
                //如果有数据，返回导入数据状态
                if (payItems.Any())
                {
                    if (payItems.Any(item => item == wageItem.Name))
                    {
                        result.Add(new PayItemInputer()
                        {
                            PayItemName = wageItem.Name,
                            InputerName = wageItem.RealName,
                            IsImport = true
                        });
                    }
                }
                else
                {
                    result.Add(new PayItemInputer()
                    {
                        PayItemName = wageItem.Name,
                        InputerName = wageItem.RealName,
                        IsImport = false
                    });
                }
            }

            return result;
        }

        /// <summary>
        /// 提交工资项
        /// </summary>
        /// <param name="items"></param>
        /// <param name="dateIndex"></param>
        public void SubmitPayItems(string[] items, string dateIndex)
        {
            if (items.Length <= 0 || string.IsNullOrWhiteSpace(dateIndex)) return;

            foreach (var item in items)
            {
                //this.Reponsitory.Update<PayItems>(new
                //{
                //    Status = (int)PayItemStatus.Submit
                //}, t => t.DateIndex == dateIndex && t.Name == item && t.Status == (int)PayItemStatus.Save);

                this.Reponsitory.Command($"update PayItems set Status={(int)PayItemStatus.Submit} where DateIndex='{dateIndex}' and Name='{item}' and (Status={(int)PayItemStatus.Save} or Status is null)");
            }
        }
    }
}