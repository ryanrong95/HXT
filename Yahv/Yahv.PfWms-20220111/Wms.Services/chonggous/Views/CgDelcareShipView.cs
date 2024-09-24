using Aspose.Cells;
using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Layers.Data.Sqls.PvWms;
using Layers.Linq;
using Wms.Services.chonggous.Models;
using Yahv.Linq;
using Yahv.Payments;
using Yahv.Services;
using Yahv.Services.Enums;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Wms.Services.Enums;
using Yahv.Services.Models;

namespace Wms.Services.chonggous.Views
{
    /// <summary>
    /// 香港运输批次视图 
    /// </summary>
    /// <remarks>
    /// 生成：香港的出库单，必须包涵库房ID、交货人、收货人（公司名）
    /// 等待截单的接口
    /// 等待点击后
    /// 生成：深圳的入库通知、分拣、库存数据
    /// </remarks>

    [DisplayName("荣检视图请参考：申报视图(荣检直接要求).sql与PvWms.dbo.CgDelcaresTopView")]
    public partial class CgDelcareShipView : QueryView<object, PvWmsRepository>
    {
        #region 构造器

        public CgDelcareShipView()
        {

        }

        protected CgDelcareShipView(PvWmsRepository reponsitory, IQueryable<object> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        #endregion

        /// <summary>
        /// View 提供运输批次的视图
        /// </summary>
        /// <returns>为乔霞提供运输批次视图</returns>
        /// <remarks>
        /// 陈翰提供
        /// </remarks>
        protected override IQueryable<object> GetIQueryable()
        {

            /*
            一些疑问：
            根据最新的讨论，报关运输中就显示 已经截单？ 是否还需要：
            可出库状态

            运输类型：普通？这个是否是包不包车？
            */
            var waybillsView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.CgWaybillsTopView>();
            //new Wms.Services.Views.ServicesWaybillsTopView(this.Reponsitory);
            var carriersView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.CarriersTopView>();

            var linq = from waybill in waybillsView
                       where waybill.NoticeType == (int)CgNoticeType.Out
                             && (waybill.Source == (int)CgNoticeSource.AgentBreakCustoms
                                 || waybill.Source == (int)CgNoticeSource.AgentCustomsFromStorage
                                 || waybill.Source == (int)CgNoticeSource.AgentBreakCustomsForIns
                             )
                             && waybill.WareHouseID.StartsWith(nameof(WhSettings.HK))
                       join _carrier in carriersView on waybill.wbCarrierID equals _carrier.ID into carriers
                       from carrier in carriers.DefaultIfEmpty()
                           //where waybill.CuttingOrderStatus == (int)CgCuttingOrderStatus.Cutting
                       select new MyWaybill
                       {
                           ID = waybill.wbID,
                           PlanDate = waybill.chcdPlanDate,
                           CarrierID = carrier.ID,
                           CarrierName = carrier.Name,
                           CarNumber1 = waybill.chcdCarNumber1,
                           CarNumber2 = waybill.chcdCarNumber2,

                           DepartDate = waybill.chcdDepartDate, // 已经与荣检确定!
                           Condition = waybill.wbCondition,
                           CuttingOrderStatus = waybill.CuttingOrderStatus, //不要做处理，不然会破坏索引！
                           LotNumber = waybill.chcdLotNumber,
                           EnterCode = waybill.wbEnterCode,
                           Source = (CgNoticeSource)waybill.Source,

                           Driver = waybill.chcdDriver,
                           Phone = waybill.chcdPhone,

                           TotalWeight = waybill.wbTotalWeight,
                           TotalParts = waybill.wbTotalParts,
                           IsOnevehicle = waybill.chcdIsOnevehicle,
                           HKSealNumber = waybill.chcdHKSealNumber,
                       };
            return linq;

        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<MyWaybill> iquery = this.IQueryable.Cast<MyWaybill>().OrderByDescending(item => item.ID);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            var adminsTopView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.AdminsTopView>();
            var productsTopView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.ProductsTopView>();

            var ienums_myWaybill = iquery.ToArray();

            var waybillIds = ienums_myWaybill.Select(item => item.ID).Distinct().ToArray();
            var lotNumbers = ienums_myWaybill.Select(item => item.LotNumber).Distinct().ToArray();

            var filesView = from file in new Yahv.Services.Views.CenterFilesTopView()
                            where waybillIds.Contains(file.WaybillID) || lotNumbers.Contains(file.ShipID)
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
                                AdminID = file.AdminID,
                                InputID = file.InputID,
                                ShipID = file.ShipID,
                                Status = file.Status,
                            };
            var files = filesView.ToArray();

            if (pageIndex.HasValue && pageSize.HasValue)
            {
                var linq_results = from waybill in ienums_myWaybill
                                   select new
                                   {
                                       #region 视图

                                       ID = waybill.ID,
                                       PlanDate = waybill.PlanDate,
                                       CarrierName = waybill.CarrierName,//承运商
                                       CarNumber1 = waybill.CarNumber1,//车牌号
                                       CarNumber2 = waybill.CarNumber2,//车牌号

                                       DepartDate = waybill.DepartDate,//运输时间
                                       Condition = waybill.Condition.JsonTo<NoticeCondition>(),
                                       CuttingOrderStatus = (CgCuttingOrderStatus)(waybill.CuttingOrderStatus ?? (int)CgCuttingOrderStatus.Waiting),//截单状态
                                       LotNumber = waybill.LotNumber,//运输批次号
                                       waybill.EnterCode,

                                       //是否包车： 普通:false ,   包车:true
                                       IsOnevehicle = waybill.IsOnevehicle ?? false, //运输类型

                                       Driver = waybill.Driver,//司机姓名
                                       //电话
                                       Phone = waybill.Phone,

                                       HKSealNumber = waybill.HKSealNumber,
                                       Files = files.Where(file => file.WaybillID == waybill.ID || file.ShipID == waybill.LotNumber),

                                       #endregion
                                   };

                return new
                {
                    Total = total,
                    Size = pageSize ?? 20,
                    Index = pageIndex ?? 1,
                    Data = linq_results.ToArray(),
                };
            }


            var noticeView = from notice in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                             where notice.Type == (int)CgNoticeType.Out
                             && (notice.Source == (int)CgNoticeSource.AgentBreakCustoms
                                   || notice.Source == (int)CgNoticeSource.AgentCustomsFromStorage
                                   || notice.Source == (int)CgNoticeSource.AgentBreakCustomsForIns)
                             // 出库都是根据库存生成的，理论上出库的数据与storage应该无区别
                             // 出库的Notcie应同时包涵Input信息与Output信息和他们的ID
                             join product in productsTopView on notice.ProductID equals product.ID
                             //join input in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Inputs>() on notice.InputID equals input.ID //无用
                             join output in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Outputs>() on notice.OutputID equals output.ID
                             //申报这里会发送出库通知，第一个收到的报文后就会发出
                             //生成出库通知会自动完成：通知 拣货  出库  扣库存（用流水库做），出库通知一定要包涵：inputID与OutputID这样表示出库的是哪个进项，销项一定要来源于进项
                             //因此这里直接使用Output
                             //join log in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Logs_DeclareItem>() on output.TinyOrderID equals log.TinyOrderID
                             join picking in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Pickings>() on notice.ID equals picking.NoticeID
                             join storage in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>() on picking.StorageID equals storage.ID
                             join sorting in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Sortings>() on storage.SortingID equals sorting.ID
                             join admin in adminsTopView on picking.AdminID equals admin.ID
                             where waybillIds.Contains(notice.WaybillID)
                             select new
                             {
                                 notice.ID,
                                 notice.WaybillID,
                                 picking.BoxCode,
                                 Packer = admin.RealName,
                                 product.PartNumber,
                                 product.Manufacturer,
                                 notice.Quantity,//通知数量，拣货数量 ，这里简化因为必须符合申报数量
                                 output.Currency,
                                 output.Price,
                                 picking.Weight,
                                 output.TinyOrderID,
                                 PackDate = picking.CreateDate,
                                 notice.Source,
                                 //sortingNoticeID = sorting.NoticeID,
                                 sortingWaybillID = sorting.WaybillID
                             };

            var ienums_notice = noticeView.ToArray();

            //获取企业数据
            var waybillView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>();
            var clientView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WsClientsTopView>();
            //var sortingsNoticeID = ienums_notice.Select(item => item.sortingNoticeID).Distinct();
            //int sntotal = sortingsNoticeID.Count();
            //int sncurrent = 0;

            var sortingsWaybillID = ienums_notice.Select(item => item.sortingWaybillID).Distinct();

            var linqs_enter = from waybill in waybillView
                              join client in clientView on waybill.wbEnterCode equals client.EnterCode
                              where sortingsWaybillID.Contains(waybill.wbID)
                              select new
                              {
                                  waybillID = waybill.wbID,
                                  Code = waybill.wbEnterCode,
                                  Name = client.Name
                              };

            var ienums_enter = linqs_enter.ToArray();

            //按照与荣检商定用逗号分隔返回需要的前端数据

            var ienums_prices_group = from item in ienums_notice
                                      group item by item.Currency into groups
                                      select new
                                      {
                                          Currency = groups.Key,
                                          CurrencyName = ((Currency)groups.Key).GetCurrency().ShortName,
                                          Sum = groups.Sum(item => item.Price * item.Quantity ?? 0m)
                                      };


            var linq = from waybill in ienums_myWaybill
                       join notice in ienums_notice on waybill.ID equals notice.WaybillID into notices
                       select new
                       {
                           #region 视图

                           ID = waybill.ID,
                           PlanDate = waybill.PlanDate,
                           CarrierName = waybill.CarrierName,
                           CarNumber1 = waybill.CarNumber1,
                           CarNumber2 = waybill.CarNumber2,

                           DepartDate = waybill.DepartDate,
                           Condition = waybill.Condition.JsonTo<NoticeCondition>(),
                           CuttingOrderStatus = (CgCuttingOrderStatus)(waybill.CuttingOrderStatus ?? (int)CgCuttingOrderStatus.Waiting),
                           LotNumber = waybill.LotNumber,
                           waybill.EnterCode,
                           waybill.HKSealNumber,
                           Files = files.Where(file => file.WaybillID == waybill.ID || file.ShipID == waybill.LotNumber),
                           Notices = (from item in notices
                                      group item by new
                                      {
                                          item.TinyOrderID,
                                          item.Source
                                      } into groups
                                      let waybillID = groups.Select(item => item.sortingWaybillID).FirstOrDefault()
                                      let Enter = ienums_enter.FirstOrDefault(item => item.waybillID == waybillID)
                                      select new
                                      {
                                          groups.Key.TinyOrderID,
                                          groups.Key.Source,
                                          Count = groups.Count(),
                                          ClientName = Enter.Name,
                                          EnterCode = Enter.Code,
                                          PackDate = (from box in groups
                                                      group box by new
                                                      {
                                                          box.BoxCode,
                                                          PackDate = box.PackDate.ToString("yyyy-MM-dd")
                                                      } into boxgroups
                                                      select new
                                                      {
                                                          boxgroups.Key.BoxCode,
                                                          boxgroups.Key.PackDate
                                                      })
                                      }),
                           //总重量
                           TotalWeight = notices.Sum(item => item.Weight ?? 0m),
                           //总件数
                           //TotalParts = CgSzSortingsView.GetTotalPart(notices.Select(x => x.BoxCode).Distinct(), waybill.Source == CgNoticeSource.AgentBreakCustomsForIns),
                           TotalParts = CgSzSortingsView.GetTotalPart(notices.Where(x => x.BoxCode != null).Select(x => x.BoxCode.ToUpper().Trim()).Distinct()),
                           //总数量
                           TotalQuantity = notices.Sum(item => item.Quantity),
                           //总条数
                           TotalRecord = notices.Count(),
                           //总金额（暂时没有考虑币种问题）
                           TotalPrice = notices.Sum(item => item.Price * item.Quantity ?? 0m),
                           //与荣检商议后添加
                           TotalPrices = string.Join(",", ienums_prices_group.Select(price => $"{price.CurrencyName}:{price.Sum}")),
                           Currency = ((Currency)(notices.FirstOrDefault() != null ? notices.FirstOrDefault().Currency : 0)).GetCurrency().ShortName,
                           //是否包车： 普通:false ,   包车:true
                           IsOnevehicle = waybill.IsOnevehicle ?? false,

                           Driver = waybill.Driver,
                           //电话
                           Phone = waybill.Phone

                           #endregion
                       };

            // 为了计算并添加LQuantity
            var results = linq;

            if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
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
        /// 截单完成
        /// </summary>
        /// <param name="lotNumber">运输批次号</param>
        /// <remarks>
        /// 库房操作
        /// </remarks>
        public void Completed(string lotNumber, string adminID)
        {
            using (var reponsitory = new PvCenterReponsitory())
            {
                var waybillIds = reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.WayChcd>()
                     .Where(item => item.LotNumber == lotNumber).Select(item => item.ID).ToArray();

                if (waybillIds.Length == 0)
                {
                    throw new NotImplementedException("没有实现没有waybill的情况");
                }

                //可能没有
                var admin = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.AdminsTopView>().SingleOrDefault(item => item.ID == adminID);

                if (string.IsNullOrWhiteSpace(adminID) || admin == null)
                {
                    throw new ArgumentNullException($"依据{nameof(adminID)}参数值：{adminID}没有获取到期望的数据!");
                }

                // 当前在香港库房--报关运输详情页面, 点击完成出库时, 插入任务到TaskPool中去, 不再通过触发器自动生成任务
                reponsitory.Update<Layers.Data.Sqls.PvCenter.Waybills>(new
                {
                    CuttingOrderStatus = (int)CgCuttingOrderStatus.Completed,
                }, item => waybillIds.Contains(item.ID));

                reponsitory.Update<Layers.Data.Sqls.PvCenter.WayChcd>(new
                {
                    DepartDate = DateTime.Now,
                }, item => waybillIds.Contains(item.ID));

                reponsitory.Insert(new Layers.Data.Sqls.PvCenter.TasksPool
                {
                    ID = Guid.NewGuid().ToString(),
                    Name = "香港报关出库",
                    MainID = lotNumber,
                    CreateDate = DateTime.Now,
                });

                string[] keys = Layers.Data.PKeySigner.Series(PkeyType.Logs_Operator, waybillIds.Length);
                this.Reponsitory.Insert(waybillIds.Select((waybillID, index) => new Logs_Operator
                {
                    ID = keys[index],
                    MainID = waybillID,
                    Type = LogOperatorType.Insert.ToString(),
                    Conduct = "出库",
                    CreatorID = adminID,
                    CreateDate = DateTime.Now,
                    Content = $"{admin.RealName} 在 {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {LogOperatorType.Insert.GetDescription()} 出库, 报关运输批次号: {lotNumber}"
                }));


                //List<CgLogs_Operator> logsOperatorList = new List<CgLogs_Operator>();

                //foreach (var waybillId in waybillIds)
                //{
                //    CgLogs_Operator logsOperator = new CgLogs_Operator();
                //    logsOperator.MainID = waybillId;
                //    logsOperator.Type = LogOperatorType.Insert;
                //    logsOperator.Conduct = "出库";
                //    logsOperator.CreatorID = adminID;
                //    logsOperator.CreateDate = DateTime.Now;
                //    logsOperator.Content = $"{realName} 在 {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {LogOperatorType.Insert.GetDescription()} 出库, 报关运输批次号: {lotNumber}";
                //    logsOperatorList.Add(logsOperator);
                //}

                //if (logsOperatorList.Count() > 0)
                //{
                //    foreach (var log in logsOperatorList)
                //    {
                //        log.Enter(this.Reponsitory);
                //    }
                //}
            }
        }

        #region 搜索相关

        /// <summary>
        /// 运输批次号搜索
        /// </summary>
        /// <param name="number">运输批次号</param>
        /// <returns>运输批次号的运单</returns>
        public CgDelcareShipView SearchByLotNumber(string number)
        {
            var myWaybill = this.IQueryable.Cast<MyWaybill>().Where(item => item.LotNumber.Contains(number));
            var linq = myWaybill;
            var view = new CgDelcareShipView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 承运商查询
        /// </summary>
        /// <param name="realName">承运商名称</param>
        /// <returns>承运商查询的运单</returns>
        public CgDelcareShipView SearchByCarrier(string name)
        {
            var myWaybill = this.IQueryable.Cast<MyWaybill>().Where(item => item.CarrierID.Contains(name));
            var linq = myWaybill;
            var view = new CgDelcareShipView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 运输时间查询
        /// </summary>
        /// <param name="realName">开始时间</param>
        /// <returns>制定时间的批次订单</returns>
        public CgDelcareShipView SearchByShipStartDate(DateTime date)
        {
            var myWaybill = this.IQueryable.Cast<MyWaybill>().Where(item => item.DepartDate >= date);
            var linq = myWaybill;
            var view = new CgDelcareShipView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 运输时间查询
        /// </summary>
        /// <param name="realName">结束时间</param>
        /// <returns>制定时间的批次订单</returns>
        public CgDelcareShipView SearchByShipEndDate(DateTime date)
        {
            var myWaybill = this.IQueryable.Cast<MyWaybill>().Where(item => item.DepartDate < date);
            var linq = myWaybill;
            var view = new CgDelcareShipView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 司机查询
        /// </summary>
        /// <param name="realName">司机姓名</param>
        /// <returns>司机批次号的运单</returns>
        public CgDelcareShipView SearchByDriver(string name)
        {
            var myWaybill = this.IQueryable.Cast<MyWaybill>().Where(item => item.Driver.Contains(name));
            var linq = myWaybill;
            var view = new CgDelcareShipView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 司机查询
        /// </summary>
        /// <param name="realName">司机姓名</param>
        /// <returns>司机批次号的运单</returns>
        public CgDelcareShipView SearchByStatus(CgCuttingOrderStatus status)
        {
            var myWaybill = this.IQueryable.Cast<MyWaybill>().Where(item => item.CuttingOrderStatus == (int)status);
            var linq = myWaybill;
            var view = new CgDelcareShipView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据ID获取
        /// </summary>
        /// <param name="waybillID"></param>
        /// <returns></returns>
        public object SearchByID(string waybillID)
        {
            var myWaybill = this.IQueryable.Cast<MyWaybill>().Where(item => item.ID == waybillID);
            var view = new CgDelcareShipView(this.Reponsitory, myWaybill);
            var objects = view.ToMyPage() as object[];
            return objects.SingleOrDefault();
        }

        #endregion

        #region 内部帮助类

        /// <summary>
        /// 内部帮助获取notice统计信息
        /// </summary>
        class MySNotice
        {
            //public string SortingNoticeID { get; set; }
            public string Code { get; set; }
            public string Name { get; set; }


        }


        private class MyWaybill
        {
            public string ID { get; set; }
            public DateTime? PlanDate { get; set; }
            public string CarrierID { get; set; }
            public string CarrierName { get; set; }
            public string LotNumber { get; set; }

            public string CarNumber1 { get; set; }
            public string CarNumber2 { get; set; }
            public string Driver { get; set; }
            public DateTime? DepartDate { get; set; }
            public string EnterCode { get; set; }
            public string Condition { get; set; }
            public int? CuttingOrderStatus { get; set; }

            public CgNoticeSource Source { get; set; }
            public string Phone { get; set; }

            /// <summary>
            /// 总件数
            /// </summary>
            public int? TotalParts { get; set; }

            /// <summary>
            /// 总重量
            /// </summary>
            public decimal? TotalWeight { get; set; }

            /// <summary>
            /// 是否包车
            /// </summary>
            public bool? IsOnevehicle { get; set; }

            /// <summary>
            /// 香港库房封条号
            /// </summary>
            public string HKSealNumber { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        [Obsolete("商议使用已经决定：暂不添加")]
        private class CgDelcareCutting1
        {
            /// <summary>
            /// ID
            /// </summary>
            public string ID { get; set; } = string.Empty;

            /// <summary>
            /// 【车辆】香港车牌号
            /// </summary>
            public string HKLicense { get; set; } = string.Empty;

            /// <summary>
            /// 【司机】司机姓名
            /// </summary>
            public string DriverName { get; set; } = string.Empty;

            /// <summary>
            /// 【司机】司机证件编码 Drivers.Licence
            /// </summary>
            public string DriverCode { get; set; } = string.Empty;

            /// <summary>
            /// 【承运商】承运商简称
            /// </summary>
            public string CarrierCode { get; set; } = string.Empty;

            /// <summary>
            /// 【Voyage】运输时间
            /// </summary>
            public DateTime? TransportTime { get; set; }

            /// <summary>
            /// 【Voyage】运输类型
            /// </summary>
            public int Type { get; set; }

            /// <summary>
            /// 【Voyage】截单状态
            /// </summary>
            public int CutStatus { get; set; }

            /// <summary>
            /// 【Voyage】香港清关状态
            /// </summary>
            public bool HKDeclareStatus { get; set; }

            /// <summary>
            /// 【Voyage】Status
            /// </summary>
            public int Status { get; set; }

            /// <summary>
            /// 【Voyage】CreateTime
            /// </summary>
            public DateTime CreateTime { get; set; }

            /// <summary>
            /// 【Voyage】UpdateDate
            /// </summary>
            public DateTime UpdateDate { get; set; }

            /// <summary>
            /// 【Voyage】Summary
            /// </summary>
            public string Summary { get; set; } = string.Empty;

            /// <summary>
            /// 【承运商】承运商类型
            /// </summary>
            public int? CarrierType { get; set; }

            /// <summary>
            /// 【承运商】名称
            /// </summary>
            public string CarrierName { get; set; } = string.Empty;

            /// <summary>
            /// 【承运商】查询标记
            /// </summary>
            public string CarrierQueryMark { get; set; } = string.Empty;

            /// <summary>
            /// 【承运商】联系电话
            /// </summary>
            public string ContactMobile { get; set; } = string.Empty;

            /// <summary>
            /// 【承运商】承运商地址
            /// </summary>
            public string CarrierAddress { get; set; } = string.Empty;

            /// <summary>
            /// 【承运商】联系人
            /// </summary>
            public string ContactName { get; set; } = string.Empty;

            /// <summary>
            /// 【承运商】传真
            /// </summary>
            public string ContactFax { get; set; } = string.Empty;

            /// <summary>
            /// 【车辆】车辆类型
            /// </summary>
            public int? VehicleType { get; set; }

            /// <summary>
            /// 【车辆】车牌号
            /// </summary>
            public string VehicleLicence { get; set; } = string.Empty;

            /// <summary>
            /// 【车辆】车重
            /// </summary>
            public int VehicleWeight { get; set; }

            /// <summary>
            /// 【司机】大陆手机号
            /// </summary>
            public string DriverMobile { get; set; } = string.Empty;

            /// <summary>
            /// 【司机】司机海关编号
            /// </summary>
            public string DriverHSCode { get; set; } = string.Empty;

            /// <summary>
            /// 【司机】香港手机号
            /// </summary>
            public string DriverHKMobile { get; set; } = string.Empty;

            /// <summary>
            /// 【司机】司机卡号
            /// </summary>
            public string DriverCardNo { get; set; } = string.Empty;

            /// <summary>
            /// 【司机】口岸电子编号
            /// </summary>
            public string DriverPortElecNo { get; set; } = string.Empty;

            /// <summary>
            /// 【司机】寮步密码
            /// </summary>
            public string DriverLaoPaoCode { get; set; } = string.Empty;
        }

        /// <summary>
        /// 库房费用
        /// </summary>
        class Premium
        {
            /// <summary>
            /// 费用ID
            /// </summary>
            public string ID { get; set; }

            /// <summary>
            /// 小订单号
            /// </summary>
            public string TinyOrderID { get; set; }

            /// <summary>
            /// 添加人
            /// </summary>
            public string AdminID { get; set; }

            /// <summary>
            /// 库房费用类型
            /// </summary>
            public WarehousePremiumType WhesFeeType { get; set; }

            /// <summary>
            /// 数量 当前默认：1
            /// </summary>

            public int Count { get; set; } = 1;

            /// <summary>
            /// 单价 当前就写库房的金额
            /// </summary>
            public decimal UnitPrice { get; set; }

            /// <summary>
            /// 币种：CNY HKD
            /// </summary>
            public string Currency { get; set; }

            /// <summary>
            /// 1:现金(实收)  2:非现金，记账(应收)
            /// </summary>
            public WhsePaymentType PaymentType { get; set; }

            /// <summary>
            /// 创建时间
            /// </summary>
            public DateTime CreateDate { get; set; }

            /// <summary>
            /// 备注
            /// </summary>
            public string Summary { get; set; }
        }

        /// <summary>
        /// 接口：库房费用调用数据要求
        /// </summary>
        class WhesPremium
        {
            public WhesPremium(List<Premium> premiums)
            {
                this.Premiums = premiums;
            }
            /// <summary>
            /// 
            /// </summary>
            public List<Premium> Premiums { get; set; }
        }
        enum WhsePaymentType
        {
            /// <summary>
            /// 现金
            /// </summary>
            [Description("现金")]
            Cash = 1,

            /// <summary>
            /// 非现金
            /// </summary>
            [Description("非现金")]
            UnCash = 2,
        }

        /// <summary>
        /// 库房费用类型
        /// </summary>
        enum WarehousePremiumType
        {
            /// <summary>
            /// 入仓费
            /// </summary>
            [Description("入仓费")]
            EntryFee = 1,

            /// <summary>
            /// 仓储费
            /// </summary>
            [Description("仓储费")]
            StorageFee = 2,

            /// <summary>
            /// 收货异常费用
            /// </summary>
            [Description("收货异常费用")]
            UnNormalFee = 3,

            /// <summary>
            /// 处理标签费
            /// </summary>
            [Description("处理标签费")]
            ProcessLabelFee = 4,

            /// <summary>
            /// 换箱费
            /// </summary>
            [Description("换箱费")]
            ChangeBoxFee = 5,

            /// <summary>
            /// 垫付快递费
            /// </summary>
            [Description("垫付快递费")]
            ExpressFee = 6,

            /// <summary>
            /// 提货费
            /// </summary>
            [Description("提货费")]
            DeliverFee = 7,

            /// <summary>
            /// 垫付登记费
            /// </summary>
            [Description("垫付登记费")]
            RegisterFee = 8,

            /// <summary>
            /// 垫付隧道费
            /// </summary>
            [Description("垫付隧道费")]
            TunnelFee = 9,

            /// <summary>
            /// 垫付车场费
            /// </summary>
            [Description("垫付车场费")]
            parkingFee = 10,

            /// <summary>
            /// 超重费
            /// </summary>
            [Description("超重费")]
            OverweightFee = 11,

            /// <summary>
            /// 包车费（单独一车）
            /// </summary>
            [Description("包车费（单独一车）")]
            CharterFee = 12,

            /// <summary>
            /// 其他
            /// </summary>
            [Description("其他")]
            Other = 13,

            /// <summary>
            /// 大陆来货清关
            /// </summary>
            [Description("大陆来货清关")]
            MainlandClearance = 14,
        }
        #endregion

        #region 内部帮助方法

        private WarehousePremiumType ConverterSubject(string subject)
        {
            WarehousePremiumType result = WarehousePremiumType.Other;

            switch (subject)
            {
                case "入仓费(港币)":
                    result = WarehousePremiumType.EntryFee;
                    break;
                case "代增加/更换纸箱":
                    result = WarehousePremiumType.ChangeBoxFee;
                    break;
                case "大陆来货清关费":
                    result = WarehousePremiumType.MainlandClearance;
                    break;
                case "提、送货费":
                    result = WarehousePremiumType.DeliverFee;
                    break;
                case "标签更换费":
                case "标签处理费":
                case "撕标签":
                    result = WarehousePremiumType.ProcessLabelFee;
                    break;
                case "超重货物收费":
                    result = WarehousePremiumType.OverweightFee;
                    break;
                case "理货困难收费":
                    result = WarehousePremiumType.UnNormalFee;
                    break;
                case "登记费":
                    result = WarehousePremiumType.RegisterFee;
                    break;
                case "停车场杂费":
                    result = WarehousePremiumType.parkingFee;
                    break;
                case "隧道费":
                    result = WarehousePremiumType.TunnelFee;
                    break;
                default:
                    break;
            }

            return result;
        }
        #endregion

        #region 董建帮助

        /// <summary>
        /// 报关成功
        /// 生成香港库房出库通知
        /// </summary>
        public void AutoHkExitNotice_Old(CgDelcare delcare)
        {
            //首先要解决出库的waybill与WayChcd的来源，这样：库房ID、交货人、收货人（公司名）、司机信息，就都解决了
            //基于运输批次号生成实际的出库通知，一个运输批次一定会生成多个订单的通知，因此，Waybill.OrderID可以为空
            //基于以上讨论，具体参数要根据情况做
            using (var reponsitory = new PvWmsRepository())
            {

                #region 生成运单数据
                var cgWaybill = delcare.HkExitWaybill;
                var waybillid = string.Empty;
                //判断改运输批次号是否已经存在运单数据不更新（放在下面的截单接口(DelcareCutting)更新）
                var waybill = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>()
                    .Where(item => item.chcdLotNumber == cgWaybill.WayChcd.LotNumber && item.NoticeType == (int)CgNoticeType.Out).FirstOrDefault();

                //修正业务来源
                var originSource = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>().
                       Where(item => item.OrderID == cgWaybill.OrderID).
                       Select(item => item.Source).
                       FirstOrDefault();

                if (cgWaybill.Source == CgNoticeSource.AgentBreakCustoms
                    || cgWaybill.Source == CgNoticeSource.AgentCustomsFromStorage)
                {
                    cgWaybill.Source = (CgNoticeSource)originSource;
                    delcare.Notices.ForEach(item =>
                    {
                        item.Source = (NoticeSource)cgWaybill.Source;
                    });
                }

                if (waybill == null)
                {
                    //香港出库运单
                    CgWaybill HkWaybill = new CgWaybill();
                    HkWaybill.Type = WaybillType.DeliveryToWarehouse;
                    HkWaybill.Consignee = cgWaybill.Consignee;
                    HkWaybill.Consignor = cgWaybill.Consignor;
                    HkWaybill.FreightPayer = cgWaybill.FreightPayer;
                    HkWaybill.EnterCode = cgWaybill.EnterCode;
                    HkWaybill.CreatorID = Npc.Robot.Obtain();
                    HkWaybill.IsClearance = false;
                    HkWaybill.WayCharge = null;
                    HkWaybill.WayLoading = null;
                    HkWaybill.WayChcd = cgWaybill.WayChcd;
                    //HkWaybill.OrderID = cgWaybill.OrderID;//一般有多个订单，所有可为空
                    //商定后更新为null
                    HkWaybill.OrderID = null;

                    HkWaybill.Source = cgWaybill.Source;
                    HkWaybill.NoticeType = CgNoticeType.Out;
                    HkWaybill.ExcuteStatus = (int)CgPickingExcuteStatus.Completed;//入库运单的完成状态!
                    HkWaybill.CuttingOrderStatus = (int)Enums.CutStatus.UnCutting;
                    HkWaybill.TotalParts = cgWaybill.TotalParts;
                    HkWaybill.TotalWeight = cgWaybill.TotalWeight;
                    HkWaybill.TotalVolume = cgWaybill.TotalVolume;
                    HkWaybill.AppointTime = cgWaybill.AppointTime;
                    HkWaybill.Enter();

                    waybillid = HkWaybill.ID;
                    //深圳入库运单
                    CgWaybill SzWaybill = new CgWaybill();
                    SzWaybill.Type = WaybillType.DeliveryToWarehouse;
                    SzWaybill.Consignee = cgWaybill.Consignee;
                    SzWaybill.Consignor = cgWaybill.Consignor;
                    SzWaybill.FreightPayer = cgWaybill.FreightPayer;
                    SzWaybill.EnterCode = cgWaybill.EnterCode;
                    SzWaybill.CreatorID = Npc.Robot.Obtain();
                    SzWaybill.IsClearance = false;
                    SzWaybill.WayCharge = null;
                    SzWaybill.WayLoading = null;
                    SzWaybill.WayChcd = cgWaybill.WayChcd;
                    //SzWaybill.OrderID = cgWaybill.OrderID;
                    //商定后更新为null
                    HkWaybill.OrderID = null;
                    SzWaybill.AppointTime = cgWaybill.AppointTime;

                    SzWaybill.Source = cgWaybill.Source;
                    SzWaybill.NoticeType = CgNoticeType.Enter;
                    SzWaybill.ExcuteStatus = (int)CgSortingExcuteStatus.Sorting;
                    HkWaybill.CuttingOrderStatus = (int)Enums.CutStatus.UnCutting;

                    SzWaybill.TotalParts = cgWaybill.TotalParts;
                    SzWaybill.TotalWeight = cgWaybill.TotalWeight;
                    SzWaybill.TotalVolume = cgWaybill.TotalVolume;

                    SzWaybill.Enter();
                }
                else
                {
                    waybillid = waybill.wbID;
                }

                #endregion

                #region 保存产品数据

                Task task = new Task((sender) =>
                {
                    var _delcare = sender as CgDelcare;

                    foreach (var notice in _delcare.Notices)
                    {
                        Yahv.Services.Views.ProductsTopView<PvWsOrderReponsitory>.Enter(notice.Product);
                    }
                }, delcare);
                task.Start();

                #endregion

                #region 生成销项、通知、拣货数据

                Task task1 = new Task((sender) =>
                {
                    var _delcare = sender as CgDelcare;
                    foreach (var notice in _delcare.Notices)
                    {
                        var sortingView = from storage in reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>()
                                          join sorting in reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Sortings>() on storage.SortingID equals sorting.ID
                                          where storage.ID == notice.StorageID
                                          select sorting;

                        var boxCode = sortingView.First().BoxCode;

                        #region 生成出库销项数据（转报关已经存在）
                        Layers.Data.Sqls.PvWms.Outputs output = null;
                        if (string.IsNullOrEmpty(notice.Output.ID))
                        {
                            output = new Layers.Data.Sqls.PvWms.Outputs()
                            {
                                ID = Layers.Data.PKeySigner.Pick(PkeyType.Outputs),
                                InputID = notice.Output.InputID,
                                OrderID = notice.Output.OrderID,
                                TinyOrderID = notice.Output.TinyOrderID,
                                ItemID = notice.Output.ItemID,
                                OwnerID = string.Empty,
                                PurchaserID = notice.Output.PurchaserID,
                                Currency = (int?)notice.Output.Currency,
                                Price = notice.Output.Price,
                                CreateDate = DateTime.Now,
                                ReviewerID = notice.Output.ReviewerID,
                                TrackerID = notice.Output.TrackerID
                            };
                            //回填OutputID,给后面赋值使用
                            notice.OutputID = output.ID;
                        }
                        #endregion

                        #region 生成出库通知
                        var newnotice = new Layers.Data.Sqls.PvWms.Notices()
                        {
                            ID = Layers.Data.PKeySigner.Pick(PkeyType.Notices),
                            Type = (int)CgNoticeType.Out,
                            WareHouseID = notice.WareHouseID,
                            WaybillID = waybillid,
                            InputID = notice.InputID,
                            OutputID = notice.OutputID,
                            ProductID = notice.Product.ID,
                            DateCode = notice.DateCode,
                            Origin = notice.Origin,
                            Quantity = notice.Quantity,
                            Conditions = new NoticeCondition().Json(),
                            Status = (int)NoticesStatus.Waiting,
                            Source = (int)notice.Source,
                            Target = (int)NoticesTarget.Default,
                            Weight = notice.Weight,
                            NetWeight = notice.NetWeight,
                            Volume = notice.Volume,
                            BoxCode = boxCode,
                            ShelveID = notice.ShelveID,
                            BoxingSpecs = notice.BoxingSpecs,
                            CreateDate = DateTime.Now,
                            StorageID = notice.StorageID,
                            CustomsName = notice.CustomsName
                        };

                        #endregion

                        #region 生成拣货数据
                        var picking = new Layers.Data.Sqls.PvWms.Pickings
                        {
                            ID = Layers.Data.PKeySigner.Pick(PkeyType.Pickings),
                            StorageID = notice.StorageID,
                            NoticeID = newnotice.ID,
                            OutputID = notice.OutputID,
                            BoxCode = boxCode,
                            Quantity = notice.Quantity,
                            AdminID = Npc.Robot.Obtain(),
                            CreateDate = DateTime.Now,
                            Weight = notice.Weight,
                            NetWeight = notice.NetWeight,
                            Volume = notice.Volume,
                        };
                        #endregion

                        if (output != null)
                        {
                            reponsitory.Insert(output);
                        }
                        reponsitory.Insert(newnotice);
                        reponsitory.Insert(picking);
                    }
                }, delcare);
                task1.Start();

                #endregion

                Task.WaitAll(task, task1);
            }

            //依照马莲要求：香港出库会变 待发货 状态
            #region 更新订单状态Logs_PvWsOrder
            using (var pvcenterReponsitory = new PvCenterReponsitory())
            {
                foreach (var orderID in delcare.Notices.Select(notice => notice.Output.OrderID))
                {
                    pvcenterReponsitory.Update<Layers.Data.Sqls.PvCenter.Logs_PvWsOrder>(new
                    {
                        IsCurrent = false,
                    }, item => item.Type == (int)OrderStatusType.MainStatus && item.MainID == orderID);

                    //delcare.Notices.Select(.Output.TrackerID)

                    //delcare.Notices.First().BoxCode 可从装箱人这里取出

                    pvcenterReponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_PvWsOrder
                    {
                        ID = Guid.NewGuid().ToString(),
                        MainID = orderID,
                        Type = (int)OrderStatusType.MainStatus, //订单支付状态，  
                        Status = (int)CgOrderStatus.待收货,
                        CreateDate = DateTime.Now,
                        CreatorID = Npc.Robot.Obtain(),
                        IsCurrent = true,
                    });
                }

            }
            #endregion

        }


        /// <summary>
        /// 单一窗口调用
        /// </summary>
        /// <param name="delcare"></param>
        public void AutoHkExitNotice(CgDelcare delcare)
        {
            using (var reponsitory = new PvCenterReponsitory())
            {
                string id = Layers.Data.PKeySigner.Pick(Wms.Services.PkeyType.TasksPool);
                reponsitory.Insert(new Layers.Data.Sqls.PvCenter.TasksPool
                {
                    Context = delcare.Json(),
                    CreateDate = DateTime.Now,
                    ID = id,
                    MainID = delcare?.HkExitWaybill?.WayChcd?.LotNumber,
                    Name = "单一窗口申报后深圳出库"
                });
            }
        }


        #region 香港库房重构修改前
        
        /// <summary>
        /// 报关成功
        /// 生成香港库房出库通知
        /// 按照陈经理要求, 在生成香港库房出库通知时, 把对应的 报关香港出库, 深圳库房的入库数据全部补全
        /// </summary>
        public void AutoHkExitNoticeForTask(CgDelcare delcare)
        {
            //首先要解决出库的waybill与WayChcd的来源，这样：库房ID、交货人、收货人（公司名）、司机信息，就都解决了
            //基于运输批次号生成实际的出库通知，一个运输批次一定会生成多个订单的通知，因此，Waybill.OrderID可以为空
            //基于以上讨论，具体参数要根据情况做
            var waybillid = string.Empty;  //香港出库WaybillID
            var szwaybillid = string.Empty;//深圳入库WaybillID
            var cgWaybill = delcare.HkExitWaybill;
            //var cgNotices = delcare.Notices;
            var hkListOutNoticesId = new List<string>();
            StringBuilder sqlTrunacateBulk = new StringBuilder();
            sqlTrunacateBulk.AppendLine($@"Delete from [{nameof(Layers.Data.Sqls.PvWms.Pickings_Temp)}]");
            sqlTrunacateBulk.AppendLine($@"Delete from [{nameof(Layers.Data.Sqls.PvWms.Storages_Temp)}]");
            sqlTrunacateBulk.AppendLine($@"Delete from [{nameof(Layers.Data.Sqls.PvWms.Sortings_Temp)}]");
            sqlTrunacateBulk.AppendLine($@"Delete from [{nameof(Layers.Data.Sqls.PvWms.Outputs_Temp)}]");
            sqlTrunacateBulk.AppendLine($@"Delete from [{nameof(Layers.Data.Sqls.PvWms.Notices_Temp)}]");
            sqlTrunacateBulk.AppendLine($@"Delete from [{nameof(Layers.Data.Sqls.PvWms.Inputs_Temp)}]");

            #region 香港库房出库，深圳库房入库，深圳内单出库
            try
            {
                using (var reponsitory = new PvWmsRepository())
                using (var centerreponsitory = new PvCenterReponsitory())
                {
                    #region 生成运单数据
                    //var cgWaybill = delcare.HkExitWaybill;

                    //判断改运输批次号是否已经存在运单数据不更新（放在下面的截单接口(DelcareCutting)更新）
                    var waybill = centerreponsitory.GetTable<Layers.Data.Sqls.PvCenter.WaybillsTopView>()
                        .Where(item => item.chcdLotNumber == cgWaybill.WayChcd.LotNumber && item.NoticeType == (int)CgNoticeType.Out).FirstOrDefault();

                    //修正业务来源
                    var originSource = centerreponsitory.GetTable<Layers.Data.Sqls.PvCenter.WaybillsTopView>().
                           Where(item => item.OrderID == cgWaybill.OrderID).
                           Select(item => item.Source).
                           FirstOrDefault();

                    if (cgWaybill.Source == CgNoticeSource.AgentBreakCustoms
                        || cgWaybill.Source == CgNoticeSource.AgentCustomsFromStorage)
                    {
                        cgWaybill.Source = (CgNoticeSource)originSource;
                        delcare.Notices.ForEach(item =>
                        {
                            item.Source = (NoticeSource)cgWaybill.Source;
                        });
                    }

                    if (waybill == null)
                    {
                        //香港出库运单
                        CgWaybill HkWaybill = new CgWaybill();
                        HkWaybill.Type = WaybillType.DeliveryToWarehouse;
                        HkWaybill.Consignee = cgWaybill.Consignee;
                        HkWaybill.Consignor = cgWaybill.Consignor;
                        HkWaybill.FreightPayer = cgWaybill.FreightPayer;
                        HkWaybill.EnterCode = cgWaybill.EnterCode;
                        HkWaybill.CreatorID = Npc.Robot.Obtain();
                        HkWaybill.IsClearance = false;
                        HkWaybill.WayCharge = null;
                        HkWaybill.WayLoading = null;
                        HkWaybill.WayChcd = cgWaybill.WayChcd;
                        //HkWaybill.OrderID = cgWaybill.OrderID;//一般有多个订单，所有可为空
                        //商定后更新为null
                        HkWaybill.OrderID = null;
                        HkWaybill.Status = CgWaybillStatus.Normal;

                        HkWaybill.Source = cgWaybill.Source;
                        HkWaybill.NoticeType = CgNoticeType.Out;
                        HkWaybill.ExcuteStatus = (int)CgPickingExcuteStatus.Completed;//入库运单的完成状态!
                        HkWaybill.CuttingOrderStatus = (int)Enums.CutStatus.UnCutting;
                        HkWaybill.TotalParts = cgWaybill.TotalParts;
                        HkWaybill.TotalWeight = cgWaybill.TotalWeight;
                        HkWaybill.TotalVolume = cgWaybill.TotalVolume;
                        HkWaybill.AppointTime = cgWaybill.AppointTime;
                        HkWaybill.Enter(centerreponsitory);

                        waybillid = HkWaybill.ID;
                        //深圳入库运单
                        CgWaybill SzWaybill = new CgWaybill();
                        SzWaybill.Type = WaybillType.DeliveryToWarehouse;
                        SzWaybill.Consignee = cgWaybill.Consignee;
                        SzWaybill.Consignor = cgWaybill.Consignor;
                        SzWaybill.FreightPayer = cgWaybill.FreightPayer;
                        SzWaybill.EnterCode = cgWaybill.EnterCode;
                        SzWaybill.CreatorID = Npc.Robot.Obtain();
                        SzWaybill.IsClearance = false;
                        SzWaybill.WayCharge = null;
                        SzWaybill.WayLoading = null;
                        SzWaybill.WayChcd = cgWaybill.WayChcd;
                        //SzWaybill.OrderID = cgWaybill.OrderID;
                        //商定后更新为null
                        SzWaybill.OrderID = null;
                        SzWaybill.AppointTime = cgWaybill.AppointTime;
                        SzWaybill.Status = CgWaybillStatus.Normal;
                        SzWaybill.Source = cgWaybill.Source;
                        SzWaybill.NoticeType = CgNoticeType.Enter;
                        SzWaybill.ExcuteStatus = (int)CgSortingExcuteStatus.Sorting;
                        SzWaybill.CuttingOrderStatus = (int)Enums.CutStatus.UnCutting;

                        SzWaybill.TotalParts = cgWaybill.TotalParts;
                        SzWaybill.TotalWeight = cgWaybill.TotalWeight;
                        SzWaybill.TotalVolume = cgWaybill.TotalVolume;

                        SzWaybill.Enter(centerreponsitory);
                        szwaybillid = SzWaybill.ID;
                    }
                    else
                    {
                        waybillid = waybill.wbID;
                        szwaybillid = centerreponsitory.GetTable<Layers.Data.Sqls.PvCenter.WaybillsTopView>()
                        .Where(item => item.chcdLotNumber == cgWaybill.WayChcd.LotNumber && item.NoticeType == (int)CgNoticeType.Enter).FirstOrDefault().wbID;
                    }

                    #endregion

                    #region 保存产品数据

                    Task task = new Task((sender) =>
                    {
                        var _delcare = sender as CgDelcare;

                        foreach (var notice in _delcare.Notices)
                        {
                            Yahv.Services.Views.ProductsTopView<PvWsOrderReponsitory>.Enter(notice.Product);
                        }
                    }, delcare);
                    task.Start();

                    #endregion


                    #region 生成销项、通知、拣货数据

                    Task task1 = new Task((sender) =>
                    {
                        // 使用事务处理确保临时表数据完整
                        using (var tran = reponsitory.OpenTransaction())
                        {
                            var _delcare = sender as CgDelcare;
                            foreach (var notice in _delcare.Notices)
                            {
                                var log_storageView = from storage in reponsitory.GetTable<Layers.Data.Sqls.PvWms.Storages>()
                                                      join log_storage in reponsitory.GetTable<Layers.Data.Sqls.PvWms.Logs_Storage>()
                                                      on new { StorageID = storage.ID, IsCurrent = true } equals new { StorageID = log_storage.StorageID, IsCurrent = log_storage.IsCurrent }
                                                      //join sorting in reponsitory.GetTable<Layers.Data.Sqls.PvWms.Sortings>() on storage.SortingID equals sorting.ID
                                                      where storage.ID == notice.StorageID
                                                      select log_storage;

                                var boxCode = log_storageView.First().BoxCode;

                                #region 生成出库销项数据（转报关已经存在）
                                Layers.Data.Sqls.PvWms.Outputs_Temp output = null;
                                if (string.IsNullOrEmpty(notice.Output.ID))
                                {
                                    output = new Layers.Data.Sqls.PvWms.Outputs_Temp()
                                    {
                                        ID = Layers.Data.PKeySigner.Pick(PkeyType.Outputs),
                                        InputID = notice.Output.InputID,
                                        OrderID = notice.Output.OrderID,
                                        TinyOrderID = notice.Output.TinyOrderID,
                                        ItemID = notice.Output.ItemID,
                                        OwnerID = string.Empty,
                                        PurchaserID = notice.Output.PurchaserID,
                                        Currency = (int?)notice.Output.Currency,
                                        Price = notice.Output.Price,
                                        CreateDate = DateTime.Now,
                                        ReviewerID = notice.Output.ReviewerID,
                                        TrackerID = notice.Output.TrackerID
                                    };
                                    //回填OutputID,给后面赋值使用
                                    notice.OutputID = output.ID;
                                }
                                #endregion

                                #region 生成出库通知
                                var newnotice = new Layers.Data.Sqls.PvWms.Notices_Temp()
                                {
                                    ID = Layers.Data.PKeySigner.Pick(PkeyType.Notices),
                                    Type = (int)CgNoticeType.Out,
                                    WareHouseID = notice.WareHouseID,
                                    WaybillID = waybillid,
                                    InputID = notice.InputID,
                                    OutputID = notice.OutputID,
                                    ProductID = notice.Product.ID,
                                    DateCode = notice.DateCode,
                                    Origin = notice.Origin,
                                    Quantity = notice.Quantity,
                                    Conditions = new NoticeCondition().Json(),
                                    Status = (int)NoticesStatus.Waiting,
                                    Source = (int)notice.Source,
                                    Target = (int)NoticesTarget.Default,
                                    Weight = notice.Weight,
                                    NetWeight = notice.NetWeight,
                                    Volume = notice.Volume,
                                    BoxCode = boxCode,
                                    ShelveID = notice.ShelveID,
                                    BoxingSpecs = notice.BoxingSpecs,
                                    CreateDate = DateTime.Now,
                                    StorageID = notice.StorageID,
                                    CustomsName = notice.CustomsName
                                };


                                hkListOutNoticesId.Add(newnotice.ID);

                                #endregion

                                #region 生成拣货数据
                                var picking = new Layers.Data.Sqls.PvWms.Pickings_Temp
                                {
                                    ID = Layers.Data.PKeySigner.Pick(PkeyType.Pickings),
                                    StorageID = notice.StorageID,
                                    NoticeID = newnotice.ID,
                                    OutputID = notice.OutputID,
                                    BoxCode = boxCode,
                                    Quantity = notice.Quantity,
                                    AdminID = Npc.Robot.Obtain(),
                                    CreateDate = DateTime.Now,
                                    Weight = notice.Weight,
                                    NetWeight = notice.NetWeight,
                                    Volume = notice.Volume,
                                };
                                #endregion

                                if (output != null)
                                {
                                    reponsitory.Insert(output);
                                }
                                reponsitory.Insert(newnotice);
                                reponsitory.Insert(picking);
                            }

                            tran.Commit();
                        }
                    }, delcare);
                    task1.Start();

                    #endregion

                    Task.WaitAll(task, task1);

                    //}

                    //#region 补全报关香港出库数据, 生成深圳库房的入库数据, 以及内单的深圳出库数据

                    //WaybillsTopView hkWaybill = null;
                    //WaybillsTopView szWaybill = null;
                    //using (var reponsitory = new PvWmsRepository())
                    //using (var tran = reponsitory.OpenTransaction())
                    //{
                    #region 获取基础数据
                    var hkWaybill = centerreponsitory.GetTable<Layers.Data.Sqls.PvCenter.WaybillsTopView>().Single(item => item.wbID == waybillid);
                    var szWaybill = centerreponsitory.GetTable<Layers.Data.Sqls.PvCenter.WaybillsTopView>().Single(item => item.wbID == szwaybillid);

                    //香港报关出库通知
                    //只处理本次的


                    var noticesView = reponsitory.GetTable<Layers.Data.Sqls.PvWms.Notices_Temp>().
                        Where(item => item.WaybillID == hkWaybill.wbID && hkListOutNoticesId.Contains(item.ID));

                    //没有处理出库的
                    var linq_storage = from storageID in reponsitory.GetTable<Layers.Data.Sqls.PvWms.Notices_Temp>().
                                        Where(item => item.WaybillID == hkWaybill.wbID).
                                        Select(item => item.StorageID).Distinct()
                                       join storage in reponsitory.GetTable<Layers.Data.Sqls.PvWms.Storages>() on storageID equals storage.ID
                                       join log_storage in reponsitory.GetTable<Layers.Data.Sqls.PvWms.Logs_Storage>()
                                       on new { StorageID = storage.ID, IsCurrent = true } equals new { StorageID = log_storage.StorageID, IsCurrent = log_storage.IsCurrent }
                                       //join sorting in reponsitory.GetTable<Layers.Data.Sqls.PvWms.Sortings>() on storage.SortingID equals sorting.ID
                                       select new
                                       {
                                           storage.ID,
                                           storage.Quantity,
                                           //sorting.BoxCode
                                           log_storage.BoxCode,
                                       };

                    var storages = linq_storage.ToArray();
                    #endregion

                    #region 深圳数据自动处理 

                    #region 报关内单的深圳出库运单
                    //内单客户(注：内单出库运单的个数按客户个数生成、不按订单个数生成)
                    List<CgWaybill> listWaybills = null;//运单列表
                    List<Yahv.Services.Models.WsClient> listClients = null;//内单客户列表
                    var clientIds = (from notice in noticesView
                                     join input in reponsitory.GetTable<Layers.Data.Sqls.PvWms.Inputs>() on notice.InputID equals input.ID
                                     where notice.Source == (int)CgNoticeSource.AgentBreakCustomsForIns
                                     select input.ClientID).Distinct().ToArray();
                    if (clientIds.Count() > 0)
                    {
                        listWaybills = new List<CgWaybill>();
                        using (var pvbcrmReponsitory = new PvbCrmReponsitory())
                        {
                            //交货人（公司）
                            var ConsignorID = szWaybill.coeID;
                            //收货人（客户）
                            listClients = new Yahv.Services.Views.WsClientsTopView<PvbCrmReponsitory>(pvbcrmReponsitory)
                                .Where(item => clientIds.Contains(item.ID)).ToList();
                            var Consignees = new Yahv.Services.Views.WsConsigneesTopView<PvbCrmReponsitory>(pvbcrmReponsitory);

                            Action<CgWaybillsTopView> gainCgWaybill = new Action<CgWaybillsTopView>((waybill_) =>
                            {
                                CgWaybill szoutwaybill = new CgWaybill();
                                szoutwaybill.ID = waybill_.wbID;
                                szoutwaybill.Type = WaybillType.DeliveryToWarehouse;
                                szoutwaybill.Consignor = new WayParter()
                                {
                                    ID = waybill_.wbConsignorID,
                                };
                                szoutwaybill.Consignee = new Yahv.Services.Models.WayParter()
                                {
                                    ID = waybill_.wbConsigneeID,
                                };
                                szoutwaybill.FreightPayer = WaybillPayer.Consignee;
                                szoutwaybill.EnterCode = waybill_.wbEnterCode;
                                szoutwaybill.CreatorID = ConfigurationManager.AppSettings["OutAdminID"];
                                szoutwaybill.IsClearance = false;
                                szoutwaybill.WayCharge = null;
                                szoutwaybill.AppointTime = DateTime.Now.AddDays(1);
                                szoutwaybill.WayLoading = new Yahv.Services.Models.WayLoading()
                                {
                                    TakingDate = null,
                                    TakingAddress = null,
                                    TakingContact = null,
                                    TakingPhone = ConfigurationManager.AppSettings["AutoPhone"],
                                    CarNumber1 = ConfigurationManager.AppSettings["AutoVehicle"],
                                    Driver = ConfigurationManager.AppSettings["AutoDriver"],
                                    Carload = null,
                                    CreateDate = DateTime.Now,
                                    CreatorID = ConfigurationManager.AppSettings["OutAdminID"],
                                };
                                szoutwaybill.WayChcd = new WayChcd()
                                {
                                    LotNumber = cgWaybill.WayChcd.LotNumber,
                                    CarNumber1 = null,
                                    CarNumber2 = null,
                                    Carload = null,
                                    Driver = null,
                                    IsOnevehicle = null,
                                    Phone = null,
                                    PlanDate = null,
                                    TotalQuantity = null,
                                    DepartDate = null,
                                };
                                szoutwaybill.OrderID = null;//一般有多个订单，所以这里可为空
                                szoutwaybill.Status = CgWaybillStatus.Normal;
                                szoutwaybill.Source = CgNoticeSource.AgentBreakCustomsForIns;
                                szoutwaybill.NoticeType = CgNoticeType.Out;
                                szoutwaybill.ExcuteStatus = (int)CgPickingExcuteStatus.Picking;
                                szoutwaybill.CuttingOrderStatus = null;
                                szoutwaybill.Enter(centerreponsitory);
                                listWaybills.Add(szoutwaybill);
                            });

                            foreach (var clientId in clientIds)
                            {
                                var client = listClients.Single(item => item.ID == clientId);

                                var waybillExsit = reponsitory.GetTable<Layers.Data.Sqls.PvWms.CgWaybillsTopView>().
                                       Where(item => item.chcdLotNumber == cgWaybill.WayChcd.LotNumber &&
                                   item.NoticeType == (int)CgNoticeType.Out && item.WareHouseID.StartsWith("SZ")).
                                     SingleOrDefault(item => item.wbEnterCode == client.EnterCode);

                                if (waybillExsit != null)
                                {
                                    gainCgWaybill(waybillExsit);
                                    continue;
                                }

                                var consigneeView = Consignees.Where(item => item.EnterpriseID == clientId).ToArray();

                                var consignee = consigneeView.FirstOrDefault(item => item.IsDefault) ?? consigneeView.FirstOrDefault();//取收货信息的第一个默认值

                                if (client == null || consignee == null)
                                {
                                    throw new Exception("内单客户" + clientId + "不存在或其深圳收货信息为空!");
                                }

                                CgWaybill waybill_ = new CgWaybill();
                                waybill_.Type = WaybillType.DeliveryToWarehouse;
                                waybill_.Consignor = new Yahv.Services.Models.WayParter()
                                {
                                    ID = ConsignorID,
                                };
                                waybill_.Consignee = new Yahv.Services.Models.WayParter()
                                {
                                    Company = client.Name,
                                    Place = Origin.CHN.GetOrigin().Code,
                                    Address = consignee.Address,
                                    Contact = consignee.Name,
                                    Phone = consignee.Mobile + ";" + consignee.Tel,
                                    Zipcode = consignee.Postzip,
                                    Email = consignee.Email,
                                };
                                waybill_.FreightPayer = WaybillPayer.Consignee;
                                waybill_.EnterCode = client.EnterCode;
                                waybill_.CreatorID = ConfigurationManager.AppSettings["OutAdminID"];
                                waybill_.IsClearance = false;
                                waybill_.WayCharge = null;
                                waybill_.AppointTime = DateTime.Now.AddDays(1);
                                waybill_.WayLoading = new Yahv.Services.Models.WayLoading()
                                {
                                    TakingDate = null,
                                    TakingAddress = null,
                                    TakingContact = null,
                                    TakingPhone = ConfigurationManager.AppSettings["AutoPhone"],
                                    CarNumber1 = ConfigurationManager.AppSettings["AutoVehicle"],
                                    Driver = ConfigurationManager.AppSettings["AutoDriver"],
                                    Carload = null,
                                    CreateDate = DateTime.Now,
                                    CreatorID = ConfigurationManager.AppSettings["OutAdminID"],
                                };
                                waybill_.WayChcd = new WayChcd()
                                {
                                    LotNumber = cgWaybill.WayChcd.LotNumber,
                                    CarNumber1 = null,
                                    CarNumber2 = null,
                                    Carload = null,
                                    Driver = null,
                                    IsOnevehicle = null,
                                    Phone = null,
                                    PlanDate = null,
                                    TotalQuantity = null,
                                    DepartDate = null,
                                };
                                waybill_.OrderID = null;//一般有多个订单，所以这里可为空
                                waybill_.Status = CgWaybillStatus.Normal;
                                waybill_.Source = CgNoticeSource.AgentBreakCustomsForIns;
                                waybill_.NoticeType = CgNoticeType.Out;
                                waybill_.ExcuteStatus = (int)CgPickingExcuteStatus.Picking;
                                waybill_.CuttingOrderStatus = null;
                                waybill_.Enter(centerreponsitory);
                                //添加到列表
                                listWaybills.Add(waybill_);
                            }
                        }
                    }
                    #endregion

                    //var noticeSource = (CgNoticeSource)hkWaybill.Source;

                    foreach (var notice in noticesView)
                    {
                        var boxCode = storages.First(item => item.ID == notice.StorageID).BoxCode;

                        #region 生成深圳入库数据                        
                        var hkOuput_tempView = reponsitory.GetTable<Layers.Data.Sqls.PvWms.Outputs_Temp>()
                            .Where(item => item.ID == notice.OutputID).FirstOrDefault();

                        var hkOuputView = reponsitory.GetTable<Layers.Data.Sqls.PvWms.Outputs>()
                            .Where(item => item.ID == notice.OutputID).FirstOrDefault();

                        var hkOutput = hkOuput_tempView != null ? new
                        {
                            hkOuput_tempView.ID,
                            hkOuput_tempView.OrderID,
                            hkOuput_tempView.TinyOrderID,
                            hkOuput_tempView.ItemID,
                            hkOuput_tempView.TrackerID,
                            hkOuput_tempView.Currency,
                            hkOuput_tempView.Price,
                            hkOuput_tempView.InputID
                        } : new
                        {
                            hkOuputView.ID,
                            hkOuputView.OrderID,
                            hkOuputView.TinyOrderID,
                            hkOuputView.ItemID,
                            hkOuputView.TrackerID,
                            hkOuputView.Currency,
                            hkOuputView.Price,
                            hkOuputView.InputID
                        };

                        var hkInput = reponsitory.GetTable<Layers.Data.Sqls.PvWms.Inputs>()
                           .Where(item => item.ID == hkOutput.InputID).FirstOrDefault();

                        //深圳进项
                        Layers.Data.Sqls.PvWms.Inputs_Temp szInput;
                        var szInputID = Layers.Data.PKeySigner.Pick(PkeyType.Inputs);
                        if (notice.Source == (int)CgNoticeSource.AgentCustomsFromStorage)
                        {
                            szInput = new Layers.Data.Sqls.PvWms.Inputs_Temp()
                            {
                                ID = szInputID,
                                Code = szInputID,
                                OrderID = hkOutput.OrderID,
                                TinyOrderID = hkOutput.TinyOrderID,
                                ItemID = hkOutput.ItemID,
                                ProductID = notice.ProductID,
                                ClientID = hkInput.ClientID,
                                TrackerID = hkOutput.TrackerID,
                                Currency = hkOutput.Currency,
                                UnitPrice = hkOutput.Price,//由荣检开票后提供准确深圳入库单价（报关价格+关税）
                                CreateDate = DateTime.Now,
                                PayeeID = hkInput.PayeeID,
                            };
                        }
                        else
                        {
                            szInput = new Layers.Data.Sqls.PvWms.Inputs_Temp()
                            {
                                ID = szInputID,
                                Code = szInputID,
                                OrderID = hkInput.OrderID,
                                TinyOrderID = hkInput.TinyOrderID,
                                ItemID = hkInput.ItemID,
                                ProductID = notice.ProductID,
                                ClientID = hkInput.ClientID,
                                TrackerID = hkInput.TrackerID,
                                Currency = hkOutput.Currency,
                                UnitPrice = hkOutput.Price,//由荣检开票后提供准确深圳入库单价（报关价格+关税）
                                CreateDate = DateTime.Now,
                            };
                        }
                        reponsitory.Insert(szInput);



                        //深圳入库通知
                        var szNotice = new Layers.Data.Sqls.PvWms.Notices_Temp()
                        {
                            ID = Layers.Data.PKeySigner.Pick(PkeyType.Notices),
                            Type = (int)CgNoticeType.Enter,
                            //WareHouseID = ConfigurationManager.AppSettings["SzWareHouseID"],//深圳库房ID 
                            WareHouseID = WhSettings.SZ[szWaybill.coeCompany].ID,
                            WaybillID = szWaybill.wbID,
                            InputID = szInput.ID,
                            ProductID = notice.ProductID,
                            DateCode = notice.DateCode,
                            Origin = notice.Origin,
                            Quantity = notice.Quantity,
                            Weight = notice.Weight,
                            NetWeight = notice.NetWeight,
                            Volume = notice.Volume,
                            BoxCode = boxCode,
                            BoxingSpecs = notice.BoxingSpecs,
                            Source = notice.Source,
                            Target = (int)NoticesTarget.Default,
                            Conditions = new NoticeCondition().Json(),
                            CreateDate = DateTime.Now,
                            Status = (int)NoticesStatus.Waiting,
                            Summary = notice.Summary,
                            CustomsName = notice.CustomsName,

                        };
                        reponsitory.Insert(szNotice);

                        //深圳分拣数据
                        var szSorting = new Layers.Data.Sqls.PvWms.Sortings_Temp()
                        {
                            ID = Layers.Data.PKeySigner.Pick(PkeyType.Sortings),
                            NoticeID = szNotice.ID,
                            InputID = szInput.ID,
                            WaybillID = szWaybill.wbID,
                            BoxCode = szNotice.BoxCode,
                            Quantity = szNotice.Quantity,
                            Weight = szNotice.Weight,
                            NetWeight = szNotice.NetWeight,
                            Volume = szNotice.Volume,
                            AdminID = Npc.Robot.Obtain(),
                            CreateDate = DateTime.Now,
                        };
                        reponsitory.Insert(szSorting);

                        //深圳库存数据
                        var szStorage = new Layers.Data.Sqls.PvWms.Storages_Temp()
                        {
                            ID = Layers.Data.PKeySigner.Pick(PkeyType.Storages),
                            Type = (int)CgStoragesType.Flows, //当前库存类型删除报关库，
                            WareHouseID = WhSettings.SZ[szWaybill.coeCompany].ID,
                            SortingID = szSorting.ID,
                            InputID = szInput.ID,
                            ProductID = szInput.ProductID,
                            Total = szSorting.Quantity,
                            Quantity = szSorting.Quantity,
                            DateCode = szNotice.DateCode,
                            Origin = szNotice.Origin,
                            IsLock = false,
                            Supplier = szNotice.Supplier,
                            CreateDate = DateTime.Now,
                            Status = (int)GeneralStatus.Normal,
                        };
                        reponsitory.Insert(szStorage);
                        #endregion

                        #region 生成深圳出库数据
                        if (notice.Source == (int)NoticeSource.AgentBreakCustomsForIns)
                        {
                            var client = listClients.Single(item => item.ID == szInput.ClientID);
                            var szck_waybill = listWaybills.Single(item => item.EnterCode == client.EnterCode);
                            //深圳销项（产品的价值需要计算处理）
                            var szOutput = new Layers.Data.Sqls.PvWms.Outputs_Temp()
                            {
                                ID = Layers.Data.PKeySigner.Pick(PkeyType.Outputs),
                                InputID = szInput.ID,
                                OrderID = szInput.OrderID,
                                TinyOrderID = szInput.TinyOrderID,
                                ItemID = szInput.ItemID,
                                OwnerID = szInput.ClientID,
                                PurchaserID = szInput.PurchaserID,
                                Currency = szInput.Currency,
                                Price = szInput.UnitPrice,//由荣检开票后提供准确深圳出库单价（入库价格+代理费等）
                                CreateDate = DateTime.Now,
                                TrackerID = szInput.TrackerID,
                            };
                            reponsitory.Insert(szOutput);
                            //深圳出库通知
                            var szNoticeOut = new Layers.Data.Sqls.PvWms.Notices_Temp()
                            {
                                ID = Layers.Data.PKeySigner.Pick(PkeyType.Notices),
                                Type = (int)CgNoticeType.Out,
                                WareHouseID = WhSettings.SZ[szWaybill.coeCompany].ID,
                                WaybillID = szck_waybill.ID,
                                InputID = szInput.ID,
                                OutputID = szOutput.ID,
                                ProductID = szNotice.ProductID,
                                Supplier = szNotice.Supplier,
                                DateCode = szNotice.DateCode,
                                Origin = szNotice.Origin,
                                Quantity = szNotice.Quantity,
                                Weight = szNotice.Weight,
                                NetWeight = szNotice.NetWeight,
                                Volume = szNotice.Volume,
                                BoxCode = szNotice.BoxCode,
                                BoxingSpecs = szNotice.BoxingSpecs,
                                Source = szNotice.Source,
                                Target = (int)NoticesTarget.Default,
                                Conditions = new NoticeCondition().Json(),
                                CreateDate = DateTime.Now,
                                Status = (int)NoticesStatus.Waiting,
                                Summary = szNotice.Summary,
                                ShelveID = szStorage.ShelveID,
                                StorageID = szStorage.ID,
                                CustomsName = notice.CustomsName
                            };

                            reponsitory.Insert(szNoticeOut);
                            //深圳拣货
                            var szPicking = new Layers.Data.Sqls.PvWms.Pickings_Temp()
                            {
                                ID = Layers.Data.PKeySigner.Pick(PkeyType.Pickings),
                                StorageID = szNoticeOut.StorageID,
                                NoticeID = szNoticeOut.ID,
                                OutputID = szNoticeOut.OutputID,
                                BoxCode = szNoticeOut.BoxCode,
                                Quantity = szNoticeOut.Quantity,
                                Weight = szNoticeOut.Weight,
                                NetWeight = szNoticeOut.NetWeight,
                                Volume = szNoticeOut.Volume,
                                AdminID = Npc.Robot.Obtain(),
                                CreateDate = DateTime.Now,
                            };
                            reponsitory.Insert(szPicking);
                        }
                        #endregion
                    }
                    #endregion

                }
            }
            catch (Exception ex)
            {
                using (var reponsitory = new PvWmsRepository())
                {
                    // 异常发生时确保所有临时表清空, 不会有残余数据,避免下次数据的混杂错误
                    reponsitory.Command(sqlTrunacateBulk.ToString());
                }
                throw ex;
            }
            #endregion

            #region 更新执行SQL语句
            // 把临时表中的数据插入到原表中
            StringBuilder sqlInsertBulk = new StringBuilder();

            try
            {
                // sqlUpdateOrderStatus.AppendLine($@"INSERT INTO [{nameof(Layers.Data.Sqls.PvCenter.Logs_PvWsOrder)}] VALUES('{Guid.NewGuid()}','{id}',1,600,'{DateTime.Now}','{Npc.Robot.Obtain()}',1)");
                using (var reponsitory = new PvWmsRepository())
                using (var tran = reponsitory.OpenTransaction())
                {
                    // For Inputs_Temp
                    sqlInsertBulk.AppendLine($@"INSERT INTO [{nameof(Layers.Data.Sqls.PvWms.Inputs)}] ([ID],[Code],[OriginID],[OrderID],[TinyOrderID],[ItemID],[ProductID],[ClientID],[PayeeID],[ThirdID],[TrackerID],[SalerID],[PurchaserID],[Currency],[UnitPrice],[CreateDate])
Select [ID],[Code],[OriginID],[OrderID],[TinyOrderID],[ItemID],[ProductID],[ClientID],[PayeeID],[ThirdID],[TrackerID],[SalerID],[PurchaserID],[Currency],[UnitPrice],[CreateDate] From [{nameof(Layers.Data.Sqls.PvWms.Inputs_Temp)}]");

                    // For Outputs_Temp
                    sqlInsertBulk.AppendLine($@"INSERT INTO [{nameof(Layers.Data.Sqls.PvWms.Outputs)}] ([ID],[InputID],[OrderID],[TinyOrderID],[ItemID],[OwnerID],[SalerID],[PurchaserID],[Currency],[Price],[ReviewerID],[TrackerID],[CreateDate],[CustomerServiceID])
Select [ID],[InputID],[OrderID],[TinyOrderID],[ItemID],[OwnerID],[SalerID],[PurchaserID],[Currency],[Price],[ReviewerID],[TrackerID],[CreateDate],[CustomerServiceID] From [{nameof(Layers.Data.Sqls.PvWms.Outputs_Temp)}]");

                    // For Notices_Temp
                    sqlInsertBulk.AppendLine($@"INSERT INTO [{nameof(Layers.Data.Sqls.PvWms.Notices)}] ([ID],[Type],[WareHouseID],[WaybillID],[InputID],[OutputID],[ProductID],[Quantity],[DateCode],[Origin],[Weight],[NetWeight],[Volume],[Source],[Target],[BoxCode],[BoxingSpecs],[ShelveID],[Conditions],[Supplier],[Summary],[StorageID],[Status],[CreateDate],[CustomsName])
Select [ID],[Type],[WareHouseID],[WaybillID],[InputID],[OutputID],[ProductID],[Quantity],[DateCode],[Origin],[Weight],[NetWeight],[Volume],[Source],[Target],[BoxCode],[BoxingSpecs],[ShelveID],[Conditions],[Supplier],[Summary],[StorageID],[Status],[CreateDate],[CustomsName] From [{nameof(Layers.Data.Sqls.PvWms.Notices_Temp)}]");

                    // For Sortings_Temp
                    sqlInsertBulk.AppendLine($@"INSERT INTO [{nameof(Layers.Data.Sqls.PvWms.Sortings)}] ([ID],[NoticeID],[InputID],[WaybillID],[BoxCode],[Quantity],[AdminID],[CreateDate],[Weight],[NetWeight],[Volume],[Summary])
Select [ID],[NoticeID],[InputID],[WaybillID],[BoxCode],[Quantity],[AdminID],[CreateDate],[Weight],[NetWeight],[Volume],[Summary] From [{nameof(Layers.Data.Sqls.PvWms.Sortings_Temp)}]");

                    // For Storages_Temp
                    sqlInsertBulk.AppendLine($@"INSERT INTO [{nameof(Layers.Data.Sqls.PvWms.Storages)}] ([ID],[Type],[WareHouseID],[SortingID],[InputID],[ProductID],[Total],[Quantity],[Origin],[IsLock],[CreateDate],[Status],[ShelveID],[Supplier],[DateCode],[Summary],[CustomsName])
Select [ID],[Type],[WareHouseID],[SortingID],[InputID],[ProductID],[Total],[Quantity],[Origin],[IsLock],[CreateDate],[Status],[ShelveID],[Supplier],[DateCode],[Summary],[CustomsName] From [{nameof(Layers.Data.Sqls.PvWms.Storages_Temp)}]");

                    // For Pickings_Temp
                    sqlInsertBulk.AppendLine($@"INSERT INTO [{nameof(Layers.Data.Sqls.PvWms.Pickings)}] ([ID],[StorageID],[NoticeID],[OutputID],[BoxCode],[Quantity],[AdminID],[CreateDate],[Weight],[NetWeight],[Volume],[Summary])
Select [ID],[StorageID],[NoticeID],[OutputID],[BoxCode],[Quantity],[AdminID],[CreateDate],[Weight],[NetWeight],[Volume],[Summary] From [{nameof(Layers.Data.Sqls.PvWms.Pickings_Temp)}]");

                    // 临时表中的数据写入到原表中
                    reponsitory.Command(sqlInsertBulk.ToString());
                    tran.Commit();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                using (var reponsitory = new PvWmsRepository())
                {
                    // 再次清除所有临时表,确保临时表清空, 不会有残余数据
                    reponsitory.Command(sqlTrunacateBulk.ToString());
                }
            }
            #endregion

            //依照马莲要求：香港出库会变 待发货 状态
            #region 更新订单状态Logs_PvWsOrder
            using (var pvcenterReponsitory = new PvCenterReponsitory())
            {
                foreach (var orderID in delcare.Notices.Select(notice => notice.Output.OrderID)?.Distinct())
                {
                    pvcenterReponsitory.Update<Layers.Data.Sqls.PvCenter.Logs_PvWsOrder>(new
                    {
                        IsCurrent = false,
                    }, item => item.Type == (int)OrderStatusType.MainStatus && item.MainID == orderID);

                    //delcare.Notices.Select(.Output.TrackerID)

                    //delcare.Notices.First().BoxCode 可从装箱人这里取出

                    pvcenterReponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_PvWsOrder
                    {
                        ID = Guid.NewGuid().ToString(),
                        MainID = orderID,
                        Type = (int)OrderStatusType.MainStatus, //订单支付状态，  
                        Status = (int)CgOrderStatus.待收货,
                        CreateDate = DateTime.Now,
                        CreatorID = Npc.Robot.Obtain(),
                        IsCurrent = true,
                    });
                }

            }
            #endregion
        }
        
        #endregion

        /// <summary>
        /// 香港库房报关重构
        /// 香港报关成功后，直接调用,传过来报关内单，外单的Json
        /// 生成深圳库房入库通知， 并把对应的数据补全，
        /// 针对报关内单生成深圳库房的出库通知， 报关外单则不生成深圳出库通知.
        /// 过来的Json说明，均已报关完，并且截单完成，所以深圳的出库通知直接显示
        /// </summary>
        public void AutoHkExitNoticeForTaskNew(CgDelcare delcare)
        {
            //首先要解决出库的waybill与WayChcd的来源，这样：库房ID、交货人、收货人（公司名）、司机信息，就都解决了
            //基于运输批次号生成实际的出库通知，一个运输批次一定会生成多个订单的通知，因此，Waybill.OrderID可以为空
            //基于以上讨论，具体参数要根据情况做
            //var waybillid = string.Empty;  //香港出库WaybillID
            var szwaybillid = string.Empty;//深圳入库WaybillID
            var cgWaybill = delcare.HkExitWaybill;
            //var cgNotices = delcare.Notices;
            var hkListOutNoticesId = new List<string>();
            StringBuilder sqlTrunacateBulk = new StringBuilder();
            sqlTrunacateBulk.AppendLine($@"Delete from [{nameof(Layers.Data.Sqls.PvWms.Pickings_Temp)}]");
            sqlTrunacateBulk.AppendLine($@"Delete from [{nameof(Layers.Data.Sqls.PvWms.Storages_Temp)}]");
            sqlTrunacateBulk.AppendLine($@"Delete from [{nameof(Layers.Data.Sqls.PvWms.Sortings_Temp)}]");
            sqlTrunacateBulk.AppendLine($@"Delete from [{nameof(Layers.Data.Sqls.PvWms.Outputs_Temp)}]");
            sqlTrunacateBulk.AppendLine($@"Delete from [{nameof(Layers.Data.Sqls.PvWms.Notices_Temp)}]");
            sqlTrunacateBulk.AppendLine($@"Delete from [{nameof(Layers.Data.Sqls.PvWms.Inputs_Temp)}]");
            var carrierTopView = new CarriersTopView();

            #region 香港库房出库，深圳库房入库，深圳内单出库
            try
            {
                using (var reponsitory = new PvWmsRepository())
                using (var centerreponsitory = new PvCenterReponsitory())
                {
                    #region 生成运单数据
                    //判断改运输批次号是否已经存在运单数据不更新（放在下面的截单接口(DelcareCutting)更新）
                    var waybill = reponsitory.GetTable<Layers.Data.Sqls.PvWms.CgWaybillsTopView>()
                        .Where(item => item.chcdLotNumber == cgWaybill.WayChcd.LotNumber && item.NoticeType == (int)CgNoticeType.Enter && item.WareHouseID.StartsWith("SZ")).FirstOrDefault();

                    #region 修正业务来源
                    //修正业务来源
                    //var originSource = centerreponsitory.GetTable<Layers.Data.Sqls.PvCenter.WaybillsTopView>().
                    //       Where(item => item.OrderID == cgWaybill.OrderID).
                    //       Select(item => item.Source).
                    //       FirstOrDefault();

                    if (cgWaybill.Source == CgNoticeSource.AgentBreakCustoms
                        || cgWaybill.Source == CgNoticeSource.AgentCustomsFromStorage)
                    {
                        cgWaybill.Source = CgNoticeSource.AgentBreakCustoms;
                        delcare.Notices.ForEach(item =>
                        {
                            item.Source = (NoticeSource)cgWaybill.Source;
                        });
                    }
                    #endregion

                    #region 生成深圳入库运单
                    if (waybill == null)
                    {
                        var carrierID = carrierTopView.FirstOrDefault(item => item.Code == cgWaybill.CarrierID)?.ID;                     
                        //深圳入库运单
                        CgWaybill SzWaybill = new CgWaybill();
                        SzWaybill.Type = WaybillType.DeliveryToWarehouse;
                        SzWaybill.CarrierID = carrierID;
                        SzWaybill.Consignee = cgWaybill.Consignee;
                        SzWaybill.Consignor = cgWaybill.Consignor;
                        SzWaybill.FreightPayer = cgWaybill.FreightPayer;
                        SzWaybill.EnterCode = cgWaybill.EnterCode;
                        SzWaybill.CreatorID = Npc.Robot.Obtain();
                        SzWaybill.IsClearance = false;
                        SzWaybill.WayCharge = null;
                        SzWaybill.WayLoading = null;
                        SzWaybill.WayChcd = cgWaybill.WayChcd;
                        //SzWaybill.OrderID = cgWaybill.OrderID;
                        //商定后更新为null
                        SzWaybill.OrderID = null;
                        SzWaybill.AppointTime = cgWaybill.AppointTime;
                        SzWaybill.Status = CgWaybillStatus.Normal;
                        SzWaybill.Source = cgWaybill.Source;
                        SzWaybill.NoticeType = CgNoticeType.Enter;
                        SzWaybill.ExcuteStatus = (int)CgSortingExcuteStatus.Sorting;
                        SzWaybill.CuttingOrderStatus = (int)Enums.CutStatus.UnCutting;  // UnCutting --->Cutting

                        SzWaybill.TotalParts = cgWaybill.TotalParts;
                        SzWaybill.TotalWeight = cgWaybill.TotalWeight;
                        SzWaybill.TotalVolume = cgWaybill.TotalVolume;

                        SzWaybill.Enter(centerreponsitory);
                        szwaybillid = SzWaybill.ID;
                    }
                    else
                    {
                        szwaybillid = reponsitory.GetTable<Layers.Data.Sqls.PvWms.CgWaybillsTopView>()
                        .Where(item => item.chcdLotNumber == cgWaybill.WayChcd.LotNumber && item.NoticeType == (int)CgNoticeType.Enter && item.WareHouseID.StartsWith("SZ")).FirstOrDefault().wbID;
                    }
                    #endregion

                    #endregion

                    #region 保存产品数据

                    Task task = new Task((sender) =>
                    {
                        var _delcare = sender as CgDelcare;

                        foreach (var notice in _delcare.Notices)
                        {
                            Yahv.Services.Views.ProductsTopView<PvWsOrderReponsitory>.Enter(notice.Product);
                        }
                    }, delcare);
                    task.Start();

                    #endregion

                    //Task.WaitAll(task);
                    
                    #region 获取基础数据                    
                    var szWaybill = centerreponsitory.GetTable<Layers.Data.Sqls.PvCenter.WaybillsTopView>().Single(item => item.wbID == szwaybillid);

                    //var orders = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.OrdersTopView>()
                    //    .Where(item => item.ID == cgWaybill.OrderID && cgWaybill.Source == CgNoticeSource.AgentBreakCustomsForIns);

                    var orders = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.OrdersTopView>()
                       .Where(item => item.ID == cgWaybill.OrderID);
                    var orderItems = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.OrderItemsTopView>()
                        .Where(item => item.OrderID == cgWaybill.OrderID)?.ToList();

                    var order = orders.FirstOrDefault();
                    #endregion
                    
                    #region 深圳数据自动处理 

                    #region 报关内单的深圳出库运单
                    //内单客户(注：内单出库运单的个数按客户个数生成、不按订单个数生成)
                    List<CgWaybill> listWaybills = null;//运单列表
                    List<Yahv.Services.Models.WsClient> listClients = null;//内单客户列表
                    if (cgWaybill.Source == CgNoticeSource.AgentBreakCustomsForIns)
                    {
                        var clientIds = orders.Select(item => item.ClientID).Distinct().ToArray();
                        if (clientIds.Count() > 0)
                        {
                            listWaybills = new List<CgWaybill>();
                            using (var pvbcrmReponsitory = new PvbCrmReponsitory())
                            {
                                //交货人（公司）
                                var ConsignorID = szWaybill.coeID;
                                //收货人（客户）
                                listClients = new Yahv.Services.Views.WsClientsTopView<PvbCrmReponsitory>(pvbcrmReponsitory)
                                    .Where(item => clientIds.Contains(item.ID)).ToList();
                                var Consignees = new Yahv.Services.Views.WsConsigneesTopView<PvbCrmReponsitory>(pvbcrmReponsitory);

                                Action<CgWaybillsTopView> gainCgWaybill = new Action<CgWaybillsTopView>((waybill_) =>
                                {
                                    CgWaybill szoutwaybill = new CgWaybill();
                                    szoutwaybill.ID = waybill_.wbID;
                                    szoutwaybill.Type = WaybillType.DeliveryToWarehouse;
                                    szoutwaybill.Consignor = new WayParter()
                                    {
                                        ID = waybill_.wbConsignorID,
                                    };
                                    szoutwaybill.Consignee = new Yahv.Services.Models.WayParter()
                                    {
                                        ID = waybill_.wbConsigneeID,
                                    };
                                    szoutwaybill.FreightPayer = WaybillPayer.Consignee;
                                    szoutwaybill.EnterCode = waybill_.wbEnterCode;
                                    szoutwaybill.CreatorID = ConfigurationManager.AppSettings["OutAdminID"];
                                    szoutwaybill.IsClearance = false;
                                    szoutwaybill.WayCharge = null;
                                    szoutwaybill.AppointTime = DateTime.Now.AddDays(1);
                                    szoutwaybill.WayLoading = new Yahv.Services.Models.WayLoading()
                                    {
                                        TakingDate = null,
                                        TakingAddress = null,
                                        TakingContact = null,
                                        TakingPhone = ConfigurationManager.AppSettings["AutoPhone"],
                                        CarNumber1 = ConfigurationManager.AppSettings["AutoVehicle"],
                                        Driver = ConfigurationManager.AppSettings["AutoDriver"],
                                        Carload = null,
                                        CreateDate = DateTime.Now,
                                        CreatorID = ConfigurationManager.AppSettings["OutAdminID"],
                                    };
                                    szoutwaybill.WayChcd = new WayChcd()
                                    {
                                        LotNumber = cgWaybill.WayChcd.LotNumber,
                                        CarNumber1 = null,
                                        CarNumber2 = null,
                                        Carload = null,
                                        Driver = null,
                                        IsOnevehicle = null,
                                        Phone = null,
                                        PlanDate = null,
                                        TotalQuantity = null,
                                        DepartDate = null,
                                    };
                                    szoutwaybill.OrderID = waybill_.OrderID;//一般有多个订单，所以这里可为空
                                    szoutwaybill.Status = CgWaybillStatus.Normal;
                                    szoutwaybill.Source = CgNoticeSource.AgentBreakCustomsForIns;
                                    szoutwaybill.NoticeType = CgNoticeType.Out;
                                    szoutwaybill.ExcuteStatus = (int)CgPickingExcuteStatus.Picking;
                                    szoutwaybill.CuttingOrderStatus = waybill_.CuttingOrderStatus;
                                    szoutwaybill.Enter(centerreponsitory);
                                    listWaybills.Add(szoutwaybill);
                                });

                                foreach (var clientId in clientIds)
                                {
                                    var client = listClients.Single(item => item.ID == clientId);

                                    var waybillExsit = reponsitory.GetTable<Layers.Data.Sqls.PvWms.CgWaybillsTopView>().
                                           Where(item => item.chcdLotNumber == cgWaybill.WayChcd.LotNumber &&
                                       item.NoticeType == (int)CgNoticeType.Out && item.WareHouseID.StartsWith("SZ")).
                                         SingleOrDefault(item => item.wbEnterCode == client.EnterCode);

                                    if (waybillExsit != null)
                                    {
                                        gainCgWaybill(waybillExsit);
                                        continue;
                                    }

                                    var consigneeView = Consignees.Where(item => item.EnterpriseID == clientId).ToArray();

                                    var consignee = consigneeView.FirstOrDefault(item => item.IsDefault) ?? consigneeView.FirstOrDefault();//取收货信息的第一个默认值

                                    if (client == null || consignee == null)
                                    {
                                        throw new Exception("内单客户" + clientId + "不存在或其默认的代仓储类型的深圳收货信息为空!");
                                    }

                                    #region 创建运单 并把运单waybill_加入到listWaybills中
                                    CgWaybill waybill_ = new CgWaybill();
                                    waybill_.Type = WaybillType.DeliveryToWarehouse;
                                    waybill_.Consignor = new Yahv.Services.Models.WayParter()
                                    {
                                        ID = ConsignorID,
                                    };
                                    waybill_.Consignee = new Yahv.Services.Models.WayParter()
                                    {
                                        Company = client.Name,
                                        Place = Origin.CHN.GetOrigin().Code,
                                        Address = consignee.Address,
                                        Contact = consignee.Name,
                                        Phone = consignee.Mobile + ";" + consignee.Tel,
                                        Zipcode = consignee.Postzip,
                                        Email = consignee.Email,
                                    };
                                    waybill_.FreightPayer = WaybillPayer.Consignee;
                                    waybill_.EnterCode = client.EnterCode;
                                    waybill_.CreatorID = ConfigurationManager.AppSettings["OutAdminID"];
                                    waybill_.IsClearance = false;
                                    waybill_.WayCharge = null;
                                    waybill_.AppointTime = DateTime.Now.AddDays(1);
                                    waybill_.WayLoading = new Yahv.Services.Models.WayLoading()
                                    {
                                        TakingDate = null,
                                        TakingAddress = null,
                                        TakingContact = null,
                                        TakingPhone = ConfigurationManager.AppSettings["AutoPhone"],
                                        CarNumber1 = ConfigurationManager.AppSettings["AutoVehicle"],
                                        Driver = ConfigurationManager.AppSettings["AutoDriver"],
                                        Carload = null,
                                        CreateDate = DateTime.Now,
                                        CreatorID = ConfigurationManager.AppSettings["OutAdminID"],
                                    };
                                    waybill_.WayChcd = new WayChcd()
                                    {
                                        LotNumber = cgWaybill.WayChcd.LotNumber,
                                        CarNumber1 = null,
                                        CarNumber2 = null,
                                        Carload = null,
                                        Driver = null,
                                        IsOnevehicle = null,
                                        Phone = null,
                                        PlanDate = null,
                                        TotalQuantity = null,
                                        DepartDate = null,
                                    };
                                    waybill_.OrderID = cgWaybill.OrderID;//一般有多个订单，所以这里可为空
                                    waybill_.Status = CgWaybillStatus.Normal;
                                    waybill_.Source = CgNoticeSource.AgentBreakCustomsForIns;
                                    waybill_.NoticeType = CgNoticeType.Out;
                                    waybill_.ExcuteStatus = (int)CgPickingExcuteStatus.Picking;
                                    waybill_.CuttingOrderStatus = (int)CgCuttingOrderStatus.Waiting;
                                    waybill_.Enter(centerreponsitory);
                                    //添加到列表
                                    listWaybills.Add(waybill_);
                                    #endregion
                                }
                            }
                        }
                    }
                    #endregion

                    
                    foreach (var notice in delcare.Notices)
                    {
                        var boxCode = notice.BoxCode;

                        #region 生成深圳入库数据                        

                        //深圳进项
                        Layers.Data.Sqls.PvWms.Inputs_Temp szInput;
                        var szInputID = Layers.Data.PKeySigner.Pick(PkeyType.Inputs);
                        //var szInputID = notice.Output.InputID;
                        // 香港重构后不在区分转报关与不普通的内单，外单
                        szInput = new Layers.Data.Sqls.PvWms.Inputs_Temp()
                        {
                            ID = szInputID,
                            Code = szInputID,
                            OrderID = notice.Output.OrderID,
                            TinyOrderID = notice.Output.TinyOrderID,
                            ItemID = notice.Output.InputID,
                            ProductID = notice.Product.ID,
                            ClientID = order.ClientID,
                            TrackerID = notice.Output.TrackerID,
                            Currency = (int?)notice.Output.Currency,
                            UnitPrice = notice.Output.Price,//由荣检开票后提供准确深圳入库单价（报关价格+关税）
                            CreateDate = DateTime.Now,
                            PayeeID = order.PayeeID,
                        };                        
                        
                        reponsitory.Insert(szInput);

                        //深圳入库通知
                        var szNotice = new Layers.Data.Sqls.PvWms.Notices_Temp()
                        {
                            ID = Layers.Data.PKeySigner.Pick(PkeyType.Notices),
                            Type = (int)CgNoticeType.Enter,                            
                            WareHouseID = WhSettings.SZ[szWaybill.coeCompany].ID,
                            WaybillID = szWaybill.wbID,
                            InputID = szInput.ID,
                            ProductID = notice.Product.ID,
                            DateCode = notice.DateCode,
                            Origin = notice.Origin,
                            Quantity = notice.Quantity,
                            Weight = notice.Weight,
                            NetWeight = notice.NetWeight,
                            Volume = notice.Volume,
                            BoxCode = boxCode,
                            BoxingSpecs = notice.BoxingSpecs,
                            Source = (int)notice.Source,
                            Target = (int)NoticesTarget.Default,
                            Conditions = new NoticeCondition().Json(),
                            CreateDate = DateTime.Now,
                            Status = (int)NoticesStatus.Waiting,
                            Summary = null,
                            CustomsName = notice.CustomsName,
                        };
                        reponsitory.Insert(szNotice);

                        //深圳分拣数据
                        var szSorting = new Layers.Data.Sqls.PvWms.Sortings_Temp()
                        {
                            ID = Layers.Data.PKeySigner.Pick(PkeyType.Sortings),
                            NoticeID = szNotice.ID,
                            InputID = szInput.ID,
                            WaybillID = szWaybill.wbID,
                            BoxCode = szNotice.BoxCode,
                            Quantity = szNotice.Quantity,
                            Weight = szNotice.Weight,
                            NetWeight = szNotice.NetWeight,
                            Volume = szNotice.Volume,
                            AdminID = Npc.Robot.Obtain(),
                            CreateDate = DateTime.Now,
                        };
                        reponsitory.Insert(szSorting);

                        //深圳库存数据
                        var szStorage = new Layers.Data.Sqls.PvWms.Storages_Temp()
                        {
                            ID = Layers.Data.PKeySigner.Pick(PkeyType.Storages),
                            Type = (int)CgStoragesType.Flows, //当前库存类型删除报关库，
                            WareHouseID = WhSettings.SZ[szWaybill.coeCompany].ID,
                            SortingID = szSorting.ID,
                            InputID = szInput.ID,
                            ProductID = szInput.ProductID,
                            Total = szSorting.Quantity,
                            Quantity = szSorting.Quantity,
                            DateCode = szNotice.DateCode,
                            Origin = szNotice.Origin,
                            IsLock = false,
                            Supplier = szNotice.Supplier,
                            CreateDate = DateTime.Now,
                            Status = (int)GeneralStatus.Normal,
                        };
                        reponsitory.Insert(szStorage);
                        #endregion

                        #region 生成深圳出库数据
                        if (notice.Source == NoticeSource.AgentBreakCustomsForIns)
                        {
                            var client = listClients.Single(item => item.ID == szInput.ClientID);
                            var szck_waybill = listWaybills.Single(item => item.EnterCode == client.EnterCode);
                            bool szckOrderIDChange = false;
                            if (string.IsNullOrEmpty(szck_waybill.OrderID))
                            {
                                szck_waybill.OrderID = szInput.OrderID;
                                szckOrderIDChange = true;
                            }
                            else
                            {
                                var ckOrderIDs = szck_waybill.OrderID.Split(',');
                                if (!ckOrderIDs.Contains(szInput.OrderID))
                                {
                                    szck_waybill.OrderID += "," + szInput.OrderID;
                                    szckOrderIDChange = true;
                                }
                            }

                            // 如果深圳出库内单的运单中的OrderID改变，则更新深圳出库内单的内单
                            if (szckOrderIDChange == true)
                            {
                                centerreponsitory.Update<Layers.Data.Sqls.PvCenter.Waybills>(new
                                {
                                    OrderID = szck_waybill.OrderID,
                                }, item => item.ID == szck_waybill.ID);
                            }

                            //深圳销项（产品的价值需要计算处理）
                            var szOutput = new Layers.Data.Sqls.PvWms.Outputs_Temp()
                            {
                                ID = Layers.Data.PKeySigner.Pick(PkeyType.Outputs),
                                InputID = szInput.ID,
                                OrderID = szInput.OrderID,
                                TinyOrderID = szInput.TinyOrderID,
                                ItemID = szInput.ItemID,
                                OwnerID = szInput.ClientID,
                                PurchaserID = szInput.PurchaserID,
                                Currency = szInput.Currency,
                                Price = szInput.UnitPrice,//由荣检开票后提供准确深圳出库单价（入库价格+代理费等）
                                CreateDate = DateTime.Now,
                                TrackerID = szInput.TrackerID,
                            };
                            reponsitory.Insert(szOutput);
                            //深圳出库通知
                            var szNoticeOut = new Layers.Data.Sqls.PvWms.Notices_Temp()
                            {
                                ID = Layers.Data.PKeySigner.Pick(PkeyType.Notices),
                                Type = (int)CgNoticeType.Out,
                                WareHouseID = WhSettings.SZ[szWaybill.coeCompany].ID,
                                WaybillID = szck_waybill.ID,
                                InputID = szInput.ID,
                                OutputID = szOutput.ID,
                                ProductID = szNotice.ProductID,
                                Supplier = szNotice.Supplier,
                                DateCode = szNotice.DateCode,
                                Origin = szNotice.Origin,
                                Quantity = szNotice.Quantity,
                                Weight = szNotice.Weight,
                                NetWeight = szNotice.NetWeight,
                                Volume = szNotice.Volume,
                                BoxCode = szNotice.BoxCode,
                                BoxingSpecs = szNotice.BoxingSpecs,
                                Source = szNotice.Source,
                                Target = (int)NoticesTarget.Default,
                                Conditions = new NoticeCondition().Json(),
                                CreateDate = DateTime.Now,
                                Status = (int)NoticesStatus.Waiting,
                                Summary = szNotice.Summary,
                                ShelveID = szStorage.ShelveID,
                                StorageID = szStorage.ID,
                                CustomsName = notice.CustomsName
                            };

                            reponsitory.Insert(szNoticeOut);
                            //深圳拣货
                            var szPicking = new Layers.Data.Sqls.PvWms.Pickings_Temp()
                            {
                                ID = Layers.Data.PKeySigner.Pick(PkeyType.Pickings),
                                StorageID = szNoticeOut.StorageID,
                                NoticeID = szNoticeOut.ID,
                                OutputID = szNoticeOut.OutputID,
                                BoxCode = szNoticeOut.BoxCode,
                                Quantity = szNoticeOut.Quantity,
                                Weight = szNoticeOut.Weight,
                                NetWeight = szNoticeOut.NetWeight,
                                Volume = szNoticeOut.Volume,
                                AdminID = Npc.Robot.Obtain(),
                                CreateDate = DateTime.Now,
                            };
                            reponsitory.Insert(szPicking);
                        }
                        #endregion
                    }
                    #endregion

                }
            }
            catch (Exception ex)
            {
                using (var reponsitory = new PvWmsRepository())
                {
                    // 异常发生时确保所有临时表清空, 不会有残余数据,避免下次数据的混杂错误
                    reponsitory.Command(sqlTrunacateBulk.ToString());
                }
                throw ex;
            }
            #endregion

            #region 更新执行SQL语句
            // 把临时表中的数据插入到原表中
            StringBuilder sqlInsertBulk = new StringBuilder();

            try
            {
                // sqlUpdateOrderStatus.AppendLine($@"INSERT INTO [{nameof(Layers.Data.Sqls.PvCenter.Logs_PvWsOrder)}] VALUES('{Guid.NewGuid()}','{id}',1,600,'{DateTime.Now}','{Npc.Robot.Obtain()}',1)");
                using (var reponsitory = new PvWmsRepository())
                using (var tran = reponsitory.OpenTransaction())
                {
                    // For Inputs_Temp
                    sqlInsertBulk.AppendLine($@"INSERT INTO [{nameof(Layers.Data.Sqls.PvWms.Inputs)}] ([ID],[Code],[OriginID],[OrderID],[TinyOrderID],[ItemID],[ProductID],[ClientID],[PayeeID],[ThirdID],[TrackerID],[SalerID],[PurchaserID],[Currency],[UnitPrice],[CreateDate])
Select [ID],[Code],[OriginID],[OrderID],[TinyOrderID],[ItemID],[ProductID],[ClientID],[PayeeID],[ThirdID],[TrackerID],[SalerID],[PurchaserID],[Currency],[UnitPrice],[CreateDate] From [{nameof(Layers.Data.Sqls.PvWms.Inputs_Temp)}]");

                    // For Outputs_Temp
                    sqlInsertBulk.AppendLine($@"INSERT INTO [{nameof(Layers.Data.Sqls.PvWms.Outputs)}] ([ID],[InputID],[OrderID],[TinyOrderID],[ItemID],[OwnerID],[SalerID],[PurchaserID],[Currency],[Price],[ReviewerID],[TrackerID],[CreateDate],[CustomerServiceID])
Select [ID],[InputID],[OrderID],[TinyOrderID],[ItemID],[OwnerID],[SalerID],[PurchaserID],[Currency],[Price],[ReviewerID],[TrackerID],[CreateDate],[CustomerServiceID] From [{nameof(Layers.Data.Sqls.PvWms.Outputs_Temp)}]");

                    // For Notices_Temp
                    sqlInsertBulk.AppendLine($@"INSERT INTO [{nameof(Layers.Data.Sqls.PvWms.Notices)}] ([ID],[Type],[WareHouseID],[WaybillID],[InputID],[OutputID],[ProductID],[Quantity],[DateCode],[Origin],[Weight],[NetWeight],[Volume],[Source],[Target],[BoxCode],[BoxingSpecs],[ShelveID],[Conditions],[Supplier],[Summary],[StorageID],[Status],[CreateDate],[CustomsName])
Select [ID],[Type],[WareHouseID],[WaybillID],[InputID],[OutputID],[ProductID],[Quantity],[DateCode],[Origin],[Weight],[NetWeight],[Volume],[Source],[Target],[BoxCode],[BoxingSpecs],[ShelveID],[Conditions],[Supplier],[Summary],[StorageID],[Status],[CreateDate],[CustomsName] From [{nameof(Layers.Data.Sqls.PvWms.Notices_Temp)}]");

                    // For Sortings_Temp
                    sqlInsertBulk.AppendLine($@"INSERT INTO [{nameof(Layers.Data.Sqls.PvWms.Sortings)}] ([ID],[NoticeID],[InputID],[WaybillID],[BoxCode],[Quantity],[AdminID],[CreateDate],[Weight],[NetWeight],[Volume],[Summary])
Select [ID],[NoticeID],[InputID],[WaybillID],[BoxCode],[Quantity],[AdminID],[CreateDate],[Weight],[NetWeight],[Volume],[Summary] From [{nameof(Layers.Data.Sqls.PvWms.Sortings_Temp)}]");

                    // For Storages_Temp
                    sqlInsertBulk.AppendLine($@"INSERT INTO [{nameof(Layers.Data.Sqls.PvWms.Storages)}] ([ID],[Type],[WareHouseID],[SortingID],[InputID],[ProductID],[Total],[Quantity],[Origin],[IsLock],[CreateDate],[Status],[ShelveID],[Supplier],[DateCode],[Summary],[CustomsName])
Select [ID],[Type],[WareHouseID],[SortingID],[InputID],[ProductID],[Total],[Quantity],[Origin],[IsLock],[CreateDate],[Status],[ShelveID],[Supplier],[DateCode],[Summary],[CustomsName] From [{nameof(Layers.Data.Sqls.PvWms.Storages_Temp)}]");

                    // For Pickings_Temp
                    sqlInsertBulk.AppendLine($@"INSERT INTO [{nameof(Layers.Data.Sqls.PvWms.Pickings)}] ([ID],[StorageID],[NoticeID],[OutputID],[BoxCode],[Quantity],[AdminID],[CreateDate],[Weight],[NetWeight],[Volume],[Summary])
Select [ID],[StorageID],[NoticeID],[OutputID],[BoxCode],[Quantity],[AdminID],[CreateDate],[Weight],[NetWeight],[Volume],[Summary] From [{nameof(Layers.Data.Sqls.PvWms.Pickings_Temp)}]");

                    // 临时表中的数据写入到原表中
                    reponsitory.Command(sqlInsertBulk.ToString());
                    tran.Commit();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                using (var reponsitory = new PvWmsRepository())
                {
                    // 再次清除所有临时表,确保临时表清空, 不会有残余数据
                    reponsitory.Command(sqlTrunacateBulk.ToString());
                }
            }
            #endregion

            //依照马莲要求：香港出库会变 待发货 状态
            #region 更新订单状态Logs_PvWsOrder
            using (var pvcenterReponsitory = new PvCenterReponsitory())
            {
                foreach (var orderID in delcare.Notices.Select(notice => notice.Output.OrderID)?.Distinct())
                {
                    pvcenterReponsitory.Update<Layers.Data.Sqls.PvCenter.Logs_PvWsOrder>(new
                    {
                        IsCurrent = false,
                    }, item => item.Type == (int)OrderStatusType.MainStatus && item.MainID == orderID);
                                        
                    pvcenterReponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_PvWsOrder
                    {
                        ID = Guid.NewGuid().ToString(),
                        MainID = orderID,
                        Type = (int)OrderStatusType.MainStatus, //订单支付状态，  
                        Status = (int)CgOrderStatus.待收货,
                        CreateDate = DateTime.Now,
                        CreatorID = Npc.Robot.Obtain(),
                        IsCurrent = true,
                    });
                }

            }
            #endregion
        }

        /// <summary>
        /// 报关截单
        /// 香港库房可以出库标志
        /// </summary>
        public void DelcareCutting(CgDelcareCutting cutting)
        {
            using (var reponsitory = new PvCenterReponsitory())
            {
                var waybillIds = reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.WayChcd>()
                    .Where(item => item.LotNumber == cutting.ID).Select(item => item.ID).ToArray();
                //更新中港运单数据
                reponsitory.Update<Layers.Data.Sqls.PvCenter.WayChcd>(new
                {
                    CarNumber1 = cutting.HKLicense,
                    CarNumber2 = cutting.VehicleLicence,
                    Carload = String.IsNullOrEmpty(cutting.VehicleWeight) ? (int?)null : int.Parse(cutting.VehicleWeight),
                    Driver = cutting.DriverName,
                    Phone = cutting.DriverMobile,
                    PlanDate = cutting.TransportTime,
                    DepartDate = cutting.TransportTime,
                    DriverCode = cutting.DriverCode,
                    VehicleType = cutting.VehicleType,
                    HKPhone = cutting.DriverHKMobile,
                    DriverCardNo = cutting.DriverCardNo,
                    CustomsPort = cutting.CustomsPort,
                    VehicleSize = cutting.VehicleSize,
                }, item => item.LotNumber == cutting.ID);

                var carrier = reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.CarriersTopView>().
                    Where(item => item.Name.Contains(cutting.CarrierName)
                        || item.Code == cutting.CarrierCode
                        || item.Name.Contains(cutting.CarrierCode)).FirstOrDefault();

                //更新运单的截单状态、总件数、总重量、承运商
                reponsitory.Update<Layers.Data.Sqls.PvCenter.Waybills>(new
                {
                    CarrierID = carrier?.ID,
                    TotalParts = cutting.TotalPacks,
                    TotalWeight = cutting.TotalWeight,
                    CuttingOrderStatus = cutting.CutStatus,
                }, item => waybillIds.Contains(item.ID));
            }
        }

        /// <summary>
        /// 报关香港出库
        /// 生成深圳库房的入库数据
        /// </summary>
        /// <param name="lotNumber">运输批次号</param>
        public void AutoHkExit_Old(string lotNumber)
        {
            //  点击后，先完成如下香港出库的内容：
            //  目前报关直接使用的是流水库，storages一定要扣除库存！

            WaybillsTopView hkWaybill = null;
            WaybillsTopView szWaybill = null;

            using (var reponsitory = new PvWmsRepository())
            //using (var pvcenterReponsitory = new PvCenterReponsitory())
            using (var trans = reponsitory.OpenTransaction())
            {
                #region 获取基础数据

                //根据运输批次号，查找报关运单
                var waybills = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>()
                    .Where(item => item.chcdLotNumber == lotNumber).ToArray();
                //香港出库运单
                hkWaybill = waybills.Single(item => item.chcdLotNumber == lotNumber
                   && item.NoticeType == (int)CgNoticeType.Out);
                if (hkWaybill.CuttingOrderStatus == (int)Enums.CutStatus.UnCutting)
                {
                    throw new Exception("运单未截单,出库失败!!!");
                }
                //深圳入库运单
                szWaybill = waybills.Single(item => item.chcdLotNumber == lotNumber
                   && item.NoticeType == (int)CgNoticeType.Enter);


                //香港报关出库通知
                var notices = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>().
                Where(item => item.WaybillID == hkWaybill.wbID).ToArray();



                //没有处理出库的
                var linq_storage = from storageID in reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>().
                                    Where(item => item.WaybillID == hkWaybill.wbID).
                                    Select(item => item.StorageID).Distinct()
                                   join storage in reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>() on storageID equals storage.ID
                                   join sorting in reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Sortings>() on storage.SortingID equals sorting.ID
                                   select new
                                   {
                                       storage.ID,
                                       storage.Quantity,
                                       sorting.BoxCode
                                   };

                var storages = linq_storage.ToArray();
                #endregion

                #region 香港数据自动处理

                foreach (var storage in storages)
                {
                    var outQty = notices.Where(item => item.StorageID == storage.ID).Sum(item => item.Quantity);
                    var quantity = storage.Quantity - outQty;
                    if (quantity < 0)
                    {
                        throw new Exception("库存数量不足，出库失败");
                    }
                    reponsitory.Update<Layers.Data.Sqls.PvWms.Storages>(new
                    {

                        Quantity = quantity,
                    }, item => item.ID == storage.ID);
                }

                using (var pvcenterReponsitory = new PvCenterReponsitory())
                {

                    // 更新香港运单状态完成
                    pvcenterReponsitory.Update<Layers.Data.Sqls.PvCenter.Waybills>(new
                    {
                        ExcuteStatus = (int)CgPickingExcuteStatus.Completed,
                        ModifyDate = DateTime.Now,
                        ModifierID = Npc.Robot.Obtain(),
                    }, item => item.ID == hkWaybill.wbID);

                    /* 根据新要求, 报关相关的已发货状态不再使用
                    // 更新香港运单状态
                    if (string.IsNullOrEmpty(hkWaybill.OrderID))
                    {
                        //如果出库运单的OrderID为空
                        var waybillId = hkWaybill.wbID;
                        var outputids = notices.Select(item => item.OutputID).ToArray();
                        var orderids = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Outputs>().Where(item => outputids.Contains(item.ID))
                            .Select(item => item.OrderID).ToArray();
                        foreach (var id in orderids)
                        {
                            UpdateOrderStatus(id, pvcenterReponsitory);
                        }
                    }
                    else
                    {
                        UpdateOrderStatus(hkWaybill.OrderID, pvcenterReponsitory);
                    }
                    */

                    //更新深圳入库运单为“完成入库”
                    pvcenterReponsitory.Update<Layers.Data.Sqls.PvCenter.Waybills>(new
                    {
                        ExcuteStatus = (int)CgSortingExcuteStatus.Completed,
                        ModifyDate = DateTime.Now,
                        ModifierID = Npc.Robot.Obtain(),
                    }, item => item.ID == szWaybill.wbID);

                }
                #endregion

                #region 深圳数据自动处理 

                #region 报关内单的深圳出库运单
                //内单客户(注：内单出库运单的个数按客户个数生成、不按订单个数生成)
                List<CgWaybill> listWaybills = null;//运单列表
                List<Yahv.Services.Models.WsClient> listClients = null;//内单客户列表
                var clientIds = (from notice in notices
                                 join input in reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Inputs>() on notice.InputID equals input.ID
                                 where notice.Source == (int)CgNoticeSource.AgentBreakCustomsForIns
                                 select input.ClientID).Distinct().ToArray();
                if (clientIds.Count() > 0)
                {
                    listWaybills = new List<CgWaybill>();
                    using (var pvbcrmReponsitory = new PvbCrmReponsitory())
                    {
                        //交货人（公司）
                        var ConsignorID = szWaybill.coeID;
                        //收货人（客户）
                        listClients = new Yahv.Services.Views.WsClientsTopView<PvbCrmReponsitory>(pvbcrmReponsitory)
                            .Where(item => clientIds.Contains(item.ID)).ToList();
                        var Consignees = new Yahv.Services.Views.WsConsigneesTopView<PvbCrmReponsitory>(pvbcrmReponsitory);

                        foreach (var clientId in clientIds)
                        {
                            var client = listClients.Single(item => item.ID == clientId);
                            var consigneeView = Consignees.Where(item => item.EnterpriseID == clientId).ToArray();

                            var consignee = consigneeView.FirstOrDefault(item => item.IsDefault) ?? consigneeView.FirstOrDefault();//取收货信息的第一个默认值

                            if (client == null || consignee == null)
                            {
                                throw new Exception("内单客户" + clientId + "不存在或其深圳收货信息为空!");
                            }

                            CgWaybill waybill = new CgWaybill();
                            waybill.Type = WaybillType.DeliveryToWarehouse;
                            waybill.Consignor = new Yahv.Services.Models.WayParter()
                            {
                                ID = ConsignorID,
                            };
                            waybill.Consignee = new Yahv.Services.Models.WayParter()
                            {
                                Company = client.Name,
                                Place = Origin.CHN.GetOrigin().Code,
                                Address = consignee.Address,
                                Contact = consignee.Name,
                                Phone = consignee.Mobile + ";" + consignee.Tel,
                                Zipcode = consignee.Postzip,
                                Email = consignee.Email,
                            };
                            waybill.FreightPayer = WaybillPayer.Consignee;
                            waybill.EnterCode = client.EnterCode;
                            waybill.CreatorID = ConfigurationManager.AppSettings["OutAdminID"];
                            waybill.IsClearance = false;
                            waybill.WayCharge = null;
                            waybill.AppointTime = DateTime.Now.AddDays(1);
                            waybill.WayLoading = new Yahv.Services.Models.WayLoading()
                            {
                                TakingDate = null,
                                TakingAddress = null,
                                TakingContact = null,
                                TakingPhone = ConfigurationManager.AppSettings["AutoPhone"],
                                CarNumber1 = ConfigurationManager.AppSettings["AutoVehicle"],
                                Driver = ConfigurationManager.AppSettings["AutoDriver"],
                                Carload = null,
                                CreateDate = DateTime.Now,
                                CreatorID = ConfigurationManager.AppSettings["OutAdminID"],
                            };
                            waybill.WayChcd = null;
                            waybill.OrderID = null;//一般有多个订单，所以这里可为空
                            waybill.Source = CgNoticeSource.AgentBreakCustomsForIns;
                            waybill.NoticeType = CgNoticeType.Out;
                            waybill.ExcuteStatus = (int)CgPickingExcuteStatus.Picking;
                            waybill.CuttingOrderStatus = null;
                            waybill.Enter();
                            //添加到列表
                            listWaybills.Add(waybill);
                        }
                    }
                }
                #endregion

                var noticeSource = (CgNoticeSource)hkWaybill.Source;

                foreach (var notice in notices)
                {
                    var boxCode = storages.First(item => item.ID == notice.StorageID).BoxCode;

                    #region 生成深圳入库数据
                    var hkOuput = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Outputs>()
                        .Where(item => item.ID == notice.OutputID).FirstOrDefault();

                    var hkInput = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Inputs>()
                        .Where(item => item.ID == hkOuput.InputID).FirstOrDefault();

                    //深圳进项
                    Layers.Data.Sqls.PvWms.Inputs szInput;
                    var szInputID = Layers.Data.PKeySigner.Pick(PkeyType.Inputs);
                    if (noticeSource == CgNoticeSource.AgentCustomsFromStorage)
                    {
                        szInput = new Layers.Data.Sqls.PvWms.Inputs()
                        {
                            ID = szInputID,
                            Code = szInputID,
                            OrderID = hkOuput.OrderID,
                            TinyOrderID = hkOuput.TinyOrderID,
                            ItemID = hkOuput.ItemID,
                            ProductID = notice.ProductID,
                            ClientID = hkInput.ClientID,
                            TrackerID = hkOuput.TrackerID,
                            Currency = hkOuput.Currency,
                            UnitPrice = hkOuput.Price,//由荣检开票后提供准确深圳入库单价（报关价格+关税）
                            CreateDate = DateTime.Now,
                            PayeeID = hkInput.PayeeID,
                        };
                    }
                    else
                    {
                        szInput = new Layers.Data.Sqls.PvWms.Inputs()
                        {
                            ID = szInputID,
                            Code = szInputID,
                            OrderID = hkInput.OrderID,
                            TinyOrderID = hkInput.TinyOrderID,
                            ItemID = hkInput.ItemID,
                            ProductID = notice.ProductID,
                            ClientID = hkInput.ClientID,
                            TrackerID = hkInput.TrackerID,
                            Currency = hkOuput.Currency,
                            UnitPrice = hkOuput.Price,//由荣检开票后提供准确深圳入库单价（报关价格+关税）
                            CreateDate = DateTime.Now,
                        };
                    }
                    reponsitory.Insert(szInput);



                    //深圳入库通知
                    var szNotice = new Layers.Data.Sqls.PvWms.Notices()
                    {
                        ID = Layers.Data.PKeySigner.Pick(PkeyType.Notices),
                        Type = (int)CgNoticeType.Enter,
                        //WareHouseID = ConfigurationManager.AppSettings["SzWareHouseID"],//深圳库房ID 
                        WareHouseID = WhSettings.SZ[szWaybill.coeCompany].ID,
                        WaybillID = szWaybill.wbID,
                        InputID = szInput.ID,
                        ProductID = notice.ProductID,
                        DateCode = notice.DateCode,
                        Origin = notice.Origin,
                        Quantity = notice.Quantity,
                        Weight = notice.Weight,
                        NetWeight = notice.NetWeight,
                        Volume = notice.Volume,
                        BoxCode = boxCode,
                        BoxingSpecs = notice.BoxingSpecs,
                        Source = notice.Source,
                        Target = (int)NoticesTarget.Default,
                        Conditions = new NoticeCondition().Json(),
                        CreateDate = DateTime.Now,
                        Status = (int)NoticesStatus.Waiting,
                        Summary = notice.Summary,
                        CustomsName = notice.CustomsName,

                    };
                    reponsitory.Insert(szNotice);
                    //深圳分拣数据
                    var szSorting = new Layers.Data.Sqls.PvWms.Sortings()
                    {
                        ID = Layers.Data.PKeySigner.Pick(PkeyType.Sortings),
                        NoticeID = szNotice.ID,
                        InputID = szInput.ID,
                        WaybillID = szWaybill.wbID,
                        BoxCode = szNotice.BoxCode,
                        Quantity = szNotice.Quantity,
                        Weight = szNotice.Weight,
                        NetWeight = szNotice.NetWeight,
                        Volume = szNotice.Volume,
                        AdminID = Npc.Robot.Obtain(),
                        CreateDate = DateTime.Now,
                    };
                    reponsitory.Insert(szSorting);
                    //深圳库存数据
                    var szStorage = new Layers.Data.Sqls.PvWms.Storages()
                    {
                        ID = Layers.Data.PKeySigner.Pick(PkeyType.Storages),
                        Type = (int)CgStoragesType.Flows, //当前库存类型删除报关库，
                        WareHouseID = WhSettings.SZ[szWaybill.coeCompany].ID,
                        SortingID = szSorting.ID,
                        InputID = szInput.ID,
                        ProductID = szInput.ProductID,
                        Total = szSorting.Quantity,
                        Quantity = szSorting.Quantity,
                        DateCode = szNotice.DateCode,
                        Origin = szNotice.Origin,
                        IsLock = false,
                        Supplier = szNotice.Supplier,
                        CreateDate = DateTime.Now,
                        Status = (int)GeneralStatus.Normal,
                    };
                    reponsitory.Insert(szStorage);
                    #endregion

                    #region 生成深圳出库数据
                    if (notice.Source == (int)NoticeSource.AgentBreakCustomsForIns)
                    {
                        var client = listClients.Single(item => item.ID == szInput.ClientID);
                        var waybill = listWaybills.Single(item => item.EnterCode == client.EnterCode);
                        //深圳销项（产品的价值需要计算处理）
                        var szOutput = new Layers.Data.Sqls.PvWms.Outputs()
                        {
                            ID = Layers.Data.PKeySigner.Pick(PkeyType.Outputs),
                            InputID = szInput.ID,
                            OrderID = szInput.OrderID,
                            TinyOrderID = szInput.TinyOrderID,
                            ItemID = szInput.ItemID,
                            OwnerID = szInput.ClientID,
                            PurchaserID = szInput.PurchaserID,
                            Currency = szInput.Currency,
                            Price = szInput.UnitPrice,//由荣检开票后提供准确深圳出库单价（入库价格+代理费等）
                            CreateDate = DateTime.Now,
                            TrackerID = szInput.TrackerID,
                        };
                        reponsitory.Insert(szOutput);
                        //深圳出库通知
                        var szNoticeOut = new Layers.Data.Sqls.PvWms.Notices()
                        {
                            ID = Layers.Data.PKeySigner.Pick(PkeyType.Notices),
                            Type = (int)CgNoticeType.Out,
                            WareHouseID = WhSettings.SZ[szWaybill.coeCompany].ID,
                            WaybillID = waybill.ID,
                            InputID = szInput.ID,
                            OutputID = szOutput.ID,
                            ProductID = szNotice.ProductID,
                            Supplier = szNotice.Supplier,
                            DateCode = szNotice.DateCode,
                            Origin = szNotice.Origin,
                            Quantity = szNotice.Quantity,
                            Weight = szNotice.Weight,
                            NetWeight = szNotice.NetWeight,
                            Volume = szNotice.Volume,
                            BoxCode = szNotice.BoxCode,
                            BoxingSpecs = szNotice.BoxingSpecs,
                            Source = szNotice.Source,
                            Target = (int)NoticesTarget.Default,
                            Conditions = new NoticeCondition().Json(),
                            CreateDate = DateTime.Now,
                            Status = (int)NoticesStatus.Waiting,
                            Summary = szNotice.Summary,
                            ShelveID = szStorage.ShelveID,
                            StorageID = szStorage.ID,
                            CustomsName = notice.CustomsName
                        };
                        reponsitory.Insert(szNoticeOut);
                        //深圳拣货
                        var szPicking = new Layers.Data.Sqls.PvWms.Pickings()
                        {
                            ID = Layers.Data.PKeySigner.Pick(PkeyType.Pickings),
                            StorageID = szNoticeOut.StorageID,
                            NoticeID = szNoticeOut.ID,
                            OutputID = szNoticeOut.OutputID,
                            BoxCode = szNoticeOut.BoxCode,
                            Quantity = szNoticeOut.Quantity,
                            Weight = szNoticeOut.Weight,
                            NetWeight = szNoticeOut.NetWeight,
                            Volume = szNoticeOut.Volume,
                            AdminID = Npc.Robot.Obtain(),
                            CreateDate = DateTime.Now,
                        };
                        reponsitory.Insert(szPicking);
                    }
                    #endregion
                }
                #endregion

                trans.Commit();
            }
            //基于运输批次入库，乔霞这已经提供基于运输批次入库的功能
            //库存的库位可以为空，用目前乔霞的深圳入库页面进行操作。这是华芯通特殊的要求。
            //基于以上的讨论可以确定生成深圳的自动入库的通知的参数。根据运输批次号可以获取唯一的香港出库的Waybill,并根据这个Waybill生成深圳的入库通知

        }

        /// <summary>
        /// 报关香港出库
        /// 生成深圳库房的入库数据
        /// 新的AutoHkExit 只做库存数据的扣减,以及订单状态的更新
        /// </summary>
        /// <param name="lotNumber">运输批次号</param>
        public void AutoHkExit(string lotNumber)
        {
            //  点击后，先完成如下香港出库的内容：
            //  目前报关直接使用的是流水库，storages一定要扣除库存！

            CgWaybillsTopView hkWaybill = null;
            CgWaybillsTopView szWaybill = null;

            using (var reponsitory = new PvWmsRepository())
            //using (var pvcenterReponsitory = new PvCenterReponsitory())
            using (var trans = reponsitory.OpenTransaction())
            {
                #region 获取基础数据

                //根据运输批次号，查找报关运单
                var waybills = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.CgWaybillsTopView>()
                    .Where(item => item.chcdLotNumber == lotNumber).ToArray();
                //香港出库运单
                hkWaybill = waybills.Single(item => item.chcdLotNumber == lotNumber
                   && item.NoticeType == (int)CgNoticeType.Out && item.WareHouseID.StartsWith("HK"));
                if (hkWaybill.CuttingOrderStatus == (int)Enums.CutStatus.UnCutting)
                {
                    throw new Exception("运单未截单,出库失败!!!");
                }
                //深圳入库运单
                szWaybill = waybills.Single(item => item.chcdLotNumber == lotNumber
                   && item.NoticeType == (int)CgNoticeType.Enter && item.WareHouseID.StartsWith("SZ"));

                var linq_notice = from notice in reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                                  join output in reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Outputs>() on notice.OutputID equals output.ID
                                  where notice.WaybillID == hkWaybill.wbID
                                  select new
                                  {
                                      notice.ID,
                                      notice.StorageID,
                                      output.OrderID,
                                      notice.Quantity,
                                      notice.OutputID,
                                  };

                //香港报关出库通知
                var notices = linq_notice.ToArray();



                //没有处理出库的
                //证明了一下：只要索引与最终输出是合理的，linq 获取 几十万数据都是一瞬间
                var linq_storage = from storageID in reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>().
                                    Where(item => item.WaybillID == hkWaybill.wbID).
                                    Select(item => item.StorageID).Distinct()
                                   join storage in reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>() on storageID equals storage.ID
                                   join sorting in reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Sortings>() on storage.SortingID equals sorting.ID
                                   select new
                                   {
                                       storage.ID,
                                       storage.Quantity,
                                       sorting.BoxCode
                                   };

                var storages = linq_storage.ToArray();
                #endregion

                #region 香港数据自动处理
                //方案1:使用linq的Command
                Layers.Data.Sqls.PvWms.Storages storageNaming;
                //方案2:使用StringBuilder建立SQL 完成批处理
                StringBuilder sqlUpdateStorage = new StringBuilder();
                foreach (var storage in storages)
                {
                    var outQty = notices.Where(item => item.StorageID == storage.ID).Sum(item => item.Quantity);
                    var quantity = storage.Quantity - outQty;
                    if (quantity < 0)
                    {
                        throw new Exception("库存数量不足，出库失败");
                    }

                    //方案1 上
                    //陈翰说明：变为批处理执行
                    //                    reponsitory.Command($@"
                    //UPDATE [{nameof(Layers.Data.Sqls.PvWms.Storages)}]
                    //   SET [{nameof(storageNaming.Quantity)}] = {1}
                    // WHERE [ID]={0}", storage.ID, quantity);

                    //方案2
                    sqlUpdateStorage.AppendLine($@"UPDATE [{nameof(Layers.Data.Sqls.PvWms.Storages)}] SET [{nameof(storageNaming.Quantity)}] = {quantity} WHERE [ID]='{storage.ID}'");

                    //reponsitory.Update<Layers.Data.Sqls.PvWms.Storages>(new
                    //{
                    //    Quantity = quantity,
                    //}, item => item.ID == storage.ID);
                }

                //方案2 中
                reponsitory.Command(sqlUpdateStorage.ToString());

                //方案3  下
                //利用传统Ado的方式在服务端建立临时表
                //利用  SqlBulkCopyByDatatable 完成对临时表的 批处理 掺入
                // 利用 update from 对实际Storage 完成更新



                using (var pvcenterReponsitory = new PvCenterReponsitory())
                {

                    // 更新香港运单状态完成
                    pvcenterReponsitory.Update<Layers.Data.Sqls.PvCenter.Waybills>(new
                    {
                        ExcuteStatus = (int)CgPickingExcuteStatus.Completed,
                        ModifyDate = DateTime.Now,
                        ModifierID = Npc.Robot.Obtain(),
                    }, item => item.ID == hkWaybill.wbID);


                    // 更新香港订单状态， 根据新要求代报关,转报关,及报关内单的状态不再变成已发货，不再更新香港订单那状态
                    /*
                    if (string.IsNullOrEmpty(hkWaybill.OrderID))
                    {
                        //如果出库运单的OrderID为空
                        //var waybillId = hkWaybill.wbID;
                        //var outputids = notices.Select(item => item.OutputID).ToArray();
                        //var orderids = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Outputs>().Where(item => outputids.Contains(item.ID))
                        //    .Select(item => item.OrderID).Distinct().ToArray();
                        var orderids = notices.Select(item => item.OrderID).Distinct().ToArray();

                        StringBuilder sqlUpdateOrderStatus = new StringBuilder();
                        Layers.Data.Sqls.PvCenter.Logs_PvWsOrder ordersNaming;
                                                
                        foreach (var id in orderids)
                        {
                            //UpdateOrderStatus(id, pvcenterReponsitory);
                            sqlUpdateOrderStatus.AppendLine($@"UPDATE [{nameof(Layers.Data.Sqls.PvCenter.Logs_PvWsOrder)}] SET [{nameof(ordersNaming.IsCurrent)}] = 0 WHERE [Type] = 1 and [MainID]='{id}'");
                            sqlUpdateOrderStatus.AppendLine($@"INSERT INTO [{nameof(Layers.Data.Sqls.PvCenter.Logs_PvWsOrder)}] VALUES('{Guid.NewGuid()}','{id}',1,600,'{DateTime.Now}','{Npc.Robot.Obtain()}',1)");
                        }
                        pvcenterReponsitory.Command(sqlUpdateOrderStatus.ToString());

                    }
                    */

                    // 根据新要求代报关,转报关,及报关内单的已发货状态不再需要
                    //else
                    //{
                    //    UpdateOrderStatus(hkWaybill.OrderID, pvcenterReponsitory);
                    //}

                    //更新深圳入库运单为“完成入库”
                    pvcenterReponsitory.Update<Layers.Data.Sqls.PvCenter.Waybills>(new
                    {
                        ExcuteStatus = (int)CgSortingExcuteStatus.Completed,
                        ModifyDate = DateTime.Now,
                        ModifierID = Npc.Robot.Obtain(),
                    }, item => item.ID == szWaybill.wbID);

                }
                #endregion

                trans.Commit();
            }
            //基于运输批次入库，乔霞这已经提供基于运输批次入库的功能
            //库存的库位可以为空，用目前乔霞的深圳入库页面进行操作。这是华芯通特殊的要求。
            //基于以上的讨论可以确定生成深圳的自动入库的通知的参数。根据运输批次号可以获取唯一的香港出库的Waybill,并根据这个Waybill生成深圳的入库通知

        }

        /// <summary>
        /// 更新订单日志, 非代报关业务订单的状态为已收货 
        /// </summary>
        /// <param name="orderid"></param>
        /// <param name="pvcenterReponsitory"></param>
        public void UpdateOrderStatus(string orderid, PvCenterReponsitory pvcenterReponsitory)
        {
            //保存订单日志, 非代报关业务订单的状态为已发货
            pvcenterReponsitory.Update<Layers.Data.Sqls.PvCenter.Logs_PvWsOrder>(new
            {
                IsCurrent = false,
            }, item => item.Type == 1 && item.MainID == orderid);

            pvcenterReponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_PvWsOrder
            {
                ID = Guid.NewGuid().ToString(),
                MainID = orderid,
                Type = 1, //订单主状态，  
                Status = (int)CgOrderStatus.客户待收货,
                CreateDate = DateTime.Now,
                CreatorID = Npc.Robot.Obtain(),
                IsCurrent = true,
            });
        }

        /// <summary>
        /// 更新香港封条号
        /// </summary>
        /// <param name="waybillID"></param>
        /// <param name="hkSealNumber"></param>
        public void UpdateHKSealNumber(string waybillID, string hkSealNumber)
        {
            using (var pvcenterReponsitory = new PvCenterReponsitory())
            {
                pvcenterReponsitory.Update<Layers.Data.Sqls.PvCenter.WayChcd>(new
                {
                    HKSealNumber = hkSealNumber,
                }, item => item.ID == waybillID);
            }
        }

        /// <summary>
        /// 导出WaybillID对应的 货物流转书，或提货委托书
        /// </summary>
        /// <param name="waybillID"></param>
        /// <param name="fileName"></param>
        /// <param name="basePath"></param>
        /// <returns></returns>
        public string ExportFile(string waybillID, string fileName, string basePath, int totalParts)
        {
            using (var reponsitory = new PvWmsRepository())
            {
                //string realName = $"{waybillID}{fileName}{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-ffff")}.pdf";
                //string realName = $"{waybillID}{fileName}.pdf";
                string realName = $"{waybillID}{fileName}{Guid.NewGuid()}.pdf";
                string exportPath = $"{basePath}Export/";
                string templatePath = $"{basePath}Template/";
                string realFilePath = exportPath + realName;
                string templateFilePath = string.Empty;

                if (!Directory.Exists(exportPath))
                {
                    Directory.CreateDirectory(exportPath);
                }

                if (fileName == "thwt")
                {
                    templateFilePath = templatePath + "提货委托书模板.xlsx";
                }

                if (fileName == "hwlz")
                {
                    templateFilePath = templatePath + "货物流转书模板.xlsx";
                }

                Workbook workBook = new Workbook(templateFilePath);
                WorkbookDesigner designer = new WorkbookDesigner(workBook);

                var waybillChcdView = from entity in reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>()
                                      join _carrier in reponsitory.ReadTable<Layers.Data.Sqls.PvWms.CarriersTopView>() on entity.wbCarrierID equals _carrier.ID into carriers
                                      from carrier in carriers.DefaultIfEmpty()
                                      where entity.wbID == waybillID
                                      select new
                                      {
                                          entity.chcdDriver,
                                          chcdCarrierName = carrier.Name,
                                          entity.chcdDriverCode,
                                          entity.chcdCarNumber2, //境内车牌
                                          entity.chcdCarNumber1, // 香港车牌
                                          entity.chcdPhone,
                                          entity.chcdHKPhone,
                                          entity.chcdVehicleSize,
                                          entity.chcdDriverCardNo,
                                          entity.chcdVehicleType,
                                          entity.chcdCustomsPort,
                                          entity.chcdCarload,
                                          entity.chcdLotNumber,
                                          entity.wbTotalParts,
                                          entity.wbTotalWeight,
                                          entity.chcdHKSealNumber
                                      };

                var waybillChcd = waybillChcdView.FirstOrDefault();
                designer.SetDataSource("DriverName", waybillChcd.chcdDriver); //司机姓名
                designer.SetDataSource("CarrierName", waybillChcd.chcdCarrierName); //牌头公司
                designer.SetDataSource("DriverCode", waybillChcd.chcdDriverCode); //证件号
                designer.SetDataSource("VehicleLicence", waybillChcd.chcdCarNumber2); //境内车牌
                designer.SetDataSource("DriverMobile", waybillChcd.chcdPhone); //大陆手机
                designer.SetDataSource("HKLicense", waybillChcd.chcdCarNumber1); //港境外车牌
                designer.SetDataSource("DriverHKMobile", waybillChcd.chcdHKPhone); //香港手机
                designer.SetDataSource("DriverSize", waybillChcd.chcdVehicleSize); //尺寸
                designer.SetDataSource("DriverCardNo", waybillChcd.chcdDriverCardNo); //司机卡号
                designer.SetDataSource("VehicleType", waybillChcd.chcdVehicleType.HasValue ? (((VehicleType)waybillChcd.chcdVehicleType.Value).GetDescription()) : ""); //车型
                designer.SetDataSource("Location", waybillChcd.chcdCustomsPort); //口岸
                designer.SetDataSource("VehicleWeight", waybillChcd.chcdCarload.HasValue ? (waybillChcd.chcdCarload.Value + "KG") : ""); //车重
                designer.SetDataSource("LotNumber", waybillChcd.chcdLotNumber); //车辆批次号
                designer.SetDataSource("TotalParts", totalParts.ToString() + "件"); //总件数
                designer.SetDataSource("TotalWeight", waybillChcd.wbTotalWeight.HasValue ? (decimal.Round(waybillChcd.wbTotalWeight.Value, 2) + "KG") : ""); //总重量
                designer.SetDataSource("CurrentDateTime", DateTime.Now.ToString("yyyy-MM-dd")); //当前日期
                designer.SetDataSource("HKSealNumber", waybillChcd.chcdHKSealNumber); //当前日期
                designer.Process();
                workBook.Save(realFilePath, SaveFormat.Pdf);
                designer = null;
                return realName;
            }
        }

        /// <summary>
        /// 自动处理财务数据，后续深圳这边的出库时依然要使用这个方法来处理财务数据
        /// </summary>
        public void AutoVouchers(string lotNumber, string orderId = "")
        {
            using (var repCrm = new PvbCrmReponsitory())
            using (var repWms = new PvWmsRepository())
            {
                if (string.IsNullOrEmpty(lotNumber)) return;

                //  获取参数视图
                //  var linqs = waybillTopView join notices join outputs
                //  select outputs.tinyOrderID ,outputs.OrderID
                //  where lotNumber
                //  var arry = linqs.toarry();
                var linqs = from waybill in repWms.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>()
                            join notice in repWms.ReadTable<Layers.Data.Sqls.PvWms.Notices>() on waybill.wbID equals notice.WaybillID
                            join output in repWms.ReadTable<Layers.Data.Sqls.PvWms.Outputs>() on notice.OutputID equals output.ID
                            where waybill.chcdLotNumber == lotNumber
                            && (waybill.Source == (int)CgNoticeSource.AgentBreakCustoms
                            || waybill.Source == (int)CgNoticeSource.AgentBreakCustomsForIns
                            || waybill.Source == (int)CgNoticeSource.AgentCustomsFromStorage)
                            select new
                            {
                                OrderID = output.OrderID,
                                TinyOrderID = output.TinyOrderID,
                            };

                if (!string.IsNullOrWhiteSpace(orderId))
                {
                    linqs = linqs.Where(item => item.OrderID == orderId);
                }

                var array = linqs.ToArray();
                if (array.Length <= 0) return;

                //分组
                var groups = array.GroupBy(item => item.OrderID).Select(item => new
                {
                    OrderID = item.Key,
                    //TinyOrdersID = item.Select(toi => toi.TinyOrderID)
                    TinyOrdersID = item.FirstOrDefault(toi => !string.IsNullOrEmpty(toi.TinyOrderID))?.TinyOrderID
                }).ToArray();

                foreach (var igroup in groups)
                {
                    repCrm.Update<Layers.Data.Sqls.PvbCrm.Receivables>(new
                    {
                        TinyID = igroup.TinyOrdersID
                    }, item => item.OrderID == igroup.OrderID && item.TinyID == null);


                    repCrm.Update<Layers.Data.Sqls.PvbCrm.Payables>(new
                    {
                        TinyID = igroup.TinyOrdersID
                    }, item => item.OrderID == igroup.OrderID && item.TinyID == null);
                    // 索引一定要做正确
                }

                // 循环 arry 分组的 OrderID  并自动自动更新 ,理论上应该是 Order 与 TinyOrder 为一对一的
                // 把没有指定 tinyID 的应收都更新为正确 tinyOrderID


                //收入 通现金 或者 大陆来货清关费（记账）
                //根据订单ID和小订单ID筛选对账单
                var vouchersView = new Wms.Services.Views.VouchersStatisticsView(repWms);
                //型号ID为空，代表是库房添加的费用
                var vouchers = vouchersView.Where(item => (item.ItemID == null || item.ItemID == "")
                                        && (item.Catalog == CatalogConsts.杂费 || item.Catalog == CatalogConsts.仓储服务费)
                                        && (item.RightPrice != null || (item.RightPrice == null && item.Subject == "大陆来货清关费")))
                                        .Where(item => groups.Select(g => g.OrderID).Contains(item.OrderID) && groups.Select(g => g.TinyOrdersID).Contains(item.TinyID)).ToArray();
                //库房费用格式
                List<Premium> premiums = new List<Premium>();
                foreach (var voucher in vouchers)
                {
                    premiums.Add(new Premium()
                    {
                        ID = voucher.ReceivableID,
                        TinyOrderID = voucher.TinyID,
                        Currency = voucher.Currency.GetCurrency().ShortName,
                        AdminID = voucher.AdminID,
                        PaymentType = (voucher.RightDate == voucher.LeftDate) ? WhsePaymentType.Cash : WhsePaymentType.UnCash,
                        UnitPrice = voucher.LeftPrice,
                        WhesFeeType = ConverterSubject(voucher.Subject),
                        Count = 1,
                        CreateDate = voucher.LeftDate,
                    });
                }

                //支出费用【非现金】
                var paymentView = new Wms.Services.Views.PaymentsStatisticsView(repWms);
                var payments = paymentView.Where(item => (item.Catalog == CatalogConsts.杂费 || item.Catalog == CatalogConsts.仓储服务费) && item.RightPrice == null)
                                    .Where(item => groups.Select(g => g.OrderID).Contains(item.OrderID) && groups.Select(g => g.TinyOrdersID).Contains(item.TinyID)).ToArray();
                foreach (var payment in payments)
                {
                    premiums.Add(new Premium()
                    {
                        ID = payment.PayableID,
                        TinyOrderID = payment.TinyID,
                        Currency = payment.Currency.GetCurrency().ShortName,
                        AdminID = payment.AdminID,
                        PaymentType = (payment.RightDate == payment.LeftDate) ? WhsePaymentType.Cash : WhsePaymentType.UnCash,
                        UnitPrice = payment.LeftPrice,
                        WhesFeeType = ConverterSubject(payment.Subject),
                        Count = 1,
                        CreateDate = payment.LeftDate,
                    });
                }

                //调用荣检接口  排重的 tinyOrderID
                string url = FromType.ForReceivables.GetDescription();
                var data = new WhesPremium(premiums);
                try
                {
                    if (data.Premiums.Count <= 0)
                    {
                        return;
                    }

                    var result = Yahv.Utils.Http.ApiHelper.Current.JPost(url, data);
                    Yahv.Payments.Oplogs.Oplog(FixedRole.Npc.ToString(), url, "Wms", nameof(Wms.Services.chonggous.Views.CgDelcareShipView.AutoVouchers), $"华芯通推送财务数据完成|{lotNumber}|{result}", data.Json());
                }
                catch (Exception ex)
                {
                    Yahv.Payments.Oplogs.Oplog(FixedRole.Npc.ToString(), url, "Wms", nameof(Wms.Services.chonggous.Views.CgDelcareShipView.AutoVouchers), $"华芯通推送财务数据失败!|{lotNumber}|{data.Json()}", ex.ToString());
                }
            }
        }

        /// <summary>
        /// 深圳上架
        /// </summary>
        /// <param name="Boxcode">箱号</param>
        /// <param name="shelveID">库位</param>
        public void SzPlace(string shelveID, string Boxcode)
        {
            //深圳上架有特殊的华芯通要求的流程，把指定的箱号或是产品(单条申报)直接分配库位
            //具体可以参加乔霞的页面
            using (var repository = new PvWmsRepository())
            {
                //更新库存库位
                var linq = from notice in repository.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                           join sorting in repository.ReadTable<Layers.Data.Sqls.PvWms.Sortings>() on notice.ID equals sorting.NoticeID
                           where notice.Type == (int)CgNoticeType.Enter && notice.BoxCode == Boxcode && notice.WareHouseID.Contains(WhSettings.SZ.ID)
                           select sorting.ID;

                var sortingsID = linq.ToArray();

                repository.Update<Layers.Data.Sqls.PvWms.Storages>(new
                {
                    ShelveID = shelveID,
                }, item => sortingsID.Contains(item.SortingID));
            }
        }

        /// <summary>
        /// 深圳上架
        /// </summary>
        /// <param name="boxcodes">箱号列表</param>
        /// <param name="shelveID">库位号</param>
        /// <remarks></remarks>
        public void SzPlace(string shelveID, params string[] boxcodes)
        {
            using (var repository = new PvWmsRepository())
            {
                //更新库存库位
                var linq = from notice in repository.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                           join sorting in repository.ReadTable<Layers.Data.Sqls.PvWms.Sortings>() on notice.ID equals sorting.NoticeID
                           where notice.Type == (int)CgNoticeType.Enter && boxcodes.Contains(notice.BoxCode) && notice.WareHouseID.Contains(WhSettings.SZ.ID)
                           select sorting.ID;

                var sortingsID = linq.ToArray();

                repository.Update<Layers.Data.Sqls.PvWms.Storages>(new
                {
                    ShelveID = shelveID,
                }, item => sortingsID.Contains(item.SortingID));
            }
        }

        /// <summary>
        /// 深圳出入库单价更新(开票后)
        /// </summary>
        /// <remarks>
        /// 分批次报关的三个小订单，在深圳可以随意取货发送给客户
        /// 开票以小订单为单位进行
        /// 需要统一修改Input=(人民币)货值+关税？、Output=增值+杂费+....
        /// Update (OrderID,TinyOrderID,OrderItemID,inPrice,outPrice)  ？Curreny, [ok]
        /// 需要自动化定义库房
        /// </remarks>
        public void SzPriceUpdate(CgDelcareSZPrice szPrice)
        {
            using (var reponsitory = new PvWmsRepository())
            {
                var inputs = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Inputs>();
                var notices = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>();
                var outputs = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Outputs>();

                var updateInputs = from input in inputs
                                   join notice in notices on input.ID equals notice.InputID
                                   where (notice.Type == (int)CgNoticeType.Enter && notice.WareHouseID.StartsWith(WhSettings.SZ.ID))
                                   select new
                                   {
                                       ID = input.ID,
                                       OrderID = input.OrderID,
                                       TinyOrderID = input.TinyOrderID,
                                       ItemID = input.ItemID,
                                   };
                var updateOutputs = from output in outputs
                                    join notice in notices on output.ID equals notice.OutputID
                                    where (notice.Type == (int)CgNoticeType.Out && notice.WareHouseID.StartsWith(WhSettings.SZ.ID))
                                    select new
                                    {
                                        ID = output.ID,
                                        OrderID = output.OrderID,
                                        TinyOrderID = output.TinyOrderID,
                                        ItemID = output.ItemID,
                                    };

                //var tinyOrderIds = szPrice.Items.Select(item => item.TinyOrderID);
                //var orderIds = szPrice.Items.Select(item => item.OrderID);

                ////跨订单
                //var inputIDs1 = updateInputs.Where(t => tinyOrderIds.Contains(t.TinyOrderID)
                //    && orderIds.Contains(t.OrderID)).Select(t => t.ID).ToArray();
                //var outputIDs1 = updateOutputs.Where(t => tinyOrderIds.Contains(t.TinyOrderID)
                //    && orderIds.Contains(t.OrderID)).Select(t => t.ID).ToArray();

                foreach (var item in szPrice.Items)
                {
                    var inputIDs = updateInputs.Where(t => t.ItemID == item.OrderItemID).Select(t => t.ID).ToArray();
                    var outputIDs = updateOutputs.Where(t => t.ItemID == item.OrderItemID).Select(t => t.ID).ToArray();

                    reponsitory.Update<Layers.Data.Sqls.PvWms.Inputs>(new
                    {
                        UnitPrice = item.InUnitPrice,
                        Currency = (int)Currency.CNY,
                    }, t => inputIDs.Contains(t.ID));

                    reponsitory.Update<Layers.Data.Sqls.PvWms.Outputs>(new
                    {
                        Price = item.OutUnitPrice,
                        Currency = (int)Currency.CNY,
                    }, t => outputIDs.Contains(t.ID));
                }
            }
        }

        /// <summary>
        /// 根据订单ID获取批次号
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public string GetLotNumberByOrderID(string orderId)
        {
            using (var reposntiory = LinqFactory<PvWmsRepository>.Create())
            {
                var linqs = from waybill in reposntiory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>()
                            join notice in reposntiory.ReadTable<Layers.Data.Sqls.PvWms.Notices>() on waybill.wbID equals notice.WaybillID
                            join output in reposntiory.ReadTable<Layers.Data.Sqls.PvWms.Outputs>() on notice.OutputID equals output.ID
                            where output.OrderID == orderId
                            && (waybill.Source == (int)CgNoticeSource.AgentBreakCustoms
                            || waybill.Source == (int)CgNoticeSource.AgentBreakCustomsForIns
                            || waybill.Source == (int)CgNoticeSource.AgentCustomsFromStorage)
                            select waybill.chcdLotNumber;

                return linqs.FirstOrDefault();
            }
        }

        public void InitData(DateTime begin, DateTime end)
        {
            using (var repWms = new PvWmsRepository())
            {
                //  获取参数视图
                var lotnumbers = (from waybill in repWms.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>()
                                  join notice in repWms.ReadTable<Layers.Data.Sqls.PvWms.Notices>() on waybill.wbID equals notice.WaybillID
                                  join output in repWms.ReadTable<Layers.Data.Sqls.PvWms.Outputs>() on notice.OutputID equals output.ID
                                  where waybill.wbCreateDate > begin && waybill.wbCreateDate <= end
                                  && (waybill.Source == (int)CgNoticeSource.AgentBreakCustoms
                                  || waybill.Source == (int)CgNoticeSource.AgentBreakCustomsForIns
                                  || waybill.Source == (int)CgNoticeSource.AgentCustomsFromStorage)
                                  select waybill.chcdLotNumber).Distinct().ToArray();

                if (lotnumbers.Length > 0)
                {
                    foreach (var lotnumber in lotnumbers)
                    {
                        //if (lotnumber == "1100365044194")
                        //{
                        //    AutoVouchers(lotnumber);
                        //}

                        //AutoVouchers(lotnumber);
                        Thread.Sleep(500);
                    }
                }
            }
        }
        #endregion

        #region 高会航帮助

        /// <summary>
        /// 导出xls
        /// </summary>
        /// <param name="waybillID">运单号</param>
        public void ForXls(string waybillID)
        {
            var data = SearchByID(waybillID);



        }

        #endregion

    }
}
