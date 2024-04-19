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
    /// 用户收入表
    /// </summary>
    public class UserInput :IUnique,  IPersist,IFulSuccess, IFulError
    {
        public UserInput()
        {
            this.CreateDate = DateTime.Now;
        }

        #region 属性

        public string ID { get; set; }

        /// <summary>
        /// 账户类型
        /// </summary>
        public UserAccountType Type { get; set; }
        /// <summary>
        /// 来源
        /// </summary>
        public InputFrom From { get; set; }

        /// <summary>
        /// UserID
        /// </summary>
        internal string ClientID { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        public string Code { get; set; }
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

        #endregion

        #region 扩展属性

        /// <summary>
        /// 客户
        /// </summary>
        public ClientTop Client { get; set; }

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
                        this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.UserInput);
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
}
