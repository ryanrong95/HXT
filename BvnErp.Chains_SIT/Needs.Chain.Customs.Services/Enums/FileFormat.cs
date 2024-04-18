using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    [Obsolete("已作废")]
    public enum FileFormat
    {
        [Description("PDF")]
        PDF = 1,

        [Description("JPG")]
        JPG = 2,

        [Description("BNG")]
        BNG = 3,

        [Description("EXE")]
        EXE = 4,
    }
}
