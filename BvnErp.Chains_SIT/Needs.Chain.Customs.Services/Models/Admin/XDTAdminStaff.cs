using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class XDTAdminStaff : IUnique
    {
        public string ID { get; set; }

        public string OriginID { get; set; }

        public string StaffID { get; set; }

        public string RealName { get; set; }

        public int Status { get; set; }

        public string DyjCode { get; set; }

        public string DyjCompanyCode { get; set; }

        public string DyjCompany { get; set; }

        public string DyjDepartmentCode { get; set; }

        public string DyjDepartment { get; set; }

        public string EntryCompany { get; set; }

        public string EnterpriseID { get; set; }

    }
}
