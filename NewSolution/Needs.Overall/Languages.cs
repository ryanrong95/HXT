using Needs.Overall.Models;
using Needs.Overall.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Needs.Overall
{
    /// <summary>
    /// 语言集合
    /// </summary>
    public sealed class Languages : IEnumerable<ILanguage>
    {
        List<Language> source;

        Languages()
        {
            this.source = new List<Language>();

            this.Ensure();
            this.Regular();
        }

        /// <summary>
        /// 默认
        /// </summary>
        Language Default
        {
            get
            {
                return this["zh-CN"] ?? new Language
                {
                    ShortName = "zh-CN",
                    DisplayName = "简体中文(中国)",
                    EnglishName = "Simplified Chinese",
                    DataName = "zh"
                };
            }
        }

        /// <summary>
        /// 索引
        /// </summary>
        /// <param name="index">Language.ID</param>
        /// <returns></returns>
        public Language this[string index]
        {
            get
            {
                return this.source.SingleOrDefault(item => item.ShortName.Equals(index, StringComparison.OrdinalIgnoreCase)
                    || item.EnglishName.Equals(index, StringComparison.OrdinalIgnoreCase)
                    || item.DisplayName.Equals(index, StringComparison.OrdinalIgnoreCase)
                    || item.DataName.Equals(index, StringComparison.OrdinalIgnoreCase));
            }
        }

        #region 数据源

        void Ensure()
        {
            //数据库的， 要去判断。不然无法保障，添加的计入，剔除的不管
            using (var adapter = Needs.Linq.Factory<Language, LanguagesView>.Current)
            {
                this.source = adapter.ToList();
            }
        }

        void Regular()
        {
            var t = new Thread(delegate ()
            {
                while (true)
                {
                    this.Ensure();
                    Thread.Sleep(1000);
                }
            })
            {
                IsBackground = true,
                Priority = ThreadPriority.Highest,
                Name = "languages source"
            };
            t.Start();
        }

        #endregion

        #region 单例

        static Languages current;
        static object locker = new object();

        static public Languages Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
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

        #endregion

        public IEnumerator<ILanguage> GetEnumerator()
        {
            return this.source.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

    }
}
