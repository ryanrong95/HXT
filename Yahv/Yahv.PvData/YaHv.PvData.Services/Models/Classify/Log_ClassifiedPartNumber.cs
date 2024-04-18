using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Usually;

namespace YaHv.PvData.Services.Models
{
    /// <summary>
    /// 归类历史记录
    /// </summary>
    public class Log_ClassifiedPartNumber : IUnique
    {
        #region 属性

        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string PartNumber { get; set; }

        /// <summary>
        /// 品牌/制造商
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// 海关编码
        /// </summary>
        public string HSCode { get; set; }

        /// <summary>
        /// 报关品名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 增值税率
        /// </summary>
        public decimal VATRate { get; set; }

        /// <summary>
        /// 进口优惠税率
        /// </summary>
        public decimal ImportPreferentialTaxRate { get; set; }

        /// <summary>
        /// 产地加征税率
        /// </summary>
        public decimal OriginATRate { get; set; }

        /// <summary>
        /// 消费税率
        /// </summary>
        public decimal ExciseTaxRate { get; set; }

        /// <summary>
        /// 申报要素
        /// </summary>
        public string Elements { get; set; }

        /// <summary>
        /// 税务编码
        /// </summary>
        public string TaxCode { get; set; }

        /// <summary>
        /// 税务名称
        /// </summary>
        public string TaxName { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public decimal? Quantity { get; set; }

        /// <summary>
        /// 是否商检
        /// </summary>
        public bool CIQ { get; set; }

        /// <summary>
        /// 商检费
        /// </summary>
        public decimal CIQprice { get; set; }

        /// <summary>
        /// 创建人/报关员
        /// </summary>
        public string CreatorID { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        #endregion

        #region 事件

        public event SuccessHanlder EnterSuccess;

        #endregion

        #region 持久化

        public void Enter()
        {
            using (var repository = LinqFactory<PvDataReponsitory>.Create())
            {
                this.ID = Layers.Data.PKeySigner.Pick(PKeyType.ClassifiedPartNumberLog);
                repository.Insert(new Layers.Data.Sqls.PvData.Logs_ClassifiedPartNumber()
                {
                    ID = this.ID,
                    PartNumber = this.PartNumber,
                    Manufacturer = this.Manufacturer,
                    HSCode = this.HSCode,
                    Name = this.Name,
                    VATRate = this.VATRate,
                    ImportPreferentialTaxRate = this.ImportPreferentialTaxRate,
                    OriginATRate = this.OriginATRate,
                    ExciseTaxRate = this.ExciseTaxRate,
                    Elements = this.Elements,
                    TaxCode = this.TaxCode,
                    TaxName = this.TaxName,
                    Currency = this.Currency,
                    UnitPrice = this.UnitPrice,
                    Quantity = this.Quantity,
                    CIQ = this.CIQ,
                    CIQprice = this.CIQprice,
                    CreatorID = this.CreatorID,
                    CreateDate = DateTime.Now
                });
            }

            if (this != null && this.EnterSuccess != null)
            {
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }

        #endregion
    }
}
