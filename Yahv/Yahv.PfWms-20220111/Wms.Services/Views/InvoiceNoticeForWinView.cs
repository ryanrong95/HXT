using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Wms.Services.Models;

namespace Wms.Services.Views
{
    public class InvoiceNoticeForWinView : QueryView<Models.InvoiceNoticeForWin, PvWmsRepository>
    {
        protected override IQueryable<InvoiceNoticeForWin> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.InvoiceNoticeForWin>()
                   //join invoicefile in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.InvoiceNoticeFilesView>()
                   //on entity.InvoiceNoticeID equals invoicefile.InvoiceNoticeID
                   select new InvoiceNoticeForWin
                   {
                       InvoiceNoticeID = entity.InvoiceNoticeID,
                       ClientCode = entity.ClientCode,
                       CompanyName = entity.CompanyName,
                       InvoiceType = (Enums.InvoiceType)entity.InvoiceType,
                       CreateDate = entity.CreateDate,
                       Amount = entity.Amount,
                       DeliveryType = (Enums.InvoiceDeliveryType)entity.DeliveryType,
                       InvoiceNoticeStatus = (Enums.InvoiceNoticeStatus)entity.InvoiceNoticeStatus,
                       ApplyName = entity.ApplyName,
                       //InvoiceNoticeFilesViews=invoicefile
                   };
        }
    }

    public class InvoiceFilesView : QueryView<Models.InvoiceNoticeFiles, PvWmsRepository>
    {
        protected override IQueryable<Models.InvoiceNoticeFiles> GetIQueryable()
        {
            return from invoicefile in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.InvoiceNoticeFilesView>()
                       //on entity.InvoiceNoticeID equals invoicefile.InvoiceNoticeID
                   select new InvoiceNoticeFiles
                   {
                       InvoiceNoticeFileID = invoicefile.InvoiceNoticeFileID,
                       InvoiceNoticeID = invoicefile.InvoiceNoticeID,
                       ErmAdminID = invoicefile.ErmAdminID,
                       RealName = invoicefile.RealName,
                       Name = invoicefile.Name,
                       FileType = invoicefile.FileType,
                       FileFormat = invoicefile.FileFormat,
                       Url = invoicefile.Url,
                       CreateDate = invoicefile.CreateDate
                   };
        }
    }
}
