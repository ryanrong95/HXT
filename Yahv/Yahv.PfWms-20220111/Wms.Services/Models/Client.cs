using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Linq.Persistence;

namespace Wms.Services.Models
{
    public class Client 
    {
        /// <summary>
        /// 编号
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 管理员编码
        /// </summary>
        public string AdminCode { get; set; }

        /// <summary>
        /// 法人
        /// </summary>
        public string Corporation { get; set; }

        /// <summary>
        /// 注册地址
        /// </summary>
        public string RegAddress { get; set; }

        /// <summary>
        /// 统一社会信用编码
        /// </summary>
        public string Uscc { get; set; }

        /// <summary>
        /// 等级/级别
        /// </summary>
        public int Grade { get; set; }

        /// <summary>
        /// 是否是VIP用户
        /// </summary>
        public bool Vip { get; set; }

        /// <summary>
        /// 入仓号
        /// </summary>
        public string EnterCode { get; set; }

        /// <summary>
        /// 海关编码
        /// </summary>
        public string CustomsCode { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// 添加人
        /// </summary>
        public string AdminID { get; set; }
    }
}
