using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class FinanceStockInModel : IUnique
    {

        public string ID { get; set; }

        public string DecHeadID { get; set; }

        public string ContrNo { get; set; }

        /// <summary>
        /// 报关单号/海关编号
        /// </summary>
        public string EntryId { get; set; }

        /// <summary>
        /// 申报日期
        /// </summary>
        public DateTime? DDate { get; set; }

        /// <summary>
        /// 缴税状态
        /// </summary>
        public Enums.DecTaxStatus DecTaxStatus { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string ClientName { get; set; }
        /// <summary>
        ///订单号
        /// </summary>
        public string OrderID { get; set; }

        public string Currency { get; set; }

        public decimal? DecTotalAmount { get; set; }


        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;

        public void BatchPush()
        {

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //外部公司插入apinotice,推送进价
                int apiNoticeCount = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ApiNotices>().Count(x => x.ItemID == this.DecHeadID && x.PushType == (int)Enums.PushType.PurchasePrice);
                if (apiNoticeCount == 0)
                {
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.ApiNotices
                    {
                        ID = ChainsGuid.NewGuidUp(),
                        PushType = (int)Enums.PushType.PurchasePrice,
                        PushStatus = (int)Enums.PushStatus.Unpush,
                        ItemID = this.DecHeadID,
                        ClientID = "",
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now
                    });

                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecTaxs>(new
                    {
                        IsPutInSto = 1
                    }, item => item.ID == this.DecHeadID);
                }
            }
            this.OnEnter();

        }

        virtual public void OnEnter()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

    }

    /// <summary>
    /// 批量推送
    /// </summary>
    //public class BatchPush

    //{
    //    private List<FinanceStockInModel> List { get; set; }

    //    public BatchPush(List<FinanceStockInModel> list)
    //    {
    //        this.List = list;
    //    }


    //    public void Insert()
    //    {

    //        using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
    //        {
    //            // var ids = this.List.Select(item => item.DecHeadID);
    //            //外部公司插入apinotice,推送进价
    //            var apiNotices = this.List.Select(entity => new Layer.Data.Sqls.ScCustoms.ApiNotices
    //            {
    //                ID = ChainsGuid.NewGuidUp(),
    //                PushType = (int)Enums.PushType.PurchasePrice,
    //                PushStatus = (int)Enums.PushStatus.Unpush,
    //                ItemID = entity.DecHeadID,
    //                ClientID = "",
    //                CreateDate = DateTime.Now,
    //                UpdateDate = DateTime.Now
    //            }).ToArray();

    //            reponsitory.Insert<Layer.Data.Sqls.ScCustoms.ApiNotices>(apiNotices);

    //        }

    //    }


    //}
}
