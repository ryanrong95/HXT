using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Models.Generic
{
    public class AdminDossier
    {
        /// <summary>
        /// 管理员
        /// </summary>
        public AdminTop Admin
        {
            get;set;
        }

        /// <summary>
        /// 管理员管理的品牌
        /// </summary>
        public Company[] Manufactures
        {
            get;set;
        }
    }
}
