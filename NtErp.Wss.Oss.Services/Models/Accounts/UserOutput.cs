using Needs.Linq;
using Needs.Underly;
using NtErp.Wss.Oss.Services.Extends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Oss.Services.Models
{
    /// <summary>
    /// 用户支出
    /// </summary>
    public class UserOutput : IUnique, IPersist, IFulSuccess, IFulError
    {
        public UserOutput()
        {
            this.CreateDate = DateTime.Now;
        }

        #region 属性

        public string ID { get; set; }
        /// <summary>
        /// 收入ID
        /// </summary>
        public string UserInputID { get; set; }
        /// <summary>
        /// 客户ID
        /// </summary>
        public string ClientID { get; set; }
        /// <summary>
        /// 账户类型
        /// </summary>
        public UserAccountType Type { get; set; }
        /// <summary>
        /// 支出类型
        /// </summary>
        public OutputTo From { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 账期
        /// </summary>
        public int? DateIndex { get; set; }



        #endregion


        #region 持久化

        public event SuccessHanlder EnterSuccess;
        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event ErrorHanlder AbandonError;

        public void Enter()
        {
            try
            {
                using (var reponsitory = new Layer.Data.Sqls.CvOssReponsitory())
                {
                    if (string.IsNullOrWhiteSpace(this.ID))
                    {
                        this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.UserOutput);
                        reponsitory.Insert(this.ToLinq());
                    }
                }
                if (this != null && this.EnterSuccess != null)
                {
                    this.EnterSuccess(this, new SuccessEventArgs(this));
                }
            }
            catch (Exception ex)
            {
                if (this != null && this.EnterError != null)
                {
                    this.EnterError(this, new ErrorEventArgs(ex.Message, ErrorType.System));
                }
            }
        }
        #endregion
    }


    public class AdminUserOutput : UserOutput
    {
        public NtErp.Services.Models.AdminTop Admin { get; set; }
    }
}
