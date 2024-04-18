using Needs.Utils.Converters;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Needs.Underly
{
    public class StandardProduct// : IUnique, INaming, IStandardProduct, IProduct
    {
        Document properties;

        public StandardProduct()
        {
            this.properties = new Document();
        }

        public Document Properties
        {
            get
            {
                return this.properties;
            }
            set
            {
                this.properties = value;
            }
        }

        public dynamic this[string index]
        {
            get { return this.properties[index]; }
            set { this.properties[index] = value; }
        }

        virtual public string ID
        {
            get
            {
                return this.Prefix + this.GetFroms().MD5();
            }
            set
            {
                this[nameof(this.ID)] = value;
            }
        }

        public string Origin
        {
            get { return this[nameof(this.Origin)]; }
            set { this[nameof(this.Origin)] = value; }
        }

        public string Supplier
        {
            get { return this[nameof(this.Supplier)]; }
            set { this[nameof(this.Supplier)] = value; }
        }

        public string Name
        {
            get { return this[nameof(this.Name)]; }
            set { this[nameof(this.Name)] = value; }
        }

        public string Manufacturer
        {
            get { return this[nameof(this.Manufacturer)]; }
            set { this[nameof(this.Manufacturer)] = value; }
        }

        virtual protected string[] GetFroms()
        {
            return new[] { this.Name, this.Supplier, this.Origin, this.Manufacturer };
        }

        virtual protected string Prefix
        {
            get
            {
                return "SPR";
            }
        }
    }
}
