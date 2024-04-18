using System;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 
    /// </summary>
    public static class PayExchangeApplyFileExtends
    {
        public static Layer.Data.Sqls.ScCustoms.PayExchangeApplyFiles ToLinq(this Models.PayExchangeApplyFile entity)
        {
            return new Layer.Data.Sqls.ScCustoms.PayExchangeApplyFiles
            {
                ID = entity.ID,
                PayExchangeApplyID = entity.PayExchangeApplyID,
                AdminID = entity.AdminID,
                UserID = entity.UserID,
                Name = entity.Name,
                FileFormat = entity.FileFormat,
                FileType = (int)entity.FileType,
                Url = entity.Url,
                Status = (int)entity.Status,
                CreateDate = DateTime.Now,
                Summary = entity.Summary
            };
        }
    }
}