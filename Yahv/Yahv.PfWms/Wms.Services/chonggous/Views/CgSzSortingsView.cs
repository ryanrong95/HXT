using Layers.Data.Sqls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Wms.Services.chonggous.Models;
using Wms.Services.Enums;
using Yahv.Linq;
using Yahv.Services.Enums;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Wms.Services.chonggous.Views
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// 深圳入库
    /// 业务来源：转报关、代报关
    /// 通知类型：入库
    /// 
    /// 深圳入库已经保存了如下对象
    /// input notice sorting storage都有了
    ///
    /// </remarks>
    public class CgSzSortingsView : QueryView<object, PvWmsRepository>
    {
        public CgSzSortingsView()
        {
        }

        protected CgSzSortingsView(PvWmsRepository reponsitory) : base(reponsitory)
        {
        }

        protected CgSzSortingsView(PvWmsRepository reponsitory, IQueryable<object> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<object> GetIQueryable()
        {
            string whid = nameof(Yahv.Services.WhSettings.SZ);
            var types = new[] { (int)Yahv.Services.Enums.CgNoticeType.Enter };
            var sources = new[] { (int)Yahv.Services.Enums.CgNoticeSource.AgentBreakCustoms,
            (int)Yahv.Services.Enums.CgNoticeSource.AgentCustomsFromStorage,
            (int)Yahv.Services.Enums.CgNoticeSource.AgentBreakCustomsForIns};

            //var waybillIDViews = from notice in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
            //                     where notice.WareHouseID.StartsWith(whid)
            //                       && types.Contains(notice.Type) && sources.Contains(notice.Source)
            //                     select notice.WaybillID;
            //var waybillIDs = waybillIDViews.Distinct();


            var linq = from waybill in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>()
                       join carrier in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.CarriersTopView>()
                       on waybill.wbCarrierID equals carrier.ID
                       //join wbid in waybillIDs on waybill.wbID equals wbid
                       where types.Contains((int)waybill.NoticeType) && sources.Contains((int)waybill.Source) && waybill.CuttingOrderStatus.HasValue == true && waybill.CuttingOrderStatus.Value == 2
                       select new MyWaybill
                       {
                           ID = waybill.wbID,
                           Code = waybill.wbCode,
                           EnterCode = waybill.wbEnterCode,
                           //Type = (waybill.chcdIsOnevehicle.HasValue && waybill.chcdIsOnevehicle.Value) ? "包车" : "普通",
                           IsOneVehicle = waybill.chcdIsOnevehicle,
                           ExcuteStatus = (CgSortingExcuteStatus)waybill.wbExcuteStatus,
                           CarNumber1 = waybill.chcdCarNumber1,
                           CarNumber2 = waybill.chcdCarNumber2,
                           LotNumber = waybill.chcdLotNumber,
                           Driver = waybill.chcdDriver,
                           PlanDate = waybill.chcdPlanDate,
                           DepartDate = waybill.chcdDepartDate,
                           TotalQuantity = waybill.chcdTotalQuantity,
                           CarrierName = carrier.Name,
                           Source = (CgNoticeSource)waybill.Source,
                       };

            return linq;
        }

        /// <summary>
        /// 补全数据
        /// </summary>
        /// <returns></returns>
        public object[] ToMyArray()
        {
            return this.ToMyPage(null, null) as object[];
        }

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

                if (string.IsNullOrEmpty(prex))
                {
                    sets.Add(context);
                    continue;
                }

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


        /// <summary>
        /// 获取总件数
        /// </summary>
        /// <param name="boxCodes">箱号</param>
        /// <returns></returns>
        [Obsolete("原则上讲：废弃不用了")]
        static public int GetTotalPart(IEnumerable<string> boxCodes, bool isForIns)
        {
            if (isForIns)
            {
                return boxCodes.Count();
            }
            else
            {
                using (PvWmsRepository repository = new PvWmsRepository())
                {
                    var linq = from box in repository.ReadTable<Layers.Data.Sqls.PvWms.Boxes>()
                               where boxCodes.Contains(box.Series)
                               select box.ID;
                    var count = linq.Count();
                    return count;
                }
            }

        }



        /// <summary>
        /// 分页方法
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            var iquery = this.IQueryable.Cast<MyWaybill>();
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            var ienum_waybills = iquery.ToArray();
            var waybillids = ienum_waybills.Select(item => item.ID).Distinct().ToArray();
            var lotNumbers = ienum_waybills.Select(item => item.LotNumber).Distinct().ToArray();

            #region 获取相关文件
            var filesView = from file in new Yahv.Services.Views.CenterFilesTopView()
                            where waybillids.Contains(file.WaybillID) || lotNumbers.Contains(file.ShipID)
                            select new CenterFileDescription
                            {
                                ID = file.ID,
                                WaybillID = file.WaybillID,
                                NoticeID = file.NoticeID,
                                StorageID = file.StorageID,
                                CustomName = file.CustomName,
                                Type = file.Type,
                                Url = CenterFile.Web + file.Url,
                                CreateDate = file.CreateDate,
                                ClientID = file.ClientID,
                                ShipID = file.ShipID,
                                AdminID = file.AdminID,
                                InputID = file.InputID,
                                Status = file.Status,
                            };
            var files = filesView.ToArray();
            #endregion

            #region 获取Notices
            var noticeView = from notice in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                             join sorting in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Sortings>()
                             on notice.ID equals sorting.NoticeID
                             join input in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Inputs>()
                             on notice.InputID equals input.ID
                             join client in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.ClientsTopView>()
                             on input.ClientID equals client.ID
                             join storage in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>()
                             on notice.InputID equals storage.InputID
                             join product in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.ProductsTopView>()
                             on storage.ProductID equals product.ID
                             where waybillids.Contains(notice.WaybillID)
                             orderby input.TinyOrderID descending
                             select new
                             {
                                 notice.WaybillID,
                                 input.TinyOrderID,
                                 input.ClientID,
                                 input.ItemID,
                                 product.PartNumber,
                                 product.Manufacturer,
                                 client.Name,
                                 sorting.BoxCode,
                                 sorting.CreateDate,
                                 sorting.Weight,
                                 StorageID = storage.ID,
                                 storage.ShelveID,
                             };
            var ienum_notices = noticeView.ToArray();
            #endregion

            var linq = from waybill in ienum_waybills
                       join notice in ienum_notices
                         on waybill.ID equals notice.WaybillID into notices
                       select new
                       {
                           waybill,
                           notices = notices.Select(notice => new
                           {
                               notice.WaybillID,
                               notice.TinyOrderID,
                               notice.ClientID,
                               notice.ItemID,
                               notice.PartNumber,
                               notice.Manufacturer,
                               notice.Name,
                               notice.BoxCode,
                               notice.CreateDate,
                               notice.Weight,
                               notice.StorageID,
                               notice.ShelveID,
                           }).ToArray(),
                       };

            var results = linq.Select(item => new
            {
                Waybill = new
                {
                    item.waybill.ID,
                    item.waybill.Code,
                    item.waybill.EnterCode,
                    Type = (item.waybill.IsOneVehicle.HasValue && item.waybill.IsOneVehicle.Value) ? "包车" : "普通",
                    item.waybill.IsOneVehicle,
                    item.waybill.CarNumber1,
                    item.waybill.CarNumber2,
                    item.waybill.LotNumber,
                    item.waybill.PlanDate,
                    item.waybill.DepartDate,
                    item.waybill.Driver,
                    item.waybill.CarrierName,
                    Files = files.Where(file => file.WaybillID == item.waybill.ID || file.ShipID == item.waybill.LotNumber),
                    TotalQuantity = item.notices.Count(),//总条数，按照陈经理说法就是通知的数量   //型号的数量
                    //ShelveQuantity = GetTotalPart(item.notices.Where(notice => notice.ShelveID != null).Select(n => n.BoxCode).Distinct(), item.waybill.Source == CgNoticeSource.AgentBreakCustomsForIns),//已上架数量
                    ShelveQuantity = GetTotalPart(item.notices.Where(notice => notice.ShelveID != null && notice.BoxCode != null).Select(n => n.BoxCode.ToUpper().Trim()).Distinct()),//已上架数量
                    TotalWeight = item.notices.Where(notice => notice.Weight.HasValue).Sum(notice => notice.Weight.Value),
                    //TotalParts = GetTotalPart(item.notices.Select(x => x.BoxCode).Distinct(), item.waybill.Source == CgNoticeSource.AgentBreakCustomsForIns),//总箱数
                    TotalParts = GetTotalPart(item.notices.Where(x => x.BoxCode != null).Select(x => x.BoxCode.ToUpper().Trim()).Distinct()),//总箱数
                },
                notices = item.notices.Select(notice => new
                {
                    notice.WaybillID,
                    notice.TinyOrderID,
                    notice.ClientID,
                    notice.Name,
                    notice.BoxCode,
                    ShelveID = notice.ShelveID?.Substring(3),
                    status = notice.ShelveID != null ? "已上架" : "未上架",
                }).Distinct(),
            }).OrderByDescending(item =>item.Waybill.DepartDate);

            if (!pageIndex.HasValue && !pageSize.HasValue)
            {
                return results.Select(item =>
                {
                    object o = item;
                    return o;
                }).ToArray();
            }

            return new
            {
                Total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                Data = results.ToArray(),
            };
        }


        /// <summary>
        /// 获取打印入库单数据
        /// </summary>
        /// <returns></returns>
        public object ToPrint(string waybillID, string adminID)
        {
            var iquery = this.IQueryable.Cast<MyWaybill>();
            int total = iquery.Count();

            var ienum_waybills = iquery.ToArray();
            var waybillids = ienum_waybills.Select(item => item.ID).Distinct().ToArray();

            var noticeView = from notice in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                             join sorting in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Sortings>()
                             on notice.ID equals sorting.NoticeID
                             join input in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Inputs>()
                             on notice.InputID equals input.ID
                             join client in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.ClientsTopView>()
                             on input.ClientID equals client.ID
                             join storage in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>()
                             on notice.InputID equals storage.InputID
                             join product in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.ProductsTopView>()
                             on storage.ProductID equals product.ID
                             where waybillids.Contains(notice.WaybillID)
                             orderby input.TinyOrderID descending
                             select new
                             {
                                 notice.WaybillID,
                                 input.TinyOrderID,
                                 input.ClientID,
                                 input.ItemID,
                                 product.PartNumber,
                                 product.Manufacturer,
                                 storage.Origin,
                                 client.Name,
                                 client.EnterCode,
                                 sorting.BoxCode,
                                 sorting.Weight,
                                 sorting.CreateDate,
                                 StorageID = storage.ID,
                             };
            var ienum_notices = noticeView.ToArray();

            var linq = from waybill in ienum_waybills
                       join notice in ienum_notices
                         on waybill.ID equals notice.WaybillID into notices
                       select new
                       {
                           waybill,
                           notices = notices.Select(notice => new
                           {
                               notice.WaybillID,
                               notice.TinyOrderID,
                               notice.ClientID,
                               notice.EnterCode,
                               notice.ItemID,
                               notice.Name,
                               notice.BoxCode,
                               notice.Weight,
                               notice.CreateDate,
                               notice.StorageID
                           }).ToArray(),
                       };

            var results = linq.Select(item => new
            {
                Waybill = new
                {
                    item.waybill.ID,
                    item.waybill.Code,
                    item.waybill.EnterCode,
                    Type = (item.waybill.IsOneVehicle.HasValue && item.waybill.IsOneVehicle.Value) ? "包车" : "普通",
                    item.waybill.IsOneVehicle,
                    item.waybill.CarNumber1,
                    item.waybill.CarNumber2,
                    item.waybill.LotNumber,
                    item.waybill.PlanDate,
                    item.waybill.DepartDate,
                    item.waybill.Driver,
                    item.waybill.CarrierName,
                    TotalWeight = item.notices.Where(notice => notice.Weight.HasValue).Sum(notice => notice.Weight.Value),
                    //TotalParts = GetTotalPart(item.notices.Select(x => x.BoxCode).Distinct(), item.waybill.Source == CgNoticeSource.AgentBreakCustomsForIns),
                    TotalParts = GetTotalPart(item.notices.Select(x => x.BoxCode.ToUpper().Trim()).Distinct()),
                },
                notices = item.notices.OrderBy(s => s.EnterCode).Select(notice => new
                {
                    notice.TinyOrderID,
                    notice.EnterCode,
                    notice.Name,
                    notice.BoxCode,
                }).Distinct(),
            });

            string realName = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.AdminsTopView>().Single(item => item.ID == adminID).RealName;
            CgLogs_Operator logsOperator = new CgLogs_Operator();
            logsOperator.MainID = waybillID;
            logsOperator.Type = LogOperatorType.Insert;
            logsOperator.Conduct = "入库";
            logsOperator.CreatorID = adminID;
            logsOperator.CreateDate = DateTime.Now;
            logsOperator.Content = $"{realName} 在 {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 打印深圳入库单 {LogOperatorType.Insert.GetDescription()}";
            logsOperator.Enter(this.Reponsitory);

            return results.Select(item =>
            {
                object o = item;
                return o;
            }).ToArray();
        }

        /// <summary>
        /// 上架
        /// </summary>
        /// <param name="updateInfo"></param>
        public void UpdateStorageShelve(JToken updateInfo)
        {
            var storageView = from storage in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>()
                              join sorting in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Sortings>()
                              on storage.SortingID equals sorting.ID
                              where storage.WareHouseID.StartsWith("SZ")
                              select new
                              {
                                  storage.ShelveID,
                                  storage.SortingID,
                                  sorting.BoxCode,
                                  StorageID = storage.ID,
                                  sorting.WaybillID,
                              };

            List<CgLogs_Operator> logsOperatorList = new List<CgLogs_Operator>();

            foreach (var info in updateInfo)
            {
                var shelveID = "PSZ" + info["ShelveID"].Value<string>();
                var waybillID = info["WaybillID"].Value<string>();
                var boxCode = info["BoxCode"].Value<string>();
                var newBoxCode = boxCode.Length > 10 ? boxCode.Substring(10) : boxCode;
                var storages = storageView.Where(s => s.BoxCode == boxCode && s.WaybillID == waybillID);
                var adminID = info["AdminID"].Value<string>();
                string realName = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.AdminsTopView>().Single(item => item.ID == adminID).RealName;

                CgLogs_Operator logsOperator = new CgLogs_Operator();
                logsOperator.MainID = waybillID;
                logsOperator.Type = LogOperatorType.Insert;
                logsOperator.Conduct = "入库";
                logsOperator.CreatorID = adminID;
                logsOperator.CreateDate = DateTime.Now;
                logsOperator.Content = $"{realName} 在 {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 深圳入库上架 {LogOperatorType.Insert.GetDescription()}, 箱号:{newBoxCode}, 库位号:{info["ShelveID"].Value<string>()}";
                logsOperatorList.Add(logsOperator);
                if (storages != null)
                {
                    var storageIDs = storages.Select(item => item.StorageID).ToArray();
                    this.Reponsitory.Update<Layers.Data.Sqls.PvWms.Storages>(new
                    {
                        ShelveID = shelveID,
                        CreateDate = DateTime.Now,
                    }, item => storageIDs.Contains(item.ID));
                }
            }

            if (logsOperatorList.Count() > 0)
            {
                foreach (var log in logsOperatorList)
                {
                    log.Enter(this.Reponsitory);
                }
            }
        }

        #region 查询条件

        /// <summary>
        /// 根据运输批次号查询
        /// </summary>
        /// <param name="lotNumber"></param>
        /// <returns></returns>
        public CgSzSortingsView SearchByLotNumber(string lotNumber)
        {
            var waybillView = IQueryable.Cast<MyWaybill>();

            var linq = from waybill in waybillView
                       where waybill.LotNumber == lotNumber
                       select waybill;

            var view = new CgSzSortingsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };

            return view;
        }

        /// <summary>
        /// 根据CarrierName 进行搜索
        /// </summary>
        /// <param name="carrierName"></param>
        /// <returns></returns>
        public CgSzSortingsView SearchByCarrierName(string carrierName)
        {
            var waybillView = this.IQueryable.Cast<MyWaybill>();

            var linq = from waybill in waybillView
                       where waybill.CarrierName == carrierName
                       select waybill;

            var view = new CgSzSortingsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };

            return view;
        }

        /// <summary>
        /// 根据车牌号搜索
        /// </summary>
        /// <param name="carNumber">车牌号</param>
        /// <returns></returns>
        public CgSzSortingsView SearchByCarNumber(string carNumber)
        {
            var waybillView = this.IQueryable.Cast<MyWaybill>();

            var linq = from waybill in waybillView
                       where waybill.CarNumber1 == carNumber || waybill.CarNumber2 == carNumber
                       select waybill;

            var view = new CgSzSortingsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };

            return view;
        }

        /// <summary>
        /// 根据运单号过滤
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public CgSzSortingsView SearchByCode(string code)
        {
            var waybillView = this.IQueryable.Cast<MyWaybill>();

            var linq = from waybill in waybillView
                       where waybill.Code == code
                       select waybill;

            var view = new CgSzSortingsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };

            return view;
        }

        /// <summary>
        /// 根据运单号查询
        /// </summary>
        /// <param name="waybillID"></param>
        /// <returns></returns>
        public CgSzSortingsView SearchByWaybillID(string waybillID)
        {
            var waybillView = IQueryable.Cast<MyWaybill>();

            var linq = from waybill in waybillView
                       where waybill.ID.Contains(waybillID)
                       select waybill;
            var view = new CgSzSortingsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };
            return view;
        }

        /// <summary>
        /// 根据运单入库的执行状态查找
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public CgSzSortingsView SearchByStatus(params CgSortingExcuteStatus[] status)
        {
            var waybillView = this.IQueryable.Cast<MyWaybill>();

            var linq = from waybill in waybillView
                       where status.Contains(waybill.ExcuteStatus)
                       select waybill;
            var view = new CgSzSortingsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID
            };
            return view;
        }

        /// <summary>
        /// 根据入库时间查找
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public CgSzSortingsView SearchByDate(DateTime? startDate, DateTime? endDate)
        {
            Expression<Func<MyWaybill, bool>> predicate = waybill => (startDate.HasValue ? waybill.DepartDate >= startDate.Value : true)
            && (endDate.HasValue ? waybill.DepartDate < endDate.Value.AddDays(1) : true);

            var waybillView = this.IQueryable.Cast<MyWaybill>();
            var linq = waybillView.Where(predicate);

            var view = new CgSzSortingsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };

            return view;
        }

        string wareHouseID;
        /// <summary>
        /// 根据库房ID查询
        /// </summary>
        /// <param name="wareHouseID"></param>
        /// <returns></returns>
        public CgSzSortingsView SearchByWareHouseID(string wareHouseID)
        {
            this.wareHouseID = wareHouseID;

            var waybillView = this.IQueryable.Cast<MyWaybill>();
            if (string.IsNullOrEmpty(this.wareHouseID))
            {
                return new CgSzSortingsView(this.Reponsitory, waybillView);
            }

            var sources = new[] { (int)Yahv.Services.Enums.CgNoticeSource.AgentBreakCustoms,
            (int)Yahv.Services.Enums.CgNoticeSource.AgentCustomsFromStorage,
            (int)Yahv.Services.Enums.CgNoticeSource.AgentBreakCustomsForIns};

            var linq_waybillIDs = from notice in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                                  where notice.WareHouseID.StartsWith(wareHouseID) && notice.Type == (int)CgNoticeType.Enter && sources.Contains(notice.Source)
                                  select notice.WaybillID;
            var ienum_waybillIDs = linq_waybillIDs.Distinct();

            var linq = from waybill in waybillView
                       join id in ienum_waybillIDs on waybill.ID equals id
                       orderby waybill.ID descending
                       select waybill;
            var view = new CgSzSortingsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };

            return view;
        }

        /// <summary>
        /// 根据是否已处理状态来搜索
        /// </summary>
        /// <param name="done"></param>
        /// <returns></returns>
        public CgSzSortingsView SearchByShelved(bool done)
        {
            var waybillView = this.IQueryable.Cast<MyWaybill>();
            var unShelvedWaybills = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.CgSzShelvedTopView>().Select(item => item.WaybillID).Distinct().ToArray();

            //unShelvedWaybills 中为未处理的waybillIDs
            var linq = from waybill in waybillView
                       where !unShelvedWaybills.Contains(waybill.ID) == done
                       select waybill;

            var view = new CgSzSortingsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID
            };

            return view;
        }

        #endregion

        #region Helper Class

        private class MyWaybill
        {
            public string ID { get; set; }
            public string Code { get; set; }
            public string EnterCode { get; set; }
            public string Type { get; set; }
            public bool? IsOneVehicle { get; set; }
            public CgSortingExcuteStatus ExcuteStatus { get; set; }
            public string CarNumber1 { get; set; }
            public string CarNumber2 { get; set; }
            public string LotNumber { get; set; }
            public string Driver { get; set; }
            public DateTime? PlanDate { get; set; }
            public DateTime? DepartDate { get; set; }
            public decimal? TotalQuantity { get; set; }
            public decimal? ShelveQuantity { get; set; }
            public string CarrierName { get; set; }
            public CgNoticeSource Source { get; set; }
        }

        #endregion
    }
}
