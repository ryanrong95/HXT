using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using Yahv.PvWsClient.Setting;
using Yahv.PvWsOrder.Services.ClientModels;
using Yahv.Underly;
using Yahv.Usually;
using Yahv.Utils.Converters.Contents;
using Yahv.Utils.Http;

namespace Yahv.PvWsClient.Model
{
    public partial class ClientUser : IUser, Yahv.Linq.IUnique
    {
        /// <summary>
        /// 内部访问构造函数
        /// </summary>
        public ClientUser()
        {
            this.LoginSuccess += User_Token_LoginSuccess;
        }

        #region 属性
        /// <summary>
        /// 用户 ID
        /// </summary>
        public string ID { get; internal set; }

        /// <summary>
        /// 用户 登录名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 用户真实姓名
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 是否记住我
        /// </summary>
        public bool IsRemeber { get; set; }

        /// <summary>
        /// 客户ID
        /// </summary>
        public string EnterpriseID
        {
            get
            {
                return this.MyClients.ID;
            }
        }

        /// <summary>
        /// 企业入仓号
        /// </summary>
        public string EnterCode
        {
            get
            {
                return this.MyClients.EnterCode;
            }
        }

        /// <summary>
        /// 芯达通客户名称
        /// </summary>
        public string XDTClientName { get; set; }

        /// <summary>
        /// 芯达通客户ID
        /// </summary>
        public string XDTClientID { get; set; }

        /// <summary>
        /// 客户类型
        /// </summary>
        public Yahv.PvWsOrder.Services.Enums.ClientType XDTClientType { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Mobile { get; internal set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; internal set; }

        /// <summary>
        /// 是否为主账号
        /// </summary>
        public bool IsMain { get; internal set; }

        /// <summary>
        /// 是否正式用户
        /// </summary>
        public bool IsValid { get; internal set; }

        /// <summary>
        /// 登录令牌
        /// </summary>
        public string Token { get; internal set; }

        /// <summary>
        /// 登录的IP地址
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        public GeneralStatus UserStatus { get; set; }

        /// <summary>
        /// 手机端上一次登录时间
        /// </summary>
        public DateTime? MobileLastLoginDate { get; set; }

        #endregion

        /// <summary>
        /// 登录成功
        /// </summary>
        public event SuccessHanlder LoginSuccess;

        /// <summary>
        /// Token生成时间
        /// </summary>
        public DateTime? TokenCreateDate { get; set; }

        #region 业务函数
        /// <summary>
        /// 登录
        /// </summary>
        public void Login()
        {
            string password = this.Password.StrToMD5();
            using (var view = new Views.UsersAlls())
            {
                var user = (from item in view
                            where item.UserName == this.UserName && item.UserStatus == GeneralStatus.Normal
                            select item).FirstOrDefault();

                //是否没有当前用户
                if (user == null || (user.Password != password && user.Password != this.Password))
                {
                    throw new Exception("账号或密码不正确!");
                }

                if (this != null && LoginSuccess != null)
                {
                    this.LoginSuccess(user, new SuccessEventArgs(this));
                }
            }
        }

        /// <summary>
        /// 登录成功生成Token
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void User_Token_LoginSuccess(object sender, SuccessEventArgs e)
        {
            var user = sender as ClientUser;

            if (user == null)
            {
                throw new Exception("A serious mistake!");
            }
            string ip = ClientIP.GetWebClientIp();
            DateTime loginDate = DateTime.Now;
            string token = string.Concat("$", user.UserName, "*", "&", loginDate.ToString("yyyyMMddHHmmssffff"), "#", Guid.NewGuid().ToString("N")).MD5("x");

            using (var reponsitory = LinqFactory<ScCustomReponsitory>.Create())
            {
                reponsitory.Insert(new Layers.Data.Sqls.ScCustoms.UserTokens()
                {
                    ID = "TOKEN" + loginDate.Ticks,
                    CreateDate = loginDate,
                    UserID = user.ID,
                    Token = token,
                    IP = ip,
                    RealCreateTime = loginDate,
                });
            }

            if (Cookies.Supported)
            {
                if (!string.IsNullOrWhiteSpace(Yahv.PvWsOrder.Services.PvClientConfig.Domain))
                {
                    var cookie = Cookies.Domain[Yahv.PvWsOrder.Services.PvClientConfig.Domain];
                    cookie[SettingsManager<IUserSetting>.Current.WebCookieName] = token;
                }
                Cookies.Current[SettingsManager<IUserSetting>.Current.LoginCookieName] = token;
                Cookies.Current[SettingsManager<IUserSetting>.Current.LoginRemeberName] = this.IsRemeber.ToString();
            }
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        public void LoginOut()
        {
            if (!Cookies.Supported)
            {
                throw new NotImplementedException("没有实现不支持cookies的浏览器的登入与退出");
            }
            string token = Cookies.Current[SettingsManager<IUserSetting>.Current.LoginCookieName];
            //先把Session信息清空
            iSession.Current[token] = null;

            //清空Cookie
            //Cookies.Current[SettingsManager<IUserSetting>.Current.LoginCookieName] = "";
            Cookies.Current.Append(SettingsManager<IUserSetting>.Current.LoginCookieName, "", DateTime.Now.AddDays(-1));
            if (this.IsRemeber)
            {
                Cookies.Current[SettingsManager<IUserSetting>.Current.LoginUserIDName] = this.UserName + "," + this.Password;
            }
            else
            {
                //Cookies.Current[SettingsManager<IUserSetting>.Current.LoginRemeberName] = "";
                Cookies.Current.Append(SettingsManager<IUserSetting>.Current.LoginRemeberName, "False", DateTime.Now.AddDays(-1));
                //Cookies.Current[SettingsManager<IUserSetting>.Current.LoginUserIDName] = "";
                Cookies.Current.Append(SettingsManager<IUserSetting>.Current.LoginUserIDName, "", DateTime.Now.AddDays(-1));
            }
            if (!string.IsNullOrWhiteSpace(Yahv.PvWsOrder.Services.PvClientConfig.Domain))
            {
                //var cookie = Cookies.Domain[Yahv.PvWsOrder.Services.PvClientConfig.Domain];
                //cookie[SettingsManager<IUserSetting>.Current.WebCookieName] = "";
                Cookies.Domain[Yahv.PvWsOrder.Services.PvClientConfig.Domain].Append(SettingsManager<IUserSetting>.Current.WebCookieName, "", DateTime.Now.AddDays(-1));
            }
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="password">新密码</param>
        public void ChangePassWord(string oldpassword, string newpassword)
        {
            newpassword = newpassword.StrToMD5();
            oldpassword = oldpassword.StrToMD5();

            //持久化到数据库
            using (var reponsitory = LinqFactory<ScCustomReponsitory>.Create())
            {
                string password = reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.Users>().
                    SingleOrDefault(item => item.ID == this.ID).Password;
                if (password != oldpassword)
                {
                    throw new Exception("当前密码输入错误，请重新输入！");
                }
                reponsitory.Update<Layers.Data.Sqls.ScCustoms.Users>(new
                {
                    Password = newpassword,
                }, item => item.ID == this.ID);
            }
            this.ClearSession();
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="newPwd"></param>
        public void ResetPassWord(string newPwd)
        {
            newPwd = newPwd.StrToMD5();

            //持久化到数据库
            using (var reponsitory = LinqFactory<ScCustomReponsitory>.Create())
            {
                //string password = reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.Users>().
                //    SingleOrDefault(item => item.ID == this.ID).Password;
                reponsitory.Update<Layers.Data.Sqls.ScCustoms.Users>(new
                {
                    Password = newPwd,
                }, item => item.ID == this.ID);
            }
            this.ClearSession();
        }

        /// <summary>
        /// 修改登录名
        /// </summary>
        /// <param name="UserName">登录名</param>
        public void ChangeUserName(string UserName)
        {
            if (string.IsNullOrWhiteSpace(UserName))
            {
                throw new Exception("用户名不能为空!");
            }

            //持久化到数据库
            using (var reponsitory = LinqFactory<ScCustomReponsitory>.Create())
            {
                reponsitory.Update<Layers.Data.Sqls.ScCustoms.Users>(new
                {
                    Name = UserName,
                }, item => item.ID == this.ID);
            }
            this.ClearSession();
        }

        /// <summary>
        /// 修改绑定手机号码
        /// </summary>
        /// <param name="Mobile">新手机号码</param>
        public void ChangeMobile(string Mobile)
        {
            if (string.IsNullOrWhiteSpace(Mobile))
            {
                throw new Exception("手机号码不能为空!");
            }

            //持久化到数据库
            using (var reponsitory = LinqFactory<ScCustomReponsitory>.Create())
            {
                reponsitory.Update<Layers.Data.Sqls.ScCustoms.Users>(new
                {
                    Mobile,
                }, item => item.ID == this.ID);
            }
            this.ClearSession();
        }

        /// <summary>
        /// 修改绑定邮箱
        /// </summary>
        /// <param name="Email">新邮箱</param>
        public void ChangeEmail(string Email)
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                throw new Exception("邮箱不能为空!");
            }
            //持久化到数据库
            using (var reponsitory = LinqFactory<ScCustomReponsitory>.Create())
            {
                reponsitory.Update<Layers.Data.Sqls.ScCustoms.Users>(new
                {
                    Email,
                }, item => item.ID == this.ID);
            }
            this.ClearSession();
        }

        /// <summary>
        /// 清空Session
        /// </summary>
        public void ClearSession()
        {
            string token = Cookies.Current[SettingsManager<IUserSetting>.Current.LoginCookieName];
            //先把Session信息清空
            iSession.Current[token] = null;
        }
        #endregion

        #region 手机

        /// <summary>
        /// 手机端登录
        /// </summary>
        public Tuple<bool, string, LoginMobileReturnModel> LoginMobile(bool checkPassword = true)
        {
            string password = this.Password.StrToMD5();

            ClientUser user;
            using (var view = new Views.UsersAlls())
            {
                user = (from item in view
                        where item.UserName == this.UserName && item.UserStatus == GeneralStatus.Normal
                        select item).FirstOrDefault();
            }

            //要验证密码
            if (checkPassword)
            {
                //是否没有当前用户
                if (user == null || (user.Password != password && user.Password != this.Password))
                {
                    return new Tuple<bool, string, LoginMobileReturnModel>(false, "账号或密码不正确!", new LoginMobileReturnModel());
                }
            }

            string ip = ClientIP.GetWebClientIp();
            DateTime loginDate = DateTime.Now;
            string token = string.Concat("$", user.UserName, "*", "&", loginDate.ToString("yyyyMMddHHmmssffff"), "#", Guid.NewGuid().ToString("N")).MD5("x");

            string TerminalType = null, TerminalSoftwareVersion = null, TerminalMachineInfo = null;
            try
            {
                TerminalType = HttpContext.Current.Request.Headers.Get("TerminalType");
                TerminalSoftwareVersion = HttpContext.Current.Request.Headers.Get("TerminalSoftwareVersion");
                TerminalMachineInfo = HttpContext.Current.Request.Headers.Get("TerminalMachineInfo");

                TerminalType = LimitStringLength(TerminalType, 20);
                TerminalSoftwareVersion = LimitStringLength(TerminalSoftwareVersion, 50);
                TerminalMachineInfo = LimitStringLength(TerminalMachineInfo, 200);
            }
            catch
            {
            }

            using (var reponsitory = LinqFactory<ScCustomReponsitory>.Create())
            {
                reponsitory.Insert(new Layers.Data.Sqls.ScCustoms.UserTokens()
                {
                    ID = "TOKEN" + loginDate.Ticks,
                    CreateDate = loginDate,
                    UserID = user.ID,
                    Token = token,
                    IP = ip,
                    RealCreateTime = loginDate,
                    TerminalType = TerminalType,
                    TerminalSoftwareVersion = TerminalSoftwareVersion,
                    TerminalMachineInfo = TerminalMachineInfo,
                });
            }

            return new Tuple<bool, string, LoginMobileReturnModel>(true, "", new LoginMobileReturnModel
            {
                Token = token,
            });
        }

        /// <summary>
        /// 限制字符串长度
        /// </summary>
        /// <param name="originString"></param>
        /// <returns></returns>
        private string LimitStringLength(string originString, int limitLength)
        {
            if (string.IsNullOrEmpty(originString))
            {
                return originString;
            }

            if (originString.Length > limitLength)
            {
                return originString.Substring(0, limitLength);
            }

            return originString;
        }

        public class LoginMobileReturnModel
        {
            public string Token { get; set; }
        }

        #endregion

        /// <summary>
        /// 根据手机号获取用户
        /// </summary>
        public List<GetUsersByMobileReturnModel> GetUsersByMobile(string mobile)
        {
            using (var view = new Views.UsersAlls())
            {
                var clientUsers = (from item in view
                                   where item.Mobile == mobile && item.UserStatus == GeneralStatus.Normal
                                   select item).ToList();
                if (clientUsers == null)
                {
                    return null;
                }
                return clientUsers.Select(item => new GetUsersByMobileReturnModel
                {
                    ID = item.ID,
                    UserName = item.UserName,
                }).ToList();
            }
        }

        public class GetUsersByMobileReturnModel
        {
            public string ID { get; set; }

            public string UserName { get; set; }
        }

        /// <summary>
        /// 获取当前用户账号
        /// </summary>
        /// <returns></returns>
        public List<GetCurrentUserAccountsReturnModel> GetCurrentUserAccounts()
        {
            List<GetCurrentUserAccountsReturnModel> result = new List<GetCurrentUserAccountsReturnModel>();
            if (string.IsNullOrEmpty(this.Mobile))
            {
                //如果没有手机号，只有一个账号
                result.Add(new GetCurrentUserAccountsReturnModel
                {
                    ID = this.ID,
                    UserName = this.UserName,
                    IsCurrent = true,
                });
            }
            else
            {
                //否则可能有多个账号
                List<ClientUser> users = new List<ClientUser>();
                using (var view = new Views.UsersAlls())
                {
                    users = (from item in view
                             where item.Mobile == this.Mobile && item.UserStatus == GeneralStatus.Normal
                             select item).ToList();
                }

                if (users == null || !users.Any())
                {
                    result.Add(new GetCurrentUserAccountsReturnModel
                    {
                        ID = this.ID,
                        UserName = this.UserName,
                        IsCurrent = true,
                    });
                }
                else
                {
                    result = users.Select(t => new GetCurrentUserAccountsReturnModel
                    {
                        ID = t.ID,
                        UserName = t.UserName,
                        IsCurrent = t.ID == this.ID,
                    }).ToList();
                }
            }

            return result;
        }

        public class GetCurrentUserAccountsReturnModel
        {
            public string ID { get; set; }

            public string UserName { get; set; }

            public bool IsCurrent { get; set; }

            public string AvatarUrl { get; set; }
        }

        /// <summary>
        /// 更换token
        /// </summary>
        /// <returns></returns>
        public ChangeTokenReturnModel ChangeToken()
        {
            var originToken = HttpContext.Current.Request.Headers.Get("token");
            if (string.IsNullOrEmpty(originToken))
            {
                return null;
            }

            using (var reponsitory = LinqFactory<ScCustomReponsitory>.Create())
            {
                var originTokenData = reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.UserTokens>().FirstOrDefault(t => t.Token == originToken);
                if (originTokenData == null)
                {
                    return null;
                }

                var user = reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.Users>().FirstOrDefault(t => t.ID == originTokenData.UserID);
                if (user == null)
                {
                    return null;
                }

                //将原先token设置为还有几分钟就会过期
                reponsitory.Update<Layers.Data.Sqls.ScCustoms.UserTokens>(new
                {
                    CreateDate = DateTime.Now.AddHours(-Consts.TokenOverdueHour).AddMinutes(Consts.TokenTempValidMinute),
                }, item => item.ID == originTokenData.ID);
                //重新登录
                var loginReturn = new ClientUser()
                {
                    UserName = user.Name,
                    Password = "",
                }.LoginMobile(checkPassword: false);

                if (!loginReturn.Item1)
                {
                    //重新登录失败
                    return null;
                }

                //重新登录成功
                return new ChangeTokenReturnModel
                {
                    NewToken = loginReturn.Item3.Token,
                };
            }



        }

        public class ChangeTokenReturnModel
        {
            public string NewToken { get; set; }
        }

        /// <summary>
        /// 注销账号
        /// </summary>
        public Tuple<bool, string> DeleteLoginAccount()
        {
            var token = HttpContext.Current.Request.Headers.Get("token");
            if (string.IsNullOrEmpty(token))
            {
                return new Tuple<bool, string>(false, "header中没有token");
            }

            using (var reponsitory = LinqFactory<ScCustomReponsitory>.Create())
            {
                var tokenData = reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.UserTokens>().FirstOrDefault(t => t.Token == token);
                if (tokenData == null)
                {
                    return new Tuple<bool, string>(false, "不存在该token数据");
                }
                var user = reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.Users>().FirstOrDefault(t => t.ID == tokenData.UserID);
                if (user == null)
                {
                    return new Tuple<bool, string>(false, "不存在该客户数据");
                }

                reponsitory.Update<Layers.Data.Sqls.ScCustoms.Users>(new
                {
                    Status = (int)GeneralStatus.Deleted,
                }, item => item.ID == user.ID);
                reponsitory.Update<Layers.Data.Sqls.ScCustoms.Users>(new
                {
                    Mobile = "注销_" + (!string.IsNullOrEmpty(user.Mobile) ? user.Mobile : string.Empty),
                }, item => item.ID == user.ID);
                reponsitory.Update<Layers.Data.Sqls.ScCustoms.Users>(new
                {
                    Name = "注销_" + (!string.IsNullOrEmpty(user.Name) ? user.Name : string.Empty),
                }, item => item.ID == user.ID);

                return new Tuple<bool, string>(true, "注销账号成功");
            }
        }

    }
}
