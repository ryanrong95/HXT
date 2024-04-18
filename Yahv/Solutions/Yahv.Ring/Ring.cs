using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Models;
using Yahv.Services.Json;
using Yahv.Settings;
using Yahv.Utils.Http;
using Yahv.Utils.Serializers;

namespace Yahv
{
    /// <summary>
    /// 
    /// </summary>
    public class Ring
    {
        RingAdmin admin;
        static Ring current;
        static object locker = new object();

        /// <summary>
        /// 当前登入Erp管理员
        /// </summary>
        static public RingAdmin Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new Ring();
                        }
                    }
                }

                if (current.admin == null)
                {
                    lock (locker)
                    {
                        if (current.admin == null)
                        {
                            //获取文件中的 本地 cookie
                            //获取admin
                            //cookies

                            string txt = File.ReadAllText(RingAdmin.CookiePath, Encoding.UTF8);
                            var settrings = JObject.Parse(txt);
                            string loginCookieName = settrings["cookies"][SettingsManager<IAdminSettings>.Current.LoginCookieName].Value<string>();

                            string url = $"{Yahv.Underly.DomainConfig.ErmApi}Admins/Token/{loginCookieName}";
                            var json = ApiHelper.Current[Agent.Warehouse].Get<AdminJson>(url);

                            current.admin = new RingAdmin
                            {
                                ID = json.ID,
                                RealName = json.RealName,
                                Role = new RingRole
                                {
                                    ID = json.Role.ID,
                                    Name = json.Role.Name,
                                    Status = json.Role.Status
                                },
                                Status = json.Status,
                                UserName = json.UserName,
                            };

                        }
                    }
                }

                return current.admin;
            }
        }

        /// <summary>
        /// 退出当前登入Erp管理员
        /// </summary>
        static public void Logout()
        {
            //删除本地的文件
            lock (locker)
            {
                if (File.Exists(RingAdmin.CookiePath))
                {
                    File.Delete(RingAdmin.CookiePath);
                }
            }
        }
        /// <summary>
        /// 写模拟Cookie
        /// </summary>
        /// <param name="context">数据内容</param>
        static public void Cookie(object context)
        {

            lock (locker)
            {
                var jObject = new JObject();

                var cookie = jObject["cookies"] = new JObject();

                foreach (var item in context.ToString().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(item =>
                {
                    return item.Trim().Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                }))
                {
                    cookie[item[0]] = item[1];
                }

                File.WriteAllText(RingAdmin.CookiePath, jObject.ToString(), Encoding.UTF8);
            }
        }
    }
}
