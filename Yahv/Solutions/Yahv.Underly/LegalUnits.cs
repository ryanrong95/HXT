using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.Underly
{
    /// <summary>
    /// 法定单位 调用者
    /// </summary>
    public class LegalUnits
    {
        SortedDictionary<string, string> namings;
        SortedDictionary<string, string> codes;
        SortedDictionary<int, string> others;

        /// <summary>
        /// 字符索引器
        /// </summary>
        /// <param name="index">字符索引</param>
        /// <returns>索引值</returns>
        public string this[string index]
        {
            get
            {
                string value;
                if (this.namings.TryGetValue(index, out value))
                {
                    return value;
                }

                if (this.codes.TryGetValue(index, out value))
                {
                    return value;
                }

                return value;
            }
        }

        /// <summary>
        /// 数字索引器
        /// </summary>
        /// <param name="index">数字索引</param>
        /// <returns>索引值</returns>
        public string this[int index]
        {
            get
            {
                string value;
                if (this.others.TryGetValue(index, out value))
                {
                    return value;
                }

                return value;
            }
        }

        /// <summary>
        /// 索引器 纯演示可以不实用
        /// </summary>
        /// <param name="index">枚举索引</param>
        /// <returns>索引值</returns>
        public string this[LegalUnit index]
        {
            get
            {
                int key = (int)index;
                string value;
                if (this.others.TryGetValue(key, out value))
                {
                    return value;
                }

                return value;
            }
        }

        /// <summary>
        /// 私有构造器
        /// </summary>
        LegalUnits()
        {
            var alls = Enum.GetValues(typeof(LegalUnit)).Cast<LegalUnit>().Select(item => item.GetUnit());
            this.namings = new SortedDictionary<string, string>(alls.ToDictionary(item => item.Name, item => item.Code));
            this.codes = new SortedDictionary<string, string>(alls.ToDictionary(item => item.Code, item => item.Name));
            this.others = new SortedDictionary<int, string>(alls.ToDictionary(item => int.Parse(item.Code), item => item.Name));
        }

        static LegalUnits current;
        static object locker = new object();
        static public LegalUnits Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new LegalUnits();
                        }
                    }
                }

                return current;
            }
        }
    }
}
