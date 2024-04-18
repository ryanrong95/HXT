using System;

namespace Yahv.Settings.Attributes
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
