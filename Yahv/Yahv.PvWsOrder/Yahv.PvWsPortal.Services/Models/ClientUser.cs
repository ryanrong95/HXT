using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace Yahv.PvWsPortal.Services.Models
{
    public class ClientUser : IUnique
    {
        /// <summary>
        /// Users 表的 ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// ClientID
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string OpenID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsMain { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string AdminID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Summary { get; set; }
    }
}
