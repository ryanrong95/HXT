using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Underly
{
    /// <summary>
    /// 语言总调用者
    /// </summary>
    public sealed class Languages : NameTargetConllection<NtErp.Wss.Sales.Services.Underly.Translators.Language>
    {
        /// <summary>
        /// 默认项目
        /// </summary>
        public Underly.Translators.Language Default { get; private set; }

        public Languages()
        {
            this["zh-CN"] = this.Default = new Underly.Translators.Language
            {
                ShortName = "zh-CN",
                DisplayName = "简体中文(中国)",
                EnglishName = "Simplified Chinese",
                DataName = "zh"
            };
            this["en-US"] = new Translators.Language
            {
                ShortName = "en-US",
                DisplayName = "English",
                EnglishName = "English",
                DataName = "en",
            };
            this["zh-Hant"] = new Translators.Language
            {
                ShortName = "zh-Hant",
                DisplayName = "繁体中文",
                EnglishName = "Chinese Traditional",
                DataName = "zh",
            };
        }
    }
}
