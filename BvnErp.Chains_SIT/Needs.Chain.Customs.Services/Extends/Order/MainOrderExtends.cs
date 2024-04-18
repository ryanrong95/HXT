using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 代理订单轨迹扩展方法
    /// </summary>
    public static class MainOrderExtends
    {
        public static Layer.Data.Sqls.ScCustoms.MainOrders ToLinq(this Models.MainOrder entity)
        {
            return new Layer.Data.Sqls.ScCustoms.MainOrders
            {
                ID = entity.ID,
                ClientID = entity.ClientID,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                Summary = entity.Summary,  
                AdminID = entity.AdminID,
                UserID = entity.UserID,
                Type = (int)entity.Type,           
            };
        }
    }
}
