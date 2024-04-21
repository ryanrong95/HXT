using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.Linq;

using Yahv.Underly;
using Yahv.Usually;
using YaHv.CrmPlus.Services.Models.Origins;

namespace Yahv.CrmPlus.Service.Models.Origins
{
    /// <summary>
    /// 内部公司
    /// </summary>
    public class Company : Enterprise, IUnique
    {
        #region Company属性

        public DataStatus CompanyStatus { get; set; }

        public DateTime CompanyCreateDate { get; set; }

        public Admin Creator { get; internal set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatorID { get; set; }


        #endregion

        #region  拓展
        Views.Rolls.InvoicesRoll invoices;
        /// <summary>
        /// 发票信息
        /// </summary>
        public Views.Rolls.InvoicesRoll Invoices
        {
            get
            {
                if (this.invoices == null || this.invoices.Disposed)
                {
                    this.invoices = new InvoicesRoll(this.ID, RelationType.Own);
                }
                return this.invoices;
            }
        }

        /// <summary>
        /// 联系人
        /// </summary>
        ContactsRoll contacts;
        public Views.Rolls.ContactsRoll Contacts
        {
            get
            {
                if (this.contacts == null || this.invoices.Disposed)
                {
                    this.contacts = new ContactsRoll(this.ID, RelationType.Own);
                }
                return this.contacts;
            }
        }

        #endregion
        public Company()
        {
            this.CompanyCreateDate = DateTime.Now;
            this.CompanyStatus = DataStatus.Normal;
        }
        #region   持久化
        public override void Enter()
        {
            using (var reponsitory = LinqFactory<PvdCrmReponsitory>.Create())
            //using (var tran = reponsitory.OpenTransaction())
            //{
                {
                    //1.Company是否存在
                    //2.Enterprise是否存在
                    if (string.IsNullOrWhiteSpace(this.ID))
                    {
                        bool companyexist = new Views.Origins.CompaniesOrigin().Any(item => item.Name == this.Name);
                        if (companyexist)
                        {
                            this.Repeat(this, new ErrorEventArgs());
                            return;
                        }
                        else
                        {
                            var existids = new Views.Origins.EnterprisesOrigin().Where(item => item.Name == this.Name && item.IsDraft == false).Select(item => item.ID).ToArray();
                            if (existids.Count() > 0)
                            {
                                this.ID = existids.First();
                            }
                            base.Enter(reponsitory);
                            reponsitory.Insert(new Layers.Data.Sqls.PvdCrm.Companies()
                            {
                                ID = this.ID,
                                Status = (int)this.CompanyStatus,
                                CreateDate = this.CompanyCreateDate,
                                CreatorID = this.CreatorID
                            });
                        }
                    }
                    else
                    {
                        reponsitory.Update<Layers.Data.Sqls.PvdCrm.EnterpriseRegisters>(new
                        {
                            Corperation = this.EnterpriseRegister.Corperation,
                            RegAddress = this.EnterpriseRegister.RegAddress,
                            Uscc = this.EnterpriseRegister.Uscc,
                        }, item => item.ID == this.ID);
                    //}
                    //tran.Commit();
                }
                this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this));
            }
            // this.EnterError?.Invoke(this, new ErrorEventArgs());


        }


        /// <summary>
        /// 停用
        /// </summary>
        public void Close()
        {
            using (var repository = LinqFactory<PvdCrmReponsitory>.Create())
            {
                repository.Update<Layers.Data.Sqls.PvdCrm.Companies>(new
                {
                    Status = (int)DataStatus.Closed
                }, item => item.ID == this.ID);
                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this));
                }
            }
        }



        #endregion  事件

        /// <summary>
        /// EnterSuccess
        /// </summary>
        public event SuccessHanlder EnterSuccess;
        /// <summary>
        /// 重复
        /// </summary>
        public event ErrorHanlder Repeat;

        /// <summary>
        /// EnterError
        /// </summary>
        public event ErrorHanlder EnterError;


        public event SuccessHanlder AbandonSuccess;

    }
}

