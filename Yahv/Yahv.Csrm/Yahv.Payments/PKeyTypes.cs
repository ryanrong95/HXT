using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;

namespace Yahv.Payments
{
    public enum PKeyTypes
    {
        /// <summary>
        /// 应收日志
        /// </summary>
        [Repository(typeof(PvbCrmReponsitory))]
        [PKey("LogsRece", PKeySigner.Mode.Date, 4)]
        Logs_Receivable,
    }
}