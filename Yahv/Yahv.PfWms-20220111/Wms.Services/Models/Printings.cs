using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;

namespace Wms.Services.Models
{
    /// <summary>
    /// 打印模板
    /// </summary>
    public class Printings : IUnique
    {
        /// <summary>
        /// 编号
        /// </summary>     
        public string ID { get; internal set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public PrintingType Type { get; set; }

        public string TypeDes
        {
            get
            {
                return this.Type.GetDescription();
            }
        }
        /// <summary>
        /// 模板地址
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 宽度
        /// </summary>
        public int? Width { get; set; }
        /// <summary>
        /// 高度
        /// </summary>
        public int? Height { get; set; }
        /// <summary>
        /// 摘要
        /// </summary>
        public string Summary { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public PrintingStatus Status { get; set; }


        public string StatusDes {get{ return this.Status.GetDescription(); } }

    }
}
