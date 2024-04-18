namespace Yahv.Services.Models
{
    /// <summary>
    /// 品牌制作商
    /// </summary>
    public class Manufacturer
    {

        public Manufacturer()
        {

        }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get;  set; }

        /// <summary>
        /// 是否代理
        /// </summary>
        public bool Agent { get;  set; }
    }
}
