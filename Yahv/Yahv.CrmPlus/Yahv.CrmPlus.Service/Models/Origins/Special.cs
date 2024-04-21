using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Extends;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Usually;

namespace Yahv.CrmPlus.Service.Models.Origins
{
    public class Special : IUnique
    {
        public Special()
        {
            this.CreateDate = DateTime.Now;
            this.Status = Underly.AuditStatus.Waiting;
        }
        #region 属性
        public string ID { set; get; }
        public string EnterpriseID { set; get; }
        /// <summary>
        /// 供应商名称
        /// </summary>
        public string SupplierName { internal set; get; }
        /// <summary>
        /// 类型
        /// </summary>
        public Underly.nBrandType Type { set; get; }
        /// <summary>
        /// 品牌
        /// </summary>
        public string Brand { set; get; }
        /// <summary>
        /// 型号
        /// </summary>
        public string PartNumber { set; get; }
        /// <summary>
        /// 状态
        /// </summary>
        public Underly.AuditStatus Status { set; get; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { internal set; get; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatorID { set; get; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { set; get; }
        public string CreatorName { internal set; get; }
        /// <summary>
        /// 附件
        /// </summary>
        public List<CallFile> SpecialFiles { set; get; }

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
        /// <summary>
        /// Repeat
        /// </summary>
        public event ErrorHanlder Repeat;
        #endregion

        #region 持久化
        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvdCrmReponsitory>.Create())
            using (var tran = reponsitory.OpenTransaction())
            {
                {
                    var exist = reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Specials>().Where(item => item.EnterpriseID == this.EnterpriseID
                    && item.Brand == this.Brand
                    && item.PartNumber == this.PartNumber
                    && item.Status == (int)Underly.AuditStatus.Waiting);
                    if (exist.Any())
                    {
                        this.Repeat(this, new ErrorEventArgs());
                        return;
                    }
                    else
                    {
                        this.ID = Guid.NewGuid().ToString("N");
                        reponsitory.Insert(new Layers.Data.Sqls.PvdCrm.Specials
                        {
                            ID = this.ID,
                            EnterpriseID = this.EnterpriseID,
                            Type = (int)this.Type,
                            Brand = this.Brand,
                            PartNumber = this.PartNumber,
                            Status = (int)this.Status,
                            CreateDate = this.CreateDate,
                            CreatorID = this.CreatorID,
                            Summary = this.Summary
                        });
                        (new ApplyTask
                        {
                            MainID = this.ID,
                            MainType = MainType.Suppliers,
                            ApplierID = this.CreatorID,
                            ApplyTaskType = ApplyTaskType.SupplierSpecials,
                            Status = ApplyStatus.Waiting,
                            CreateDate = this.CreateDate
                        }).Enter();
                    }
                    if (this.SpecialFiles?.Count() > 0)
                    {
                        var specialslist = new Views.Rolls.FilesDescriptionRoll()[this.EnterpriseID, this.ID, CrmFileType.SpecialBrands].ToArray();
                        specialslist.Abandon();//废弃
                        List<FilesDescription> listFile = new List<FilesDescription>();
                        foreach (var item in this.SpecialFiles)
                        {
                            var file = new FilesDescription
                            {
                                EnterpriseID = this.EnterpriseID,
                                SubID = this.ID,
                                CustomName = item.FileName,
                                Url = item.CallUrl,
                                Type = CrmFileType.SpecialBrands,
                                CreatorID = this.CreatorID
                            };
                            listFile.Add(file);
                        }
                        listFile.Enter();
                    }

                }
                tran.Commit();
            }
            this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this));
        }
        public void Abandon()
        {
            using (var reponsitory = LinqFactory<PvdCrmReponsitory>.Create())
            {
                if (this.Status == Underly.AuditStatus.Voted)
                {
                    reponsitory.Update<Layers.Data.Sqls.PvdCrm.Specials>(new
                    {
                        Status = (int)Underly.AuditStatus.Closed
                    }, item => item.ID == this.ID);
                }
                this.AbandonSuccess?.Invoke(this, new SuccessEventArgs(this));

            }
        }
        public void Approve(Underly.AuditStatus status, string approveid)
        {
            using (var reponsitory = LinqFactory<PvdCrmReponsitory>.Create())
            using (var tran = reponsitory.OpenTransaction())
            {
                {
                    if (this.Status == Underly.AuditStatus.Waiting)
                    {
                        reponsitory.Update<Layers.Data.Sqls.PvdCrm.Specials>(new
                        {
                            Status = (int)status
                        }, item => item.ID == this.ID);
                        var applystatus = status == Underly.AuditStatus.Normal ? ApplyStatus.Allowed : ApplyStatus.Voted;
                        reponsitory.Update<Layers.Data.Sqls.PvdCrm.ApplyTasks>(new
                        {
                            ApproverID = approveid,
                            ApproveDate = DateTime.Now,
                            Status = (int)applystatus
                        }, item => item.MainID == this.ID
                        && item.ApplyTaskType == (int)ApplyTaskType.SupplierSpecials
                        && item.Status == (int)ApplyStatus.Waiting);
                    }
                }
                tran.Commit();
            }
            this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this));
        }
        #endregion
    }
}
