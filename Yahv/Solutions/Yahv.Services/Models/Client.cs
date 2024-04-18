using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Services.Models
{
    /// <summary>
    /// 客户信息
    /// </summary>

    [ViewFactory(typeof(Views.ClientsAll<>))]
    public class Client : Enterprise
    {
        #region 属性
        
        /// <summary>
        /// 客户性质
        /// </summary>
        public ClientType Nature { set; get; }
        /// <summary>
        /// 等级
        /// </summary>
        public ClientGrade? Grade { set; get; }
        /// <summary>
        /// 客户类型
        /// </summary>
        public AreaType AreaType { set; get; }

        /// <summary>
        /// 纳税人识别号
        /// </summary>
        public string TaxperNumber { set; get; }
        ///// <summary>
        ///// 是否是Vip客户
        ///// </summary>
        //public bool Vip { set; get; }
        /// <summary>
        /// Vip等级
        /// </summary>
        public VIPLevel Vip { set; get; }
        /// <summary>
        /// 客户状态
        /// </summary>
        public ApprovalStatus Status { set; get; }
        /// <summary>
        /// 大赢家编码
        /// </summary>
        public string DyjCode { set; get; }

        #endregion

        #region 扩展属性
        /// <summary>
        /// 客户性质描述
        /// </summary>
        public string NatureDes
        {
            get
            {
                return this.Nature.GetDescription();
            }
        }
        /// <summary>
        /// 客户等级描述
        /// </summary>
        public string GradeDes
        {
            get
            {
                return this.Grade?.GetDescription();
            }
        }
        /// <summary>
        /// 客户类型描述
        /// </summary>
        public string AreaTypeDes
        {
            get
            {
                return this.AreaType.GetDescription();
            }
        }
        /// <summary>
        /// 状态描述
        /// </summary>
        public string StatusDes
        {
            get
            {
                return this.Status.GetDescription();
            }
        }
        /// <summary>
        /// Vip等级描述
        /// </summary>
        public string VipDes
        {
            get
            {
                return this.Vip.GetDescription();
            }
        }
        #endregion

    }
}
