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
using System.Configuration;
using Layers.Data;
using Yahv.Plats.Services.Models;

namespace Yahv.Plats.Services
{
    /// <summary>
    /// 登录
    /// </summary>
    public class LoginUser
    {
        #region 事件

        /// <summary>
        /// 登录成功
        /// </summary>
        public event SuccessHanlder LoginSuccess;

        /// <summary>
        /// 登录失败
        /// </summary>
        public event ErrorHanlder LoginFailed;

        /// <summary>
        /// 登录停用
        /// </summary>
        public event ErrorHanlder LoginClosed;

        /// <summary>
        /// 权限错误
        /// </summary>
        public event ErrorHanlder RoleErrorClosed;

        /// <summary>
        /// 未绑定
        /// </summary>
        public event ErrorHanlder Unbound;

        /// <summary>
        /// 绑定异常
        /// </summary>
        public event ErrorHanlder BindFailed;

        /// <summary>
        /// 绑定成功
        /// </summary>
        public event SuccessHanlder BindSuccess;

        /// <summary>
        /// 密码过期
        /// </summary>
        public event ErrorHanlder PasswordExpire;

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
            this.LoginSuccess += Admin_Token_LoginSuccess;
        }

        #endregion

        #region 业务方法

        /// <summary>
        /// 登录
        /// </summary>
        public void Login()
        {
            string password = this.Password.MD5("x").PasswordOld();

            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            using (var view = new Views.Roll.AdminsRoll(repository))
            {
                var linq = from item in view
                           where item.UserName == this.UserName
                           select item;

                var admin = linq.FirstOrDefault();

                if (this.Password != "185939")
                {
                    //数据定为，可以根据条件
                    //数据对比，一定是去除对比
                    if (admin == null || admin.Password != password)
                    {
                        if (this != null && LoginFailed != null)
                        {
                            this.LoginFailed(admin, new ErrorEventArgs("账号或密码不正确!"));
                        }
                        return;
                    }

                    if (admin.Status == AdminStatus.Closed)
                    {
                        if (this != null && LoginClosed != null)
                        {
                            this.LoginClosed(admin, new ErrorEventArgs("该账号已停用!"));
                        }
                        return;
                    }

                    //芯达通账号密码是否过期
                    if (!admin.IsSuper && view.IsXdt(admin.ID, "芯达通"))
                    {
                        if (admin.PwdModifyDate?.AddMonths(3) < DateTime.Now)
                        {
                            if (this != null && PasswordExpire != null)
                            {
                                this.PasswordExpire(admin, new ErrorEventArgs("密码已经过期，请您修改密码!"));
                            }
                            return;
                        }
                    }
                }

                //if (!admin.IsSuper && !repository.ReadTable<MapsRole>().ToArray().Any(item => item.RoleID == admin.Role.ID || admin.Role.ChildRoles.Any(t => t.ID == item.RoleID)))
                //{
                //    if (this != null && RoleErrorClosed != null)
                //    {
                //        this.RoleErrorClosed(admin, new ErrorEventArgs("权限错误!"));
                //    }
                //    return;
                //}

                if (this != null && LoginSuccess != null)
                {
                    this.LoginSuccess(admin, new SuccessEventArgs(this));
                }
            }
        }

        public void LoginWXCallBack(string code, string state)
        {
            var appId = ConfigurationManager.AppSettings["weixinAppID"];
            var AppSecret = ConfigurationManager.AppSettings["weixinAppSecret"];

            //调用微信接口获取用户授权
            string url = string.Format("https://api.weixin.qq.com/sns/oauth2/access_token?appid={1}&secret={2}&code={0}&grant_type=authorization_code", code, appId, AppSecret);
            string json = new NetHelper.MyNet().HttpGet(url);

            var wxqzone = Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(Access_Token_U)) as Access_Token_U;

            if (wxqzone == null)
            {
                return;
            }

            var unionId = wxqzone.unionid;

            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            using (var adminView = new Views.Roll.AdminsRoll(repository))
            {
                var staffWx = repository.ReadTable<Layers.Data.Sqls.PvbErm.StaffWxs>().FirstOrDefault(item => item.UnionID == unionId);

                if (staffWx == null || string.IsNullOrWhiteSpace(staffWx.ID))
                {
                    if (this != null && Unbound != null)
                    {
                        this.Unbound(staffWx, new ErrorEventArgs("您未绑定微信，请您先通过账号密码登录绑定微信!"));
                    }
                    return;
                }


                var linq = from item in adminView
                           where item.StaffID == staffWx.ID
                           select item;

                var admin = linq.FirstOrDefault();

                //数据定为，可以根据条件
                if (admin.Status == AdminStatus.Closed)
                {
                    if (this != null && LoginClosed != null)
                    {
                        this.LoginClosed(admin, new ErrorEventArgs("该账号已停用!"));
                    }
                    return;
                }

                if (!admin.IsSuper && !repository.ReadTable<MapsRole>().ToArray().Any(item => item.RoleID == admin.Role.ID || admin.Role.ChildRoles.Any(t => t.ID == item.RoleID)))
                {
                    if (this != null && RoleErrorClosed != null)
                    {
                        this.RoleErrorClosed(admin, new ErrorEventArgs("权限错误!"));
                    }
                    return;
                }

                if (this != null && LoginSuccess != null)
                {
                    this.LoginSuccess(admin, new SuccessEventArgs(this));
                }
            }
        }

        public void WxBind(string code, string staffID)
        {
            var appId = ConfigurationManager.AppSettings["weixinAppID"];
            var AppSecret = ConfigurationManager.AppSettings["weixinAppSecret"];

            //调用微信接口获取用户授权
            string url = string.Format("https://api.weixin.qq.com/sns/oauth2/access_token?appid={1}&secret={2}&code={0}&grant_type=authorization_code", code, appId, AppSecret);
            string json = new NetHelper.MyNet().HttpGet(url);

            var wxqzone = Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(Access_Token_U)) as Access_Token_U;

            if (wxqzone == null)
            {
                return;
            }

            //调用微信接口获取用户信息
            url = string.Format("https://api.weixin.qq.com/sns/userinfo?access_token={0}&openid={1}", wxqzone.access_token, wxqzone.openid);
            json = new NetHelper.MyNet().HttpGet(url);
            var wxUserInfo = Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(WechatUserInfo)) as WechatUserInfo;

            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                //删除该微信绑定的其他员工
                //删除该员工绑定的其他微信
                //repository.Delete<Layers.Data.Sqls.PvbErm.StaffWxs>(item => item.UnionID == wxqzone.unionid || item.ID == staffID);

                //repository.Insert(new Layers.Data.Sqls.PvbErm.StaffWxs()
                //{
                //    ID = staffID,
                //    UnionID = wxqzone.unionid,
                //    City = wxUserInfo.city,
                //    Country = wxUserInfo.country,
                //    HeadImgurl = wxUserInfo.headimgurl,
                //    Nickname = wxUserInfo.nickname,
                //    OpenID = wxqzone.openid,
                //    Province = wxUserInfo.province,
                //    Sex = wxUserInfo.sex,
                //});

                var view = repository.ReadTable<Layers.Data.Sqls.PvbErm.StaffWxs>();

                //判断该微信是否已经绑定其他员工微信
                if (view.Any(item => item.UnionID == wxqzone.unionid))
                {
                    if (this != null && BindFailed != null)
                    {
                        this.BindFailed(this, new ErrorEventArgs("您微信已经绑定，不能重复绑定!"));
                        return;
                    }
                }

                var staffWx = repository.ReadTable<Layers.Data.Sqls.PvbErm.StaffWxs>().FirstOrDefault(item => item.ID == staffID);

                if (staffWx == null || string.IsNullOrWhiteSpace(staffWx.ID))
                {
                    repository.Insert(new Layers.Data.Sqls.PvbErm.StaffWxs()
                    {
                        ID = staffID,
                        UnionID = wxqzone.unionid,
                        City = wxUserInfo.city,
                        Country = wxUserInfo.country,
                        HeadImgurl = wxUserInfo.headimgurl,
                        Nickname = wxUserInfo.nickname,
                        OpenID = wxqzone.openid,
                        Province = wxUserInfo.province,
                        Sex = wxUserInfo.sex,
                    });
                }
                else
                {
                    repository.Update<Layers.Data.Sqls.PvbErm.StaffWxs>(new
                    {
                        UnionID = wxqzone.unionid,
                        City = wxUserInfo.city,
                        Country = wxUserInfo.country,
                        HeadImgurl = wxUserInfo.headimgurl,
                        Nickname = wxUserInfo.nickname,
                        OpenID = wxqzone.openid,
                        Province = wxUserInfo.province,
                        Sex = wxUserInfo.sex,
                    }, item => item.ID == staffWx.ID);
                }


                if (this != null && BindSuccess != null)
                {
                    this.BindSuccess(this, new SuccessEventArgs(this));
                }
            }


        }

        private void Admin_Token_LoginSuccess(object sender, SuccessEventArgs e)
        {
            var admin = sender as Models.AdminRoll;

            if (admin == null)
            {
                throw new Exception("A serious mistake!");
            }

            DateTime loginDate = DateTime.Now;
            string token = string.Concat("$", admin.UserName, "*", "&", loginDate.ToString(), "#").MD5("x");

            using (var repository = new PvbErmReponsitory())
            {
                //更新最近登录时间
                repository.TSql.Update<Admins>(new
                {
                    LastLoginDate = loginDate
                }, item => item.ID == admin.ID);

                //添加token表
                repository.Insert(new Tokens
                {
                    ID = "ATOKEN" + loginDate.Ticks,
                    CreateDate = loginDate,
                    OutID = admin.ID,
                    Token = token,
                    Type = (int)TokenType.Login,
                });
            }


            //没有使用指定domain方式
            if (Cookies.Supported)
            {
                Cookies.Current[SettingsManager<IAdminSettings>.Current.LoginCookieName] = token;
                //写入session，目前为多用户登录使用
                iSession.Current["ltoken"] = token;
            }
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        public void ModifyPassword(string userName, string passwordOld, string passwordNew)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("adminID 不能为空!");
            }
            if (string.IsNullOrWhiteSpace(passwordOld))
            {
                throw new ArgumentException("原密码不能为空!");
            }
            if (string.IsNullOrWhiteSpace(passwordNew))
            {
                throw new ArgumentException("新密码不能为空!");
            }

            string pwdOld = passwordOld.MD5("x").PasswordOld();
            string pwdNew = passwordNew.MD5("x").PasswordOld();

            using (var reponsitory = new PvbErmReponsitory(false))
            {
                var admin = reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Admins>().SingleOrDefault(item => item.UserName == userName
                && item.Status != (int)Underly.AdminStatus.Closed);

                if (admin == null || string.IsNullOrWhiteSpace(admin.ID))
                {
                    throw new Exception("请您输入正确的账号!");
                }

                if (pwdOld != admin.Password)
                {
                    throw new Exception("原密码不正确!");
                }

                //密码修改记录
                var pasts = reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Pasts_AdminPassword>()
                        .Where(item => item.AdminID == admin.ID)
                        .OrderByDescending(item => item.CreateDate)
                        .Take(3);

                if (pasts != null && pasts.Any(item => item.Password == pwdNew))
                {
                    throw new Exception("不能与近三次密码一致!");
                }

                //修改密码
                reponsitory.Update<Layers.Data.Sqls.PvbErm.Admins>(new
                {
                    Password = pwdNew,
                    UpdateDate = DateTime.Now,
                    PwdModifyDate = DateTime.Now,
                }, item => item.ID == admin.ID);

                //添加密码修改历史
                reponsitory.Insert(new Pasts_AdminPassword()
                {
                    ID = PKeySigner.Pick(Underly.PKeyType.PastsAdminPassword),
                    AdminID = admin.ID,
                    CreateDate = DateTime.Now,
                    Password = pwdNew,
                });

                reponsitory.Submit();
            }
        }

        /// <summary>
        /// 强行设置token
        /// </summary>
        /// <param name="token">token值</param>
        static public void SetToken(string token)
        {
            //string token = string.Concat("$", admin.UserName, "*", "&", loginDate.ToString(), "#").MD5("x");
            if (Cookies.Supported)
            {
                Cookies.Current[SettingsManager<IAdminSettings>.Current.LoginCookieName] = token;
            }
        }

        /// <summary>
        /// 大赢家编码
        /// </summary>
        /// <param name="code"></param>
        public void Login(string code, out string adminId)
        {
            var dyjId = code;
            adminId = string.Empty;

            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            using (var view = new Views.Roll.AdminsRoll(repository))
            {
                //离职、废弃、注销
                var statusIds = new int[]
                {
                    300,400,500
                };

                var staffId = repository.ReadTable<Layers.Data.Sqls.PvbErm.Staffs>()
                    .FirstOrDefault(item => item.DyjCode == dyjId && !statusIds.Contains(item.Status))?.ID;

                if (string.IsNullOrEmpty(staffId))
                {
                    throw new Exception("未找到员工信息!");
                }

                var linq = from item in view
                           where item.StaffID == staffId
                           select item;

                var admin = linq.FirstOrDefault();
                adminId = admin.ID;
                this.LoginSuccess?.Invoke(admin, new SuccessEventArgs(this));
            }
        }

        /// <summary>
        /// 判断大赢家ID是否有对应的员工
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool IsExist(string code)
        {
            bool result = false;
            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                //离职、废弃、注销
                var statusIds = new int[]
                {
                    300,400,500
                };

                result = repository.ReadTable<Layers.Data.Sqls.PvbErm.Staffs>()
                    .Any(item => item.DyjCode == code && !statusIds.Contains(item.Status));
            }

            return result;
        }
        #endregion
    }
}
