using System;
using System.Collections.Generic;

namespace Yahv.Finance.Services.Models
{
    /// <summary>
    /// 输入参数
    /// </summary>
    public class InputParam : InputParam<string>
    {
    }

    /// <summary>
    /// 输入参数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class InputParam<T> where T : class
    {
        public string Sender { get; set; }
        public string Option { get; set; }
        public T Model { get; set; }
    }

    /// <summary>
    /// 批量输入参数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class InputsParam<T> where T : class
    {
        public string Sender { get; set; }
        public string Option { get; set; }
        public IEnumerable<T> Model { get; set; }
    }
}