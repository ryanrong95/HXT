using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace Yahv.Services
{

    public class LogContext
    {
        public LogContext()
        {

        }
    }

    public class PlatNotice
    {
        //D:\Projects_vs2015\Yahv\Solutions\Yahv.Underly\Enums\Enum.LogNoticeType.cs

        /// <summary>
        /// 跟单员 角色名称
        /// </summary>
        public const string Tracker = "跟单员";
        /// <summary>
        /// 报关员 角色名称
        /// </summary>
        public const string Declarants = "报关员";

        PlatNotice()
        {

        }

        string index;

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="index">索引（别通知人标识）</param>
        /// <returns></returns>
        public PlatNotice this[string index]
        {
            get
            {
                this.index = index;
                return this;
            }
        }

        /// <summary>
        /// 录入消息
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="context">内容</param>
        /// <param name="mainID">一般是订单ID</param>
        /// <param name="type">类型按照要求选择</param>
        public void Enter(string title, string context, string mainID = null, Yahv.Underly.LogNoticeType? type = null)
        {
            if (string.IsNullOrWhiteSpace(this.index))
            {
                throw new Exception();
            }

            using (var reponsitory = new Layers.Data.Sqls.PvbErmReponsitory())
            {
                if (this == byAdmin)
                {
                    reponsitory.Insert(new Layers.Data.Sqls.PvbErm.Logs_Notice
                    {
                        Title = title,
                        Context = context,
                        CreateDate = DateTime.Now,
                        Readed = false,
                        ReadDate = null,
                        AdminID = this.index,
                        MainID = mainID,
                        Type = (int?)type,
                    });
                }

                if (this == ByRole)
                {
                    var linqs = from role in reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Roles>()
                                join admin in reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Admins>() on role.ID equals admin.RoleID
                                where role.Name.Contains(this.index)
                                select admin.ID;

                    var arry = linqs.ToArray();

                    foreach (var item in arry)
                    {
                        ByAdmin[item].Enter(title, context, mainID, type);
                    }
                }
            }

            if (this == ByhsSite)
            {
                using (var reponsitory = new Layers.Data.Sqls.PvWsOrderReponsitory())
                {
                    reponsitory.Insert(new Layers.Data.Sqls.PvWsOrder.Notices
                    {
                        ID = Layers.Data.PKeySigner.Pick(Yahv.Underly.PKeyType.wsNotice),
                        Title = title,
                        Context = context,
                        CreateDate = DateTime.Now,
                        ReadDate = null,
                        MainID = mainID,
                        Type = (int?)type,
                        CreatorID = null,
                        EnterCode = this.index,
                    });
                }
            }
        }

        //通用视图再提供吧
        //protected override IQueryable<LogContext> GetIQueryable()
        //{
        //    if (this == byAdmin || this == ByRole)
        //    {
        //        var reponsitory = new Layers.Data.Sqls.PvbErmReponsitory();

        //        return from log in reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Logs_Notice>()
        //               select new LogContext
        //               {

        //               };
        //    }
        //}

        static PlatNotice byAdmin;
        static object locker = new object();
        /// <summary>
        /// 后台
        /// </summary>
        /// <remarks>
        /// 操作员
        /// </remarks>
        static public PlatNotice ByAdmin
        {
            get
            {
                if (byAdmin == null)
                {
                    lock (locker)
                    {
                        if (byAdmin == null)
                        {
                            byAdmin = new PlatNotice();
                        }
                    }
                }
                return byAdmin;
            }
        }

        static PlatNotice byRole;
        /// <summary>
        /// 后台角色
        /// </summary>
        static public PlatNotice ByRole
        {
            get
            {
                if (byRole == null)
                {
                    lock (locker)
                    {
                        if (byRole == null)
                        {
                            byRole = new PlatNotice();
                        }
                    }
                }
                return byRole;
            }
        }

        static PlatNotice byhsSite;
        /// <summary>
        /// 前端
        /// </summary>
        /// <remarks>
        /// 代仓储专用
        /// </remarks>
        static public PlatNotice ByhsSite
        {
            get
            {
                if (byhsSite == null)
                {
                    lock (locker)
                    {
                        if (byhsSite == null)
                        {
                            byhsSite = new PlatNotice();
                        }
                    }
                }
                return byhsSite;
            }
        }
    }
}
