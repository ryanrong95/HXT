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
using YaHv.Csrm.Services.Extends;

namespace YaHv.Csrm.Services.Models.Origins
{
    public class nContact : Yahv.Linq.IUnique
    {
        public nContact()
        {
            this.Status = GeneralStatus.Normal;
            this.CreateDate = this.UpdateDate = DateTime.Now;
        }
        #region 属性
        /// <summary>
        /// 联系人唯一标识号
        /// </summary>
        public string ID { set; get; }
        /// <summary>
        /// 供应商ID
        /// </summary>
        public string nSupplierID { set; get; }
        /// <summary>
        ///所属客户企业ID
        /// </summary>
        public string EnterpriseID { get; set; }
        /// <summary>
        /// 供应商企业ID
        /// </summary>
        public string RealID { set; get; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string Tel { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 电子邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 传真
        /// </summary>
        public string Fax { set; get; }
        /// <summary>
        /// QQ
        /// </summary>
        public string QQ { set; get; }
        /// <summary>
        /// 状态
        /// </summary>
        public GeneralStatus Status { get; set; }
        /// <summary>
        /// 记录创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 记录最后修改时间
        /// </summary>
        public DateTime UpdateDate { get; set; }
        /// <summary>
        /// 添加人
        /// </summary>
        public string Creator { get; set; }
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
                    //联系人已存在，只能修改状态
                    repository.Update(this.ToLinq(), item => item.ID == this.ID);
                }
                else
                {
                    this.ID = PKeySigner.Pick(PKeyType.nContact);
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
