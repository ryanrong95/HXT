using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class TaxManage : IUnique, IPersistence
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 发票代码
        /// </summary>
        public string InvoiceCode { get; set; }

        /// <summary>
        /// 发票号码
        /// </summary>
        public string InvoiceNo { get; set; }

        /// <summary>
        /// 开票日期
        /// </summary>
        public DateTime? InvoiceDate { get; set; }

        /// <summary>
        /// 销方名称
        /// </summary>
        public string SellsName { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 有效税额
        /// </summary>
        public decimal? VaildAmount { get; set; }

        /// <summary>
        /// 确认日期
        /// </summary>
        public DateTime? ConfrimDate { get; set; }

        /// <summary>
        /// 认证月份
        /// </summary>
        public DateTime? AuthenticationMonth { get; set; }

        /// <summary>
        /// 发票是否有效
        /// </summary>
        public Enums.InvoiceVaildStatus IsVaild { get; set; }

        /// <summary>
        /// 发票详情ID
        /// </summary>
        public string InvoiceDetailID { get; set; }

        /// <summary>
        /// 发票类型：增值税，全额，服务费
        /// </summary>
        public Enums.InvoiceType InvoiceType { get; set; }

        /// <summary>
        /// 业务类型：代报关，代仓储
        /// </summary>
        public Enums.BusinessType BusinessType { get; set; }

        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.TaxManage>().Count(item => item.ID == this.ID);
                if (count == 0)
                {
                    reponsitory.Insert<Layer.Data.Sqls.ScCustoms.TaxManage>(new Layer.Data.Sqls.ScCustoms.TaxManage
                    {
                        ID = this.ID,
                        InvoiceCode = this.InvoiceCode,
                        InvoiceNo = this.InvoiceNo,
                        InvoiceDate = this.InvoiceDate,
                        SellsName = this.SellsName,
                        Amount = this.Amount,
                        VaildAmount = this.VaildAmount,
                        ConfrimDate = this.ConfrimDate,
                        AuthenticationMonth = this.AuthenticationMonth,
                        IsVaild = (int)this.IsVaild,
                        InvoiceDetailID = this.InvoiceDetailID,
                        InvoiceType = (int)this.InvoiceType,
                        BusinessType = (int)this.BusinessType,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary,
                    });
                }
                else
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.TaxManage>(new
                    {
                        InvoiceCode = this.InvoiceCode,
                        InvoiceNo = this.InvoiceNo,
                        InvoiceDate = this.InvoiceDate,
                        SellsName = this.SellsName,
                        Amount = this.Amount,
                        VaildAmount = this.VaildAmount,
                        ConfrimDate = this.ConfrimDate,
                        AuthenticationMonth = this.AuthenticationMonth,
                        IsVaild = this.IsVaild,
                        InvoiceDetailID = this.InvoiceDetailID,
                        InvoiceType = (int)this.InvoiceType,
                        BusinessType = (int)this.BusinessType,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary,
                    }, item => item.ID == this.ID);
                }
            }
        }

        public void Abandon()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.TaxManage>(
                    new
                    {
                        Status = Enums.Status.Delete
                    }, item => item.ID == this.ID);
            }
        }
    }

    public class TaxManageUpload : TaxManage
    {
        public TaxManageUpload()
        {
            this.Status = Enums.Status.Normal;
            this.UpdateDate = this.CreateDate = DateTime.Now;
        }
        public void Upload()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.TaxManage>().Count(item => item.InvoiceCode == this.InvoiceCode && item.InvoiceNo==this.InvoiceNo);
                if (count == 0)
                {
                    reponsitory.Insert<Layer.Data.Sqls.ScCustoms.TaxManage>(new Layer.Data.Sqls.ScCustoms.TaxManage
                    {
                        ID = this.ID,
                        InvoiceCode = this.InvoiceCode,
                        InvoiceNo = this.InvoiceNo,
                        InvoiceDate = this.InvoiceDate,
                        SellsName = this.SellsName,
                        Amount = this.Amount,
                        VaildAmount = this.VaildAmount,
                        ConfrimDate = this.ConfrimDate,
                        AuthenticationMonth = this.AuthenticationMonth,
                        IsVaild = (int)this.IsVaild,
                        InvoiceDetailID = this.InvoiceDetailID,
                        InvoiceType = (int)this.InvoiceType,
                        BusinessType = (int)this.BusinessType,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary,
                    });

                    string InvoiceNoticeID = ChainsGuid.NewGuidUp();

                    reponsitory.Insert<Layer.Data.Sqls.ScCustoms.TaxMap>(new Layer.Data.Sqls.ScCustoms.TaxMap
                    {
                        ID = ChainsGuid.NewGuidUp(),
                        InvoiceNoticeID = InvoiceNoticeID,
                        InvoiceCode = this.InvoiceCode,
                        InvoiceNo = this.InvoiceNo,
                        InvoiceDate = this.InvoiceDate == null ? DateTime.Now : this.InvoiceDate.Value,
                        IsMapped = true,
                        ApiStatus = (int)Enums.TaxMapApiStatus.RevUnHandled,                       
                        InvoiceType = (int)Enums.InvoiceType.Full,
                        Status = (int)Enums.Status.Normal,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now
                    });

                    reponsitory.Insert<Layer.Data.Sqls.ScCustoms.TaxManageMap>(new Layer.Data.Sqls.ScCustoms.TaxManageMap
                    {
                        ID = ChainsGuid.NewGuidUp(),
                        InvoiceNoticeID = InvoiceNoticeID,
                        TaxManageID = this.ID,
                    });

                }
                else
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.TaxManage>(new
                    {                       
                       
                        AuthenticationMonth = this.AuthenticationMonth,                      
                        UpdateDate = this.UpdateDate,
                       
                    }, item => item.InvoiceCode == this.InvoiceCode&&item.InvoiceNo==this.InvoiceNo);
                }
            }
        }
    }
}
