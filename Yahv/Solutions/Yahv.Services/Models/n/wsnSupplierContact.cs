using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Usually;

namespace Yahv.Services.Models
{
    /// <summary>
    /// 客户的供应商
    /// </summary>
    public class wsnContact : IUnique
    {
        #region 属性
        public string ID { set; get; }
        /// <summary>
        /// 私有供应商ID
        /// </summary>
        public string nSupplierID { set; get; }
        /// <summary>
        /// 所属客户企业ID
        /// </summary>
        public string OwnID { set; get; }
        /// <summary>
        /// 所属客户企业名称
        /// </summary>
        public string OwnName { set; get; }
        /// <summary>
        /// 供应商的企业ID
        /// </summary>
        public string RealID { set; get; }
        /// <summary>
        /// 供应商的企业名称
        /// </summary>
        public string RealEnterpriseName { set; get; }
        /// <summary>
        /// 联系人姓名
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { set; get; }
        /// <summary>
        /// 电话
        /// </summary>
        public string Tel { set; get; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { set; get; }
        /// <summary>
        /// 传真
        /// </summary>
        public string Fax { set; get; }
        /// <summary>
        /// QQ
        /// </summary>
        public string QQ { set; get; }
        /// <summary>
        /// 供应商等级
        /// </summary>
        public GeneralStatus Status { set; get; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreaterID { set; internal get; }
        #endregion

        #region 事件
        /// <summary>
        /// EnterSuccess
        /// </summary>
        public event SuccessHanlder EnterSuccess;
        public event SuccessHanlder AbandonSuccess;
        #endregion

        #region 持久化
        public void Enter()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                if (repository.GetTable<Layers.Data.Sqls.PvbCrm.nContacts>().Any(item => item.ID == this.ID))
                {
                    repository.Update<Layers.Data.Sqls.PvbCrm.nContacts>(new
                    {
                        nSupplierID = this.nSupplierID,//供应商ID
                        EnterpriseID = this.OwnID,//所属企业，客户ID
                        RealID = this.RealID,//供应商的企业ID
                        Name = this.Name,
                        Tel = this.Tel,
                        Mobile = this.Mobile,
                        Email = this.Email,
                        Fax = this.Fax,
                        QQ = this.QQ,
                        UpdateDate = DateTime.Now,
                        Creator = this.CreaterID
                    }, item => item.ID == this.ID);
                }
                else
                {
                    this.ID = PKeySigner.Pick(Yahv.Underly.PKeyType.nContact);
                    repository.Insert(this.ToLinq());

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
                if (repository.ReadTable<Layers.Data.Sqls.PvbCrm.nContacts>().Any(item => item.ID == this.ID))
                {
                    repository.Update<Layers.Data.Sqls.PvbCrm.nContacts>(new { Status = GeneralStatus.Deleted }, item => item.ID == this.ID);
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
