using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.PsWms.DappApi.Services.Enums;

namespace Yahv.PsWms.DappApi.Services.Models
{
    public class Taker
    {
        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 接受任务的人 TakerType: Driver	司机 1, Picker 拿货人	2 一般拿货人
        /// </summary>
        public TakerType Type { get; set; }

        /// <summary>
        /// 受托人
        /// </summary>
        public bool IsTrustee { get; set; }

        /// <summary>
        /// 车牌
        /// </summary>
        public string Licence { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifyDate { get; set; }

        /// <summary>
        /// 枚举,正常 1, 删除 0,
        /// </summary>
        public Underly.GeneralStatus Status { get; set; }
    }
}
