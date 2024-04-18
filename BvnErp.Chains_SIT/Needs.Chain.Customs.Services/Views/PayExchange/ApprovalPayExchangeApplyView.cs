using Layer.Data.Sqls;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class ApprovalPayExchangeApplyView : QueryView<Models.ApprovalPayExchangApply, ScCustomsReponsitory>
    {
        public ApprovalPayExchangeApplyView()
        {
        }

        protected ApprovalPayExchangeApplyView(ScCustomsReponsitory reponsitory, IQueryable<ApprovalPayExchangApply> iQueryable) : base(reponsitory, iQueryable)
        {
        }
        protected override IQueryable<Models.ApprovalPayExchangApply> GetIQueryable()
        {
            var payExchangeApplies = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>();
            var clientView = new ClientsView(this.Reponsitory);
            var usersView = new UsersView(this.Reponsitory);
            var adminsView = new AdminsTopView(this.Reponsitory);

            var result = from payApply in payExchangeApplies
                         join oneApply in payExchangeApplies.Where(t => t.FatherID != null) on payApply.ID equals oneApply.FatherID into one_apply
                         from one in one_apply.DefaultIfEmpty()
                         join client in clientView on payApply.ClientID equals client.ID
                         join user in usersView on payApply.UserID equals user.ID into users
                         from _user in users.DefaultIfEmpty()
                         join admin in adminsView on payApply.AdminID equals admin.ID into admins
                         from _admin in admins.DefaultIfEmpty()
                         where payApply.Status == (int)Enums.Status.Normal
                         && one.ID == null
                         select new ApprovalPayExchangApply
                         {
                             ID = payApply.ID,
                             SupplierName = payApply.SupplierName,
                             SupplierEnglishName = payApply.SupplierEnglishName,
                             SupplierAddress = payApply.SupplierAddress,
                             BankName = payApply.BankName,
                             BankAccount = payApply.BankAccount,
                             BankAddress = payApply.BankAddress,
                             SwiftCode = payApply.SwiftCode,
                             ClientID = payApply.ClientID,
                             Client = client,
                             Currency = payApply.Currency,
                             ExchangeRate = payApply.ExchangeRate,
                             ExchangeRateType = (Enums.ExchangeRateType)payApply.ExchangeRateType,
                             ExpectPayDate = payApply.ExpectPayDate,
                             SettlemenDate = payApply.SettlemenDate,
                             PayExchangeApplyStatus = (Enums.PayExchangeApplyStatus)payApply.PayExchangeApplyStatus,
                             PaymentType = (Enums.PaymentType)payApply.PaymentType,
                             OtherInfo = payApply.OtherInfo,
                             User = _user,
                             Admin = _admin,
                             Status = (Enums.Status)payApply.Status,
                             CreateDate = payApply.CreateDate,
                             UpdateDate = payApply.UpdateDate,
                             Summary = payApply.Summary,
                             ABA = payApply.ABA,
                             IBAN = payApply.IBAN,
                             FatherID = payApply.FatherID,
                             IsAdvanceMoney = payApply.IsAdvanceMoney
                         };

            return result;
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<Models.ApprovalPayExchangApply> iquery = this.IQueryable.Cast<Models.ApprovalPayExchangApply>().OrderByDescending(item => item.CreateDate);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue) //如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_my = iquery.ToArray();

            ///分组获取总金额
            var queryAmount = from payApplyItems in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>()
                              group payApplyItems by payApplyItems.PayExchangeApplyID into payApplyItem
                              select new
                              {
                                  PayExchangeApplyID = payApplyItem.Key,
                                  TotalAmount = payApplyItem.Sum(t => t.Amount),
                              };

            var ienums_linq = from payApplyNew in ienum_my
                              let totalAmount = queryAmount.SingleOrDefault(item => item.PayExchangeApplyID == payApplyNew.ID)
                              select new ApprovalPayExchangApply
                              {
                                  ID = payApplyNew.ID,
                                  SupplierName = payApplyNew.SupplierName,
                                  SupplierEnglishName = payApplyNew.SupplierEnglishName,
                                  SupplierAddress = payApplyNew.SupplierAddress,
                                  BankName = payApplyNew.BankName,
                                  BankAccount = payApplyNew.BankAccount,
                                  BankAddress = payApplyNew.BankAddress,
                                  SwiftCode = payApplyNew.SwiftCode,
                                  ClientID = payApplyNew.ClientID,
                                  Client = payApplyNew.Client,
                                  Currency = payApplyNew.Currency,
                                  ExchangeRate = payApplyNew.ExchangeRate,
                                  ExchangeRateType = payApplyNew.ExchangeRateType,
                                  ExpectPayDate = payApplyNew.ExpectPayDate,
                                  SettlemenDate = payApplyNew.SettlemenDate,
                                  PayExchangeApplyStatus = payApplyNew.PayExchangeApplyStatus,
                                  PaymentType = payApplyNew.PaymentType,
                                  OtherInfo = payApplyNew.OtherInfo,
                                  User = payApplyNew.User,
                                  Admin = payApplyNew.Admin,
                                  Status = payApplyNew.Status,
                                  CreateDate = payApplyNew.CreateDate,
                                  UpdateDate = payApplyNew.UpdateDate,
                                  Summary = payApplyNew.Summary,
                                  ABA = payApplyNew.ABA,
                                  IBAN = payApplyNew.IBAN,
                                  TotalAmount = totalAmount.TotalAmount,
                                  FatherID = payApplyNew.FatherID,
                                  IsAdvanceMoney = payApplyNew.IsAdvanceMoney
                              };

            var results = ienums_linq;

            if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                return results.Select(item =>
                {
                    object o = item;
                    return o;
                }).ToArray();
            }

            Func<ApprovalPayExchangApply, object> convert = payExchangApply => new
            {
                ID = payExchangApply.ID,
                CreateDate = payExchangApply.CreateDate.ToShortDateString(),
                ClientCode = payExchangApply.Client.ClientCode,
                SupplierName = payExchangApply.SupplierName,
                SupplierEnglishName = payExchangApply.SupplierEnglishName,
                BankName = payExchangApply.BankName,
                BankAccount = payExchangApply.BankAccount,
                Status = payExchangApply.PayExchangeApplyStatus == PayExchangeApplyStatus.Audited ? "待审批" : payExchangApply.PayExchangeApplyStatus.GetDescription(),
                Currency = payExchangApply.Currency,
                TotalAmount = payExchangApply.TotalAmount,
                FatherID = payExchangApply.FatherID != null ? "Ⅱ" : "Ⅰ",
                IsAdvanceMoney = payExchangApply.IsAdvanceMoney == 0 ? "是" : "否"
            };

            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = results.OrderByDescending(item => item.CreateDate).Select(convert).ToArray(),
            };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ApprovalPayExchangeApplyView SearchByID(string id)
        {
            var linq = from query in this.IQueryable
                       where query.ID.Contains(id)
                       select query;

            var view = new ApprovalPayExchangeApplyView(this.Reponsitory, linq);
            return view;
        }


        /// <summary>
        /// 查询客户名称
        /// </summary>
        /// <param name="ownerName">客户编号</param>
        /// <returns>视图</returns>
        public ApprovalPayExchangeApplyView SearchByClientCode(string ClientCode)
        {
            var linq = from query in this.IQueryable
                       where query.Client.ClientCode == ClientCode
                       select query;

            var view = new ApprovalPayExchangeApplyView(this.Reponsitory, linq);
            return view;
        }
        /// <summary>
        /// 查询报告日期
        /// </summary>
        /// <param name="startTime"></param>
        /// <returns></returns>
        public ApprovalPayExchangeApplyView SearchByStartDate(DateTime startTime)
        {
            var linq = from query in this.IQueryable
                       where query.CreateDate >= startTime
                       select query;

            var view = new ApprovalPayExchangeApplyView(this.Reponsitory, linq);
            return view;
        }
        /// <summary>
        /// 根据申请日期结束时间查询
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        public ApprovalPayExchangeApplyView SearchByEndDate(DateTime endTime)
        {
            var linq = from query in this.IQueryable
                       where query.CreateDate < endTime
                       select query;

            var view = new ApprovalPayExchangeApplyView(this.Reponsitory, linq);
            return view;
        }
        /// <summary>
        /// 审核状态
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        public ApprovalPayExchangeApplyView SearchByStatus(int AuditedInt)
        {
            if (AuditedInt == (int)Enums.PayExchangeApplyStatus.Audited)
            {
                var linq = from query in this.IQueryable
                           where query.PayExchangeApplyStatus == Enums.PayExchangeApplyStatus.Audited
                           select query;

                var view = new ApprovalPayExchangeApplyView(this.Reponsitory, linq);
                return view;
            }
            else
            {
                var linq = from query in this.IQueryable
                           where query.PayExchangeApplyStatus == Enums.PayExchangeApplyStatus.Approvaled || query.PayExchangeApplyStatus == Enums.PayExchangeApplyStatus.Completed
                           select query;

                var view = new ApprovalPayExchangeApplyView(this.Reponsitory, linq);
                return view;
            }

        }

        // <summary>
        /// 根据订单号查询 付汇申请
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        public ApprovalPayExchangeApplyView SearchByOrderID(string orderid)
        {
            //var payexchangeitems = new PayExchangeApplieItemsOriginView(this.Reponsitory);
            var linq = from query in this.IQueryable
                       join item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>() on query.ID equals item.PayExchangeApplyID
                       where item.OrderID == orderid
                       select query;

            var view = new ApprovalPayExchangeApplyView(this.Reponsitory, linq);
            return view;
        }
    }
}
