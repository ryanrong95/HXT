using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Services.Models
{
    /// <summary>
    /// 子库房
    /// </summary>
    public class Plate : IUnique
    {
        public Plate()
        {

        }

        #region 属性
        /// <summary>
        /// 唯一标识
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 子库房名称
        /// </summary>

        public string Name { set; get; }
        /// <summary>
        /// 大库房ID
        /// </summary>
        public string EnterpriseID { set; get; }

        /// <summary>
        /// 库房编码 HK BJ
        /// </summary>
        /// <remarks>
        /// 应在收获地址中增加   Code
        /// </remarks>
        public string Code { set; get; }

        /// <summary>
        /// 具体地址
        /// </summary>
        public string Address { set; get; }


        /// <summary>
        /// 邮编
        /// </summary>
        public string PostZip { set; get; }


        #endregion
    }
}
