using Layers.Data.Sqls;
using Yahv.Linq;
using System.Linq;
using Yahv.Utils.Converters.Contents;
using Newtonsoft.Json.Linq;

namespace Yahv.Web.Erp.Views
{
    /// <summary>
    /// 颗粒化 视图
    /// </summary>
    class ParticleSettingsRoll : QueryView<Models.ParticleSetting, PvbErmReponsitory>
    {
        //string roleid;

        ///// <summary>
        ///// 默认构造器
        ///// </summary>
        //public ParticleSettingsRoll(string roleid)
        //{
        //    this.roleid = roleid;
        //}

        Underly.Erps.IRole role;

        /// <summary>
        /// 默认构造器
        /// </summary>
        public ParticleSettingsRoll(Underly.Erps.IRole role)
        {
            this.role = role;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">Linq支持者</param>
        internal ParticleSettingsRoll(PvbErmReponsitory reponsitory) : base(reponsitory)
        {
        }
        /// <summary>
        /// 颗粒化 可查询集
        /// </summary>
        /// <returns>可查询集</returns>
        sealed protected override IQueryable<Models.ParticleSetting> GetIQueryable()
        {
            IQueryable<Layers.Data.Sqls.PvbErm.ParticleSettings> iQuery;

            //依据类型获取视图
            if (role.Type == Underly.RoleType.Compose)
            {
                var roleIds_linq = from map in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.MapsRoleCompose>()
                                   where map.RoleID == this.role.ID
                                   select map.ChildID;

                var roleIds = roleIds_linq.Distinct().ToArray();

                iQuery = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.ParticleSettings>()
                         where roleIds.Contains(entity.RoleID)
                         select entity;
            }
            else
            {
                iQuery = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.ParticleSettings>()
                         where entity.RoleID == this.role.ID
                         select entity;
            }

            return iQuery.Select(entity => new Models.ParticleSetting
            {
                RoleID = entity.RoleID,
                UrlCode = entity.UrlCode,
                Url = entity.Url,
                Context = entity.Context,
                Type = entity.Type,
            });
        }

        /// <summary>
        /// 获取指定的UrlCode的设置数据
        /// </summary>
        /// <param name="path">地址</param>
        /// <returns>设置数据</returns>
        public Models.ParticleSetting this[string path]
        {
            get
            {
                string code = path?.ToLower().MD5();

                //加入逻辑
                if (this.role.Type == Underly.RoleType.Compose)
                {
                    var arry = this.Where(item => item.UrlCode == code).ToArray();
                    if (arry.Length > 0)
                    {
                        var first = arry.First();
                        var jfirst = JArray.Parse(first.Context);
                        foreach (var item in arry.Skip(1))
                        {
                            var jarry = JArray.Parse(item.Context);
                            foreach (var jitem in jarry)
                            {
                                if (!jfirst.Any(ifirst => ifirst["ID"] == jitem["ID"]))
                                {
                                    jfirst.Add(jitem);
                                 
                                }
                            }
                        }
                        //以上的做法不符合公式的要求，需要与王辉再次讨论
                        return new Models.ParticleSetting
                        {
                            Context = jfirst.ToString(),
                            RoleID = this.role.ID,
                            Type = "default",
                            Url = path,
                            UrlCode = code
                        };

                    }
                }

                return this.SingleOrDefault(item => item.UrlCode == code);
            }
        }
    }
}