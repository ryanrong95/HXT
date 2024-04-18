/*
 没有用废弃
 */

using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Yahv.Linq;
using Yahv.Linq.Extends;
using Yahv.Services.Enums;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Utils.Serializers;

namespace Yahv.Services.Views
{
    /// <summary>
    /// 入库报表视图
    /// </summary>
    public class CgInputReportsView : QueryView<object, PvWmsRepository>
    {
        private string wareHouseID;

        #region 构造函数

        public CgInputReportsView()
        {

        }

        protected CgInputReportsView(PvWmsRepository reponsitory) : base(reponsitory)
        {
        }

        protected CgInputReportsView(PvWmsRepository reponsitory, IQueryable<object> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        #endregion

        protected override IQueryable<object> GetIQueryable()
        {
            var adminTopView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.AdminsTopView>();

            var linq = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.CgInputReportTopView>()
                       join admin in adminTopView on entity.AdminID equals admin.ID
                       select new InputReport
                       {
                           SortingID = entity.SortingID,
                           StorageID = entity.StorageID,
                           InputID = entity.InputID,
                           ItemID = entity.ItemID,
                           OrderID = entity.OrderID,
                           TinyOrderID = entity.TinyOrderID,
                           WaybillID = entity.WaybillID,
                           wbCode = entity.wbCode,
                           CarrierName = entity.CarrierName,
                           CarrierID = entity.CarrierID,
                           NoticeID = entity.NoticeID,
                           WareHouseID = entity.WareHouseID,
                           ClientID = entity.ClientID,
                           ClientName = entity.Name,
                           EnterCode = entity.EnterCode,

                           EnterQuantity = entity.EnterQuantity,//入库数量
                           EnterDate = entity.EnterDate,//入库日期
                           NoticeQuantity = entity.NoticeQuantity,//通知数量
                           NoticeDate = entity.NoticeDate,//通知日期
                           Source = entity.Source,//业务来源

                           Supplier = entity.Supplier,
                           SortingAdminID = entity.AdminID,//入库分拣人员
                           SortingAdminRealName = admin.RealName,
                           Summary = entity.Summary,//异常信息
                           ShelveID = entity.ShelveID,
                           Product = new CenterProduct
                           {
                               ID = entity.ProductID,
                               PartNumber = entity.PartNumber,
                               Manufacturer = entity.Manufacturer,
                           },
                           Origin = entity.Origin,
                           DateCode = entity.DateCode,
                           CustomsName = entity.CustomsName,
                           Conditions = entity.Conditions,
                           UnitPrice = entity.UnitPrice,
                           Currency = entity.Currency,
                           BoxCode = entity.BoxCode,
                           Weight = entity.Weight,
                           Volume = entity.Volume,
                           DelcareStatus = entity.DelcareStatus,
                           Display = (entity.DelcareStatus.HasValue && entity.DelcareStatus >= 30) ? false : true,
                       };
            return linq;
        }

        #region 数据分组
        /// <summary>
        /// 根据InputID进行入库单分组
        /// </summary>
        /// <param name="status">是否正常入库，默认正常入库</param>
        /// <returns></returns>
        public object ToGroupByInputID(bool status = true)
        {
            var items = this.IQueryable.Cast<InputReport>().ToArray();

            var linq = from item in items
                       group item by new
                       {
                           item.InputID,
                       } into groups
                       let single = groups.FirstOrDefault(item => item.InputID == groups.Key.InputID)
                       select new
                       {
                           InputID = single.InputID,
                           WareHouseID = single.WareHouseID,
                           NoticeID = single.NoticeID,
                           YdQty = single.NoticeQuantity,
                           SdQty = groups.Sum(item => item.EnterQuantity),

                           OrderID = single.OrderID,
                           TinyOrderID = single.TinyOrderID,
                           ClientID = single.ClientID,
                           ClientName = single.ClientName,
                           Supplier = single.Supplier,

                           EnterCode = single.EnterCode,
                           NoticeDate = single.NoticeDate,
                           Source = single.Source,
                           LastEnterDate = groups.Max(item => item.EnterDate),
                           PartNumber = single.Product.PartNumber,

                           Datas = groups.Select(item => new
                           {
                               SortingID = item.SortingID,
                               StorageID = item.StorageID,
                               WaybillID = item.WaybillID,
                               wbCode = item.wbCode,
                               CarrierName = item.CarrierName,
                               Product = item.Product,
                               Origin = ((Origin)Enum.Parse(typeof(Origin), item.Origin ?? nameof(Origin.Unknown))).GetDescription(),
                               DateCode = item.DateCode,
                               CustomsName = item.CustomsName,
                               Quantity = item.EnterQuantity,//本次入库数量
                               BoxCode = item.BoxCode,
                               Weight = item.Weight,
                               Volume = item.Volume,
                               EnterDate = item.EnterDate,
                               UnitPrice = item.UnitPrice,
                               Currency = ((Currency)(item.Currency.HasValue ? item.Currency.Value : 0)).GetCurrency().ShortName,
                               SortingAdminID = item.SortingAdminID,
                               SortingAdminRealName = item.SortingAdminRealName,
                               Summary = item.Summary,
                               item.DelcareStatus,
                               item.Display,
                           }).OrderByDescending(item => item.EnterDate),
                       };

            if (status)
            {
                return linq.Where(item => item.NoticeID != null).Where(item => item.YdQty >= item.SdQty && item.Datas.All(s => string.IsNullOrEmpty(s.Summary))).OrderByDescending(item => item.LastEnterDate).ToArray();
            }
            else
            {
                return linq.Where(item => item.NoticeID == null || (item.NoticeID != null && item.YdQty < item.SdQty) || (item.NoticeID != null && item.Datas.Any(s => !string.IsNullOrEmpty(s.Summary)))).OrderByDescending(item => item.LastEnterDate).ToArray();
            }
        }

        /// <summary>
        /// 根据InputID进行分组，传入的参数为当前的视图
        /// </summary>
        /// <param name="reports">参数为CgInputReportsView视图的实例</param>
        /// <returns></returns>
        public object ToGroupByInput(CgInputReportsView reports, bool status = true)
        {
            var items = reports.Cast<InputReport>().ToArray();

            var linq = from item in items
                       group item by new
                       {
                           item.InputID,
                       } into groups
                       let single = groups.FirstOrDefault(item => item.InputID == groups.Key.InputID)
                       select new
                       {
                           InputID = single.InputID,
                           WareHouseID = single.WareHouseID,
                           NoticeID = single.NoticeID,
                           YdQty = single.NoticeQuantity,
                           SdQty = groups.Sum(item => item.EnterQuantity),

                           OrderID = single.OrderID,
                           TinyOrderID = single.TinyOrderID,
                           ClientID = single.ClientID,
                           ClientName = single.ClientName,
                           Supplier = single.Supplier,

                           EnterCode = single.EnterCode,
                           UnitPrice = single.UnitPrice,
                           Currency = single.Currency,
                           NoticeDate = single.NoticeDate,
                           Source = single.Source,
                           LastEnterDate = groups.Max(item => item.EnterDate),
                           PartNumber = single.Product.PartNumber,

                           TotalWeight = groups.Sum(item => item.Weight),
                           BoxCode = groups.FirstOrDefault().BoxCode,
                           Datas = groups.Select(item => new
                           {
                               SortingID = item.SortingID,
                               StorageID = item.StorageID,
                               WaybillID = item.WaybillID,
                               wbCode = item.wbCode,
                               CarrierName = item.CarrierName,

                               Product = item.Product,
                               //Enum.Parse(typeof(Origin), item.Origin ?? nameof(Origin.Unknown)).ToString(),
                               Origin = ((Origin)Enum.Parse(typeof(Origin), (item.Origin ?? nameof(Origin.Unknown)))).GetDescription(),
                               DateCode = item.DateCode,
                               CustomsName = item.CustomsName,
                               Quantity = item.EnterQuantity,//本次入库数量
                               BoxCode = item.BoxCode,

                               item.ShelveID,
                               Weight = item.Weight,
                               Volume = item.Volume,
                               EnterDate = item.EnterDate,
                               SortingAdminID = item.SortingAdminID,
                               Summary = item.Summary,
                               item.DelcareStatus,
                               item.Display,
                               Conditions = string.IsNullOrEmpty(item.Conditions)? null : item.Conditions.JsonTo<NoticeCondition>(),

                           }),
                       };

            if (status)
            {                
                var linq_normal = linq.Where(item => item.NoticeID != null).Where(item => item.YdQty >= item.SdQty && item.Datas.All(s => string.IsNullOrEmpty(s.Summary))).ToArray();
                var TotalWeight = linq_normal.Sum(item => item.TotalWeight);
                var TotalPart = GetTotalPart(linq_normal.Where(item => item.BoxCode != null).Select(item => item.BoxCode).Distinct());
                var TotalQuantity = linq_normal.Sum(item => item.SdQty);

                return new
                {
                    TotalWeight = TotalWeight,
                    TotalPart = TotalPart,
                    TotalQuantity = TotalQuantity,
                    Normal = linq_normal,
                };
                //return linq.Where(item => item.NoticeID != null).Where(item => item.YdQty >= item.SdQty && item.Datas.All(s => string.IsNullOrEmpty(s.Summary))).ToArray();
                               
            }
            else
            {
                var linq_abnormal = linq.Where(item => item.NoticeID == null || (item.NoticeID != null && item.YdQty < item.SdQty) || (item.NoticeID != null && item.Datas.Any(s => !string.IsNullOrEmpty(s.Summary)))).ToArray();
                //return linq.Where(item => item.NoticeID == null || (item.NoticeID != null && item.YdQty < item.SdQty) || (item.NoticeID != null && item.Datas.Any(s => !string.IsNullOrEmpty(s.Summary)))).ToArray();
                var TotalWeight = linq_abnormal.Sum(item => item.TotalWeight);
                var TotalPart = GetTotalPart(linq_abnormal.Where(item => item.BoxCode != null).Select(item => item.BoxCode).Distinct());
                var TotalQuantity = linq_abnormal.Sum(item => item.SdQty);

                return new
                {
                    TotalWeight = TotalWeight,
                    TotalPart = TotalPart,
                    TotalQuantity = TotalQuantity,
                    Abnormal = linq_abnormal,
                };
            }
        }


        /// <summary>
        /// 根据TinyOrderID进行分组，传入的参数为当前的视图
        /// </summary>
        /// <param name="reports">参数为CgInputReportsView视图的实例</param>
        /// <returns></returns>
        public object ToGroupByTinyOrderID(CgInputReportsView reports)
        {
            var items = reports.Cast<InputReport>().ToArray();
            var avgWeightView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.PartNumberAvgWeightsTopView>();
            var linq = from single in items
                       join _avgWeight in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.PartNumberAvgWeightsTopView>() on single.Product.PartNumber equals _avgWeight.PartNumber into avgWeights
                       from avgWeight in avgWeights.DefaultIfEmpty()
                       select new
                       {
                           InputID = single.InputID,
                           WareHouseID = single.WareHouseID,
                           NoticeID = single.NoticeID,
                           YdQty = single.NoticeQuantity,
                           SdQty = single.EnterQuantity,

                           OrderID = single.OrderID,
                           ItemID = single.ItemID,
                           TinyOrderID = single.TinyOrderID,
                           ClientID = single.ClientID,
                           ClientName = single.ClientName,
                           Supplier = single.Supplier,

                           EnterCode = single.EnterCode,
                           UnitPrice = single.UnitPrice,
                           Currency = single.Currency,
                           NoticeDate = single.NoticeDate,
                           Source = single.Source,
                           LastEnterDate = single.EnterDate,
                           PartNumber = single.Product.PartNumber,

                           TotalWeight = single.Weight,
                           BoxCode = single.BoxCode,

                           SortingID = single.SortingID,
                           StorageID = single.StorageID,
                           WaybillID = single.WaybillID,
                           wbCode = single.wbCode,
                           CarrierName = single.CarrierName,
                           CarrierID = single.CarrierID,

                           Product = single.Product,
                           //Enum.Parse(typeof(Origin), item.Origin ?? nameof(Origin.Unknown)).ToString(),
                           Origin = ((Origin)Enum.Parse(typeof(Origin), (single.Origin ?? nameof(Origin.Unknown)))).GetDescription(),
                           DateCode = single.DateCode,
                           CustomsName = single.CustomsName,
                           Quantity = single.EnterQuantity,//本次入库数量

                           single.ShelveID,
                           Weight = single.Weight,
                           AvgWeight = avgWeight == null? 0 : avgWeight.AVGWeight * single.EnterQuantity,
                           Volume = single.Volume,
                           EnterDate = single.EnterDate,
                           SortingAdminID = single.SortingAdminID,
                           Summary = single.Summary,
                           single.DelcareStatus,
                           single.Display,
                           Conditions = string.IsNullOrEmpty(single.Conditions)? null : single.Conditions.JsonTo<NoticeCondition>(),
                           IsAbleDeclare = single.Display,
                       };


            
            var linq_normal = linq.Where(item => item.NoticeID != null).Where(item => item.YdQty >= item.SdQty && string.IsNullOrEmpty(item.Summary)).ToArray();
            var TotalWeight = linq_normal.Sum(item => item.TotalWeight);
            var TotalAvgWeight = linq_normal.Sum(item => item.AvgWeight);
            var TotalPart = GetTotalPart(linq_normal.Where(item => item.BoxCode != null).Select(item => item.BoxCode).Distinct());
            var TotalQuantity = linq_normal.Sum(item => item.SdQty);
            var IsDisabledDeclareBtn = linq_normal.Any(item => item.Display == true) ? false : true;

            var TinyOrderID_Group = from item in linq_normal
                                    group item by new
                                    {
                                        item.TinyOrderID,
                                        item.IsAbleDeclare,
                                    } into itemgroups
                                    select new
                                    {
                                        TinyOrderID = itemgroups.Key.TinyOrderID,
                                        IsAbleDeclare = itemgroups.Key.IsAbleDeclare,
                                        _disabled = !itemgroups.Key.IsAbleDeclare,
                                        TotalWeight = itemgroups.Sum(t => t.Weight),
                                        TotalAvgWeight = itemgroups.Sum(t => t.AvgWeight),                                        
                                        Groups = itemgroups.Select(entity => new
                                        {
                                            entity.InputID,
                                            entity.ItemID,
                                            entity.WareHouseID,
                                            entity.NoticeID,
                                            entity.YdQty,
                                            entity.SdQty,

                                            entity.OrderID,
                                            entity.ClientID,
                                            entity.ClientName,
                                            entity.Supplier,

                                            entity.EnterCode,
                                            entity.UnitPrice,
                                            entity.Currency,
                                            entity.NoticeDate,
                                            entity.Source,
                                            entity.LastEnterDate,
                                            entity.PartNumber,

                                            entity.TotalWeight,
                                            entity.BoxCode,

                                            entity.SortingID,
                                            entity.StorageID,
                                            entity.WaybillID,
                                            entity.wbCode,
                                            entity.CarrierName,
                                            entity.CarrierID,

                                            entity.Product,

                                            entity.Origin,
                                            entity.DateCode,
                                            entity.CustomsName,
                                            entity.Quantity,//本次入库数量

                                            entity.ShelveID,
                                            entity.Weight,
                                            entity.AvgWeight,
                                            entity.Volume,
                                            entity.EnterDate,
                                            entity.SortingAdminID,
                                            entity.Summary,
                                            entity.DelcareStatus,
                                            entity.Display,
                                            entity.Conditions,
                                        }).OrderByDescending(s => s.BoxCode)
                                    };

            return new
            {
                TotalWeight = TotalWeight,
                TotalAvgWeight=TotalAvgWeight,
                TotalPart = TotalPart,
                TotalQuantity = TotalQuantity,
                IsDisabledDeclareBtn = IsDisabledDeclareBtn,
                TinyOrderID_Group,
            };
            
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <param name="status">是否正常入库，默认正常入库</param>
        /// <param name="pageIndex">当前页面的Index, 默认值1</param>
        /// <param name="pageSize">当前页面显示的数据数量，默认20</param>
        /// <returns></returns>
        public object ToMyPage(bool status = true, int? pageIndex = 1, int? pageSize = 20)
        {
            var iquery = this.ToGroupByInputID(status) as object[];
            int total = iquery.Count();

            iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value).ToArray();

            return new
            {
                Total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                Data = iquery,
            };
        }

        #endregion

        #region 查询条件搜索

        /// <summary>
        /// 库房ID
        /// </summary>
        /// <param name="warehouseID"></param>
        /// <returns></returns>
        public CgInputReportsView SearchByWareHouseID(string wareHouseID)
        {
            this.wareHouseID = wareHouseID;
            var inputReportView = this.IQueryable.Cast<InputReport>();
            if (string.IsNullOrWhiteSpace(this.wareHouseID))
            {
                return new CgInputReportsView(this.Reponsitory, inputReportView);
            }

            var linq = from entity in inputReportView
                       where entity.WareHouseID.Contains(this.wareHouseID)
                       select entity;
            var view = new CgInputReportsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };
            return view;
        }

        /// <summary>
        /// 订单ID
        /// </summary>
        /// <param name="OrderID"></param>
        /// <returns></returns>
        public CgInputReportsView SearchByOrderID(string OrderID)
        {
            var inputReportView = this.IQueryable.Cast<InputReport>();

            var linq = from entity in inputReportView
                       where entity.OrderID.Contains(OrderID)
                       select entity;

            var view = new CgInputReportsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };
            return view;
        }

        /// <summary>
        /// 小订单号
        /// </summary>
        /// <param name="TinyOrderID"></param>
        /// <returns></returns>
        public CgInputReportsView SearchByTinyOrderID(string TinyOrderID)
        {
            var inputReportView = this.IQueryable.Cast<InputReport>();

            var linq = from entity in inputReportView
                       where entity.OrderID.Contains(TinyOrderID)
                       select entity;

            var view = new CgInputReportsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };
            return view;
        }

        /// <summary>
        /// 入库日期
        /// </summary>
        /// <param name="startdate">开始时间</param>
        /// <param name="enddate">结束时间</param>
        /// <returns></returns>
        public CgInputReportsView SearchByEnterDate(DateTime? startdate, DateTime? enddate)
        {
            Expression<Func<InputReport, bool>> predicate = report => (startdate == null ? true : report.EnterDate >= startdate)
                && (enddate == null ? true : report.EnterDate < enddate.Value.AddDays(1));

            var inputReportView = this.IQueryable.Cast<InputReport>();
            var linq = inputReportView.Where(predicate);

            var view = new CgInputReportsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };
            return view;
        }

        /// <summary>
        /// 业务来源
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public CgInputReportsView SearchBySource(params CgNoticeSource[] sources)
        {
            var inputReportView = this.IQueryable.Cast<InputReport>();
            var includes = sources.Select(item => (int?)item);

            var linq = from entity in inputReportView
                       where includes.Contains(entity.Source)
                       select entity;

            var view = new CgInputReportsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };
            return view;
        }

        /// <summary>
        /// 产品型号
        /// </summary>
        /// <param name="PartNumber"></param>
        /// <returns></returns>
        public CgInputReportsView SearchByPartNumber(string PartNumber)
        {
            var inputReportView = this.IQueryable.Cast<InputReport>();

            var linq = from entity in inputReportView
                       where entity.Product.PartNumber.Contains(PartNumber)
                       select entity;

            var view = new CgInputReportsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };
            return view;
        }

        /// <summary>
        /// 产品品牌
        /// </summary>
        /// <param name="Manufacturer"></param>
        /// <returns></returns>
        public CgInputReportsView SearchByManufacturer(string Manufacturer)
        {
            var inputReportView = this.IQueryable.Cast<InputReport>();

            var linq = from entity in inputReportView
                       where entity.Product.Manufacturer.Contains(Manufacturer)
                       select entity;

            var view = new CgInputReportsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };
            return view;
        }

        /// <summary>
        /// 原产地
        /// </summary>
        /// <param name="origins"></param>
        /// <returns></returns>
        public CgInputReportsView SearchByOrigin(params Origin[] origins)
        {
            var inputReportView = this.IQueryable.Cast<InputReport>();
            var includes = origins.Select(item => item.GetOrigin().Code);

            var linq = from entity in inputReportView
                       where includes.Contains(entity.Origin)
                       select entity;

            var view = new CgInputReportsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };
            return view;
        }

        /// <summary>
        /// 入库币种
        /// </summary>
        /// <param name="currencys"></param>
        /// <returns></returns>
        public CgInputReportsView SearchByCurrency(params Currency[] currencys)
        {
            var inputReportView = this.IQueryable.Cast<InputReport>();
            var includes = currencys.Select(item => (int?)item);

            var linq = from entity in inputReportView
                       where includes.Contains(entity.Currency)
                       select entity;

            var view = new CgInputReportsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };
            return view;
        }

        /// <summary>
        /// 供应商名称
        /// </summary>
        /// <param name="Supplier"></param>
        /// <returns></returns>
        public CgInputReportsView SearchBySupplier(string Supplier)
        {
            var inputReportView = this.IQueryable.Cast<InputReport>();

            var linq = from entity in inputReportView
                       where entity.Supplier.Contains(Supplier)
                       select entity;

            var view = new CgInputReportsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };
            return view;
        }

        /// <summary>
        /// 客户ID
        /// </summary>
        /// <param name="ClientID"></param>
        /// <returns></returns>
        public CgInputReportsView SearchByClientID(string ClientID)
        {
            var inputReportView = this.IQueryable.Cast<InputReport>();

            var linq = from entity in inputReportView
                       where entity.ClientID == ClientID
                       select entity;

            var view = new CgInputReportsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };
            return view;
        }

        /// <summary>
        /// 客户名称
        /// </summary>
        /// <param name="ClientName"></param>
        /// <returns></returns>
        public CgInputReportsView SearchByClientName(string ClientName)
        {
            var inputReportView = this.IQueryable.Cast<InputReport>();

            var linq = from entity in inputReportView
                       where entity.ClientName.Contains(ClientName)
                       select entity;

            var view = new CgInputReportsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };
            return view;
        }

        /// <summary>
        /// 客户入仓号
        /// </summary>
        /// <param name="EnterCode"></param>
        /// <returns></returns>
        public CgInputReportsView SearchByEnterCode(string EnterCode)
        {
            var inputReportView = this.IQueryable.Cast<InputReport>();

            var linq = from entity in inputReportView
                       where entity.EnterCode == EnterCode
                       select entity;

            var view = new CgInputReportsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };
            return view;
        }

        /// <summary>
        /// 根据分拣人姓名查询入库信息
        /// </summary>
        /// <param name="RealName"></param>
        /// <returns></returns>
        public CgInputReportsView SearchByAdminName(string RealName)
        {
            var inputReportView = this.IQueryable.Cast<InputReport>();

            var linq = from entity in inputReportView
                       where entity.SortingAdminRealName == RealName
                       select entity;
            var view = new CgInputReportsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };
            return view;
        }

        #endregion

        /// <summary>
        /// 为避免重复使用正则表达式用切割
        /// </summary>
        static Regex regex_number = new Regex(@"^(\D*)(\d+)$", RegexOptions.Singleline);

        /// <summary>
        /// 为避免重复使用正则表达式用切割
        /// </summary>
        static Regex regex_boxPrex = new Regex(@"^\[\d{8}\]", RegexOptions.Singleline);

        /// <summary>
        /// 填充箱号
        /// </summary>
        /// <param name="boxcodes"></param>
        /// <returns></returns>
        /// <remarks>
        /// 配合hash 自动排重
        /// </remarks>
        static public int GetTotalPart(IEnumerable<string> boxcodes)
        {
            if (boxcodes.Count() == 0)
            {
                return 0;
            }

            Dictionary<string, HashSet<string>> dic = new Dictionary<string, HashSet<string>>();

            foreach (var code in boxcodes)
            {
                string date;
                string context = code;
                if (regex_boxPrex.IsMatch(code))
                {
                    date = code.Substring(0, 10);
                    context = code.Substring(10);
                }
                else
                {
                    date = DateTime.Now.ToString("yyyyMMdd");
                    context = code;
                }

                HashSet<string> sets;
                if (!dic.TryGetValue(date, out sets))
                {
                    sets = dic[date] = new HashSet<string>();
                }

                var splits = context.Split('-');
                if (splits.Length == 1)
                {
                    sets.Add(context);
                    continue;
                }

                //以组的方式主要是利用切断

                var prex = regex_number.Match(splits.First()).Groups[1].Value;

                var firstTxt = regex_number.Match(splits.First()).Groups[2].Value;
                var lastTxt = regex_number.Match(splits.Last()).Groups[2].Value;

                var lastPrex = regex_number.Match(splits.Last()).Groups[1].Value;
                if (string.IsNullOrWhiteSpace(lastPrex))
                {
                    sets.Add(prex + firstTxt);
                    continue;
                }

                var first = int.Parse(firstTxt);
                var last = int.Parse(lastTxt);

                var pad = Math.Min(firstTxt.Length, lastTxt.Length);

                //这样写主要就是为可以报错！
                for (int index = first; index <= last; index++)
                {
                    sets.Add(prex + index.ToString().PadLeft(pad, '0'));
                }
            }

            return dic.SelectMany(item => item.Value).Count();
        }

        #region 帮助类

        private class InputReport
        {
            public string SortingID { get; set; }
            public string StorageID { get; set; }
            public string InputID { get; set; }
            public string ItemID { get; set; }
            public string OrderID { get; set; }
            public string TinyOrderID { get; set; }
            public string WaybillID { get; set; }
            public string NoticeID { get; set; }
            public string WareHouseID { get; set; }
            public string ClientID { get; set; }
            public string ClientName { get; set; }
            /// <summary>
            /// 入仓号
            /// </summary>
            public string EnterCode { get; set; }
            public CenterProduct Product { get; set; }
            /// <summary>
            /// 入库单价
            /// </summary>
            public decimal? UnitPrice { get; set; }
            /// <summary>
            /// 币种
            /// </summary>
            public int? Currency { get; set; }
            /// <summary>
            /// 原产地
            /// </summary>
            public string Origin { get; set; }
            /// <summary>
            /// 批次号
            /// </summary>
            public string DateCode { get; set; }
            /// <summary>
            /// 通知数量
            /// </summary>
            public decimal? NoticeQuantity { get; set; }
            /// <summary>
            /// 通知日期
            /// </summary>
            public DateTime? NoticeDate { get; set; }
            /// <summary>
            /// 入库来源
            /// </summary>
            public int? Source { get; set; }
            /// <summary>
            /// 入库数量
            /// </summary>
            public decimal? EnterQuantity { get; set; }
            /// <summary>
            /// 入库日期
            /// </summary>
            public DateTime EnterDate { get; set; }
            /// <summary>
            /// 供应商
            /// </summary>
            public string Supplier { get; set; }
            /// <summary>
            /// 分拣人
            /// </summary>
            public string SortingAdminID { get; set; }
            /// <summary>
            /// 分拣人真实姓名
            /// </summary>
            public string SortingAdminRealName { get; set; }
            /// <summary>
            /// 异常信息
            /// </summary>
            public string Summary { get; set; }
            /// <summary>
            /// 报关品名
            /// </summary>
            public string CustomsName { get; set; }
            /// <summary>
            /// 产品标识
            /// </summary>
            public string Conditions { get; set; }
            /// <summary>
            /// 箱号
            /// </summary>
            /// <remarks>
            /// 依照马莲建议，增加库位展示
            /// </remarks>
            public string BoxCode { get; set; }
            public decimal? Weight { get; set; }
            public decimal? Volume { get; set; }

            /// <summary>
            /// 库位
            /// </summary>
            /// <remarks>
            /// 依照马莲建议，增加库位展示
            /// </remarks>
            public string ShelveID { get; set; }

            /// <summary>
            /// 
            /// </summary>
            /// <remarks>
            /// 请参考pvwms 中的 枚举 TinyOrderDeclareStatus 设定，通用本地就不增加申报状态
            /// </remarks>
            public int? DelcareStatus { get; set; }
            /// <summary>
            /// 是否显示删除按钮
            /// </summary>
            public bool Display { get; set; }

            /// <summary>
            /// 运单ID
            /// </summary>
            public string wbCode { get; set; }

            /// <summary>
            /// 承运商Name
            /// </summary>
            public string CarrierName { get; set; }

            /// <summary>
            /// 承运商ID
            /// </summary>
            public string CarrierID { get; set; }
        }

        #endregion
    }

    public class CgCustomsStorageReportsView : QueryView<object, PvWmsRepository>
    {
        //private string wareHouseID;

        #region 构造函数
        public CgCustomsStorageReportsView()
        {
        }

        protected CgCustomsStorageReportsView(PvWmsRepository reponsitory)
        {
        }

        protected CgCustomsStorageReportsView(PvWmsRepository reponsitory, IQueryable<object> iQueryable) :base(reponsitory, iQueryable)       
        {

        }
        #endregion

        protected override IQueryable<object> GetIQueryable()
        {   var adminTopView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.AdminsTopView>();

            var linq = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.CgCustomStoragePickingReportTopView>()
                       join admin in adminTopView on entity.AdminID equals admin.ID
                       select new CustomsStorageReport
                       {
                           PickingID = entity.PickingID,
                           StorageID = entity.StorageID,
                           InputID = entity.InputID,
                           OutputID = entity.OutputID,
                           ItemID = entity.ItemID,
                           OrderID = entity.OrderID,
                           TinyOrderID = entity.TinyOrderID,
                           WaybillID = entity.WaybillID,
                           NoticeID = entity.NoticeID,
                           ClientName = entity.ClientName,
                           EnterCode = entity.EnterCode,
                                                      
                           EnterQuantity = entity.EnterQuantity,//入库数量
                           EnterDate = entity.EnterDate,//入库日期
                           NoticeQuantity = entity.NoticeQuantity,//通知数量
                           NoticeDate = entity.NoticeDate,//通知日期
                           Source = entity.Source,//业务来源
                           
                           Supplier = entity.Supplier,                           
                           PickingAdminID = entity.AdminID,//入库分拣人员
                           PickingAdminRealName = admin.RealName,
                           Summary = entity.Summary,//异常信息
                           ShelveID = entity.ShelveID,
                           Product = new CenterProduct
                           {
                               ID = entity.ProductID,
                               PartNumber = entity.PartNumber,
                               Manufacturer = entity.Manufacturer,
                           },
                           Origin = entity.Origin,
                           DateCode = entity.DateCode,
                           CustomsName = entity.CustomsName,
                           Conditions = entity.Conditions,
                           Price = entity.Price,
                           Currency = entity.Currency,
                           BoxCode = entity.BoxCode,
                           Weight = entity.Weight,
                           Volume = entity.Volume,
                           DelcareStatus = entity.DelcareStatus,
                           Display = (entity.DelcareStatus.HasValue && entity.DelcareStatus >= 30) ? false : true,
                       };
            return linq;
        }

        /// <summary>
        /// 根据OutputID进行入库单分组
        /// </summary>
        /// <param name="status">是否正常入库，默认正常入库</param>
        /// <returns></returns>
        public object ToGroupByOutputID(bool status = true)
        {
            var items = this.IQueryable.Cast<CustomsStorageReport>().ToArray();

            var linq = from item in items
                       group item by new
                       {
                           item.OutputID,
                       } into groups
                       let single = groups.FirstOrDefault(item => item.OutputID == groups.Key.OutputID)
                       select new
                       {
                           InputID = single.InputID,
                           OutputID = single.OutputID,
                           NoticeID = single.NoticeID,
                           YdQty = single.NoticeQuantity,
                           SdQty = groups.Sum(item => item.EnterQuantity),

                           OrderID = single.OrderID,
                           TinyOrderID = single.TinyOrderID,
                           ClientName = single.ClientName,
                           Supplier = single.Supplier,

                           EnterCode = single.EnterCode,
                           NoticeDate = single.NoticeDate,
                           Source = single.Source,
                           LastEnterDate = groups.Max(item => item.EnterDate),
                           PartNumber = single.Product.PartNumber,

                           Datas = groups.Select(item => new
                           {
                               PickingID = item.PickingID,
                               StorageID = item.StorageID,
                               WaybillID = item.WaybillID,
                               Product = item.Product,
                               Origin = ((Origin)Enum.Parse(typeof(Origin), item.Origin ?? nameof(Origin.Unknown))).GetDescription(),
                               DateCode = item.DateCode,
                               CustomsName = item.CustomsName,
                               Quantity = item.EnterQuantity,//本次入库数量
                               BoxCode = item.BoxCode,
                               Weight = item.Weight,
                               Volume = item.Volume,
                               EnterDate = item.EnterDate,
                               Price = item.Price,
                               Currency = ((Currency)(item.Currency.HasValue ? item.Currency.Value : 0)).GetCurrency().ShortName,
                               PickingAdminID = item.PickingAdminID,
                               PickingAdminRealName = item.PickingAdminRealName,
                               Summary = item.Summary,
                               item.DelcareStatus,
                               item.Display,
                           }).OrderByDescending(item => item.EnterDate),
                       };

            if (status)
            {
                return linq.Where(item => item.NoticeID != null).Where(item => item.YdQty >= item.SdQty && item.Datas.All(s => string.IsNullOrEmpty(s.Summary))).OrderByDescending(item => item.LastEnterDate).ToArray();
            }
            else
            {
                return linq.Where(item => item.NoticeID == null || (item.NoticeID != null && item.YdQty < item.SdQty) || (item.NoticeID != null && item.Datas.Any(s => !string.IsNullOrEmpty(s.Summary)))).OrderByDescending(item => item.LastEnterDate).ToArray();
            }
        }

        /// <summary>
        /// 根据TinyOrderID进行分组，传入的参数为当前的视图
        /// </summary>
        /// <param name="reports">CgCustomsStorageReportsView</param>
        /// <returns></returns>
        public object ToGroupByTinyOrderID(CgCustomsStorageReportsView reports)
        {
            var items = reports.Cast<CustomsStorageReport>().ToArray();
            var avgWeightView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.PartNumberAvgWeightsTopView>();
            var linq = from single in items
                       join _avgWeight in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.PartNumberAvgWeightsTopView>() on single.Product.PartNumber equals _avgWeight.PartNumber into avgWeights
                       from avgWeight in avgWeights.DefaultIfEmpty()
                       select new
                       {
                           InputID = single.InputID,
                           NoticeID = single.NoticeID,
                           YdQty = single.NoticeQuantity,
                           SdQty = single.EnterQuantity,

                           OrderID = single.OrderID,
                           ItemID = single.ItemID,
                           TinyOrderID = single.TinyOrderID,
                           ClientName = single.ClientName,
                           EnterCode = single.EnterCode,
                           Supplier = single.Supplier,

                           UnitPrice = single.Price,
                           Currency = single.Currency,
                           NoticeDate = single.NoticeDate,
                           Source = single.Source,
                           LastEnterDate = single.EnterDate,
                           PartNumber = single.Product.PartNumber,

                           TotalWeight = single.Weight,
                           BoxCode = single.BoxCode,

                           PickingID = single.PickingID,
                           StorageID = single.StorageID,
                           WaybillID = single.WaybillID,
                           
                           Product = single.Product,
                           //Enum.Parse(typeof(Origin), item.Origin ?? nameof(Origin.Unknown)).ToString(),
                           Origin = ((Origin)Enum.Parse(typeof(Origin), (single.Origin ?? nameof(Origin.Unknown)))).GetDescription(),
                           DateCode = single.DateCode,
                           CustomsName = single.CustomsName,
                           Quantity = single.EnterQuantity,//本次入库数量

                           single.ShelveID,
                           Weight = single.Weight,
                           AvgWeight = avgWeight == null ? 0 : avgWeight.AVGWeight * single.EnterQuantity,
                           Volume = single.Volume,
                           EnterDate = single.EnterDate,
                           PickingAdminID = single.PickingAdminID,
                           PickingAdminRealName = single.PickingAdminRealName,
                           Summary = single.Summary,
                           single.DelcareStatus,
                           single.Display,
                           Conditions = string.IsNullOrEmpty(single.Conditions) ? null : single.Conditions.JsonTo<NoticeCondition>(),
                           IsAbleDeclare = single.Display,
                       };



            var linq_normal = linq.Where(item => item.NoticeID != null).Where(item => item.YdQty >= item.SdQty && string.IsNullOrEmpty(item.Summary)).ToArray();
            var TotalWeight = linq_normal.Sum(item => item.TotalWeight);
            var TotalAvgWeight = linq_normal.Sum(item => item.AvgWeight);
            var TotalPart = CgInputReportsView.GetTotalPart(linq_normal.Where(item => item.BoxCode != null).Select(item => item.BoxCode).Distinct());
            var TotalQuantity = linq_normal.Sum(item => item.SdQty);
            var IsDisabledDeclareBtn = linq_normal.Any(item => item.Display == true) ? false : true;

            var TinyOrderID_Group = from item in linq_normal
                                    group item by new
                                    {
                                        item.TinyOrderID,
                                        item.IsAbleDeclare,
                                    } into itemgroups
                                    select new
                                    {
                                        TinyOrderID = itemgroups.Key.TinyOrderID,
                                        IsAbleDeclare = itemgroups.Key.IsAbleDeclare,
                                        _disabled = !itemgroups.Key.IsAbleDeclare,
                                        TotalWeight = itemgroups.Sum(t => t.Weight),
                                        TotalAvgWeight = itemgroups.Sum(t => t.AvgWeight),
                                        Groups = itemgroups.Select(entity => new
                                        {
                                            entity.InputID,
                                            entity.ItemID,                                            
                                            entity.NoticeID,
                                            entity.YdQty,
                                            entity.SdQty,

                                            entity.OrderID,
                                            entity.EnterCode,
                                            entity.ClientName,
                                            entity.Supplier,
                                                                                       
                                            entity.UnitPrice,
                                            entity.Currency,
                                            entity.NoticeDate,
                                            entity.Source,
                                            entity.LastEnterDate,
                                            entity.PartNumber,

                                            entity.TotalWeight,
                                            entity.BoxCode,

                                            entity.PickingID,
                                            entity.StorageID,
                                            entity.WaybillID,

                                            entity.Product,

                                            entity.Origin,
                                            entity.DateCode,
                                            entity.CustomsName,
                                            entity.Quantity,//本次入库数量

                                            entity.ShelveID,
                                            entity.Weight,
                                            entity.AvgWeight,
                                            entity.Volume,
                                            entity.EnterDate,
                                            entity.PickingAdminID,
                                            entity.PickingAdminRealName,
                                            entity.Summary,
                                            entity.DelcareStatus,
                                            entity.Display,
                                            entity.Conditions,
                                        }).OrderByDescending(s => s.BoxCode)
                                    };

            return new
            {
                TotalWeight = TotalWeight,
                TotalAvgWeight = TotalAvgWeight,
                TotalPart = TotalPart,
                TotalQuantity = TotalQuantity,
                IsDisabledDeclareBtn = IsDisabledDeclareBtn,
                TinyOrderID_Group,
            };

        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <param name="status">是否正常入库，默认正常入库</param>
        /// <param name="pageIndex">当前页面的Index, 默认值1</param>
        /// <param name="pageSize">当前页面显示的数据数量，默认20</param>
        /// <returns></returns>
        public object ToMyPage(bool status = true, int? pageIndex = 1, int? pageSize = 20)
        {
            var iquery = this.ToGroupByOutputID(status) as object[];
            int total = iquery.Count();

            iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value).ToArray();

            return new
            {
                Total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                Data = iquery,
            };
        }

        #region 查询条件搜索
        /// <summary>
        /// 订单ID
        /// </summary>
        /// <param name="OrderID"></param>
        /// <returns></returns>
        public CgCustomsStorageReportsView SearchByOrderID(string OrderID)
        {
            var inputReportView = this.IQueryable.Cast<CustomsStorageReport>();

            var linq = from entity in inputReportView
                       where entity.OrderID.Contains(OrderID)
                       select entity;

            var view = new CgCustomsStorageReportsView(this.Reponsitory, linq)
            {
            };
            return view;
        }

        /// <summary>
        /// 小订单号
        /// </summary>
        /// <param name="TinyOrderID"></param>
        /// <returns></returns>
        public CgCustomsStorageReportsView SearchByTinyOrderID(string TinyOrderID)
        {
            var customsStorageReportView = this.IQueryable.Cast<CustomsStorageReport>();

            var linq = from entity in customsStorageReportView
                       where entity.TinyOrderID.Contains(TinyOrderID)
                       select entity;

            var view = new CgCustomsStorageReportsView(this.Reponsitory, linq)
            {
            };
            return view;
        }

        /// <summary>
        /// 入库日期
        /// </summary>
        /// <param name="startdate">开始时间</param>
        /// <param name="enddate">结束时间</param>
        /// <returns></returns>
        public CgCustomsStorageReportsView SearchByEnterDate(DateTime? startdate, DateTime? enddate)
        {
            Expression<Func<CustomsStorageReport, bool>> predicate = report => (startdate == null ? true : report.EnterDate >= startdate)
                && (enddate == null ? true : report.EnterDate < enddate.Value.AddDays(1));

            var customsStorageReportView = this.IQueryable.Cast<CustomsStorageReport>();
            var linq = customsStorageReportView.Where(predicate);

            var view = new CgCustomsStorageReportsView(this.Reponsitory, linq)
            {
            };
            return view;
        }

        /// <summary>
        /// 业务来源
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public CgCustomsStorageReportsView SearchBySource(params CgNoticeSource[] sources)
        {
            var inputReportView = this.IQueryable.Cast<CustomsStorageReport>();
            var includes = sources.Select(item => (int?)item);

            var linq = from entity in inputReportView
                       where includes.Contains(entity.Source)
                       select entity;

            var view = new CgCustomsStorageReportsView(this.Reponsitory, linq)
            {
            };
            return view;
        }

        /// <summary>
        /// 产品型号
        /// </summary>
        /// <param name="PartNumber"></param>
        /// <returns></returns>
        public CgCustomsStorageReportsView SearchByPartNumber(string PartNumber)
        {
            var inputReportView = this.IQueryable.Cast<CustomsStorageReport>();

            var linq = from entity in inputReportView
                       where entity.Product.PartNumber.Contains(PartNumber)
                       select entity;

            var view = new CgCustomsStorageReportsView(this.Reponsitory, linq)
            {
            };
            return view;
        }

        /// <summary>
        /// 产品品牌
        /// </summary>
        /// <param name="Manufacturer"></param>
        /// <returns></returns>
        public CgCustomsStorageReportsView SearchByManufacturer(string Manufacturer)
        {
            var inputReportView = this.IQueryable.Cast<CustomsStorageReport>();

            var linq = from entity in inputReportView
                       where entity.Product.Manufacturer.Contains(Manufacturer)
                       select entity;

            var view = new CgCustomsStorageReportsView(this.Reponsitory, linq)
            {
            };
            return view;
        }

        /// <summary>
        /// 原产地
        /// </summary>
        /// <param name="origins"></param>
        /// <returns></returns>
        public CgCustomsStorageReportsView SearchByOrigin(params Origin[] origins)
        {
            var inputReportView = this.IQueryable.Cast<CustomsStorageReport>();
            var includes = origins.Select(item => item.GetOrigin().Code);

            var linq = from entity in inputReportView
                       where includes.Contains(entity.Origin)
                       select entity;

            var view = new CgCustomsStorageReportsView(this.Reponsitory, linq)
            {
            };
            return view;
        }

        /// <summary>
        /// 入库币种
        /// </summary>
        /// <param name="currencys"></param>
        /// <returns></returns>
        public CgCustomsStorageReportsView SearchByCurrency(params Currency[] currencys)
        {
            var inputReportView = this.IQueryable.Cast<CustomsStorageReport>();
            var includes = currencys.Select(item => (int?)item);

            var linq = from entity in inputReportView
                       where includes.Contains(entity.Currency)
                       select entity;

            var view = new CgCustomsStorageReportsView(this.Reponsitory, linq)
            {
            };
            return view;
        }

        /// <summary>
        /// 供应商名称
        /// </summary>
        /// <param name="Supplier"></param>
        /// <returns></returns>
        public CgCustomsStorageReportsView SearchBySupplier(string Supplier)
        {
            var inputReportView = this.IQueryable.Cast<CustomsStorageReport>();

            var linq = from entity in inputReportView
                       where entity.Supplier.Contains(Supplier)
                       select entity;

            var view = new CgCustomsStorageReportsView(this.Reponsitory, linq)
            {
            };
            return view;
        }        

        /// <summary>
        /// 客户名称
        /// </summary>
        /// <param name="ClientName"></param>
        /// <returns></returns>
        public CgCustomsStorageReportsView SearchByClientName(string ClientName)
        {
            var inputReportView = this.IQueryable.Cast<CustomsStorageReport>();

            var linq = from entity in inputReportView
                       where entity.ClientName.Contains(ClientName)
                       select entity;

            var view = new CgCustomsStorageReportsView(this.Reponsitory, linq)
            {
            };
            return view;
        }

        /// <summary>
        /// 客户入仓号
        /// </summary>
        /// <param name="EnterCode"></param>
        /// <returns></returns>
        public CgCustomsStorageReportsView SearchByEnterCode(string EnterCode)
        {
            var inputReportView = this.IQueryable.Cast<CustomsStorageReport>();

            var linq = from entity in inputReportView
                       where entity.EnterCode == EnterCode
                       select entity;

            var view = new CgCustomsStorageReportsView(this.Reponsitory, linq)
            {
            };
            return view;
        }

        /// <summary>
        /// 根据分拣人姓名查询入库信息
        /// </summary>
        /// <param name="RealName"></param>
        /// <returns></returns>
        public CgCustomsStorageReportsView SearchByAdminName(string RealName)
        {
            var inputReportView = this.IQueryable.Cast<CustomsStorageReport>();

            var linq = from entity in inputReportView
                       where entity.PickingAdminRealName == RealName
                       select entity;
            var view = new CgCustomsStorageReportsView(this.Reponsitory, linq)
            {
            };
            return view;
        }
        #endregion

        #region 帮助类
        private class CustomsStorageReport
        {
            public string PickingID { get; set; }
            public string StorageID { get; set; }
            public string InputID { get; set; }
            public string OutputID { get; set; }
            public string ItemID { get; set; }
            public string OrderID { get; set; }
            public string TinyOrderID { get; set; }
            public string WaybillID { get; set; }
            public string NoticeID { get; set; }
            public string ClientName { get; set; }
            /// <summary>
            /// 入仓号
            /// </summary>
            public string EnterCode { get; set; }
            public string Supplier { get; set; }
            public CenterProduct Product { get; set; }
            /// <summary>
            /// 入库单价
            /// </summary>
            public decimal? Price { get; set; }
            /// <summary>
            /// 币种
            /// </summary>
            public int? Currency { get; set; }
            /// <summary>
            /// 原产地
            /// </summary>
            public string Origin { get; set; }
            /// <summary>
            /// 批次号
            /// </summary>
            public string DateCode { get; set; }
            /// <summary>
            /// 通知数量
            /// </summary>
            public decimal? NoticeQuantity { get; set; }
            /// <summary>
            /// 通知日期
            /// </summary>
            public DateTime? NoticeDate { get; set; }
            /// <summary>
            /// 入库来源
            /// </summary>
            public int? Source { get; set; }
            /// <summary>
            /// 入库数量
            /// </summary>
            public decimal? EnterQuantity { get; set; }
            /// <summary>
            /// 入库日期
            /// </summary>
            public DateTime EnterDate { get; set; }

            /// <summary>
            /// 拣货人
            /// </summary>
            public string PickingAdminID { get; set; }
            /// <summary>
            /// 分拣人真实姓名
            /// </summary>
            public string PickingAdminRealName { get; set; }
            /// <summary>
            /// 异常信息
            /// </summary>
            public string Summary { get; set; }
            /// <summary>
            /// 报关品名
            /// </summary>
            public string CustomsName { get; set; }
            /// <summary>
            /// 产品标识
            /// </summary>
            public string Conditions { get; set; }
            /// <summary>
            /// 箱号
            /// </summary>
            /// <remarks>
            /// 依照马莲建议，增加库位展示
            /// </remarks>
            public string BoxCode { get; set; }
            public decimal? Weight { get; set; }
            public decimal? Volume { get; set; }

            /// <summary>
            /// 库位
            /// </summary>
            /// <remarks>
            /// 依照马莲建议，增加库位展示
            /// </remarks>
            public string ShelveID { get; set; }

            /// <summary>
            /// 
            /// </summary>
            /// <remarks>
            /// 请参考pvwms 中的 枚举 TinyOrderDeclareStatus 设定，通用本地就不增加申报状态
            /// </remarks>
            public int? DelcareStatus { get; set; }
            /// <summary>
            /// 是否显示删除按钮
            /// </summary>
            public bool Display { get; set; }
        }
        #endregion
    }

    /// <summary>
    /// 出库报表视图
    /// </summary>
    public class CgOutputReportsView : QueryView<object, PvWmsRepository>
    {
        private string wareHouseID;

        #region 构造函数

        public CgOutputReportsView()
        {

        }

        protected CgOutputReportsView(PvWmsRepository reponsitory) : base(reponsitory)
        {
        }

        protected CgOutputReportsView(PvWmsRepository reponsitory, IQueryable<object> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        #endregion

        protected override IQueryable<object> GetIQueryable()
        {
            var adminTopView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.AdminsTopView>();

            var linq = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.CgOutputReportTopView>()
                       join admin in adminTopView on entity.AdminID equals admin.ID
                       select new OutputReport
                       {
                           PickingID = entity.ID,
                           InputID = entity.InputID,
                           OutputID = entity.OutputID,
                           OrderID = entity.OrderID,
                           TinyOrderID = entity.TinyOrderID,
                           WaybillID = entity.WaybillID,

                           NoticeID = entity.NoticeID,
                           WareHouseID = entity.WareHouseID,
                           ClientID = entity.ClientID,
                           ClientName = entity.ClientName,
                           EnterCode = entity.EnterCode,
                           Supplier = entity.Supplier,

                           PickingQuantity = entity.PickingQuantity,//拣货数量
                           PickingDate = entity.PickingDate,//拣货日期
                           PickingAdminID = entity.AdminID,//拣货人员
                           PickingAdminRealName = admin.RealName,
                           ReviewerID = entity.ReviewerID,//复核人
                           BoxCode = entity.BoxCode,
                           Summary = entity.Summary,//异常信息

                           NoticeQuantity = entity.NoticeQuantity,//通知数量
                           NoticeDate = entity.NoticeDate,//通知日期
                           Source = entity.Source,//业务来源

                           Product = new CenterProduct
                           {
                               ID = entity.ProductID,
                               PartNumber = entity.PartNumber,
                               Manufacturer = entity.Manufacturer,
                           },
                           Origin = entity.Origin,
                           DateCode = entity.DateCode,
                           CustomsName = entity.CustomsName,

                           InUnitPrice = entity.inUnitPrice,
                           InCurrency = entity.inCurrency,
                           OutUnitPrice = entity.outUnitPrice,
                           OutCurrency = entity.outCurrency,
                       };
            return linq;
        }

        #region 数据分组

        public object ToGroupByInput()
        {
            var items = this.IQueryable.Cast<OutputReport>().ToArray();

            var linq = from item in items
                       group item by new
                       {
                           item.InputID,
                       } into groups
                       let single = groups.FirstOrDefault(item => item.InputID == groups.Key.InputID)
                       select new
                       {
                           InputID = single.InputID,
                           WareHouseID = single.WareHouseID,
                           NoticeID = single.NoticeID,
                           YcQty = single.NoticeQuantity,
                           ScQty = groups.Sum(item => item.PickingQuantity),

                           ClientID = single.ClientID,
                           ClientName = single.ClientName,
                           Supplier = single.Supplier,
                           EnterCode = single.EnterCode,

                           InUnitPrice = single.InUnitPrice,
                           InCurrency = single.InCurrency,
                           NoticeDate = single.NoticeDate,
                           Source = single.Source,
                           WaybillID = single.WaybillID,
                           DateCode = single.DateCode,
                           CustomsName = single.CustomsName,
                           PartNumber = single.Product.PartNumber,
                           Datas = groups.Select(item => new
                           {
                               OrderID = item.OrderID,
                               TinyOrderID = item.TinyOrderID,
                               PickingID = item.PickingID,
                               BoxCode = item.BoxCode,
                               Quantity = item.PickingQuantity,//本次拣货数量                               
                               Origin = ((Origin)Enum.Parse(typeof(Origin), item.Origin ?? nameof(Origin.Unknown))).GetDescription(),
                               OutUnitPrice = item.OutUnitPrice,
                               OutCurrency = ((Currency)(item.OutCurrency.HasValue ? item.OutCurrency.Value : 0)).GetCurrency().ShortName,
                               PickingDate = item.PickingDate,
                               PickingAdminID = item.PickingAdminID,
                               PickingAdminRealName = item.PickingAdminRealName,
                               ReviewerID = item.ReviewerID,
                               Product = item.Product,
                               Summary = item.Summary,
                           }),
                       };

            return linq.ToArray();
        }

        public object ToGroupByInput(CgOutputReportsView reports)
        {
            var items = reports.Cast<OutputReport>().ToArray();

            var linq = from item in items
                       group item by new
                       {
                           item.InputID,
                       } into groups
                       let single = groups.FirstOrDefault(item => item.InputID == groups.Key.InputID)
                       select new
                       {
                           InputID = single.InputID,
                           WareHouseID = single.WareHouseID,
                           NoticeID = single.NoticeID,
                           YcQty = single.NoticeQuantity,
                           ScQty = groups.Sum(item => item.PickingQuantity),

                           ClientID = single.ClientID,
                           ClientName = single.ClientName,
                           Supplier = single.Supplier,
                           EnterCode = single.EnterCode,

                           InUnitPrice = single.InUnitPrice,
                           InCurrency = single.InCurrency,
                           NoticeDate = single.NoticeDate,
                           Source = single.Source,
                           WaybillID = single.WaybillID,

                           DateCode = single.DateCode,
                           CustomsName = single.CustomsName,
                           PartNumber = single.Product.PartNumber,

                           Datas = groups.Select(item => new
                           {
                               OrderID = item.OrderID,
                               TinyOrderID = item.TinyOrderID,
                               PickingID = item.PickingID,
                               Origin = ((Origin)Enum.Parse(typeof(Origin), item.Origin ?? nameof(Origin.Unknown))).GetDescription(),
                               BoxCode = item.BoxCode,
                               Quantity = item.PickingQuantity,//本次拣货数量
                               OutUnitPrice = item.OutUnitPrice,
                               OutCurrency = ((Currency)(item.OutCurrency.HasValue ? item.OutCurrency.Value : 0)).GetCurrency().ShortName,
                               PickingDate = item.PickingDate,
                               PickingAdminID = item.PickingAdminID,
                               ReviewerID = item.ReviewerID,
                               Product = item.Product,
                               Summary = item.Summary,
                           }),
                       };

            return linq.ToArray();
        }

        /// <summary>
        /// 分组分页方法
        /// </summary>
        /// <param name="pageIndex">当前页面的Index, 默认值1</param>
        /// <param name="pageSize">当前页面显示的数据数量，默认20</param>
        /// <returns></returns>
        public object ToGroupPage(int? pageIndex = 1, int? pageSize = 20)
        {
            var iquery = this.ToGroupByInput() as object[];
            int total = iquery.Count();

            iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value).ToArray();

            return new
            {
                Total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                Data = iquery,
            };
        }

        /// <summary>
        /// 普通分页
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public object ToPage(int? pageIndex = 1, int? pageSize = 20)
        {
            var iquery = this.IQueryable.Cast<OutputReport>().OrderByDescending(item => item.PickingDate);
            int total = iquery.Count();

            var ienum_iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value).ToArray();

            var result = ienum_iquery.Select(item => new
            {
                PickingID = item.PickingID,
                InputID = item.InputID,
                OutputID = item.OutputID,
                OrderID = item.OrderID,
                TinyOrderID = item.TinyOrderID,
                WaybillID = item.WaybillID,

                NoticeID = item.NoticeID,
                WareHouseID = item.WareHouseID,
                ClientID = item.ClientID,
                ClientName = item.ClientName,
                EnterCode = item.EnterCode,
                Supplier = item.Supplier,

                PickingQuantity = item.PickingQuantity,//拣货数量
                PickingDate = item.PickingDate,//拣货日期
                PickingAdminID = item.PickingAdminID,//拣货人员
                PickingAdminRealName = item.PickingAdminRealName,
                ReviewerID = item.ReviewerID,//复核人
                BoxCode = item.BoxCode,
                Summary = item.Summary,//异常信息

                NoticeQuantity = item.NoticeQuantity,//通知数量
                NoticeDate = item.NoticeDate,//通知日期
                Source = item.Source,//业务来源

                Product = item.Product,
                Origin = ((Origin)Enum.Parse(typeof(Origin), item.Origin ?? nameof(Origin.Unknown))).GetDescription(),
                DateCode = item.DateCode,
                CustomsName = item.CustomsName,

                InUnitPrice = item.InUnitPrice,
                InCurrency = ((Currency)(item.InCurrency.HasValue ? item.InCurrency.Value : 0)).GetCurrency().ShortName,
                OutUnitPrice = item.OutUnitPrice,
                OutCurrency = ((Currency)(item.OutCurrency.HasValue ? item.OutCurrency.Value : 0)).GetCurrency().ShortName,
            });

            return new
            {
                Total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                Data = result.ToArray(),
            };
        }

        #endregion

        #region 查询条件搜索

        /// <summary>
        /// 库房ID
        /// </summary>
        /// <param name="warehouseID"></param>
        /// <returns></returns>
        public CgOutputReportsView SearchByWareHouseID(string wareHouseID)
        {
            this.wareHouseID = wareHouseID;
            var OutputReportView = this.IQueryable.Cast<OutputReport>();
            if (string.IsNullOrWhiteSpace(this.wareHouseID))
            {
                return new CgOutputReportsView(this.Reponsitory, OutputReportView);
            }

            var linq = from entity in OutputReportView
                       where entity.WareHouseID.Contains(this.wareHouseID)
                       select entity;
            var view = new CgOutputReportsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };
            return view;
        }

        /// <summary>
        /// 订单ID
        /// </summary>
        /// <param name="OrderID"></param>
        /// <returns></returns>
        public CgOutputReportsView SearchByOrderID(string OrderID)
        {
            var OutputReportView = this.IQueryable.Cast<OutputReport>();

            var linq = from entity in OutputReportView
                       where entity.OrderID.Contains(OrderID)
                       select entity;

            var view = new CgOutputReportsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };
            return view;
        }

        /// <summary>
        /// 小订单号
        /// </summary>
        /// <param name="TinyOrderID"></param>
        /// <returns></returns>
        public CgOutputReportsView SearchByTinyOrderID(string TinyOrderID)
        {
            var OutputReportView = this.IQueryable.Cast<OutputReport>();

            var linq = from entity in OutputReportView
                       where entity.OrderID.Contains(TinyOrderID)
                       select entity;

            var view = new CgOutputReportsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };
            return view;
        }

        /// <summary>
        /// 出库日期
        /// </summary>
        /// <param name="startdate">开始时间</param>
        /// <param name="enddate">结束时间</param>
        /// <returns></returns>
        public CgOutputReportsView SearchByExitDate(DateTime? startdate, DateTime? enddate)
        {
            Expression<Func<OutputReport, bool>> predicate = report => (startdate == null ? true : report.PickingDate >= startdate)
                && (enddate == null ? true : report.PickingDate < enddate.Value.AddDays(1));

            var OutputReportView = this.IQueryable.Cast<OutputReport>();
            var linq = OutputReportView.Where(predicate);

            var view = new CgOutputReportsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };
            return view;
        }

        /// <summary>
        /// 业务来源
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public CgOutputReportsView SearchBySource(params CgNoticeSource[] sources)
        {
            var OutputReportView = this.IQueryable.Cast<OutputReport>();
            var includes = sources.Select(item => (int?)item);

            var linq = from entity in OutputReportView
                       where includes.Contains(entity.Source)
                       select entity;

            var view = new CgOutputReportsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };
            return view;
        }

        /// <summary>
        /// 产品型号
        /// </summary>
        /// <param name="PartNumber"></param>
        /// <returns></returns>
        public CgOutputReportsView SearchByPartNumber(string PartNumber)
        {
            var OutputReportView = this.IQueryable.Cast<OutputReport>();

            var linq = from entity in OutputReportView
                       where entity.Product.PartNumber.Contains(PartNumber)
                       select entity;

            var view = new CgOutputReportsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };
            return view;
        }

        /// <summary>
        /// 产品品牌
        /// </summary>
        /// <param name="Manufacturer"></param>
        /// <returns></returns>
        public CgOutputReportsView SearchByManufacturer(string Manufacturer)
        {
            var OutputReportView = this.IQueryable.Cast<OutputReport>();

            var linq = from entity in OutputReportView
                       where entity.Product.Manufacturer.Contains(Manufacturer)
                       select entity;

            var view = new CgOutputReportsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };
            return view;
        }

        /// <summary>
        /// 原产地
        /// </summary>
        /// <param name="origins"></param>
        /// <returns></returns>
        public CgOutputReportsView SearchByOrigin(params Origin[] origins)
        {
            var OutputReportView = this.IQueryable.Cast<OutputReport>();
            var includes = origins.Select(item => item.GetOrigin().Code);

            var linq = from entity in OutputReportView
                       where includes.Contains(entity.Origin)
                       select entity;

            var view = new CgOutputReportsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };
            return view;
        }

        /// <summary>
        /// 出库币种
        /// </summary>
        /// <param name="currencys"></param>
        /// <returns></returns>
        public CgOutputReportsView SearchByCurrency(params Currency[] currencys)
        {
            var OutputReportView = this.IQueryable.Cast<OutputReport>();
            var includes = currencys.Select(item => (int?)item);

            var linq = from entity in OutputReportView
                       where includes.Contains(entity.OutCurrency)
                       select entity;

            var view = new CgOutputReportsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };
            return view;
        }

        /// <summary>
        /// 供应商名称
        /// </summary>
        /// <param name="Supplier"></param>
        /// <returns></returns>
        public CgOutputReportsView SearchBySupplier(string Supplier)
        {
            var OutputReportView = this.IQueryable.Cast<OutputReport>();

            var linq = from entity in OutputReportView
                       where entity.Supplier.Contains(Supplier)
                       select entity;

            var view = new CgOutputReportsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };
            return view;
        }

        /// <summary>
        /// 客户ID
        /// </summary>
        /// <param name="ClientID"></param>
        /// <returns></returns>
        public CgOutputReportsView SearchByClientID(string ClientID)
        {
            var OutputReportView = this.IQueryable.Cast<OutputReport>();

            var linq = from entity in OutputReportView
                       where entity.ClientID == ClientID
                       select entity;

            var view = new CgOutputReportsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };
            return view;
        }

        /// <summary>
        /// 客户名称
        /// </summary>
        /// <param name="ClientName"></param>
        /// <returns></returns>
        public CgOutputReportsView SearchByClientName(string ClientName)
        {
            var OutputReportView = this.IQueryable.Cast<OutputReport>();

            var linq = from entity in OutputReportView
                       where entity.ClientName.Contains(ClientName)
                       select entity;

            var view = new CgOutputReportsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };
            return view;
        }

        /// <summary>
        /// 客户入仓号
        /// </summary>
        /// <param name="EnterCode"></param>
        /// <returns></returns>
        public CgOutputReportsView SearchByEnterCode(string EnterCode)
        {
            var OutputReportView = this.IQueryable.Cast<OutputReport>();

            var linq = from entity in OutputReportView
                       where entity.EnterCode == EnterCode
                       select entity;

            var view = new CgOutputReportsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };
            return view;
        }

        /// <summary>
        /// 拣货人姓名
        /// </summary>
        /// <param name="RealName"></param>
        /// <returns></returns>
        public CgOutputReportsView SearchByAdminName(string RealName)
        {
            var OutputReportView = this.IQueryable.Cast<OutputReport>();

            var linq = from entity in OutputReportView
                       where entity.PickingAdminRealName == RealName
                       select entity;
            var view = new CgOutputReportsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };
            return view;
        }

        #endregion

        #region 帮助类

        private class OutputReport
        {
            public string PickingID { get; set; }
            public string InputID { get; set; }
            public string OutputID { get; set; }
            public string OrderID { get; set; }
            public string TinyOrderID { get; set; }
            public string WaybillID { get; set; }
            public string NoticeID { get; set; }
            public string WareHouseID { get; set; }
            public string ClientID { get; set; }
            public string ClientName { get; set; }
            /// <summary>
            /// 入仓号
            /// </summary>
            public string EnterCode { get; set; }
            public CenterProduct Product { get; set; }
            /// <summary>
            /// 入库单价
            /// </summary>
            public decimal? InUnitPrice { get; set; }
            /// <summary>
            /// 币种
            /// </summary>
            public int? InCurrency { get; set; }
            /// <summary>
            /// 出库单价
            /// </summary>
            public decimal? OutUnitPrice { get; set; }
            /// <summary>
            /// 出库币种
            /// </summary>
            public int? OutCurrency { get; set; }
            /// <summary>
            /// 原产地
            /// </summary>
            public string Origin { get; set; }
            /// <summary>
            /// 批次号
            /// </summary>
            public string DateCode { get; set; }
            /// <summary>
            /// 通知数量
            /// </summary>
            public decimal? NoticeQuantity { get; set; }
            /// <summary>
            /// 通知日期
            /// </summary>
            public DateTime? NoticeDate { get; set; }
            /// <summary>
            /// 入库来源
            /// </summary>
            public int? Source { get; set; }
            /// <summary>
            /// 入库数量
            /// </summary>
            public decimal PickingQuantity { get; set; }
            /// <summary>
            /// 入库日期
            /// </summary>
            public DateTime PickingDate { get; set; }
            /// <summary>
            /// 供应商
            /// </summary>
            public string Supplier { get; set; }
            /// <summary>
            /// 拣货人
            /// </summary>
            public string PickingAdminID { get; set; }
            /// <summary>
            /// 拣货人真实姓名
            /// </summary>
            public string PickingAdminRealName { get; set; }
            /// <summary>
            /// 异常信息
            /// </summary>
            public string Summary { get; set; }
            /// <summary>
            /// 报关品名
            /// </summary>
            public string CustomsName { get; set; }

            /// <summary>
            /// 出库复核人
            /// </summary>
            public string ReviewerID { get; set; }

            /// <summary>
            /// 箱号
            /// </summary>
            public string BoxCode { get; set; }
        }

        #endregion
    }

    /// <summary>
    /// 客户的出入库报告
    /// </summary>
    /// <remarks>
    /// 目前只做深圳的入库与出库的对应视图，并提供利润
    /// </remarks>
    public class CgClientReportsView : QueryView<object, PvWmsRepository>
    {
        private string wareHouseID;

        #region 构造函数

        public CgClientReportsView()
        {

        }

        protected CgClientReportsView(PvWmsRepository reponsitory) : base(reponsitory)
        {
        }

        protected CgClientReportsView(PvWmsRepository reponsitory, IQueryable<object> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        #endregion

        protected override IQueryable<object> GetIQueryable()
        {
            var clientReportView = Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.CgClientIOReportTopView>();
            var linq = from entity in clientReportView
                       select new ClientReport
                       {
                           InputID = entity.InputID,
                           OutputID = entity.OutputID,
                           WareHouseID = entity.WareHouseID,
                           CustomsName = entity.CustomsName,
                           PartNumber = entity.PartNumber,
                           Manufacturer = entity.Manufacturer,
                           Quantity = entity.Quantity,
                           Date = entity.Date,
                           InUnitPrice = entity.inUnitPrice,
                           InCurrency = entity.inCurrency,
                           OutUnitPrice = entity.OutUnitPrice,
                           OutCurrency = entity.OutCurrency,
                           ClientName = entity.ClientName,
                           EnterCode = entity.EnterCode,
                           profit = entity.profit,
                           PastInQuantity = entity.PastInQuantity,
                           PastOutQuantity = entity.PastOutQuantity,
                           Origin = entity.Origin
                       };

            return linq;
        }

        /// <summary>
        /// 普通分页
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public object ToPage(int? pageIndex = 1, int? pageSize = 20)
        {
            var iquery = this.IQueryable.Cast<ClientReport>().OrderByDescending(item => item.Date);
            int total = iquery.Count();

            var ienum_iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value).ToArray();

            var result = ienum_iquery.Select(item => new
            {
                InputID = item.InputID,
                OutputID = item.OutputID,
                WareHouseID = item.WareHouseID,
                CustomsName = item.CustomsName,
                PartNumber = item.PartNumber,
                Manufacturer = item.Manufacturer,
                Quantity = item.Quantity,
                Date = item.Date,
                InUnitPrice = item.InUnitPrice,
                InCurrency = item.InCurrency,
                OutUnitPrice = item.OutUnitPrice,
                OutCurrency = item.OutCurrency,
                ClientName = item.ClientName,
                EnterCode = item.EnterCode,
                profit = item.profit,
                PastInQuantity = item.PastInQuantity,
                PastOutQuantity = item.PastOutQuantity,
                Origin = item.Origin,
                ShowType = item.ShowType,
            });

            return new
            {
                Total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                Data = result.ToArray(),
            };
        }

        #region 查询条件搜索

        /// <summary>
        /// 库房ID
        /// </summary>
        /// <param name="warehouseID"></param>
        /// <returns></returns>
        public CgClientReportsView SearchByWareHouseID(string wareHouseID)
        {
            this.wareHouseID = wareHouseID;
            var ClientReportView = this.IQueryable.Cast<ClientReport>();
            if (string.IsNullOrWhiteSpace(this.wareHouseID))
            {
                return new CgClientReportsView(this.Reponsitory, ClientReportView);
            }

            var linq = from entity in ClientReportView
                       where entity.WareHouseID.StartsWith(this.wareHouseID)
                       select entity;
            var view = new CgClientReportsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };
            return view;
        }
        /// <summary>
        /// 客户ID
        /// </summary>
        /// <param name="ClientID"></param>
        /// <returns></returns>
        public CgClientReportsView SearchByClientName(string ClientName)
        {
            var ClientReportView = this.IQueryable.Cast<ClientReport>();

            var linq = from entity in ClientReportView
                       where entity.ClientName.Contains(ClientName)
                       select entity;

            var view = new CgClientReportsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };
            return view;
        }
        /// <summary>
        /// 入仓号
        /// </summary>
        /// <param name="ClientID"></param>
        /// <returns></returns>
        public CgClientReportsView SearchByEnterCode(string EnterCode)
        {
            var ClientReportView = this.IQueryable.Cast<ClientReport>();

            var linq = from entity in ClientReportView
                       where entity.EnterCode == EnterCode
                       select entity;

            var view = new CgClientReportsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };
            return view;
        }
        /// <summary>
        /// 产品型号
        /// </summary>
        /// <param name="ClientID"></param>
        /// <returns></returns>
        public CgClientReportsView SearchByPartNumber(string PartNumber)
        {
            var ClientReportView = this.IQueryable.Cast<ClientReport>();

            var linq = from entity in ClientReportView
                       where entity.PartNumber.Contains(PartNumber)
                       select entity;

            var view = new CgClientReportsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };
            return view;
        }
        /// <summary>
        /// 产品品牌
        /// </summary>
        /// <param name="ClientID"></param>
        /// <returns></returns>
        public CgClientReportsView SearchByManufacturer(string Manufacturer)
        {
            var ClientReportView = this.IQueryable.Cast<ClientReport>();

            var linq = from entity in ClientReportView
                       where entity.Manufacturer.Contains(Manufacturer)
                       select entity;

            var view = new CgClientReportsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };
            return view;
        }
        /// <summary>
        /// 出入库时间
        /// </summary>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <returns></returns>
        public CgClientReportsView SearchByDate(DateTime startdate, DateTime enddate)
        {
            Expression<Func<ClientReport, bool>> predicate = item => item.Date >= startdate && item.Date < enddate;

            var ClientReportView = this.IQueryable.Cast<ClientReport>();
            var linq = ClientReportView.Where(predicate);

            var view = new CgClientReportsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };
            return view;
        }

        #endregion

        #region 帮助类
        private class ClientReport
        {
            public string InputID { get; set; }
            /// <summary>
            /// 当销项ID为空时表示入库，非空时表示出库
            /// </summary>
            public string OutputID { get; set; }
            public string ClientName { get; set; }
            public string EnterCode { get; set; }
            public string WareHouseID { get; set; }
            public string PartNumber { get; set; }
            public string Manufacturer { get; set; }
            public string CustomsName { get; set; }


            /// <summary>
            /// 原产地
            /// </summary>
            public string Origin { get; set; }


            /// <summary>
            /// 
            /// </summary>
            /// <remarks>
            /// 
            /// </remarks>
            public int ShowType
            {
                get
                {
                    if (InCurrency != null
                        && InUnitPrice != null
                        && OutUnitPrice == null
                        && OutCurrency == null)
                    {
                        return 1; //视为借记录
                    }

                    if (OutUnitPrice != null
                      && OutCurrency != null)
                    {
                        return 2; //视为贷记录
                    }

                    return 0;//忽略
                }
            }

            /// <summary>
            /// 出入库日期
            /// </summary>
            public DateTime Date { get; set; }

            /// <summary>
            /// 出入库数量
            /// </summary>
            public decimal? Quantity { get; set; }

            /// <summary>
            /// 入库单价
            /// </summary>       
            public decimal? InUnitPrice { get; set; }

            /// <summary>
            /// 出入库币种
            /// </summary>
            public int? InCurrency { get; set; }

            /// <summary>
            /// 入库金额
            /// </summary>
            public decimal? InTotal
            {
                get
                {
                    return this.InCurrency * this.InUnitPrice;
                }
            }


            /// <summary>
            /// 出库单价
            /// </summary>       
            public decimal? OutUnitPrice { get; set; }

            /// <summary>
            /// 出库币种
            /// </summary>
            public int? OutCurrency { get; set; }

            /// <summary>
            /// 入库金额
            /// </summary>
            public decimal? OutTotal
            {
                get
                {
                    return this.OutUnitPrice * this.OutCurrency;
                }
            }


            /// <summary>
            /// 历史到货数量
            /// </summary>
            public decimal? PastInQuantity { get; set; }

            /// <summary>
            /// 历史出货数量
            /// </summary>
            public decimal? PastOutQuantity { get; set; }

            /// <summary>
            /// 利率金额
            /// </summary>
            public decimal? profit { get; set; }

            /// <summary>
            /// 结存数量
            /// </summary>
            public decimal? BalanceQuantity
            {
                get
                {
                    if (string.IsNullOrEmpty(this.OutputID))
                    {
                        return this.PastInQuantity - this.PastOutQuantity + this.Quantity;
                    }
                    else
                    {
                        return this.PastInQuantity - this.PastOutQuantity - this.Quantity;
                    }
                }
            }

            /// <summary>
            /// 结存单价
            /// </summary>
            public decimal? BalanceUnitPrice
            {
                get
                {
                    if (this.BalanceQuantity == null)
                    {
                        return null;
                    }
                    return this.InUnitPrice;
                }
            }

            /// <summary>
            /// 结存金额
            /// </summary>
            public decimal? BalanceTotal
            {
                get
                {
                    return this.BalanceQuantity * this.BalanceUnitPrice;
                }
            }
        }
        #endregion
    }
}