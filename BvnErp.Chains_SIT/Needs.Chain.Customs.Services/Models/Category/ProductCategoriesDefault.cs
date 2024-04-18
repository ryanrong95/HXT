using Needs.Ccs.Services.Enums;
using Needs.Linq;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 自动归类历史记录
    /// </summary>
    public class ProductCategoriesDefault : IUnique, IFulError, IFulSuccess
    {
        #region 属性

        string id;
        public string ID
        {
            get
            {
                return this.id ?? string.Concat(this.Model).MD5();

            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// 型号
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// 品名
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 海关编码
        /// </summary>
        public string HSCode { get; set; }

        /// <summary>
        /// 关税率
        /// </summary>
        public decimal? TariffRate { get; set; }

        /// <summary>
        /// 增值税率
        /// </summary>
        public decimal? AddedValueRate { get; set; }

        /// <summary>
        /// 消费税率
        /// </summary>
        public decimal? ExciseTaxRate { get; set; }

        /// <summary>
        /// 税务编码
        /// </summary>
        public string TaxCode { get; set; }

        /// <summary>
        /// 税务名称
        /// </summary>
        public string TaxName { get; set; }

        /// <summary>
        /// 归类类型
        /// </summary>
        public ItemCategoryType? Type { get; set; }

        /// <summary>
        /// 预归类类型
        /// </summary>
        public IcgooClassifyTypeEnums? ClassifyType { get; set; }

        /// <summary>
        /// 商检费
        /// </summary>
        public decimal? InspectionFee { get; set; }

        /// <summary>
        /// 法定第一单位
        /// </summary>
        public string Unit1 { get; set; }

        /// <summary>
        /// 法定第二单位
        /// </summary>
        public string Unit2 { get; set; }

        /// <summary>
        /// 检验检疫编码
        /// </summary>
        public string CIQCode { get; set; }

        /// <summary>
        /// 申报要素
        /// </summary>
        public string Elements { get; set; }

        /// <summary>
        /// 状态：正常、删除
        /// </summary>
        public Status Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Summary { get; set; }

        #endregion

        public event ErrorHanlder EnterError;
        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event SuccessHanlder EnterSuccess;

        public ProductCategoriesDefault()
        {
            this.Status = Status.Normal;
            this.CreateDate = this.UpdateDate = DateTime.Now;
        }

        /// <summary>
        /// 持久化
        /// </summary>
        public void DefaultEnter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ProductCategoriesDefaults>().Count(item => item.Model == this.Model);
                if (count == 0)
                {
                    reponsitory.Insert(this.ToLinq());
                }
                else
                {
                    reponsitory.Update(this.ToLinq(), item => item.Model == this.Model);
                }
            }
        }
    }
}
