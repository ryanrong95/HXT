using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CnslApp
{
    class Class1
    {
        public Class1()
        {

        }


        void Subs()
        {
            using (new Needs.Linq.LinqContext())
            {
                foreach (var item in Needs.Overall.Devlopers.Currents)
                {
                    Console.WriteLine(item.ID);
                }
            }
        }


        public void test()
        {
            using (new Needs.Linq.LinqContext())
            {
                foreach (var item in Needs.Overall.Devlopers.Currents)
                {
                    //this.Subs();
                    Console.WriteLine(item.ID);
                }
            }
        }
    }
}
