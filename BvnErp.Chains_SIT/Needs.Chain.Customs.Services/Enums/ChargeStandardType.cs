using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    /// <summary>
    /// 收费标准类型
    /// </summary>
    public enum ChargeStandardType
    {
        /// <summary>
        /// 普通单选
        /// </summary>
        CommonSingleSelect = 1,

        /// <summary>
        /// 最后一级 CheckBox
        /// </summary>
        LastCheckBox = 2,

        /// <summary>
        /// 最后一级 RadioButton
        /// </summary>
        LastRadioButton = 3,
    }
}
