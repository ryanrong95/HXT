using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 开票申请的视图
    /// </summary>
    public class InvoiceNoticeView : UniqueView<Models.InvoiceNotice, ScCustomsReponsitory>
    {
        public InvoiceNoticeView()
        {
        }

        internal InvoiceNoticeView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<InvoiceNotice> GetIQueryable()
        {
            var adminView = new AdminsTopView(this.Reponsitory);
            var clientView = new ClientsView(this.Reponsitory);
            var clientInvoiceView = new ClientInvoicesView(this.Reponsitory);
            var clientAgreementView = new ClientAgreementsView(this.Reponsitory).Where(item => item.Status == Enums.Status.Normal);
            var clientConsigneeView = new ClientInvoiceConsigneesView(this.Reponsitory);
            var NoticeItemView = new InvoiceNoticeItemView(this.Reponsitory);
            var WaybillView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceWaybills>();
            var invoiceNoticeFiles = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeFiles>();

            var result = from invoiceNotice in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNotices>()
                         join apply in adminView on invoiceNotice.ApplyID equals apply.ID
                         join admin in adminView on invoiceNotice.AdminID equals admin.ID into admins
                         from admin in admins.DefaultIfEmpty()
                         join client in clientView on invoiceNotice.ClientID equals client.ID
                         join clientInvoice in clientInvoiceView on invoiceNotice.ClientID equals clientInvoice.ClientID
                         join clientAgreement in clientAgreementView on invoiceNotice.ClientID equals clientAgreement.ClientID
                         join clientConsignee in clientConsigneeView on invoiceNotice.ClientID equals clientConsignee.ClientID
                         join item in NoticeItemView on invoiceNotice.ID equals item.InvoiceNoticeID into items
                         join waybill in WaybillView on invoiceNotice.ID equals waybill.InvoiceNoticeID into waybills
                         from waybill in waybills.DefaultIfEmpty()

                         join invoiceNoticeFile in invoiceNoticeFiles 
                                on new { InvoiceNoticeID = invoiceNotice.ID, InvoiceNoticeFileDataStatus = (int)Enums.Status.Normal, }
                                equals new { InvoiceNoticeID = invoiceNoticeFile.InvoiceNoticeID, InvoiceNoticeFileDataStatus = invoiceNoticeFile.Status, }
                                into invoiceNoticeFiles2

                         where invoiceNotice.Status != (int)Enums.InvoiceNoticeStatus.Canceled
                         orderby invoiceNotice.CreateDate descending
                         select new Models.InvoiceNotice
                         {
                             ID = invoiceNotice.ID,
                             Apply = apply,
                             Admin = admin,
                             Client = client,
                             ClientInvoice = clientInvoice,
                             InvoiceType = (Enums.InvoiceType)invoiceNotice.InvoiceType,
                             InvoiceTaxRate = invoiceNotice.InvoiceTaxRate,
                             Address = clientInvoice.Address,
                             Tel = clientInvoice.Tel,
                             BankName = clientInvoice.BankName,
                             BankAccount = clientInvoice.BankAccount,
                             DeliveryType = clientInvoice.DeliveryType,
                             MailName = invoiceNotice.MailName,
                             MailMobile = invoiceNotice.MailMobile,
                             MailAddress = invoiceNotice.MailAddress,
                             InvoiceNoticeStatus = (Enums.InvoiceNoticeStatus)invoiceNotice.Status,
                             CreateDate = invoiceNotice.CreateDate,
                             UpdateDate = invoiceNotice.UpdateDate,
                             Summary = invoiceNotice.Summary,
                             Amount = items.Sum(t => t.Amount),
                             Difference = items.Sum(t => t.Difference),

                             WaybillCode = waybill == null ? "" : waybill.WaybillCode,
                             InvoiceNoticeFileCount = invoiceNoticeFiles2 != null ? invoiceNoticeFiles2.Count() : 0,
                             AmountLimit = invoiceNotice.AmountLimit,
                         };
            return result;
        }
    }

    public class InvoiceNotices1View : Needs.Linq.Generic.Query1Classics<Models.InvoiceNotice, ScCustomsReponsitory>
    {
        public InvoiceNotices1View()
        {
        }

        internal InvoiceNotices1View(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.InvoiceNotice> GetIQueryable(Expression<Func<InvoiceNotice, bool>> expression, params LambdaExpression[] expressions)
        {
            var clientsView = new ClientsView(this.Reponsitory);

            var linq = from invoiceNotice in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNotices>()
                       join client in clientsView on invoiceNotice.ClientID equals client.ID
                       where invoiceNotice.Status != (int)Enums.InvoiceNoticeStatus.Canceled
                       orderby invoiceNotice.CreateDate descending
                       select new Models.InvoiceNotice
                       {
                           ID = invoiceNotice.ID,
                           ApplyID = invoiceNotice.ApplyID,
                           AdminID = invoiceNotice.AdminID,
                           ClientID = invoiceNotice.ClientID,
                           Client = client,
                           ClientInvoiceID = invoiceNotice.ClientInvoiceID,
                           InvoiceType = (Enums.InvoiceType)invoiceNotice.InvoiceType,
                           InvoiceTaxRate = invoiceNotice.InvoiceTaxRate,
                           InvoiceNoticeStatus = (Enums.InvoiceNoticeStatus)invoiceNotice.Status,
                           CreateDate = invoiceNotice.CreateDate,
                           UpdateDate = invoiceNotice.UpdateDate,
                           Summary = invoiceNotice.Summary,
                           AmountLimit =  invoiceNotice.AmountLimit,
                       };

            foreach (var predicate in expressions)
            {
                linq = linq.Where(predicate as Expression<Func<InvoiceNotice, bool>>);
            }

            return linq.Where(expression);
        }

        protected override IEnumerable<Models.InvoiceNotice> OnReadShips(Models.InvoiceNotice[] results)
        {
            var adminsView = new AdminsTopView(this.Reponsitory);
            var clientInvoicesView = new ClientInvoicesView(this.Reponsitory);
            var clientConsigneesView = new ClientInvoiceConsigneesView(this.Reponsitory);

            var ids = results.Select(a => a.ID).ToArray();
            var noticeItemsView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeItems>().Where(item => ids.Contains(item.InvoiceNoticeID)).ToArray();
            var waybillsView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceWaybills>().Where(item => ids.Contains(item.InvoiceNoticeID)).ToArray();

            return from invoiceNotice in results
                   join apply in adminsView on invoiceNotice.ApplyID equals apply.ID
                   join admin in adminsView on invoiceNotice.AdminID equals admin.ID into admins
                   from admin in admins.DefaultIfEmpty()
                   join clientInvoice in clientInvoicesView on invoiceNotice.ClientID equals clientInvoice.ClientID
                   join clientConsignee in clientConsigneesView on invoiceNotice.ClientID equals clientConsignee.ClientID
                   join item in noticeItemsView on invoiceNotice.ID equals item.InvoiceNoticeID into items
                   join waybill in waybillsView on invoiceNotice.ID equals waybill.InvoiceNoticeID into waybills
                   from waybill in waybills.DefaultIfEmpty()
                   select new Models.InvoiceNotice
                   {
                       ID = invoiceNotice.ID,
                       ApplyID = invoiceNotice.ApplyID,
                       Apply = apply,
                       AdminID = invoiceNotice.AdminID,
                       Admin = admin,
                       ClientID = invoiceNotice.ClientID,
                       Client = invoiceNotice.Client,
                       ClientInvoiceID = invoiceNotice.ClientInvoiceID,
                       ClientInvoice = clientInvoice,
                       InvoiceType = invoiceNotice.InvoiceType,
                       InvoiceTaxRate = invoiceNotice.InvoiceTaxRate,
                       Address = clientInvoice.Address,
                       Tel = clientInvoice.Tel,
                       BankName = clientInvoice.BankName,
                       BankAccount = clientInvoice.BankAccount,
                       DeliveryType = clientInvoice.DeliveryType,
                       MailName = clientConsignee.Name,
                       MailMobile = clientConsignee.Mobile,
                       MailAddress = clientConsignee.Address,
                       InvoiceNoticeStatus = invoiceNotice.InvoiceNoticeStatus,
                       CreateDate = invoiceNotice.CreateDate,
                       UpdateDate = invoiceNotice.UpdateDate,
                       Summary = invoiceNotice.Summary,
                       Amount = items.Sum(t => t.Amount),
                       Difference = items.Sum(t => t.Difference),
                       InvoiceNoSummary = string.Join(",",items.Select(t=>t.InvoiceNo).Distinct().ToArray()),

                       WaybillCode = waybill == null ? "" : waybill.WaybillCode,
                       AmountLimit = invoiceNotice.AmountLimit,
                   };
        }
    }

    public class InvoiceNoticeViewRJ : UniqueView<Models.InvoiceNotice, ScCustomsReponsitory>
    {
        public string NoticeID;

        public InvoiceNoticeViewRJ()
        {
        }

        public InvoiceNoticeViewRJ(string NoticeID)
        {
            this.NoticeID = NoticeID;
        }

        internal InvoiceNoticeViewRJ(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<InvoiceNotice> GetIQueryable()
        {
            return null;
        }

        public InvoiceNotice GetInvoice()
        {
            var invoice = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNotices>().FirstOrDefault(t => t.ID == this.NoticeID && t.Status != (int)Enums.InvoiceNoticeStatus.Canceled);

            var adminView = new AdminsTopView(this.Reponsitory);


            var client = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>().FirstOrDefault(t => t.ID == invoice.ClientID && t.Status == (int)Enums.Status.Normal);

            var compamy = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>().FirstOrDefault(t => t.ID == client.CompanyID && t.Status == (int)Enums.Status.Normal);

            var clientinvoice = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientInvoices>().FirstOrDefault(t => t.ClientID == invoice.ClientID && t.Status == (int)Enums.Status.Normal);

            var cientinvoiceconsignee = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientInvoiceConsignees>().FirstOrDefault(t => t.ClientID == invoice.ClientID && t.Status == (int)Enums.Status.Normal);

            var waybill = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceWaybills>().FirstOrDefault(t => t.InvoiceNoticeID == invoice.ID);

            var noticeitems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeItems>().Where(t => t.InvoiceNoticeID == invoice.ID && t.Status == (int)Enums.Status.Normal).ToArray();

            var invoicefiles = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeFiles>().Where(t => t.InvoiceNoticeID == invoice.ID && t.Status == (int)Enums.Status.Normal).ToArray();

            return new Models.InvoiceNotice
            {
                ID = invoice.ID,
                //Apply = apply,
                //Admin = admin,
                Client = new Client {
                    ID = client.ID,              
                    Company = new Company {Name = compamy.Name },
                    //Invoice = new ClientInvoice { TaxCode = clientinvoice.TaxCode}
                },
                ClientInvoice = new ClientInvoice { TaxCode = clientinvoice.TaxCode},
                InvoiceType = (Enums.InvoiceType)invoice.InvoiceType,
                InvoiceTaxRate = invoice.InvoiceTaxRate,
                Address = clientinvoice.Address,
                Tel = clientinvoice.Tel,
                BankName = clientinvoice.BankName,
                BankAccount = clientinvoice.BankAccount,
                DeliveryType = (Enums.InvoiceDeliveryType)clientinvoice.DeliveryType,
                MailName = cientinvoiceconsignee.Name,
                MailMobile = cientinvoiceconsignee.Mobile,
                MailAddress = cientinvoiceconsignee.Address,
                InvoiceNoticeStatus = (Enums.InvoiceNoticeStatus)invoice.Status,
                CreateDate = invoice.CreateDate,
                UpdateDate = invoice.UpdateDate,
                Summary = invoice.Summary,
                Amount = noticeitems.Sum(t => t.Amount),
                Difference = noticeitems.Sum(t => t.Difference),

                WaybillCode = waybill == null ? "" : waybill.WaybillCode,
                InvoiceNoticeFileCount = invoicefiles != null ? invoicefiles.Count() : 0,
                AmountLimit = invoice.AmountLimit,

            };
        }


        public List<InVItemJoinDeclist> GetVItemJoinDeclist()
        {
            var xmlViews = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeXmlItems>();
            var noticeViews = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeItems>();
            var listViews = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>();

            var linq = from xml in xmlViews
                       join notice in noticeViews on xml.InvoiceNoticeItemID equals notice.ID into notice_temp
                       from notice in notice_temp.DefaultIfEmpty()
                       join list in listViews on notice.OrderItemID equals list.OrderItemID into list_temp
                       from list in list_temp.DefaultIfEmpty()
                       where notice.InvoiceNoticeID == this.NoticeID
                       select new InVItemJoinDeclist
                       {
                           InvoiceNoticeItemID = notice.ID,
                           DeclistItemID = list == null ? "" : list.DecListID,
                           OrderItemID = list == null ? "" : list.OrderItemID,
                           Quantity = list == null ? 0M : list.GQty
                       };

            return linq.ToList();
        }

        public List<VItemJoinDecList4Allocate> GetVItemJoinDeclist4Allocate()
        {
            var xmlViews = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeXmlItems>();
            var noticeViews = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeItems>();
            var listViews = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>();

            var linq = from xml in xmlViews
                       join notice in noticeViews on xml.InvoiceNoticeItemID equals notice.ID into notice_temp
                       from notice in notice_temp.DefaultIfEmpty()
                       join list in listViews on notice.OrderItemID equals list.OrderItemID into list_temp
                       from list in list_temp.DefaultIfEmpty()
                       where notice.InvoiceNoticeID == this.NoticeID
                       select new VItemJoinDecList4Allocate
                       {
                           InvoiceNoticeXmlItemID = xml.ID,
                           DeclistItemID = list == null ? "" : list.DecListID,                          
                           Quantity = list == null ? 0M : list.GQty
                       };

            return linq.ToList();
        }
    }
}
