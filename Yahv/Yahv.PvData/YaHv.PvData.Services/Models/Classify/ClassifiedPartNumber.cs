using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Usually;
using Yahv.Utils.Converters.Contents;

namespace YaHv.PvData.Services.Models
{
    /// <summary>
    /// 海关税则归类信息
    /// </summary>
    public class ClassifiedPartNumber : IUnique
    {
        #region 属性

        string id;
        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID
        {
            get
            {
                //编码规则：所有归类字段的MD5
                return this.id ?? string.Concat(this.PartNumber, this.Manufacturer, this.HSCode, this.TariffName,
                                               this.LegalUnit1, this.LegalUnit2, 
                                               this.VATRate, this.ImportPreferentialTaxRate, this.ExciseTaxRate,
                                               this.Elements, this.SupervisionRequirements, this.CIQC, this.CIQCode,
                                               this.TaxCode, this.TaxName).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// 型号
        /// </summary>
        public string PartNumber { get; set; }

        /// <summary>
        /// 制造商
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// 海关编码
        /// </summary>
        public string HSCode { get; set; }

        /// <summary>
        /// 报关品名
        /// </summary>
        public string TariffName { get; set; }

        /// <summary>
        /// 法定第一单位
        /// </summary>
        public string LegalUnit1 { get; set; }

        /// <summary>
        /// 法定第二单位
        /// </summary>
        public string LegalUnit2 { get; set; }

        /// <summary>
        /// 增值税率
        /// </summary>
        public decimal VATRate { get; set; }

        /// <summary>
        /// 进口优惠税率
        /// </summary>
        public decimal ImportPreferentialTaxRate { get; set; }

        /// <summary>
        /// 消费税率
        /// </summary>
        public decimal ExciseTaxRate { get; set; }

        /// <summary>
        /// 申报要素
        /// </summary>
        public string Elements { get; set; }

        /// <summary>
        /// 监管条件
        /// </summary>
        public string SupervisionRequirements { get; set; }

        /// <summary>
        /// 检验检疫类别
        /// </summary>
        public string CIQC { get; set; }

        /// <summary>
        /// 检验检疫编码
        /// </summary>
        public string CIQCode { get; set; }

        /// <summary>
        /// 税务编码
        /// </summary>
        public string TaxCode { get; set; }

        /// <summary>
        /// 税务名称
        /// </summary>
        public string TaxName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 用于排序的时间字段
        /// </summary>
        public DateTime OrderDate { get; set; }

        #endregion

        #region 事件

        public event SuccessHanlder EnterSuccess;

        #endregion

        #region 持久化

        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvDataReponsitory>.Create())
            {
                //添加
                if (!reponsitory.ReadTable<Layers.Data.Sqls.PvData.ClassifiedPartNumbers>().Any(t => t.ID == this.ID))
                {
                    reponsitory.Insert(new Layers.Data.Sqls.PvData.ClassifiedPartNumbers()
                    {
                        ID = this.ID,
                        PartNumber = this.PartNumber,
                        Manufacturer = this.Manufacturer,
                        HSCode = this.HSCode,
                        Name = this.TariffName,
                        LegalUnit1 = this.LegalUnit1,
                        LegalUnit2 = this.LegalUnit2,
                        VATRate = this.VATRate,
                        ImportPreferentialTaxRate = this.ImportPreferentialTaxRate,
                        ExciseTaxRate = this.ExciseTaxRate,
                        Elements = this.Elements,
                        SupervisionRequirements = this.SupervisionRequirements,
                        CIQC = this.CIQC,
                        CIQCode = this.CIQCode,
                        TaxCode = this.TaxCode,
                        TaxName = this.TaxName,
                        CreateDate = DateTime.Now,
                        OrderDate = DateTime.Now
                    });
                }
                //修改
                else
                {
                    reponsitory.Update<Layers.Data.Sqls.PvData.ClassifiedPartNumbers>(new
                    {
                        OrderDate = DateTime.Now
                    }, a => a.ID == this.ID);
                }
            }

            if (this != null && this.EnterSuccess != null)
            {
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }

        #endregion
    }
}
