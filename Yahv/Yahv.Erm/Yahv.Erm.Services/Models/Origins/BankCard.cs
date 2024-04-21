using System;
using System.Linq;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbErm;
using Layers.Linq;
using Yahv.Linq;
using Yahv.Usually;

namespace Yahv.Erm.Services.Models.Origins
{
    /// <summary>
    /// 银行卡信息
    /// </summary>
    public class BankCard : IUnique
    {
        #region 事件

        /// <summary>
        /// EnterSuccess
        /// </summary>
        public event SuccessHanlder EnterSuccess;

        /// <summary>
        /// EnterError
        /// </summary>
        public event ErrorHanlder EnterError;

        /// <summary>
        /// AbandonSuccess
        /// </summary>
        public event SuccessHanlder AbandonSuccess;

        #endregion

        #region 属性
        /// <summary>
        /// 通StaffID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 开户行
        /// </summary>
        public string Bank { get; set; }

        /// <summary>
        /// 开户行地址
        /// </summary>
        public string BankAddress { get; set; }

        /// <summary>
        /// 银行账号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 银行编码 (国际)
        /// </summary>
        public string SwiftCode { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 正常 停用
        /// </summary>
        public Status Status { get; set; }
        #endregion

        #region 持久化

        /// <summary>
        /// 添加/修改
        /// </summary>
        public void Enter()
        {
            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                //添加
                if (!repository.ReadTable<BankCards>().Any(t => t.ID == this.ID))
                {
                    repository.Insert(new BankCards()
                    {
                        ID = this.ID,
                        CreateDate = DateTime.Now,
                        Account = this.Account,
                        Bank = this.Bank,
                        BankAddress = this.BankAddress,
                        SwiftCode = this.SwiftCode,
                    });
                }
                //修改
                else
                {
                    //判断是否存在
                    repository.Update<BankCards>(new
                    {
                        Account = this.Account,
                        Bank = this.Bank,
                        BankAddress = this.BankAddress,
                        SwiftCode = this.SwiftCode,
                    }, a => a.ID == this.ID);
                }

                //操作成功
                if (this != null && EnterSuccess != null)
                    this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }

        #endregion
    }
}
