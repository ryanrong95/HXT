namespace Needs.Wl.Models
{
    public static class PayExchangeLogExtends
    {
        public static Layer.Data.Sqls.ScCustoms.PayExchangeApplyLogs ToLinq(this Models.PayExchangeLog entity)
        {
            return new Layer.Data.Sqls.ScCustoms.PayExchangeApplyLogs
            {
                ID = entity.ID,
                PayExchangeApplyID = entity.PayExchangeApplyID,
                AdminID = entity.AdminID,
                UserID = entity.UserID,
                PayExchangeApplyStatus = (int)entity.PayExchangeApplyStatus,
                CreateDate = entity.CreateDate,
                Summary = entity.Summary
            };
        }
    }
}
