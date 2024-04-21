using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Yahv.Finance.Services.Models.Rolls;
using Yahv.Finance.Services.Views.Origins;
using Yahv.Utils.Serializers;

namespace Yahv.Finance.Services.Views
{
    public class TaxRatesAlls : IDictionary<string, TaxRatio>
    {
        Dictionary<string, Models.Rolls.TaxRatio> data;

        #region 构造函数
        TaxRatesAlls()
        {
            using (var view = new TaxRatesOrigin())
            {
                var dic = this.data = view.Select(item => new
                {
                    item.Code,
                    item.JsonName, //还是允许修改一下
                    item.Name,
                    item.Rate
                }).ToArray().ToDictionary(item => item.JsonName, item => new Models.Rolls.TaxRatio
                {
                    Code = item.Code,
                    Name = item.Name,
                    Rate = item.Rate
                });
            }
        }
        #endregion

        #region 单例
        static object locker = new object();
        private static TaxRatesAlls current;
        public static TaxRatesAlls Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new TaxRatesAlls();
                        }
                    }
                }

                return current;
            }
        }
        #endregion

        #region IDictionary
        public IEnumerator<KeyValuePair<string, TaxRatio>> GetEnumerator()
        {
            return (IEnumerator<KeyValuePair<string, TaxRatio>>)this.data.Select(item => new KeyValuePair<string, TaxRatio>(item.Key, item.Value));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<string, TaxRatio> item)
        {
            this.data.Add(item.Key, new TaxRatio()
            {
                Name = item.Value.Name,
                Code = item.Value.Code,
                Rate = item.Value.Rate,
            });
        }

        public void Clear()
        {
            this.data.Clear();
        }

        public bool Contains(KeyValuePair<string, TaxRatio> item)
        {
            return this.data.ContainsKey(item.Key);
        }

        public void CopyTo(KeyValuePair<string, TaxRatio>[] array, int arrayIndex)
        {
            if (array == null && arrayIndex <= 0)
            {
                throw new Exception("array is empty");
            }

            this.data = array.ToDictionary(item => item.Key, item => new TaxRatio()
            {
                Name = item.Value.Name,
                Code = item.Value.Code,
                Rate = item.Value.Rate,
            });
        }

        public bool Remove(KeyValuePair<string, TaxRatio> item)
        {
            return this.data.Remove(item.Key);
        }

        public int Count
        {
            get { return this.data.Count; }
        }
        public bool IsReadOnly { get; }
        public bool ContainsKey(string key)
        {
            return this.data.ContainsKey(key);
        }

        public void Add(string key, TaxRatio value)
        {
            this.data.Add(key, value);
        }

        public bool Remove(string key)
        {
            return this.data.Remove(key);
        }

        public bool TryGetValue(string key, out TaxRatio value)
        {
            value = this.data[key];
            return ContainsKey(key);
        }

        public TaxRatio this[string key]
        {
            get { return this.data[key]; }
            set { throw new NotImplementedException(); }
        }

        public ICollection<string> Keys
        {
            get
            { return this.data.Select(item => item.Key).ToArray(); }
        }

        public ICollection<TaxRatio> Values
        {
            get { return this.data.Select(item => item.Value).ToArray(); }
        }

        #endregion

        #region Json
        /// <summary>
        /// json数据
        /// </summary>
        /// <returns></returns>
        public string Json()
        {
            return this.data.Json();
        }
        #endregion
    }

    #region _bak
    //public class _bak_TaxRatesAlls
    //{
    //    #region devloping

    //    Dictionary<string, Models.Rolls.TaxRatio> data;

    //    _bak_TaxRatesAlls()
    //    {
    //        using (var view = new TaxRatesOrigin())
    //        {
    //            var dic = this.data = view.Select(item => new
    //            {
    //                item.Code,
    //                item.JsonName, //还是允许修改一下
    //                item.Name,
    //                item.Rate
    //            }).ToArray().ToDictionary(item => item.JsonName, item => new Models.Rolls.TaxRatio
    //            {
    //                Code = item.Code,
    //                Name = item.Name,
    //                Rate = item.Rate
    //            });
    //        }
    //    }

    //    /// <summary>
    //    /// 索引器
    //    /// </summary>
    //    /// <param name="index">索引</param>
    //    /// <returns>指定税率信息</returns>
    //    public Models.Rolls.TaxRatio this[string index]
    //    {
    //        get { return data[index]; }
    //    }


    //    private static object locker = new object();
    //    static TaxRatesAlls _current;
    //    /// <summary>
    //    /// 单利
    //    /// </summary>
    //    static public TaxRatesAlls Current
    //    {
    //        get
    //        {
    //            if (_current == null)
    //            {
    //                lock (locker)
    //                {
    //                    if (_current == null)
    //                    {
    //                        _current = new TaxRatesAlls();
    //                    }
    //                }
    //            }
    //            return _current;
    //        }
    //    }


    //    /// <summary>
    //    /// json数据
    //    /// </summary>
    //    /// <returns></returns>
    //    public string Json()
    //    {
    //        return this.data.Json();
    //    }
    //    #endregion
    //} 
    #endregion
}
