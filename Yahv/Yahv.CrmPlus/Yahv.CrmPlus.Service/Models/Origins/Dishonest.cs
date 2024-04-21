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
using YaHv.CrmPlus.Services.Models.Origins;

namespace Yahv.CrmPlus.Service.Models.Origins
{
    public class SupplierDisHonest : Dishonest
    {
        public string SupplierID {internal set; get; }
    }
    public class Dishonest : IUnique
    {
        public Dishonest()
        {
            this.CreateDate = DateTime.Now;
            this.Status = DataStatus.Normal;
        }
        #region 属性
        public string ID { set; get; }
        public string EnterpriseID { set; get; }
        public string EnterpriseName { internal set; get; }
        /// <summary>
        /// 失信原因
        /// </summary>
        public string Reason { set; get; }
        /// <summary>
        /// 相关单据号
        /// </summary>
        public string Code { set; get; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { set; get; }
        public string CreatorID { set; get; }
        /// <summary>
        /// 创建人
        /// </summary>
        public Admin Creator { internal set; get; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { set; get; }
        /// <summary>
        /// 失信时间
        /// </summary>
        public DateTime OccurTime { set; get; }
        /// <summary>
        /// 状态
        /// </summary>
        public DataStatus Status { set; get; }
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
        #endregion

        #region 持久化
        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvdCrmReponsitory>.Create())
            {
                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    this.ID = PKeySigner.Pick(Yahv.CrmPlus.Service.PKeyType.Dishonest);
                    reponsitory.Insert(new Layers.Data.Sqls.PvdCrm.Dishonests
                    {
                        ID = this.ID,
                        EnterpriseID = this.EnterpriseID,
                        Reason = this.Reason,
                        OccurTime = this.OccurTime,
                        Summary = this.Summary,
                        CreateDate = this.CreateDate,
                        CreatorID = this.CreatorID,
                        Code = this.Code,
                        Status = (int)this.Status
                    });
                }
                this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this));
            }
        }
        public void Abondon()
        {
            using (var reponsitory = LinqFactory<PvdCrmReponsitory>.Create())
            {
                reponsitory.Update<Layers.Data.Sqls.PvdCrm.Dishonests>(new
                {
                    Status = DataStatus.Closed
                }, item => item.ID == this.ID);
                this.AbandonSuccess?.Invoke(this, new SuccessEventArgs(this));
            }
        }
        #endregion
    }
}
