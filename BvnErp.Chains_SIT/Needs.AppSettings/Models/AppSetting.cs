using Needs.Linq;
using System;
using System.Linq;

namespace Needs.Wl.Settings
{
    /// <summary>
    /// 系统参数配置
    /// </summary>
    [Serializable]
    public class AppSetting : IUnique
    {
        public AppSetting()
        {

        }

        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 配置名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 是否多值
        /// </summary>
        public bool IsMultiValue { get; set; }

        /// <summary>
        /// 是否复合值
        /// </summary>
        public bool IsComplex { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Summary { get; set; }

        public AppSettingItems Items
        {
            get; set;
        }

        /// <summary>
        /// 获取配置项
        /// </summary>
        public string Value
        {
            get
            {
                if (this.IsMultiValue)
                {
                    throw new Exception("多值配置项目，不可直接获取Value");
                }

                return this.Items.FirstOrDefault().Value;
            }
        }
    }
}
