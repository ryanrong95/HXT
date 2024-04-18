using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Underly
{
    public sealed class Languages
    {
        Dictionary<string, LangTerm> dic;
        /// <summary>
        /// 默认项目
        /// </summary>
        public LangTerm Default { get; private set; }

        Languages()
        {
            this.dic = new Dictionary<string, LangTerm>();
            this.dic.Add("zh-CN", new LangTerm
            {
                ShortName = "zh-CN",
                DisplayName = "简体中文(中国)",
                EnglishName = "Simplified Chinese",
                DataName = "zh"
            });
            this.dic.Add("zh-Hant", new LangTerm
            {
                ShortName = "zh-Hant",
                DisplayName = "繁体中文",
                EnglishName = "Chinese Traditional",
                DataName = "zh",
            });

            this.dic.Add("en", this.Default = new LangTerm
            {
                ShortName = "en",
                DisplayName = "English",
                EnglishName = "English",
                DataName = "en",
            });
            this.dic.Add("en-US", this.Default = new LangTerm
            {
                ShortName = "en-US",
                DisplayName = "English",
                EnglishName = "English",
                DataName = "en",
            });
           
        }

        public LangTerm this[string index]
        {
            get { return this.dic[index]; }
        }

        static Languages current;
        static object lockcurrent = new object();

        public static Languages Current
        {
            get
            {
                if (current == null)
                {
                    lock (lockcurrent)
                    {
                        if (current == null)
                        {
                            current = new Languages();
                        }
                    }
                }

                return current;
            }
        }
    }
}
