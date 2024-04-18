using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Utils.Descriptions
{
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
    sealed public class DescriptionAttribute : Attribute
    {
        readonly string[] contexts;

        public DescriptionAttribute(string context)
        {
            this.contexts = new[] { context };
        }

        public DescriptionAttribute(params string[] arry)
        {
            this.contexts = arry;
        }

        public string Context
        {
            get { return contexts[0]; }
        }

        public string[] Contexts
        {
            get { return contexts; }
        }
    }
}
