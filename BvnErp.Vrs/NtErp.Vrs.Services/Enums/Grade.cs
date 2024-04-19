using System;
using System.Collections.Generic;
using Needs.Utils.Descriptions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Vrs.Services.Enums
{
    public enum Grade
    {
        [Description("一级")]
        ClassA = 1,
        [Description("二级")]
        SecondLevel = 2,
        [Description("三级")]
        ThreeLevel = 3
    }
}
