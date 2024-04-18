using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using YaHv.PvData.Services.Interfaces;
using YaHv.PvData.Services.Models;

namespace YaHv.PvData.Services.Extends
{
    public static class ClassifyProductExtend
    {
        /// <summary>
        /// 批量锁定
        /// </summary>
        /// <param name="items">需要锁定的OrderItems</param>
        public static void BatchLock(this IEnumerable<IClassifyProduct> items)
        {
            using (var reponsitory = LinqFactory<PvDataReponsitory>.Create())
            {
                reponsitory.Insert(items.Select(item => new Layers.Data.Sqls.PvData.Locks_Classify()
                {
                    ID = Utils.GuidUtil.NewGuidUp(),
                    MainID = item.ID,
                    LockDate = DateTime.Now,
                    LockerID = item.CreatorID,
                    LockerName = item.CreatorName,
                    Status = (int)GeneralStatus.Normal
                }).ToArray());

                reponsitory.Insert(items.Select(item => new Layers.Data.Sqls.PvData.Logs_ClassifyOperating()
                {
                    ID = Layers.Data.PKeySigner.Pick(PKeyType.ClassifyOperatingLog),
                    MainID = item.ID,
                    CreatorID = item.CreatorID,
                    LogType = (int)LogType.Lock,
                    CreateDate = DateTime.Now,
                    Summary = "报关员【" + item.CreatorName + "】锁定了" + item.Step.GetDescription()
                }).ToArray());
            }
        }
    }
}
