using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Services.Views;
using Yahv.Services.Models;

namespace YaHv.PvData.Services.Views.Alls
{
    /// <summary>
    /// Ccc信息查询视图
    /// </summary>
    public class CccInfosAll : OthersAll
    {
        protected override IQueryable<Models.Other> GetIQueryable()
        {
            return base.GetIQueryable().OrderByDescending(item => item.OrderDate);
        }
    }
}
