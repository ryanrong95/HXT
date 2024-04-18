using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv
{
    public class PositionPrinter
    {

        #region 属性
        public Config config { get; set; }

        /// <summary>
        /// 库位编号
        /// </summary>
        public string PositionID { get; set; }

        /// <summary>
        /// 所属库区
        /// </summary>
        public string RegionName { get; set; }

        /// <summary>
        /// 所属货架
        /// </summary>
        public string ShelveName { get; set; }

        /// <summary>
        /// 位置
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// 子码
        /// </summary>
        public string ChildrenCode { get; set; }

        /// <summary>
        /// 负责人
        /// </summary>
        public string ManagerID { get; set; }

        /// <summary>
        /// 所有人
        /// </summary>
        public string OwnerID { get; set; }

        /// <summary>
        /// 打印机
        /// </summary>
        public string Printer { get; set; }

        #endregion

        public void Print(PositionPrinter position)
        {

        }
    }

    public class Config
    {

        /// <summary>
        /// 地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 宽
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// 高
        /// </summary>
        public int Height { get; set; }
    }
}
