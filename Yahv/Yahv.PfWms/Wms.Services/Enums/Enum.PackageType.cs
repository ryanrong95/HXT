using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Wms.Services
{

    class MyClass
    {
        public MyClass()
        {
            foreach (var item in PackageTypes.Current)
            {

            }

        }
    }

    public class PackageTypes : IEnumerable<PackageType>
    {

        #region 包装种类代码表

        /// <summary>
        /// 散装
        /// </summary>
        static public readonly PackageType Bulk = new PackageType
        {
            ChineseName = "散装",
            GBCode = "00",
            OriginHSCode = "4",
            OriginHSName = "散装",
            OriginCiqCode = "9993",
            OriginCiqName = "散装"
        };

        /// <summary>
        /// 裸装
        /// </summary>
        static public readonly PackageType Nude = new PackageType
        {
            ChineseName = "裸装",
            GBCode = "01",
            OriginHSCode = "7",
            OriginHSName = "其它",
            OriginCiqCode = "9994",
            OriginCiqName = "裸装"
        };

        /// <summary>
        /// 球状罐类
        /// </summary>
        static public readonly PackageType SphericalTanks = new PackageType
        {
            ChineseName = "球状罐类",
            GBCode = "04",
            OriginHSCode = "7",
            OriginHSName = "其它",
            OriginCiqCode = "390",
            OriginCiqName = "其他罐"
        };

        /// <summary>
        /// 包/袋
        /// </summary>
        static public readonly PackageType Bag = new PackageType
        {
            ChineseName = "包/袋",
            GBCode = "06",
            OriginHSCode = "6",
            OriginHSName = "包",
            OriginCiqCode = "590",
            OriginCiqName = "包/袋类"
        };

        /// <summary>
        /// 纸制或纤维板制盒/箱
        /// </summary>
        static public readonly PackageType CartonBox = new PackageType
        {
            ChineseName = "纸制或纤维板制盒/箱",
            GBCode = "22",
            OriginHSCode = "2",
            OriginHSName = "纸箱",
            OriginCiqCode = "4M",
            OriginCiqName = "纸箱"
        };

        /// <summary>
        /// 木制或竹藤等植物性材料制盒/箱
        /// </summary>
        static public readonly PackageType WoodenBox = new PackageType
        {
            ChineseName = "木制或竹藤等植物性材料制盒/箱",
            GBCode = "23",
            OriginHSCode = "1",
            OriginHSName = "木箱",
            OriginCiqCode = "4C11",
            OriginCiqName = "木制箱"
        };

        /// <summary>
        /// 其他材料制盒/箱
        /// </summary>
        static public readonly PackageType OtherBox = new PackageType
        {
            ChineseName = "其他材料制盒/箱",
            GBCode = "29",
            OriginHSCode = "7",
            OriginHSName = "其它",
            OriginCiqCode = "490",
            OriginCiqName = "其他箱"
        };

        /// <summary>
        /// 纸制或纤维板制桶
        /// </summary>
        static public readonly PackageType FiberDrum = new PackageType
        {
            ChineseName = "纸制或纤维板制桶",
            GBCode = "32",
            OriginHSCode = "3",
            OriginHSName = "桶装",
            OriginCiqCode = "1G",
            OriginCiqName = "纤维圆桶"
        };

        /// <summary>
        /// 木制或竹藤等植物性材料制桶
        /// </summary>
        static public readonly PackageType WoodenBarrel = new PackageType
        {
            ChineseName = "木制或竹藤等植物性材料制桶",
            GBCode = "33",
            OriginHSCode = "3",
            OriginHSName = "桶装",
            OriginCiqCode = "1C",
            OriginCiqName = "木圆桶"
        };

        /// <summary>
        /// 其他材料制桶
        /// </summary>
        static public readonly PackageType OtherBarrels = new PackageType
        {
            ChineseName = "其他材料制桶",
            GBCode = "39",
            OriginHSCode = "3",
            OriginHSName = "桶装",
            OriginCiqCode = "190",
            OriginCiqName = "其他桶"
        };

        /// <summary>
        /// 再生木托
        /// </summary>
        static public readonly PackageType ReWoodBracket = new PackageType
        {
            ChineseName = "再生木托",
            GBCode = "92",
            OriginHSCode = "5",
            OriginHSName = "托盘",
            OriginCiqCode = "9F91",
            OriginCiqName = "再生木托"
        };

        /// <summary>
        /// 天然木托
        /// </summary>
        static public readonly PackageType NaturalWoodBracke = new PackageType
        {
            ChineseName = "天然木托",
            GBCode = "93",
            OriginHSCode = "5",
            OriginHSName = "托盘",
            OriginCiqCode = "9C91",
            OriginCiqName = "天然木托"
        };

        /// <summary>
        /// 植物性铺垫材料
        /// </summary>
        static public readonly PackageType PlantBedding = new PackageType
        {
            ChineseName = "植物性铺垫材料",
            GBCode = "98",
            OriginHSCode = "7",
            OriginHSName = "其它",
            OriginCiqCode = "9999",
            OriginCiqName = "其他"
        };

        /// <summary>
        /// 其他包装
        /// </summary>
        static public readonly PackageType OtherPackage = new PackageType
        {
            ChineseName = "其他包装",
            GBCode = "99",
            OriginHSCode = "7",
            OriginHSName = "其它",
            OriginCiqCode = "9999",
            OriginCiqName = "其他"
        };

        #endregion

        PackageType[] data;

        PackageTypes()
        {
            this.data = this.GetType().GetFields(BindingFlags.Static | BindingFlags.Public).Select(item =>
            {
                return item.GetValue(null) as PackageType;
            }).ToArray();
        }

        public IEnumerator<PackageType> GetEnumerator()
        {
            return this.data.AsEnumerable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }


        static object locker = new object();
        static PackageTypes current;
        static public PackageTypes Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new PackageTypes();
                        }
                    }
                }
                return current;
            }
        }

    }

    /// <summary>
    /// 包装类型
    /// </summary>
    public class PackageType
    {
        /// <summary>
        /// 构造器
        /// </summary>
        internal PackageType() { }

        /// <summary>
        /// 中文名称
        /// </summary>
        public string ChineseName { get; internal set; }

        /// <summary>
        /// 国标码
        /// </summary>
        public string GBCode { get; internal set; }

        /// <summary>
        /// 原报关代码
        /// </summary>
        public string OriginHSCode { get; internal set; }

        /// <summary>
        /// 原报关名称
        /// </summary>
        public string OriginHSName { get; internal set; }

        /// <summary>
        /// 原报检代码
        /// </summary>
        public string OriginCiqCode { get; internal set; }

        /// <summary>
        /// 原报检名称
        /// </summary>
        public string OriginCiqName { get; internal set; }
    }
}
