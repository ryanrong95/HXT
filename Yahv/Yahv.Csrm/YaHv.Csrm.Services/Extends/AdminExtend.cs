using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using Yahv.Utils.Converters.Contents;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Extends
{
    public static class AdminExtend
    {
        /// <summary>
        /// 绑定
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="mainid"></param>
        /// <param name="Type"></param>
        static public void Binding(this Admin entity, string enterpriseid, string realid, MapsType Type, bool Isdefault = false)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                string mapsid = string.Join("", Business.Trading, Type.ToString(), "_", realid, entity.ID).MD5();
                if (Type == MapsType.Supplier || Type == MapsType.Client)
                {
                    mapsid = string.Join("", Business.Trading, Type.ToString(), "_", enterpriseid, entity.ID).MD5();
                }
                if (!repository.GetTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>().Any(item => item.ID == mapsid))
                {
                    repository.Insert(new Layers.Data.Sqls.PvbCrm.MapsBEnter
                    {
                        ID = mapsid,
                        EnterpriseID = enterpriseid,
                        Bussiness = (int)Business.Trading,
                        Type = (int)Type,
                        SubID = realid,
                        CtreatorID = entity.ID,
                        CreateDate = DateTime.Now,
                        IsDefault = Isdefault
                    });
                }
            }
        }
        /// <summary>
        /// 解绑
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="mainid"></param>
        /// <param name="Type"></param>
        static public void Unbind(this Admin entity, string realid, MapsType Type)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                string mapsid = string.Join("", Business.Trading, Type.ToString(), "_" + realid + entity.ID).MD5();
                if (repository.GetTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>().Any(item =>
                         item.ID == mapsid))
                {
                    repository.Delete<Layers.Data.Sqls.PvbCrm.MapsBEnter>(item => item.ID == mapsid);
                }

            }
        }
        /// <summary>
        /// 设置默认
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="realid">实际ID</param>
        /// <param name="Type">类型</param>
        /// <param name="Isdefault">是否默认</param>
        static public void SetDefault(this Admin entity, string realid, MapsType Type)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                string mapsid = string.Join("", Business.Trading, Type.ToString(), "_", realid + entity.ID).MD5();
                if (repository.GetTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>().Any(item =>
                         item.EnterpriseID == realid
                         && item.Bussiness == (int)Business.Trading
                         && item.Type == (int)Type))
                {
                    repository.Update<Layers.Data.Sqls.PvbCrm.MapsBEnter>(new
                    {
                        IsDefault = false
                    }, item => item.EnterpriseID == realid
                         && item.Bussiness == (int)Business.Trading
                         && item.Type == (int)Type);
                }
                repository.Update<Layers.Data.Sqls.PvbCrm.MapsBEnter>(new
                {
                    IsDefault = true
                }, item => item.ID == mapsid);
            }
        }
    }
}
