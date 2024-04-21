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
using Yahv.Utils.Converters.Contents;
using YaHv.Csrm.Services.Extends;

namespace YaHv.Csrm.Services.Models.Origins
{
    /// <summary>
    /// 库房
    /// </summary>
    public class WareHouse : Yahv.Linq.IUnique
    {
        #region 属性
        /// <summary>
        /// 唯一码
        /// </summary>
        /// <chenhan>保障全局唯一</chenhan>
        public string ID { set; get; }

        /// <summary>
        /// 大赢家编码
        /// </summary>
        public string DyjCode { set; get; }

        /// <summary>
        /// 等级
        /// </summary>
        public WarehouseGrade Grade { set; get; }
        /// <summary>
        /// 所属地区
        /// </summary>
        public Region District { set; get; }
        /// <summary>
        /// 具体地址
        /// </summary>
        public string Address { set; get; }
        /// <summary>
        /// 企业基本信息
        /// </summary>
        public Enterprise Enterprise { set; get; }
        /// <summary>
        /// 状态
        /// </summary>
        public ApprovalStatus Status { set; get; }
        /// <summary>
        /// 地区+仓库编码
        /// </summary>
        public string WsCode { get; set; }
        public string CreatorID { set; get; }
        /// <summary>
        /// 录入人信息
        /// </summary>
        public Admin Creator { internal set; get; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { set; get; }
        public DateTime UpdateDate { set; get; }
        #endregion

        #region 扩展(发票，到货地址，联系人)
        /// <summary>
        /// 客户的所有到货地址
        /// </summary>
        Views.Rolls.ConsigneesRoll consignees;
        public Views.Rolls.ConsigneesRoll Consignees
        {
            get
            {
                if (this.consignees == null || this.consignees.Disposed)
                {
                    this.consignees = new Views.Rolls.ConsigneesRoll(this.Enterprise);
                }
                return this.consignees;
            }
        }
        /// <summary>
        /// 客户的所有联系人信息
        /// </summary>
        Views.Rolls.ContactsRoll contacts;
        public Views.Rolls.ContactsRoll Contacts
        {
            get
            {
                if (this.contacts == null || this.contacts.Disposed)
                {
                    this.contacts = new Views.Rolls.ContactsRoll(this.Enterprise);
                }
                return this.contacts;
            }
        }
        #endregion
        #region 事件
        /// <summary>
        /// EnterSuccess
        /// </summary>
        public event SuccessHanlder EnterSuccess;
        /// <summary>
        /// AbandonSuccess
        /// </summary>
        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder NameRepeat;
        #endregion

        #region 持久化
        public void Enter()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                this.Enterprise.Enter();
                this.CreateDate = this.UpdateDate = DateTime.Now;
                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    if (repository.GetTable<Layers.Data.Sqls.PvbCrm.WareHouses>().Any(item => item.ID == this.Enterprise.ID))
                    {
                        if (this != null && this.NameRepeat != null)
                        {
                            this.NameRepeat(this, new ErrorEventArgs());
                        }
                    }
                    else
                    {
                        this.ID = this.Enterprise.ID;
                        this.Status = ApprovalStatus.Normal;
                        this.WsCode = PKeySigner.Pick(YaHv.Csrm.Services.PKeyType.WareHouse, this.District.ToString().ToUpper());
                        repository.Insert(this.ToLinq());
                    }
                }
                else
                {
                    repository.Update<Layers.Data.Sqls.PvbCrm.WareHouses>(new
                    {
                        DyjCode = this.DyjCode,
                        District = this.District,
                        Address = this.Address,
                        Grade = (int)this.Grade,
                        UpdateDate = this.UpdateDate
                    }, item => item.ID == this.ID);

                }

                if (this != null && this.EnterSuccess != null)
                {
                    this.EnterSuccess(this, new SuccessEventArgs(this));
                }

            }
        }
        public void Abandon()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                if (repository.GetTable<Layers.Data.Sqls.PvbCrm.WareHouses>().Any(item => item.ID == this.ID))
                {
                    repository.Delete<Layers.Data.Sqls.PvbCrm.WareHouses>(item => item.ID == this.ID);
                }
                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this));
                }
            }

        }
        #endregion
    }
}
