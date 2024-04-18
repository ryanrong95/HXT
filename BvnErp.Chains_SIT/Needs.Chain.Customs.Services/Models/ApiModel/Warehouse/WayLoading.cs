using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 提货单信息
    /// </summary>
    public class WayLoading : IUnique
    {
        #region 属性

        /// <summary>
        /// 主键ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 提货时间
        /// </summary>
        public DateTime? TakingDate { get; set; }

        /// <summary>
        /// 提货地址
        /// </summary>
        public string TakingAddress { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public string TakingContact { get; set; }

        /// <summary>
        /// 联系人电话
        /// </summary>
        public string TakingPhone { get; set; }

        /// <summary>
        /// 两地车牌1
        /// </summary>
        public string CarNumber1 { get; set; }

        /// <summary>
        /// 司机名字
        /// </summary>
        public string Driver { get; set; }

        /// <summary>
        /// 汽车荷载量
        /// </summary>
        public int? Carload { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? ModifyDate { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatorID { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        public string ModifierID { get; set; }


        public CgLoadingExcuteStauts LoadingExcuteStauts { get; set; }

        #endregion

        public WayLoading()
        {
            this.CreateDate = this.ModifyDate = DateTime.Now;
        }
    }
}
