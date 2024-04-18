using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Models
{
    public interface IApplyStep : IPersistence
    {
        string ApplyID { get; set; }

        int Step { get; set; }

        string AdminID { get; set; }

        Enums.ApplyStep Status { get; set; }
    }
}
