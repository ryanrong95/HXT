using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class CostApplyLog : IUnique, IFulError, IFulSuccess
    {
        /// <summary>
        /// 
        /// </summary>
        public string ID { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string CostApplyID { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string AdminID { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public Enums.CostStatusEnum CurrentCostStatus { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Enums.CostStatusEnum NextCostStatus { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Summary { get; set; } = string.Empty;

        public event ErrorHanlder EnterError;
        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event SuccessHanlder EnterSuccess;

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.CostApplyLogs>().Count(item => item.ID == this.ID);
                if (count == 0)
                {
                    reponsitory.Insert<Layer.Data.Sqls.ScCustoms.CostApplyLogs>(new Layer.Data.Sqls.ScCustoms.CostApplyLogs
                    {
                        ID = this.ID,
                        CostApplyID = this.CostApplyID,
                        AdminID = this.AdminID,
                        CurrentCostStatus = (int)this.CurrentCostStatus,
                        NextCostStatus = (int)this.NextCostStatus,
                        CreateDate = this.CreateDate,
                        Summary = this.Summary,
                    });
                }
                else
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.CostApplyLogs>(new
                    {
                        CostApplyID = this.CostApplyID,
                        AdminID = this.AdminID,
                        CurrentCostStatus = (int)this.CurrentCostStatus,
                        NextCostStatus = (int)this.NextCostStatus,
                        CreateDate = this.CreateDate,
                        Summary = this.Summary,
                    }, item => item.ID == this.ID);
                }
            }
            this.OnEnterSuccess();
        }

        virtual public void OnEnterSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }



    }
}
