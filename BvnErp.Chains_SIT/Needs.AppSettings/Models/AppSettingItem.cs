using Needs.Linq;
using System;

namespace Needs.Wl.Settings
{
    /// <summary>
    /// 系统参数配置内容
    /// </summary>
    [Serializable]
    public class AppSettingItem : IUnique
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public AppSettingItem()
        {

        }

        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 配置项ID
        /// </summary>
        public string AppSettingItemID { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 排序序号
        /// </summary>
        public int SortNo { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Summary { get; set; }
    }
}
