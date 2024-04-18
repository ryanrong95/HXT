using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Needs.Ccs.Services.Models
{
    public class SwapNoticeLog : IUnique, IFulError, IFulSuccess
    {
        #region

        public string ID { get; set; }

        public string SwapNoticeID { get; set; }

        public string Admin { get; set; }

        public Enums.SwapStatus Status { get; set; }

        public DateTime CreateDate { get; set; }

        public string Summary { get; set; }

        #endregion

        //事件
        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    this.ID = Guid.NewGuid().ToString("N");
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.SwapNoticesLogs
                    {
                        ID = this.ID,
                        SwapNoticeID = this.SwapNoticeID,
                        AdminID = this.Admin,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        Summary = this.Summary,
                    });
                }
            }
            catch (Exception ex)
            {
                this.EnterError(this, new ErrorEventArgs(ex.Message));
            }
        }
        public void Update()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.SwapNotices>(new
                    {
                      //  AdminID = this.Admin,
                        Status = (int)this.Status,
                        UpdateDate = this.CreateDate,
                    }, t => t.ID == this.ID);
                }
            }
            catch (Exception ex)
            {
                this.EnterError(this, new ErrorEventArgs(ex.Message));
            }
        }
    }
}