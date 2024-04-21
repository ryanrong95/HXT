using System;
using System.Linq;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Linq;
using Yahv.Usually;

namespace Yahv.Finance.Services.Models.Origins
{
    /// <summary>
    /// 认领表
    /// </summary>
    public class AccountWork : IUnique
    {
        #region 事件
        /// <summary>
        /// 认领
        /// </summary>
        public event SuccessHanlder UpdateSuccess;
        #endregion

        #region 数据库属性
        public string ID { get; set; }

        /// <summary>
        /// 收款ID
        /// </summary>
        public string PayeeLeftID { get; set; }

        /// <summary>
        /// 业务
        /// </summary>
        public string Conduct { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// 认领人ID
        /// </summary>
        public string ClaimantID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifyDate { get; set; }
        #endregion

        #region 持久化

        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            {
                //新增
                if (!reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.AccountWorks>().Any(item => item.ID == this.ID))
                {
                    this.ID = this.ID ?? PKeySigner.Pick(PKeyType.AccountWorks);

                    reponsitory.Insert(new Layers.Data.Sqls.PvFinance.AccountWorks()
                    {
                        ID = this.ID,
                        ModifyDate = DateTime.Now,
                        ClaimantID = this.ClaimantID,
                        Company = this.Company,
                        Conduct = this.Conduct,
                        CreateDate = DateTime.Now,
                        PayeeLeftID = this.PayeeLeftID
                    });
                }
                else
                {
                    reponsitory.Update<Layers.Data.Sqls.PvFinance.AccountWorks>(new
                    {
                        ModifyDate = DateTime.Now,
                        ClaimantID = this.ClaimantID,
                        Company = this.Company,
                    }, item => item.ID == this.ID);

                    this.UpdateSuccess?.Invoke(this, new SuccessEventArgs(this));
                }
            }
        }
        #endregion
    }
}