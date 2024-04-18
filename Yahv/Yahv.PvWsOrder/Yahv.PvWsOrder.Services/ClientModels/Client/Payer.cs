//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Layers.Data.Sqls;
//using Yahv.Linq;
//using Yahv.Underly;
//using Yahv.Usually;

//namespace Yahv.PvWsOrder.Services.ClientModels
//{
//    public class Payer : IUnique
//    {
//        #region 
//        /// <summary>
//        /// 主键
//        /// </summary>
//        public string ID { get; set; }

//        /// <summary>
//        /// 客户ID
//        /// </summary>
//        public string ClientID { get; set; }

//        /// <summary>
//        /// 开户行
//        /// </summary>
//        public string Bank { get; set; }

//        /// <summary>
//        /// 开户行地址
//        /// </summary>
//        public string BankAddress { get; set; }

//        /// <summary>
//        /// 银行账户
//        /// </summary>
//        public string Account { get; set; }

//        /// <summary>
//        /// 银行代码
//        /// </summary>
//        public string SwiftCode { get; set; }

//        /// <summary>
//        /// 支付方式
//        /// </summary>
//        public Methord Methord { get; set; }

//        /// <summary>
//        /// 币种
//        /// </summary>
//        public Currency Currency { get; set; }

//        /// <summary>
//        /// 地区
//        /// </summary>
//        public District District { get; set; }

//        /// <summary>
//        /// 姓名
//        /// </summary>
//        public string Name { get; set; }

//        /// <summary>
//        /// 电话
//        /// </summary>
//        public string Tel { get; set; }

//        /// <summary>
//        /// 手机号码
//        /// </summary>
//        public string Mobile { get; set; }

//        /// <summary>
//        /// 邮箱
//        /// </summary>
//        public string Email { get; set; }

//        /// <summary>
//        /// 状态
//        /// </summary>
//        public GeneralStatus Status { get; set; }

//        /// <summary>
//        /// 创建人
//        /// </summary>
//        public string AdminID { get; set; }

//        /// <summary>
//        /// 创建时间
//        /// </summary>
//        public DateTime CreateDate { get; set; }

//        /// <summary>
//        /// 更新时间
//        /// </summary>
//        public DateTime UpdateDate { get; set; }
//        #endregion

//        public Payer()
//        {
//            Status = GeneralStatus.Normal;
//            CreateDate = UpdateDate = DateTime.Now;
//        }

//        #region 事件
//        public event SuccessHanlder EnterSuccess;

//        public void OnEnterSuccess()
//        {
//            if (this != null && this.EnterSuccess != null)
//            {
//                //成功后触发事件
//                this.EnterSuccess(this, new SuccessEventArgs(this));
//            }
//        }
//        #endregion

//        /// <summary>
//        /// 持久化
//        /// </summary>
//        public void Enter()
//        {
//            using (PvWsOrderReponsitory Reponsitory = new PvWsOrderReponsitory())
//            {
//                this.ID = Guid.NewGuid().ToString();
//                Reponsitory.Insert(new Layers.Data.Sqls.PvWsOrder.Payers
//                {
//                    ID = this.ID,
//                    ClientID = this.ClientID,
//                    Bank = this.Bank,
//                    BankAddress = this.BankAddress,
//                    Account = this.Account,
//                    SwiftCode = this.SwiftCode,
//                    Methord = (int)this.Methord,
//                    Currency = (int)this.Currency,
//                    District = (int)this.District,
//                    Name = this.Name,
//                    Tel = this.Tel,
//                    Mobile = this.Mobile,
//                    Email = this.Email,
//                    Status = (int)this.Status,
//                    AdminID = this.AdminID,
//                    CreateDate = this.CreateDate,
//                    UpdateDate = this.UpdateDate,
//                });
//            }
//            this.OnEnterSuccess();
//        }
//    }
//}
