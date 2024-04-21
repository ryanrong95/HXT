using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Usually;
using YaHv.CrmPlus.Services.Models.Origins;

namespace Yahv.CrmPlus.Service.Models.Origins
{
    /// <summary>
    ///客户与我方公司的关联关系
    /// </summary>
    public class Relation: IUnique
    {
        #region  属性
        public string ID { get; set; }

        /// <summary>
        /// 合作类型
        /// </summary>
        public ConductType Type { get; set; }  

        /// <summary>
        /// 所有人
        /// </summary>
        public string OwnerID { get; set; }

        /// <summary>
        /// 所属人
        /// </summary>
        public Admin Admin { get; set; }

        public string CompanyID { get; set; }


        public Enterprise Company { get; set; }

      
        public Enterprise Enterprise { get; set; }


        public string ClientID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Yahv.Underly.AuditStatus  Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Summary { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreatorID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 审批时间
        /// </summary>
        public DateTime? OfferDate { get; set; }
        #endregion

        public Relation() {

            this.CreateDate = DateTime.Now;
            this.Status=AuditStatus.Waiting;
        }
        #region  持久化
        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvdCrmReponsitory>.Create())
            {
                var exsit = reponsitory.GetTable<Layers.Data.Sqls.PvdCrm.Relations>().FirstOrDefault(x => x.ClientID == this.ClientID &&x.CompanyID==this.CompanyID);
                if (exsit==null)
                {
                    reponsitory.Insert(new Layers.Data.Sqls.PvdCrm.Relations()
                    {
                        ID = PKeySigner.Pick(PKeyType.Relations),
                        Type=(int)this.Type,
                        OwnerID=this.OwnerID,
                        CompanyID=this.CompanyID,
                        ClientID=this.ClientID,
                        Status=(int)this.Status,
                        Summary=this.Summary,
                        CreatorID=this.CreatorID,
                        CreateDate=this.CreateDate,
                        OfferDate=this.OfferDate
                      
                    });
                }
                else
                {
                    reponsitory.Update<Layers.Data.Sqls.PvdCrm.Relations>(new
                    {
                        Type = (int)this.Type,
                        OwnerID = this.OwnerID,
                        CompanyID = this.CompanyID,
                        ClientID = this.ClientID,
                        Status = (int)this.Status,
                        Summary = this.Summary,
                        CreatorID = this.CreatorID,
                        OfferDate = this.OfferDate

                    }, x => x.ID==exsit.ID);

                }
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


        public event SuccessHanlder AbandonSuccess;

        #endregion

    }
}
