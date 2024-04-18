using Needs.Erp.Generic;
using Needs.Settings;
using Needs.Utils.Converters;
using Needs.Utils.Http;
using System;
using System.Linq;

namespace Needs.Erp.Generic
{
    /// <summary>
    /// 不能任何被调用
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Startor<T> where T : class
    {
        static object locker = new object();
        static Startor()
        {
            lock (locker)
            {
                string superid = SettingsManager<IAdminSettings>.Current.SuperID;
                string superRoleId = SettingsManager<IAdminSettings>.Current.SuperRoleID;
                using (var reponsitory = new Layer.Data.Sqls.BvnErpReponsitory())
                {
                    int count = reponsitory.ReadTable<Layer.Data.Sqls.BvnErp.Admins>().Count(item => item.ID == superid);
                    if (count == 0)
                    {
                        reponsitory.Insert(new Layer.Data.Sqls.BvnErp.Admins
                        {
                            ID = superid,
                            UserName = "sa",
#if true
                            Password = "sa".MD5("x").PasswordOld(),
#else
                                Password = "sa".MD5("x").Password(),
#endif

                            RealName = "超级管理员",
                            CreateDate = DateTime.Now,
                            UpdateDate = DateTime.Now,
                            Status = (int)Status.Normal,
                            Summary = "System default superadministrator",
                            LoginDate = null
                        });
                    }
                    // 超级权限
                    int cnt = reponsitory.ReadTable<Layer.Data.Sqls.BvnErp.Roles>().Count(item => item.ID == superRoleId);
                    if (cnt == 0)
                    {
                        reponsitory.Insert(new Layer.Data.Sqls.BvnErp.Roles
                        {
                            ID = superRoleId,
                            Name = "超级权限",
                            Status = (int)RoleStatus.Super,
                            Summary = "超级权限不可更改删除",
                            CreateDate = DateTime.Now,
                            UpdateDate = DateTime.Now
                        });
                    }
                }
            }
        }

        static protected string Token
        {
            get { return Cookies.Current[SettingsManager<IAdminSettings>.Current.LoginCookieName]; }
        }

        static protected T GetAdmin(string token)
        {
            using (var view = new Views.AdminsToken(token))
            {
                return view.SingleOrDefault() as T;
            }
        }

        static public T Current
        {
            get
            {
                if (Cookies.Supported)
                {
                    string token = Token;
                    if (string.IsNullOrWhiteSpace(token))
                    {
                        return null;
                    }

                    return GetAdmin(token);
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
