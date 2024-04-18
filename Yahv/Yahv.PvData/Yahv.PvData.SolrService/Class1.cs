using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvData.SolrService
{


    [System.AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = true)]
    sealed class SolrFieldAttribute : Attribute
    {
        readonly string route = "";

        // This is a positional argument
        public SolrFieldAttribute(string route)
        {
            this.route = route;
            this.Version = "7.7.7.7";
        }


        public SolrFieldAttribute(Func<Property, bool> predicate)
        {

        }

        public string Route
        {
            get { return route; }
        }

        // This is a named argument
        public string Version { get; private set; }
    }

    public class vStandardProduct
    {
        [SolrField("Order.Items.Product")]
        public string ID { get; set; }

    }


    public class Property
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class Properties : IEnumerable<Property>
    {
        public Property this[string index]
        {
            get { return null; }
        }

        public IEnumerator<Property> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }


    [SolrField("tableName")]
    public class nsStandardProduct
    {
        public string ID { get; set; }

        //[SolrField(item => item.Name.Contains("PartNumber"))]
        public string PartNumber { get; set; }
    }


    class Tester1111
    {
        public Tester1111()
        {
            #region MyRegion

            var doc1 = new
            {
                ID = "1",
                PartNumber = "2",
                Manufacturer = "3",
                PackageCase = "4",
                Packaging = "5",
                CreateDate = DateTime.Now
            };

            var doc2 = new
            {
                ID = "2",
                PartNumber = "3",
                Manufacturer = "4",
                PackageCase = "5",
                Packaging = "6",
                CreateDate = DateTime.Now
            };

            var doc3 = new
            {
                ID = "2",
                product = new
                {
                    PartNumber = "3",
                },
                Manufacturer = "4",
                PackageCase = "5",
                Packaging = "6",
                CreateDate = DateTime.Now
            };

            #endregion

            var ppty = doc3.GetType().GetProperties().Where(item => item.Name == "PartNumber").Single();//通过一定的手段你可以拿到所有的字段
            ppty.GetValue(this, null);

            Console.WriteLine(doc1.GetType() == doc2.GetType()); //true 
            Console.WriteLine(doc1.GetType() == doc3.GetType()); //false 
        }
    }

}
