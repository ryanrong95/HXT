using System;

namespace Yahv.Underly.Attributes
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    class DistrictAttribute : Attribute, IDistrict
    {
        readonly string shortName;
        readonly string name;
        readonly string domain;
        readonly string showName;
        readonly string chineseName;

        public string ShortName { get { return this.shortName; } }

        public string Name { get { return this.name; } }

        public string Domain { get { return this.domain; } }
        public string ShowName { get { return this.showName; } }

        public string ChineseName
        {
            get { return this.chineseName; }
        }

        public DistrictAttribute(string shortName, string name, string domain, string showName, string chineseName)
        {
            this.shortName = shortName;
            this.name = name;
            this.domain = domain;
            this.showName = showName;
            this.chineseName = chineseName;
        }
    }

}
