using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 预归类产品管控审批扩展方法
    /// </summary>
    public static class PreProductControlExtends
    {
        /// <summary>
        /// 生成推送通知
        /// </summary>
        /// <param name="ppc"></param>
        /// <param name="reponsitory"></param>
        public static void ToApiNotice(this PreProductControl ppc, Layer.Data.Sqls.ScCustomsReponsitory reponsitory)
        {
            //如果该预归类产品的管控已经全部审批完，则生成推送通知
            int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PreProductControls>()
                .Count(item => item.PreProductID == ppc.PreProductID && item.Status == (int)Enums.PreProductControlStatus.Waiting);
            if (count == 0)
            {
                reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.ApiNotices()
                {
                    ID = ChainsGuid.NewGuidUp(),
                    PushType = (int)Enums.PushType.ClassifyResult,
                    ClientID = ppc.PreProduct.ClientID,
                    ItemID = ppc.Category.ID,
                    PushStatus = (int)Enums.PushStatus.Unpush,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now
                });
            }
        }
    }
}
