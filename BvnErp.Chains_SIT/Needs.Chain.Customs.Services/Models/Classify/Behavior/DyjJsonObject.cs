using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 大赢家归类大数据
    /// </summary>
    public class DyjJsonObject
    {
        public string status { get; set; }

        public IEnumerable<HSCode> HSCodes { get; set; }

        public IEnumerable<HSElements> HSElements { get; set; }

        public IEnumerable<Inspection> Inspections { get; set; }

        public IEnumerable<Embargo> Embargos { get; set; }

        public IEnumerable<Manu> Manus { get; set; }

        public IEnumerable<CCC> CCCs { get; set; }

        public IEnumerable<Price> Prices { get; set; }

        public IEnumerable<TariffRate> TariffRates { get; set; }

        public IEnumerable<TaxCode> TaxCodes { get; set; }
    }

    /// <summary>
    /// 海关编码
    /// </summary>
    public class HSCode
    {
        public string hscode { get; set; }
        public string source { get; set; }
        public int level { get; set; }
    }

    /// <summary>
    /// 海关申报要素
    /// </summary>
    public class HSElements
    {
        public string id { get; set; }
        public string source { get; set; }
        public int level { get; set; }
        public DateTime 时间 { get; set; }
        public string declare_class { get; set; }
    }

    /// <summary>
    /// 商检
    /// </summary>
    public class Inspection
    {
        public string id { get; set; }
        public string inspection { get; set; }
        public string source { get; set; }
        public int level { get; set; }
    }

    /// <summary>
    /// 禁运
    /// </summary>
    public class Embargo
    {
        public string id { get; set; }
        public string embargo { get; set; }
        public string source { get; set; }
        public int level { get; set; }
    }

    /// <summary>
    /// 品牌
    /// </summary>
    public class Manu
    {
        public string id { get; set; }
        public string manu { get; set; }
        public string source { get; set; }
        public int level { get; set; }
    }

    /// <summary>
    /// 3C
    /// </summary>
    public class CCC
    {
        public string id { get; set; }
        public string ccc { get; set; }
        public string source { get; set; }
        public int level { get; set; }
    }

    /// <summary>
    /// 报关价格
    /// </summary>
    public class Price
    {
        public int level { get; set; }
        public DateTime 时间 { get; set; }
        public decimal 报关价格 { get; set; }
    }

    /// <summary>
    /// 关税率
    /// </summary>
    public class TariffRate
    {
        public int level { get; set; }
        public DateTime 时间 { get; set; }
        public decimal 关税率 { get; set; }
    }

    /// <summary>
    /// 税务编码
    /// </summary>
    public class TaxCode
    {
        public string source { get; set; }
        public int level { get; set; }
        public string tax_code { get; set; }
    }
}
