using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using Yahv.Usually;
using YaHv.CrmPlus.Services.Models.Origins;

namespace Yahv.CrmPlus.Service.Models.Origins
{
    /// <summary>
    /// 发票信息
    /// </summary>
    public class Invoice : Yahv.Linq.IUnique
    {
        public Invoice()
        {
            this.Status = DataStatus.Normal;
            this.CreateDate = DateTime.Now;
        }
        #region 属性
        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 企业ID
        /// </summary>
        public string EnterpriseID { get; set; }
        /// <summary>
        /// 企业类型
        /// </summary>
        public RelationType RelationType { set; get; }
        /// <summary>
        ///企业地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string Tel { get; set; }
        /// <summary>
        /// <summary>
        /// 开户银行
        /// </summary>
        public string Bank { get; set; }

        /// 银行账号
        /// </summary>
        public string Account { set; get; }

        /// <summary>
        /// 状态
        /// </summary>
        public DataStatus Status { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; internal set; }

        /// </summary>
        public Enterprise Enterprise { set; get; }
        public string CreatorID { set; get; }
        /// <summary>
        /// 添加人
        /// </summary>
        public Admin Admin { get; internal set; }

        #endregion
        #region 拓展字段
        /// <summary>
        /// 纳税人识别号
        /// </summary>
        public string Uscc { get; set; }

        #endregion

        #region 事件
        /// <summary>
        /// EnterSuccess
        /// </summary>
        public event SuccessHanlder EnterSuccess;

        public event ErrorHanlder EnterError;
        /// <summary>
        /// AbandonSuccess
        /// </summary>
        public event SuccessHanlder AbandonSuccess;
        #endregion


        #region 持久化
        public void Enter()
        {
            using (var repository = LinqFactory<PvdCrmReponsitory>.Create())
            {
                if (!repository.ReadTable<Layers.Data.Sqls.PvdCrm.Invoices>().Any(x => x.ID == this.ID))
                {

                    this.ID = PKeySigner.Pick(Yahv.CrmPlus.Service.PKeyType.Invoices);
                    repository.Insert(new Layers.Data.Sqls.PvdCrm.Invoices()
                    {
                        ID = this.ID,
                        EnterpriseID = this.EnterpriseID,
                        RelationType = (int)this.RelationType,
                        Address = this.Address,
                        Tel = this.Tel,
                        Bank = this.Bank,
                        Account = this.Account,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        CreatorID = this.CreatorID
                    });
                }
                else
                {
                    repository.Update<Layers.Data.Sqls.PvdCrm.Invoices>(new
                    {
                        ID = this.ID,
                        EnterpriseID = this.EnterpriseID,
                        RelationType = (int)this.RelationType,
                        Address = this.Address,
                        Tel = this.Tel,
                        Bank = this.Bank,
                        Account = this.Account,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        CreatorID = this.CreatorID
                    }, item => item.ID == this.ID);

                }
                this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this));
            }
        }

        virtual public void Closed()
        {
            using (var repository = LinqFactory<PvdCrmReponsitory>.Create())
            {
                repository.Update<Layers.Data.Sqls.PvdCrm.Invoices>(new
                {
                    Status = DataStatus.Closed
                }, item => item.ID == this.ID);
                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this));
                }
            }
        }

        virtual public void Enable()
        {
            using (var repository = LinqFactory<PvdCrmReponsitory>.Create())
            {
                repository.Update<Layers.Data.Sqls.PvdCrm.Invoices>(new
                {
                    Status = DataStatus.Normal
                }, item => item.ID == this.ID);
                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this));
                }
            }
        }
        #endregion
    }
}
