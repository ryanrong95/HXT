using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Payments.Models;
using Yahv.Underly;

namespace Wms.Services.chonggous.Views
{
    public class PayStatisticsView : QueryView<object, PvWmsRepository>
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
        /// <param name="name"></param>
        /// <param name="code"></param>
        /// <param name="currency"></param>
        /// <param name="payer"></param>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        public object GetData(string begin, string end, string name, string code , string currency, string payer, int pageindex, int pagesize)
        {
            var paymentView = new Yahv.Payments.Views.PaymentsStatisticsView().Where(item => item.Source == "香港库房");

            if (!string.IsNullOrWhiteSpace(begin))
            {
                paymentView = paymentView.Where(item => item.LeftDate >= DateTime.Parse(begin));
            }
            if (!string.IsNullOrWhiteSpace(end))
            {
                paymentView = paymentView.Where(item => item.LeftDate < DateTime.Parse(end).AddDays(1));
            }
            if (!string.IsNullOrWhiteSpace(payer))
            {
                paymentView = paymentView.Where(item => item.Payer == payer);
            }

            var payments = paymentView.ToArray();
            var companies = new Yahv.Payments.Views.CompaniesTopView().Where(item => payments.Select(c => c.Payer).Contains(item.ID)).ToArray();
            var orders = new Yahv.Payments.Views.WsOrdersTopView().Where(item => payments.Select(i => i.OrderID).Contains(item.ID)).Select(item => new
            {
                item.ID,
                item.ClientID,
            }).ToArray();
            var clients = new Yahv.Payments.Views.WsClientsTopView().Where(item => orders.Select(v => v.ClientID).Contains(item.ID)).ToArray();

            var query = from v in payments
                        join o in orders on v.OrderID equals o.ID
                        join c in clients on o.ClientID equals c.ID
                        join p in companies on v.Payer equals p.ID
                        select new
                        {
                            v.Payee,        //承运商
                            v.Payer,        //内部公司
                            v.Currency,
                            c.EnterCode,
                            v.LeftPrice,
                            v.LeftDate,
                            v.RightPrice,
                            v.RightDate,
                            o.ClientID,
                            ClientName = c.Name,        //客户
                            PayerName = p.Name,       //内部公司
                        };

            //客户名称
            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(item => item.ClientID == name);
            }

            //入仓号
            if (!string.IsNullOrWhiteSpace(code))
            {
                query = query.Where(item => item.EnterCode.ToLower().Contains(code.ToLower()));
            }

            //币种
            if (!string.IsNullOrWhiteSpace(currency))
            {
                query = query.Where(item => item.Currency == ((Currency)int.Parse(currency)));
            }

            var array = query.ToArray();
            string summary = string.Empty;

            //记账
            var records = array.Where(item => item.LeftDate != item.RightDate)
                .GroupBy(item => new { item.Currency, item.ClientID, item.ClientName, item.EnterCode, item.Payer, item.PayerName })
                .Select(item => new
                {
                    item.Key.ClientID,
                    item.Key.ClientName,
                    item.Key.Payer,
                    item.Key.PayerName,
                    item.Key.Currency,
                    item.Key.EnterCode,
                    LeftPrice = item.Sum(a => a.LeftPrice),
                    RightPrice = -1,
                    Count = item.Count(),
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
                .GroupBy(item => new { item.Currency, item.ClientID, item.ClientName, item.EnterCode, item.PayerName, item.Payer })
                .Select(item => new
                {
                    item.Key.ClientID,
                    item.Key.ClientName,
                    item.Key.Payer,
                    item.Key.PayerName,
                    item.Key.Currency,
                    item.Key.EnterCode,
                    RightPrice = item.Sum(a => a.RightPrice),
                    LeftPric = -1,
                    Count = item.Count(),
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
                            join r in cashes on new { l.ClientID, l.Currency, l.Payer } equals new { r.ClientID, r.Currency, r.Payer } into _right
                            from right in _right.DefaultIfEmpty()
                            select new WmsStats()
                            {
                                ClientID = l.ClientID,
                                ClientName = l.ClientName,
                                CurrencyName = l.Currency.GetDescription(),
                                Currency = l.Currency,
                                LeftPrice = l.LeftPrice,
                                RightPrice = right?.RightPrice,
                                EnterCode = l.EnterCode,
                                Payer = l.Payer,
                                PayerName = l.PayerName,
                                Count = l.Count,
                            };

            var rightQuery = from r in cashes
                             join l in records on new { r.ClientID, r.Currency, r.Payer } equals new { l.ClientID, l.Currency, l.Payer } into _l
                             from left in _l.DefaultIfEmpty()
                             select new WmsStats()
                             {
                                 ClientID = r.ClientID,
                                 ClientName = r.ClientName,
                                 CurrencyName = r.Currency.GetDescription(),
                                 Currency = r.Currency,
                                 LeftPrice = left?.LeftPrice,
                                 RightPrice = r?.RightPrice,
                                 EnterCode = r?.EnterCode,
                                 Payer = r.Payer,
                                 PayerName = r.PayerName,
                                 Count = r.Count,
                             };

            var result = leftQuery.Concat(rightQuery)
                .GroupBy(item => new { item.ClientID, item.ClientName, item.CurrencyName, item.Currency, item.LeftPrice, item.RightPrice, item.EnterCode, item.Payer, item.PayerName })
                .Select(item => new
                {
                    item.Key.ClientID,
                    item.Key.ClientName,
                    CurrencyName = item.Key.Currency.GetDescription(),
                    Currency = item.Key.Currency,
                    item.Key.LeftPrice,
                    item.Key.RightPrice,
                    item.Key.EnterCode,
                    item.Key.Payer,
                    item.Key.PayerName,
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

        
        public object GetDetail(string begin, string end, string clientId, string payerId, string currency)
        {
            var paymentView = new Yahv.Payments.Views.PaymentsStatisticsView().Where(item => item.Source == "香港库房");

            if (!string.IsNullOrWhiteSpace(begin))
            {
                paymentView = paymentView.Where(item => item.LeftDate >= DateTime.Parse(begin));
            }
            if (!string.IsNullOrWhiteSpace(end))
            {
                paymentView = paymentView.Where(item => item.LeftDate < DateTime.Parse(end).AddDays(1));
            }

            var payments = paymentView.Where(item => item.Payer == payerId).ToArray();
            var suppliers = new Yahv.Payments.Views.EnterprisesTopView().Where(item => payments.Select(p => p.Payee).Contains(item.ID)).ToArray();
            var orders = new Yahv.Payments.Views.WsOrdersTopView().Where(item => item.ClientID == clientId).Where(item => payments.Select(i => i.OrderID).Contains(item.ID)).ToArray();
            var clients = new Yahv.Payments.Views.WsClientsTopView().Where(item => orders.Select(v => v.ClientID).Contains(item.ID)).ToArray();

            var query = from v in payments
                        join o in orders on v.OrderID equals o.ID
                        join c in clients on o.ClientID equals c.ID
                        join s in suppliers on v.Payee equals s.ID
                        select new
                        {
                            v.Payee,
                            PayeeName = s.Name,
                            v.Currency,
                            c.EnterCode,
                            v.LeftPrice,
                            v.LeftDate,
                            v.RightPrice,
                            v.RightDate,
                            v.OrderID,
                            o.ClientID,
                            ClientName = c.Name,
                            v.Catalog,
                            v.Subject,
                        };

            //币种
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
                    ClientID = item.ClientID,
                    ClientName = item.ClientName,
                    Currency = item.Currency,
                    EnterCode = item.EnterCode,
                    LeftPrice = item.LeftPrice,
                    CreateDate = item.LeftDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    OrderID = item.OrderID,
                    Catalog = item.Catalog,
                    Subject = item.Subject,
                });

            //现金
            var cashes = query.Where(item => item.LeftDate == item.RightDate)
                .Select(item => new WmsStats
                {
                    Payee = item.Payee,
                    PayeeName = item.PayeeName,
                    ClientID = item.ClientID,
                    ClientName = item.ClientName,
                    Currency = item.Currency,
                    EnterCode = item.EnterCode,
                    RightPrice = item?.RightPrice,
                    CreateDate = item?.RightDate?.ToString("yyyy-MM-dd HH:mm:ss"),
                    OrderID = item.OrderID,
                    Catalog = item.Catalog,
                    Subject = item.Subject,
                });

            var result = records.Concat(cashes).Select(item => new
            {
                Payee = item.Payee,
                PayeeName = item.PayeeName,
                ClientID = item.ClientID,
                ClientName = item.ClientName,
                Currency = item.Currency,
                CurrencyName = item.Currency.GetDescription(),
                EnterCode = item.EnterCode,
                LeftPrice = item.LeftPrice,
                RightPrice = item?.RightPrice,
                CreateDate = item?.CreateDate,
                OrderID = item.OrderID,
                Catalog = item.Catalog,
                Subject = item.Subject,
                Begin = begin,
                End = end                
            });

            return result.OrderByDescending(item => item.OrderID).ThenByDescending(item => item.CreateDate);
            
        }
    }
}
