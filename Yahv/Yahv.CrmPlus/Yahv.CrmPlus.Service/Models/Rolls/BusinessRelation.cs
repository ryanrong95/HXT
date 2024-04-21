using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Extends;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Usually;

namespace Yahv.CrmPlus.Service.Models.Rolls
{
    /// <summary>
    /// 关联关系
    /// </summary>
    public class BusinessRelation : Yahv.Linq.IUnique
    {
        public BusinessRelation()
        {
            this.CreateDate = DateTime.Now;
            this.Status = AuditStatus.Waiting;
        }
        #region 属性
        public string ID { get; set; }
        /// <summary>
        /// 商务关系
        /// </summary>

        public Yahv.Underly.BusinessRelationType BusinessRelationType { get; set; }

        /// <summary>
        ///主体企业ID
        /// </summary>
        public string MainID { get; set; }
        public string MainName { internal set; get; }

        /// <summary>
        /// 从属企业ID
        /// </summary>
        public string SubID { get; set; }
        public string SubName { internal set; get; }


        public Yahv.Underly.AuditStatus Status { get; set; }
        public DateTime CreateDate { set; get; }
        public string CreatorID { set; get; }
        public string CreatorName { internal set; get; }
        public List<CallFile> RelationFiles { set; get; }
        #endregion

        #region 审批任务
        public MainType MainType { set; get; }
        public ApplyTaskType TaskType { set; get; }
        #endregion
        #region 事件

        /// <summary>
        /// EnterSuccess
        /// </summary>
        public event SuccessHanlder EnterSuccess;

        /// <summary>
        /// EnterError
        /// </summary>
        public event ErrorHanlder EnterError;
        /// <summary>
        /// Repeat
        /// </summary>
        public event ErrorHanlder Repeat;
        /// <summary>
        /// AbandonSuccess
        /// </summary>
        //public event SuccessHanlder AbandonSuccess;
        #endregion

        #region  持久化
        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvdCrmReponsitory>.Create())
            using (var tran = reponsitory.OpenTransaction())
            {
                {
                    var exist = reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.MapsEnterprise>().Where(x => x.MainID == this.MainID && x.SubID == this.SubID && x.Type == (int)this.BusinessRelationType);
                    if (string.IsNullOrWhiteSpace(this.ID))
                    {
                        if (exist.Any())
                        {
                            this.Repeat(this, new ErrorEventArgs());
                            return;
                        }
                        this.ID = Guid.NewGuid().ToString("N");
                        reponsitory.Insert(new Layers.Data.Sqls.PvdCrm.MapsEnterprise()
                        {
                            ID = this.ID,
                            Type = (int)this.BusinessRelationType,
                            MainID = this.MainID,
                            SubID = this.SubID,
                            Status = (int)this.Status,
                            CreateDate = this.CreateDate,
                            CreatorID = this.CreatorID
                        });
                        (new ApplyTask
                        {
                            MainID = this.ID,
                            MainType = this.MainType,
                            ApplierID = CreatorID = this.CreatorID,
                            ApplyTaskType = this.TaskType,
                            Status = ApplyStatus.Waiting,
                            CreateDate = this.CreateDate
                        }).Enter();
                    }
                    else
                    {
                        reponsitory.Update<Layers.Data.Sqls.PvdCrm.MapsEnterprise>(new
                        {
                            Status = (int)this.Status
                        }, item => item.ID == this.ID);
                    }
                    if (this.RelationFiles?.Count() > 0)
                    {
                        //var licenselist = new Views.Rolls.FilesDescriptionRoll()[this.MainID, this.ID, CrmFileType.EnterpriseRelation].ToArray();
                        //licenselist.Abandon();//废弃
                        List<FilesDescription> listFile = new List<FilesDescription>();
                        foreach (var item in this.RelationFiles)
                        {
                            var file = new FilesDescription
                            {
                                EnterpriseID = this.MainID,
                                SubID = this.ID,
                                CustomName = item.FileName,
                                Url = item.CallUrl,
                                Type = CrmFileType.EnterpriseRelation,
                                CreatorID = this.CreatorID
                            };
                            listFile.Add(file);
                        }
                        listFile.Enter();
                    }
                }
                tran.Commit();
            }
            this.EnterError?.Invoke(this, new ErrorEventArgs());
            this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this));
        }

        public void Approve(AuditStatus status, string approveid)
        {
            using (var reponsitory = LinqFactory<PvdCrmReponsitory>.Create())
            using (var tran = reponsitory.OpenTransaction())
            {
                {
                    if (this.Status == AuditStatus.Waiting)
                    {
                        reponsitory.Update<Layers.Data.Sqls.PvdCrm.MapsEnterprise>(new
                        {
                            Status = (int)status
                        }, item => item.ID == this.ID);
                        reponsitory.Update<Layers.Data.Sqls.PvdCrm.ApplyTasks>(new
                        {
                            ApproverID = approveid,
                            ApproveDate = DateTime.Now,
                            Status = (int)status
                        }, item => item.MainType == (int)this.MainType
                        && item.MainID == ID
                        && item.ApplyTaskType == (int)this.TaskType);
                    }
                }
                tran.Commit();
            }
            this.EnterError?.Invoke(this, new ErrorEventArgs());
            this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this));
        }

        //public void Closed()
        //{
        //    using (var repository = LinqFactory<PvdCrmReponsitory>.Create())
        //    {
        //        repository.Update<Layers.Data.Sqls.PvdCrm.MapsEnterprise>(new
        //        {
        //            Status = DataStatus.Closed
        //        }, item => item.ID == this.ID);
        //        if (this != null && this.AbandonSuccess != null)
        //        {
        //            this.AbandonSuccess(this, new SuccessEventArgs(this));
        //        }
        //    }
        //}

        //public void Enable()
        //{
        //    using (var repository = LinqFactory<PvdCrmReponsitory>.Create())
        //    {
        //        repository.Update<Layers.Data.Sqls.PvdCrm.MapsEnterprise>(new
        //        {
        //            Status = DataStatus.Normal
        //        }, item => item.ID == this.ID);
        //        if (this != null && this.AbandonSuccess != null)
        //        {
        //            this.EnterSuccess(this, new SuccessEventArgs(this));
        //        }
        //    }
        //}
        #endregion



    }

}
