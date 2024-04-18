using Needs.Underly.Attributes;
using Needs.Underly.Legals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Underly
{
    /// <summary>
    /// 法理
    /// </summary>
    /// <typeparam name="Tenum">指定枚举类型</typeparam>
    /// <typeparam name="Tattribute">指定属性</typeparam>
    [Obsolete("准备废弃")]
    class Legal<Tenum, Tattribute>
        where Tenum : struct, IComparable, IFormattable, IConvertible
        where Tattribute : LegalAttribute, ILegal
    {
        Dictionary<Tenum, Tattribute> dic;

        public Legal()
        {
            this.dic = new Dictionary<Tenum, Tattribute>();
        }

        public Tattribute this[Tenum index]
        {
            get
            {
                Tattribute value;
                if (!this.dic.TryGetValue(index, out value))
                {
                    lock (this)
                    {
                        if (!this.dic.TryGetValue(index, out value))
                        {
                            value = dic[index] = this.GetLegal(index);
                        }
                    }
                }

                return value;
            }
        }

        Tattribute GetLegal(object current)
        {
            string name = Enum.GetName(current.GetType(), current);
            if (name == null)
            {
                return null;
            }
            MemberInfo[] members = current.GetType().GetMember(name);
            if (members.Length != 1)
            {
                throw new NotSupportedException($"The member:{name} was not found");
            }
            Tattribute attribute = members.First().GetCustomAttribute<Tattribute>();
            if (attribute == null)
            {
                throw new NotSupportedException($"The attribute:{nameof(CurrenyAttribute)} was not found");
            }

            return attribute;
        }

        static Legal<Tenum, Tattribute> current;
        static object locker = new object();
        static public Legal<Tenum, Tattribute> Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new Legal<Tenum, Tattribute>();
                        }
                    }
                }
                return current;
            }
        }
    }
}
