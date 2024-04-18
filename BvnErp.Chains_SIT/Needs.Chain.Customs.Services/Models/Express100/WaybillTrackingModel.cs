using Needs.Linq;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class WaybillTrackingModel : IUnique, IPersist
    {
        string id;
        public string ID
        {
            get
            {
                return this.id;
            }
            set
            {
                this.id = value;
            }
        }
        public string ExpCompanyCode { get; set; }
        public string ExpCompanyName { get; set; }
        public string ExpNumber { get; set; }
        public Enums.Status Status { get; set; }
        public Enums.State State { get; set; }
        public string WaybillTrackingID { get; set; }
        public DateTime WaybillTrackingTime { get; set; }
        public string WaybillTrackingContext { get; set; }
        public string Url { get; set; }
        public string Summary { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public IEnumerable<JToken> DatasList { get; set; }

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

        virtual protected void OnAbandonSuccess()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        public void Enter()
        {

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Express100WaybillTracking>().Where(item => item.ExpCompanyCode == this.ExpCompanyCode &&
                item.ExpNumber == this.ExpNumber && item.State != (int)this.State).Count();
                if (count == 0)
                {
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.Express100WaybillTracking
                    {
                        ID = this.ID,
                        ExpCompanyCode = this.ExpCompanyCode,
                        ExpNumber = this.ExpNumber,
                        Status = (int)this.Status,
                        State = (int)this.State,
                        Url = "http://poll.kuaidi100.com/poll/query.do",
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now
                    });

                }
                else
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.Express100WaybillTracking>(new
                    {
                        State = (int)this.State,
                        UpdateDate = DateTime.Now,
                    }, item => item.ID == this.ID);

                    reponsitory.Delete<Layer.Data.Sqls.ScCustoms.Express100WaybillTrackingDetails>(item => item.WaybillTrackingID == this.WaybillTrackingID);

                }

                foreach (var item in this.DatasList)
                {
                    var WaybillTrackingTime = Convert.ToDateTime(item["ftime"].ToString());
                    var WaybillTrackingContext = item["context"].ToString();
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.Express100WaybillTrackingDetails
                    {
                        ID = Guid.NewGuid().ToString("N"),
                        WaybillTrackingID = this.WaybillTrackingID,
                        WaybillTrackingTime = WaybillTrackingTime,
                        WaybillTrackingContext = WaybillTrackingContext,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now
                    });
                }
            }

            this.OnEnterSuccess();
        }
        virtual protected void OnEnterSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }
    }
}
