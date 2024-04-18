using System;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 会员扩展方法
    /// </summary>
    public static partial class ClientExtends
    {
        public static Layer.Data.Sqls.ScCustoms.Clients ToLinq(this Client entity)
        {
            return new Layer.Data.Sqls.ScCustoms.Clients
            {
                ID = entity.ID,
                CompanyID = entity.Company.ID,
                ClientType = (int)entity.ClientType,
                ClientRank = (int)entity.ClientRank,
                ClientCode = entity.ClientCode,
                AdminID = entity.Admin.ID,
                ClientStatus = (int)entity.ClientStatus,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = DateTime.Now,
                Summary = entity.Summary
            };
        }
    }
}