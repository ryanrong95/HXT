using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.Underly
{
    /// <summary>
    /// 原产地 调用者
    /// </summary>
    public class Origins
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
        public string this[Origin index]
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
        Origins()
        {
            var alls = Enum.GetValues(typeof(Origin)).Cast<Origin>().Select(item => item.GetOrigin());
            this.namings = new SortedDictionary<string, string>(alls.ToDictionary(item => item.ChineseName, item => item.Code));
            this.codes = new SortedDictionary<string, string>(alls.ToDictionary(item => item.Code, item => item.ChineseName));
            this.others = new SortedDictionary<int, string>(alls.ToDictionary(item => int.Parse(item.Code), item => item.ChineseName));
        }

        static Origins current;
        static object locker = new object();
        static public Origins Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new Origins();
                        }
                    }
                }

                return current;
            }
        }
    }
}
