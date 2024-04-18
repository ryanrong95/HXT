using NtErp.Crm.Services.Models;

namespace NtErp.Crm.Services.Extends
{
    public static class AppliesExtands
    {
        /// <summary>
        /// model数据映射数据库对象
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        static internal Layer.Data.Sqls.BvCrm.Applies ToLinq(this Apply entity)
        {
            return new Layer.Data.Sqls.BvCrm.Applies()
            {
                ID = entity.ID,
                Type = (int)entity.Type,
                MainID = entity.MainID,
                AdminID = entity.Admin.ID,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                Summary = entity.Summary
            };
        }
    }
}
