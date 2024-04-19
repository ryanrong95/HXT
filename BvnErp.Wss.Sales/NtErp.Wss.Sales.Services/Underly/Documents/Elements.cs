#define Convert
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NtErp.Wss.Sales.Services.Utils.Convertibles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Underly
{
    /// <summary>
    /// 文档元素
    /// </summary>
    public partial class Elements : ElementsBase, IDocument<Elements>
    {
        /// <summary>
        /// 当前元素总名称
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 赋值构造函数
        /// </summary>
        /// <param name="_value">设置的值</param>
        protected Elements(object value) : base(value)
        {

        }

        internal Elements(string name, object value) : base(value)
        {
            this.Name = name;
        }

        /// <summary>
        /// 获取当前值的类型
        /// </summary>
        /// <returns>当前值的类型</returns>
        public Type GetValueType()
        {
            if (this.Value == null)
            {
                return null;
            }

            return this.Value.GetType();
        }

        public override string ToString()
        {
            if (this.Value == null)
            {
                return $"{this.Name}:{this.GetType().FullName}[{this.Count}]";
            }

            return this.Value.ToString();
        }

        /// <summary>
        /// 密封的Equals
        /// </summary>
        /// <param name="obj">要与当前对象进行比较的对象</param>
        /// <returns>如果指定的对象等于当前对象，则为 true，否则为 false</returns>
        sealed public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        /// <summary>
        /// 密封的GetHashCode
        /// </summary>
        /// <returns>HashCode</returns>
        sealed public override int GetHashCode()
        {
            return base.GetHashCode();
        }


        #region 单例转换

        public static implicit operator Guid(Elements entity)
        {
            Type current = entity.Value.GetType();
            if (current == typeof(Guid))
            {
                return (Guid)entity.Value;
            }

            string input = entity.Value as string;
            Guid guid;
            if (!string.IsNullOrWhiteSpace(input) && Guid.TryParse(input, out guid))
            {
                return (Guid)(entity.Value = guid);
            }

            return new Guid();
        }
        public static implicit operator Elements(Guid v)
        {
            return new Elements(v);
        }

        public static implicit operator string(Elements entity)
        {
            if (entity.Value == null)
            {
                return null;
            }
            Type current = entity.Value.GetType();
            if (current == typeof(string))
            {
                return (string)entity.Value;
            }

            return null;
        }
        public static implicit operator Elements(string v)
        {
            return new Elements(v);
        }

        public static implicit operator decimal(Elements entity)
        {
#if Convert
            return Convert.ToDecimal(entity.Value);
#else
            return (decimal)entity.Value;
#endif

        }
        public static implicit operator Elements(decimal v)
        {
            return new Elements(v);
        }

        public static implicit operator int(Elements entity)
        {

#if Convert
            return Convert.ToInt32(entity.Value);
#else
            return (int)entity.Value;
#endif
        }
        public static implicit operator Elements(int v)
        {
            return new Elements(v);
        }

        public static implicit operator bool(Elements entity)
        {

#if Convert
            return Convert.ToBoolean(entity.Value);
#else
            return (bool)entity.Value;
#endif
        }
        public static implicit operator Elements(bool v)
        {
            return new Elements(v);
        }

        public static implicit operator byte(Elements entity)
        {

#if Convert
            return Convert.ToByte(entity.Value);
#else
            return (byte)entity.Value;
#endif
        }
        public static implicit operator Elements(byte v)
        {
            return new Elements(v);
        }

        public static implicit operator double(Elements entity)
        {
#if Convert
            return Convert.ToDouble(entity.Value);
#else
            return (double)entity.Value;
#endif
        }
        public static implicit operator Elements(double v)
        {
            return new Elements(v);
        }

        public static implicit operator float(Elements entity)
        {

#if Convert
            return Convert.ToSingle(entity.Value);
#else
            return (float)entity.Value;
#endif
        }
        public static implicit operator Elements(float v)
        {
            return new Elements(v);
        }

        public static implicit operator long(Elements entity)
        {
#if Convert
            return Convert.ToInt64(entity.Value);
#else
            return (long)entity.Value;
#endif
        }
        public static implicit operator Elements(long v)
        {
            return new Elements(v);
        }

        public static implicit operator short(Elements entity)
        {
#if Convert
            return Convert.ToInt16(entity.Value);
#else
            return (short)entity.Value;
#endif
        }
        public static implicit operator Elements(short v)
        {
            return new Elements(v);
        }

        public static implicit operator sbyte(Elements entity)
        {
#if Convert
            return Convert.ToSByte(entity.Value);
#else
            return (sbyte)entity.Value;
#endif
        }
        public static implicit operator Elements(sbyte v)
        {
            return new Elements(v);
        }

        public static implicit operator uint(Elements entity)
        {
#if Convert
            return Convert.ToUInt32(entity.Value);
#else
            return (uint)entity.Value;
#endif
        }
        public static implicit operator Elements(uint v)
        {
            return new Elements(v);
        }

        public static implicit operator ulong(Elements entity)
        {
#if Convert
            return Convert.ToUInt64(entity.Value);
#else
            return (ulong)entity.Value;
#endif
        }
        public static implicit operator Elements(ulong v)
        {
            return new Elements(v);
        }

        public static implicit operator ushort(Elements entity)
        {
#if Convert
            return Convert.ToUInt16(entity.Value);
#else
            return (ushort)entity.Value;
#endif
        }
        public static implicit operator Elements(ushort v)
        {
            return new Elements(v);
        }

        public static implicit operator DateTime(Elements entity)
        {
#if Convert
            return Convert.ToDateTime(entity.Value);
#else
            return (DateTime)entity.Value;
#endif
        }
        public static implicit operator Elements(DateTime v)
        {
            return new Elements(v);
        }

        public static implicit operator char(Elements entity)
        {
#if Convert
            return Convert.ToChar(entity.Value);
#else
            return (char)entity.Value;
#endif
        }
        public static implicit operator Elements(char v)
        {
            return new Elements(v);
        }

        #endregion

        #region 数组转换

        public static implicit operator int[] (Elements entity)
        {
            if (entity.Value is double[])
            {
                return entity.Value as int[];
            }

            Type currentType = typeof(int);

            if (entity.Value is IEnumerable)
            {
                ArrayList arryList = new ArrayList();
                foreach (var item in entity.Value as IEnumerable)
                {
                    arryList.Add(item.ChangeType(currentType));
                }

                return (entity.Value = arryList.ToArray(currentType)) as int[];
            }
            else
            {
                return (entity.Value = new int[] { (int)entity.Value.ChangeType(currentType) }) as int[];
            }
        }
        public static implicit operator Elements(int[] v)
        {
            return new Elements(v);
        }

        public static implicit operator string[] (Elements entity)
        {
            if (entity.Value is double[])
            {
                return entity.Value as string[];
            }

            Type currentType = typeof(string);

            if (entity.Value is IEnumerable)
            {
                ArrayList arryList = new ArrayList();
                foreach (var item in entity.Value as IEnumerable)
                {
                    arryList.Add(item.ChangeType(currentType));
                }

                return (entity.Value = arryList.ToArray(currentType)) as string[];
            }
            else
            {
                return (entity.Value = new string[] { (string)entity.Value.ChangeType(currentType) }) as string[];
            }
        }
        public static implicit operator Elements(string[] v)
        {
            return new Elements(v);
        }

        public static implicit operator double[] (Elements entity)
        {
            if (entity.Value is double[])
            {
                return entity.Value as double[];
            }

            Type currentType = typeof(double);

            if (entity.Value is IEnumerable)
            {
                ArrayList arryList = new ArrayList();
                foreach (var item in entity.Value as IEnumerable)
                {
                    arryList.Add(item.ChangeType(currentType));
                }

                return (entity.Value = arryList.ToArray(currentType)) as double[];
            }
            else
            {
                return (entity.Value = new double[] { (double)entity.Value.ChangeType(currentType) }) as double[];
            }
        }
        public static implicit operator Elements(double[] v)
        {
            return new Elements(v);
        }

        public static implicit operator decimal[] (Elements entity)
        {
            if (entity.Value is double[])
            {
                return entity.Value as decimal[];
            }

            Type currentType = typeof(decimal);

            if (entity.Value is IEnumerable)
            {
                ArrayList arryList = new ArrayList();
                foreach (var item in entity.Value as IEnumerable)
                {
                    arryList.Add(item.ChangeType(currentType));
                }

                return (entity.Value = arryList.ToArray(currentType)) as decimal[];
            }
            else
            {
                return (entity.Value = new decimal[] { (decimal)entity.Value.ChangeType(currentType) }) as decimal[];
            }
        }
        public static implicit operator Elements(decimal[] v)
        {
            return new Elements(v);
        }

        public static implicit operator bool[] (Elements entity)
        {
            if (entity.Value is bool[])
            {
                return entity.Value as bool[];
            }

            Type currentType = typeof(bool);

            if (entity.Value is IEnumerable)
            {
                ArrayList arryList = new ArrayList();
                foreach (var item in entity.Value as IEnumerable)
                {
                    arryList.Add(item.ChangeType(currentType));
                }

                return (entity.Value = arryList.ToArray(currentType)) as bool[];
            }
            else
            {
                return (entity.Value = new bool[] { (bool)entity.Value.ChangeType(currentType) }) as bool[];
            }
        }
        public static implicit operator Elements(bool[] v)
        {
            return new Elements(v);
        }

        public static implicit operator DateTime[] (Elements entity)
        {
            if (entity.Value is DateTime[])
            {
                return entity.Value as DateTime[];
            }

            Type currentType = typeof(DateTime);

            if (entity.Value is IEnumerable)
            {
                ArrayList arryList = new ArrayList();
                foreach (var item in entity.Value as IEnumerable)
                {
                    arryList.Add(item.ChangeType(currentType));
                }

                return (entity.Value = arryList.ToArray(currentType)) as DateTime[];
            }
            else
            {
                return (entity.Value = new DateTime[] { (DateTime)entity.Value.ChangeType(currentType) }) as DateTime[];
            }
        }
        public static implicit operator Elements(DateTime[] v)
        {
            return new Elements(v);
        }

        #endregion

        #region 集合转换

        public static implicit operator List<int>(Elements entity)
        {
            if (entity.Value is List<int>)
            {
                return entity.Value as List<int>;
            }

            Type currentType = typeof(int);
            List<int> list = new List<int>();
            if (entity.Value is IEnumerable)
            {
                foreach (var item in entity.Value as IEnumerable)
                {
                    list.Add((int)item.ChangeType(currentType));
                }
            }
            else
            {
                list.Add((int)entity.Value.ChangeType(currentType));
            }
            entity.Value = list;
            return list;
        }
        public static implicit operator Elements(List<int> v)
        {
            return new Elements(v);
        }

        public static implicit operator List<string>(Elements entity)
        {
            if (entity.Value is List<string>)
            {
                return entity.Value as List<string>;
            }

            Type currentType = typeof(string);
            List<string> list = new List<string>();
            if (entity.Value is IEnumerable)
            {
                foreach (var item in entity.Value as IEnumerable)
                {
                    list.Add((string)item.ChangeType(currentType));
                }
            }
            else
            {
                list.Add((string)entity.Value.ChangeType(currentType));
            }
            entity.Value = list;
            return list;
        }
        public static implicit operator Elements(List<string> v)
        {
            return new Elements(v);
        }

        public static implicit operator List<double>(Elements entity)
        {
            if (entity.Value is List<double>)
            {
                return entity.Value as List<double>;
            }

            Type currentType = typeof(double);
            List<double> list = new List<double>();
            if (entity.Value is IEnumerable)
            {
                foreach (var item in entity.Value as IEnumerable)
                {
                    list.Add((double)item.ChangeType(currentType));
                }
            }
            else
            {
                list.Add((double)entity.Value.ChangeType(currentType));
            }
            entity.Value = list;
            return list;
        }
        public static implicit operator Elements(List<double> v)
        {
            return new Elements(v);
        }

        public static implicit operator List<decimal>(Elements entity)
        {
            if (entity.Value is List<decimal>)
            {
                return entity.Value as List<decimal>;
            }

            Type currentType = typeof(decimal);
            List<decimal> list = new List<decimal>();
            if (entity.Value is IEnumerable)
            {
                foreach (var item in entity.Value as IEnumerable)
                {
                    list.Add((decimal)item.ChangeType(currentType));
                }
            }
            else
            {
                list.Add((decimal)entity.Value.ChangeType(currentType));
            }
            entity.Value = list;
            return list;
        }
        public static implicit operator Elements(List<decimal> v)
        {
            return new Elements(v);
        }

        public static implicit operator List<bool>(Elements entity)
        {
            if (entity.Value is List<bool>)
            {
                return entity.Value as List<bool>;
            }

            Type currentType = typeof(bool);
            List<bool> list = new List<bool>();
            if (entity.Value is IEnumerable)
            {
                foreach (var item in entity.Value as IEnumerable)
                {
                    list.Add((bool)item.ChangeType(currentType));
                }
            }
            else
            {
                list.Add((bool)entity.Value.ChangeType(currentType));
            }
            entity.Value = list;
            return list;
        }
        public static implicit operator Elements(List<bool> v)
        {
            return new Elements(v);
        }

        public static implicit operator List<DateTime>(Elements entity)
        {
            if (entity.Value is List<DateTime>)
            {
                return entity.Value as List<DateTime>;
            }

            Type currentType = typeof(DateTime);
            List<DateTime> list = new List<DateTime>();
            if (entity.Value is IEnumerable)
            {
                foreach (var item in entity.Value as IEnumerable)
                {
                    list.Add((DateTime)item.ChangeType(currentType));
                }
            }
            else
            {
                list.Add((DateTime)entity.Value.ChangeType(currentType));
            }
            entity.Value = list;
            return list;
        }
        public static implicit operator Elements(List<DateTime> v)
        {
            return new Elements(v);
        }

        #endregion
        #region Document转换 JZF 20180511
        public static implicit operator Document[] (Elements entity)
        {
            if (entity.Value is Document[])
            {
                return entity.Value as Document[];
            }
            else
            {
                return null;
            }
        }
        public static implicit operator Elements(Document[] v)
        {
            return new Elements(v);
        }
        public static implicit operator Document (Elements entity)
        {
            if (entity.Value is Document)
            {
                return entity.Value as Document;
            }
            else
            {
                return null;
            }
        }
        public static implicit operator Elements(Document v)
        {
            return new Elements(v);
        }
        #endregion
    }
}
