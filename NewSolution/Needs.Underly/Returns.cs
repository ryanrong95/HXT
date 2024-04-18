using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Underly
{
    public class Address
    {
        public int ID { get; internal set; }
        public string Tel { get; internal set; }
        public string Name { get; internal set; }
        public string Context { get; internal set; }
        public District[] District { get; internal set; }
    }
    public class Returns
    {

        /// <summary>
        /// 退货地址
        /// </summary>
        Address[] db { get; set; }

        public Dictionary<int, Address> this[District index]
        {
            get
            {
                return this.db.Where(item => item.District.Contains(index)).ToDictionary(item => item.ID);
            }
        }

        internal Returns()
        {
            List<Address> list = new List<Address>();

            var di_rmb = Enum.GetValues(typeof(District))
                .Cast<District>().Where(item => item == District.CN).ToArray();
            var di_ors = Enum.GetValues(typeof(District))
                .Cast<District>().Where(item => item != District.CN).ToArray();

            list.Add(new Address
            {
                ID = list.Count,
                Tel = "010-62105503",
                Name = "北京退货点",
                District = di_rmb,
                Context = "北京市海淀区知春路108号豪景大厦C座1503室"
            });
            list.Add(new Address
            {
                ID = list.Count,
                Tel = "14753727868",
                Name = "深圳退货点",
                District = di_rmb,
                Context = "深圳市龙岗区吉华路393号英达丰科技园1号楼5楼"
            });
            list.Add(new Address
            {

                ID = list.Count,
                Tel = "00852-34264977",
                Name = "香港退货点",
                District = di_ors,
                Context = "觀塘鴻圖道16號志成工業大廈2/F B室。Unit B, 2/F.,Houtex Ind. Bldg., 16 Hung To Rd., Kwun Tong, Kowloon"
            });

            this.db = list.ToArray();
        }

        static object lockobj = new object();
        static Returns _instance = null;

        public static Returns Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobj)
                    {
                        if (_instance == null)
                        {
                            _instance = new Returns();
                        }
                    }
                }
                return _instance;
            }
        }


    }
}
