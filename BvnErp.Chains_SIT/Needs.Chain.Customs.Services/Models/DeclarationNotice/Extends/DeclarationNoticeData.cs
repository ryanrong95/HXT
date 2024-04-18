using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 用于拆分申报
    /// </summary>
    public class DeclarationNoticeData
    {
        public string OrderID { get; set; }

        public string Summary { get; set; }

        /// <summary>
        /// 拆分时，按照packing封箱
        /// </summary>
        public IEnumerable<Packing> Packings { get; set; }


        public void SplitDeclare()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //插入Notice
                var timeNow = DateTime.Now;
                var noticeID = Needs.Overall.PKeySigner.Pick(PKeyType.DeclareNotice);
                reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.DeclarationNotices
                {
                    ID = noticeID,
                    OrderID = this.OrderID,
                    Status = (int)Enums.DeclareNoticeStatus.UnDec,
                    CreateDate = timeNow,
                    UpdateDate = timeNow,
                    Summary = this.Summary
                });

                this.Packings.ToList().ForEach(pack=> {
                    //插入notice项
                    var itemid = string.Concat(noticeID, pack.ID).MD5();
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.DeclarationNoticeItems
                    {
                        ID = itemid,
                        DeclarationNoticeID = noticeID,
                        SortingID = pack.ID,
                        Status = (int)Enums.DeclareNoticeItemStatus.UnMake,
                        CreateDate = timeNow,
                        UpdateDate = timeNow,
                    });

                    //按照pack封箱
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.Packings>(new { PackingStatus = Enums.PackingStatus.Sealed }, item => item.ID == pack.ID);

                    //修改订单item申报数量
                    foreach (var packitem in pack.Items)
                    {
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderItems>(new { DeclaredQuantity = packitem.Sorting.Quantity }, item => item.ID == packitem.Sorting.OrderItem.ID);
                    }
                });               
            }
        }
    }
}
