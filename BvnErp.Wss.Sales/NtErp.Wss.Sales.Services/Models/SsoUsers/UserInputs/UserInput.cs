
using NtErp.Wss.Sales.Services.Models.SsoUsers;
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
    public class UserInput : IUserInput
    {
        public UserInput()
        {
            this.CreateDate = DateTime.Now;
        }

        public UserInput(string id) : this()
        {
            this.ID = id;
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
        public InputSource Source { get; set; }
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

        #region 实现
        /// <summary>
        /// 获取需要平销的收入记录
        /// </summary>
        internal UserInput Balanced()
        {
            using (var reponsitory = new Layer.Data.Sqls.BvOrdersReponsitory())
            {
                var linqs = from input in reponsitory.ReadTable<Layer.Data.Sqls.BvOrders.UserInputs>()
                            join output in reponsitory.ReadTable<Layer.Data.Sqls.BvOrders.UserOutputs>()
                            on input.ID equals output.UserInputID into temp
                            from tep in temp.DefaultIfEmpty()
                            where input.UserID == this.UserID && input.Type == (int)this.Type && input.Currency == (int)this.Currency
                            && input.Amount > temp.Sum(item => item.Amount as decimal?).GetValueOrDefault()
                            select input;

                return linqs.Select(item => new UserInput
                {
                    ID = item.ID,
                    UserID = item.UserID,
                    Amount = item.Amount,
                    Code = item.Code,
                    Currency = (Currency)item.Currency,
                    Source = (InputSource)item.Source,
                    Type = (UserAccountType)item.Type,
                    CreateDate = item.CreateDate
                }).OrderBy(item => item.CreateDate).FirstOrDefault();
            }
        }
        #endregion
    }
}
