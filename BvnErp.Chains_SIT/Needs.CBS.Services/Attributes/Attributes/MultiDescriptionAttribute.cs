using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Cbs.Services
{
    /// <summary>
    /// 多值的枚举特性描述
    /// </summary>
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public class MultiDescriptionAttribute : Attribute
    {
        readonly AttributeItems contexts = new AttributeItems();

        public MultiDescriptionAttribute(string code, string text)
        {
            this.contexts.Add(code, text);
        }

        public MultiDescriptionAttribute(string code, string text,string englishText)
        {
            this.contexts.Add(code, text);
        }

        public AttributeItems Contexts
        {
            get { return contexts; }
        }
    }

    public class AttributeItems
    {
        public string Code { get; private set; }

        public string Text { get; private set; }

        public AttributeItems()
        {

        }

        public void Add(string code, string text)
        {
            this.Code = code;
            this.Text = text;
        }
    }
}
