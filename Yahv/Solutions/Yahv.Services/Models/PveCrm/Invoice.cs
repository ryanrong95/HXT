using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Services.Models.PveCrm
{
    public class Invoice : IUnique
    {
        public Invoice()
        {
        }

        public string ID { get; set; }

        public string EnterpriseID { get; set; }

        public string Address { get; set; }

        public string Tel { get; set; }

        public string Bank { get; set; }

        public string Account { get; set; }

        public DataStatus Status { get; set; }

        public DateTime CreateDate { get; set; }

        public string CreatorID { get; set; }

        public string Source { get; set; }

        public string Title { get; set; }

        #region 纯粹为了兼容原来的PvbCrm的命名，让其他使用公共视图的系统少一点代码改动

        public string BankAddress { get; set; }

        public string Name { get; set; }

        public string Mobile { get; set; }

        public string Email { get; set; }

        public string District { get; set; }

        public string DistrictDesc { get; set; }

        public string Postzip { get; set; }

        public string EnterpriseName { get; set; }

        public string TaxperNumber { get; set; }

        #endregion
    }
}
