//using Layers.Data.Sqls;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Yahv.Linq;
//using Wms.Services;
//using Yahv.Services.Enums;
//using Newtonsoft.Json.Linq;
//using Yahv.Utils.Serializers;
//using Yahv.Underly;
//using Layers.Data;
//using Layers.Data.Sqls.PvWms;
//using Yahv.Utils.Converters.Contents;
//using Wms.Services.Extends;
//using Yahv.Underly.Enums;
//using Wms.Services.Enums;

//namespace Wms.Services.chonggous.Views
//{
//    /// <summary>
//    /// 申报视图
//    /// </summary>
//    /// <remarks>帮助乔霞的UI要求重构</remarks>
//    [Obsolete]
//    public class CgReportsView_bak_bak : QueryView<object, PvWmsRepository>
//    {

//        #region 构造函数 

//        public CgReportsView_bak()
//        {

//        }

//        protected CgReportsView_bak(PvWmsRepository reponsitory) : base(reponsitory)
//        {
//        }

//        protected CgReportsView_bak(PvWmsRepository reponsitory, IQueryable<object> iQueryable) : base(reponsitory, iQueryable)
//        {
//        }

//        #endregion

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <returns></returns>
//        /// <remarks>
//        /// 乔霞UI字段要求
//        /// 订单号
//        /// 申报状态
//        /// 装箱时间
//        /// 装箱人
//        /// 客户编号
//        /// 库位
//        /// 规格
//        /// 总重
//        /// 毛重
//        /// 型号
//        /// 品牌
//        /// 批次
//        /// 数量
//        /// 原产地
//        /// </remarks>
//        protected override IQueryable<object> GetIQueryable()
//        {
//            var waybillViews = new Wms.Services.Views.ServicesWaybillsTopView();

//            // sorting.Quantity
//            // sorting.Weight
//            // 总毛重：sum(sorting.Weight)
//            // 毛重：sum(sorting.Weight)/所有的数量*分拣的数量

//            //已经第数不清楚的次，进行修改了。

//            return from log in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Logs_Declare>()
//                   orderby log.CreateDate descending
//                   select new MyReport
//                   {
//                       TinyID = log.ID,
//                       DeclareStatus = (TinyOrderDeclareStatus)log.Status,
//                   };
//        }


//        /// <summary>
//        /// 分页方法
//        /// </summary>
//        /// <returns></returns>
//        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
//        {
//            var iquery = this.IQueryable.Cast<MyReport>();
//            int total = iquery.Count();

//            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
//            {
//                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
//            }

//            #region 视图的准备

//            var adminTopView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.AdminsTopView>();
//            var waybillsTopView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>();

//            //需要申报日志数据的支持 Logs_Declare

//            #region 分拣

//            var linq_sortings =
//               from storage in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>() //最终的那个。
//               join product in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.ProductsTopView>() on storage.ProductID equals product.ID
//               join sorting in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Sortings>() on storage.SortingID equals sorting.ID
//               join admin in adminTopView on sorting.AdminID equals admin.ID //分拣人员
//               join input in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Inputs>() on storage.InputID equals input.ID
//               join notice in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>() on sorting.NoticeID equals notice.ID
//               join waybill in waybillsTopView on notice.WaybillID equals waybill.wbID
//               join log in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Logs_Declare>() on input.TinyOrderID equals log.TinyOrderID
//               where sorting.BoxCode != null
//                && ((notice.Type == (int)CgNoticeType.Enter && notice.Source == (int)NoticeSource.AgentBreakCustoms)
//                        || notice.Type == (int)CgNoticeType.Boxing)
//               select new
//               {
//                   #region 视图
//                   input.TinyOrderID,
//                   Operation = new
//                   {
//                       ID = sorting.ID,
//                       sorting.BoxCode,
//                       sorting.Quantity,//使用分拣数量
//                       admin.RealName, //分拣人
//                       BoxingDate = sorting.CreateDate, // 装箱日期
//                       ClientCode = waybill.wbEnterCode,// 客户编号}
//                       DeclareStatus = (TinyOrderDeclareStatus)log.Status,
//                   },
//                   Info = new
//                   {
//                       input.OrderID,
//                       input.TinyOrderID,
//                   },
//                   StorageProcut = new
//                   {
//                       PartNumber = product.PartNumber,
//                       Manufacturer = product.Manufacturer,
//                       PackageCase = product.PackageCase,
//                       Packaging = product.Packaging,
//                   }

//                   #endregion
//               };

//            #endregion

//            #region 拣货

//            //join output in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Inputs>() on picking.ID equals output.ID

//            var linq_pickings =
//                 from storage in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>()
//                 join product in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.ProductsTopView>() on storage.ProductID equals product.ID
//                 join picking in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Pickings>() on storage.ID equals picking.StorageID
//                 join admin in adminTopView on picking.AdminID equals admin.ID
//                 join output in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Inputs>() on picking.ID equals output.ID
//                 join notice in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>() on picking.NoticeID equals notice.ID
//                 join waybill in waybillsTopView on notice.WaybillID equals waybill.wbID
//                 join log in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Logs_Declare>() on output.TinyOrderID equals log.TinyOrderID
//                 where picking.BoxCode != null
//                     && notice.Type == (int)CgNoticeType.Boxing
//                 select new
//                 {
//                     output.TinyOrderID,
//                     Operation = new
//                     {
//                         ID = picking.ID,
//                         picking.BoxCode,
//                         picking.Quantity,//使用分拣数量
//                         admin.RealName, //分拣人
//                         BoxingDate = picking.CreateDate, // 装箱日期
//                         ClientCode = waybill.wbEnterCode,// 客户编号}
//                         DeclareStatus = (TinyOrderDeclareStatus)log.Status,
//                     },
//                     Info = new
//                     {
//                         output.OrderID,
//                         output.TinyOrderID,
//                     },
//                     StorageProcut = new
//                     {
//                         PartNumber = product.PartNumber,
//                         Manufacturer = product.Manufacturer,
//                         PackageCase = product.PackageCase,
//                         Packaging = product.Packaging,
//                     }
//                 };

//            #endregion


//            #endregion

//            // 获取符合条件的ID            
//            var ienum_myReport = iquery.ToArray();
//            var tinyIds = ienum_myReport.Select(item => item.TinyID).Distinct();

//            var ienum_sortings = linq_sortings.Where(item => tinyIds.Contains(item.TinyOrderID)).ToArray();
//            var ienum_pickings = linq_pickings.Where(item => tinyIds.Contains(item.TinyOrderID)).ToArray();
//            var ienum_merges = ienum_sortings.Concat(ienum_pickings);

//            var linq = from report in ienum_myReport
//                       join merge in ienum_merges on report.TinyID equals merge.TinyOrderID into merges
//                       select new
//                       {
//                           report.TinyID,
//                           report.DeclareStatus,
//                           items = merges.Select(item => new
//                           {
//                               item.StorageProcut,
//                               item.Operation,
//                               item.Info
//                           })
//                       };


//            // 为了计算并添加LQuantity
//            var results = linq;

//            if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
//            {
//                return results.Select(item =>
//                {
//                    object o = item;
//                    return o;
//                }).ToArray();
//            }

//            return new
//            {
//                Total = total,
//                Size = pageSize ?? 20,
//                Index = pageIndex ?? 1,
//                Data = results.ToArray(),
//            };
//        }

//        #region 搜索方法

//        string wareHouseID;

//        ///// <summary>
//        ///// 根据库房ID搜索
//        ///// </summary>
//        ///// <param name="warehouseID"></param>
//        ///// <returns></returns>
//        //public CgReportsView_bak SearchByWareHouseID(string wareHouseID)
//        //{
//        //    this.wareHouseID = wareHouseID;
//        //    var waybillView = this.IQueryable.Cast<MyReport>();

//        //    if (string.IsNullOrWhiteSpace(this.wareHouseID))
//        //    {
//        //        throw new ArgumentNullException(nameof(wareHouseID), "参数必须要有有效值");
//        //    }

//        //    var linq = from notice in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
//        //               join waybill in waybillView on notice.WaybillID equals waybill.ID
//        //               where notice.WareHouseID == this.wareHouseID
//        //               select waybill;

//        //    var view = new CgReportsView_bak(this.Reponsitory, linq)
//        //    {
//        //        wareHouseID = this.wareHouseID,
//        //    };

//        //    return view;
//        //}

//        /// <summary>
//        /// 根据状态查询 
//        /// </summary>
//        /// <param name="status">状态</param>
//        /// <returns></returns>
//        public CgReportsView_bak SearchByStatus(TinyOrderDeclareStatus status)
//        {
//            var linq = this.IQueryable.Cast<MyReport>().Where(item => item.DeclareStatus == status);

//            var view = new CgReportsView_bak(this.Reponsitory, linq)
//            {
//                wareHouseID = this.wareHouseID,
//            };

//            return view;
//        }

//        /// <summary>
//        /// 根据包装人（装箱人）
//        /// </summary>
//        /// <param name="adminRealName">装箱人真实姓名</param>
//        /// <returns>装箱人的视图</returns>
//        public CgReportsView_bak SearchByPacker(string adminRealName)
//        {
//            var adminTopView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.AdminsTopView>();
//            var linq1 = from report in this.IQueryable.Cast<MyReport>()
//                        join input in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Inputs>() on report.TinyID equals input.TinyOrderID
//                        join sorting in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Sortings>() on input.ID equals sorting.InputID
//                        join admin in adminTopView on sorting.AdminID equals admin.ID //分拣人员
//                        where admin.RealName.Contains(adminRealName)
//                        select report.TinyID;

//            var linq2 = from report in this.IQueryable.Cast<MyReport>()
//                        join output in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Outputs>() on report.TinyID equals output.TinyOrderID
//                        join picking in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Pickings>() on output.ID equals picking.OutputID
//                        join admin in adminTopView on picking.AdminID equals admin.ID //分拣人员
//                        where admin.RealName.Contains(adminRealName)
//                        select report.TinyID;

//            linq1 = linq1.Distinct().OrderByDescending(item => item).Take(500);
//            linq2 = linq2.Distinct().OrderByDescending(item => item).Take(500);


//            var linq = from tinyOrderId in linq1.Concat(linq2)
//                       join log in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Logs_Declare>() on tinyOrderId equals log.TinyOrderID
//                       orderby log.CreateDate
//                       select new MyReport
//                       {
//                           TinyID = tinyOrderId,
//                           DeclareStatus = (TinyOrderDeclareStatus)log.Status,
//                       };

//            var view = new CgReportsView_bak(this.Reponsitory, linq)
//            {
//                wareHouseID = this.wareHouseID,
//            };
//            return view;
//        }


//        /// <summary>
//        /// 根据库房ID搜索
//        /// </summary>
//        /// <param name="txt">箱号、型号、品牌</param>
//        /// <returns></returns>
//        public CgReportsView_bak SearchByFirst(string txt)
//        {
//            var adminTopView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.AdminsTopView>();

//            var linq1 = from report in this.IQueryable.Cast<MyReport>()
//                        join input in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Inputs>() on report.TinyID equals input.TinyOrderID
//                        join storage in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>() on input.ID equals storage.InputID
//                        join product in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.ProductsTopView>() on storage.ProductID equals product.ID
//                        join sorting in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Sortings>() on input.ID equals sorting.InputID
//                        where product.PartNumber.Contains(txt) || product.Manufacturer.Contains(txt) || sorting.BoxCode.Contains(txt)
//                        select report.TinyID;

//            var linq2 = from report in this.IQueryable.Cast<MyReport>()
//                        join output in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Outputs>() on report.TinyID equals output.TinyOrderID
//                        join storage in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>() on output.StorageID equals storage.ID
//                        join product in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.ProductsTopView>() on storage.ProductID equals product.ID
//                        join picking in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Pickings>() on output.ID equals picking.OutputID
//                        where product.PartNumber.Contains(txt) || product.Manufacturer.Contains(txt) || picking.BoxCode.Contains(txt)
//                        select report.TinyID;

//            linq1 = linq1.Distinct().OrderByDescending(item => item).Take(500);
//            linq2 = linq2.Distinct().OrderByDescending(item => item).Take(500);


//            var linq = from tinyOrderId in linq1.Concat(linq2)
//                       join log in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Logs_Declare>() on tinyOrderId equals log.TinyOrderID
//                       orderby log.CreateDate
//                       select new MyReport
//                       {
//                           TinyID = tinyOrderId,
//                           DeclareStatus = (TinyOrderDeclareStatus)log.Status,
//                       };

//            var view = new CgReportsView_bak(this.Reponsitory, linq)
//            {
//                wareHouseID = this.wareHouseID,
//            };
//            return view;
//        }

//        #endregion

//        #region Helper Class
//        /// <summary>
//        /// 符合Sorting视图头部定义的内部类
//        /// </summary>
//        private class MyReport
//        {
//            public string TinyID { get; set; }
//            public TinyOrderDeclareStatus DeclareStatus { get; set; }
//        }
//        #endregion
//    }
//}
