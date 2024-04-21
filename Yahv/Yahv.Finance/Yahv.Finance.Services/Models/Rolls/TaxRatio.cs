namespace Yahv.Finance.Services.Models.Rolls
{
    /// <summary>
    /// 税率字典类
    /// </summary>
    public class TaxRatio
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public decimal Rate { get; set; }
    }
}