using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// Icgoo 推送卡板数结构
    /// </summary>
    public class PalletNumberJson
    {
        /// <summary>
        /// 库房
        /// </summary>
        public string Stock { get; set; }
        /// <summary>
        /// 卡板数
        /// </summary>
        public string Pallet { get; set; }
        /// <summary>
        /// 申报日期
        /// </summary>
        public string DeclareDate { get; set; }
    }
}
