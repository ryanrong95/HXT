using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Settings;
using Needs.Utils.Converters;
using Needs.Utils.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Erp.Models
{
    public class ErpAdmin
    {
        public event SuccessHanlder LoginSuccess;
        public event ErrorHanlder LoginFailed;
        public string ID { get; private set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public ErpAdmin()
        {
            this.LoginSuccess += Admin_Token_LoginSuccess;
        }

        private void Admin_Token_LoginSuccess(object sender, SuccessEventArgs e)
        {
            var admin = e.Object as ErpAdmin;
            if (admin == null)
            {
                throw new Exception("A serious mistake!");
            }

            DateTime loginDate = DateTime.Now;
            string token = string.Concat("$", admin.UserName, "*", "&", loginDate.ToString(), "#").MD5("x");

            using (var reponsitory = new BvnErpReponsitory())
            {
                //纯属多余，历史遗留
                reponsitory.Update<Layer.Data.Sqls.BvnErp.Admins>(new
                {
                    LoginDate = loginDate
                }, item => item.ID == admin.ID);

                reponsitory.Insert(new Layer.Data.Sqls.BvnErp.Tokens
                {
                    ID = "ATOKEN" + loginDate.Ticks,
                    CreateDate = loginDate,
                    OutID = admin.ID,
                    Token = token
                });
            }

            if (Cookies.Supported)
            {
                Cookies.Current[SettingsManager<IAdminSettings>.Current.LoginCookieName] = token;
            }
        }

        public void Login()
        {
            string password;

#if true
            password = this.Password.PasswordOld();
#else
            password = this.Password.Password();
#endif

            using (BvnErpReponsitory reponsitory = new BvnErpReponsitory())
            {
                var linq = from item in reponsitory.ReadTable<Layer.Data.Sqls.BvnErp.Admins>()
                           where item.UserName == this.UserName && item.Status != (int)Generic.Status.Delete
                           select new
                           {
                               item.ID,
                               item.UserName,
                               item.Password
                           };

                var admin = linq.SingleOrDefault();

                if (admin != null && admin.Password == password)
                {
                    this.ID = admin.ID;

                    if (this != null && this.LoginSuccess != null)
                    {
                        this.LoginSuccess(admin, new SuccessEventArgs(this));
                    }
                }
                else
                {
                    if (this != null && this.LoginFailed != null)
                    {
                        this.LoginFailed(this, new ErrorEventArgs("Login failure"));
                    }
                }
            }
        }

    }
}
