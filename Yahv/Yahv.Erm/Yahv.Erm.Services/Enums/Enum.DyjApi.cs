using System.Configuration;
using Yahv.Underly.Attributes;

namespace Yahv.Erm.Services
{
    /// <summary>
    /// 大赢家api
    /// </summary>
    public enum DyjApi
    {
        [ApiAddress("DyjToken")]
        Token,

        /// <summary>
        /// 通讯录接口
        /// </summary>
        [ApiAddress("DyjApiHostName", "/ApiCenter/outService/StandardData.ashx")]
        Contact,
    }
}