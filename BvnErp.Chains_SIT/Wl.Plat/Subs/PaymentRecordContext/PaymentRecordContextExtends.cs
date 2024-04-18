
namespace Needs.Wl.User.Plat
{
    public class PaymentRecordContextExtends
    {
        string ID;

        public PaymentRecordContextExtends(string id)
        {
            this.ID = id;
        }

        /// <summary>
        /// 客户的付款记录的对应的订单的收款确认明细
        /// 实收
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public Needs.Wl.Client.Services.Views.PaymentRecordReceivedsView Receiveds
        {
            get { return new Needs.Wl.Client.Services.Views.PaymentRecordReceivedsView(this.ID); }
        }
    }
}