using Needs.Underly.Legals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Underly.Attributes
{

    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    class DistrictAttribute : LegalAttribute, IDistrict
    {
        readonly string shortName;
        readonly string name;
        readonly string domain;
        readonly string showName;

        public string ShortName { get { return this.shortName; } }

        public string Name { get { return this.name; } }

        public string Domain { get { return this.domain; } }
        public string ShowName { get { return this.showName; } }


        public DistrictAttribute(string shortName, string name, string domain, string showName)
        {
            this.shortName = shortName;
            this.name = name;
            this.domain = domain;
            this.showName = showName;
        }
    }

}
