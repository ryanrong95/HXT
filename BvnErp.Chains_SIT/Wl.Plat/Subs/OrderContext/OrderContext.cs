using Needs.Wl.Models.Views;
using Needs.Wl.User.Plat.Models;
using System.Linq;

namespace Needs.Wl.User.Plat
{
    public sealed class OrderContext
    {
        IPlatUser User;

        private OrderContext()
        {

        }

        public OrderContext(IPlatUser user)
        {
            this.User = user;
        }

        public OrderContextExtends this[string id]
        {
            get
            {
                if (string.IsNullOrEmpty(id))
                {
                    throw new System.Exception("非法调用了订单信息");
                }

                //验证是否为本用户的订单
                //抛出异常，禁止用户查看
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    int count = 0;
                    if (this.User.IsMain)
                    {
                        count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>().Count(s => s.ClientID == this.User.Client.ID && s.ID == id);
                    }
                    else
                    {
                        count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>().Count(s => s.UserID == this.User.ID && s.ID == id);
                    }

                    if (count == 0)
                    {
                        throw new System.Exception("非法调用了订单信息");
                    }
                }

                return new OrderContextExtends(id);
            }
        }
    }
}