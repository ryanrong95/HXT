using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 产品的包装类型
    /// </summary>
    public enum Package
    {

        [Description("纸制或纤维板制桶")]
        [Package("32", "纸制或纤维板制桶")]
        纸制或纤维板制桶 = 32,

        [Description("包或袋")]
        [Package("06", "包或袋")]
        包或袋 = 6,

        [Description("其他包装")]
        [Package("99", "其他包装")]
        其他包装 = 99,

        [Description("其他材料制盒/箱")]
        [Package("29", "其他材料制盒/箱")]
        其他材料制盒 = 29,

        [Description("裸装")]
        [Package("01", "裸装")]
        裸装 = 1,

        [Description("散装")]
        [Package("00", "散装")]
        散装 = 0,

        [Description("其他材料制桶")]
        [Package("39", "其他材料制桶")]
        其他材料制桶 = 39,

        [Description("纸制或纤维板制盒/箱")]
        [Package("22", "纸制或纤维板制盒/箱")]
        纸制或纤维板制盒 = 22,

        [Description("再生木托")]
        [Package("92", "再生木托")]
        再生木托 = 92,

        [Description("天然木托")]
        [Package("93", "天然木托")]
        天然木托 = 93,

        [Description("木制或竹藤等植物性材料制桶")]
        [Package("33", "木制或竹藤等植物性材料制桶")]
        木制或竹藤等植物性材料制桶 = 33,

        [Description("木制或竹藤等植物性材料制盒/箱")]
        [Package("23", "木制或竹藤等植物性材料制盒/箱")]
        木制或竹藤等植物性材料制盒 = 23,

        [Description("球状罐类")]
        [Package("04", "球状罐类")]
        球状罐类 = 4,

    }
}
