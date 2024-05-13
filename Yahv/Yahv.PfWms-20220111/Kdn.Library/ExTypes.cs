using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kdn.Library
{
    /// <summary>
    /// 编码与值
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    public class CodeValue<T>
    {
        /// <summary>
        /// 命名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public T Value { get; set; }

        public CodeValue(string name, T value)
        {
            this.Name = name;
            this.Value = value;
        }

        public CodeValue()
        {

        }
    }

    /// <summary>
    /// 抽象编码类型
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    abstract public class CodeType<T> : IEnumerable<CodeValue<T>>
    {
        protected CodeValue<T>[] data;

        /// <summary>
        /// 构造器
        /// </summary>
        public CodeType()
        {
            this.data = this.GetType().GetFields().Where(item => item.IsStatic).Select(item => new CodeValue<T>
            {
                Name = item.Name,
                Value = (T)item.GetValue(this)
            }).ToArray();
        }

        /// <summary>
        /// 可枚举
        /// </summary>
        /// <returns></returns>
        public IEnumerator<CodeValue<T>> GetEnumerator()
        {
            return this.data.Where(this.Filer()).Where(item => item.Name != "Default").Select(item => item).GetEnumerator();
        }

        /// <summary>
        /// 获取名称
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>名称</returns>
        public string this[T index]
        {
            get { return this.Single(item => item.Value.Equals(index)).Name; }
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// 过滤器
        /// </summary>
        /// <returns>返回过滤条件</returns>
        virtual protected Func<CodeValue<T>, bool> Filer()
        {
            return item => true;
        }
    }

    /// <summary>
    /// 编码默认为值为整型类型
    /// </summary>
    abstract public class CodeType : CodeType<int>
    {

    }

    /// <summary>
    /// 快递商编号
    /// </summary>
    public class ShipperCode : CodeType<string>
    {
        /// <summary>
        /// 顺丰速运
        /// </summary>
        /// <remarks>顺丰</remarks>
        public const string SF = nameof(SF);
        /// <summary>
        /// 跨越速运
        /// </summary>
        /// <remarks>跨越</remarks>
        public const string KYSY = nameof(KYSY);
        /// <summary>
        /// EMS
        /// </summary>
        public const string EMS = nameof(EMS);
    }


    /// <summary>
    /// 顺丰快递类型
    /// </summary>
    public class SfExpType : CodeType
    {
        static public int Default = 电商专配;

        public const int 顺丰标快 = 1;
        public const int 顺丰特惠 = 2;
        public const int 电商特惠 = 3;
        public const int 四日件 = 4;
        public const int 顺丰次晨 = 5;
        public const int 顺丰即日 = 6;
        public const int 电商速配 = 7;
        public const int 医药常温 = 11;
        public const int 医药温控 = 12;
        public const int 物流普运 = 13;
        public const int 冷运到家 = 14;
        public const int 生鲜速配 = 15;
        public const int 大闸蟹专递 = 16;
        public const int 汽配吉运 = 17;
        public const int 重货快运 = 18;
        public const int 行邮专列 = 20;
        public const int 医药专运_常温 = 21;
        public const int 医药专运_温控 = 22;
        public const int 电商专配 = 28;
        public const int 即日2200 = 34;
        public const int 物资配送 = 35;
        public const int 汇票专送 = 36;
        public const int 证照专递产品 = 110;
        public const int 顺丰空配 = 112;
        public const int 专线普运 = 125;
        public const int 夜配 = 134;
        public const int 重货包裹 = 154;
        public const int 小票零担 = 155;
        public const int 医药常温_陆 = 195;
        public const int 顺丰微小件 = 202;
        public const int 医药快运 = 203;
        public const int 陆运微小件 = 204;
        public const int 特惠专配 = 208;

        protected override Func<CodeValue<int>, bool> Filer()
        {
            int[] includs = new[] { 1, 2, 5, 6, 13, 18 };
            return item => item.Name != "Default" && includs.Contains((int)item.Value);
        }
    }

    /// <summary>
    /// 跨越快递类型
    /// </summary>
    public class KysyExpType : CodeType
    {
        static public int Default = 次日达;

        public const int 当天达 = 1;
        public const int 次日达 = 2;
        public const int 隔日达 = 3;
        public const int 同城即日 = 5;
        public const int 同城次日 = 6;
        public const int 陆运件 = 7;
        public const int 省内次日 = 8;
        public const int 省内即日 = 9;
        public const int 空运 = 10;
        public const int 专运 = 11;
        public const int 次晨达 = 12;
        public const int 航空件 = 13;
        public const int 早班件 = 14;
        public const int 中班件 = 15;
        public const int 晚班件 = 16;

        protected override Func<CodeValue<int>, bool> Filer()
        {
            int[] includs = new[] { 1, 2, 3, 5, 6, 7, 8, 9, 10, 11 };
            return item => item.Name != "Default" && includs.Contains((int)item.Value);
        }

    }

    /// <summary>
    /// EMS快递类型
    /// </summary>
    public class EmsExpType : CodeType
    {
        static public int Default = 标准快递;

        public const int 标准快递 = 1;

        protected override Func<CodeValue<int>, bool> Filer()
        {
            return item => item.Name != "Default";
        }
    }
}
