using Yahv.Underly;

namespace Yahv.Services.Models
{
    /// <summary>
    /// 供应商
    /// </summary>
    public class Supplier : Enterprise
    {
        public Supplier()
        {

        }

        #region 属性

        /// <summary>
        /// 唯一标识
        /// </summary>
        //public string ID { get; set; }

        /// <summary>
        /// 供应商类型
        /// </summary>
        public SupplierType Type { set; get; }
        /// <summary>
        /// 纳税人识别号
        /// </summary>
        public string TaxperNumber { set;  get; }

        /// <summary>
        /// 供应商性质
        /// </summary>
        public SupplierNature Nature { set;  get; }

        /// <summary>
        /// 等级
        /// </summary>
        public SupplierGrade? Grade { set;  get; }
        /// <summary>
        /// 所在地区
        /// </summary>
        public AreaType AreaType { set;  get; }
        //// <summary>
        /// 是否可以带票采购
        /// </summary>
        public InvoiceType? InvoiceType { set; get; }
        /// <summary>
        /// 是否是原厂供应商
        /// </summary>
        public bool IsFactory { set;  get; }
        /// <summary>
        /// 代理公司（内部公司）
        /// </summary>
        public string AgentCompany { set;  get; }

        /// <summary>
        /// 客户状态
        /// </summary>
        public ApprovalStatus Status { set;  get; }
        /// <summary>
        /// 大赢家编码
        /// </summary>
        public string DyjCode { set; get; }
        /// <summary>
        /// 账期
        /// </summary>
        public int RepayCycle { set; get; }
        /// <summary>
        /// 额度
        /// </summary>
        public decimal Price { set; get; }
        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { set; get; }

        #endregion

        #region 扩展属性
        /// <summary>
        /// 供应商类型描述
        /// </summary>
        public string TypeDes
        {
            get
            {
                return this.Type.GetDescription();
            }
        }
        /// <summary>
        /// 供应商性质描述
        /// </summary>
        public string NatureDes
        {
            get
            {
                return this.Nature.GetDescription();
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
        /// <summary>
        /// 所在地描述
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
        #endregion
    }
}
