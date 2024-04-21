using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using Yahv.Utils.Converters.Contents;
using Yahv.Utils.Serializers;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Extends
{
    public static class SupplierExtend
    {
        /// <summary>
        /// 审批结果：正常，否决
        /// </summary>
        static public void Approve(this Models.Origins.Supplier entity, ApprovalStatus Status)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                repository.Update<Layers.Data.Sqls.PvbCrm.Suppliers>(new
                {
                    Type = (int)entity.Type,
                    Nature = (int)entity.Nature,
                    DyjCode = entity.DyjCode,
                    TaxperNumber = entity.TaxperNumber,
                    AreaType = (int)entity.AreaType,
                    InvoiceType = (int)entity.InvoiceType,
                    IsFactory = entity.IsFactory,
                    AgentCompany = entity.AgentCompany,
                    Grade = (int?)entity.Grade,
                    Status = (int)Status,
                    RepayCycle = (int)entity.RepayCycle,
                    Currency = (int)entity.Currency,
                    Price = entity.Price,
                    Place = entity.Place,
                    IsForwarder = entity.IsForwarder
                }, item => item.ID == entity.ID);
                if (!repository.GetTable<Layers.Data.Sqls.PvbCrm.Clients>().Any(item => item.ID == entity.ID))
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
        static public void Approve(this IEnumerable<Models.Origins.Supplier> suppliers, ApprovalStatus Status)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                foreach (var entity in suppliers)
                {
                    repository.Update<Layers.Data.Sqls.PvbCrm.Suppliers>(new
                    {
                        Type = (int)entity.Type,
                        Nature = (int)entity.Nature,
                        DyjCode = entity.DyjCode,
                        TaxperNumber = entity.TaxperNumber,
                        AreaType = (int)entity.AreaType,
                        InvoiceType = (int)entity.InvoiceType,
                        IsFactory = entity.IsFactory,
                        AgentCompany = entity.AgentCompany,
                        Grade = (int?)entity.Grade,
                        Status = (int)Status,
                        RepayCycle = (int)entity.RepayCycle,
                        Currency = (int)entity.Currency,
                        Price = entity.Price,
                        Place = entity.Place
                    }, item => item.ID == entity.ID);
                    if (!repository.GetTable<Layers.Data.Sqls.PvbCrm.Clients>().Any(item => item.ID == entity.ID))
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
        static public void Delete(this IEnumerable<Models.Origins.Supplier> ienumers)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                var arry = ienumers.Select(item => item.ID);
                repository.Update<Layers.Data.Sqls.PvbCrm.Suppliers>(new
                {
                    Status = ApprovalStatus.Deleted
                }, item => arry.Contains(item.ID));
            }
        }
        /// <summary>
        /// 加入黑名单
        /// </summary>
        static public void Blacked(this IEnumerable<Models.Origins.Supplier> ienumers)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                var arry = ienumers.Select(item => item.ID);
                repository.Update<Layers.Data.Sqls.PvbCrm.Suppliers>(new
                {
                    Status = ApprovalStatus.Black
                }, item => arry.Contains(item.ID));
            }
        }
        /// <summary>
        /// 停用
        /// </summary>
        static public void Unable(this IEnumerable<Models.Origins.Supplier> ienumers)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                var arry = ienumers.Select(item => item.ID);
                repository.Update<Layers.Data.Sqls.PvbCrm.Suppliers>(new
                {
                    Status = (int)ApprovalStatus.Closed
                }, item => arry.Contains(item.ID));
            }
        }
        /// <summary>
        /// 启用
        /// </summary>
        static public void Enable(this IEnumerable<Models.Origins.Supplier> ienumers)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                var arry = ienumers.Select(item => item.ID);
                repository.Update<Layers.Data.Sqls.PvbCrm.Suppliers>(new
                {
                    Status = ApprovalStatus.Waitting
                }, item => arry.Contains(item.ID));
            }
        }


        #region 合作公司与采购
        ///// <summary>
        ///// 绑定
        ///// </summary>
        ///// <param name="entity"></param>
        ///// <param name="adminid"></param>
        ///// <param name="isDefault"></param>
        //static public void AdminBinding(this Models.Origins.TradingSupplier entity, string currentAdmin, string purachaserid, bool isdefault = false, Business business = Business.Trading_Purchase)
        //{
        //    using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
        //    {
        //        string mapsid = string.Join("", business, MapsType.Supplier.ToString(), "_", entity.ID + purachaserid).MD5();
        //        if (!repository.GetTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>().Any(item => item.ID == mapsid))
        //        {
        //            repository.Insert(new Layers.Data.Sqls.PvbCrm.MapsBEnter
        //            {
        //                ID = mapsid,
        //                EnterpriseID = entity.ID,
        //                Bussiness = (int)business,
        //                Type = (int)MapsType.Supplier,
        //                SubID = purachaserid,
        //                CtreatorID = currentAdmin,
        //                CreateDate = DateTime.Now,
        //                IsDefault = isdefault
        //            });
        //        }
        //    }
        //}
        ///// <summary>
        ///// 解绑
        ///// </summary>
        ///// <param name="entity"></param>
        ///// <param name="adminids"></param>
        //static public void AdminUnbind(this Models.Origins.TradingSupplier entity, string[] adminids, Business business = Business.Trading_Purchase)
        //{
        //    using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
        //    {
        //        foreach (var purachaserid in adminids)
        //        {
        //            string mapsid = string.Join("", business, MapsType.Supplier.ToString(), "_", entity.ID + purachaserid).MD5();
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
        //static public void SetDefault(this Models.Origins.TradingSupplier entity, string adminid, Business business = Business.Trading_Purchase)
        //{
        //    using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
        //    {
        //        string mapsid = string.Join("", business, MapsType.Supplier.ToString(), "_", entity.ID + adminid).MD5();
        //        if (repository.GetTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>().Any(item => item.Bussiness == (int)business && item.EnterpriseID == entity.ID
        //                && item.Type == (int)MapsType.Supplier
        //                && item.IsDefault == true))
        //        {
        //            repository.Update<Layers.Data.Sqls.PvbCrm.MapsBEnter>(new
        //            {
        //                IsDefault = false
        //            }, item => item.Bussiness == (int)business && item.EnterpriseID == entity.ID
        //             && item.Type == (int)MapsType.Supplier);
        //        }
        //        repository.Update<Layers.Data.Sqls.PvbCrm.MapsBEnter>(new
        //        {
        //            IsDefault = true
        //        }, item => item.ID == mapsid);
        //    }
        //}
        #endregion
        /// <summary>
        /// 绑定
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="adminid"></param>
        /// <param name="isDefault"></param>
        static public void AdminBinding(this Models.Origins.TradingSupplier entity, string currentAdmin, string purachaserid, bool isdefault = false)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                string mapsid = string.Join("", Business.Trading, MapsType.Supplier.ToString(), "_", entity.ID + purachaserid).MD5();
                if (!repository.GetTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>().Any(item => item.ID == mapsid))
                {
                    repository.Insert(new Layers.Data.Sqls.PvbCrm.MapsBEnter
                    {
                        ID = mapsid,
                        EnterpriseID = entity.ID,
                        Bussiness = (int)Business.Trading,
                        Type = (int)MapsType.Supplier,
                        SubID = purachaserid,
                        CtreatorID = currentAdmin,
                        CreateDate = DateTime.Now,
                        IsDefault = isdefault
                    });
                }
            }
        }
        /// <summary>
        /// 解绑
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="adminids"></param>
        static public void AdminUnbind(this Models.Origins.TradingSupplier entity, string[] adminids)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                foreach (var purachaserid in adminids)
                {
                    string mapsid = string.Join("", Business.Trading, MapsType.Supplier.ToString(), "_", entity.ID + purachaserid).MD5();
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
        static public void SetDefault(this Models.Origins.TradingSupplier entity, string adminid)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                string mapsid = string.Join("", Business.Trading, MapsType.Supplier.ToString(), "_", entity.ID + adminid).MD5();
                if (repository.GetTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>().Any(item => item.Bussiness == (int)Business.Trading && item.EnterpriseID == entity.ID
                        && item.Type == (int)MapsType.Supplier
                        && item.IsDefault == true))
                {
                    repository.Update<Layers.Data.Sqls.PvbCrm.MapsBEnter>(new
                    {
                        IsDefault = false
                    }, item => item.Bussiness == (int)Business.Trading && item.EnterpriseID == entity.ID
                     && item.Type == (int)MapsType.Supplier);
                }
                repository.Update<Layers.Data.Sqls.PvbCrm.MapsBEnter>(new
                {
                    IsDefault = true
                }, item => item.ID == mapsid);
            }
        }
    }
}
