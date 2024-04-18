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
using Yahv.Utils.Converters.Contents;
using Yahv.Utils.Serializers;

namespace Yahv.Services.Models
{
    /// <summary>
    /// 供应商的私有交货地址
    /// </summary>
    public class wsnSupplierConsignor : IUnique
    {
        #region 属性
        public string ID { set; get; }
        /// <summary>
        /// 供应商ID
        /// </summary>
        public string nSupplierID { set; get; }
        /// <summary>
        /// 所属企业ID：客户ID
        /// </summary>
        public string OwnID { set; get; }
        /// <summary>
        /// 所属企业名称：客户名称
        /// </summary>
        public string OwnName { set; get; }
        /// <summary>
        /// 真正的所属ID：供应商ID
        /// </summary>
        public string RealEnterpriseID { set; get; }
        /// <summary>
        /// 真正的所属名称：供应商名称
        /// </summary>
        public string RealEnterpriseName { set; get; }
        /// <summary>
        /// 供应商等级
        /// </summary>
        public SupplierGrade nGrade { set; get; }
        /// <summary>
        /// 收件单位或其他
        /// </summary>
        public string Title { set; get; }
        /// <summary>
        /// 邮编
        /// </summary>
        public string PostZip { set; get; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { set; get; }
        /// <summary>
        /// 国家/地区
        /// </summary>
        public string Place { set; get; }
        /// <summary>
        /// 联系人姓名
        /// </summary>
        public string Contact { set; get; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { set; get; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Tel { set; get; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { set; get; }
        /// <summary>
        /// 客户入仓号
        /// </summary>
        public string EnterCode { set; get; }
        /// <summary>
        /// 客户等级
        /// </summary>
        public ClientGrade ClientGrade { set; get; }
        /// <summary>
        /// 是否默认
        /// </summary>
        public bool IsDefault { set; get; }
        /// <summary>
        /// 状态
        /// </summary>
        public GeneralStatus Status { set; get; }
        /// <summary>
        /// 录入人
        /// </summary>
        public string CreatorID { set; get; }
        /// <summary>
        /// 唯一性标志
        /// </summary>
        string uniquesign;

        string UniqueSign
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
                    var consignors = new Views.wsnSupplierConsignorsTopView<PvbCrmReponsitory>().Where(item => item.nSupplierID == this.nSupplierID).ToArray();
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
                        this.ID = PKeySigner.Pick(Yahv.Underly.PKeyType.nConsignor);
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
                    repository.Update(this.ToLinq(), item => item.ID == this.ID); if (this != null && this.EnterSuccess != null)
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
        /// <summary>
        /// 同步方法
        /// </summary>
        /// <returns>接口返回结果</returns>
        public object Synchro()
        {
            if (this.Place == Origin.HKG.GetOrigin().Code)
            {
                string url = Commons.UnifyApiUrl + "/suppliers/address";
                var json = new
                {
                    Enterprise = new Enterprise { Name = this.OwnName },
                    SupplierName = this.RealEnterpriseName,
                    Address = this.Address.Replace("中国 ", ""),
                    Name = this.Contact,
                    Tel = this.Tel,
                    Mobile = this.Mobile,
                    Place = this.Place,
                    IsDefault = this.IsDefault
                }.Json();
                var response = Commons.HttpPostRaw(url, json);
                return response;
            }
            return "非香港地区不可同步";
        }
        /// <summary>
        /// 同步删除
        /// </summary>
        /// <param name="OwnName">OwnName是客户名称</param>
        /// <param name="RealEnterpriseName">供应商的企业名称</param>
        /// <param name="address"></param>
        public void AbandonSynchro()
        {
            if (this.Place == Origin.CHN.GetOrigin().Code)
            {
                string url = Commons.UnifyApiUrl + "/suppliers/address";
                Commons.CommonHttpRequest(url + "?name=" + this.OwnName + "&supplierName=" + this.RealEnterpriseName + "&address=" + this.Address.Replace("中国 ", ""), "DELETE");
            }
        }
        #endregion
    }
}
