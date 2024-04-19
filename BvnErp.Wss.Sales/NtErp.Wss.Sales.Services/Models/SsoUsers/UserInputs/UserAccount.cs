using NtErp.Wss.Sales.Services.Underly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Model
{
    /// <summary>
    /// 用户账户收入
    /// </summary>
    public class UserAccount
    {
        public UserAccount()
        {
            this.CreateDate = DateTime.Now;
        }

        public UserAccount(string id) : this()
        {
            this.ID = id;
        }

        #region 属性
        public string ID { get; set; }
        /// <summary>
        /// 账户类型
        /// </summary>
        public Models.SsoUsers.UserAccountType Type { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        public Models.SsoUsers.InputSource Source { get; set; }
        /// <summary>
        /// UserID
        /// </summary>
        public string UserID { get; set; }
        /// <summary>
        /// 编号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 币种
        /// </summary>
        public Underly.Currency Currency { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        #endregion

        public event ErrorHanlder Error;
        public event EnterSuccessHanlder EnterSuccess;
        public event AbandonSuccessHanlder AbandonSuccess;

        #region 持久化
        public void Enter()
        {
              
        }

        public void Abandon()
        {
            throw new NotImplementedException();
        }
        #endregion


    }
}
