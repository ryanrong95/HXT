using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Extends;
using Yahv.CrmPlus.Service.Views.Rolls.TraceRecords;
using Yahv.Underly;
using Yahv.Usually;
using YaHv.CrmPlus.Services.Models.Origins;

namespace Yahv.CrmPlus.Service.Models.Origins
{
    public class TraceRecord : Yahv.Linq.IUnique
    {
        #region  属性
        public string ID { get; set; }
        /// <summary>
        /// 客户ID
        /// </summary>
        public string ClientID { get; set; }

        public Enterprise Enterprise { get; set; }
        /// <summary>
        /// 销售机会ID
        /// </summary>
        public string ProjectID { get; set; }
        /// <summary>
        /// 跟进方式
        /// </summary>
        public FollowWay FollowWay { get; set; }
        /// <summary>
        /// 跟进日期
        /// </summary>

        public DateTime TraceDate { get; set; }
        /// <summary>
        /// 下次跟进日期
        /// </summary>
        public DateTime? NextDate { get; set; }
        /// <summary>
        /// 原厂陪同人
        /// </summary>
        public string SupplierStaffs { get; set; }

        // public  Admin SupplierStaff { get; set; }
        /// <summary>
        /// 公司陪同人
        /// </summary>
        public string CompanyStaffs { get; set; }
        /// <summary>
        /// 跟进内容
        /// </summary>
        public string Context { get; set; }
        /// <summary>
        /// 进一步计划
        /// </summary>
        public string NextPlan { get; set; }
        /// <summary>
        /// 联系人信息
        /// </summary>
        public string ClientContactID { get; set; }
        /// <summary>
        /// 所属人
        /// </summary>
        public string OwnerID { get; set; }

        /// <summary>
        /// 跟进人
        /// </summary>
        public Admin Owner { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 更新日期
        /// </summary>
        public DateTime ModifyDate { get; set; }

        public List<CallFile> Files { get; set; }

        public string ReadIDs { get; set; }

        public TraceRecord()
        {
            this.CreateDate = this.ModifyDate = DateTime.Now;
        }


        #endregion

        #region 持久化
        public void Enter()
        {
            using (var reponsitory = new PvdCrmReponsitory())
            using (var tran = reponsitory.OpenTransaction())
            {
                {
                    //添加
                    if (!reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.TraceRecords>().Any(item => item.ID == this.ID))
                    {
                        this.ID = PKeySigner.Pick(Yahv.CrmPlus.Service.PKeyType.Trace);
                        reponsitory.Insert(new Layers.Data.Sqls.PvdCrm.TraceRecords()
                        {
                            ID = this.ID,
                            ClientID = this.ClientID,
                            ProjectID = this.ProjectID,
                            Type = (int)this.FollowWay,
                            TraceDate = this.TraceDate,
                            SupplierStaffs = this.SupplierStaffs,
                            CompanyStaffs = this.CompanyStaffs,
                            Context = this.Context,
                            NextDate = this.NextDate,
                            NextPlan = this.NextPlan,
                            ClientContactID = this.ClientContactID,
                            OwnerID = this.OwnerID,
                            CreateDate = this.CreateDate,
                            ModifyDate = this.ModifyDate

                        });
                    }
                    //修改
                    else
                    {
                        reponsitory.Update<Layers.Data.Sqls.PvdCrm.TraceRecords>(new
                        {
                            ID = this.ID,
                            ClientID = this.ClientID,
                            ProjectID = this.ProjectID,
                            Type = (int)this.FollowWay,
                            TraceDate = this.TraceDate,
                            SupplierStaffs = this.SupplierStaffs,
                            CompanyStaffs = this.CompanyStaffs,
                            Context = this.Context,
                            NextDate = this.NextDate,
                            NextPlan = this.NextPlan,
                            ClientContactID = this.ClientContactID,
                            OwnerID = this.OwnerID,
                            ModifyDate = this.ModifyDate

                        }, item => item.ID == this.ID);
                    }
                    if (this.Files?.Count() > 0)
                    {
                        var cardlist = new Views.Rolls.FilesDescriptionRoll()[this.ClientID, this.ID, CrmFileType.TraceRecords].ToArray();
                        cardlist.Abandon();
                        List<FilesDescription> listFile = new List<FilesDescription>();
                        foreach (var item in this.Files)
                        {
                            var file = new FilesDescription
                            {
                                EnterpriseID = this.ClientID,
                                SubID = this.ID,
                                CustomName = item.FileName,
                                Url = item.CallUrl,
                                Type = CrmFileType.TraceRecords,
                                CreatorID = this.OwnerID
                            };
                            listFile.Add(file);
                        }
                        listFile.Enter();
                    }
                    if (!string.IsNullOrEmpty(this.ReadIDs))
                    {

                        var ids = this.ReadIDs.Split(',');
                        var Tracelist = reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.TraceComments>().Where(x => x.TraceRecordID == this.ID).Select(x => x.AdminID).ToArray();
                        var newIds = ids.Except(Tracelist);
                        if (newIds.Count()>0)
                        {
                            reponsitory.Insert(newIds.Select(item => new Layers.Data.Sqls.PvdCrm.TraceComments
                            {
                                ID = PKeySigner.Pick(Yahv.CrmPlus.Service.PKeyType.Comment),
                                AdminID = item,
                                TraceRecordID = this.ID,
                                IsPointed = true,
                                CreateDate = this.CreateDate,
                                ModifyDate = this.ModifyDate
                            }).ToArray());
                        }
                    }
                }
                tran.Commit();
            }
            this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this));
        }


        public void Closed()
        {
            using (var repository = LinqFactory<PvdCrmReponsitory>.Create())
            {
                repository.Update<Layers.Data.Sqls.PvdCrm.TraceRecords>(new
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
                repository.Update<Layers.Data.Sqls.PvdCrm.TraceRecords>(new
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
