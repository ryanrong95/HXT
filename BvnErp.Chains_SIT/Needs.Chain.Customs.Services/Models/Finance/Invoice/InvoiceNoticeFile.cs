using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 开票通知文件
    /// </summary>
    public class InvoiceNoticeFile : IUnique, IPersistence, IFulError, IFulSuccess
    {
        public string ID { get; set; } = string.Empty;

        public string InvoiceNoticeID { get; set; } = string.Empty;

        public string AdminID { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public Enums.InvoiceNoticeFileType FileType { get; set; }

        public string FileFormat { get; set; } = string.Empty;

        public string Url { get; set; } = string.Empty;

        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public string Summary { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string ErmAdminID { get; set; } = string.Empty;

        public event ErrorHanlder EnterError;
        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event SuccessHanlder EnterSuccess;

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeFiles>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    reponsitory.Insert<Layer.Data.Sqls.ScCustoms.InvoiceNoticeFiles>(new Layer.Data.Sqls.ScCustoms.InvoiceNoticeFiles()
                    {
                        ID = this.ID,
                        InvoiceNoticeID = this.InvoiceNoticeID,
                        AdminID = this.AdminID,
                        Name = this.Name,
                        FileType = (int)this.FileType,
                        FileFormat = this.FileFormat,
                        Url = this.Url,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        Summary = this.Summary,
                    });
                }
                else
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.InvoiceNoticeFiles>(new
                    {
                        InvoiceNoticeID = this.InvoiceNoticeID,
                        AdminID = this.AdminID,
                        Name = this.Name,
                        FileType = (int)this.FileType,
                        FileFormat = this.FileFormat,
                        Url = this.Url,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        Summary = this.Summary,
                    }, item => item.ID == this.ID);
                }
            }

            this.OnEnterSuccess();
        }

        virtual public void OnEnterSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        public void Abandon()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.InvoiceNoticeFiles>(new { Status = Enums.Status.Delete }, item => item.ID == this.ID);
            }

            this.OnAbandonSuccess();
        }

        virtual protected void OnAbandonSuccess()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                //成功后触发事件
                this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

    }
}
