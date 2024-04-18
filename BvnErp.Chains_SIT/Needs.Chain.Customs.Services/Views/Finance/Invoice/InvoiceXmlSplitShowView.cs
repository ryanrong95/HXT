using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class InvoiceXmlSplitShowView : QueryView<InvoiceXmlVo, ScCustomsReponsitory>
    {
        public InvoiceXmlSplitShowView()
        {
        }

        internal InvoiceXmlSplitShowView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected InvoiceXmlSplitShowView(ScCustomsReponsitory reponsitory, IQueryable<InvoiceXmlVo> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<InvoiceXmlVo> GetIQueryable()
        {           
            var iQuery = from notice in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeXmls>()                        
                         where notice.Status == (int)Enums.Status.Normal 
                         select new InvoiceXmlVo
                         {
                             ID = notice.ID,
                             InvoiceNoticeID = notice.InvoiceNoticeID,
                             CreateDate = notice.CreateDate
                         };
            return iQuery;
        }

        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<Models.InvoiceXmlVo> iquery = this.IQueryable.Cast<Models.InvoiceXmlVo>().OrderBy(item => item.CreateDate);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            var ienum_Xmls = iquery.ToArray();

            var InvoiceNoticeIds = ienum_Xmls.Select(t => t.InvoiceNoticeID).Distinct().ToList();
            var XmlIds = ienum_Xmls.Select(t => t.ID).Distinct().ToList();

            var iquery_InvoiceNotice = from notice in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNotices>()
                                       join noticeItem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeItems>() on notice.ID equals noticeItem.InvoiceNoticeID
                                       where InvoiceNoticeIds.Contains(notice.ID)
                                       group new { notice.ID, notice.InvoiceTaxRate, notice.InvoiceType, noticeItem.Amount, noticeItem.Difference } by
                                       new { notice.ID, notice.InvoiceTaxRate, notice.InvoiceType } into g
                                       select new
                                       {
                                           ID = g.Key.ID,
                                           InvoiceTaxRate = g.Key.InvoiceTaxRate,
                                           InvoiceType = g.Key.InvoiceType,
                                           Amount = g.Sum(t=>t.Amount),
                                           Difference = g.Sum(t=>t.Difference)
                                       };
            var ienum_InvoiceNotice = iquery_InvoiceNotice.ToArray();

            var iquery_Xml = from xml in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeXmls>()
                             join xmlItem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeXmlItems>() on xml.ID equals xmlItem.InvoiceNoticeXmlID
                             where XmlIds.Contains(xml.ID)
                             group new { xml.ID, xmlItem.Je, xmlItem.Se }
                             by new { xml.ID,xml.InvoiceNoticeID } into g
                             select new
                             {
                                 ID = g.Key.ID,
                                 InvoiceNoticeID = g.Key.InvoiceNoticeID,
                                 Amount = g.Sum(t => t.Je),
                                 Tax = g.Sum(t => t.Se)
                             };

            var ienum_Xml = iquery_Xml.ToArray();

            var temp_result = from xml in ienum_Xml
                              join invoice in ienum_InvoiceNotice on xml.InvoiceNoticeID equals invoice.ID
                              orderby invoice.ID
                              select new InvoiceXmlVo
                              {
                                  InvoiceNoticeID = invoice.ID,
                                  InvoiceNoticeAmount = invoice.Amount,
                                  InvoiceNoticeDiff = invoice.Difference,
                                  InvoiceType = invoice.InvoiceType,
                                  TaxRate = invoice.InvoiceTaxRate,
                                  ID = xml.ID,
                                  XmlAmount = xml.Amount,
                                  XmlTax = xml.Tax
                              };

            var ienum_temp_result = temp_result.ToArray();

            var res = ienum_temp_result.Select(
                        item => new
                        {
                            InvoiceNoticeID = item.InvoiceNoticeID,
                            InvoiceNoticeAmount = item.InvoiceNoticeAmount,
                            InvoiceNoticeDiff = item.InvoiceNoticeDiff,
                            InvoicePrice = item.InvoicePrice,
                            InvoiceType = item.InvoiceType,
                            TaxRate = item.TaxRate,
                            ID = item.ID,
                            XmlAmount = item.XmlAmount,
                            XmlTax = item.XmlTax,
                            XmlPrice = item.XmlPrice
                        }
                     ).ToArray();


            if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                return ienum_temp_result.Select(item =>
                {
                    object o = item;
                    return o;
                }).ToArray();
            }

            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = res.ToArray(),
            };
        }

        public InvoiceXmlSplitShowView SearchByInvoiceNoticeID(string invoiceNoticeID)
        {
            var linq = from query in this.IQueryable
                       where query.InvoiceNoticeID == invoiceNoticeID
                       select query;

            var view = new InvoiceXmlSplitShowView(this.Reponsitory, linq);
            return view;
        }

        public InvoiceXmlSplitShowView SearchByInvoiceNoticeIDs(string[] invoiceNoticeIDs)
        {
            var linq = from query in this.IQueryable
                       where invoiceNoticeIDs.Contains(query.InvoiceNoticeID)
                       select query;

            var view = new InvoiceXmlSplitShowView(this.Reponsitory, linq);
            return view;
        }

        public InvoiceXmlSplitShowView SearchByInvoiceNoticeIDs(List<string> invoiceNoticeIDs)
        {
            var linq = from query in this.IQueryable
                       where invoiceNoticeIDs.Contains(query.InvoiceNoticeID)
                       select query;

            var view = new InvoiceXmlSplitShowView(this.Reponsitory, linq);
            return view;
        }
    }
}
