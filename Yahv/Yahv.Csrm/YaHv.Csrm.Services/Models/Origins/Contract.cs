using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Usually;
using Yahv.Utils.Converters.Contents;
using Yahv.Underly;
using YaHv.Csrm.Services.Extends;
using Yahv.Services.Models;

namespace YaHv.Csrm.Services.Models.Origins
{
    //代仓储客户的电子合同
    public class Contract : Yahv.Linq.IUnique
    {
        public Contract()
        {
            this.Status = GeneralStatus.Normal;
            this.CreateDate = this.UpdateDate = DateTime.Now;
        }
        #region 属性
        string id;
        public string ID
        {
            get
            {
                //结合有效数据，以及子项的ID（子项ID已做变更验证）验证是否需插入(不同的内部公司)
                return this.id ?? string.Concat(this.Enterprise.ID, this.StartDate, this.EndDate, this.AgencyRate, this.MinAgencyFee, this.ExchangeMode, this.InvoiceType, this.InvoiceTaxRate).MD5();
            }
            set
            {
                this.id = value;
            }
        }
        /// <summary>
        /// 客户企业信息
        /// </summary>
        public Enterprise Enterprise { set; get; }
        /// <summary>
        /// 合同协议开始时间
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 合同协议结束日期
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 代理费率
        /// </summary>
        public decimal AgencyRate { get; set; }

        /// <summary>
        /// 最低代理费
        /// </summary>
        public decimal MinAgencyFee { get; set; }
        /// <summary>
        /// 换汇方式：预换汇，90天内换汇
        /// </summary>
        public ExchangeMode ExchangeMode { get; set; }

        /// <summary>
        /// 开票类型:服务费发票，全额发票
        /// </summary>
        public BillingType InvoiceType { get; set; }

        /// <summary>
        /// 开票的税率 增值税 16%  服务费：3% 6% 固定不变，可以用系统常量表示
        /// </summary>
        public decimal InvoiceTaxRate { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { set; get; }
        public string CreatorID { set; get; }
        /// <summary>
        /// 添加人
        /// </summary>
        public Admin Creator { internal set; get; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { set; get; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime UpdateDate { set; get; }
        /// <summary>
        /// 状态
        /// </summary>
        public GeneralStatus Status { set; get; }
        /// <summary>
        /// 服务协议
        /// </summary>

        //Views.Rolls.FilesRoll serviceagreement;
        //public FileDescription ServiceAgreement
        //{
        //    get
        //    {
        //        if (this.serviceagreement == null || this.serviceagreement.Disposed)
        //        {
        //            this.serviceagreement = new Views.Rolls.FilesRoll(this.Enterprise, FileType.ServiceAgreement, this.Company.ID);
        //        }
        //        return this.serviceagreement.FirstOrDefault(item => item.Type == FileType.ServiceAgreement && item.Status == ApprovalStatus.Normal);
        //    }
        //}
        #region 协议文件
        CenterFileDescription serviceagreement;
        public CenterFileDescription ServiceAgreement
        {
            get
            {
                if (this.serviceagreement == null)
                {
                    this.serviceagreement = new Views.Rolls.CenterFiles(FileType.ServiceAgreement, this.Enterprise.ID).FirstOrDefault();
                }
                return this.serviceagreement;
            }
            set
            {
                this.serviceagreement = value;
            }
        }
        #endregion
        internal string CompanyID { set; get; }

        public Enterprise Company { set; get; }
        public string MapsID
        {
            get { return string.Join("", "Contract_", this.ID); }
        }
        #endregion


        #region 事件
        /// <summary>
        /// EnterSuccess
        /// </summary>
        virtual public event SuccessHanlder EnterSuccess;
        virtual public event SuccessHanlder AbandonSuccess;
        #endregion

        #region 持久化
        public void Enter()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                var contractids = new Views.Rolls.ContractsRoll(this.Enterprise, this.Company.ID).Select(item => item.ID).ToArray();

                if (!repository.ReadTable<Layers.Data.Sqls.PvbCrm.Contracts>().Any(item => item.ID == this.ID))
                {
                    if (contractids.Count() > 0)
                    {
                        //失效协议
                        repository.Update<Layers.Data.Sqls.PvbCrm.Contracts>(new { Status = (int)GeneralStatus.Deleted }, item => contractids.Contains(item.ID));
                    }
                    repository.Insert(this.ToLinq());
                }
                else
                {
                    repository.Update<Layers.Data.Sqls.PvbCrm.Contracts>(new { Status = (int)GeneralStatus.Normal }, item => item.ID == this.ID);
                }

                //合同与内部公司的关系
                if (!repository.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>().Any(item => item.ID == this.MapsID))
                {
                    repository.Insert(new Layers.Data.Sqls.PvbCrm.MapsBEnter
                    {
                        ID = this.MapsID,
                        Bussiness = (int)Business.WarehouseServicing,
                        Type = (int)MapsType.Contract,
                        EnterpriseID = this.Company.ID,
                        SubID = this.ID,
                        CreateDate = this.CreateDate,
                        CtreatorID = CreatorID,
                        IsDefault = false
                    });
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
                if (repository.ReadTable<Layers.Data.Sqls.PvbCrm.Contracts>().Any(item => item.ID == this.ID))
                {
                    repository.Update<Layers.Data.Sqls.PvbCrm.Contracts>(new { Status = (int)GeneralStatus.Deleted }, item => item.ID == this.ID);
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
