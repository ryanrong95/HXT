using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms.Services.Models;
using Yahv.Underly;

namespace Wms.Services
{
    public class InvoiceServieces
    {
        public object GetInvoices()
        {

            

            using (var rep = new PvWmsRepository())
            {
                List<Models.InvoiceNoticeForWin> list = new List<Models.InvoiceNoticeForWin>();
                var invoiceNoticeForWins = new Views.InvoiceNoticeForWinView().ToArray();
                foreach (var item in invoiceNoticeForWins)
                {
                    var invoiceNoticeFilesViews = new Views.InvoiceFilesView().Where(tem => tem.InvoiceNoticeID == item.InvoiceNoticeID).ToArray();

                    list.Add(new Models.InvoiceNoticeForWin
                    {
                        InvoiceNoticeID = item.InvoiceNoticeID,
                        ClientCode = item.ClientCode,
                        CompanyName = item.CompanyName,
                        InvoiceType = (Enums.InvoiceType)item.InvoiceType,
                        CreateDate = item.CreateDate,
                        Amount = item.Amount,
                        DeliveryType = (Enums.InvoiceDeliveryType)item.DeliveryType,
                        InvoiceNoticeStatus = (Enums.InvoiceNoticeStatus)item.InvoiceNoticeStatus,
                        ApplyName = item.ApplyName,
                        InvoiceNoticeFilesViews = invoiceNoticeFilesViews
                    });
                }

                return list;
            }
        }

        public void InsertInvoiceNoticeFiles(InvoiceNoticeFiles[] files)
        {
            string api = string.Concat(FromType.InsertInvoiceNoticeFiles.GetDescription());
            var result = Yahv.Utils.Http.ApiHelper.Current.JPost(api, files);
        }

        public void DeleteInvoiceNoticeFiles(string[] invoiceNoticeFileIDs)
        {
            string api = string.Concat(FromType.DeleteInvoiceNoticeFiles.GetDescription());
            var result = Yahv.Utils.Http.ApiHelper.Current.JPost(api, invoiceNoticeFileIDs);
        }
    }
}
