using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Views.Rolls.SalesChances;
using Yahv.CrmPlus.Service.Views.Rolls.TraceRecords;
using Yahv.Underly;
using Yahv.Underly.CrmPlus;
using Yahv.Usually;
using YaHv.CrmPlus.Services.Models.Origins;

namespace Yahv.CrmPlus.Service.Models.Origins
{
    /// <summary>
    ///项目
    /// </summary>

    public class Project : Yahv.Linq.IUnique
    {
        #region  属性
        public string ID { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>

        public string Name { get; set; }
        /// <summary>
        /// （实际终端）客户
        /// </summary>

        public string EndClientID { get; set; }

        public Client Client { get; set; }
        /// <summary>
        /// （实际下单）客户
        /// </summary>

        public string AssignClientID { get; set; }

        /// <summary>
        /// （实际下单）客户
        /// </summary>
        public Client OrderClient { get; set; }
        /// <summary>
        /// 我方公司
        /// </summary>
        public string CompanyID { get; set; }
        /// <summary>
        /// 立项日期
        /// </summary>
        public DateTime EstablishDate { get; set; }
        /// <summary>
        /// 量产日期
        /// </summary>
        public DateTime? ProductDate { get; set; }
        /// <summary>
        /// 预计研发日期
        /// </summary>
        public DateTime? RDDate { get; set; }
        /// <summary>
        /// (客户)联系人ID
        /// </summary>
        public string ClientContactID { get; set; }
        public Contact Contact { get; set; }


        public string Summary { set; get; }
        /// <summary>
        /// 所属人
        /// </summary>
        public string OwnerID { get; set; }
        /// <summary>
        /// 当前登录人
        /// </summary>
        public Admin Admin { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }

        //public string EndClientID { get; set; }



        public DataStatus Status { get; set; }



        public List<ProjectProduct> LstProjectProduct { get; set; }



        public MapsEnterprise MapsEnterprise { get; set; }

        public string TraceId { get; set; }
        #endregion

        public Project()
        {
            this.CreateDate = this.ModifyDate = DateTime.Now;
            this.Status = DataStatus.Normal;
        }

        #region 持久化
        public void Enter()
        {
            using (var reponsitory = new PvdCrmReponsitory())
            using (var tran = reponsitory.OpenTransaction())
            {
                #region  项目
                //添加
                if (!reponsitory.GetTable<Layers.Data.Sqls.PvdCrm.Projects>().Any(item => item.ID == this.ID))
                {
                    this.ID = PKeySigner.Pick(Yahv.CrmPlus.Service.PKeyType.Project);
                    //var orderclient = this.AssignClientID;
                    reponsitory.Insert(new Layers.Data.Sqls.PvdCrm.Projects()
                    {
                        ID = this.ID,
                        Name = this.Name,
                        EndClientID = this.EndClientID,
                        AssignClientID = this.AssignClientID,
                        CompanyID = this.CompanyID,
                        EstablishDate = this.EstablishDate,
                        ProductDate = this.ProductDate,
                        RDDate = this.RDDate,
                        ClientContactID = this.ClientContactID,
                        Summary = this.Summary,
                        CreateDate = this.CreateDate,
                        ModifyDate = this.ModifyDate,
                        OwnerID = this.OwnerID,
                        Status = (int)this.Status
                    });
                }
                //修改
                else
                {
                    reponsitory.Update<Layers.Data.Sqls.PvdCrm.Projects>(new
                    {
                        Name = this.Name,
                        EndClientID = this.EndClientID,
                        AssignClientID = this.AssignClientID,
                        CompanyID = this.CompanyID,
                        EstablishDate = this.EstablishDate,
                        ProductDate = this.ProductDate,
                        RDDate = this.RDDate,
                        ClientContactID = this.ClientContactID,
                        Summary = this.Summary,
                        ModifyDate = this.ModifyDate,
                        OwnerID = this.OwnerID,
                        Status = (int)this.Status
                    }, item => item.ID == this.ID);
                }

                ////if (this.LstProjectProduct.Count > 0)
                ////{
                ////    var projectproduct = reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.ProjectProducts>().Where(x => x.ProjectID == this.ID).ToArray();
                ////    foreach (var product in this.LstProjectProduct)
                ////    {
                ////        if (!projectproduct.Any(item => item.SpnID == product.SpnID))
                ////        {
                ////            reponsitory.Insert(new Layers.Data.Sqls.PvdCrm.ProjectProducts()
                ////            {
                ////                ID = PKeySigner.Pick(Yahv.CrmPlus.Service.PKeyType.ProjectProduct),
                ////                ProjectID = this.ID,
                ////                AssignClientID = product.AssignClientID,
                ////                SpnID = product.SpnID,
                ////                UnitProduceQuantity = product.UnitProduceQuantity,
                ////                ProduceQuantity = product.ProduceQuantity,
                ////                ProjectStatus = (int)product.ProjectStatus,
                ////                Currency = (int)product.Currency,
                ////                ExpectUnitPrice = product.ExpectUnitPrice,
                ////                CreateDate = this.CreateDate,
                ////                ModifyDate = this.ModifyDate,
                ////                Status = (int)Yahv.Underly.AuditStatus.Normal
                ////            });

                ////        }
                ////        else
                ////        {
                ////            var TempProduct = projectproduct.FirstOrDefault(item => item.SpnID == product.SpnID);
                ////            //如果有状态变更 ，需要生成审批
                ////            if (product.ProjectStatus > ProductStatus.DO && product.ProjectStatus != (ProductStatus)TempProduct.ProjectStatus)
                ////            {
                ////                var exist = reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.ApplyTasks>().FirstOrDefault(item => item.MainID == TempProduct.ID
                ////                                           && item.ApplyTaskType == (int)ApplyTaskType.ClientProjectStatus
                ////                                            && item.Status == (int)ApplyStatus.Waiting);
                ////                if (exist==null)
                ////                {
                ////                    reponsitory.Insert(new Layers.Data.Sqls.PvdCrm.ApplyTasks
                ////                    {
                ////                        ID = Guid.NewGuid().ToString("N"),
                ////                        MainID = TempProduct.ID,
                ////                        MainType = (int)MainType.Clients,
                ////                        CreateDate = DateTime.Now,
                ////                        ApplierID = this.OwnerID,
                ////                        ApplyTaskType = (int)ApplyTaskType.ClientProjectStatus,
                ////                        Status = (int)(int)ApplyStatus.Waiting
                ////                    });

                ////                }



                ////            }

                ////            reponsitory.Update<Layers.Data.Sqls.PvdCrm.ProjectProducts>(new
                ////            {
                ////                ProjectID = this.ID,
                ////                AssignClientID = product.AssignClientID,
                ////                SpnID = product.SpnID,
                ////                UnitProduceQuantity = product.UnitProduceQuantity,
                ////                ProduceQuantity = product.ProduceQuantity,
                ////                ProjectStatus = (int)product.ProjectStatus,
                ////                Currency = (int)product.Currency,
                ////                ExpectUnitPrice = product.ExpectUnitPrice,
                ////                CreateDate = this.CreateDate,
                ////                ModifyDate = this.ModifyDate,
                ////                Status = (int)Yahv.Underly.AuditStatus.Normal
                ////            }, item => item.ProjectID == this.ID && item.SpnID == product.SpnID);


                ////        }
                ////    }

                ////}
                #endregion

                #region 产生关联关系和待审批

                //if (this.MapsEnterprise != null)
                //{
                //    var exist = reponsitory.GetTable<Layers.Data.Sqls.PvdCrm.MapsEnterprise>().Where(x => x.MainID == this.MapsEnterprise.MainID && x.Type == (int)this.MapsEnterprise.BusinessRelationType);

                //    if (!exist.Any())
                //    {
                //        this.MapsEnterprise.ID = Guid.NewGuid().ToString("N");
                //        reponsitory.Insert(new Layers.Data.Sqls.PvdCrm.MapsEnterprise()
                //        {
                //            ID = this.MapsEnterprise.ID,
                //            Type = (int)this.MapsEnterprise.BusinessRelationType,
                //            MainID = this.MapsEnterprise.MainID,
                //            SubID = this.MapsEnterprise.SubID,
                //            Status = (int)this.MapsEnterprise.AuditStatus,
                //            CreateDate = this.CreateDate
                //        });
                //    }
                //    else
                //    {
                //        //TODO ,已审批的需要确认是否可以被更新
                //        if (exist.FirstOrDefault().Status == (int)Yahv.Underly.AuditStatus.Waiting)
                //        {
                //            reponsitory.Update<Layers.Data.Sqls.PvdCrm.MapsEnterprise>(new
                //            {
                //                Type = (int)this.MapsEnterprise.BusinessRelationType,
                //                MainID = this.MapsEnterprise.MainID,
                //                SubID = this.MapsEnterprise.SubID,
                //                Status = (int)this.MapsEnterprise.AuditStatus,
                //            }, x => x.MainID == this.MapsEnterprise.MainID && x.Type == (int)this.MapsEnterprise.BusinessRelationType);
                //        }

                //    }

                //    //var exist1 = reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.ApplyTasks>().FirstOrDefault(item => item.MainID == this.MapsEnterprise.MainID
                //    //                                    && item.ApplyTaskType == (int)Underly.CrmPlus.ApplyTaskType.ClientBusinessRelation
                //    //                                     && item.Status == (int)Underly.CrmPlus.ApplyStatus.Waiting);
                //    //if (exist1 == null)
                //    //{
                //    //    reponsitory.Insert(new Layers.Data.Sqls.PvdCrm.ApplyTasks
                //    //    {
                //    //        ID = Guid.NewGuid().ToString("N"),
                //    //        MainID = this.MapsEnterprise.MainID,
                //    //        MainType = (int)Underly.CrmPlus.MainType.Clients,
                //    //        CreateDate = DateTime.Now,
                //    //        ApplierID = this.OwnerID,
                //    //        ApplyTaskType = (int)Underly.CrmPlus.ApplyTaskType.ClientBusinessRelation,
                //    //        Status = (int)(int)Underly.CrmPlus.ApplyStatus.Waiting
                //    //    });

                //    //}

                //}
                #endregion

                #region  生成待报备数据
                //if (this.LstProjectProduct.Count > 0) {
                //    var ids = this.LstProjectProduct.Select(x => x.SpnID).Distinct().ToArray();
                //    var reports = reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.ProjectReports>().ToArray();
                //    var projectSpnid = reports.Where(x => x.ProjectID == this.ID).Select(x => x.SpnID).ToArray();
                //    var newIDs = ids.Except(projectSpnid);
                //    foreach (var item in newIDs)
                //    {
                //        //获取代理品牌的PM
                //        var admin = Yahv.CrmPlus.Service.GetPmBySpnID.GetPm(item);
                //        if (admin.ID != null)
                //        {
                //            if (!reports.Any(x => x.ProjectID == this.ID && x.SpnID == item))
                //            {
                //                reponsitory.Insert(new Layers.Data.Sqls.PvdCrm.ProjectReports()
                //                {
                //                    ID = PKeySigner.Pick(Yahv.CrmPlus.Service.PKeyType.ProjectReport),
                //                    ClientID = this.EndClientID,
                //                    ProjectID = this.ID,
                //                    SpnID = item,
                //                    ReporterID = admin.ID,
                //                    Status = (int)ReportStatus.Waiting,
                //                    CreateDate = DateTime.Now,
                //                    ModifyDate = DateTime.Now,
                //                });
                //            }
                //        }
                //    }

                //}

                #endregion

                #region 填充跟踪记录的项目ID

                if (!string.IsNullOrEmpty(this.TraceId))
                {
                    var traceRecord = new TraceRecordsRoll()[this.TraceId];
                    traceRecord.ProjectID = this.ID;
                    reponsitory.Update<Layers.Data.Sqls.PvdCrm.TraceRecords>(new
                    {
                        ProjectID = traceRecord.ProjectID,
                        ModifyDate = DateTime.Now
                    }, item => item.ID == traceRecord.ID);
                }
                #endregion



                tran.Commit();
            }
            this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this));
        }


        public void Closed()
        {
            using (var repository = LinqFactory<PvdCrmReponsitory>.Create())
            {
                repository.Update<Layers.Data.Sqls.PvdCrm.Projects>(new
                {
                    Status = DataStatus.Closed
                }, item => item.ID == this.ID);
                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this));
                }
            }
        }

        public void Enable()
        {
            using (var repository = LinqFactory<PvdCrmReponsitory>.Create())
            {
                repository.Update<Layers.Data.Sqls.PvdCrm.Projects>(new
                {
                    Status = DataStatus.Normal
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
