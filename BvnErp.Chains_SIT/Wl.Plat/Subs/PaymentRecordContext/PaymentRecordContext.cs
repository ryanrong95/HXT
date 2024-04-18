using Needs.Wl.User.Plat.Models;
using System.Linq;

namespace Needs.Wl.User.Plat
{
    public sealed class PaymentRecordContext
    {
        IPlatUser User;

        private PaymentRecordContext()
        {

        }

        public PaymentRecordContext(IPlatUser user)
        {
            this.User = user;
        }

        public PaymentRecordContextExtends this[string id]
        {
            get
            {
                if (string.IsNullOrEmpty(id))
                {
                    throw new System.Exception("非法调用了用户的付款记录信息");
                }

                //验证是否为本用户的付款记录
                //抛出异常，禁止用户查看
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ReceiptNotices>().Count(s => s.ClientID == this.User.ClientID && s.ID == id);

                    if (count == 0)
                    {
                        throw new System.Exception("非法调用了用户的付款记录信息");
                    }
                }

                return new PaymentRecordContextExtends(id);
            }
        }
    }
}