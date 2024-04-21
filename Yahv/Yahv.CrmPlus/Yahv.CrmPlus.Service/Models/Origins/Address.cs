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
   public  class Address: Yahv.Linq.IUnique
 
    {
        #region  属性
        public string ID { get; set; }

        public string EnterpriseID { get; set; }

        public Enterprise Enterprise { get; set; }
        /// <summary>
        /// 所属业务
        /// </summary>
        public RelationType RelationType { get; set; }
        /// <summary>
        /// 收货人类型
        /// </summary>
        public AddressType AddressType { get; set; }
        /// <summary>
        /// 地址抬头
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 国家与地区
        /// </summary>
        public string District { get; set; }

        /// <summary>
        /// 国家地区中文描述
        /// </summary>
        public string PlaceDescription
        {
            get
            {
                if (string.IsNullOrEmpty(this.District))
                { return ""; }
                var origins = Enum.GetValues(typeof(Origin)).Cast<Origin>().Select(item => item.GetOrigin());
                origins = origins.Where(item => item.Code == this.District);
                if (origins.Count() == 0)
                {
                    try
                    {
                        return ((Origin)int.Parse(this.District)).GetOrigin().ChineseName;
                    }
                    catch
                    {
                        return "";
                    }
                }

                var name = origins.FirstOrDefault().ChineseName;
                return name;
            }
        }
        /// <summary>
        /// 地址内容
        /// </summary>
        public string Context { get; set; }
        /// <summary>
        /// 联系人
        /// </summary>
        public string Contact { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string Phone { get; set; }
        public string PostZip { get; set; }
        public string DyjCode { get; set; }
        public AuditStatus Status { get; set; }
        /// <summary>
        ///创建人
        /// </summary>
        public YaHv.CrmPlus.Services.Models.Origins.Admin Admin { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatorID { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime ModifyDate { get; set; }

        public string Summary { get; set; }

        public Address()
        {
            this.CreateDate = DateTime.Now;
            this.ModifyDate = DateTime.Now;
            this.Status = AuditStatus.Waiting;
        }
        #endregion

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
        #region 持久化
        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvdCrmReponsitory>.Create())
            {
                using (var tran = reponsitory.OpenTransaction())
                {
                    if (!reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Addresses>().Any(item => item.ID == this.ID))
                    {
                        this.ID = PKeySigner.Pick(Yahv.CrmPlus.Service.PKeyType.Addresse);
                        reponsitory.Insert(new Layers.Data.Sqls.PvdCrm.Addresses() {
                            ID = this.ID,
                            EnterpriseID = this.EnterpriseID,
                            Contact = this.Contact,
                            Type=(int)this.AddressType,
                            RelationType=(int)this.RelationType,
                            Title=this.Title,
                            District=this.District,
                            Context=this.Context,
                            Phone=this.Phone,
                            PostZip=this.PostZip,
                            DyjCode=this.DyjCode,
                            Status=(int)this.Status,
                            CreatorID=this.CreatorID,
                            CreateDate=this.CreateDate,
                            ModifyDate=this.ModifyDate,
                            Summary=this.Summary
                        });

                    }
                    else {
                        reponsitory.Update<Layers.Data.Sqls.PvdCrm.Addresses>(new {
                            ID =this.ID,
                            EnterpriseID = this.EnterpriseID,
                            Contact = this.Contact,
                            Type = (int)this.AddressType,
                            RelationType = (int)this.RelationType,
                            Title = this.Title,
                            District = this.District,
                            Context = this.Context,
                            Phone = this.Phone,
                            PostZip = this.PostZip,
                            DyjCode = this.DyjCode,
                            Status = (int)this.Status,
                            CreatorID = this.CreatorID,
                            CreateDate = this.CreateDate,
                            ModifyDate = this.ModifyDate,
                            this.Summary

                        }, item => item.ID==this.ID);
                        }

                    tran.Commit();

                    }

               // this.EnterError?.Invoke(this, new ErrorEventArgs());
                this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this));

            }

        }


        public void Closed()
        {
            using (var repository = LinqFactory<PvdCrmReponsitory>.Create())
            {
                repository.Update<Layers.Data.Sqls.PvdCrm.Addresses>(new
                {
                    Status = AuditStatus.Closed
                }, item => item.ID == this.ID);
                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
                }
            }
        }

        public void Enable()
        {
            using (var repository = LinqFactory<PvdCrmReponsitory>.Create())
            {
                repository.Update<Layers.Data.Sqls.PvdCrm.Addresses>(new
                {
                    Status = AuditStatus.Normal
                }, item => item.ID == this.ID);
                if (this != null && this.EnterSuccess != null)
                {
                    this.EnterSuccess(this, new SuccessEventArgs(this.ID));
                }
            }
        }

        public void Refuse()
        {
            using (var repository = LinqFactory<PvdCrmReponsitory>.Create())
            {
                repository.Update<Layers.Data.Sqls.PvdCrm.Addresses>(new
                {
                    Status = AuditStatus.Voted,
                    Summary=this.Summary
                }, item => item.ID == this.ID);
                if (this != null && this.EnterSuccess != null)
                {
                    this.EnterSuccess(this, new SuccessEventArgs(this.ID));
                }
            }
        }

        public void Approve()
        {
            using (var repository = LinqFactory<PvdCrmReponsitory>.Create())
            {
                repository.Update<Layers.Data.Sqls.PvdCrm.Addresses>(new
                {
                    Status = AuditStatus.Normal
                }, item => item.ID == this.ID);
                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this));
                }
            }
        }
        #endregion



    }
}
