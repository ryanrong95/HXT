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
    /// 联系人
    /// </summary>
    public class Contact : Yahv.Linq.IUnique
    {
        public Contact()
        {
            this.Status = Status.Normal;
            this.CreateDate = this.UpdateDate = DateTime.Now;
        }
        #region 属性
        /// <summary>
        /// 联系人唯一标识号
        /// </summary>
        string id;
        virtual public string ID
        {
            get
            {
                return this.id ?? string.Join("",
                    this.EnterpriseID,
                    this.Type,
                    this.Name,
                    this.Tel,
                    this.Mobile,
                    this.Email
                    ).MD5();
            }
            set { this.id = value; }
        }
        /// <summary>
        /// 公司唯一标识号
        /// </summary>
        public string EnterpriseID { get; set; }

        /// <summary>
        /// 联系人类型
        /// </summary>
        public ContactType Type { get; set; }

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
        /// 状态
        /// </summary>
        public Status Status { get; set; }
        /// <summary>
        /// 记录创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 记录最后修改时间
        /// </summary>
        public DateTime UpdateDate { get; set; }
        /// <summary>
        /// 公司基本信息
        /// </summary>
        public Enterprise Enterprise { set; get; }
        public string CreatorID { set; get; }
        /// <summary>
        /// 添加人
        /// </summary>
        public Admin Creator { get; internal set; }
        /// <summary>
        /// 默认联系人
        /// </summary>
        public bool IsDefault { set; get; }
        #endregion

        #region 事件
        /// <summary>
        /// EnterSuccess
        /// </summary>
        virtual public event SuccessHanlder EnterSuccess;
        virtual public event SuccessHanlder AbandonSuccess;
        #endregion

        #region 持久化
        virtual public void Enter()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                if (repository.GetTable<Layers.Data.Sqls.PvbCrm.Contacts>().Any(item => item.ID == this.ID))
                {
                    this.UpdateDate = DateTime.Now;
                    //联系人已存在，只能修改状态
                    repository.Update(this.ToLinq(), item => item.ID == this.ID);
                }
                else
                {
                    repository.Insert(this.ToLinq());

                }
                if (this != null && this.EnterSuccess != null)
                {
                    this.EnterSuccess(this, new SuccessEventArgs(this));
                }
            }
        }
        virtual public void Abandon()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                if (repository.ReadTable<Layers.Data.Sqls.PvbCrm.Contacts>().Any(item => item.ID == this.ID))
                {
                    repository.Update<Layers.Data.Sqls.PvbCrm.Contacts>(new { Status = Services.Status.Closed }, item => item.ID == this.ID);
                }
                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this));
                }
            }
        }
        #endregion
    }


    public class WsContact : Contact
    {
        string mapsid;
        public string MapsID
        {
            get
            {
                return "WsContact_" + this.EnterpriseID;
            }
            set
            {
                this.mapsid = value;
            }
        }
        #region 时间
        public override event SuccessHanlder EnterSuccess;
        public override event SuccessHanlder AbandonSuccess;
        #endregion

        #region 持久化
        public override void Enter()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                //提货地id址是否存在
                if (repository.ReadTable<Layers.Data.Sqls.PvbCrm.Contacts>().Any(item => item.ID == this.ID))
                {
                    repository.Update<Layers.Data.Sqls.PvbCrm.Contacts>(new
                    {
                        Type = (int)this.Type,
                        Name = this.Name,
                        Tel = this.Tel,
                        Mobile = this.Mobile,
                        Email = this.Email,
                        Fax = this.Fax,
                        Status = (int)this.Status,
                        UpdateDate = DateTime.Now
                    }, item => item.ID == this.ID);
                }
                else
                {
                    //录入
                    repository.Insert(this.ToLinq());
                }
                //是否默认
                //if (this.IsDefault)
                //{
                //    repository.Update<Layers.Data.Sqls.PvbCrm.MapsBEnter>(new
                //    {
                //        IsDefault = false
                //    }, item => item.EnterpriseID == this.EnterpriseID && item.Type == (int)MapsType.Contact && item.Bussiness == (int)Business.WarehouseServicing);
                //}
                //关系
                if (repository.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>().Any(item => item.ID == this.MapsID))
                {
                    repository.Update<Layers.Data.Sqls.PvbCrm.MapsBEnter>(new
                    {
                        IsDefault = this.IsDefault,
                        SubID = base.ID
                    }, item => item.ID == MapsID);
                }
                else
                {
                    repository.Insert(new Layers.Data.Sqls.PvbCrm.MapsBEnter
                    {
                        ID = this.MapsID,
                        Bussiness = (int)Business.WarehouseServicing,
                        Type = (int)MapsType.Contact,
                        EnterpriseID = this.EnterpriseID,
                        SubID = this.ID,
                        CreateDate = DateTime.Now,
                        CtreatorID = this.CreatorID,
                        IsDefault = this.IsDefault
                    });
                }
                if (this != null && this.EnterSuccess != null)
                {
                    this.EnterSuccess(this, new SuccessEventArgs(this));
                }
            }
        }
        public override void Abandon()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                if (repository.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>().Any(item => item.ID == this.MapsID))
                {
                    repository.Delete<Layers.Data.Sqls.PvbCrm.MapsBEnter>(item => item.ID == this.MapsID);
                }
                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this));
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// 不同业务关系下的联系人（传统贸易客户的联系人）
    /// </summary>
    public class TradingContact : Contact
    {
        string mapsid;
        virtual public string MapsID
        {
            get
            {
                return string.Join("", Business.Trading, MapsType.Contact, "_", base.ID + this.CreatorID).MD5();
            }
            set
            {
                this.mapsid = value;
            }
        }
        #region 
        public override event SuccessHanlder EnterSuccess;
        public override event SuccessHanlder AbandonSuccess;
        #endregion

        #region 持久化
        public override void Enter()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                //提货地id址是否存在
                if (repository.ReadTable<Layers.Data.Sqls.PvbCrm.Contacts>().Any(item => item.ID == this.ID))
                {
                    repository.Update<Layers.Data.Sqls.PvbCrm.Contacts>(new
                    {
                        Type = (int)this.Type,
                        Name = this.Name,
                        Tel = this.Tel,
                        Mobile = this.Mobile,
                        Email = this.Email,
                        Fax = this.Fax,
                        Status = (int)this.Status,
                        UpdateDate = DateTime.Now
                    }, item => item.ID == this.ID);
                }
                else
                {
                    //录入
                    repository.Insert(this.ToLinq());
                }
                //非超级管理员添加关系

                if (!repository.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>().Any(item => item.ID == this.MapsID))
                {
                    repository.Insert(new Layers.Data.Sqls.PvbCrm.MapsBEnter
                    {
                        ID = this.MapsID,
                        Bussiness = (int)Business.Trading,
                        Type = (int)MapsType.Contact,
                        EnterpriseID = this.EnterpriseID,
                        SubID = this.ID,
                        CreateDate = DateTime.Now,
                        CtreatorID = this.CreatorID,
                        IsDefault = this.IsDefault
                    });
                }

                if (this != null && this.EnterSuccess != null)
                {
                    this.EnterSuccess(this, new SuccessEventArgs(this));
                }
            }
        }
        public override void Abandon()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                if (repository.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>().Any(item => item.ID == this.MapsID))
                {
                    repository.Delete<Layers.Data.Sqls.PvbCrm.MapsBEnter>(item => item.ID == this.MapsID);
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