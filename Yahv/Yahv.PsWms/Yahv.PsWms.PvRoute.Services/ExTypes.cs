using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PsWms.PvRoute.Services
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
        public const string KY = nameof(KY);

        /// <summary>
        /// EMS
        /// </summary>
        ///<remarks>EMS</remarks>
        public const string EMS = nameof(EMS);

    }


    /// <summary>
    /// 顺丰快递类型
    /// </summary>
    public class SfExpType : CodeType
    {
        //标准文档多了174,221,233,234,238，还有一些名称发生变化（23，31,111,201），27,203没了

        public const int 顺丰标快 = 1;//有效
        public const int 顺丰标快_陆运  = 2;//有效
        public const int 顺丰次晨 = 5;//有效
        public const int 顺丰即日 = 6;//有效
        //public const int 国际小包平邮 = 9;//无效，使用该产品需签订指定专属协议
        //public const int 国际小包挂号 = 10;//无效，使用该产品需签订指定专属协议
        //public const int 国际特惠 = 23;//无效，付款方式不满足
        public const int 国际特惠_商家代理 = 27;//有效
        //public const int 国际电商专递_标准 = 29;//无效，使用该产品需签订指定专属协议
        public const int 便利箱产品 = 31;//有效
        public const int 岛内件_80CM = 33;//有效
        public const int 高铁站配 = 53;//有效
        public const int 顺丰特惠D = 111;//有效
        public const int 顺丰空配 = 112;//有效
        //public const int 整车直达 = 153;//无效，使用该产品需签订指定专属协议
        public const int 重货包裹 = 154;//有效
        public const int 标准零担 = 155;//有效
        //public const int 重货包裹B = 174;//无效，快件类型为空或未配置
        public const int 极速包裹 = 199;//有效
        public const int 冷运特惠 = 201;//有效
        //public const int 顺丰微小件 = 202;//无效，使用该产品需签订指定专属协议
        public const int 医药快运 = 203;//有效
        //public const int 特惠专配 = 208;//无效，使用该产品需签订指定专属协议
        public const int 高铁专送 = 209;//有效
        public const int 大票直送 = 215;//有效
        public const int 冷运 = 221;//有效
        public const int 精温专递 = 229;//有效
        public const int 陆运包裹 = 231;//有效
        public const int 精温专递_样本陆 = 233;//有效
        public const int 商务标快 = 234;//有效
        //public const int 极效前置 = 235;//无效，使用该产品需签订指定专属协议
        //public const int 纯重特配 = 238;//无效，使用该产品需签订指定专属协议
        //public const int 丰网速运 = 242;//无效，使用该产品需签订指定专属协议
        //public const int 同城即日 = 243;//无效，快件类型为空或未配置
        //public const int 电商标快 = 247;//无效，快件类型为空或未配置

        protected override Func<CodeValue<int>, bool> Filer()
        {
            ////int[] includs = new[] { 1, 2, 5, 6, 13, 18 };
            //int[] includs = new[] { 1, 2, 5, 6 };//顺丰快运类型表上没有13和18类型??
            //return item => item.Name != "Default" && includs.Contains((int)item.Value);

            return null;
        }
    }

    /// <summary>
    /// 跨越快递类型
    /// </summary>
    public class KysyExpType : CodeType
    {
        static public int Default = 次日达;

        public const int 当天达 = 10;//有效
        public const int 次日达 = 20;//有效
        public const int 隔日达 = 30;//有效
        public const int 陆运件 = 40;//有效
        public const int 同城次日 = 50;//有效
        //public const int 次晨达 = 60;//无效
        public const int 同城即日 = 70;//有效
        //public const int 航空件 = 80;//无效
        public const int 省内次日 = 160;//有效
        public const int 省内即日 = 170;//有效
        //public const int 空运 = 210;//无效
        //public const int 专运 = 220;//无效

        //public const int 当天达 = 1;
        //public const int 次日达 = 2;
        //public const int 隔日达 = 3;
        //public const int 同城即日 = 5;
        //public const int 同城次日 = 6;
        //public const int 陆运件 = 7;
        //public const int 省内次日 = 8;
        //public const int 省内即日 = 9;
        //public const int 空运 = 10;
        //public const int 专运 = 11;
        //public const int 次晨达 = 12;
        //public const int 航空件 = 13;
        //public const int 早班件 = 14;
        //public const int 中班件 = 15;
        //public const int 晚班件 = 16;


        protected override Func<CodeValue<int>, bool> Filer()
        {
            //int[] includs = new[] { 10, 20, 30, 70, 50, 40, 160, 170, 210, 220 };
            //return item => item.Name != "Default" && includs.Contains((int)item.Value);

            return null;
        }

    }
}
