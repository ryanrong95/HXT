using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class AfterBooking
    {
        public string SwapNoticeID { get; set; }
        public string Uid { get; set; }

        public AfterBooking(string uid, string swapID)
        {
            this.SwapNoticeID = swapID;
            this.Uid = uid;
        }

        /// <summary>
        /// 更新SwapNotices表中的uid
        /// 将FXPricing 记录的IsLcoked 改为 true
        /// </summary>
        public void Process()
        {
            try
            {
                using (var reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.SwapNotices>(new
                    {
                        uid = this.Uid
                    }, item => item.ID == this.SwapNoticeID);

                }

                using (var reponsitory = new Layer.Data.Sqls.ForicDBSReponsitory())
                {
                    reponsitory.Update<Layer.Data.Sqls.foricDBS.FXResponse>(new
                    {
                        isLocked = true
                    }, item => item.uid == this.Uid);
                }
            }
            catch
            {

            }

        }
    }  

    public class AfterACT
    {
        public string TxnRefId { get; set; }

        public AfterACT(string txnRefId)
        {           
            this.TxnRefId = txnRefId;
        }

        public void Process()
        {
            try
            {               
                using (var reponsitory = new Layer.Data.Sqls.ForicDBSReponsitory())
                {
                    reponsitory.Update<Layer.Data.Sqls.foricDBS.FXResponse>(new
                    {
                        isACT = true
                    }, item => item.txnRefId == this.TxnRefId);
                }
            }
            catch
            {

            }

        }
    }

    public class AfterTT
    {
        public string TxnRefId { get; set; }

        public AfterTT(string txnRefId)
        {
            this.TxnRefId = txnRefId;
        }

        public void Process()
        {
            try
            {
                using (var reponsitory = new Layer.Data.Sqls.ForicDBSReponsitory())
                {
                    reponsitory.Update<Layer.Data.Sqls.foricDBS.FXResponse>(new
                    {
                        isTT = true
                    }, item => item.txnRefId == this.TxnRefId);
                }
            }
            catch
            {

            }

        }
    }
}
