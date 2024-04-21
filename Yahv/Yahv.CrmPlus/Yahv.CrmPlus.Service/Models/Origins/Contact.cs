using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Extends;
using Yahv.Underly;
using Yahv.Usually;
using YaHv.CrmPlus.Services.Models.Origins;

namespace Yahv.CrmPlus.Service.Models.Origins
{
    public class Contact : Yahv.Linq.IUnique
    {
        #region  属性
        public string ID { get; set; }

        public string EnterpriseID { get; set; }

        public Enterprise Enterprise { get; set; }

        public RelationType RelationType { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        public string Department { get; set; }
        /// <summary>
        /// 职位
        /// </summary>
        public string Positon { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Tel { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { set; get; }

        /// <summary>
        /// 性别
        /// </summary>
        public string Gender { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string QQ { get; set; }
        /// <summary>
        /// 
        /// </summary>

        public string Wx { get; set; }
        /// <summary>
        /// 
        /// </summary>

        public string Skype { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Summary { get; set; }
        /// <summary>
        /// 性格
        /// </summary>
        public string Character { get; set; }
        /// <summary>
        /// 忌讳
        /// </summary>
        public string Taboo { get; set; }
        /// <summary>
        /// 所属人
        /// </summary>
        public string OwnerID { get; set; }
        /// <summary>
        /// 所属人
        /// </summary>
        public Admin Admin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DataStatus Status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 名片
        /// </summary>

        public List<CallFile> Cards { set; get; }

        #endregion

        public Contact()
        {
            this.CreateDate = DateTime.Now;
            this.Status = DataStatus.Normal;
        }

        #region 持久化
        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvdCrmReponsitory>.Create())
            using (var tran = reponsitory.OpenTransaction())
            {
                {
                    //添加
                    if (!reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Contacts>().Any(item => item.ID == this.ID))
                    {
                        this.ID = PKeySigner.Pick(Yahv.CrmPlus.Service.PKeyType.Contacts);
                        reponsitory.Insert(new Layers.Data.Sqls.PvdCrm.Contacts()
                        {
                            ID = this.ID,
                            EnterpriseID = this.EnterpriseID,
                            RelationType = (int)this.RelationType,
                            Name = this.Name,
                            Mobile = this.Mobile,
                            Email = this.Email,
                            Department = this.Department,
                            Positon = this.Positon,
                            Tel = this.Tel,
                            Gender = this.Gender,
                            qq = this.QQ,
                            Wx = this.Wx,
                            Skype = this.Skype,
                            Summary = this.Summary,
                            Character = this.Character,
                            Taboo = this.Taboo,
                            CreateDate = this.CreateDate,
                            OwnerID = this.OwnerID,
                            Status = (int)this.Status
                        });
                    }
                    //修改
                    else
                    {
                        reponsitory.Update<Layers.Data.Sqls.PvdCrm.Contacts>(new
                        {
                            //EnterpriseID = this.EnterpriseID,
                            //RelationType = (int)this.RelationType,
                            Name = this.Name,
                            Mobile = this.Mobile,
                            Department = this.Department,
                            Email = this.Email,
                            Positon = this.Positon,
                            Tel = this.Tel,
                            Gender = this.Gender,
                            qq = this.QQ,
                            Wx = this.Wx,
                            Skype = this.Skype,
                            Summary = this.Summary,
                            Character = this.Character,
                            Taboo = this.Taboo,
                            //CreateDate = this.CreateDate,
                            //OwnerID = this.OwnerID,
                            //Status = (int)this.Status
                        }, item => item.ID == this.ID);
                    }
                    if (this.Cards?.Count() > 0)
                    {
                        var cardlist = new Views.Rolls.FilesDescriptionRoll()[this.EnterpriseID, this.ID, CrmFileType.VisitingCard].ToArray();
                        cardlist.Abandon();//废弃
                        List<FilesDescription> listFile = new List<FilesDescription>();
                        foreach (var item in this.Cards)
                        {
                            var file = new FilesDescription
                            {
                                EnterpriseID = this.EnterpriseID,
                                SubID = this.ID,
                                CustomName = item.FileName,
                                Url = item.CallUrl,
                                Type = CrmFileType.VisitingCard,
                                CreatorID = this.OwnerID
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


        public void Closed()
        {
            using (var repository = LinqFactory<PvdCrmReponsitory>.Create())
            {
                repository.Update<Layers.Data.Sqls.PvdCrm.Contacts>(new
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
                repository.Update<Layers.Data.Sqls.PvdCrm.Contacts>(new
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
