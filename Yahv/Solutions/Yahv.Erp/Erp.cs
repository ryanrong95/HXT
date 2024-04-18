using Yahv.Models;
using Yahv.Settings;
using Yahv.Utils.Http;
using System.Linq;
using System.Threading;
using Yahv.Underly;
using System.Collections.Generic;
using Yahv.Services.Json;
using System;

namespace Yahv
{
    /// <summary>
    /// 入口基类
    /// </summary>
    abstract public class Portal
    {
        /// <summary>
        /// 通过 Token 获取 Admin 数据
        /// </summary>
        /// <returns>Admin 实例</returns>
        protected ErpAdmin GetByToken(string token)
        {
            using (var view = new Views.AdminsToken(token))
            {
                var entity = view.SingleOrDefault();
                return entity;
            }
        }

        /// <summary>
        /// 通过 ID 获取 Admin 数据
        /// </summary>
        /// <returns>Admin 实例</returns>
        protected ErpAdmin GetByID(string id)
        {
            using (var view = new Views.AdminsAlls())
            {
                var entity = view[id];
                return entity;
            }
        }

        /// <summary>
        /// 通过 ID 获取 Admin 数据
        /// </summary>
        /// <returns>Admin 实例</returns>
        protected ErpAdmin[] GetByID(params string[] arry)
        {
            using (var view = new Views.AdminsAlls())
            {
                return view.Where(item => arry.Contains(item.ID)).ToArray();
            }
        }
    }

    /// <summary>
    /// 调用起始
    /// </summary>
    public sealed class Erp : Portal
    {
        Erp()
        {
        }

        static Erp current;
        static object locker = new object();

        /// <summary>
        /// 当前登入Erp管理员
        /// </summary>
        static public ErpAdmin Current
        {
            get
            {
                //经过测试，感觉还是session效率高，随时接受王亚的要求。

                if (!Cookies.Supported)
                {
                    return null;
                }

                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new Erp();
                        }
                    }
                }

                //这里写的不错，利用单例开发了示例！
                //简单工厂+单例这个就是经典！

                string token = Cookies.Current[SettingsManager<IAdminSettings>.Current.LoginCookieName];
                if (string.IsNullOrWhiteSpace(token))
                {
                    return null;
                }

                var admin = iSession.Current[token] as ErpAdmin;

                if (admin == null)
                {
                    lock (locker)
                    {
                        if (admin == null)
                        {
                            admin = current.GetByToken(token);
                            iSession.Current[token] = admin;

                        }
                    }
                }

                return admin;
            }
        }

        /// <summary>
        /// 退出当前登入Erp管理员
        /// </summary>
        static public void Logout()
        {
            string token = Cookies.Current[SettingsManager<IAdminSettings>.Current.LoginCookieName];
            iSession.Current[token] = null;
            Cookies.Current[SettingsManager<IAdminSettings>.Current.LoginCookieName] = "";
        }

        /// <summary>
        /// 选择指定的 Erp管理员
        /// </summary>
        static public Choices Choice
        {
            get { return Choices.Current; }
        }

        /// <summary>
        /// 根据Token获取ErpAdmin
        /// </summary>
        /// <param name="Token">Token</param>
        /// <returns>ErpAdmin</returns>
        static public AdminJson Token(string Token)
        {
            var admin = new Erp().GetByToken(Token);

            return new AdminJson
            {
                ID = admin.ID,
                Role = new RoleJson
                {
                    ID = admin.Role.ID,
                    Name = admin.Role.Name,
                    Status = ((Role)admin.Role).Status,
                },
                RealName = admin.RealName,
                UserName = admin.UserName,
                Status = admin.Status
            };
        }
    }

    /// <summary>
    /// Erp管理员选择器
    /// </summary>
    public class Choices : Portal
    {
        /// <summary>
        /// 选择索引
        /// </summary>
        /// <param name="index">AdminID</param>
        /// <returns>Erp管理员</returns>
        public ErpAdmin this[string index]
        {
            get
            {
                string name = nameof(Choices) + index;

                var admin = Thread.GetData(Thread.GetNamedDataSlot(name)) as ErpAdmin;

                if (admin == null)
                {
                    admin = this.GetByID(index);
                    Thread.SetData(Thread.GetNamedDataSlot(name), admin);
                }

                return admin;
            }
        }

        /// <summary>
        /// 选择索引
        /// </summary>
        /// <param name="index">AdminID</param>
        /// <returns>Erp管理员</returns>
        /// <remarks>先实现，优化留给王亚组</remarks>
        public ErpAdmin[] this[params string[] arry]
        {

            //写法太啰嗦

            get
            {
                List<ErpAdmin> admins = new List<ErpAdmin>();
                List<string> list = new List<string>();
                foreach (var index in arry)
                {
                    string name = nameof(Choices) + index;
                    var admin = Thread.GetData(Thread.GetNamedDataSlot(name)) as ErpAdmin;
                    if (admin == null)
                    {
                        list.Add(index);
                    }
                    else
                    {
                        admins.Add(admin);
                    }
                }
                if (list.Count > 0)
                {
                    var news = this.GetByID(list.ToArray());
                    foreach (var item in news)
                    {
                        string name = nameof(Choices) + item.ID;
                        Thread.SetData(Thread.GetNamedDataSlot(name), item);
                    }
                    admins.AddRange(news);
                }

                return admins.ToArray();
            }
        }

        /// <summary>
        /// 选择索引
        /// </summary>
        /// <param name="index">AdminID</param>
        /// <returns>Erp管理员</returns>
        public ErpAdmin this[Npc index]
        {
            get
            {
                string name = index.Obtain();

                var admin = Thread.GetData(Thread.GetNamedDataSlot(name)) as ErpAdmin;

                if (admin == null)
                {
                    lock (locker)
                    {
                        if (admin == null)
                        {
                            admin = this.GetByID(name);
                            Thread.SetData(Thread.GetNamedDataSlot(name), admin);
                        }
                    }
                }
                return admin;
            }
        }

        /// <summary>
        /// 选择索引
        /// </summary>
        /// <param name="index">AdminID</param>
        /// <returns>Erp管理员</returns>
        [Obsolete("建议废弃，这里不应该提供类似方法")]
        public ErpAdmin[] this[params FixedRole[] indexs]
        {
            get
            {
                using (var view = new Views.AdminsAlls())
                {
                    return view.Where(item => indexs.Select(t => t.GetFixedID()).Contains(item.Role.ID)).ToArray();
                }
            }
        }


        static Choices choices;
        static object locker = new object();

        /// <summary>
        /// 选择器当前示例
        /// </summary>
        static internal Choices Current
        {
            get
            {
                if (choices == null)
                {
                    lock (locker)
                    {
                        if (choices == null)
                        {
                            choices = new Choices();
                        }
                    }
                }
                return choices;
            }
        }

        /// <summary>
        /// 根据角色ID获取管理员
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <returns></returns>
        [Obsolete("建议废弃，用通用视图的方式开发")]
        public ErpAdmin[] GetByRoleID(params string[] ids)
        {
            using (var view = new Views.AdminsAlls())
            {
                return view.Where(item => ids.Contains(item.Role.ID)).ToArray();
            }
        }
        /// <summary>
        /// 根据角色名称获取管理员
        /// </summary>
        /// <param name="roleId">角色名称</param>
        /// <returns></returns>
        [Obsolete("建议废弃，用通用视图的方式开发")]
        public ErpAdmin[] GetByRoleName(string roleName)
        {
            using (var view = new Views.AdminsAlls())
            {
                return view.Where(item => item.Role.Name == roleName).ToArray();
            }
        }
    }
}
