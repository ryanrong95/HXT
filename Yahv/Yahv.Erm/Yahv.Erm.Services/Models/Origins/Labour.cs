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
    /// 劳资信息
    /// </summary>
    public class Labour : IUnique
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
        /// ID(StaffID)
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 入职时间
        /// </summary>
        public DateTime EntryDate { get; set; }

        /// <summary>
        /// 入职公司
        /// </summary>
        public string EntryCompany { get; set; }

        /// <summary>
        /// 所属企业（合同所属企业）
        /// </summary>
        public string EnterpriseID { get; set; }

        /// <summary>
        /// 离职时间 可空 空表示未离职，有代表离职
        /// </summary>
        public DateTime? LeaveDate { get; set; }

        /// <summary>
        /// 劳动合同签订时间
        /// </summary>
        public DateTime? SigningTime { get; set; }

        /// <summary>
        /// 劳动合同期限
        /// </summary>
        public DateTime? ContractPeriod { get; set; }

        /// <summary>
        /// 劳动合同类型
        /// </summary>
        public string ContractType { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// 试用期时长（月）
        /// </summary>
        public string ProbationMonths { get; set; }

        /// <summary>
        /// 社保账号
        /// </summary>
        public string SocialSecurityAccount { get; set; }

        #endregion

        #region 扩展属性

        public DateTime? ProbationEndDate
        {
            get
            {
                if (string.IsNullOrEmpty(ProbationMonths))
                {
                    return null;
                }
                var day = Math.Round(double.Parse(ProbationMonths) * 30, 1);
                return this.EntryDate.AddDays((int)day);
            }
        }
        #endregion

        public Labour()
        {
            this.CreateDate = this.UpdateDate = DateTime.Now;
        }

        #region 持久化

        public void Enter()
        {
            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                //添加
                if (!repository.ReadTable<Labours>().Any(t => t.ID == this.ID))
                {
                    repository.Insert(new Labours()
                    {
                        ID = this.ID,
                        ContractPeriod = this.ContractPeriod,
                        ContractType = this.ContractType,
                        EnterpriseID = this.EnterpriseID,
                        EntryCompany = this.EntryCompany,
                        EntryDate = this.EntryDate,
                        LeaveDate = this.LeaveDate,
                        SigningTime = this.SigningTime,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        SocialSecurityAccount = this.SocialSecurityAccount,
                        ProbationMonths = this.ProbationMonths,
                    });
                }
                //修改
                else
                {
                    repository.Update<Labours>(new
                    {
                        ContractPeriod = this.ContractPeriod,
                        ContractType = this.ContractType,
                        EnterpriseID = this.EnterpriseID,
                        EntryCompany = this.EntryCompany,
                        EntryDate = this.EntryDate,
                        LeaveDate = this.LeaveDate,
                        SigningTime = this.SigningTime,
                        UpdateDate = DateTime.Now,
                        SocialSecurityAccount = this.SocialSecurityAccount,
                        ProbationMonths = this.ProbationMonths,
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