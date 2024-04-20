using NtErp.Wss.Services.Generic.Extends;

namespace NtErp.Wss.Services.Generic.Models
{
    public partial class ClientTop
    {/// <summary>
     /// 获取全部账期
     /// </summary>
        public int[] AccountPeriod
        {
            get
            {
                return this.GetDebtIndexes();
            }
        }
    }
}
