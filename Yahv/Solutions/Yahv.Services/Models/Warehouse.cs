using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Services.Models
{
    /// <summary>
    /// 库房
    /// </summary>
    public class Warehouse :Enterprise
    {
        public Warehouse()
        {

        }

        #region 属性
        /// <summary>
        /// 唯一标识
        /// </summary>
        //public string ID { get; set; }
        /// <summary>
        /// 库房编码
        /// </summary>
        public string WsCode { set; get; }

        /// <summary>
        /// 所属地区
        /// </summary>
        public Region Region { set;  get; }
        /// <summary>
        /// 具体地址
        /// </summary>
        public string Address { set;  get; }
        /// <summary>
        /// 大赢家编码
        /// </summary>
        public string DyjCode { set; get; }
        /// <summary>
        /// 等级
        /// </summary>
        public WarehouseGrade Grade { get; set; }
        /// <summary>
        /// 库房状态
        /// </summary>
        public ApprovalStatus Status { set; get; }

        #endregion

        #region 扩展属性
        /// <summary>
        /// 所在地描述
        /// </summary>
        public string RegionDes
        {
            get
            {
                return this.Region.GetDescription();
            }
        }
        /// <summary>
        /// 等级描述
        /// </summary>
        public string GradeDes
        {
            get
            {
                return this.Grade.GetDescription();
            }
        }
        #endregion
    }


}
