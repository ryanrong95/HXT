namespace Yahv.Finance.Services.Models.Rolls
{
    /// <summary>
    /// 金库输入传输类
    /// </summary>
    public class GoldStoreInputDto
    {
        /// <summary>
        /// 金库名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 原始金库名称（用于修改）
        /// </summary>
        public string OriginName { get; set; }

        /// <summary>
        /// 负责人ID
        /// </summary>
        public string OwnerID { get; set; }

        /// <summary>
        /// 创建人ID
        /// </summary>
        public string CreatorID { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Summary { get; set; }
    }
}