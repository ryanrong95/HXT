using Needs.Erp.Generic;

namespace Needs.Wl.Admin.Plat.Models
{
    public partial interface IAdmin : IGenericErmAdmin, IAdminLocale, Needs.Linq.IUnique
    {
        event Linq.SuccessHanlder PasswordSuccess;
        event Linq.ErrorHanlder PasswordError;

        void ChangePassword(string oldPassWord, string newPassword);

        void Enter();
    }

    public interface IAdminLocale : IGenericErmAdmin
    {
        /// <summary>
        /// 会员、客户
        /// </summary>
        Clients Clients { get; }

        /// <summary>
        /// 报关
        /// </summary>
        Customs Customs { get; }

        /// <summary>
        /// 订单
        /// </summary>
        Order Order { get; }

        /// <summary>
        /// 库房
        /// </summary>
        Warehouse Warehouse { get; }

        /// <summary>
        /// 产品归类
        /// </summary>
        Classify Classify { get; }

        /// <summary>
        /// 报关通知
        /// </summary>
        Declaration Declaration { get; }

        /// <summary>
        /// 订单管控
        /// </summary>
        Control Control { get; }

        /// <summary>
        /// 产品归类
        /// </summary>
        PreProduct PreProduct { get; }
        /// <summary>
        /// 财务管理
        /// </summary>
        Finance Finance { get; }

        /// <summary>
        /// 角色管理
        /// </summary>
        Permissions Permissions { get; }

        /// <summary>
        /// 运输批次
        /// </summary>
        Voyage Voyage { get; }

        /// <summary>
        /// 基础数据
        /// </summary>
        Cbs Cbs { get; }
    }
}
