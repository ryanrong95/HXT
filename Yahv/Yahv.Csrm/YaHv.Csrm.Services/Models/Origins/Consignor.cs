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
    ///发货人，提货地址
    /// </summary>
    public class Consignor : Yahv.Linq.IUnique
    {
        public Consignor()
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
        public Admin Admin { get; internal set; }
        /// <summary>
        /// 是否默认
        /// </summary>
        public bool IsDefault { set; get; }
        /// <summary>
        /// 国家/地区
        /// </summary>
        public string Place { set; get; }
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
                //默认地址修改
                if (this.IsDefault)
                {
                    repository.Update<Layers.Data.Sqls.PvbCrm.Consignors>(new { IsDefault = false }, item => item.EnterpriseID == this.EnterpriseID);
                }
                if (repository.GetTable<Layers.Data.Sqls.PvbCrm.Consignors>().Any(item => item.ID == this.ID))
                {
                    this.UpdateDate = DateTime.Now;
                    //修改
                    repository.Update<Layers.Data.Sqls.PvbCrm.Consignors>(new
                    {
                        Title = this.Title,
                        DyjCode = this.DyjCode,
                        Province = this.Province,
                        City = this.City,
                        Land = this.Land,
                        Address = this.Address,
                        Postzip = this.Postzip,
                        Name = this.Name,
                        Tel = this.Tel,
                        Mobile = this.Mobile,
                        Email = this.Email,
                        Status = (int)this.Status,
                        UpdateDate = this.UpdateDate,
                        IsDefault = this.IsDefault,
                        Place = this.Place
                    }, item => item.ID == this.ID);
                }
                else
                {
                    this.Status = ApprovalStatus.Normal;
                    this.CreateDate = this.UpdateDate = DateTime.Now;
                    //录入
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
                repository.Update<Layers.Data.Sqls.PvbCrm.Consignors>(new
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

    public class WsConsignor : Consignor
    {
        public string MapsID
        {
            get
            {
                return "WsConsignor_" + string.Join("", this.WsClient.ID, this.ID).MD5();
            }
        }
        public Enterprise WsClient { set; get; }
        public override event SuccessHanlder EnterSuccess;
        public override event SuccessHanlder AbandonSuccess;
        public override void Enter()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                //提货地id址是否存在
                if (repository.ReadTable<Layers.Data.Sqls.PvbCrm.Consignors>().Any(item => item.ID == this.ID))
                {
                    repository.Update<Layers.Data.Sqls.PvbCrm.Consignors>(new
                    {
                        Title = this.Title,
                        DyjCode = this.DyjCode,
                        Province = this.Province,
                        City = this.City,
                        Land = this.Land,
                        Address = this.Address,
                        Postzip = this.Postzip,
                        Name = this.Name,
                        Tel = this.Tel,
                        Mobile = this.Mobile,
                        Email = this.Email,
                        Status = (int)this.Status,
                        UpdateDate = DateTime.Now,
                        IsDefault = this.IsDefault,
                        Place = this.Place
                    }, item => item.ID == this.ID);
                }
                else
                {
                    this.Status = ApprovalStatus.Normal;
                    this.CreateDate = this.UpdateDate = DateTime.Now;
                    //录入
                    repository.Insert(this.ToLinq());
                }
                //是否默认
                if (this.IsDefault)
                {
                    repository.Update<Layers.Data.Sqls.PvbCrm.MapsBEnter>(new
                    {
                        IsDefault = false
                    }, item => item.EnterpriseID == this.WsClient.ID && item.Type == (int)MapsType.Consignor && item.Bussiness == (int)Business.WarehouseServicing);
                }
                //关系
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
                        Type = (int)MapsType.Consignor,
                        EnterpriseID = this.WsClient.ID,
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

    }
}