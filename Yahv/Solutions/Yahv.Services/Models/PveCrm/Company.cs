using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace Yahv.Services.Models.PveCrm
{
    public class Company : Enterprise
    {
        public Underly.CrmPlus.CompanyType Type { get; set; }

        public Underly.DataStatus Status { get; set; }

        #region 纯粹为了兼容原来的PvbCrm的命名，让其他使用公共视图的系统少一点代码改动

        public AreaType Range
        {
            get
            {
                if (FixedArea.MainLand.GetFixedID() == this.District)
                    return AreaType.domestic;
                else
                    return AreaType.International;
            }
        }

        public string RangeDes
        {
            get
            {
                return this.Range.GetDescription();
            }
        }

        #endregion
    }
}
