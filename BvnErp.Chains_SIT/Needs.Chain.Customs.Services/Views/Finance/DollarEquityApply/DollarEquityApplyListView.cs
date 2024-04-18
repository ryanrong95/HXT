using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class DollarEquityApplyListView : QueryView<DollarEquityApplyListViewModel, ScCustomsReponsitory>
    {
        public DollarEquityApplyListView()
        {
        }

        protected DollarEquityApplyListView(ScCustomsReponsitory reponsitory, IQueryable<DollarEquityApplyListViewModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<DollarEquityApplyListViewModel> GetIQueryable()
        {
            var dollarEquityApplies = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DollarEquityApplies>();

            var iQuery = from dollarEquityApply in dollarEquityApplies
                         where dollarEquityApply.Status == (int)Enums.Status.Normal
                         orderby dollarEquityApply.CreateDate descending
                         select new DollarEquityApplyListViewModel
                         {
                             DollarEquityApplyID = dollarEquityApply.ID,
                             ApplyID = dollarEquityApply.ApplyID,
                             Amount = dollarEquityApply.Amount,
                             Currency = dollarEquityApply.Currency,
                             SupplierChnName = dollarEquityApply.SupplierChnName,
                             SupplierEngName = dollarEquityApply.SupplierEngName,
                             BankName = dollarEquityApply.BankName,
                             BankAccount = dollarEquityApply.BankAccount,
                             SwiftCode = dollarEquityApply.SwiftCode,
                             BankAddress = dollarEquityApply.BankName,
                             IsPaid = dollarEquityApply.IsPaid,
                             CreateDate = dollarEquityApply.CreateDate,
                         };

            return iQuery;
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<DollarEquityApplyListViewModel> iquery = this.IQueryable.Cast<DollarEquityApplyListViewModel>();
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_dollarEquityApplys = iquery.ToArray();

            var dollarEquityApplyIDs = ienum_dollarEquityApplys.Select(item => item.DollarEquityApplyID);

            #region 支付凭证

            var dollarEquityApplyFiles = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DollarEquityApplyFiles>();

            var linq_dollarEquityApplyFile = from dollarEquityApplyFile in dollarEquityApplyFiles
                                             where dollarEquityApplyFile.Status == (int)Enums.Status.Normal
                                                && dollarEquityApplyIDs.Contains(dollarEquityApplyFile.DollarEquityApplyID)
                                             orderby dollarEquityApplyFile.CreateDate descending
                                             group dollarEquityApplyFile by new { dollarEquityApplyFile.DollarEquityApplyID, } into g
                                             select new
                                             {
                                                 DollarEquityApplyID = g.Key.DollarEquityApplyID,
                                                 DollarEquityApplyFileID = g.FirstOrDefault().ID,
                                                 DollarEquityApplyFileName = g.FirstOrDefault().Name,
                                                 DollarEquityApplyFileUrl = g.FirstOrDefault().Url,
                                             };

            var ienums_dollarEquityApplyFile = linq_dollarEquityApplyFile.ToArray();

            #endregion

            var ienums_linq = from dollarEquityApply in ienum_dollarEquityApplys
                              join dollarEquityApplyFile in ienums_dollarEquityApplyFile on dollarEquityApply.DollarEquityApplyID equals dollarEquityApplyFile.DollarEquityApplyID
                              into ienums_dollarEquityApplyFile2
                              from dollarEquityApplyFile in ienums_dollarEquityApplyFile2.DefaultIfEmpty()
                              select new DollarEquityApplyListViewModel
                              {
                                  DollarEquityApplyID = dollarEquityApply.DollarEquityApplyID,
                                  ApplyID = dollarEquityApply.ApplyID,
                                  Amount = dollarEquityApply.Amount,
                                  Currency = dollarEquityApply.Currency,
                                  SupplierChnName = dollarEquityApply.SupplierChnName,
                                  SupplierEngName = dollarEquityApply.SupplierEngName,
                                  BankName = dollarEquityApply.BankName,
                                  BankAccount = dollarEquityApply.BankAccount,
                                  SwiftCode = dollarEquityApply.SwiftCode,
                                  BankAddress = dollarEquityApply.BankName,
                                  IsPaid = dollarEquityApply.IsPaid,
                                  CreateDate = dollarEquityApply.CreateDate,

                                  FileName = dollarEquityApplyFile != null ? dollarEquityApplyFile.DollarEquityApplyFileName : "",
                                  FileUrl = dollarEquityApplyFile != null ? dollarEquityApplyFile.DollarEquityApplyFileUrl : "",
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

            string FileServerUrl = System.Configuration.ConfigurationManager.AppSettings["FileServerUrl"];

            Func<DollarEquityApplyListViewModel, object> convert = dollarEquityApply => new
            {
                DollarEquityApplyID = dollarEquityApply.DollarEquityApplyID,
                ApplyID = dollarEquityApply.ApplyID,
                Amount = dollarEquityApply.Amount,
                Currency = dollarEquityApply.Currency,
                SupplierChnName = dollarEquityApply.SupplierChnName,
                SupplierEngName = dollarEquityApply.SupplierEngName,
                BankName = dollarEquityApply.BankName,
                BankAccount = dollarEquityApply.BankAccount,
                SwiftCode = dollarEquityApply.SwiftCode,
                BankAddress = dollarEquityApply.BankName,
                IsPaid = dollarEquityApply.IsPaid ? "1" : "0",
                IsPaidStr = dollarEquityApply.IsPaid ? "是" : "否",

                FileName = dollarEquityApply.FileName,
                FileUrl = !string.IsNullOrEmpty(dollarEquityApply.FileUrl) ? FileServerUrl + @"/" + dollarEquityApply.FileUrl.ToUrl() : "",
            };

            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = results.Select(convert).ToArray(),
            };
        }

        /// <summary>
        /// 根据是否处理查询
        /// </summary>
        /// <param name="isPaid"></param>
        /// <returns></returns>
        public DollarEquityApplyListView SearchByIsPaid(bool isPaid)
        {
            var linq = from query in this.IQueryable
                       where query.IsPaid == isPaid
                       select query;

            var view = new DollarEquityApplyListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据开始申请日期查询
        /// </summary>
        /// <param name="startDate"></param>
        /// <returns></returns>
        public DollarEquityApplyListView SearchByStartDate(DateTime startDate)
        {
            var linq = from query in this.IQueryable
                       where query.CreateDate >= startDate
                       select query;

            var view = new DollarEquityApplyListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据结束申请日期查询
        /// </summary>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public DollarEquityApplyListView SearchByEndDate(DateTime endDate)
        {
            var linq = from query in this.IQueryable
                       where query.CreateDate < endDate
                       select query;

            var view = new DollarEquityApplyListView(this.Reponsitory, linq);
            return view;
        }

    }

    public class DollarEquityApplyListViewModel
    {
        /// <summary>
        /// DollarEquityApplyID
        /// </summary>
        public string DollarEquityApplyID { get; set; }

        /// <summary>
        /// 申请ID
        /// </summary>
        public string ApplyID { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 币制
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 供应商中文名称
        /// </summary>
        public string SupplierChnName { get; set; }

        /// <summary>
        /// 供应商英文名称
        /// </summary>
        public string SupplierEngName { get; set; }

        /// <summary>
        /// 银行名称
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// 银行账号
        /// </summary>
        public string BankAccount { get; set; }

        /// <summary>
        /// 银行代码
        /// </summary>
        public string SwiftCode { get; set; }

        /// <summary>
        /// 银行地址
        /// </summary>
        public string BankAddress { get; set; }

        /// <summary>
        /// 是否付款
        /// </summary>
        public bool IsPaid { get; set; }

        /// <summary>
        /// 申请日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 支付凭证文件名称
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 支付凭证文件Url
        /// </summary>
        public string FileUrl { get; set; }
    }

}
