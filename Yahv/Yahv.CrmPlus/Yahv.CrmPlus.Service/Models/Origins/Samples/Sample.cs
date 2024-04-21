using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using Yahv.Underly.CrmPlus;
using Yahv.Usually;
using YaHv.CrmPlus.Services.Models.Origins;

namespace Yahv.CrmPlus.Service.Models.Origins
{
    public class Sample : Yahv.Linq.IUnique
    {
        #region 属性
        /// <summary>
        /// 
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 
        /// </summary>

        public string ProjectID { get; set; }

        public Project Project { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string AddressID { get; set; }

        public Address Address { get; set; }
        /// <summary>
        /// 
        /// </summary>

        public string ContactID { get; set; }

        public Contact Contact { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string WaybillCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? Freight { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? DeliveryDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ApplierID { get; set; }

        public Admin Applier { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ApproverID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 
        /// </summary>

        public DateTime ModifyDate { get; set; }

        public string Summary { get; set; }

        public AuditStatus AuditStatus { get; set; }

        public List<SampleItem> lstSampleItem { get; set; }
        public SampleItem SampleItem { get; set; }
        #endregion

        public Sample()
        {
            this.CreateDate = this.ModifyDate = DateTime.Now;
            this.AuditStatus = AuditStatus.Waiting;

        }

        #region 持久化
        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvdCrmReponsitory>.Create())
          
                {

                    #region  sample
                    //添加
                    if (!reponsitory.GetTable<Layers.Data.Sqls.PvdCrm.Samples>().Any(item => item.ID == this.ID))
                    {
                        this.ID = PKeySigner.Pick(Yahv.CrmPlus.Service.PKeyType.Sample);
                        reponsitory.Insert(new Layers.Data.Sqls.PvdCrm.Samples()
                        {
                            ID = this.ID,
                            ProjectID = this.ProjectID,
                            AddressID = this.AddressID,
                            ContactID = this.ContactID,
                            WaybillCode = this.WaybillCode,
                            Freight = this.Freight,
                            DeliveryDate = this.DeliveryDate,
                            ApplierID = this.ApplierID,
                            ApproverID = this.ApproverID,
                            CreateDate = this.CreateDate,
                            ModifyDate = this.ModifyDate,
                            Status = (int)this.AuditStatus
                        });
                    }
                    //修改
                    else
                    {
                        reponsitory.Update<Layers.Data.Sqls.PvdCrm.Samples>(new
                        {
                            ProjectID = this.ProjectID,
                            AddressID = this.AddressID,
                            ContactID = this.ContactID,
                            WaybillCode = this.WaybillCode,
                            Freight = this.Freight,
                            DeliveryDate = this.DeliveryDate,
                            ApplierID = this.ApplierID,
                            ApproverID = this.ApproverID,
                            CreateDate = this.CreateDate,
                            ModifyDate = this.ModifyDate,
                            Status = (int)this.AuditStatus
                        }, item => item.ID == this.ID);
                    }

                #endregion

                #region  Item
                if (this.SampleItem != null)
                {
                    this.SampleItem.SampleID = this.ID;
                    this.SampleItem.Enter();

                }

                    //if (this.lstSampleItem.Count > 0)
                    //{
                    //    reponsitory.Delete<Layers.Data.Sqls.PvdCrm.SampleItems>(item => item.SampleID == this.ID);
                    //    reponsitory.Insert(this.lstSampleItem.Select(item => new Layers.Data.Sqls.PvdCrm.SampleItems
                    //    {
                    //        ID = PKeySigner.Pick(Yahv.CrmPlus.Service.PKeyType.SampleIt),
                    //        SampleID = this.ID,
                    //        SpnID = item.SpnID,
                    //        Type = (int)item.SampleType,
                    //        Quantity = item.Quantity,
                    //        Price = item.Price,
                    //        CreateDate = DateTime.Now,
                    //        ModifyDate = DateTime.Now,
                    //        Status = (int)this.AuditStatus
                    //    }).ToArray());

                    //}
                    #endregion
            }
            this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this));
        }

        public void Closed()
        {
            using (var repository = LinqFactory<PvdCrmReponsitory>.Create())
            {
                repository.Update<Layers.Data.Sqls.PvdCrm.SampleItems>(new
                {
                    Status = AuditStatus.Closed
                }, item => item.SampleID == this.ID);
                repository.Update<Layers.Data.Sqls.PvdCrm.Samples>(new
                {
                    Status = AuditStatus.Closed
                }, item => item.ID == this.ID);
                repository.Update<Layers.Data.Sqls.PvdCrm.ApplyTasks>(new
                {
                    Status = ApplyStatus.Cancel
                }, item => item.MainID == this.ID && item.ApplyTaskType == (int)ApplyTaskType.ClientSample);

                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this));
                }
            }
        }

        public void Approve()
        {
            using (var reponsitory = LinqFactory<PvdCrmReponsitory>.Create())
            using (var tran = reponsitory.OpenTransaction())
            {
                {
                    reponsitory.Update<Layers.Data.Sqls.PvdCrm.Samples>(new
                    {
                        Status = this.AuditStatus,
                        this.ModifyDate,
                        this.ApproverID
                    }, item => item.ID == this.ID);
                    if (this.lstSampleItem.Count > 0)
                    {
                        reponsitory.Delete<Layers.Data.Sqls.PvdCrm.SampleItems>(item => item.SampleID == this.ID);
                        reponsitory.Insert(this.lstSampleItem.Select(item => new Layers.Data.Sqls.PvdCrm.SampleItems
                        {
                            ID = PKeySigner.Pick(Yahv.CrmPlus.Service.PKeyType.SampleIt),
                            SampleID = this.ID,
                            SpnID = item.SpnID,
                            Type = (int)item.SampleType,
                            Quantity = item.Quantity,
                            Price = item.Price,
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            Status = (int)this.AuditStatus
                        }).ToArray());

                    }
                }
                tran.Commit();

                if (this != null && this.EnterSuccess != null)
                {
                    this.EnterSuccess(this, new SuccessEventArgs(this));
                }
            }

        }


        public void UpdateWaybillNo()
        {

            using (var reponsitory = LinqFactory<PvdCrmReponsitory>.Create())
            using (var tran = reponsitory.OpenTransaction())
            {
                {
                    reponsitory.Update<Layers.Data.Sqls.PvdCrm.Samples>(new
                    {
                        this.WaybillCode,
                        this.Summary,
                        this.DeliveryDate,
                        this.ModifyDate,
                    }, item => item.ID == this.ID);
                   
                }
                tran.Commit();

                if (this != null && this.EnterSuccess != null)
                {
                    this.EnterSuccess(this, new SuccessEventArgs(this));
                }
            }


        }
        #endregion

        #region 事件
        public event SuccessHanlder EnterSuccess;



        public event SuccessHanlder AbandonSuccess;


        public event ErrorHanlder EnterError;
        #endregion


    }
}
