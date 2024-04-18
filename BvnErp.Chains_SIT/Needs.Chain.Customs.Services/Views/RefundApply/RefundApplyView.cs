using Layer.Data.Sqls;
using Needs.Ccs.Services.Enums;
using Needs.Linq;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class RefundApplyView : UniqueView<Models.RefundApply, ScCustomsReponsitory>
    {
        public RefundApplyView()
        {
        }

        protected RefundApplyView(ScCustomsReponsitory reponsitory, IQueryable<Models.RefundApply> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<Models.RefundApply> GetIQueryable()
        {
            var clientsView = new ClientsView(this.Reponsitory);

            return from apply in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.RefundApply>()
                   join account in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceAccounts>() on apply.PayeeAccountID equals account.ID into accounts
                   from account in accounts.DefaultIfEmpty()
                   join client in clientsView on apply.ClientID equals client.ID into clients
                   from client in clients.DefaultIfEmpty()
                   select new Models.RefundApply
                   {
                       ID = apply.ID,
                       FinanceReceiptID = apply.FinanceReceiptID,
                       Client = client,
                       PayeeAccount = new Models.FinanceAccount
                       {
                           ID = apply.PayeeAccountID,
                           AccountName = account==null?"":account.AccountName,
                           BankAccount = account == null ? "" : account.BankAccount,
                           BankName = account == null ? "" : account.BankName,
                       },
                       PayeeAccountID = apply.PayeeAccountID,
                       Amount = apply.Amount,
                       Currency = apply.Currency,
                       ExchangeRate = apply.ExchangeRate,
                       ApplyStatus = (RefundApplyStatus)apply.ApplyStatus,
                       Applicant = new Models.Admin
                       {
                           ID = apply.AdminID
                       },
                       Status = (Status)apply.Status,
                       CreateDate = apply.CreateDate,
                       UpdateDate = apply.UpdateDate,                      
                       Summary = apply.Summary,
                   };
        }

        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<Models.RefundApply> iquery = this.IQueryable.Cast<Models.RefundApply>().OrderByDescending(item => item.CreateDate);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            var ienum_myDeclares = iquery.ToArray();
            var AdminIDs = ienum_myDeclares.Select(item => item.Applicant.ID).ToList().Distinct();
            var adminTopView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView2>().Where(t=>AdminIDs.Contains(t.OriginID)).ToArray();

            var results = from apply in ienum_myDeclares
                          join admin in adminTopView on apply.Applicant.ID equals admin.OriginID
                          select new Models.RefundApply
                          {
                              ID = apply.ID,
                              Client = apply.Client,
                              FinanceReceiptID = apply.FinanceReceiptID,
                              PayeeAccount = apply.PayeeAccount,
                              PayeeAccountID = apply.PayeeAccountID,
                              Amount = apply.Amount,
                              Currency = apply.Currency,
                              ExchangeRate = apply.ExchangeRate,
                              ApplyStatus = apply.ApplyStatus,
                              Applicant = new Models.Admin
                              {
                                  ID = apply.Applicant.ID,
                                  RealName = admin.RealName
                              },
                              Status = apply.Status,
                              CreateDate = apply.CreateDate,
                              UpdateDate = apply.UpdateDate,                             
                              Summary = apply.Summary,
                          };

            Func<Needs.Ccs.Services.Models.RefundApply, object> convert = apply => new
            {
                ID = apply.ID,
                FinanceReceiptID = apply.FinanceReceiptID,
                CompanyName = apply.Client.Company.Name,
                Amount = apply.Amount,
                Currency = apply.Currency,
                ExchangeRate = apply.ExchangeRate,
                ApplyStatus = apply.ApplyStatus,
                ApplyStatusDesc = apply.ApplyStatus.GetDescription(),
                ApplicantName = apply.Applicant.RealName,
                Status = apply.Status,
                CreateDate = apply.CreateDate.ToString("yyyy-MM-dd"),
                UpdateDate = apply.UpdateDate,
                Summary = apply.Summary,
            };


            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = results.Select(convert).ToArray(),
            };
        }

        public RefundApplyView SearchByFrom(DateTime fromtime)
        {
            var linq = from query in this.IQueryable
                       where query.CreateDate >= fromtime
                       select query;

            var view = new RefundApplyView(this.Reponsitory, linq);
            return view;
        }

        public RefundApplyView SearchByTo(DateTime fromtime)
        {
            var linq = from query in this.IQueryable
                       where query.CreateDate <= fromtime
                       select query;

            var view = new RefundApplyView(this.Reponsitory, linq);
            return view;
        }

        public RefundApplyView SearchByAccountName(string accountName)
        {
            var linq = from query in this.IQueryable
                       where query.Client.Company.Name == accountName
                       select query;

            var view = new RefundApplyView(this.Reponsitory, linq);
            return view;
        }

        public RefundApplyView SearchByAdminID(string adminID)
        {
            var linq = from query in this.IQueryable
                       where query.Applicant.ID == adminID
                       select query;

            var view = new RefundApplyView(this.Reponsitory, linq);
            return view;
        }

        public RefundApplyView SearchByApplyStatus(RefundApplyStatus status)
        {
            var linq = from query in this.IQueryable
                       where query.ApplyStatus == status
                       select query;

            var view = new RefundApplyView(this.Reponsitory, linq);
            return view;
        }

        public RefundApplyView SearchPayeeAccountIsNull()
        {
            var linq = from query in this.IQueryable
                       where query.PayeeAccountID == null
                       select query;

            var view = new RefundApplyView(this.Reponsitory, linq);
            return view;
        }
    }
}
