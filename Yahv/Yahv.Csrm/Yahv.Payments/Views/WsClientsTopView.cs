using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.Services.Models;

namespace Yahv.Payments.Views
{
    /// <summary>
    /// 代仓储客户视图
    /// </summary>
    public class WsClientsTopView : UniqueView<WsClient, PvbCrmReponsitory>
    {
        public WsClientsTopView()
        {

        }

        public WsClientsTopView(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<WsClient> GetIQueryable()
        {
            return new Yahv.Services.Views.WsClientsTopView<PvbCrmReponsitory>();
        }

        /// <summary>
        /// 索引器
        /// </summary>
        //public WsClient this[string enterpriseId]
        //{
        //    get { return this.SingleOrDefault(item => item.ID == enterpriseId); }
        //}

        public IQueryable<WsClient> this[Enterprise Company]
        {
            get
            {
                return this.Where(item => item.ID == Company.ID);
            }
        }
    }
}
