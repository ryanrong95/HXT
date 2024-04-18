using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Yahv.Underly.Erps;

namespace Yahv
{
    /// <summary>
    /// 消息通知
    /// </summary>
    static public class Notices
    {
        /// <summary>
        /// 通知提醒
        /// </summary>
        /// <param name="adminIDs">被提醒管理员ID集合</param>
        /// <param name="title">消息标题</param>
        /// <param name="context">消息内容</param>
        static public void Notice(this IEnumerable<string> adminIDs, string title, string context)
        {
            using (var reponsitory = LinqFactory<PvbErmReponsitory>.Create())
            {
                var admins = new Services.Views.AdminsAll<PvbErmReponsitory>(reponsitory).Where(item => adminIDs.Contains(item.ID));

                foreach (var admin in admins)
                {
                    reponsitory.Insert(new Layers.Data.Sqls.PvbErm.Logs_Notice
                    {
                        Title = title,
                        Context = context,
                        CreateDate = DateTime.Now,
                        Readed = false,
                        AdminID = admin.ID,
                    });
                }
            }

        }

        /// <summary>
        /// 标识通知已读
        /// </summary>
        /// <param name="admin">当前登录人</param>
        /// <param name="arry">消息 id 集合</param>
        static public void Read(this IErpAdmin admin, params long[] arry)
        {
            using (var reponsitory = LinqFactory<PvbErmReponsitory>.Create())
            {
                reponsitory.Update<Layers.Data.Sqls.PvbErm.Logs_Notice>(new
                {
                    Readed = true,
                    ReadDate = DateTime.Now
                }, item => arry.Contains(item.ID) && item.AdminID == admin.ID);
            }
        }

        /// <summary>
        /// 获取消息通知集合
        /// </summary>
        /// <param name="admin">当前登录人</param>
        /// <param name="top">数据条数</param>
        /// <returns></returns>
        static public Letter[] Tops(this IErpAdmin admin, int top = 500)
        {
            using (var reponsitory = LinqFactory<PvbErmReponsitory>.Create())
            {
                return reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Logs_Notice>().Where(item => item.AdminID == admin.ID).OrderByDescending(item => item.CreateDate).Take(top)
                    .Select(item => new Letter
                    {
                        ID = item.ID,
                        Title = item.Title,
                        Context = item.Context,
                        Readed = item.Readed,
                        CreateDate = item.CreateDate,
                        ReadDate = item.ReadDate,
                        AdminID = item.AdminID
                    }).ToArray();
            }
        }

        /// <summary>
        /// 消息实体类
        /// </summary>
        public class Letter
        {
            /// <summary>
            /// 标识ID
            /// </summary>
            public long ID { get; internal set; }

            /// <summary>
            /// 标题
            /// </summary>
            public string Title { get; internal set; }

            /// <summary>
            /// 内容
            /// </summary>
            public string Context { get; internal set; }

            /// <summary>
            /// 是否已读
            /// </summary>
            public bool Readed { get; internal set; }

            /// <summary>
            /// 创建时间
            /// </summary>
            public DateTime CreateDate { get; internal set; }

            /// <summary>
            /// 读取时间
            /// </summary>
            public DateTime? ReadDate { get; internal set; }

            /// <summary>
            /// 消息接受人
            /// </summary>
            public string AdminID { get; internal set; }
        }

        /// <summary>
        /// 通知提醒
        /// </summary>
        /// <param name="admins">被提醒管理员集合</param>
        /// <param name="title">消息标题</param>
        /// <param name="context">消息内容</param>
        static public void Notice(this IEnumerable<IErpAdmin> admins, string title, string context)
        {
            using (var reponsitory = LinqFactory<PvbErmReponsitory>.Create())
            {
                foreach (var admin in admins)
                {
                    reponsitory.Insert(new Layers.Data.Sqls.PvbErm.Logs_Notice
                    {
                        Title = title,
                        Context = context,
                        CreateDate = DateTime.Now,
                        Readed = false,
                        AdminID = admin.ID,
                    });
                }
            }
        }

        /// <summary>
        /// 通知提醒
        /// </summary>
        /// <param name="admin">管理员</param>
        /// <param name="title">消息标题</param>
        /// <param name="context">消息内容</param>
        static public void Notice(this IErpAdmin admin, string title, string context)
        {
            using (var reponsitory = LinqFactory<PvbErmReponsitory>.Create())
            {
                reponsitory.Insert(new Layers.Data.Sqls.PvbErm.Logs_Notice
                {
                    Title = title,
                    Context = context,
                    CreateDate = DateTime.Now,
                    Readed = false,
                    AdminID = admin.ID,
                });
            }
        }

    }
}
