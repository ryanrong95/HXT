using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Payments.Models;
using Yahv.Payments.Views;
using Yahv.Underly;

namespace Wms.Services.chonggous.Views
{
    /// <summary>
    /// 香港库房收入统计视图
    /// </summary>
    public class IncomeStatisticsView : QueryView<object, PvWmsRepository>
    {
        protected override IQueryable<object> GetIQueryable()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="payee"></param>
        /// <param name="name"></param>
        /// <param name="code"></param>
        /// <param name="currency"></param>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        public object GetData(string begin, string end, string payee, string name, string code, string currency, int pageindex, int pagesize)
        {
            var vorchersView = new Yahv.Payments.Views.VouchersStatisticsView().Where(item => item.Source == "香港库房");
            
            if (!string.IsNullOrWhiteSpace(begin))
            {
                vorchersView = vorchersView.Where(item => item.LeftDate >= DateTime.Parse(begin));
            }
            if (!string.IsNullOrWhiteSpace(end))
            {
                vorchersView = vorchersView.Where(item => item.LeftDate < DateTime.Parse(end).AddDays(1));
            }
            if (!string.IsNullOrWhiteSpace(payee))
            {
                vorchersView = vorchersView.Where(item => item.Payee == payee);
            }

            var vouchers = vorchersView.ToArray();
            var companies = new Yahv.Payments.Views.CompaniesTopView().Where(item => vouchers.Select(c => c.Payee).Contains(item.ID)).ToArray();
            var orders = new Yahv.Payments.Views.WsOrdersTopView().Where(item => vouchers.Select(i => i.OrderID).Contains(item.ID)).Select(item => new
            {
                item.ID,
                item.ClientID,
            }).ToArray();

            var clients = new Yahv.Payments.Views.WsClientsTopView().Where(item => orders.Select(v => v.ClientID).Contains(item.ID)).ToArray();
            var query = from v in vouchers
                        join o in orders on v.OrderID equals o.ID
                        join c in clients on o.ClientID equals c.ID
                        join p in companies on v.Payee equals p.ID
                        select new
                        {
                            v.Payee,        //收款公司
                            v.Payer,        //客户 或者 承运商
                            PayeeName = p.Name,
                            v.Currency,
                            c.EnterCode,
                            v.LeftPrice,
                            v.LeftDate,
                            v.RightPrice,
                            v.RightDate,
                            o.ClientID,
                            ClientName = c.Name
                        };

            // 客户名称
            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(item => item.ClientID == name);
            }

            // 入仓号
            if (!string.IsNullOrWhiteSpace(code))
            {
                query = query.Where(item => item.EnterCode.ToLower().Contains(code.ToLower()));
            }

            // 币种
            if (!string.IsNullOrWhiteSpace(currency))
            {
                query = query.Where(item => item.Currency == ((Currency)int.Parse(currency)));
            }

            var array = query.ToArray();
            string summary = string.Empty;

            // 记账
            var records = array.Where(item => item.LeftDate != item.RightDate)
                .GroupBy(item => new { item.Currency, item.ClientID, item.ClientName, item.EnterCode, item.Payee, item.PayeeName })
                .Select(item => new
                {
                    item.Key.Payee,
                    item.Key.PayeeName,
                    item.Key.ClientID,
                    item.Key.ClientName,
                    item.Key.Currency,
                    item.Key.EnterCode,
                    //item.Key.Payer,
                    //item.Key.PayerName,
                    LeftPrice = item.Sum(a => a.LeftPrice),
                    Count = item.Count()
                }).ToArray();

            if (records.Length > 0)
            {
                summary += " 记账[";
                foreach (var r in records.GroupBy(item => item.Currency))
                {
                    summary += decimal.Parse(r.Sum(item => item.LeftPrice).ToString()).Round() + r.Key.GetDescription() + "、";
                }
                summary = summary.TrimEnd('、') + "]; ";
            }

            //现金
            var cashes = array.Where(item => item.LeftDate == item.RightDate)
                .GroupBy(item => new { item.Currency, item.Payee, item.ClientID, item.PayeeName, item.ClientName, item.EnterCode })
                .Select(item => new
                {
                    item.Key.Payee,
                    item.Key.PayeeName,
                    item.Key.ClientID,
                    item.Key.ClientName,
                    item.Key.Currency,
                    item.Key.EnterCode,
                    //item.Key.Payer,
                    //item.Key.PayerName,
                    RightPrice = item.Sum(a => a.RightPrice),
                    Count = item.Count()
                }).ToArray();

            if (cashes.Length > 0)
            {
                summary += "  现金[";
                foreach (var r in cashes.GroupBy(item => item.Currency))
                {
                    summary += decimal.Parse(r.Sum(item => item.RightPrice).ToString()).Round() + r.Key.GetDescription() + "、";
                }
                summary = summary.TrimEnd('、') + "]";
            }

            var leftQuery = from l in records
                            join r in cashes on new { l.ClientID, l.Currency, l.Payee } equals new { r.ClientID, r.Currency, r.Payee } into _right
                            from right in _right.DefaultIfEmpty()
                            select new WmsStats()
                            {
                                Payee = l.Payee,
                                PayeeName = l.PayeeName,
                                CurrencyName = l.Currency.GetDescription(),
                                Currency = l.Currency,
                                LeftPrice = l.LeftPrice,
                                RightPrice = right?.RightPrice,
                                EnterCode = l.EnterCode,
                                ClientID = l.ClientID,
                                ClientName = l.ClientName,
                                Count = l.Count,
                            };

            var rightQuery = from r in cashes
                             join l in records on new { r.ClientID, r.Currency, r.Payee } equals new { l.ClientID, l.Currency, l.Payee } into _left
                             from left in _left.DefaultIfEmpty()
                             select new WmsStats()
                             {
                                 Payee = r.Payee,
                                 PayeeName = r.PayeeName,
                                 CurrencyName = r.Currency.GetDescription(),
                                 Currency = r.Currency,
                                 RightPrice = r?.RightPrice,
                                 LeftPrice = left?.LeftPrice,
                                 EnterCode = r.EnterCode,
                                 ClientID = r.ClientID,
                                 ClientName = r.ClientName,
                                 Count = r.Count,
                             };

            var result = leftQuery.Concat(rightQuery)
                .GroupBy(item => new { item.Currency, item.LeftPrice, item.RightPrice, item.EnterCode, item.ClientID, item.ClientName, item.Payee, item.PayeeName, item.Payer, item.PayerName })
                .Select(item => new
                {
                    item.Key.Payee,
                    item.Key.PayeeName,
                    item.Key.Payer,
                    item.Key.PayerName,
                    item.Key.ClientID,
                    item.Key.ClientName,
                    CurrencyName = item.Key.Currency.GetDescription(),
                    Currency = item.Key.Currency,
                    item.Key.LeftPrice,
                    item.Key.RightPrice,
                    item.Key.EnterCode,
                    summary,
                    Count = item.Sum(i => i.Count),
                    Begin = begin,
                    End = end,
                });

            int total = result.Count();

            var resultNew = result.Skip(pagesize * (pageindex - 1)).Take(pagesize);
            return new
            {
                Total = total,
                Size = pagesize,
                Index = pageindex,
                Data = resultNew.ToArray(),
            };

        }

        /// <summary>
        /// 获取详情数据
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="clientId"></param>
        /// <param name="payeeId"></param>
        /// <param name="currency"></param>
        /// <returns></returns>
        public object GetDetail(string begin, string end, string clientId, string payeeId, string currency)
        {
            var vorchersView = new Yahv.Payments.Views.VouchersStatisticsView().Where(item => item.Source == "香港库房");

            if (!string.IsNullOrWhiteSpace(begin))
            {
                vorchersView = vorchersView.Where(item => item.LeftDate >= DateTime.Parse(begin));
            }

            if (!string.IsNullOrWhiteSpace(end))
            {
                vorchersView = vorchersView.Where(item => item.LeftDate < DateTime.Parse(end).AddDays(1));
            }

            var vouchers = vorchersView.Where(item => item.Payee == payeeId).ToArray();
            var companies = new Yahv.Payments.Views.CompaniesTopView().Where(item => vouchers.Select(c => c.Payee).Contains(item.ID)).ToArray();
            var orders = new Yahv.Payments.Views.WsOrdersTopView().Where(item => item.ClientID == clientId)
                .Where(item => vouchers.Select(i => i.OrderID).Contains(item.ID)).ToArray();

            var clients = new Yahv.Payments.Views.WsClientsTopView().Where(item => orders.Select(v => v.ClientID).Contains(item.ID)).ToArray();
            var enterprises = new Yahv.Payments.Views.EnterprisesTopView().Where(item => vouchers.Select(c => c.Payer).Contains(item.ID)).ToArray();

            var query = from v in vouchers
                        join o in orders on v.OrderID equals o.ID
                        join c in clients on o.ClientID equals c.ID
                        join p in companies on v.Payee equals p.ID
                        join e in enterprises on v.Payer equals e.ID
                        select new
                        {
                            v.Payee,        //收款公司
                            v.Payer,        //客户 或者 承运商
                            PayeeName = p.Name,
                            PayerName = e.Name,
                            v.Currency,
                            c.EnterCode,
                            v.LeftPrice,
                            v.LeftDate,
                            v.RightPrice,
                            v.RightDate,
                            v.OrderID,
                            v.Catalog,
                            v.Subject,
                            o.ClientID,
                            ClientName = c.Name
                        };

            if (!string.IsNullOrWhiteSpace(currency))
            {
                query = query.Where(item => item.Currency == ((Currency)int.Parse(currency)));
            }

            //记账
            var records = query.Where(item => item.LeftDate != item.RightDate)
                .Select(item => new WmsStats
                {
                    Payee = item.Payee,
                    PayeeName = item.PayeeName,
                    Payer = item.Payer,
                    PayerName = item.PayerName,
                    Currency = item.Currency,
                    EnterCode = item.EnterCode,
                    LeftPrice = item.LeftPrice,
                    CreateDate = item.LeftDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    OrderID = item.OrderID,
                    Catalog = item.Catalog,
                    Subject = item.Subject,
                    ClientID = item.ClientID,
                    ClientName = item.ClientName,
                });

            //现金
            var cashes = query.Where(item => item.LeftDate == item.RightDate)
                .Select(item => new WmsStats
                {
                    Payee = item.Payee,
                    PayeeName = item.PayeeName,
                    Payer = item.Payer,
                    PayerName = item.PayerName,
                    Currency = item.Currency,
                    EnterCode = item.EnterCode,
                    RightPrice = item?.RightPrice,
                    CreateDate = item?.RightDate?.ToString("yyyy-MM-dd HH:mm:ss"),
                    OrderID = item.OrderID,
                    Catalog = item.Catalog,
                    Subject = item.Subject,
                    ClientID = item.ClientID,
                    ClientName = item.ClientName,
                });

            var result = records.Concat(cashes).Select(item => new
            {
                Payee = item.Payee,
                PayeeName = item.PayeeName,
                Payer = item.Payer,
                PayerName = item.PayerName,
                Currency = item.Currency,
                CurrencyName = item.Currency.GetDescription(),
                EnterCode = item.EnterCode,
                LeftPrice = item.LeftPrice,
                RightPrice = item?.RightPrice,
                CreateDate = item?.CreateDate,
                OrderID = item.OrderID,
                Catalog = item.Catalog,
                Subject = item.Subject,
                ClientID = item.ClientID,
                ClientName = item.ClientName,
                Begin = begin,
                End = end,            
            });

            return result.OrderByDescending(item => item.OrderID).ThenByDescending(item => item.CreateDate);
        }

        /// <summary>
        /// 获取客户
        /// </summary>
        /// <returns></returns>
        public object GetClients()
        {
            var wsClientsView = new Yahv.Payments.Views.WsClientsTopView();
            var linq = wsClientsView.Select(item => new
            {
                text = item.Name,
                value = item.ID
            });
            return linq;
        }

        /// <summary>
        /// 获取币种
        /// </summary>
        /// <returns></returns>
        public object GetCurrencies()
        {
            return ExtendsEnum.ToDictionary<Currency>().Select(item => new { text = item.Value, value = item.Key });
        }

        /// <summary>
        /// 获取内部公司
        /// </summary>
        /// <returns></returns>
        public object GetCompanies()
        {
            var companiesView = new Yahv.Payments.Views.CompaniesTopView();
            var linq = companiesView.Select(item => new
            {
                text = item.Name,
                value = item.ID,
            });
            return linq;
        }

    }
}
