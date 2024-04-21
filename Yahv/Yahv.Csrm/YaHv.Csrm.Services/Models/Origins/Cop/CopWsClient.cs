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
using Yahv.Utils.Converters.Contents;

namespace YaHv.Csrm.Services.Models.Origins
{
    public class CopWsClient : Yahv.Linq.IUnique
    {
        public CopWsClient()
        {
            this.CreateDate = this.UpdateDate = DateTime.Now;
            this.Status = ApprovalStatus.UnComplete;
        }
        #region 属性
        public string ID { set; get; }
        public string ClientID { set; get; }
        /// <summary>
        /// 客户名称
        /// </summary>
        public string Name { set; get; }
        public string Corperation { set; get; }
        public string Uscc { set; get; }
        public string RegAddress { set; get; }

        public bool Vip { set; get; }
        public int Grade { set; get; }
        /// <summary>
        /// 入仓号
        /// </summary>
        public string EnterCode { set; get; }
        /// <summary>
        /// 海关编码
        /// </summary>
        public string CustomsCode { set; get; }
        /// <summary>
        /// 性质：
        /// </summary>
        public ClientType Nature { set; get; }
        /// <summary>
        /// 国家/地区
        /// </summary>
        public string Origin { set; get; }
        /// <summary>
        /// 合作公司ID
        /// </summary>
        public string CompanyID { set; get; }
        /// <summary>
        /// 合作公司名称
        /// </summary>
        public string CompanyName { set; get; }
        string coperid;
        /// <summary>
        /// 合作关系ID
        /// </summary>
        public string CoperID
        {
            get
            {
                return string.Join("", this.ClientID, this.Name).MD5();

            }
            set
            {
                this.coperid = value;
            }
        }

        /// <summary>
        /// 合作类型/业务类型：代仓储-采购，代仓储-销售
        /// </summary>
        public Business CooperType { set; get; }
        public DateTime CreateDate { set; get; }
        public DateTime UpdateDate { set; get; }
        public string CreatorID { set; get; }
        public Admin Creator { set; get; }
        public string Summary { set; get; }
        public ApprovalStatus Status { set; get; }
        #endregion


        #region 事件
        /// <summary>
        /// EnterSuccess
        /// </summary>
        virtual public event SuccessHanlder EnterSuccess;
        /// <summary>
        /// AbandonSuccess
        /// </summary>
        virtual public event SuccessHanlder AbandonSuccess;
        virtual public event ErrorHanlder Repeat;
        #endregion

        #region 持久化
        public void Enter()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                if (repository.ReadTable<Layers.Data.Sqls.PvbCrm.MapsCoperation>().Any(item => ID == this.CoperID))
                {
                    if (string.IsNullOrEmpty(this.ID))
                    {
                        if (this != null && this.Repeat != null)
                        {
                            this.Repeat(this, new ErrorEventArgs());
                        }
                    }
                    else
                    {
                        repository.Update<Layers.Data.Sqls.PvbCrm.CopWsClients>(new
                        {
                            Nature = (int)this.Nature,
                            Vip = this.Vip,
                            Grade = (int)this.Grade,
                            UpdateDate = this.UpdateDate,
                            CustomsCode = this.CustomsCode,
                            Summary = this.Summary,
                            Status = (int)this.Status
                        }, item => item.ID == this.ID);

                    }
                }
                else
                {
                    #region 入仓号
                    if (this.EnterCode == "WL")
                    {
                        this.EnterCode = PKeySigner.Pick(PKeyType.WL);
                    }
                    else if (this.EnterCode == "XL")
                    {
                        this.EnterCode = PKeySigner.Pick(PKeyType.XL);
                    }
                    //else if (this.EnterCode == "ICG")
                    //{
                    //    this.EnterCode = PKeySigner.Pick(PKeyType.ICGO);
                    //}
                    else
                    {
                        this.EnterCode = this.EnterCode;
                    }
                    #endregion
                    // this.ID = "";//ID的生成规则？
                    repository.Insert(new Layers.Data.Sqls.PvbCrm.CopWsClients
                    {
                        ID = this.ID,
                        Nature = (int)this.Nature,
                        Vip = this.Vip,
                        CoperID = this.CoperID,
                        Grade = (int)this.Grade,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        AdminID = this.Creator.ID,
                        EnterCode = this.EnterCode,
                        CustomsCode = this.CustomsCode,
                        Summary = this.Summary,
                        Status = (int)this.Status
                    });
                    repository.Insert(new Layers.Data.Sqls.PvbCrm.MapsCoperation
                    {
                        ID = this.CoperID,
                        Type = (int)this.CooperType,
                        MainID = this.CompanyID,
                        SubID = this.CompanyID
                    });

                }
                if (this != null && this.EnterSuccess != null)
                {
                    this.EnterSuccess(this, new SuccessEventArgs(this));
                }
            }
        }
        public void Abandon()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                if (repository.ReadTable<Layers.Data.Sqls.PvbCrm.CopWsClients>().Any(item => item.ID == this.ID))
                {
                    repository.Update<Layers.Data.Sqls.PvbCrm.CopWsClients>(new
                    {
                        Status = (int)GeneralStatus.Deleted
                    }, item => item.ID == this.ID);
                }
                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this));
                }
            }
        }
        #endregion
    }
}
