using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.ClientModels;

namespace Yahv.PvWsOrder.Services.ClientViews
{
    /// <summary>
    /// 获取特殊要求
    /// </summary>
    public class OrderRequirementOrigin : UniqueView<OrderRequirement, PvWsOrderReponsitory>
    {
        public OrderRequirementOrigin()
        {

        }

        public OrderRequirementOrigin(PvWsOrderReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<OrderRequirement> GetIQueryable()
        {
            return from require in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderRequirements>()
                   select new OrderRequirement
                   {
                       ID = require.ID,
                       OrderID = require.OrderID,
                       Type = (Underly.SpecialRequire)require.Type,
                       Name = require.Name,
                       Quantity = require.Quantity,
                       UnitPrice = require.UnitPrice,
                       TotalPrice=require.TotalPrice,
                       Requirement = require.Requirement,
                       CreateDate = require.CreateDate,
                       ModifyDate = require.ModifyDate,
                   };
        }
    }
}
