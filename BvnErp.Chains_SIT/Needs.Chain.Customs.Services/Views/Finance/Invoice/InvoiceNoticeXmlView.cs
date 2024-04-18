using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class InvoiceNoticeXmlView : UniqueView<Models.InvoiceNoticeXml, ScCustomsReponsitory>
    {
        public InvoiceNoticeXmlView()
        {
        }

        internal InvoiceNoticeXmlView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.InvoiceNoticeXml> GetIQueryable()
        {
            var adminView = new AdminsTopView2(this.Reponsitory);

            var result = from xml in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeXmls>()
                         join admin in adminView on xml.AdminID equals admin.OriginID
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
                             UpdateDate = xml.UpdateDate
                         };

            return result;
        }
    }
}
