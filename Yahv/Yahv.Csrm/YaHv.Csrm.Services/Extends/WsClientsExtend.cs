using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using Yahv.Utils.Converters.Contents;
using YaHv.Csrm.Services.Views.Rolls;

namespace YaHv.Csrm.Services.Extends
{
    public static class WsClientsExtend
    {
        /// <summary>
        /// 审批结果：正常，否决
        /// </summary>
        static public void Approve(this Models.Origins.WsClient entity, ApprovalStatus Status)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                repository.Update<Layers.Data.Sqls.PvbCrm.WsClients>(new
                {
                    Grade = (int)entity.Grade,
                    Vip = entity.Vip,
                    EnterCode = entity.EnterCode,
                    CustomsCode = entity.CustomsCode,
                    UpdateDate = entity.UpdateDate,
                    Summary = entity.Summary,
                    Status = (int)Status,
                    Place = entity.Place
                }, item => item.ID == entity.ID);
            }
        }
        static public void Complete(this Models.Origins.WsClient entity)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                repository.Update<Layers.Data.Sqls.PvbCrm.WsClients>(new
                {
                    Status = ApprovalStatus.Normal
                }, item => item.ID == entity.ID);
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        //static public void Delete(this IEnumerable<Models.Origins.WsClient> ienumers)
        //{
        //    using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
        //    {
        //        var arry = ienumers.Select(item => item.ID);
        //        repository.Update<Layers.Data.Sqls.PvbCrm.WsClients>(new
        //        {
        //            Status = ApprovalStatus.Deleted
        //        }, item => arry.Contains(item.ID));
        //    }
        //}
        ///删除的是与内部公司的关系
        static public void Delete(this IEnumerable<Models.Origins.WsClient> ienumers)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                var arry = ienumers.Select(item => item.MapsID).ToArray();
                repository.Delete<Layers.Data.Sqls.PvbCrm.MapsBEnter>(item => arry.Contains(item.ID));
            }
        }
        /// <summary>
        /// 加入黑名单
        /// </summary>
        static public void Blacked(this IEnumerable<Models.Origins.WsClient> ienumers)
        {
            var arry = ienumers.Select(item => item.ID);
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                repository.Update<Layers.Data.Sqls.PvbCrm.WsClients>(new
                {
                    Status = (int)ApprovalStatus.Black
                }, item => arry.Contains(item.ID));
            }
        }
        /// <summary>
        /// 停用
        /// </summary>
        static public void Unable(this IEnumerable<Models.Origins.WsClient> ienumers)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                var arry = ienumers.Select(item => item.ID);
                repository.Update<Layers.Data.Sqls.PvbCrm.WsClients>(new
                {
                    Status = (int)ApprovalStatus.Closed
                }, item => arry.Contains(item.ID));
            }
        }
        /// <summary>
        /// 启用
        /// </summary>
        static public void Enable(this IEnumerable<Models.Origins.WsClient> ienumers)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                var arry = ienumers.Select(item => item.ID);
                repository.Update<Layers.Data.Sqls.PvbCrm.WsClients>(new
                {
                    Status = (int)ApprovalStatus.Waitting
                }, item => arry.Contains(item.ID));
            }
        }
        /// <summary>
        /// 添加与供应商的关系
        /// </summary>
        /// <param name="client"></param>
        /// <param name="supplierid"></param>
        /// <param name="adminid"></param>
        static public void MapsSupplier(this Models.Origins.XdtWsSupplier supplier, string clientid, string adminid, bool IsDefault = false)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                if (!repository.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>().Any(item => item.ID == supplier.MapsID))
                {
                    repository.Insert(new Layers.Data.Sqls.PvbCrm.MapsBEnter
                    {
                        ID = supplier.MapsID,
                        Bussiness = (int)Business.WarehouseServicing,
                        Type = (int)MapsType.WsSupplier,
                        EnterpriseID = clientid,
                        SubID = supplier.ID,
                        CreateDate = DateTime.Now,
                        CtreatorID = adminid,
                        IsDefault = IsDefault
                    });
                }
            }
        }

        /// <summary>
        /// 与内部公司关系
        /// </summary>
        /// <param name="wsclient"></param>
        /// <param name="enterpriseid">内部公司ID</param>
        /// <param name="adminid">添加人</param>
        /// <param name="IsDefault">是否默认</param>
        static public void MapsCompany(this Models.Origins.WsClient wsclient, bool IsDefault = false)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                if (!repository.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>().Any(item => item.ID == wsclient.MapsID))
                {
                    repository.Insert(new Layers.Data.Sqls.PvbCrm.MapsBEnter
                    {
                        ID = wsclient.MapsID,
                        Bussiness = (int)Business.WarehouseServicing,
                        Type = (int)MapsType.WsClient,
                        EnterpriseID = wsclient.Enterprise.ID,
                        SubID = wsclient.Company.ID,
                        CreateDate = DateTime.Now,
                        CtreatorID = wsclient.CreatorID,
                        IsDefault = IsDefault
                    });
                }
            }
        }
        /// <summary>
        /// 分配业务员或跟单员
        /// </summary>
        /// <param name="client"></param>
        /// <param name="supplierid"></param>
        static public void Assin(this Models.Origins.WsClient client, string adminid, MapsType type)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                var admin = new AdminsAllRoll()[adminid];
                string id = string.Join("",
                      client.Company.ID,
                      client.ID,
                      Business.WarehouseServicing,
                      type
                  ).MD5();
                if (repository.ReadTable<Layers.Data.Sqls.PvbCrm.MapsTracker>().Any(item => item.ID == id))
                {
                    repository.Update<Layers.Data.Sqls.PvbCrm.MapsTracker>(new { AdminID = adminid }, item => item.ID == id);
                }
                else
                {
                    repository.Insert(new Layers.Data.Sqls.PvbCrm.MapsTracker
                    {
                        ID = id,
                        Bussiness = (int)Business.WarehouseServicing,
                        AdminID = adminid,
                        RealID = client.MapsID,
                        Type = (int)type,
                        IsDefault = true
                    });
                }
            }
        }
    }
}
