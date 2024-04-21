using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbErm;
using Layers.Linq;
using Yahv.Settings;
using Yahv.Underly;
using Yahv.Utils.Converters.Contents;
using Yahv.Utils.Http;
using Yahv.Usually;

namespace Yahv.Plats.Services
{
    /// <summary>
    /// 登录
    /// </summary>
    public class LoginUser
    {
        #region 事件

      
        

        #endregion

        #region 属性

        /// <summary>
        /// 登录名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        #endregion

        #region 构造函数

        public LoginUser()
        {
            //this.LoginSuccess += Admin_Token_LoginSuccess;
        }

        #endregion

        #region 业务方法

        /// <summary>
        /// 登录
        /// </summary>
        public void Login()
        {
            //var user = ApiHelper.Current.Get<User>("", new
            //{
            //    UserName = this.UserName,
            //    Password = this.Password
            //});


            

        }

        private void Admin_Token_LoginSuccess(object sender, SuccessEventArgs e)
        {
            //var admin = sender as Models.AdminRoll;

            //if (admin == null)
            //{
            //    throw new Exception("A serious mistake!");
            //}

            //DateTime loginDate = DateTime.Now;
            //string token = string.Concat("$", admin.UserName, "*", "&", loginDate.ToString(), "#").MD5("x");

            //using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            //{
            //    //更新最近登录时间
            //    repository.Update<Admins>(new
            //    {
            //        LastLoginDate = loginDate
            //    }, item => item.ID == admin.ID);

            //    //添加token表
            //    repository.Insert(new Tokens
            //    {
            //        ID = "ATOKEN" + loginDate.Ticks,
            //        CreateDate = loginDate,
            //        OutID = admin.ID,
            //        Token = token,
            //        Type = (int)TokenType.Login,
            //    });
            //}

            //if (Cookies.Supported)
            //{
            //    Cookies.Current[SettingsManager<IAdminSettings>.Current.LoginCookieName] = token;
            //}
        }

        #endregion
    }
}
