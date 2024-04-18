using NtErp.Crm.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Models.Generic
{
    public class ProjectDossier
    {
        public Project Project
        {
            get; set;
        }

        public ProductItem[] Products { get; set; }

        
        public Industry[] Industries
        {
            get;set;
        }

        public Company[] Manufactures
        {
            get;set;
        }

        public AdminTop[] ClientAdmins
        {
            get;set;
        }
    }
}
