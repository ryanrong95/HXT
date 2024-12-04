using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yahv.Utils.Serializers;

namespace Yahv.Services
{
    public class WhEnterprise
    {
        public string ID { get; internal set; }
        public string Name { get; internal set; }
    }

    /// <summary>
    /// 门牌
    /// </summary>
    public class WhDoor
    {
        /// <summary>
        /// ID 也是 编号
        /// </summary>
        public string ID { get; internal set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; internal set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; internal set; }
        public WhEnterprise Enterprise { get; internal set; }
    }

    /// <summary>
    /// 库房
    /// </summary>
    /// <remarks>
    /// 库房编码规范[HK]
    /// 通知发送到的库房[HK][01] , 01就代表万路通门牌
    /// 以上可以用coding方式实践
    /// 
    /// 库位[HK][QQ][JJ][PP*]
    /// Q:库区：经[0-9A-Z]
    /// J 架号：纬[0-9]
    /// P 位置：立体[0-9]
    /// </remarks>
    public class WhSettings : WhDoor
    {
        /// <summary>
        /// 香港库房
        /// </summary>
        static public readonly WhSettings HK;
        /// <summary>
        /// 深圳库房
        /// </summary>
        static public readonly WhSettings SZ;
        /// <summary>
        /// 提货地址
        /// </summary>
        static public readonly WhSettings TH;

        static WhDoor[] allDoors;

        /// <summary>
        /// 全部的门牌
        /// </summary>
        /// <remarks>
        /// 代仓储（沈忱）前端需要，要不要限制一下只有他自行选择的？
        /// </remarks>
        static public IEnumerable<WhDoor> AllDoors
        {
            get
            {
                if (allDoors == null)
                {
                    allDoors = HK.Doors.Concat(SZ.Doors).ToArray();
                }
                return allDoors;
            }
        }

        static WhSettings[] allWhs;

        /// <summary>
        /// 全部的门牌
        /// </summary>
        /// <remarks>
        /// 代仓储（沈忱）前端需要，要不要限制一下只有他自行选择的？
        /// </remarks>
        static public IEnumerable<WhDoor> AllWhs
        {
            get
            {
                if (allWhs == null)
                {
                    allWhs = new WhSettings[] { HK, SZ };
                }
                return allWhs;
            }
        }




        static WhSettings()
        {
            /*
            香港畅运库房、香港万路通库房
            深圳创新恒远库房、深圳华芯通库房
            */

            #region 库房门牌处理

            var door1 = new WhDoor
            {
                ID = "HK01",
                Name = "香港速逹國際物流",
                Address = "香港九龙观塘成业街27号日昇中心1204室",
                Enterprise = new WhEnterprise
                {
                    ID = "8C7BF4F7F1DE9F69E1D96C96DAF6768E",
                    Name = "香港速逹國際物流有限公司"
                }
            };

            var door2 = new WhDoor
            {
                ID = "HK02",
                Name = "香港速逹國際物流1",
                Address = "香港九龙观塘成业街27号日昇中心1204室", //"香港九龍觀塘成業街27號日昇中心3樓318室",
                Enterprise = new WhEnterprise
                {
                    ID = "8C7BF4F7F1DE9F69E1D96C96DAF6768E2",
                    Name = "香港速逹國際物流有限公司"
                }
            };

            var door3 = new WhDoor
            {
                ID = "SZ01",
                Name = "深圳华芯通库房",
                Address = "深圳市龙华区龙华街道富康社区富康商业广场7号富康科技大厦12层1201-1202",
                Enterprise = new WhEnterprise
                {
                    ID = "DBAEAB43B47EB4299DD1D62F764E6B6A",
                    Name = "深圳市华芯通供应链管理有限公司"
                }
            };
            var door4 = new WhDoor
            {
                ID = "SZ02",
                Name = "深圳华芯通库房2",
                Address = "深圳市龙华区龙华街道富康社区富康商业广场7号富康科技大厦12层1201-1202",
                Enterprise = new WhEnterprise
                {
                    ID = "DBAEAB43B47EB4299DD1D62F764E6B6A2",
                    Name = "深圳市华芯通供应链管理有限公司"
                }
            };
            var door5 = new WhDoor
            {
                ID = "SZ03",
                Name = "深圳华芯通库房3",
                Address = "深圳市龙华区龙华街道富康社区富康商业广场7号富康科技大厦12层1201-1202",
                Enterprise = new WhEnterprise
                {
                    ID = "DBAEAB43B47EB4299DD1D62F764E6B6A3",
                    Name = "深圳市华芯通供应链管理有限公司"
                }
            };
            var door6 = new WhDoor
            {
                ID = "SZ04",
                Name = "深圳华芯通库房4",
                Address = "深圳市龙华区龙华街道富康社区富康商业广场7号富康科技大厦12层1201-1202",
                Enterprise = new WhEnterprise
                {
                    ID = "DBAEAB43B47EB4299DD1D62F764E6B6A4",
                    Name = "深圳市华芯通供应链管理有限公司"
                }
            };

            HK = new WhSettings
            {
                ID = "HK",
                Name = "香港库房",
                Doors = new WhDoor[] { door1, door2 },
            };
            SZ = new WhSettings
            {
                ID = "SZ",
                Name = "深圳库房",
                Doors = new WhDoor[] { door3, door4, door5 },
            };
            //提货地址
            TH = new WhSettings
            {
                ID = "TH",
                Name = "提货地址",
                Doors = new WhDoor[] { door6 },
            };

            #endregion

            #region 库房菜单处理

            //获得页面菜单数据
            var data = Encoding.UTF8.GetString(Properties.Resource.WhMenu).JsonTo<Menu[]>();

            //香港页面菜单
            HK.Menus = data.Where(item => item.Fors.Contains(nameof(HK))).ToArray();

            //深圳页面菜单
            SZ.Menus = data.Where(item => item.Fors.Contains(nameof(SZ))).ToArray();

            #endregion
        }
        public WhDoor this[int index]
        {
            get
            {
                if (index >= this.Doors.Length)
                {
                    return this.Doors.Last();
                }
                return this.Doors[index];
            }
        }
        public WhDoor this[string index]
        {
            get
            {
                return this.Doors.FirstOrDefault(item => item.ID == (index) || item.Name == index || item.Enterprise.Name == index);
            }
        }
        public WhDoor[] Doors { get; internal set; }
        /// <summary>
        /// 库房菜单
        /// </summary>
        public Menu[] Menus { get; private set; }
    }

    /// <summary>
    /// 合作人配置信息
    /// </summary>
    /// <remarks>
    /// 可以给接口使用
    /// 现在是真没有时间实现用数据库配置，我们后期实现吧。
    /// </remarks>
    public class WhPartners
    {
        Dictionary<string, WhEnterprise> indata;
        Dictionary<string, WhEnterprise> outdata;

        WhPartners()
        {
            var indata = this.indata = new Dictionary<string, WhEnterprise>();
            foreach (var item in WhSettings.SZ.Doors)
            {
                indata.Add(item.Enterprise.ID, WhSettings.HK["香港速逹國際物流"].Enterprise);
            }

            var outdata = this.outdata = new Dictionary<string, WhEnterprise>();
            foreach (var item in WhSettings.SZ.Doors)
            {
                outdata.Add(item.Enterprise.ID, WhSettings.HK["香港速逹國際物流1"].Enterprise);
            }
        }

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>合作伙伴的企业信息</returns>
        public WhEnterprise this[string index]
        {
            get
            {
                if (this == OutOrder)
                {
                    return this.outdata[index];
                }

                if (this == inorder)
                {
                    return this.indata[index];
                }

                throw new NotImplementedException("未实现引用，错误严重！");
            }
        }

        static object locker = new object();
        static WhPartners outorder;
        /// <summary>
        /// 外单引用
        /// </summary>
        static public WhPartners OutOrder
        {
            get
            {
                if (outorder == null)
                {
                    lock (locker)
                    {
                        if (outorder == null)
                        {
                            outorder = new WhPartners();
                        }
                    }
                }
                return outorder;
            }
        }

        static WhPartners inorder;
        /// <summary>
        /// 内单引用
        /// </summary>
        static public WhPartners InOrder
        {
            get
            {
                if (inorder == null)
                {
                    lock (locker)
                    {
                        if (inorder == null)
                        {
                            inorder = new WhPartners();
                        }
                    }
                }
                return inorder;
            }
        }
    }

    /// <summary>
    /// 菜单（主菜单）
    /// </summary>
    public class Menu
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 前台选中打开的左侧菜单
        /// </summary>
        public string openname { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string icon { get; set; }

        /// <summary>
        /// 子菜单
        /// </summary>
        public children[] children { get; set; }

        /// <summary>
        /// 属于HK还是SZ的权限
        /// </summary>
        public string[] Fors { get; set; }
    }

    /// <summary>
    /// 主菜单下的子菜单
    /// </summary>
    public class children
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 子菜单的选中
        /// </summary>
        public string activatname { get; set; }

        /// <summary>
        /// 对应的路由
        /// </summary>
        public string router { get; set; }
    }

    class MyClass
    {
        public MyClass()
        {

            //WhSettings.HK.

            // WhPartners.OutOrder["DBAEAB43B47EB4299DD1D62F764E6B6A"].ID
        }
    }
}
