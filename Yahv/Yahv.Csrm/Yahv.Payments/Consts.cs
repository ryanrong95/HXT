using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace Yahv.Payments
{
    public class CodeValue<T>
    {
        public string Name { get; set; }
        public T Value { get; set; }
    }

    abstract public class CodeType<T> : IEnumerable<CodeValue<T>>
    {
        CodeValue<T>[] data;
        public CodeType()
        {
            this.data = this.GetType().GetFields().Where(item => item.IsStatic).Select(item => new CodeValue<T>
            {
                Name = item.Name,
                Value = (T)item.GetValue(this)
            }).ToArray();
        }

        virtual public IEnumerator<CodeValue<T>> GetEnumerator()
        {
            return this.data.Where(item => item.Name != "Default").Select(item => item).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    #region 科目名称
    /// <summary>
    /// 科目名称
    /// </summary>
    public class SubjectConsts : CodeType<string>
    {
        #region 代理费
        public const string 代理费 = nameof(代理费);
        #endregion

        #region 货款
        public const string 代付货款 = nameof(代付货款);
        public const string 代收货款 = nameof(代收货款);
        public const string 付汇 = nameof(付汇);
        public const string 供应商付汇 = nameof(供应商付汇);
        #endregion

        #region 税款
        public const string 关税 = nameof(关税);
        public const string 消费税 = nameof(消费税);
        public const string 海关增值税 = nameof(海关增值税);
        public const string 销售增值税 = nameof(销售增值税);
        #endregion

        #region 仓储费
        public const string 仓储费 = nameof(仓储费);
        #endregion

        #region 杂费
        public const string 商检费 = nameof(商检费);
        public const string 包车费 = nameof(包车费);
        public const string 提货费 = nameof(提货费);
        public const string 空车费 = nameof(空车费);

        public const string 清关费 = nameof(清关费);

        public const string 付汇手续费 = nameof(付汇手续费);
        public const string 付汇操作费 = nameof(付汇操作费);
        public const string 入仓费 = nameof(入仓费);
        public const string 隧道费 = nameof(隧道费);
        public const string 车场费 = nameof(车场费);
        public const string 登记费 = nameof(登记费);
        public const string 垫付运费 = nameof(垫付运费);
        public const string 承运商运费 = nameof(承运商运费);
        public const string 库位租赁费 = nameof(库位租赁费);
        public const string 超重费 = nameof(超重费);
        public const string 加急费 = nameof(加急费);
        public const string 等待费 = nameof(等待费);
        public const string 标签服务费 = nameof(标签服务费);
        //public const string 仓储费 = nameof(仓储费);
        public const string 包装费 = nameof(包装费);
        public const string 送货服务费 = nameof(送货服务费);
        public const string 收货服务费 = nameof(收货服务费);
        public const string 自提服务费 = nameof(自提服务费);
        public const string 快递运费 = nameof(快递运费);
        public const string 快递其他费用 = nameof(快递其他费用);

        public const string 其他 = nameof(其他);

        public const string 送货费 = nameof(送货费);
        public const string 快递费 = nameof(快递费);
        public const string 停车费 = nameof(停车费);
        public const string 收货异常费用 = nameof(收货异常费用);
        #endregion
    }
    #endregion

    /// <summary>
    /// 分类名称
    /// </summary>
    public class CatalogConsts : CodeType<string>
    {
        public const string 货款 = nameof(货款);
        public const string 税款 = nameof(税款);
        public const string 代理费 = nameof(代理费);
        public const string 杂费 = nameof(杂费);
        public const string 仓储服务费 = nameof(仓储服务费);

        public string[] this[string index]
        {
            get
            {
                switch (index)
                {
                    case ConductConsts.代仓储:
                        return new[] { 货款, 税款, 代理费, 仓储服务费, 杂费 };
                    case ConductConsts.代报关:
                        return new[] { 货款, 税款, 代理费, 仓储服务费, 杂费 };
                    case ConductConsts.传统贸易:
                        return new string[] { };
                    default:
                        throw new NotSupportedException($"不支持指定的内容:{index}!");
                }
            }
        }
    }

    /// <summary>
    /// 业务名称
    /// </summary>
    public class ConductConsts : CodeType<string>
    {
        public const string 代仓储 = nameof(代仓储);
        public const string 代报关 = nameof(代报关);
        public const string 传统贸易 = nameof(传统贸易);
        public const string 供应链 = nameof(供应链);

        /// <summary>
        /// 转换的示例
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        static public Business Convert(string txt)
        {
            switch (txt)
            {
                case 代仓储:
                    return Business.WarehouseServicing;
                case 传统贸易:
                    return Business.Trading;
                default:
                    throw new NotSupportedException($"不支持指定的内容:{txt}!");
            }
        }
    }

    public class SubjectCollection : IReadOnlyDictionary<string, IReadOnlyDictionary<string, string[]>>
    {
        SortedDictionary<string, SortedDictionary<string, string[]>> sorts;
        public SubjectCollection()
        {
            this.sorts = new SortedDictionary<string, SortedDictionary<string, string[]>>();
            //this.sorts[ConductConsts.代报关] = this.sorts[ConductConsts.代仓储] = this.sorts[ConductConsts.供应链] = this.GetCatalogAndSubject();
            this.sorts[ConductConsts.供应链] = this.GetCatalogAndSubject();
        }

        /// <summary>
        /// 获取所有科目
        /// </summary>
        /// <returns></returns>
        private SortedDictionary<string, string[]> GetCatalogAndSubject()
        {
            //return new SortedDictionary<string, string[]> {
            //    { CatalogConsts.货款 , new string[] { SubjectConsts.代付货款,SubjectConsts.代收货款, SubjectConsts.付汇, SubjectConsts.供应商付汇 } },
            //    { CatalogConsts.税款 , new string[] { SubjectConsts.关税, SubjectConsts.消费税, SubjectConsts.海关增值税 , SubjectConsts.销售增值税 } },
            //    { CatalogConsts.代理费 , new string[] { CatalogConsts .代理费 } },
            //    { CatalogConsts.杂费 , new string[]{ SubjectConsts.商检费,SubjectConsts.包车费,SubjectConsts.提货费,SubjectConsts.空车费,SubjectConsts.清关费,SubjectConsts.付汇手续费 ,SubjectConsts.付汇操作费 ,SubjectConsts.入仓费,SubjectConsts.隧道费,SubjectConsts.车场费,SubjectConsts.登记费,SubjectConsts.垫付运费,SubjectConsts.承运商运费 ,SubjectConsts.库位租赁费 ,SubjectConsts.超重费,SubjectConsts.加急费,SubjectConsts.等待费,SubjectConsts.标签服务费 ,SubjectConsts.仓储费,SubjectConsts.包装费,SubjectConsts.送货服务费 ,SubjectConsts.收货服务费 ,SubjectConsts.自提服务费 ,SubjectConsts.快递运费,SubjectConsts.快递其他费用, SubjectConsts.其它, } },
            //};

            using (var subjectsView = new SubjectsView())
            {
                var subjects =
                    subjectsView.Where(item => item.Type == SubjectType.Input && item.Conduct == ConductConsts.供应链).ToArray();


                return new SortedDictionary<string, string[]> {
                { CatalogConsts.货款 , subjects.Where(item=>item.Catalog==CatalogConsts.货款).Select(item=>item.Name).ToArray()},
                { CatalogConsts.税款 , subjects.Where(item=>item.Catalog==CatalogConsts.税款).Select(item=>item.Name).ToArray()},
                { CatalogConsts.代理费 ,subjects.Where(item=>item.Catalog==CatalogConsts.代理费).Select(item=>item.Name).ToArray()},
                { CatalogConsts.仓储服务费 ,subjects.Where(item=>item.Catalog==CatalogConsts.仓储服务费).Select(item=>item.Name).ToArray()},
                { CatalogConsts.杂费 , subjects.Where(item=>item.Catalog==CatalogConsts.杂费).Select(item=>item.Name).ToArray()}
            };
            }
        }

        private static SubjectCollection current;
        static object locker = new object();
        static public SubjectCollection Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new SubjectCollection();
                        }
                    }
                }

                return current;
            }
        }

        public IEnumerable<string> Keys
        {
            get
            {
                return this.sorts.Keys;
            }
        }

        public IEnumerable<IReadOnlyDictionary<string, string[]>> Values
        {
            get
            {
                return this.sorts.Values;
            }
        }

        public int Count
        {
            get
            {
                return this.sorts.Count;
            }
        }

        public IReadOnlyDictionary<string, string[]> this[string key]
        {
            get
            {
                return this.sorts[key];
            }
        }

        public bool ContainsKey(string key)
        {
            return this.sorts.ContainsKey(key);
        }

        public bool TryGetValue(string key, out IReadOnlyDictionary<string, string[]> value)
        {
            SortedDictionary<string, string[]> t;
            var rslt = this.sorts.TryGetValue(key, out t);
            value = t;
            return rslt;
        }

        public IEnumerator<KeyValuePair<string, IReadOnlyDictionary<string, string[]>>> GetEnumerator()
        {
            return this.sorts.Select(item => new KeyValuePair<string, IReadOnlyDictionary<string, string[]>>(item.Key, item.Value)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
