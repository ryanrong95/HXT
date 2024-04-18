using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;

namespace Yahv.PvWsOrder.Services.Extends
{
    /// <summary>
    /// Crm拓展类
    /// </summary>
    internal static class PvbCrmExtends
    {
        /// <summary>
        /// 关系插入
        /// </summary>
        /// <param name="mapsBEnter">关系数据</param>
        static public void MapsBEnter(this Layers.Data.Sqls.PvbCrm.MapsBEnter mapsBEnter)
        {
            using (PvbCrmReponsitory reponsitory = Layers.Linq.LinqFactory<PvbCrmReponsitory>.Create())
            {
                if (reponsitory.GetTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>().Any(a => a.ID == mapsBEnter.ID))
                {
                    reponsitory.Update<Layers.Data.Sqls.PvbCrm.MapsBEnter>(new
                    {
                        mapsBEnter.Bussiness,
                        mapsBEnter.Type,
                        mapsBEnter.EnterpriseID,
                        mapsBEnter.IsDefault,
                        mapsBEnter.SubID,
                    },item=>item.ID == mapsBEnter.ID);
                }
                else
                {
                    reponsitory.Insert(mapsBEnter);
                }
            }
        }

        /// <summary>
        /// 关系插入
        /// </summary>
        /// <param name="mapsBEnter">关系数据</param>
        /// <param name="reponsitory">数据库连接</param>
        static public void MapsBEnter(this Layers.Data.Sqls.PvbCrm.MapsBEnter mapsBEnter, PvbCrmReponsitory reponsitory)
        {

            if (reponsitory.GetTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>().Any(a => a.ID == mapsBEnter.ID))
            {
                reponsitory.Update<Layers.Data.Sqls.PvbCrm.MapsBEnter>(new
                {
                    mapsBEnter.Bussiness,
                    mapsBEnter.Type,
                    mapsBEnter.EnterpriseID,
                    mapsBEnter.IsDefault,
                    mapsBEnter.SubID,
                }, item => item.ID == mapsBEnter.ID);
            }
            else
            {
                reponsitory.Insert(mapsBEnter);
            }
        }

        /// <summary>
        /// 企业插入
        /// </summary>
        /// <param name="mapsBEnter">关系数据</param>
        /// <param name="reponsitory">数据库连接</param>
        static public void EnterpriseEnter(this Layers.Data.Sqls.PvbCrm.Enterprises enterprise, PvbCrmReponsitory reponsitory)
        {
            if (reponsitory.GetTable<Layers.Data.Sqls.PvbCrm.Enterprises>().Any(item => item.ID == enterprise.ID))
            {
                reponsitory.Update<Layers.Data.Sqls.PvbCrm.Enterprises>(new
                {
                    enterprise.AdminCode,
                    enterprise.Name,
                    enterprise.RegAddress,
                    enterprise.Corporation,
                    enterprise.Uscc,
                }, item => item.ID == enterprise.ID);
            }
            else
            {
                reponsitory.Insert(enterprise);
            }
        }


        /// <summary>
        /// 企业插入
        /// </summary>
        /// <param name="mapsBEnter">关系数据</param>
        /// <param name="reponsitory">数据库连接</param>
        static public void SupplierEnter(this Layers.Data.Sqls.PvbCrm.WsSuppliers supplier, PvbCrmReponsitory reponsitory)
        {
            if (reponsitory.GetTable<Layers.Data.Sqls.PvbCrm.WsSuppliers>().Any(item => item.ID == supplier.ID))
            {
                reponsitory.Update<Layers.Data.Sqls.PvbCrm.WsSuppliers>(new
                {
                    supplier.ChineseName,
                    supplier.EnglishName,
                    supplier.Summary,
                    supplier.Grade,
                }, item => item.ID == supplier.ID);
            }
            else
            {
                reponsitory.Insert(supplier);
            }
        }
    }
}
