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
    public class nConsignor : Yahv.Linq.IUnique
    {
        public nConsignor()
        {
            this.CreateDate = this.UpdateDate = DateTime.Now;
            this.Status = GeneralStatus.Normal;
        }
        #region 属性
        /// <summary>
        ///ID
        /// </summary>
        public string ID { set; get; }
        /// <summary>
        /// 所属供应商ID
        /// </summary>
        public string nSupplierID { set; get; }
        /// <summary>
        /// 所属企业ID
        /// </summary>
        public string EnterpriseID { set; get; }
        /// <summary>
        ///真实所属企业ID
        /// </summary>
        public string RealID { set; get; }
        /// <summary>
        /// 名称，主要用于库房的子库房名称
        /// </summary>
        public string Title { set; get; }
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
        /// 联系人姓名
        /// </summary>
        public string Contact { set; get; }
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
        public GeneralStatus Status { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; internal set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateDate { get; internal set; }
        public string Creator { set; get; }
        /// <summary>
        /// 是否默认
        /// </summary>
        public bool IsDefault { set; get; }
        /// <summary>
        /// 唯一性标志
        /// </summary>
        string uniquesign;

        public string UniqueSign
        {
            get
            {
                return this.uniquesign ?? string.Join("",
                    this.nSupplierID,
                    this.Address,
                    this.Contact,
                    this.Mobile).MD5();
            }
            set
            {
                this.uniquesign = value;
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
        public event ErrorHanlder Repeat;
        #endregion

        #region 持久化
        public void Enter()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {

                if (string.IsNullOrEmpty(this.ID))
                {
                    var consignors = new Views.Rolls.nConsignorsRoll(this.nSupplierID).ToArray();
                    if (consignors.Any(item => item.UniqueSign == this.UniqueSign))
                    {
                        if (this != null && this.Repeat != null)
                        {
                            this.Repeat(this, new ErrorEventArgs());
                        }
                    }
                    else
                    {
                        //默认地址修改
                        if (this.IsDefault)
                        {
                            repository.Update<Layers.Data.Sqls.PvbCrm.nConsignors>(new { IsDefault = false }, item => item.nSupplierID == this.nSupplierID);
                        }
                        this.ID = PKeySigner.Pick(PKeyType.nConsignor);
                        //录入
                        repository.Insert(this.ToLinq()); if (this != null && this.EnterSuccess != null)
                        {
                            this.EnterSuccess(this, new SuccessEventArgs(this));
                        }

                    }
                }
                else
                {
                    //默认地址修改
                    if (this.IsDefault)
                    {
                        repository.Update<Layers.Data.Sqls.PvbCrm.nConsignors>(new { IsDefault = false }, item => item.nSupplierID == this.nSupplierID);
                    }
                    //修改
                    repository.Update<Layers.Data.Sqls.PvbCrm.nConsignors>(new 
                    {
                        Title = this.Title,
                        EnterpriseID = this.EnterpriseID,
                        RealID = this.RealID,
                        Province = this.Province,
                        City = this.City,
                        Land = this.Land,
                        Address = this.Address,
                        Postzip = this.Postzip,
                        Contact = this.Contact,
                        Tel = this.Tel,
                        Mobile = this.Mobile,
                        Email = this.Email,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = DateTime.Now,
                        IsDefault = this.IsDefault,
                        Place = this.Place
                    }, item => item.ID == this.ID);
                    if (this != null && this.EnterSuccess != null)
                    {
                        this.EnterSuccess(this, new SuccessEventArgs(this));
                    }
                }


            }
        }
        public void Abandon()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                repository.Update<Layers.Data.Sqls.PvbCrm.nConsignors>(new
                {
                    Status = GeneralStatus.Deleted
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
