using System;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Erm.Services.Models.Origins
{
    /// <summary>
    /// 区域
    /// </summary>
    public class RegionAc : IUnique
    {
        #region 属性
        public string ID { get; set; }
        public string FatherID { get; set; }
        public string Name { get; set; }
        public GeneralStatus Status { get; set; }
        public string CreatorID { get; set; }
        public string ModifyID { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; } 
        #endregion
    }
}