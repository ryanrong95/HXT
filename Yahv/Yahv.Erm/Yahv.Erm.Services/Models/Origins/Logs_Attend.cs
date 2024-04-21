using System;
using Yahv.Linq;

namespace Yahv.Erm.Services.Models.Origins
{
    public class Logs_Attend : IUnique
    {
        #region 属性
        public string ID { get; set; }
        public DateTime Date { get; set; }
        public string StaffID { get; set; }
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 考勤机IP
        /// </summary>
        public string IP { get; set; }
        #endregion
    }
}