
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Utils.Serializers;

namespace Yahv.Utils.Kdn
{

    public class pccArea
    {
        public string n { get; set; }
        public pccArea[] s { get; set; }

        /// <summary>
        /// 索引子项
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>子项</returns>
        public pccArea this[string index]
        {
            get { return s.Where(item => item.n == index).SingleOrDefault(); }
        }

        public override string ToString()
        {
            return $"{this.n} {this.s}";
        }
    }


    public class pccAreas : IEnumerable<pccArea>
    {
        pccArea[] data;

        public pccAreas()
        {
            this.data = Resource.area_data.JsonTo<pccArea[]>();
        }

        /// <summary>
        /// 索引子项
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>子项</returns>
        public pccArea this[string index]
        {
            get { return this.data.Where(item => item.n == index).SingleOrDefault(); }
        }

        public int Length
        {
            get { return this.data.Length; }
        }

        public IEnumerator<pccArea> GetEnumerator()
        {
            return data.Select(item => item).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        static pccAreas current;
        static object locker = new object();
        public static pccAreas Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new pccAreas();
                        }
                    }
                }
                return current;
            }
        }
    }
}
