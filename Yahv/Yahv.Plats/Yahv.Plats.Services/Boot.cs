using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbErm;
using Layers.Linq;

using Yahv.Utils.Converters.Contents;
using Yahv.Settings;
using Yahv.Underly;
using Newtonsoft.Json.Linq;
using System.Threading;
using Yahv.Usually;

namespace Yahv.Plats.Services
{
    /// <summary>
    /// 单例实现
    /// </summary>
    public class Boot
    {
        /// <summary>
        /// EnterSuccess
        /// </summary>
        public event SuccessHanlder EnterSuccess;

        /// <summary>
        /// EnterSuccess
        /// </summary>
        public event ErrorHanlder EnterError;

        /// <summary>
        /// 初始化超级权限
        /// </summary>
        void SupperRole()
        {
            string superRoleID = SettingsManager<IAdminSettings>.Current.SuperRoleID;
            string superRoleName = SettingsManager<IAdminSettings>.Current.SuperRoleName;

            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                //判断 超级角色 权限是否存在于 数据库
                var isExist = repository.ReadTable<Roles>().Where(item => item.ID == superRoleID
                    && item.Name == superRoleName);

                //不存在添加 超级角色
                if (!isExist.Any())
                {
                    repository.Insert(new Roles()
                    {
                        ID = superRoleID,
                        Name = superRoleName,
                        Status = (int)RoleStatus.Super,
                        CreateDate = DateTime.Now,
                    });
                }

                // 添加固定角色
                var arry = ExtendsEnum.ToArray<FixedRole>();
                foreach (var item in arry)
                {
                    var name = item.GetDescription();
                    var id = item.GetFixedID();
                    if (!repository.ReadTable<Roles>().Any(t => t.ID == id))
                    {
                        repository.Insert(new Roles
                        {
                            ID = id,
                            Name = name,
                            Status = (int)RoleStatus.Fixed,
                            CreateDate = DateTime.Now,
                            Type = (int)RoleType.Customer,
                        });
                    }
                }

                this.EnterSuccess?.Invoke($"固定角色初始化成功!", new SuccessEventArgs(this));
            }
        }

        /// <summary>
        /// 初始化超级管理员
        /// </summary>
        void SupperAdmin()
        {


            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                //判断 超级管理员 权限是否存在于 数据库
                if (true)
                {
                    string superRoleID = SettingsManager<IAdminSettings>.Current.SuperRoleID;
                    string superID = SettingsManager<IAdminSettings>.Current.SuperID;
                    string superName = SettingsManager<IAdminSettings>.Current.SuperName;
                    string superRealName = SettingsManager<IAdminSettings>.Current.SuperRealName;

                    var isExist = repository.ReadTable<Admins>().Where(item => item.ID == superID
                        && item.UserName == superName);

                    if (!isExist.Any())
                    {
                        //不存在添加 超级管理员
                        repository.Insert(new Admins()
                        {
                            ID = superID,
                            UserName = superName,

                            Password = "sa".MD5("x").PasswordOld(),

                            RealName = superRealName,
                            RoleID = superRoleID,
                            SelCode = superID,
                            CreateDate = DateTime.Now,
                            Status = (int)AdminStatus.Super,
                        });
                    }
                }


                foreach (var npc in ExtendsEnum.ToArray<Npc>())
                {
                    string name = npc.Obtain();
                    string realName = npc.GetDescription();

                    var isExist = repository.ReadTable<Admins>().Where(item => item.ID == name
                         && item.UserName == name);

                    if (!isExist.Any())
                    {
                        repository.Insert(new Admins
                        {
                            ID = name,
                            Password = "****",
                            RealName = realName,
                            Status = (int)AdminStatus.Npc,
                            UserName = name,
                            SelCode = name,
                            RoleID = FixedRole.Npc.GetFixedID(),
                            CreateDate = DateTime.Now,
                            UpdateDate = DateTime.Now
                        });
                    }
                }

                this.EnterSuccess?.Invoke($"超级管理员初始化成功!", new SuccessEventArgs(this));
            }
        }


        List<string> menuEnters = new List<string>();

        void LeftEnter(JObject jobject)
        {
            using (var repository = new PvbErmReponsitory())
            using (var tran = repository.OpenTransaction())
            {
                var menus = repository.ReadTable<Menus>().ToArray();

                Action<Menus> enter = (menu) =>
                {
                    var linq = menus.Where(item => (item.FatherID == null || item.FatherID == menu.FatherID)
                        && item.Name == menu.Name);

                    var entity = linq.FirstOrDefault();

                    if (entity == null)
                    {
                        menu.ID = PKeySigner.Pick(PKeyType.Menu);
                        repository.Insert(menu);
                    }
                    else
                    {
                        menu.ID = entity.ID;
                        if (!CompareMenu(menu, entity))
                        {
                            repository.Update(menu, item => item.ID == entity.ID);
                        }
                    }

                    if (!menuEnters.Contains(menu.ID))
                    {
                        this.menuEnters.Add(menu.ID);
                    }
                };
                var lefts = jobject;
                #region enters

                var business = new Menus
                {
                    FatherID = null,
                    OrderIndex = lefts["OrderIndex"]?.Value<int>(),
                    Name = lefts["Name"].Value<string>(),
                    IconUrl = lefts["IconUrl"].Value<string>(),
                    LogoUrl = lefts["LogoUrl"].Value<string>(),
                    FirstUrl = lefts["FirstUrl"].Value<string>(),
                    Company = (lefts["Company"] ?? JValue.CreateNull()).Value<string>(),
                    RightUrl = null,
                    HelpUrl = (lefts["HelpUrl"] ?? JValue.CreateNull()).Value<string>(),
                    Status = (int)Status.Normal,
                    IsLocal = true
                };

                enter(business);

                if (lefts["Menu"].HasValues && lefts["Menu"] is JArray)
                {
                    foreach (var menu in lefts["Menu"] as JArray)
                    {
                        var _menu = new Menus
                        {
                            FatherID = business.ID,
                            OrderIndex = menu["orderIndex"]?.Value<int>(),
                            Name = menu["text"].Value<string>(),
                            IconUrl = null,
                            LogoUrl = null,
                            FirstUrl = null,
                            RightUrl = menu["url"]?.Value<string>(),
                            Status = (int)Status.Normal,
                            IsLocal = menu["isLocal"]?.Value<bool>() ?? true,
                        };
                        enter(_menu);
                        if (menu["children"] != null && menu["children"].HasValues && menu["children"] is JArray)
                        {
                            foreach (var child in menu["children"] as JArray)
                            {
                                var _child = new Menus
                                {
                                    FatherID = _menu.ID,
                                    OrderIndex = child["orderIndex"]?.Value<int>(),
                                    Name = child["text"].Value<string>(),
                                    IconUrl = null,
                                    LogoUrl = null,
                                    FirstUrl = null,
                                    RightUrl = child["url"].Value<string>(),
                                    Status = (int)Status.Normal,
                                    IsLocal = child["isLocal"]?.Value<bool>() ?? true,
                                };
                                enter(_child);
                                if (child["Particles"] != null && child["Particles"].HasValues && child["Particles"] is JArray)
                                {
                                    foreach (var particle in child["Particles"] as JArray)
                                    {
                                        var _particle = new Menus
                                        {
                                            FatherID = _child.ID,
                                            OrderIndex = particle["orderIndex"]?.Value<int>(),
                                            Name = particle["text"].Value<string>(),
                                            IconUrl = null,
                                            LogoUrl = null,
                                            FirstUrl = null,
                                            RightUrl = particle["url"].Value<string>(),
                                            Status = (int)Status.Normal,
                                            IsLocal = particle["isLocal"]?.Value<bool>() ?? true,
                                        };
                                        enter(_particle);
                                    }
                                }
                            }
                        }
                    }
                }

                #endregion
                tran.Commit();
            }
        }

        void LeftBusiness()
        {
            menuEnters = new List<string>();
            var url = $"{Yahv.Underly.DomainConfig.Fixed}/Yahv/leftbusiness/";
            //string[] lefts = { "erm.json", "rfq.json", "crm.json", "srm.json", "pfwms.json", "pvdata.json", "cxhy.json", "xdt.json", "xdterm.json", "xdtchains.json", "pvfinance.json", "logistics.json", "szpswms.json", "crmplus.json", "tradesales.json", "standard.json", "ims.json", "pvfcrm.json" };
            string[] lefts = { "xdt-client.json", "xdt-business.json", "xdt-customs.json", "xdt-finance.json", "pvfinance.json", "xdt-risk.json", "xdt-general.json", "xdterm.json", "erm.json" };

            using (WebClient webClient = new WebClient())
            {
                webClient.Encoding = Encoding.UTF8;
                foreach (string name in lefts)
                {
                    try
                    {
                        string json = webClient.DownloadString(url + name);
                        LeftEnter(JObject.Parse(json));

                        this.EnterSuccess?.Invoke($"菜单[{name}],初始化成功!", new SuccessEventArgs(name));
                    }
                    catch (Exception ex)
                    {
                        this.EnterError?.Invoke($"菜单[{name}],初始化失败!!!{ex.Message}", new ErrorEventArgs(name));
                    }
                }
            }

            //更新 已删除菜单的状态
            using (var repository = new PvbErmReponsitory())
            {
                var arry = repository.ReadTable<Menus>().Select(item => item.ID).ToArray();
                var excepts = arry.Except(menuEnters).ToArray();

                repository.Update<Menus>(new
                {
                    Status = (int)Status.Delete
                }, item => excepts.Contains(item.ID));
            }
        }

        /// <summary>
        /// 比较菜单
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <returns></returns>
        private bool CompareMenu(Menus m1, Menus m2)
        {
            if (m1 == null || m2 == null)
            {
                return false;
            }

            if (m1.ID != m2.ID)
            {
                return false;
            }
            if (m1.FatherID != m2.FatherID)
            {
                return false;
            }
            if (m1.Name != m2.Name)
            {
                return false;
            }
            if (m1.Company != m2.Company)
            {
                return false;
            }
            if (m1.RightUrl != m2.RightUrl)
            {
                return false;
            }
            if (m1.IconUrl != m2.IconUrl)
            {
                return false;
            }
            if (m1.FirstUrl != m2.FirstUrl)
            {
                return false;
            }

            if (m1.LogoUrl != m2.LogoUrl)
            {
                return false;
            }
            if (m1.HelpUrl != m2.HelpUrl)
            {
                return false;
            }
            if (m1.OrderIndex != m2.OrderIndex)
            {
                return false;
            }
            if (m1.Status != m2.Status)
            {
                return false;
            }
            if (m1.IsLocal != m2.IsLocal)
            {
                return false;
            }

            return true;
        }

        public void Execute()
        {
            lock (locker)
            {
                this.SupperRole();
                this.SupperAdmin();
                this.LeftBusiness();


                try
                {
                    Boots.InitialEnum.Execute();
                }
                catch (Exception)
                {
                }

            }
        }


        static object locker = new object();
        private static Boot current;
        static public Boot Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new Boot();
                        }
                    }
                }

                return current;
            }
        }
    }
}
