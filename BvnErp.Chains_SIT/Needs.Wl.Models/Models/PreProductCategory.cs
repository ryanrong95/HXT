using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Model;

namespace Needs.Wl.Models
{
    public class PreProductCategory : ModelBase<Layer.Data.Sqls.ScCustoms.PreProductCategories, ScCustomsReponsitory>, IUnique, IPersist
    {
        public string PreProductID { get; set; }

        public string Model { get; set; }

        public string Manufacture { get; set; }

        public string ProductName { get; set; }

        public string HSCode { get; set; }

        public decimal? TariffRate { get; set; }

        public decimal? AddedValueRate { get; set; }

        public string TaxCode { get; set; }

        public string TaxName { get; set; }

        public Enums.ItemCategoryType Type { get; set; }

        /// <summary>
        /// 商检费
        /// </summary>
        public decimal? InspectionFee { get; set; }

        public string Unit1 { get; set; }

        public string Unit2 { get; set; }

        public string CIQCode { get; set; }

        public string Elements { get; set; }

        public Enums.ClassifyStatus ClassifyStatus { get; set; }

        public string ClassifyFirstOperator { get; set; }

        public string ClassifySecondOperator { get; set; }

        public override void Enter()
        {
            base.Enter();
        }
    }
}