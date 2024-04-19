using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcApp.Buyer.Services.Models
{
    public class Consignee : Needs.Linq.IUnique
    {
        public string ID { get; set; }
        public string UserID { get; set; }
        public string Country { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Contact { get; set; }
        public string Company { get; set; }
        public string Tel { get; set; }
        public string Email { get; set; }
        public string Zipcode { get; set; }
        public string Address { get; set; }

        public NtErp.Wss.Oss.Services.Models.Party ToOssParty(Needs.Underly.District district)
        {
            var company = new NtErp.Wss.Oss.Services.Models.Company
            {
                Type = NtErp.Wss.Oss.Services.CompanyType.Company,
                Name = Company,
                Address = this.Address,
                Code = "",
            };
            return new NtErp.Wss.Oss.Services.Models.Party
            {
                Address = this.Address,
                Company = company,
                Contact = new NtErp.Wss.Oss.Services.Models.Contact
                {
                    Company = company,
                    Email = this.Email,
                    Mobile = "",
                    Name = this.FirstName + this.LastName,
                    Tel = this.Tel,
                },
                District = district,
                Postzip = this.Zipcode,

            };
        }
        
    }
}
