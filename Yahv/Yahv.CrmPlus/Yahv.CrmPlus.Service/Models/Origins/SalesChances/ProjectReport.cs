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
using YaHv.CrmPlus.Services.Models.Origins;

namespace Yahv.CrmPlus.Service.Models.Origins
{
    /// <summary>
    /// 客户报备
    /// </summary>
    public class ProjectReport : Yahv.Linq.IUnique,IDataEntity
    {

        #region  属性
        public string ID { get; set; }
        /// <summary>
        /// 终端客户
        /// </summary>

        public string ClientID { get; set; }


        //  public Client Client { get; set; }

        /// <summary>
        /// 销售机会ID
        /// </summary>

        public string ProjectID { get; set; }

        public Project Project { get; set; }

        public ProjectProduct ProjectProduct { get; set; }

        /// <summary>
        /// 标准型号ID
        /// </summary>
        public string SpnID { get; set; }

        public StandardPartNumber StandardPartNumber { get; set; }
        /// <summary>
        /// 报备人ID
        /// </summary>
        public string ReporterID { get; set; }

        public Admin Reporter { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ReportStatus ReportStatus { get; set; }

        public string Summary { get; set; }

        /// <summary>
        /// 原厂编号
        /// </summary>
        public string ProjectCode { get; set; }

        public Admin FAE { get; set; }

        public Admin PM { get; set; }

        public List<AgentQuote> lstAgentQuotes { get; set; }
        #endregion

        public ProjectReport()
        {
            this.CreateDate = this.ModifyDate = DateTime.Now;
            this.ReportStatus = ReportStatus.Waiting;
        }

        #region 持久化
        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvdCrmReponsitory>.Create())
            {

                //添加
                if (!reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.ProjectReports>().Any(item => item.ClientID == this.ClientID && item.SpnID == this.SpnID))
                {
                    this.ID = PKeySigner.Pick(Yahv.CrmPlus.Service.PKeyType.ProjectReport);
                    reponsitory.Insert(new Layers.Data.Sqls.PvdCrm.ProjectReports()
                    {
                        ID = this.ID,
                        ClientID = this.ClientID,
                        ProjectID = this.ProjectID,
                        SpnID = this.SpnID,
                        ReporterID = this.ReporterID,
                        CreateDate = this.CreateDate,
                        ModifyDate = this.ModifyDate,
                        Status = (int)this.ReportStatus,
                        ProjectCode = this.ProjectCode,
                        Summary = this.Summary

                    });
                }
                //修改
                else
                {
                    reponsitory.Update<Layers.Data.Sqls.PvdCrm.ProjectReports>(new
                    {
                        ClientID = this.ClientID,
                        ProjectID = this.ProjectID,
                        SpnID = this.SpnID,
                        ReporterID = this.ReporterID,
                        CreateDate = this.CreateDate,
                        ModifyDate = this.ModifyDate,
                        Status = (int)this.ReportStatus,
                        ProjectCode = this.ProjectCode,
                        Summary = this.Summary
                    }, item => item.ClientID == this.ClientID && item.SpnID == this.SpnID);
                }

            }
            this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this));
        }

        //public void Approve()
        //{

        //    {
        //        using (var repository = LinqFactory<PvdCrmReponsitory>.Create())
        //        {
        //            repository.Update<Layers.Data.Sqls.PvdCrm.ProjectReports>(new
        //            {
        //                Status = this.ReportStatus,
        //                this.ProjectCode,
        //                Summary = this.Summary
        //            }, item => item.ID == this.ID);
        //            if (this != null && this.EnterSuccess != null)
        //            {
        //                this.EnterSuccess(this, new SuccessEventArgs(this));
        //            }
        //        }
        //    }
        //}

        public void AddPrice()
        {

            using (var reponsitory = LinqFactory<PvdCrmReponsitory>.Create())
            using (var tran = reponsitory.OpenTransaction())
            {
                {

                    if (this.lstAgentQuotes.Count() > 0)
                    {
                        reponsitory.Delete<Layers.Data.Sqls.PvdCrm.AgentQuotes>(item => item.SpnID == this.SpnID && item.ClientID == this.ClientID);
                        reponsitory.Insert(this.lstAgentQuotes.Select(item => new Layers.Data.Sqls.PvdCrm.AgentQuotes
                        {
                            ID = PKeySigner.Pick(Yahv.CrmPlus.Service.PKeyType.AgentQuotes),
                            SpnID = this.SpnID,
                            QuoteType = (int)item.QuoteType,
                            ClientID = this.ClientID,
                            MinQuantity = item.MinQuantity,
                            MaxQuantity = item.MaxQuantity,
                            Currency = (int)item.Currency,
                            UnitCostPrice = item.UnitCostPrice,
                            ResalePrice = item.ResalePrice,
                            ApprovedPrice = item.ApprovedPrice,
                            ProfitRate = item.ProfitRate,
                            StartDate = item.StartDate,
                            EndDate = item.EndDate,
                            Status = (int)item.Status,
                            CreatorID = item.CreatorID
                        }).ToArray());

                    }
                }
                tran.Commit();
            }
          //  this.AbandonSuccess.Invoke(this, new SuccessEventArgs(this));
            this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this));



        }


      
        #endregion

        #region 事件
        public event SuccessHanlder EnterSuccess;



        public event SuccessHanlder AbandonSuccess;
        /// <summary>
        /// EnterError
        /// </summary>

        public event ErrorHanlder EnterError;
        #endregion
    }
}
