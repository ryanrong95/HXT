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
    /// 受益人 单体
    /// </summary>
    public class Beneficiary : Yahv.Linq.IUnique
    {
        public Beneficiary()
        {
            this.Status = ApprovalStatus.Normal;
            this.CreateDate = this.UpdateDate = DateTime.Now;
        }
        #region 属性
        /// <summary>
        /// 受益人标识号
        /// </summary>
        string id;
        /// <summary>
        /// 唯一码
        /// </summary>
        /// <chenhan>保障全局唯一</chenhan>
        virtual public string ID
        {
            get
            {
                return this.id ?? string.Join("",
                    this.EnterpriseID,
                    this.RealID,
                    this.Bank,
                    this.BankAddress,
                    this.Account,
                    this.SwiftCode,
                    this.Methord,
                    this.Currency).MD5();
            }
            set
            {
                this.id = value;
            }
        }
        /// <summary>
        /// 企业ID
        /// </summary>
        public string EnterpriseID { set; get; }
        string realid;
        public string RealID
        {
            get
            {
                return this.realid ?? this.RealName.MD5();
            }
            set { this.realid = value; }
        }
        /// <summary>
        /// 是否可以带票采购
        /// </summary>
        public InvoiceType? InvoiceType { set; get; }
        /// <summary>
        /// 真实姓名
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        /// 开户银行
        /// </summary>
        public string Bank { get; set; }
        /// <summary>
        /// 开户行地址
        /// </summary>
        public string BankAddress { get; set; }
        /// <summary>
        /// 银行账户
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 银行编码 (国际)
        /// </summary>
        public string SwiftCode { get; set; }
        /// <summary>
        /// 汇款方式
        /// </summary>
        public Methord Methord { get; set; }
        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { get; set; }
        /// <summary>
        /// 地区
        /// </summary>
        public District District { set; get; }
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

        public string CreatorID { get; set; }

        /// <summary>
        /// 添加人
        /// </summary>
        public Admin Creator { get; internal set; }
        /// <summary>
        /// 默认受益人
        /// </summary>
        public bool IsDefault { set; get; }
        /// <summary>
        /// 银行代码（行号）
        /// </summary>
        public string BankCode { set; get; }
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

                if (repository.GetTable<Layers.Data.Sqls.PvbCrm.Beneficiaries>().Any(item => item.ID == this.ID))
                {
                    repository.Update(this.ToLinq(), item => item.ID == this.ID);
                }
                else
                {
                    repository.Insert(this.ToLinq());
                    this.Creator = new Admin
                    {
                        ID = this.CreatorID
                    };
                    this.Creator.Binding(this.EnterpriseID, this.ID, MapsType.Beneficiary, IsDefault);
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
                repository.Update<Layers.Data.Sqls.PvbCrm.Beneficiaries>(new
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


    public class WsBeneficiary : Beneficiary
    {
        public string MapsID
        {
            get
            {
                return "WsBeneficiary_" + string.Join("", this.WsClient.ID, this.ID).MD5();
            }

        }
        public Enterprise WsClient { set; get; }
        public override event SuccessHanlder EnterSuccess;
        public override event SuccessHanlder AbandonSuccess;
        public override void Enter()
        {
            this.Status = ApprovalStatus.Normal;
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                //Beneficiary
                if (repository.ReadTable<Layers.Data.Sqls.PvbCrm.Beneficiaries>().Any(item => item.ID == this.ID))
                {
                    repository.Update(this.ToLinq(), item => item.ID == this.ID);
                }
                else
                {
                    repository.Insert(this.ToLinq());
                }

                //MapsBEnter
                if (this.IsDefault)//默认
                {
                    repository.Update<Layers.Data.Sqls.PvbCrm.MapsBEnter>(new
                    {
                        IsDefault = false
                    }, item => item.EnterpriseID == this.WsClient.ID && item.Type == (int)MapsType.Beneficiary && item.Bussiness == (int)Business.WarehouseServicing);
                }
                if (repository.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>().Any(item => item.ID == this.MapsID))//关系是否存在
                {
                    repository.Update<Layers.Data.Sqls.PvbCrm.MapsBEnter>(new
                    {
                        IsDefault = this.IsDefault
                    }, item => item.ID == MapsID);
                }
                else
                {
                    repository.Insert(new Layers.Data.Sqls.PvbCrm.MapsBEnter
                    {
                        ID = this.MapsID,
                        Bussiness = (int)Business.WarehouseServicing,
                        Type = (int)MapsType.Beneficiary,
                        EnterpriseID = this.WsClient.ID,
                        SubID = base.ID,
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
        /// <summary>
        /// 删除关系
        /// </summary>
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

    public class TradingBeneficiary : Beneficiary
    {
        public string MapsID
        {
            get
            {
                return string.Join("", Business.Trading, MapsType.Beneficiary, "_", base.ID, this.CreatorID).MD5();
            }
        }
        public override event SuccessHanlder EnterSuccess;
        public override event SuccessHanlder AbandonSuccess;
        public override void Enter()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                //Beneficiary
                if (repository.GetTable<Layers.Data.Sqls.PvbCrm.Beneficiaries>().Any(item => item.ID == this.ID))
                {
                    repository.Update(this.ToLinq(), item => item.ID == this.ID);
                }
                else
                {
                    repository.Insert(this.ToLinq());
                }

                //MapsBEnter
                if (this.IsDefault)//默认
                {
                    repository.Update<Layers.Data.Sqls.PvbCrm.MapsBEnter>(new
                    {
                        IsDefault = false
                    }, item => item.ID == this.MapsID);
                }
                if (repository.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>().Any(item => item.ID == this.MapsID))//关系是否存在
                {
                    repository.Update<Layers.Data.Sqls.PvbCrm.MapsBEnter>(new
                    {
                        IsDefault = this.IsDefault
                    }, item => item.ID == MapsID);
                }
                else
                {
                    repository.Insert(new Layers.Data.Sqls.PvbCrm.MapsBEnter
                    {
                        ID = this.MapsID,
                        Bussiness = (int)Business.Trading,
                        Type = (int)MapsType.Beneficiary,
                        EnterpriseID = this.EnterpriseID,
                        SubID = base.ID,
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
        /// <summary>
        /// 删除关系
        /// </summary>
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