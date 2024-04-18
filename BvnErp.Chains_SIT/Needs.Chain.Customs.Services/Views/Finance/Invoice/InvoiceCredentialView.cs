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
    public class InvoiceCredentialView : UniqueView<Models.InvoiceNoticeXml, ScCustomsReponsitory>
    {
        public InvoiceCredentialView()
        {
        }

        internal InvoiceCredentialView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected InvoiceCredentialView(ScCustomsReponsitory reponsitory, IQueryable<Models.InvoiceNoticeXml> iQueryable) : base(reponsitory, iQueryable)
        {
        }


        protected override IQueryable<Models.InvoiceNoticeXml> GetIQueryable()
        {
            var adminView = new AdminsTopView2(this.Reponsitory);

            var result = from xml in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeXmls>()
                         join invoiceNotice in Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNotices>() on xml.InvoiceNoticeID equals invoiceNotice.ID
                         join admin in adminView on xml.AdminID equals admin.OriginID
                         where xml.Status == (int)Status.Normal
                         select new Models.InvoiceNoticeXml
                         {
                             ID = xml.ID,
                             InvoiceNoticeID = xml.InvoiceNoticeID,
                             Djh = xml.Djh,
                             Gfmc = xml.Gfmc,
                             Gfsh = xml.Gfsh,
                             Gfyhzh = xml.Gfyhzh,
                             Gfdzdh = xml.Gfdzdh,
                             Bz = xml.Bz,
                             Fhr = xml.Fhr,
                             Skr = xml.Skr,
                             Spbmbbh = xml.Spbmbbh,
                             Hsbz = xml.Hsbz,
                             FilePath = xml.FilePath,
                             InvoiceCode = xml.InvoiceCode,
                             InvoiceNo = xml.InvoiceNo,
                             InvoiceDate = xml.InvoiceDate,
                             Admin = admin,
                             Status = (Enums.Status)xml.Status,
                             CreateDate = xml.CreateDate,
                             UpdateDate = xml.UpdateDate,
                             InCreSta = xml.InCreSta,
                             InCreWord = xml.InCreWord,
                             InCreNo = xml.InCreNo,
                             InvoiceType = (Enums.InvoiceType)invoiceNotice.InvoiceType,
                         };

            return result;
        }

        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<Models.InvoiceNoticeXml> iquery = this.IQueryable.Cast<Models.InvoiceNoticeXml>().OrderByDescending(item => item.CreateDate);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            var ienums_result = iquery.ToArray();
            var invoiceNoticeXmlIDs = ienums_result.Select(t => t.ID).Distinct().ToList();

            var amountQuery = from xmlItem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeXmlItems>()
                              group xmlItem by xmlItem.InvoiceNoticeXmlID into xmlItems
                              select new
                              {
                                  xmlItems.Key,
                                  je = xmlItems.Sum(t => t.Je),
                                  se = xmlItems.Sum(t => t.Se)
                              };

            var ienums_amount = amountQuery.ToArray();

            var result = from xml in ienums_result
                         join xmlamount in ienums_amount on xml.ID equals xmlamount.Key
                         select new Models.InvoiceNoticeXmlCre
                         {
                             ID = xml.ID,
                             InvoiceNoticeID = xml.InvoiceNoticeID,
                             Djh = xml.Djh,
                             Gfmc = xml.Gfmc,
                             Gfsh = xml.Gfsh,
                             Gfyhzh = xml.Gfyhzh,
                             Gfdzdh = xml.Gfdzdh,
                             Bz = xml.Bz,
                             Fhr = xml.Fhr,
                             Skr = xml.Skr,
                             Spbmbbh = xml.Spbmbbh,
                             Hsbz = xml.Hsbz,
                             FilePath = xml.FilePath,
                             InvoiceCode = xml.InvoiceCode,
                             InvoiceNo = xml.InvoiceNo,
                             InvoiceDate = xml.InvoiceDate,
                             Admin = xml.Admin,
                             Status = (Enums.Status)xml.Status,
                             CreateDate = xml.CreateDate,
                             UpdateDate = xml.UpdateDate,
                             InCreSta = xml.InCreSta,
                             InCreWord = xml.InCreWord,
                             InCreNo = xml.InCreNo,
                             InvoiceType = (Enums.InvoiceType)xml.InvoiceType,
                             Je = xmlamount.je,
                             Se = xmlamount.se
                         };

            var results = result.ToArray();


            Func<Models.InvoiceNoticeXmlCre, object> convert = item => new
            {
                ID = item.ID,
                InvoiceNo = item.InvoiceNo,
                CompanyName = item.Gfmc,
                InvoiceDate = item.InvoiceDate == null ? "" : item.InvoiceDate.Value.ToString("yyyy-MM-dd"),
                InvoiceTypeName = item.InvoiceType.GetDescription(),
                InvoiceType = item.InvoiceType,
                item.InCreSta,
                item.InCreWord,
                item.InCreNo,
                item.Je,
                item.Se
            };

            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = results.Select(convert).ToArray(),
            };
        }

        public InvoiceCredentialView SearchByCreSta(bool creSta)
        {
            var linq = from query in this.IQueryable
                       where query.InCreSta == creSta
                       select query;

            var view = new InvoiceCredentialView(this.Reponsitory, linq);
            return view;
        }

        public InvoiceCredentialView SearchByNotNullInvoiceNo()
        {
            var linq = from query in this.IQueryable
                       where query.InvoiceNo != null
                       select query;

            var view = new InvoiceCredentialView(this.Reponsitory, linq);
            return view;
        }

      


        public InvoiceCredentialView SearchByinvoiceNo(string invoiceNo)
        {
            var linq = from query in this.IQueryable
                       where query.InvoiceNo == invoiceNo
                       select query;

            var view = new InvoiceCredentialView(this.Reponsitory, linq);
            return view;
        }

        public InvoiceCredentialView SearchBycomanyName(string comanyName)
        {
            var linq = from query in this.IQueryable
                       where query.Gfmc.Contains(comanyName)
                       select query;

            var view = new InvoiceCredentialView(this.Reponsitory, linq);
            return view;
        }

        public InvoiceCredentialView SearchByFrom(DateTime fromtime)
        {
            var linq = from query in this.IQueryable
                       where query.InvoiceDate >= fromtime
                       select query;

            var view = new InvoiceCredentialView(this.Reponsitory, linq);
            return view;
        }

        public InvoiceCredentialView SearchByTo(DateTime fromtime)
        {
            var linq = from query in this.IQueryable
                       where query.InvoiceDate < fromtime
                       select query;

            var view = new InvoiceCredentialView(this.Reponsitory, linq);
            return view;
        }

    }
}
