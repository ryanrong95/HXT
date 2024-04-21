using Yahv.Underly.Attributes;

namespace YaHv.Csrm.Services
{
    public enum SubjectType
    {
        [Description("业务")]
        Business = 1,
        [Description("分类")]
        Catalog = 2,
        [Description("科目")]
        Subject = 3,
    }
}