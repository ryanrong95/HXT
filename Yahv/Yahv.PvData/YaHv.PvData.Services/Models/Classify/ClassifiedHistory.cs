using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YaHv.PvData.Services.Extends;

namespace YaHv.PvData.Services.Models
{
    /// <summary>
    /// 归类历史数据
    /// </summary>
    public class ClassifiedHistory : Interfaces.IProductConstraint
    {
        #region 属性

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
        public string TariffName { get; set; }

        /// <summary>
        /// 税务编码
        /// </summary>
        public string TaxCode { get; set; }

        /// <summary>
        /// 税务名称
        /// </summary>
        public string TaxName { get; set; }

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
        decimal vatRate;

        /// <summary>
        /// 增值税率
        /// </summary>
        public decimal VATRate
        {
            get
            {
                return this.vatRate;
            }
            set
            {
                this.vatRate = decimal.Parse(value.ToString("0.#######"));
            }
        }

        /// <summary>
        /// 新需求，如果有暂定税率且暂定税率低于优惠税率则使用暂定税率，否则使用优惠税率
        /// </summary>
        public decimal ImportTaxRate
        {
            get
            {
                if (this.importControlTaxRate != null && this.importControlTaxRate < this.importPreferentialTaxRate)
                    return this.importControlTaxRate.Value;

                return this.ImportPreferentialTaxRate;
            }
        }

        decimal importPreferentialTaxRate;

        /// <summary>
        /// 进口优惠税率
        /// </summary>
        public decimal ImportPreferentialTaxRate
        {
            get
            {
                return this.importPreferentialTaxRate;
            }
            set
            {
                this.importPreferentialTaxRate = decimal.Parse(value.ToString("0.#######"));
            }
        }

        decimal? importControlTaxRate;

        /// <summary>
        /// 进口暂定税率
        /// </summary>
        public decimal? ImportControlTaxRate
        {
            get
            {
                return this.importControlTaxRate;
            }
            set
            {
                if (value == null)
                {
                    this.importControlTaxRate = null;
                }
                else
                {
                    this.importControlTaxRate = decimal.Parse(value?.ToString("0.#######"));
                }
            }
        }

        decimal exciseTaxRate;

        /// <summary>
        /// 消费税率
        /// </summary>
        public decimal ExciseTaxRate
        {
            get
            {
                return this.exciseTaxRate;
            }
            set
            {
                this.exciseTaxRate = decimal.Parse(value.ToString("0.#######"));
            }
        }

        /// <summary>
        /// 检验检疫编码
        /// </summary>
        public string CIQCode { get; set; }

        /// <summary>
        /// 申报要素
        /// </summary>
        public string Elements { get; set; }

        /// <summary>
        /// 是否需要3C认证
        /// </summary>
        public bool Ccc { get; set; }

        /// <summary>
        /// 是否是禁运产品
        /// </summary>
        public bool Embargo { get; set; }

        /// <summary>
        /// 是否是香港管制
        /// </summary>
        public bool HkControl { get; set; }

        /// <summary>
        /// 是否需要原产地证明
        /// </summary>
        public bool Coo { get; set; }

        /// <summary>
        /// 是否需要商检
        /// </summary>
        public bool CIQ { get; set; }

        /// <summary>
        /// 商检费
        /// </summary>
        public decimal CIQprice { get; set; }

        /// <summary>
        /// 备注信息
        /// </summary>
        public string Summary { get; set; }

        #endregion

        public string SupervisionRequirements { get; set; }

        public string CIQC { get; set; }

        public DateTime OrderDate { get; set; }

        #region 扩展属性

        public string SpecialTypes
        {
            get
            {
                StringBuilder specialType = new StringBuilder();

                if (this.Ccc)
                    specialType.Append("3C|");
                if (this.Embargo)
                    specialType.Append("禁运|");
                if (this.HkControl)
                    specialType.Append("香港管制|");
                if (this.Coo)
                    specialType.Append("原产地证明|");
                if (this.CIQ)
                    specialType.Append("商检|");

                if (specialType.Length == 0)
                    return "--";
                else
                    return specialType.ToString().TrimEnd('|');
            }
        }

        public string CreatorID { get; set; }

        public string CreatorName { get; set; }

        #endregion

        #region 持久化

        public void Enter()
        {
            using (var reponsitory = new PvDataReponsitory())
            {
                Past_ClassifiedMidifiedEnter(this, reponsitory);
                ClassifiedPartNumberEnter(this, reponsitory);
                OtherEnter(this, reponsitory);
            }
        }

        /// <summary>
        /// 备注信息
        /// </summary>
        public void Remark()
        {
            using (var reponsitory = new PvDataReponsitory())
            {
                reponsitory.Update<Layers.Data.Sqls.PvData.Others>(new
                {
                    Summary = this.Summary
                }, a => a.PartNumber == this.PartNumber && a.Manufacturer == this.Manufacturer);
            }
        }

        private void Past_ClassifiedMidifiedEnter(ClassifiedHistory ch, PvDataReponsitory reponsitory)
        {
            var cpn = new Views.Alls.ClassifiedPartNumbersAll(reponsitory)[ch.PartNumber, ch.Manufacturer];
            if (cpn != null && ch.CreatorID != null)
            {
                var histClassified = cpn.FillHistoryClassified();

                #region 记录日志

                string prefix = "报关员【" + ch.CreatorName + "】将型号【" + ch.PartNumber + "】、品牌【" + ch.Manufacturer + "】的";

                //归类已完成列表导出Excel要求单独包含海关编码的变更记录，所以HSCode单独记日志
                if (ch.HSCode != histClassified.HSCode)
                    ch.Log(prefix + "海关编码由【" + histClassified.HSCode + "】修改为【" + ch.HSCode + "】", reponsitory);

                StringBuilder sbLog = new StringBuilder();

                if (ch.Manufacturer != histClassified.Manufacturer)
                    sbLog.Append("品牌由【" + histClassified.Manufacturer + "】修改为【" + ch.Manufacturer + "】、");
                if (ch.TariffName != histClassified.TariffName)
                    sbLog.Append("报关品名由【" + histClassified.TariffName + "】修改为【" + ch.TariffName + "】、");
                if (ch.TaxCode != histClassified.TaxCode)
                    sbLog.Append("税务编码由【" + histClassified.TaxCode + "】修改为【" + ch.TaxCode + "】、");
                if (ch.TaxName != histClassified.TaxName)
                    sbLog.Append("税务名称由【" + histClassified.TaxName + "】修改为【" + ch.TaxName + "】、");
                if (ch.ImportPreferentialTaxRate != histClassified.ImportPreferentialTaxRate)
                    sbLog.Append("优惠税率由【" + histClassified.ImportPreferentialTaxRate.ToString("#0.0000") + "】修改为【" + ch.ImportPreferentialTaxRate + "】、");
                if (ch.VATRate != histClassified.VATRate)
                    sbLog.Append("增值税率由【" + histClassified.VATRate.ToString("#0.0000") + "】修改为【" + ch.VATRate + "】、");
                if (ch.ExciseTaxRate != histClassified.ExciseTaxRate)
                    sbLog.Append("消费税率由【" + histClassified.ExciseTaxRate.ToString("#0.0000") + "】修改为【" + ch.ExciseTaxRate + "】、");
                if (ch.LegalUnit1 != histClassified.LegalUnit1)
                    sbLog.Append("法定第一单位由【" + histClassified.LegalUnit1 + "】修改为【" + ch.LegalUnit1 + "】、");
                if (ch.LegalUnit2 != null && ch.LegalUnit2 != histClassified.LegalUnit2)
                    sbLog.Append("法定第二单位由【" + histClassified.LegalUnit2 + "】修改为【" + ch.LegalUnit2 + "】、");
                if (ch.CIQCode != histClassified.CIQCode)
                    sbLog.Append("检验检疫编码由【" + histClassified.CIQCode + "】修改为【" + ch.CIQCode + "】、");
                if (ch.Elements != histClassified.Elements)
                    sbLog.Append("申报要素由【" + histClassified.Elements + "】修改为【" + ch.Elements + "】、");
                if (ch.Ccc != histClassified.Ccc)
                    sbLog.Append("CCC认证由【" + (histClassified.Ccc ? "是" : "否") + "】修改为【" + (ch.Ccc ? "是" : "否") + "】、");
                if (ch.Coo != histClassified.Coo)
                    sbLog.Append("原产地证明由【" + (histClassified.Coo ? "是" : "否") + "】修改为【" + (ch.Coo ? "是" : "否") + "】、");
                if (ch.Embargo != histClassified.Embargo)
                    sbLog.Append("是否禁运由【" + (histClassified.Embargo ? "是" : "否") + "】修改为【" + (ch.Embargo ? "是" : "否") + "】、");
                if (ch.HkControl != histClassified.HkControl)
                    sbLog.Append("是否香港管制由【" + (histClassified.HkControl ? "是" : "否") + "】修改为【" + (ch.HkControl ? "是" : "否") + "】、");
                if (ch.CIQ != histClassified.CIQ)
                    sbLog.Append("是否商检由【" + (histClassified.CIQ ? "是" : "否") + "】修改为【" + (ch.CIQ ? "是" : "否") + "】、");
                if (ch.CIQprice != histClassified.CIQprice)
                    sbLog.Append("商检费由【" + histClassified.CIQprice.ToString("#0.0000") + "】修改为【" + ch.CIQprice + "】、");

                if (sbLog.Length > 0)
                {
                    ch.Log(prefix + sbLog.ToString().TrimEnd('、'), reponsitory);
                }

                #endregion
            }
        }

        private void ClassifiedPartNumberEnter(ClassifiedHistory ch, PvDataReponsitory reponsitory)
        {
            var cpnId = ch.GenID();
            if (!reponsitory.ReadTable<Layers.Data.Sqls.PvData.ClassifiedPartNumbers>().Any(t => t.ID == cpnId))
            {
                reponsitory.Insert(new Layers.Data.Sqls.PvData.ClassifiedPartNumbers()
                {
                    ID = cpnId,
                    PartNumber = ch.PartNumber,
                    Manufacturer = ch.Manufacturer,
                    HSCode = ch.HSCode,
                    Name = ch.TariffName,
                    LegalUnit1 = ch.LegalUnit1,
                    LegalUnit2 = ch.LegalUnit2,
                    VATRate = ch.VATRate,
                    ImportPreferentialTaxRate = ch.ImportPreferentialTaxRate,
                    ExciseTaxRate = ch.ExciseTaxRate,
                    Elements = ch.Elements,
                    CIQCode = ch.CIQCode,
                    TaxCode = ch.TaxCode,
                    TaxName = ch.TaxName,
                    CreateDate = DateTime.Now,
                    OrderDate = DateTime.Now
                });
            }
            else
            {
                reponsitory.Update<Layers.Data.Sqls.PvData.ClassifiedPartNumbers>(new { OrderDate = DateTime.Now }, a => a.ID == cpnId);
            }
        }

        private void OtherEnter(ClassifiedHistory ch, PvDataReponsitory reponsitory)
        {
            var id = ch.GenOtherID();
            if (!reponsitory.ReadTable<Layers.Data.Sqls.PvData.Others>().Any(t => t.PartNumber == ch.PartNumber && t.Manufacturer == ch.Manufacturer))
            {
                reponsitory.Insert(new Layers.Data.Sqls.PvData.Others()
                {
                    ID = id,
                    PartNumber = ch.PartNumber,
                    Manufacturer = ch.Manufacturer,
                    Ccc = ch.Ccc,
                    Embargo = ch.Embargo,
                    HkControl = ch.HkControl,
                    Coo = ch.Coo,
                    CIQ = ch.CIQ,
                    CIQprice = ch.CIQprice,
                    CreateDate = DateTime.Now,
                    OrderDate = DateTime.Now
                });
            }
            else
            {
                reponsitory.Update<Layers.Data.Sqls.PvData.Others>(new
                {
                    Ccc = ch.Ccc,
                    Embargo = ch.Embargo,
                    HkControl = ch.HkControl,
                    Coo = ch.Coo,
                    CIQ = ch.CIQ,
                    CIQprice = ch.CIQprice,
                    OrderDate = DateTime.Now
                }, a => a.PartNumber == ch.PartNumber && a.Manufacturer == ch.Manufacturer);
            }
        }

        #endregion
    }
}
