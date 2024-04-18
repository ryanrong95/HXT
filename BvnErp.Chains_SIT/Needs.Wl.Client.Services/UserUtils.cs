using Needs.Wl.Models.Views;

namespace Needs.Wl.Client.Services
{
    public static class UserUtils
    {
        /// <summary>
        /// 用户名是否重复
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static bool UserNameIsExist(string userID, string userName)
        {
            UsersView view = new UsersView();
            view.Predicate = item => item.ID != userID && item.Name == userName;
            return view.RecordCount > 0;
        }

        /// <summary>
        /// 手机号码是否重复
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public static bool UserPhoneIsExist(string userID, string mobile)
        {
            UsersView view = new UsersView();
            view.Predicate = item => item.ID != userID && item.Mobile == mobile;
            return view.RecordCount > 0;
        }

        /// <summary>
        /// 邮箱是否重复
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool UserEmailIsExist(string userID, string email)
        {
            UsersView view = new UsersView();
            view.Predicate = item => item.ID != userID && item.Email == email;
            return view.RecordCount > 0;
        }

        /// <summary>
        /// 邮箱是否重复
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool UserEmailIsExist(string email)
        {
            UsersView view = new UsersView();
            view.Predicate = item => item.Email == email;
            return view.RecordCount > 0;
        }
    }
}