using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Settings
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    sealed class LabelAttribute : Attribute
    {
        readonly string summary;

        public LabelAttribute(string summary)
        {
            this.summary = summary;
        }
  
        public string Summary
        {
            get { return this.summary; }
        }
    }
}
