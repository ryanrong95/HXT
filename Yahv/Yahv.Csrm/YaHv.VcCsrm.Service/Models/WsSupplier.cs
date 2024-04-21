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
using YaHv.VcCsrm.Service.Extends;

namespace YaHv.VcCsrm.Service.Models
{
    public class WsSupplier : Yahv.Linq.IUnique
    {
        #region 属性
        public string ID { set; get; }
        string enterpriseid;
        public string EnterpriseID
        {
            get
            {
                return enterpriseid ?? this.EnglishName.MD5();
            }
            set
            {
                this.enterpriseid = value;
            }
        }
        /// <summary>
        /// 供应商企业名称
        /// </summary>
       // public string Name { set; get; }
        /// <summary>
        /// 中文名称
        /// </summary>

        public string ChineseName { set; get; }
        /// <summary>
        /// 英文名称
        /// </summary>
        public string EnglishName { set; get; }
        /// <summary>
        /// 供应商等级
        /// </summary>
        public SupplierGrade Grade { set; get; }
        /// <summary>
        /// 状态
        /// </summary>
        public ApprovalStatus Status { set; get; }
        /// <summary>
        /// 企业法人
        /// </summary>

        public string Corperation { set; get; }
        /// <summary>
        /// 注册地址
        /// </summary>
        public string RegAddress { set; get; }
        /// <summary>
        /// 统一社会信用代码
        /// </summary>
        public string Uscc { set; get; }
        /// <summary>
        /// 状态
        /// </summary>
        public ApprovalStatus WsSupplierStatus { set; get; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { set; get; }
        /// <summary>
        /// 添加人ID
        /// </summary>
        public string CreatorID { set; get; }
        /// <summary>
        /// 国家、地区，（Origin的简称）
        /// </summary>
        public string Origin { set; get; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { set; get; }
        public DateTime UpdateDate { set; get; }

        /// <summary>
        /// 合作公司
        /// </summary>
        public string ShipID { set; get; }
        #endregion

        #region 子项信息
        Views.Rolls.WsPayeesRoll wspayees;
        /// <summary>
        /// 代仓储供应商的收款人
        /// </summary>
        public Views.Rolls.WsPayeesRoll WsPayees
        {
            get
            {
                if (this.wspayees == null || this.wspayees.Disposed)
                {
                    this.wspayees = new Views.Rolls.WsPayeesRoll(this.ID);
                }
                return this.wspayees;
            }
        }
        Views.Rolls.WsConsignorsRoll wsconsignors;
        /// <summary>
        /// 代仓储供应商的发货地址
        /// </summary>
        public Views.Rolls.WsConsignorsRoll WsConsifnors
        {
            get
            {
                if (this.wsconsignors == null || this.wsconsignors.Disposed)
                {
                    this.wsconsignors = new Views.Rolls.WsConsignorsRoll(this.ID);
                }
                return this.wsconsignors;
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
            using (var repository = LinqFactory<PvcCrmReponsitory>.Create())
            {
                (new Enterprise
                {
                    Name = this.EnglishName,
                    Corporation = this.Corperation,
                    RegAddress = this.RegAddress,
                    Uscc = this.Uscc,
                    AdminCode = ""
                }).Enter();
                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    if (repository.ReadTable<Layers.Data.Sqls.PvcCrm.WsSuppliers>().Any(item => item.EnterpriseID == this.EnterpriseID && item.ShipID == this.ShipID))
                    {
                        //供应商已存在
                    }
                    else
                    {
                        repository.Insert(this.ToLinq());
                    }
                }
                else
                {
                    repository.Update<Layers.Data.Sqls.PvcCrm.WsSuppliers>(new
                    {
                        EnterpriseID = this.EnterpriseID,
                        ShipID = this.ShipID,
                        Grade = (int)this.Grade,
                        EnglishName = this.EnglishName,
                        ChineseName = this.ChineseName,
                        Status = (int)this.Status,
                        AdminID = this.CreatorID,
                        Summary = this.Summary,
                        UpdateDate = this.UpdateDate,
                        Origin = this.Origin
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
            using (var repository = LinqFactory<PvcCrmReponsitory>.Create())
            {
                if (repository.ReadTable<Layers.Data.Sqls.PvcCrm.WsSuppliers>().Any(item => item.ID == this.ID))
                {
                    repository.Update<Layers.Data.Sqls.PvcCrm.WsSuppliers>(new
                    {
                        Status = (int)GeneralStatus.Deleted
                    }, item => item.ID == this.ID);
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
