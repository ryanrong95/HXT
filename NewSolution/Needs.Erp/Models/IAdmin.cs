using Needs.Erp.Generic;

namespace Needs.Erp.Models
{
    public partial interface IAdmin : IGenericAdmin, IAdminLocale, Needs.Linq.IUnique
    {
        event Linq.SuccessHanlder PasswordSuccess;
        event Linq.ErrorHanlder PasswordError;

        void ChangePassword(string oldPassWord, string newPassword);
    }

    public interface IAdminLocale : IGenericAdmin
    {
        /// <summary>
        /// 暂时的
        /// </summary>
        ClientSolution ClientSolutions { get; }
        /// <summary>
        /// 暂时的
        /// </summary>
        Plot Plots { get; }

        Limit Limits { get; }

        

        Publish Publishs { get; }

        NtErp.Wss.Generic.ErpWebsite Websites { get; }


        //IChains Chains { get; }

        /// <summary>
        /// 订单销售项目
        /// </summary>
        OrderSales OrderSales { get; }

       
    }
}
