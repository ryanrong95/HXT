using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using Yahv.Utils.Converters.Contents;

namespace YaHv.Csrm.Services.Extends
{
    public static class ClientExtend
    {
        /// <summary>
        /// 审批结果：正常，否决
        /// </summary>
        static public void Approve(this Models.Origins.Client entity, ApprovalStatus Status)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                repository.Update<Layers.Data.Sqls.PvbCrm.Clients>(new
                {
                    AreaType = (int)entity.AreaType,
                    Nature = (int)entity.Nature,
                    DyjCode = entity.DyjCode,
                    TaxperNumber = entity.TaxperNumber,
                    Grade = (int?)entity.Grade,
                    Vip = (int?)entity.Vip,
                    Status = (int)Status,
                    Place = entity.Place,
                    Major = entity.Major
                }, item => item.ID == entity.ID);
                if (!repository.GetTable<Layers.Data.Sqls.PvbCrm.Suppliers>().Any(item => item.ID == entity.ID && item.Status != (int)ApprovalStatus.Deleted))
                {
                    repository.Update<Layers.Data.Sqls.PvbCrm.Enterprises>(new
                    {
                        Status = (int)Status
                    }, item => item.ID == entity.ID);
                }
            }
        }
        /// <summary>
        /// 批量审批结果：正常，否决
        /// </summary>
        static public void Approve(this IEnumerable<Models.Origins.Client> Clients, ApprovalStatus Status)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                foreach (var entity in Clients)
                {
                    repository.Update<Layers.Data.Sqls.PvbCrm.Clients>(new
                    {
                        AreaType = (int)entity.AreaType,
                        Nature = (int)entity.Nature,
                        DyjCode = entity.DyjCode,
                        TaxperNumber = entity.TaxperNumber,
                        Grade = (int?)entity.Grade,
                        Vip = (int?)entity.Vip,
                        Status = (int)Status,
                        Place = entity.Place,
                        Major = entity.Major
                    }, item => item.ID == entity.ID);
                    if (!repository.GetTable<Layers.Data.Sqls.PvbCrm.Suppliers>().Any(item => item.ID == entity.ID && item.Status != (int)ApprovalStatus.Deleted))
                    {
                        repository.Update<Layers.Data.Sqls.PvbCrm.Enterprises>(new
                        {
                            Status = (int)Status
                        }, item => item.ID == entity.ID);
                    }
                }
            }
        }
        /// <summary>
        /// 删除
        /// </summary>
        static public void Delete(this IEnumerable<Models.Origins.Client> ienumers)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                var arry = ienumers.Select(item => item.ID);
                repository.Update<Layers.Data.Sqls.PvbCrm.Clients>(new
                {
                    Status = (int)ApprovalStatus.Deleted
                }, item => arry.Contains(item.ID));
            }
        }
        /// <summary>
        /// 加入黑名单
        /// </summary>
        static public void Blacked(this IEnumerable<Models.Origins.Client> ienumers)
        {
            var arry = ienumers.Select(item => item.ID);
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                repository.Update<Layers.Data.Sqls.PvbCrm.Clients>(new
                {
                    Status = (int)ApprovalStatus.Black
                }, item => arry.Contains(item.ID));
            }
        }
        /// <summary>
        /// 停用
        /// </summary>
        static public void Unable(this IEnumerable<Models.Origins.Client> ienumers)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                var arry = ienumers.Select(item => item.ID);
                repository.Update<Layers.Data.Sqls.PvbCrm.Clients>(new
                {
                    Status = (int)ApprovalStatus.Closed
                }, item => arry.Contains(item.ID));
            }
        }
        /// <summary>
        /// 启用
        /// </summary>
        static public void Enable(this IEnumerable<Models.Origins.Client> ienumers)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                var arry = ienumers.Select(item => item.ID);
                repository.Update<Layers.Data.Sqls.PvbCrm.Clients>(new
                {
                    Status = (int)ApprovalStatus.Waitting
                }, item => arry.Contains(item.ID));
            }
        }
        /// <summary>
        /// 是否存在客户、合作公司的关系
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        static public bool IsExist(this Models.Origins.TradingClient entity)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                return repository.GetTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>().Any(item => item.Bussiness == (int)Business.Trading_Sale && item.Type == (int)MapsType.Client && item.EnterpriseID == entity.Enterprise.ID && item.SubID == entity.CompanyID);
            }
        }

        /// <summary>
        /// 绑定销售人
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="adminid"></param>
        /// <param name="isDefault"></param>
        static public void AdminBinding(this Models.Origins.TradingClient entity, string currentAdminID, string saleid, bool isdefault = false)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                string mapsid = string.Join("", Business.Trading, MapsType.Client.ToString(), "_", entity.ID + saleid).MD5();
                if (!repository.GetTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>().Any(item => item.ID == mapsid))
                {

                    repository.Insert(new Layers.Data.Sqls.PvbCrm.MapsBEnter
                    {
                        ID = mapsid,
                        EnterpriseID = entity.ID,
                        Bussiness = (int)Business.Trading,
                        Type = (int)MapsType.Client,
                        SubID = saleid,
                        CtreatorID = currentAdminID,
                        CreateDate = DateTime.Now,
                        IsDefault = isdefault
                    });
                }
            }
        }
        /// <summary>
        /// 解绑销售人
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="adminid"></param>
        static public void AdminUnbind(this Models.Origins.TradingClient entity, string[] adminids)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                foreach (var adminid in adminids)
                {
                    string mapsid = string.Join("", Business.Trading, MapsType.Client.ToString(), "_", entity.ID + adminid).MD5();
                    if (repository.GetTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>().Any(item => item.ID == mapsid))
                    {
                        repository.Delete<Layers.Data.Sqls.PvbCrm.MapsBEnter>(item => item.ID == mapsid);
                    }
                }

            }
        }
        /// <summary>
        /// 设置默认管理员
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="adminid"></param>
        static public void SetDefault(this Models.Origins.TradingClient entity, string adminid)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                string mapsid = string.Join("", Business.Trading, MapsType.Client.ToString(), "_", entity.ID + adminid).MD5();
                if (repository.GetTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>().Any(item => item.Bussiness == (int)Business.Trading && item.EnterpriseID == entity.ID && item.Type == (int)MapsType.Client))
                {
                    repository.Update<Layers.Data.Sqls.PvbCrm.MapsBEnter>(new
                    {
                        IsDefault = false
                    }, item => item.Bussiness == (int)Business.Trading && item.EnterpriseID == entity.ID && item.Type == (int)MapsType.Client);
                }
                repository.Update<Layers.Data.Sqls.PvbCrm.MapsBEnter>(new
                {
                    IsDefault = true
                }, item => item.ID == mapsid);
            }
        }
        #region 合作公司与销售
        ///// <summary>
        ///// 绑定销售人
        ///// </summary>
        ///// <param name="entity"></param>
        ///// <param name="adminid"></param>
        ///// <param name="isDefault"></param>
        //static public void AdminBinding(this Models.Origins.TradingClient entity, string saleid, bool isdefault = false, Business business = Business.Trading)
        //{
        //    using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
        //    {
        //        string mapsid = string.Join("", business, MapsType.Client.ToString(), "_", entity.ID + saleid).MD5();
        //        if (isdefault)
        //        {
        //            //分配销售时如果设为默认，其他销售改为非默认，即把IsDefault设为false
        //            if (repository.GetTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>().Any(item => item.Bussiness == (int)Business.Trading && item.EnterpriseID == entity.ID && item.Type == (int)MapsType.Client))
        //            {
        //                repository.Update<Layers.Data.Sqls.PvbCrm.MapsBEnter>(new
        //                {
        //                    IsDefault = false
        //                }, item => item.Bussiness == (int)business && item.EnterpriseID == entity.Enterprise.ID && item.Type == (int)MapsType.Client);
        //            }
        //        }
        //        if (!repository.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>().Any(item => item.Bussiness == (int)business && item.Type == (int)MapsType.Client && item.EnterpriseID == entity.ID && item.SubID == entity.CompanyID))
        //        {
        //            if (!repository.GetTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>().Any(item => item.ID == mapsid))
        //            {
        //                repository.Insert(new Layers.Data.Sqls.PvbCrm.MapsBEnter
        //                {
        //                    ID = mapsid,
        //                    EnterpriseID = entity.ID,
        //                    Bussiness = (int)business,
        //                    Type = (int)MapsType.Client,
        //                    SubID = string.IsNullOrWhiteSpace(entity.CompanyID) ? "" : entity.CompanyID,
        //                    CtreatorID = saleid,
        //                    CreateDate = DateTime.Now,
        //                    IsDefault = isdefault
        //                });
        //            }
        //            else
        //            {
        //                repository.Update<Layers.Data.Sqls.PvbCrm.MapsBEnter>(new
        //                {
        //                    SubID = entity.CompanyID
        //                }, item => item.ID == mapsid);
        //            }
        //        }

        //    }
        //}
        ///// <summary>
        ///// 解绑销售人
        ///// </summary>
        ///// <param name="entity"></param>
        ///// <param name="adminid"></param>
        //static public void AdminUnbind(this Models.Origins.TradingClient entity, string[] adminids, Business business = Business.Trading)
        //{
        //    using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
        //    {
        //        foreach (var adminid in adminids)
        //        {
        //            string mapsid = string.Join("", business, MapsType.Client.ToString(), "_", entity.ID + adminid).MD5();
        //            if (repository.GetTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>().Any(item => item.ID == mapsid))
        //            {
        //                repository.Delete<Layers.Data.Sqls.PvbCrm.MapsBEnter>(item => item.ID == mapsid);
        //            }
        //        }

        //    }
        //}
        ///// <summary>
        ///// 设置默认管理员
        ///// </summary>
        ///// <param name="entity"></param>
        ///// <param name="adminid"></param>
        //static public void SetDefault(this Models.Origins.TradingClient entity, string adminid, Business business = Business.Trading)
        //{
        //    using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
        //    {
        //        string mapsid = string.Join("", business, MapsType.Client.ToString(), "_", entity.ID + adminid).MD5();
        //        if (repository.GetTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>().Any(item => item.Bussiness == (int)Business.Trading && item.EnterpriseID == entity.ID && item.Type == (int)MapsType.Client))
        //        {
        //            repository.Update<Layers.Data.Sqls.PvbCrm.MapsBEnter>(new
        //            {
        //                IsDefault = false
        //            }, item => item.Bussiness == (int)business && item.EnterpriseID == entity.ID && item.Type == (int)MapsType.Client);
        //        }
        //        repository.Update<Layers.Data.Sqls.PvbCrm.MapsBEnter>(new
        //        {
        //            IsDefault = true
        //        }, item => item.ID == mapsid);
        //    }
        //}
        #endregion

        /// <summary>
        /// 绑定合作关系
        /// </summary>
        /// <param name="realid">客户id,供应商id</param>
        static public void CooperBinding(this Models.Origins.Client entity, string realid, CooperType cooperType)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                if (!repository.GetTable<Layers.Data.Sqls.PvbCrm.MapsEnterprise>().Any(item => item.EnterpriseID == entity.Enterprise.ID
                && item.RealID == realid
                && item.CooperType == (int)cooperType))
                {
                    repository.Insert(new Layers.Data.Sqls.PvbCrm.MapsEnterprise
                    {
                        EnterpriseID = entity.Enterprise.ID,
                        RealID = realid,
                        CooperType = (int)cooperType
                    });
                }
            }
        }
        /// <summary>
        /// 解绑合作公司
        /// </summary>
        /// <param name="realids">客户id,供应商id</param>
        static public void CooperUnbind(this Models.Origins.Client entity, string[] realids)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                foreach (var realid in realids)
                {
                    if (repository.GetTable<Layers.Data.Sqls.PvbCrm.MapsEnterprise>().Any(item => item.EnterpriseID == entity.Enterprise.ID
                && item.RealID == realid))
                    {
                        repository.Delete<Layers.Data.Sqls.PvbCrm.MapsEnterprise>(item =>
                            item.EnterpriseID == entity.Enterprise.ID
                            && item.RealID == realid);
                    }
                }
            }
        }
        /// <summary>
        /// 解绑合作关系
        /// </summary>
        /// <param name="realids">客户id,供应商id</param>
        static public void CooperUnbind(this Models.Origins.Client entity, string[] realids, CooperType coopertype)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                foreach (var realid in realids)
                {
                    if (repository.GetTable<Layers.Data.Sqls.PvbCrm.MapsEnterprise>().Any(item => item.EnterpriseID == entity.Enterprise.ID
               && item.RealID == realid
               && item.CooperType == (int)coopertype))
                    {
                        repository.Delete<Layers.Data.Sqls.PvbCrm.MapsEnterprise>(item =>
                            item.EnterpriseID == entity.Enterprise.ID
                            && item.RealID == realid
                            && item.CooperType == (int)coopertype);
                    }
                }
            }
        }
        /// <summary>
        /// 付款人启用
        /// </summary>
        /// <param name="entity"></param>
        static public void Enable(this Models.Origins.Payer entity)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                repository.Update<Layers.Data.Sqls.PvbCrm.Payers>(new
                {
                    Status = Yahv.Underly.GeneralStatus.Normal
                }, item => item.ID == entity.ID);

            }
        }

        /// <summary>
        ///收款人启用
        /// </summary>
        /// <param name="entity"></param>
        static public void Enable(this Models.Origins.Payee entity)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                repository.Update<Layers.Data.Sqls.PvbCrm.Payees>(new
                {
                    Status = Yahv.Underly.GeneralStatus.Normal
                }, item => item.ID == entity.ID);

            }
        }
    }
}
