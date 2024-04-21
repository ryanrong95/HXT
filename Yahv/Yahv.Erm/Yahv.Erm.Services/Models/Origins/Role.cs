using System;
using System.Linq;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbErm;
using Layers.Linq;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Usually;
using Yahv.Utils.Converters.Contents;

namespace Yahv.Erm.Services.Models.Origins
{
    /// <summary>
    /// 角色
    /// </summary>
    /// <remarks>建议使用 RoleRoll 方式开发</remarks>
    public class Role : IUnique
    {
        #region 事件

        /// <summary>
        /// EnterSuccess
        /// </summary>
        public event SuccessHanlder EnterSuccess;

        /// <summary>
        /// EnterError
        /// </summary>
        public event ErrorHanlder EnterError;

        /// <summary>
        /// AbandonSuccess
        /// </summary>
        public event SuccessHanlder AbandonSuccess;

        #endregion

        #region 属性
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 角色类型
        /// </summary>
        public RoleType Type { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 状态：正常、删除、超级管理员角色（在后台开发要特殊避免的）、固定的Fixed
        /// </summary>
        public RoleStatus Status { get; set; }
        #endregion

        #region 持久化
        /// <summary>
        /// 添加
        /// </summary>
        public void Enter()
        {
            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                if (string.IsNullOrWhiteSpace(this.ID) || this.ID == "undefined")
                {
                    //判断角色名称是否存在
                    if (repository.ReadTable<Roles>().Any(a => a.Name == this.Name))
                    {
                        if (EnterError != null)
                            this.EnterError(this, new ErrorEventArgs("角色名称不能重复!"));
                        return;
                    }

                    string id = PKeySigner.Pick(PKeyType.Role);
                    repository.Insert(new Roles()
                    {
                        ID = id,
                        Status = (int)RoleStatus.Normal,
                        Name = this.Name,
                        CreateDate = DateTime.Now,
                        Type = (int)this.Type,
                    });
                    this.ID = id;
                }

                if (this != null && this.EnterSuccess != null)
                {
                    this.EnterSuccess(this, new SuccessEventArgs(this));
                }
            }
        }

        /// <summary>
        /// 废弃
        /// </summary>
        public void Abandon()
        {
            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                repository.Update<Roles>(new
                {
                    Status = RoleStatus.Delete
                }, r => r.ID == this.ID);

                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this));
                }
            }
        }

        #endregion

        static object locker = new object();
        public void Map(params string[] arry)
        {
            using (PvbErmReponsitory repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                repository.Delete<MapsRole>(item => item.RoleID == this.ID);
                var waiting = repository.ReadTable<MapsRole>().Any(item => item.RoleID == this.ID);

                foreach (var item in arry.Distinct())
                {
                    lock (locker)
                    {
                        repository.Insert(new MapsRole
                        {
                            MenuID = item,
                            RoleID = this.ID
                        });
                    }
                }
            }
        }

        public void Setting(string url, string context)
        {
            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                var code = url.ToLower().MD5();
                if (repository.ReadTable<ParticleSettings>().Any(item => item.RoleID == this.ID
                    && item.UrlCode == code))
                {
                    repository.Update(new ParticleSettings
                    {
                        RoleID = this.ID,
                        Context = context,
                        Type = "default",
                        Url = url,
                        UrlCode = code
                    }, item => item.RoleID == this.ID && item.UrlCode == code);
                }
                else
                {
                    repository.Insert(new ParticleSettings
                    {
                        RoleID = this.ID,
                        Context = context,
                        Type = "default",
                        Url = url,
                        UrlCode = url.ToLower().MD5()
                    });
                }
            }
        }

        public Services.Views.ParticleSettingsRoll Settings
        {
            get { return new Views.ParticleSettingsRoll(this); }
        }
    }
}