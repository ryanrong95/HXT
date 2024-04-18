using System;

namespace Yahv.Underly.Attributes
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    class PackageAttribute : Attribute, IPackage
    {
        readonly string code;
        readonly string name;

        public PackageAttribute(string code, string name)
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
