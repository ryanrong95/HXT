using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models
{
    public class InvoiceNoticeFileRequest
    {
        public List<InvoiceNoticeFile> InvoiceNoticeFiles { get; set; }

        /// <summary>
        /// 转正常保存ServiceModel
        /// </summary>
        /// <returns></returns>
        public List<Needs.Ccs.Services.Models.InvoiceNoticeFile> ToNormalServiceModel()
        {
            var normalInvoiceNoticeFiles = this.InvoiceNoticeFiles.Select(t => new Needs.Ccs.Services.Models.InvoiceNoticeFile()
            {
                ID = Guid.NewGuid().ToString("N"),
                InvoiceNoticeID = t.InvoiceNoticeID,
                ErmAdminID = t.ErmAdminID,
                Name = t.Name,
                FileType = Needs.Ccs.Services.Enums.InvoiceNoticeFileType.Invoice,
                FileFormat = t.FileFormat,
                Url = t.Url,
                Status = Needs.Ccs.Services.Enums.Status.Normal,
                CreateDate = DateTime.Now,
                Summary = string.Empty,
            }).ToList();

            string[] ermAdminIDs = normalInvoiceNoticeFiles.Select(t => t.ErmAdminID).ToArray();

            var admins = new Needs.Ccs.Services.Views.AdminsTopView2().Where(t => ermAdminIDs.Contains(t.ID)).ToList();

            foreach (var invoiceNoticeFile in normalInvoiceNoticeFiles)
            {
                var theAdmin = admins.Where(t => t.ID == invoiceNoticeFile.ErmAdminID).FirstOrDefault();
                if (theAdmin != null)
                {
                    invoiceNoticeFile.AdminID = theAdmin.OriginID;
                }
            }

            return normalInvoiceNoticeFiles;
        }

        /// <summary>
        /// 转删除ServiceModel
        /// </summary>
        /// <returns></returns>
        public List<Needs.Ccs.Services.Models.InvoiceNoticeFile> ToDeleteServiceModel()
        {
            return this.InvoiceNoticeFiles.Select(t => new Needs.Ccs.Services.Models.InvoiceNoticeFile()
            {
                ID = t.InvoiceNoticeFileID,
            }).ToList();
        }
    }

    public class InvoiceNoticeFile
    {
        public string InvoiceNoticeFileID { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string InvoiceNoticeID { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string ErmAdminID { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string FileFormat { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string Url { get; set; } = string.Empty;
    }
}