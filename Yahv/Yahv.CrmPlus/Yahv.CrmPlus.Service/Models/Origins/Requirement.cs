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
using Yahv.Underly.CrmPlus;
using Yahv.Usually;
using YaHv.CrmPlus.Services.Models.Origins;

namespace Yahv.CrmPlus.Service.Models.Origins
{
    public class Requirement : IUnique
    {

        #region  属性
        public string ID { get; set; }

        public string EnterpriseID { get; set; }

        public SpecialType SpecialType { get; set; }
        /// <summary>
        /// 内容说明
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 启用和禁用
        /// </summary>
        public DataStatus Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatorID { get; set; }
        public Admin Admin { get; set; }

        public string FileName { set; get; }
        public string Url { set; get; }

        public List<CallFile> Files { get; set; }


        public Requirement()
        {
            this.CreateDate = DateTime.Now;
            this.ModifyDate = DateTime.Now;
            this.Status = DataStatus.Normal;
        }

        #endregion


        #region  持久化

        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvdCrmReponsitory>.Create())
            {
                using (var tran=reponsitory.OpenTransaction())
                {
                    if (!reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Requirements>().Any(x => x.ID == this.ID))
                    {
                        this.ID = Guid.NewGuid().ToString();
                        reponsitory.Insert(new Layers.Data.Sqls.PvdCrm.Requirements()
                        {
                            ID = this.ID,
                            SpecialType = (int)this.SpecialType,
                            EnterpriseID = this.EnterpriseID,
                            Status = (int)this.Status,
                            Content = this.Content,
                            CreatorID = this.CreatorID,
                            CreateDate = this.CreateDate,
                            ModifyDate = this.ModifyDate,
                        });
                    }
                    else
                    {
                        reponsitory.Update<Layers.Data.Sqls.PvdCrm.Requirements>(new
                        {
                            ID = this.ID,
                            SpecialType = (int)this.SpecialType,
                            EnterpriseID = this.EnterpriseID,
                            Status = (int)this.Status,
                            Content = this.Content,
                            CreatorID = this.CreatorID,
                            CreateDate = this.CreateDate,
                            ModifyDate = this.ModifyDate,

                        }, item => item.ID == this.ID);

                    }
                    if (this.Files?.Count() > 0)
                    {
                        var cardlist = new Views.Rolls.FilesDescriptionRoll()[this.EnterpriseID, this.ID, CrmFileType.Requirements].ToArray();
                        cardlist.Abandon();//废弃
                        List<FilesDescription> listFile = new List<FilesDescription>();
                        foreach (var item in this.Files)
                        {
                            var file = new FilesDescription
                            {
                                EnterpriseID = this.EnterpriseID,
                                SubID = this.ID,
                                CustomName = item.FileName,
                                Url = item.CallUrl,
                                Type = CrmFileType.Requirements,
                                CreatorID = this.CreatorID
                            };
                            listFile.Add(file);
                        }
                        listFile.Enter();
                    }
                   
                    tran.Commit();
                }

              
            }

            this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this));
        }
        #endregion
        /// <summary>
        /// 禁用
        /// </summary>
        public void Closed()
        {
            using (var repository = LinqFactory<PvdCrmReponsitory>.Create())
            {
                repository.Update<Layers.Data.Sqls.PvdCrm.Requirements>(new
                {
                    Status = DataStatus.Closed
                }, item => item.ID == this.ID);
                if (this != null && this.AbandonSuccess != null)
                {
                    this.EnterSuccess(this, new SuccessEventArgs(this.ID));
                }
            }
        }

        /// <summary>
        /// 启用
        /// </summary>
        public void Enable()
        {
            using (var repository = LinqFactory<PvdCrmReponsitory>.Create())
            {
                repository.Update<Layers.Data.Sqls.PvdCrm.Requirements>(new
                {
                    Status = DataStatus.Normal
                }, item => item.ID == this.ID);
                if (this != null && this.AbandonSuccess != null)
                {
                    this.EnterSuccess(this, new SuccessEventArgs(this.ID));
                }
            }
        }


      


        #region  事件

        /// <summary>
        /// EnterSuccess
        /// </summary>
        public event SuccessHanlder EnterSuccess;

        /// <summary>
        /// EnterError
        /// </summary>
        public event ErrorHanlder EnterError;


        public event SuccessHanlder AbandonSuccess;
        #endregion

    }
}
