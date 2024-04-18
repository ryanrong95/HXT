using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Usually;

namespace YaHv.PvData.Services.Models
{
    /// <summary>
    /// 海关税则
    /// </summary>
    public class Tariff : IUnique
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
                //编码规则：与海关编码保持一致
                return this.id ?? this.HSCode;
            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// 海关编码
        /// </summary>
        public string HSCode { get; set; }

        /// <summary>
        /// 报关品名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 法定第一单位
        /// </summary>
        public string LegalUnit1 { get; set; }

        /// <summary>
        /// 法定第二单位
        /// </summary>
        public string LegalUnit2 { get; set; }

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
                if(this.importControlTaxRate != null && this.importControlTaxRate < this.importPreferentialTaxRate)
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

        decimal importGeneralTaxRate;

        /// <summary>
        /// 进口普通税率
        /// </summary>
        public decimal ImportGeneralTaxRate
        {
            get
            {
                return this.importGeneralTaxRate;
            }
            set
            {
                this.importGeneralTaxRate = decimal.Parse(value.ToString("0.#######"));
            }
        }

        decimal exciseTaxRate;

        /// <summary>
        /// 消费税率
        /// </summary>
        public decimal? ExciseTaxRate
        {
            get
            {
                return this.exciseTaxRate;
            }
            set
            {
                this.exciseTaxRate = decimal.Parse(value.GetValueOrDefault().ToString("0.#######"));
            }
        }

        /// <summary>
        /// 申报要素
        /// </summary>
        public string DeclareElements { get; set; }

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
        /// 数据状态：正常、删除
        /// </summary>
        public GeneralStatus Status { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifyDate { get; set; }

        #endregion

        #region 扩展属性

        /// <summary>
        /// 原产地税则
        /// </summary>
        private IEnumerable<OriginATRate> originsATRate;
        public IEnumerable<OriginATRate> OriginsATRate
        {
            get
            {
                if (originsATRate == null)
                {
                    using (var view = new Views.Alls.OriginsATRateAll())
                    {
                        this.originsATRate = view.Where(item => item.TariffID == this.ID).ToList();
                    }
                }

                return this.originsATRate;
            }
            set
            {
                this.originsATRate = value;
            }
        }

        /// <summary>
        /// 申报要素默认值
        /// </summary>
        private IEnumerable<ElementsDefault> elementsDefault;
        public IEnumerable<ElementsDefault> ElementsDefaults
        {
            get
            {
                if (elementsDefault == null)
                {
                    using (var view = new Views.Origins.ElementsDefaultsOrigin())
                    {
                        this.elementsDefault = view.Where(item => item.TariffID == this.ID).ToList();
                    }
                }

                return this.elementsDefault;
            }
            set
            {
                this.elementsDefault = value;
            }
        }

        #endregion

        #region 事件

        public event SuccessHanlder EnterSuccess;
        public event SuccessHanlder DeleteSuccess;

        #endregion

        #region 持久化

        public void Enter()
        {
            using (var repository = LinqFactory<PvDataReponsitory>.Create())
            {
                //添加
                if (!repository.ReadTable<Layers.Data.Sqls.PvData.Tariffs>().Any(t => t.ID == this.ID))
                {
                    repository.Insert(new Layers.Data.Sqls.PvData.Tariffs()
                    {
                        ID = this.ID,
                        HSCode = this.HSCode,
                        Name = this.Name,
                        LegalUnit1 = this.LegalUnit1,
                        LegalUnit2 = this.LegalUnit2,
                        VATRate = this.VATRate,
                        ImportPreferentialTaxRate = this.ImportPreferentialTaxRate,
                        ImportControlTaxRate = this.ImportControlTaxRate,
                        ImportGeneralTaxRate = this.ImportGeneralTaxRate,
                        ExciseTaxRate = this.ExciseTaxRate,
                        DeclareElements = this.DeclareElements,
                        SupervisionRequirements = this.SupervisionRequirements,
                        CIQC = this.CIQC,
                        CIQCode = this.CIQCode,
                        Status = (int)GeneralStatus.Normal,
                        CreateDate = DateTime.Now,
                        ModifyDate = DateTime.Now
                    });
                }
                //修改
                else
                {
                    repository.Update<Layers.Data.Sqls.PvData.Tariffs>(new
                    {
                        Name = this.Name,
                        LegalUnit1 = this.LegalUnit1,
                        LegalUnit2 = this.LegalUnit2,
                        VATRate = this.VATRate,
                        ImportPreferentialTaxRate = this.ImportPreferentialTaxRate,
                        ImportControlTaxRate = this.ImportControlTaxRate,
                        ImportGeneralTaxRate = this.ImportGeneralTaxRate,
                        ExciseTaxRate = this.ExciseTaxRate,
                        DeclareElements = this.DeclareElements,
                        SupervisionRequirements = this.SupervisionRequirements,
                        CIQC = this.CIQC,
                        CIQCode = this.CIQCode,
                        ModifyDate = DateTime.Now
                    }, a => a.ID == this.ID);
                }
            }

            if (this != null && this.EnterSuccess != null)
            {
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }

        public void Delete()
        {
            using (var repository = LinqFactory<PvDataReponsitory>.Create())
            {
                repository.Delete<Layers.Data.Sqls.PvData.ElementsDefaults>(item => item.TariffID == this.ID);
                repository.Delete<Layers.Data.Sqls.PvData.OriginsATRate>(item => item.TariffID == this.ID);
                repository.Delete<Layers.Data.Sqls.PvData.Tariffs>(item => item.ID == this.ID);
            }

            if (this != null && this.DeleteSuccess != null)
            {
                this.DeleteSuccess(this, new SuccessEventArgs(this));
            }
        }

        #endregion
    }
}
