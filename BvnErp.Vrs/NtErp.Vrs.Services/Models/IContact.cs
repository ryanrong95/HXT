using NtErp.Vrs.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Vrs.Services.Models
{
    public interface IContact : Needs.Linq.IUnique
    {
        string ID { set; get; }
        string Name { set; get; }
        string Tel { set; get; }
        string Email { set; get; }
        string Mobile { set; get; }
        JobType Job { set; get; } 
        bool Sex { get; set; }
        string Birthday { set; get; }
        string CompanyID { set; get; }
        Status Status { set; get; }
    }
}
