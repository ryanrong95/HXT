using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace YaHv.Csrm.Services.Extends
{
    public static class WsSuppliersExtend
    {
        /// <summary>
        /// 审批结果：正常，否决
        /// </summary>
        static public void Approve(this Models.Origins.WsSupplier entity, ApprovalStatus Status)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                repository.Update<Layers.Data.Sqls.PvbCrm.WsSuppliers>(new
                {
                    Grade = (int)entity.Grade,
                    Summary = entity.Summary,
                    ChineseName = entity.ChineseName,
                    EnglishName = entity.EnglishName,
                    UpdateDate = entity.UpdateDate,
                    Status = (int)Status,
                    Place = entity.Place
                }, item => item.ID == entity.ID);
            }
        }
        /// <summary>
        /// 批量审批结果：正常，否决
        /// </summary>
        static public void Approve(this IEnumerable<Models.Origins.WsSupplier> wssuppliers, ApprovalStatus Status)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                foreach (var entity in wssuppliers)
                {
                    entity.Enterprise.Enter();
                    repository.Update<Layers.Data.Sqls.PvbCrm.WsSuppliers>(new
                    {
                        Grade = (int)entity.Grade,
                        Summary = entity.Summary,
                        Status = (int)entity.WsSupplierStatus,
                        ChineseName = entity.ChineseName,
                        EnglishName = entity.EnglishName,
                        UpdateDate = entity.UpdateDate,
                        Place = entity.Place
                    }, item => item.ID == entity.ID);
                }
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        static public void Delete(this IEnumerable<Models.Origins.WsSupplier> ienumers)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                var arry = ienumers.Select(item => item.ID);
                repository.Update<Layers.Data.Sqls.PvbCrm.WsSuppliers>(new
                {
                    Status = ApprovalStatus.Deleted
                }, item => arry.Contains(item.ID));
            }
        }
        /// <summary>
        /// 加入黑名单
        /// </summary>
        static public void Blacked(this IEnumerable<Models.Origins.WsSupplier> ienumers)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                var arry = ienumers.Select(item => item.ID);
                repository.Update<Layers.Data.Sqls.PvbCrm.WsSuppliers>(new
                {
                    Status = ApprovalStatus.Black
                }, item => arry.Contains(item.ID));
            }
        }
        /// <summary>
        /// 停用
        /// </summary>
        static public void Unable(this IEnumerable<Models.Origins.WsSupplier> ienumers)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                var arry = ienumers.Select(item => item.ID);
                repository.Update<Layers.Data.Sqls.PvbCrm.WsSuppliers>(new
                {
                    Status = (int)ApprovalStatus.Closed
                }, item => arry.Contains(item.ID));
            }
        }
        /// <summary>
        /// 启用
        /// </summary>
        static public void Enable(this IEnumerable<Models.Origins.WsSupplier> ienumers)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                var arry = ienumers.Select(item => item.ID);
                repository.Update<Layers.Data.Sqls.PvbCrm.WsSuppliers>(new
                {
                    Status = ApprovalStatus.Waitting
                }, item => arry.Contains(item.ID));
            }
        }
    }
}
