using System;

namespace Yahv.Underly.Attributes
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    class UnitAttribute : Attribute, IUnit
    {
        readonly string code;
        readonly string name;

        public UnitAttribute(string code, string name)
        {
            this.code = code;
            this.name = name;
        }

        public string Code
        {
            get { return this.code; }
        }

        public string Name
        {
            get { return this.name; }
        }
    }
}
