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

namespace Yahv.CrmPlus.Service.Models.Origins
{
    public class ApplyTask : IUnique
    {
        public ApplyTask()
        {
            this.CreateDate = DateTime.Now;
            this.Status =ApplyStatus.Waiting;
        }
        #region 属性
        public string ID { set; get; }
        /// <summary>
        /// 主体ID
        /// </summary>
        public string MainID { set; get; }
        /// <summary>
        /// 主体类型
        /// </summary>
        public Underly.MainType MainType { set; get; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { set; get; }
        /// <summary>
        /// 审批时间
        /// </summary>
        public DateTime? ApproveDate { set; get; }
        /// <summary>
        /// 审批人
        /// </summary>
        public string ApproverID { set; get; }
        /// <summary>
        /// 申请人
        /// </summary>
        public string ApplierID { set; get; }
        /// <summary>
        /// 审批类型
        /// </summary>
        public Underly.ApplyTaskType ApplyTaskType { set; get; }
        /// <summary>
        /// 审批状态：待审批，
        /// </summary>
        public Underly.ApplyStatus Status { set; get; }
        #endregion

        #region 事件
        public virtual event SuccessHanlder EnterSuccess;
        #endregion

        #region 持久化
        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvdCrmReponsitory>.Create())
            {
                var exist = reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.ApplyTasks>().FirstOrDefault(item => item.MainID == this.MainID
                && item.MainType == (int)this.MainType
                && item.ApplyTaskType == (int)this.ApplyTaskType
                && item.Status == (int)ApplyStatus.Waiting);
                if (exist == null)
                {
                    this.ID = PKeySigner.Pick(Yahv.CrmPlus.Service.PKeyType.ApplyTasks);
                    reponsitory.Insert(new Layers.Data.Sqls.PvdCrm.ApplyTasks
                    {
                        ID = this.ID, 
                        MainID = this.MainID,
                        MainType = (int)this.MainType,
                        CreateDate = this.CreateDate,
                        ApproveDate = this.ApproveDate,
                        ApproverID = this.ApproverID,
                        ApplierID = this.ApplierID,
                        ApplyTaskType = (int)this.ApplyTaskType,
                        Status = (int)this.Status
                    });
                }
                this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this));
            }
        }

        public void EnterExtend()
        {
            using (var reponsitory = LinqFactory<PvdCrmReponsitory>.Create())
            using (var tran=reponsitory.OpenTransaction())
            {
                {
                    var exist = reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.ApplyTasks>().FirstOrDefault(item => item.MainID == this.MainID
                    && item.ApplyTaskType == (int)this.ApplyTaskType&&item.ApplierID==this.ApplierID);

                    if (exist == null)
                    {
                        this.ID = PKeySigner.Pick(Yahv.CrmPlus.Service.PKeyType.ApplyTasks);
                        reponsitory.Insert(new Layers.Data.Sqls.PvdCrm.ApplyTasks
                        {
                            ID = this.ID,
                            MainID = this.MainID,
                            MainType = (int)this.MainType,
                            CreateDate = this.CreateDate,
                            ApproveDate = this.ApproveDate,
                            ApproverID = this.ApproverID,
                            ApplierID = this.ApplierID,
                            ApplyTaskType = (int)this.ApplyTaskType,
                            Status = (int)this.Status
                        });
                    }
                    else {

                        reponsitory.Update<Layers.Data.Sqls.PvdCrm.ApplyTasks>(new {
                            MainID = this.MainID,
                            MainType = (int)this.MainType,
                            CreateDate = this.CreateDate,
                            ApproveDate = this.ApproveDate,
                            ApproverID = this.ApproverID,
                            ApplierID = this.ApplierID,
                            ApplyTaskType = (int)this.ApplyTaskType,
                            Status = (int)this.Status

                        }, item =>item.ID== exist.ID);


                    }

                }
                tran.Commit();
            }
                this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this));


        }


      /// <summary>
      /// 审批
      /// </summary>
        public void Approve()
        {
            using (var reponsitory = LinqFactory<PvdCrmReponsitory>.Create())
            {
                var exist = reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.ApplyTasks>().FirstOrDefault(item => item.MainID == this.MainID
               && item.MainType == (int)this.MainType
               && item.ApplyTaskType == (int)this.ApplyTaskType
               && item.Status == (int)ApplyStatus.Waiting);
                if (exist != null)
                {
                    reponsitory.Update<Layers.Data.Sqls.PvdCrm.ApplyTasks>(new
                    {
                        ApproverID=this.ApproverID,
                        Status = (int)this.Status,
                        ApproveDate = DateTime.Now
                    }, item => item.ID == exist.ID);
                }
            }
        }
        #endregion
    }
}
