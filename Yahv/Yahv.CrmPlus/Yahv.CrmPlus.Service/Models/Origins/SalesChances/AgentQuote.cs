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

namespace Yahv.CrmPlus.Service.Models.Origins
{
    public class AgentQuote : Yahv.Linq.IUnique
    {
        #region  属性
        public string ID { get; set; }
        /// <summary>
        /// 标准型号ID
        /// </summary>

        public string SpnID { get; set; }
        /// <summary>
        /// 报价类型：StepPrice,CSP,Project
        /// </summary>

        public QuoteType QuoteType { get; set; }

        public string ClientID { get; set; }



        public int MinQuantity { set; get; }
        public int? MaxQuantity { set; get; }
        /// <summary>
        /// 所属人
        /// </summary>
        public Currency Currency { get; set; }
        /// <summary>
        /// 官网价格
        /// </summary>
        public decimal UnitCostPrice { get; set; }
        /// <summary>
        /// 建议售价
        /// </summary>
        public decimal ResalePrice { get; set; }


        /// <summary>
        /// 采购价格
        /// </summary>
        public decimal? ApprovedPrice { get; set; }
        /// <summary>
        /// 进价利润率
        /// </summary>

        public decimal? ProfitRate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }


        public DataStatus Status { get; set; }


        public string CreatorID { get; set; }

        #endregion

        public AgentQuote()
        {
            this.Status = DataStatus.Normal;
        }

        #region 持久化
        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvdCrmReponsitory>.Create())
            using (var tran = reponsitory.OpenTransaction())
            {
                {
                    #region  报价
                    //添加
                    if (!reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.AgentQuotes>().Any(item => item.SpnID == this.SpnID && item.ClientID==this.ClientID))
                    {
                        this.ID = PKeySigner.Pick(Yahv.CrmPlus.Service.PKeyType.AgentQuotes);
                        reponsitory.Insert(new Layers.Data.Sqls.PvdCrm.AgentQuotes()
                        {
                            ID = this.ID,
                            SpnID = this.SpnID,
                            QuoteType = (int)this.QuoteType,
                            ClientID = this.ClientID,
                            MinQuantity = this.MinQuantity,
                            MaxQuantity = this.MaxQuantity,
                            Currency = (int)this.Currency,
                            UnitCostPrice = this.UnitCostPrice,
                            ResalePrice = this.ResalePrice,
                            ApprovedPrice = this.ApprovedPrice,
                            ProfitRate = this.ProfitRate,
                            StartDate = this.StartDate,
                            EndDate = this.EndDate,
                            Status = (int)this.Status,
                            CreatorID = this.CreatorID
                        });
                    }
                    //修改
                    else
                    {
                        reponsitory.Update<Layers.Data.Sqls.PvdCrm.AgentQuotes>(new
                        {
                            SpnID = this.SpnID,
                            QuoteType = (int)this.QuoteType,
                            ClientID = this.ClientID,
                            MinQuantity = this.MinQuantity,
                            MaxQuantity = this.MaxQuantity,
                            Currency = (int)this.Currency,
                            UnitCostPrice = this.UnitCostPrice,
                            ResalePrice = this.ResalePrice,
                            ApprovedPrice = this.ApprovedPrice,
                            ProfitRate = this.ProfitRate,
                            StartDate = this.StartDate,
                            EndDate = this.EndDate,
                            Status = (int)this.Status,
                            CreatorID = this.CreatorID
                        }, item => item.SpnID == this.SpnID && item.ClientID == this.ClientID);
                    }

                    #endregion


                    //reponsitory.Delete<Layers.Data.Sqls.PvdCrm.AgentQuotes>(item => item.SpnID == this.SpnID && item.ClientID == this.ClientID);
                    //reponsitory.Insert(this.LstProjectProduct.Select(item => new Layers.Data.Sqls.PvdCrm.ProjectProducts
                    //{
                    //    ID = PKeySigner.Pick(Yahv.CrmPlus.Service.PKeyType.ProjectProduct),
                    //    ProjectID = this.ID,
                    //    AssignClientID = item.AssignClientID,
                    //    SpnID = item.SpnID,
                    //    UnitProduceQuantity = item.UnitProduceQuantity,
                    //    ProduceQuantity = item.ProduceQuantity,
                    //    ProjectStatus = (int)item.ProjectStatus,
                    //    Currency = (int)item.Currency,
                    //    ExpectUnitPrice = item.ExpectUnitPrice,
                    //    CreateDate = this.CreateDate,
                    //    ModifyDate = this.ModifyDate,
                    //    Status = (int)Yahv.Underly.AuditStatus.Normal
                    //}).ToArray());

                }

                tran.Commit();
            }
            this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this));
        }


        public void Abandon()
        {
            using (var repository = LinqFactory<PvdCrmReponsitory>.Create())
            {
                repository.Update<Layers.Data.Sqls.PvdCrm.AgentQuotes>(new
                {
                    Status = (int)DataStatus.Closed,
                }, item => item.ID == this.ID);
                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this));
                }
            }
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
