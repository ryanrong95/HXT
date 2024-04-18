using Layers.Data.Sqls;
using System.Linq;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.ClientModels;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.PvWsOrder.Services.Views.Origins;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.ClientViews
{
    /// <summary>
    /// 我的申请
    /// </summary>
    public class MyApplicationView : QueryView<object, PvWsOrderReponsitory>
    {
        private IUser User;

        private MyApplicationView()
        {

        }

        public MyApplicationView(IUser user)
        {
            User = user;
        }

        /// <summary>
        /// 查询所有申请
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<object> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.Applications>()
                   where entity.ClientID == User.EnterpriseID && entity.Status == (int)GeneralStatus.Normal
                   select entity;
        }

        /// <summary>
        /// 代收申请
        /// </summary>
        /// <returns></returns>
        public IQueryable<Application> GetReceiveApplication()
        {
            return from apply in this.GetIQueryable().Cast<Layers.Data.Sqls.PvWsOrder.Applications>()
                   join payer in new ApplicationPayersOrigin(this.Reponsitory) on apply.ID equals payer.ApplicationID
                   where apply.Type == (int)ApplicationType.Receival
                   select new Application
                   {
                       ID = apply.ID,
                       ClientID = apply.ClientID,
                       CreateDate = apply.CreateDate,
                       ApplicationStatus = (ApplicationStatus)apply.ApplicationStatus,
                       ReceiveStatus = (ApplicationReceiveStatus)apply.ReceiveStatus,
                       TotalPrice = apply.TotalPrice,
                       PayPayer = payer,
                   };
        }

        /// <summary>
        /// 代付申请
        /// </summary>
        /// <returns></returns>
        public IQueryable<Application> GetPayApplication()
        {
            return from apply in this.GetIQueryable().Cast<Layers.Data.Sqls.PvWsOrder.Applications>()
                   join payee in new ApplicationPayeesOrigin(this.Reponsitory) on apply.ID equals payee.ApplicationID
                   where apply.Type == (int)ApplicationType.Payment
                   select new Application
                   {
                       ID = apply.ID,
                       ClientID = apply.ClientID,
                       CreateDate = apply.CreateDate,
                       ApplicationStatus = (ApplicationStatus)apply.ApplicationStatus,
                       PaymentStatus = (ApplicationPaymentStatus)apply.PaymentStatus,
                       TotalPrice = apply.TotalPrice,
                       PayPayee = payee,
                   };
        }

        /// <summary>
        /// 获取代付的详情
        /// </summary>
        /// <param name="OrderID">订单号</param>
        /// <returns></returns>
        public Application GetPayDetailByOrderID(string OrderID)
        {
            //获取最早的一条代付申请
            var apply = (from entity in this.GetIQueryable().Cast<Layers.Data.Sqls.PvWsOrder.Applications>()
                         join item in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.ApplicationItems>()
                         on entity.ID equals item.ApplicationID
                         where entity.Type == (int)ApplicationType.Payment && item.OrderID == OrderID
                         orderby entity.CreateDate
                         select entity).FirstOrDefault();

            var paypayee = new ApplicationPayeesOrigin(this.Reponsitory).SingleOrDefault(item => item.ApplicationID == apply.ID);
            var paypayer = new ApplicationPayersOrigin(this.Reponsitory).SingleOrDefault(item => item.ApplicationID == apply.ID);
            //代付货款申请
            return new Application
            {
                ID = apply.ID,
                InCompanyName = apply.InCompanyName,
                InBankName = apply.InBankName,
                InBankAccount = apply.InBankAccount,
                TotalPrice = apply.TotalPrice,
                UserID = apply.UserID,
                PayPayee = paypayee,
                PayPayer = paypayer,
            };
        }

        /// <summary>
        /// 获取代收的详情
        /// </summary>
        /// <param name="OrderID">订单号</param>
        /// <returns></returns>
        public Application GetReceiveDetailByID(string OrderID)
        {
            //获取最早的一条代付申请
            var apply = (from entity in this.GetIQueryable().Cast<Layers.Data.Sqls.PvWsOrder.Applications>()
                         join item in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.ApplicationItems>()
                         on entity.ID equals item.ApplicationID
                         where entity.Type == (int)ApplicationType.Receival && item.OrderID == OrderID
                         orderby entity.CreateDate
                         select entity).FirstOrDefault();

            var receivepayer = new ApplicationPayersOrigin(this.Reponsitory).SingleOrDefault(item => item.ApplicationID == apply.ID);

            //代收货款申请
            return new Application
            {
                ID = apply.ID,
                TotalPrice = apply.TotalPrice,
                Currency = (Currency)apply.Currency,
                InCompanyName = apply.InCompanyName,
                InBankName = apply.InBankName,
                InBankAccount = apply.InBankAccount,
                IsEntry = apply.IsEntry,
                DelivaryOpportunity = (DelivaryOpportunity)apply.DelivaryOpportunity,
                CheckDelivery = (CheckDeliveryType?)apply.CheckDelivery,
                CheckCarrier = apply.CheckCarrier,
                CheckWaybillCode = apply.CheckWaybillCode,
                CheckConsignee = apply.CheckConsignee,
                CheckTitle = apply.CheckTitle,
                CheckPayeeAccount = apply.CheckPayeeAccount,
                UserID = apply.UserID,
                ReceivePayer = receivepayer,
            };
        }

        /// <summary>
        /// 订单保存时删除历史数据
        /// </summary>
        /// <param name="orderid"></param>
        public void DeleteByOrderID(string orderid)
        {
            //是否存在申请
            if (this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.ApplicationItems>().Any(item => item.OrderID == orderid))
            {
                //获取主申请ID
                var applications = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.ApplicationItems>().Where(item => item.OrderID == orderid)
                    .Select(item => item.ApplicationID).ToArray();

                //数据直接删除
                this.Reponsitory.Delete<Layers.Data.Sqls.PvWsOrder.ApplicationItems>(item => applications.Contains(item.ApplicationID));
                this.Reponsitory.Delete<Layers.Data.Sqls.PvWsOrder.ApplicationPayers>(item => applications.Contains(item.ApplicationID));
                this.Reponsitory.Delete<Layers.Data.Sqls.PvWsOrder.ApplicationPayees>(item => applications.Contains(item.ApplicationID));
                this.Reponsitory.Delete<Layers.Data.Sqls.PvWsOrder.Applications>(item => applications.Contains(item.ID));
            }
        }
    }
}
