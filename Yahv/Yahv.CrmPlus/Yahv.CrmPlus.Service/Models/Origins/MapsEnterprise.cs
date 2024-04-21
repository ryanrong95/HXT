using Layers.Data;
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
    /// <summary>
    /// 客户的关联关系
    /// </summary>
    public class MapsEnterprise : Yahv.Linq.IUnique
    {

        #region 属性
        public string ID { get; set; }
        /// <summary>
        /// 商务关系
        /// </summary>

        public BusinessRelationType BusinessRelationType { get; set; }

        /// <summary>
        ///主要ID
        /// </summary>
        public string MainID { get; set; }

        public string MainName { internal set; get; }

        /// <summary>
        /// 从属ID
        /// </summary>
        public string SubID { get; set; }

        public string SubName { internal set; get; }

        public string CreatorID { get; set; }

        public string Creator { get; set; }
        public DateTime CreateDate { get; set; }


        public AuditStatus AuditStatus { get; set; }


        public List<CallFile> RelationFiles { get; set; }

        #endregion


        public MapsEnterprise()
        {
            this.CreateDate = DateTime.Now;
            this.AuditStatus = AuditStatus.Waiting;
        }

        #region  持久化
        public void Enter(string creatorid = null)
        {
            using (var reponsitory = LinqFactory<PvdCrmReponsitory>.Create())
            using (var tran = reponsitory.OpenTransaction())
            {
                {
                    var exist = reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.MapsEnterprise>().Where(x => x.MainID == this.MainID && x.SubID == this.SubID && x.Type == (int)this.BusinessRelationType);
                    if (!exist.Any())
                    {
                        this.ID = PKeySigner.Pick(Yahv.CrmPlus.Service.PKeyType.MapsEnterprise);
                        reponsitory.Insert(new Layers.Data.Sqls.PvdCrm.MapsEnterprise()
                        {
                            ID = this.ID,
                            Type = (int)this.BusinessRelationType,
                            MainID = this.MainID,
                            SubID = this.SubID,
                            Status = (int)this.AuditStatus,
                            CreateDate = this.CreateDate
                        });
                    }
                    else
                    {
                        reponsitory.Update<Layers.Data.Sqls.PvdCrm.MapsEnterprise>(new
                        {
                            Type = (int)this.BusinessRelationType,
                            MainID = this.MainID,
                            SubID = this.SubID,
                            Status = (int)this.AuditStatus,
                        }, x => x.MainID == this.MainID && x.SubID == this.SubID && x.Type == (int)this.BusinessRelationType);

                    }

                    if (this.RelationFiles?.Count() > 0)
                    {
                        var licenselist = new Views.Rolls.FilesDescriptionRoll()[this.MainID, this.ID, CrmFileType.EnterpriseRelation].ToArray();
                        licenselist.Abandon();
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
            {
                using (var tran = reponsitory.OpenTransaction())
                {
                    if (this.AuditStatus == AuditStatus.Waiting)
                    {
                        reponsitory.Update<Layers.Data.Sqls.PvdCrm.MapsEnterprise>(new
                        {
                            Status = (int)status
                        }, item => item.ID == this.ID);

                    }
                    tran.Commit();
                }
                // this.EnterError?.Invoke(this, new ErrorEventArgs());
                this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this));
            }
        }
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
        public event ErrorHanlder Repeat;


        //public event SuccessHanlder AbandonSuccess;

        #endregion
    }
}
