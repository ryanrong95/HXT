using System;
using System.Linq;
using Layers.Data.Sqls;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Views.Origins;
using Yahv.Linq;

namespace Yahv.Finance.Services.Views.Rolls
{
    /// <summary>
    /// 收款核销
    /// </summary>
    public class PayeeRightsRoll : UniqueView<PayeeRight, PvFinanceReponsitory>
    {
        public PayeeRightsRoll()
        {
        }

        public PayeeRightsRoll(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<PayeeRight> GetIQueryable()
        {
            //var leftsView = new PayeeLeftsOrigin(this.Reponsitory);
            //var rightsView = new PayeeRightsOrigin(this.Reponsitory);
            //return from entity in rightsView
            //       join left in rightsView on entity.PayeeLeftID equals left.ID into _left
            //       from left in _left.DefaultIfEmpty()
            //       select new PayeeRight()
            //       {
            //           Currency = entity.Currency,
            //           ID = entity.ID,
            //           CreatorID = entity.CreatorID,
            //           CreateDate = entity.CreateDate,
            //           Price = entity.Price,
            //           Price1 = entity.Price1,
            //           Currency1 = entity.Currency1,
            //           ERate1 = entity.ERate1,
            //           SenderID = entity.SenderID,
            //           Department = entity.SenderID,
            //           PayeeLeftID = entity.PayeeLeftID,
            //       };

            return new PayeeRightsOrigin(this.Reponsitory);
        }

        /// <summary>
        /// 根据流水号获取核销余额
        /// </summary>
        /// <param name="formcode"></param>
        /// <returns></returns>
        public decimal GetWriteOffBalance(string formcode)
        {
            using (var payeeView = new PayeeLeftsRoll(this.Reponsitory))
            {
                var payeeLeft = payeeView.SingleOrDefault(item => item.FormCode == formcode);
                if (payeeLeft == null || string.IsNullOrWhiteSpace(payeeLeft.ID))
                {
                    throw new Exception($"未找到该流水号!");
                }

                decimal leftPrice = payeeLeft?.Price ?? 0;
                decimal rightSum = this.Where(item => item.PayeeLeftID == payeeLeft.ID).ToArray().Sum(item => item?.Price) ?? 0;

                return leftPrice - rightSum;
            }
        }
    }
}