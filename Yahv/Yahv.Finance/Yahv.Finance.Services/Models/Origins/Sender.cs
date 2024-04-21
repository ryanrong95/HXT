using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Finance.Services.Models.Origins
{
    /// <summary>
    /// 接口发起人
    /// </summary>
    public class Sender : IUnique
    {
        #region 属性
        public string ID { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 秘钥
        /// </summary>
        public string SecretKey { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public GeneralStatus Status { get; set; }
        #endregion
    }
}