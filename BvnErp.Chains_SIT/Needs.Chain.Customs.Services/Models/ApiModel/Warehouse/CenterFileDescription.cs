using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public enum FileDescriptionStatus
    {
        /// <summary>
        /// 待审批
        /// </summary>
        Audting = 100,

        /// <summary>
        /// 正常
        /// </summary>
        Normal = 200,

        /// <summary>
        /// 审批通过
        /// </summary>
        Approved = 300,

        /// <summary>
        /// 删除
        /// </summary>
        Delete = 400
    }

    public class CenterFileDescription : CenterFileMessage, IUnique
    {
        public string ID { get; set; }
        public DateTime? CreateDate { get; set; }
        public FileDescriptionStatus Status { get; set; }
    }

    public class CenterFile
    {
        public const string Web = "http://uuws.b1b.com/";
    }
}
