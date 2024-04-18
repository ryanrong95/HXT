using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvWsClient.Model;
using Yahv.PvWsOrder.Services.Enums;

namespace Yahv.PvWsClient.Views
{
    public class UsersAlls : UniqueView<ClientUser, ScCustomReponsitory>
    {
        public  UsersAlls()
        {

        }

        protected override IQueryable<ClientUser> GetIQueryable()
        {
            return from user in this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.Users>()
                   join client in this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.Clients>() on user.ClientID equals client.ID
                   join company in this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.Companies>() on client.CompanyID equals company.ID
                   select new ClientUser
                   {
                       ID = user.ID,
                       RealName = user.RealName,
                       UserName = user.Name,
                       Password = user.Password,
                       XDTClientName = company.Name,
                       XDTClientID = user.ClientID,
                       Mobile = user.Mobile,
                       IsMain = user.IsMain,
                       Email = user.Email,
                       IsValid = client.IsValid.GetValueOrDefault(),
                       XDTClientType= (ClientType)client.ClientType,
                       UserStatus = (Underly.GeneralStatus)user.Status,
                       MobileLastLoginDate = user.MobileLastLoginDate,
                   };
        }

        /// <summary>
        /// 根据条件校验会员信息
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        private bool CheckUserInfo(Expression<Func<ClientUser, bool>> expression)
        {
            var linq = this.IQueryable.Any(expression);

            return linq;
        }

        /// <summary>
        /// 校验用户名是否重复
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="UserName">用户名</param>
        /// <returns></returns>
        public bool UserNameIsExist(string userID, string UserName)
        {
            Expression<Func<ClientUser, bool>> expression = item => item.ID != userID && item.UserName == UserName;

            return this.CheckUserInfo(expression);
        }

        /// <summary>
        /// 手机号码是否重复
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public bool UserPhoneIsExist(string userID, string mobile)
        {
            Expression<Func<ClientUser, bool>> expression = item => item.ID != userID && item.Mobile == mobile;
            return this.CheckUserInfo(expression);
        }


        /// <summary>
        /// 邮箱是否重复
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool UserEmailIsExist(string userID, string email)
        {
            Expression<Func<ClientUser, bool>> expression = item => item.ID != userID && item.Email == email;
            return this.CheckUserInfo(expression);
        }

        /// <summary>
        /// 邮箱是否存在
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool UserEmailIsExist(string email)
        {
            Expression<Func<ClientUser, bool>> expression = item => item.Email == email;
            return this.CheckUserInfo(expression);
        }
    }
}
