using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Linq;
using Yahv.Underly;
using Yahv.Usually;
using Yahv.Utils.Converters.Contents;
using YaHv.Csrm.Services.Extends;

namespace YaHv.Csrm.Services.Models.Origins
{
    /// <summary>
    /// 收货人（收件人）
    /// </summary>
    public class Consignee : Yahv.Linq.IUnique
    {
        public Consignee()
        {
            this.CreateDate = this.UpdateDate = DateTime.Now;
            this.Status = ApprovalStatus.Normal;
        }
        #region 属性
        string id;
        /// <summary>
        /// 
        /// </summary>
        virtual public string ID
        {
            get
            {
                return this.id ?? string.Join("",
                    this.EnterpriseID,
                    this.DyjCode,
                    this.District,
                    this.Address,
                    this.Postzip
                    ).MD5();
            }
            set { this.id = value; }
        }
        public string EnterpriseID { set; get; }
        /// <summary>
        /// 名称，主要用于库房的子库房名称
        /// </summary>
        public string Title { set; get; }
        /// <summary>
        /// 大赢家Code
        /// </summary>
        public string DyjCode { set; get; }
        /// <summary>
        /// 地区
        /// </summary>
        public District District { set; get; }
        /// <summary>
        /// 省
        /// </summary>
        public string Province { set; get; }
        /// <summary>
        /// 市
        /// </summary>
        public string City { set; get; }
        /// <summary>
        /// 地
        /// </summary>
        public string Land { set; get; }
        /// <summary>
        /// 收货地址
        /// </summary>
        public string Address { set; get; }
        /// <summary>
        /// 邮编
        /// </summary>
        public string Postzip { set; get; }
        /// <summary>
        /// 联系人类型
        /// </summary>
       // public ContactType Type { get; set; }
        /// <summary>
        /// 联系人姓名
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string Tel { get; set; }
        /// <summary>
        /// 联系人手机号
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 联系人邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 国家/地区
        /// </summary>
        public string Place { set; get; }
        /// <summary>
        /// 状态
        /// </summary>
        public ApprovalStatus Status { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; internal set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateDate { get; internal set; }
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
        /// 是否默认
        /// </summary>
        public bool IsDefault { set; get; }
        /// <summary>
        /// 门牌编码，库房使用
        /// </summary>
        public string PlateCode { set; get; }
        #endregion

        #region 事件
        /// <summary>
        /// EnterSuccess
        /// </summary>
        virtual public event SuccessHanlder EnterSuccess;
        /// <summary>
        /// AbandonSuccess
        /// </summary>
        virtual public event SuccessHanlder AbandonSuccess;
        #endregion

        #region 持久化
        virtual public void Enter()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                if (repository.GetTable<Layers.Data.Sqls.PvbCrm.Consignees>().Any(item => item.ID == this.ID))
                {
                    //修改
                    repository.Update(this.ToLinq(), item => item.ID == this.ID);
                }
                else
                {
                    //录入
                    repository.Insert(this.ToLinq());
                    this.Creator = new Admin
                    {
                        ID = this.CreatorID
                    };
                    this.Creator.Binding(this.EnterpriseID, this.ID, MapsType.Consignee, IsDefault);
                }
                //联系人
                if (!string.IsNullOrWhiteSpace(this.Name))
                {
                    new Contact
                    {
                        EnterpriseID = this.EnterpriseID,
                        Type = ContactType.Sales,//到货地址的联系人类型是：销售，已确认
                        Enterprise = this.Enterprise,
                        Name = this.Name,
                        Tel = this.Tel,
                        Mobile = this.Mobile,
                        Email = this.Email,
                        Status = Services.Status.Normal,//联系人的状态
                        Creator = this.Creator
                    }.Enter();
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
                repository.Update<Layers.Data.Sqls.PvbCrm.Consignees>(new
                {
                    Status = ApprovalStatus.Deleted
                }, item => item.ID == this.ID);
                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this));
                }
            }
        }
        #endregion
    }

    public class WsConsignee : Consignee
    {
        public string MapsID
        {
            get
            {
                return "WsConsignee_" + base.ID;
            }

        }
        #region 成功事件
        /// <summary>
        /// EnterSuccess
        /// </summary>
        override public event SuccessHanlder EnterSuccess;
        public override event SuccessHanlder AbandonSuccess;
        #endregion
        #region 持久化
        override public void Enter()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                //Consignee
                if (repository.GetTable<Layers.Data.Sqls.PvbCrm.Consignees>().Any(item => item.ID == this.ID))
                {
                    repository.Update(this.ToLinq(), item => item.ID == this.ID);
                }
                else
                {
                    repository.Insert(this.ToLinq());
                }

                //MapsBEnter
                //设置默认
                if (this.IsDefault)
                {
                    repository.Update<Layers.Data.Sqls.PvbCrm.MapsBEnter>(new
                    {
                        IsDefault = false
                    }, item => item.EnterpriseID == this.EnterpriseID && item.Type == (int)MapsType.Consignee && item.Bussiness == (int)Business.WarehouseServicing);
                }
                if (repository.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>().Any(item => item.ID == this.MapsID))
                {
                    repository.Update<Layers.Data.Sqls.PvbCrm.MapsBEnter>(new
                    {
                        IsDefault = this.IsDefault
                    }, item => item.ID == this.MapsID);
                }
                else
                {
                    repository.Insert(new Layers.Data.Sqls.PvbCrm.MapsBEnter
                    {
                        ID = this.MapsID,
                        Bussiness = (int)Business.WarehouseServicing,
                        Type = (int)MapsType.Consignee,
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
        //删除关系
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
            #endregion

        }
    }


    public class TradingConsignee : Consignee
    {
        virtual public string MapsID
        {
            get
            {
                return string.Join("", Business.Trading, MapsType.Consignee, "_", base.ID + this.CreatorID).MD5();
            }

        }
        #region 成功事件
        /// <summary>
        /// EnterSuccess
        /// </summary>
        override public event SuccessHanlder EnterSuccess;
        public override event SuccessHanlder AbandonSuccess;
        #endregion
        #region 持久化
        override public void Enter()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                //Consignee
                if (repository.GetTable<Layers.Data.Sqls.PvbCrm.Consignees>().Any(item => item.ID == this.ID))
                {
                    repository.Update(this.ToLinq(), item => item.ID == this.ID);
                }
                else
                {
                    repository.Insert(this.ToLinq());
                }

                //设置默认
                if (this.IsDefault)
                {
                    repository.Update<Layers.Data.Sqls.PvbCrm.MapsBEnter>(new
                    {
                        IsDefault = false
                    }, item => item.EnterpriseID == this.EnterpriseID && item.Type == (int)MapsType.Consignee && item.Bussiness == (int)Business.WarehouseServicing);
                }
                if (repository.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>().Any(item => item.ID == this.MapsID))
                {
                    repository.Update<Layers.Data.Sqls.PvbCrm.MapsBEnter>(new
                    {
                        IsDefault = this.IsDefault
                    }, item => item.ID == this.MapsID);
                }
                else
                {
                    repository.Insert(new Layers.Data.Sqls.PvbCrm.MapsBEnter
                    {
                        ID = this.MapsID,
                        Bussiness = (int)Business.Trading,
                        Type = (int)MapsType.Consignee,
                        EnterpriseID = this.EnterpriseID,
                        SubID = this.ID,
                        CreateDate = this.CreateDate,
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
        //删除关系
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
            #endregion

        }
    }

    public class SupplierConsignee : TradingConsignee
    {
        override public string MapsID
        {
            get
            {
                return string.Join("", Business.Trading, MapsType.Consignee, base.ID).MD5();
            }

        }
    }
}