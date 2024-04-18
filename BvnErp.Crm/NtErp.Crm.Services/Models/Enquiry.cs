using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NtErp.Crm.Services.Enums;
using Needs.Overall;
using NtErp.Crm.Services.Extends;
using Needs.Linq;

namespace NtErp.Crm.Services.Models
{
    /// <summary>
    /// 产品询价表
    /// </summary>
    public class Enquiry : Needs.Linq.IUnique
    {
        #region 属性
        /// <summary>
        /// 主键ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 报备时间
        /// </summary>
        public DateTime ReportDate { get; set; }
        /// <summary>
        /// 批复单价
        /// </summary>
        public decimal ReplyPrice { get; set; }

        /// <summary>
        /// 批复时间
        /// </summary>
        public DateTime ReplyDate { get; set; }

        /// <summary>
        /// 原厂RFQ号
        /// </summary>
        public string RFQ { get; set; }

        /// <summary>
        /// 原厂型号
        /// </summary>
        public string OriginModel { get; set; }

        /// <summary>
        /// 最小起订量
        /// </summary>
        public int? MOQ { get; set; }

        /// <summary>
        /// 最小包装量
        /// </summary>
        public int? MPQ { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public CurrencyType Currency { get; set; }

        /// <summary>
        /// 汇率
        /// </summary>
        public decimal? ExchangeRate { get; set; }

        /// <summary>
        /// 税率
        /// </summary>
        public decimal? TaxRate { get; set; }

        /// <summary>
        /// 关税点
        /// </summary>
        public decimal? Tariff { get; set; }

        /// <summary>
        /// 其他附加税点
        /// </summary>
        public decimal? OtherRate { get; set; }

        /// <summary>
        /// 含税人民币成本价
        /// </summary>
        public decimal? Cost { get; set; }

        /// <summary>
        /// 有效期
        /// </summary>
        public DateTime Validity { get; set; }

        /// <summary>
        /// 有效数量
        /// </summary>
        public int? ValidityCount { get; set; }

        /// <summary>
        /// 参考售价
        /// </summary>
        public decimal? SalePrice { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdateDate { get; set; }

        /// <summary>
        /// 特殊备注
        /// </summary>
        public string Summary { get; set; }
        #endregion

        #region 扩展属性
        /// <summary>
        /// 原厂批复凭证
        /// </summary>
        public Models.ProductItemFile Voucher { get; set; }

        /// <summary>
        /// ProductItemID
        /// </summary>
        internal string ProductItemID { get; set; }

        #endregion

        public Enquiry()
        {
            this.CreateDate = this.UpdateDate = DateTime.Now;
            this.EnterSuccess += Enquiry_EnterSuccess;
        }

        public event SuccessHanlder EnterSuccess;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productItemID"></param>
        public Enquiry(string productItemID) : this()
        {
            this.ProductItemID = productItemID;
        }

        public void Enter()
        {
            this.OnEnter();
            if (this != null && this.EnterSuccess != null)
            {
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }

        protected void OnEnter()
        {
            using (var reponsitory = new Layer.Data.Sqls.BvCrmReponsitory())
            {
                if (string.IsNullOrEmpty(this.ID))
                {
                    ID = PKeySigner.Pick(PKeyType.Enquiry);
                    reponsitory.Insert(this.ToLinq());

                }
                else
                {
                    this.UpdateDate = DateTime.Now;
                    reponsitory.Update<Layer.Data.Sqls.BvCrm.ProductItemEnquiries>(new
                    {
                        ReplyPrice = this.ReplyPrice,
                        ReplyDate = this.ReplyDate,
                        RFQ = this.RFQ,
                        OriginModel = this.OriginModel,
                        MOQ = this.MOQ.GetValueOrDefault(),
                        MPQ = this.MPQ,
                        Currency = (int)this.Currency,
                        ExchangeRate = this.ExchangeRate,
                        TaxRate = this.TaxRate,
                        Tariff = this.Tariff,
                        OtherRate = this.OtherRate,
                        Cost = this.Cost,
                        Validity = this.Validity,
                        ValidityCount = this.ValidityCount,
                        SalePrice = this.SalePrice,
                        CreateDate = this.CreateDate ?? DateTime.Now,
                        UpdateDate = this.UpdateDate ?? DateTime.Now,
                        ReportDate = this.ReportDate,
                        Summary = this.Summary,
                    }, item => item.ID == this.ID);
                }

                if (this.Voucher != null)
                {
                    this.FilesEnter(reponsitory);
                }
            }
        }

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="reponsitory"></param>
        private void FilesEnter(Layer.Data.Sqls.BvCrmReponsitory reponsitory)
        {
            var file = this.Voucher;
            if (string.IsNullOrWhiteSpace(file.ID))
            {
                file.ID = Guid.NewGuid().ToString();
                reponsitory.Insert(new Layer.Data.Sqls.BvCrm.ProductItemFiles
                {
                    ID = file.ID,
                    ProductItemID = this.ProductItemID,
                    SubID = this.ID,
                    Type = (int)file.Type,
                    Name = file.Name,
                    Url = file.Url,
                    CreateDate = file.CreateDate,
                    Status = (int)file.Status,
                    AdminID = file.AdminID,
                });
            }
            else
            {
                reponsitory.Update<Layer.Data.Sqls.BvCrm.ProductItemFiles>(new
                {
                    Type = (int)file.Type,
                    Name = file.Name,
                    Url = file.Url,
                    CreateDate = DateTime.Now,
                    AdminID = file.AdminID,
                }, item => item.ID == file.ID);
            }

        }

        /// <summary>
        /// 插入询价信息时建立ProductItem 与 ProductItemEnquiry的联系
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Enquiry_EnterSuccess(object sender, SuccessEventArgs e)
        {
            // Handle MapsProductItemEnquiry
            using (var reponsitory = new Layer.Data.Sqls.BvCrmReponsitory())
            {
                if (!reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsProductItemEnquiry>().Any(item => item.ProductItemEnquiryID == this.ID && item.ProductItemID == this.ProductItemID))
                {
                    reponsitory.Insert(new Layer.Data.Sqls.BvCrm.MapsProductItemEnquiry
                    {
                        ID = Guid.NewGuid().ToString(),
                        ProductItemID = this.ProductItemID,
                        ProductItemEnquiryID = this.ID,
                    });
                }
            }
        }
    }
}
