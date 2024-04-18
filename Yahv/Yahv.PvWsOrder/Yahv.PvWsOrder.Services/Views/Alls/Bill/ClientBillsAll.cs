using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Yahv.Linq;
using Yahv.Linq.Generic;
using Yahv.Payments;
using Yahv.PvWsOrder.Services.Models;
using Yahv.Services.Models;
using Yahv.Services.Views;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.Views.Alls
{
    public class BillsBase : UniqueView<Bill, PvWsOrderReponsitory>
    {
        public BillsBase()
        {
        }

        public BillsBase(PvWsOrderReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Bill> GetIQueryable()
        {
            //客户视图
            var admins = new AdminsAll(this.Reponsitory);
            var clients = new WsClientsAlls(this.Reponsitory).Where(item => item.Status == ApprovalStatus.Normal);
            var bills = Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.Bills>();

            var linq = from entity in bills
                       join client in clients on entity.ClientID equals client.ID
                       join admin in admins on entity.AdminID equals admin.ID
                       select new Bill()
                       {
                           ID = entity.ID,
                           ClientID = entity.ClientID,
                           Currency = (Currency)entity.Currency,
                           IsInvoice = entity.IsInvoice,
                           AdminID = entity.AdminID,
                           Status = (GeneralStatus)entity.Status,
                           CreateDate = entity.CreateDate,
                           ModifyDate = entity.ModifyDate,
                           Summary = entity.Summary,

                           Client = client,
                           Creater = admin,
                       };
            return linq;
        }
    }

    /// <summary>
    /// 客户账单视图
    /// </summary>
    public class BillsAll : UniqueView<Bill, PvWsOrderReponsitory>
    {
        public BillsAll()
        {
        }

        public BillsAll(PvWsOrderReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Bill> GetIQueryable()
        {
            //客户视图
            var admins = new AdminsAll(this.Reponsitory);
            var clients = new WsClientsAlls(this.Reponsitory).Where(item => item.Status == ApprovalStatus.Normal);
            var bills = Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.Bills>();
            var billItems = new BillItemsAll(this.Reponsitory);

            var linq = from entity in bills
                       join client in clients on entity.ClientID equals client.ID
                       join admin in admins on entity.AdminID equals admin.ID
                       join item in billItems on entity.ID equals item.BillID into items
                       select new Bill()
                       {
                           ID = entity.ID,
                           ClientID = entity.ClientID,
                           Currency = (Currency)entity.Currency,
                           IsInvoice = entity.IsInvoice,
                           AdminID = entity.AdminID,
                           Status = (GeneralStatus)entity.Status,
                           CreateDate = entity.CreateDate,
                           ModifyDate = entity.ModifyDate,
                           Summary = entity.Summary,

                           Client = client,
                           Items = items,
                           Creater = admin,
                       };
            return linq;
        }
    }

    /// <summary>
    /// 客户账单项视图
    /// </summary>
    public class BillItemsAll : UniqueView<BillItem, PvWsOrderReponsitory>
    {
        #region 构造函数

        public BillItemsAll()
        {
        }

        public BillItemsAll(PvWsOrderReponsitory reponsitory) : base(reponsitory)
        {

        }

        public BillItemsAll(PvWsOrderReponsitory reponsitory, IQueryable<BillItem> iQueryable) : base(reponsitory, iQueryable)
        {

        }

        #endregion

        protected override IQueryable<BillItem> GetIQueryable()
        {
            var linq = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.BillItems>()
                       select new BillItem()
                       {
                           ID = entity.ID,
                           BillID = entity.BillID,
                           OrderID = entity.OrderID,
                       };
            return linq;
        }

        public BillItem[] ToMyObject()
        {
            var ienum_query = this.IQueryable.ToArray();

            var orderids = ienum_query.Select(item => item.OrderID).Distinct().ToArray();
            var vouchersView = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.VouchersCnyStatisticsView>()
                               where orderids.Contains(entity.OrderID)
                               select entity;
            var ienum_vouchers = vouchersView
                .Where(t=>t.Payee == Common.Helper.XdtCompanyID)
                .Where(t => t.Status == (int)GeneralStatus.Normal && t.Business == ConductConsts.供应链)
                .Where(t => t.Subject != Payments.SubjectConsts.代付货款 && t.Subject != Payments.SubjectConsts.代收货款).ToArray();

            var ienum_rates = Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.ExchangeRateHistoriesTopView>().Where(t => t.Code == "HKD").ToArray();

            var statistics = from t in ienum_vouchers
                             join rate in ienum_rates on new { t.LeftDate.Year, t.LeftDate.Month, t.LeftDate.Day } equals new { rate.Date.Value.Year, rate.Date.Value.Month, rate.Date.Value.Day }                             
                             select new VoucherCnyStatistic()
                             {
                                 OrderID = t.OrderID,
                                 ApplicationID = t.ApplicationID,
                                 ReceivableID = t.ReceivableID,
                                 Business = t.Business,
                                 Subject = t.Subject,
                                 Catalog = t.Catalog,
                                 PayerID = t.Payer,
                                 PayeeID = t.Payee,

                                 OriginCurrency = (Currency)t.OriginCurrency,
                                 OriginPrice = t.OriginPrice,
                                 Currency = (Currency)t.Currency,
                                 LeftPrice = t.LeftPrice,
                                 RightPrice = t.RightPrice,
                                 ReducePrice = t.ReducePrice,
                                 LeftDate = t.LeftDate,
                                 OriginalDate = t.OriginalDate,
                                 RightDate = t.RightDate,
                                 AdminID = t.AdminID,
                                 Status = (GeneralStatus)t.Status,
                                 Rate = t.Rate,

                                 HKDTOCNYRate = rate.Rate,
                                 HKDLeftPrice = t.LeftPrice / rate.Rate,
                                 HKDRightPrice = t.RightPrice / rate.Rate,
                             };

            var outputView = new OrderOutputAlls(this.Reponsitory).Where(item => orderids.Contains(item.ID)).ToArray();

            var linq = from entity in ienum_query
                       join cny in statistics on entity.OrderID equals cny.OrderID into cnys
                       join output in outputView on entity.OrderID equals output.ID into outputs
                       from output in outputs.DefaultIfEmpty()
                       select new BillItem()
                       {
                           ID = entity.ID,
                           BillID = entity.BillID,
                           OrderID = entity.OrderID,
                           CnyStatistics = cnys,
                           OrderOutput = output,
                       };

            return linq.ToArray();
        }

        public BillItem[] ToMyObjectCNY()
        {
            var ienum_query = this.IQueryable.ToArray();

            var orderids = ienum_query.Select(item => item.OrderID).Distinct().ToArray();
            var vouchersView = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.VouchersCnyStatisticsView>()
                               where orderids.Contains(entity.OrderID)
                               select entity;
            var ienum_vouchers = vouchersView
                .Where(t => t.Payee == Common.Helper.XdtCompanyID)
                .Where(t => t.Status == (int)GeneralStatus.Normal && t.Business == ConductConsts.供应链)
                .Where(t => t.Subject != Payments.SubjectConsts.代付货款 && t.Subject != Payments.SubjectConsts.代收货款).ToArray();

            var statistics = from t in ienum_vouchers
                             select new VoucherCnyStatistic()
                             {
                                 OrderID = t.OrderID,
                                 ApplicationID = t.ApplicationID,
                                 ReceivableID = t.ReceivableID,
                                 Business = t.Business,
                                 Subject = t.Subject,
                                 Catalog = t.Catalog,
                                 PayerID = t.Payer,
                                 PayeeID = t.Payee,

                                 OriginCurrency = (Currency)t.OriginCurrency,
                                 OriginPrice = t.OriginPrice,
                                 Currency = (Currency)t.Currency,
                                 LeftPrice = t.LeftPrice,
                                 RightPrice = t.RightPrice,
                                 ReducePrice = t.ReducePrice,
                                 LeftDate = t.LeftDate,
                                 OriginalDate = t.OriginalDate,
                                 RightDate = t.RightDate,
                                 AdminID = t.AdminID,
                                 Status = (GeneralStatus)t.Status,
                                 Rate = t.Rate,

                                 HKDTOCNYRate = 0,
                                 HKDLeftPrice = 0,
                                 HKDRightPrice = 0,
                             };

            var outputView = new OrderOutputAlls(this.Reponsitory).Where(item => orderids.Contains(item.ID)).ToArray();

            var linq = from entity in ienum_query
                       join cny in statistics on entity.OrderID equals cny.OrderID into cnys
                       join output in outputView on entity.OrderID equals output.ID into outputs
                       from output in outputs.DefaultIfEmpty()
                       select new BillItem()
                       {
                           ID = entity.ID,
                           BillID = entity.BillID,
                           OrderID = entity.OrderID,
                           CnyStatistics = cnys,
                           OrderOutput = output,
                       };

            return linq.ToArray();
        }

        #region 查询方法

        /// <summary>
        /// 根据账单ID搜索
        /// </summary>
        /// <param name="billID"></param>
        /// <returns></returns>
        public BillItemsAll SearchByID(string billID)
        {
            var items = this.IQueryable.Cast<BillItem>();
            var linq = from entity in items
                       where entity.BillID == billID
                       select entity;

            var view = new BillItemsAll(this.Reponsitory, linq)
            {
            };
            return view;
        }

        /// <summary>
        /// 根据账单IDs搜索
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public BillItemsAll SearchByIDs(string[] ids)
        {
            var items = this.IQueryable.Cast<BillItem>();
            var linq = from entity in items
                       where ids.Contains(entity.BillID)
                       select entity;

            var view = new BillItemsAll(this.Reponsitory, linq)
            {
            };
            return view;
        }

        #endregion
    }

    /// <summary>
    /// 开票申请视图
    /// </summary>
    public class Bills_Show_View : QueryView<BillShow, PvWsOrderReponsitory>
    {

        #region 构造函数

        //系统管理员
        private Underly.Erps.IErpAdmin admin;

        public Bills_Show_View(Underly.Erps.IErpAdmin admin)
        {
            this.admin = admin;
        }

        protected Bills_Show_View(PvWsOrderReponsitory reponsitory, IQueryable<BillShow> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        #endregion

        protected override IQueryable<BillShow> GetIQueryable()
        {
            var bills = Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.Bills>();
            var clients = new WsClientsTopView<PvWsOrderReponsitory>(this.Reponsitory).Where(item => item.Status == ApprovalStatus.Normal);
            var admins = new AdminsAll(this.Reponsitory);

            if (!admin.IsSuper)
            {
                //管理员的代仓储客户
                var clientIds = new TrackerWsClients<PvWsOrderReponsitory>(this.Reponsitory, this.admin, Common.Helper.XdtCompanyID)
                    .Select(item => item.ID).ToArray();
                bills = bills.Where(item => clientIds.Contains(item.ClientID));
            }

            var linq = from entity in bills
                       join client in clients on entity.ClientID equals client.ID
                       join admin in admins on entity.AdminID equals admin.ID
                       select new BillShow
                       {
                           ID = entity.ID,
                           ClientID = client.ID,
                           CreateDate = entity.CreateDate,
                           Currency = (Currency)entity.Currency,
                           Status = (GeneralStatus)entity.Status,
                           IsInvoice = entity.IsInvoice,
                           ClientName = client.Name,
                           EnterCode = client.EnterCode,
                           Creator = admin.RealName,
                       };

            return linq;
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public Tuple<IEnumerable<BillShow>, int> ToMyPage(int pageIndex = 1, int pageSize = 20)
        {
            IQueryable<BillShow> iquery = this.IQueryable.Cast<BillShow>().OrderByDescending(t => t.CreateDate);
            int total = iquery.Count();
            iquery = iquery.Skip(pageSize * (pageIndex - 1)).Take(pageSize);

            #region 账单金额
            var bills = iquery.ToArray();
            var billIds = iquery.Select(item => item.ID).ToArray();
            var billItems = Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.BillItems>().Where(t => billIds.Contains(t.BillID)).ToArray();
            var orderIds = billItems.Select(item => item.OrderID).ToArray();

            var vouchersView = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.VouchersCnyStatisticsView>()
                               where orderIds.Contains(entity.OrderID)
                               where (entity.Payee == Common.Helper.XdtCompanyID)
                               where (entity.Status == (int)GeneralStatus.Normal && entity.Business == ConductConsts.供应链)
                               where (entity.Subject != Payments.SubjectConsts.代付货款 && entity.Subject != Payments.SubjectConsts.代收货款)
                               select entity;
            var ienum_vouchers = vouchersView.ToArray();

            var ienum_rates = Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.ExchangeRateHistoriesTopView>().Where(t => t.Code == "HKD").ToArray();

            var statistics = from t in ienum_vouchers
                             join rate in ienum_rates on new { t.LeftDate.Year, t.LeftDate.Month, t.LeftDate.Day } equals new { rate.Date.Value.Year, rate.Date.Value.Month, rate.Date.Value.Day }
                             select new VoucherCnyStatistic()
                             {
                                 OrderID = t.OrderID,
                                 ApplicationID = t.ApplicationID,
                                 ReceivableID = t.ReceivableID,
                                 Business = t.Business,
                                 Subject = t.Subject,
                                 Catalog = t.Catalog,
                                 PayerID = t.Payer,
                                 PayeeID = t.Payee,

                                 OriginCurrency = (Currency)t.OriginCurrency,
                                 OriginPrice = t.OriginPrice,
                                 Currency = (Currency)t.Currency,
                                 LeftPrice = t.LeftPrice,
                                 RightPrice = t.RightPrice,
                                 ReducePrice = t.ReducePrice,
                                 LeftDate = t.LeftDate,
                                 OriginalDate = t.OriginalDate,
                                 RightDate = t.RightDate,
                                 AdminID = t.AdminID,
                                 Status = (GeneralStatus)t.Status,
                                 Rate = t.Rate,

                                 HKDTOCNYRate = rate.Rate,
                                 HKDLeftPrice = t.LeftPrice / rate.Rate,
                                 HKDRightPrice = t.RightPrice / rate.Rate,
                             };

            var item_query = from entity in billItems
                             join statistic in statistics on entity.OrderID equals statistic.OrderID into ss
                             select new
                             {
                                 entity.BillID,
                                 CnyLeftPrice = ss.Sum(t => t.LeftPrice),
                                 HkdLeftPrice = ss.Sum(t => t.LeftPrice / t.Rate)
                             };

            #endregion

            var linq = from entity in bills
                       join item in item_query on entity.ID equals item.BillID into items
                       select new BillShow
                       {
                           ID = entity.ID,
                           ClientID = entity.ClientID,
                           CreateDate = entity.CreateDate,
                           Currency = entity.Currency,
                           Status = entity.Status,
                           IsInvoice = entity.IsInvoice,
                           ClientName = entity.ClientName,
                           EnterCode = entity.EnterCode,
                           Creator = entity.Creator,
                           CnyPrice = items.Sum(t => t.CnyLeftPrice),
                           HkdPrice = items.Sum(t => t.HkdLeftPrice)
                       };

            return new Tuple<IEnumerable<BillShow>, int>(linq, total);
        }

        #region 查询方法

        /// <summary>
        /// 根据客户名称查询
        /// </summary>
        /// <param name="companyName"></param>
        /// <returns></returns>
        public Bills_Show_View SearchByClientName(string clientName)
        {
            var linq = from query in this.IQueryable
                       where query.ClientName.Contains(clientName)
                       select query;

            var view = new Bills_Show_View(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据客户编号查询
        /// </summary>
        /// <param name="companyName"></param>
        /// <returns></returns>
        public Bills_Show_View SearchByClientCode(string clientCode)
        {
            var linq = from query in this.IQueryable
                       where query.EnterCode.Contains(clientCode)
                       select query;

            var view = new Bills_Show_View(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据账单币种查询
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        public Bills_Show_View SearchByCurrency(Currency currency)
        {
            var linq = from query in this.IQueryable
                       where query.Currency == currency
                       select query;

            var view = new Bills_Show_View(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据是否开票查询
        /// </summary>
        /// <param name="IsInvoice"></param>
        /// <returns></returns>
        public Bills_Show_View SearchByIsInvoice(bool isInvoice)
        {
            var linq = from query in this.IQueryable
                       where query.IsInvoice == isInvoice
                       select query;

            var view = new Bills_Show_View(this.Reponsitory, linq);
            return view;
        }

        #endregion

    }

    public class BillShow
    {
        public string ID { get; set; }
        public string ClientID { get; set; }

        public DateTime CreateDate { get; set; }

        public string ClientName { get; set; }

        public string EnterCode { get; set; }

        public Currency Currency { get; set; }

        public GeneralStatus Status { get; set; }

        public bool IsInvoice { get; set; }

        /// <summary>
        /// 本位币总金额
        /// </summary>
        public decimal CnyPrice { get; set; }

        /// <summary>
        /// 港币总金额
        /// </summary>
        public decimal HkdPrice { get; set; }

        public string Creator { get; set; }
    }
}
