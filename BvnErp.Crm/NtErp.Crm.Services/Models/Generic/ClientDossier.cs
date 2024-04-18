using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Models.Generic
{
    public class ClientDossier
    {
        public Client Client
        {
            get; set;
        }

        /// <summary>
        /// 客户的销售
        /// </summary>
        public AdminTop[] Admins
        {
            get; set;
        }

        /// <summary>
        /// 客户的行业
        /// </summary>
        public Industry[] Industries
        {
            get; set;
        }

        /// <summary>
        /// 品牌 制造商
        /// </summary>
        public Company[] Manufactures
        {
            get; set;
        }
    }
}
