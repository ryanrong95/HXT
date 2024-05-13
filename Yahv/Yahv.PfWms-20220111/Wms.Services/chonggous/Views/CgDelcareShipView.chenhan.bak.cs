using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms.Services.chonggous.Models;
using Yahv.Linq;
using Yahv.Services.Enums;
using Yahv.Underly;

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
            var waybillViews = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>();
            //new Wms.Services.Views.ServicesWaybillsTopView(this.Reponsitory);
            var carrierViews = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.CarriersTopView>();


            /*
            箱号：装箱时间
            装箱人：
            信号
            订单号
            客户编码
            客户名称
            */

            /*
            运输批次号
            运输时间
            承运商
            车牌号
            司机
            运输类型
            状态：已经去除
            截单状态
            */

            return from waybill in waybillViews
                   where waybill.NoticeType == (int)CgNoticeType.Out
                         && (waybill.Source == (int)CgNoticeSource.AgentBreakCustoms
                             || waybill.Source == (int)CgNoticeSource.AgentCustomsFromStorage
                             || waybill.Source == (int)CgNoticeSource.AgentBreakCustomsForIns
                         )
                   join carrier in carrierViews on waybill.wbCarrierID equals carrier.ID
                   where waybill.CuttingOrderStatus == (int)CuttingOrderStatus.Cutting
                   select new MyWaybill
                   {
                       ID = waybill.wbID,
                       PlanDate = waybill.chcdPlanDate,
                       CarrierName = carrier.Name,
                       CarNumber1 = waybill.chcdCarNumber1,
                       CarNumber2 = waybill.chcdCarNumber2,
                       Driver = waybill.chcdDriver,
                       DepartDate = waybill.chcdDepartDate, // 已经与荣检确定!
                       Condition = waybill.wbCondition,
                       CuttingOrderStatus = waybill.CuttingOrderStatus, //不要做处理，不然会破坏索引！
                       LotNumber = waybill.chcdLotNumber,

                   };


        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            var iquery = this.IQueryable.Cast<MyWaybill>();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            var adminsTopView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.AdminsTopView>();
            var productsTopView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.ProductsTopView>();

            var ienums_myWaybill = this.IQueryable.Cast<MyWaybill>().ToArray();
            var waybillIds = ienums_myWaybill.Select(item => item.ID);

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
                             join log in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Logs_Declare>() on output.TinyOrderID equals log.TinyOrderID
                             join admin in adminsTopView on log.AdminID equals admin.ID
                             where waybillIds.Contains(notice.WareHouseID)
                             select new
                             {
                                 notice.WaybillID,
                                 log.BoxCode,
                                 Packer = admin.RealName,
                                 product.PartNumber,
                                 product.Manufacturer,
                                 notice.Quantity,//通知数量，拣货数量 ，这里简化因为必须符合申报数量
                                 log.EnterCode,
                             };

            var ienums_notice = noticeView.ToArray();


            var linq = from waybill in ienums_myWaybill
                       join notice in ienums_notice on waybill.ID equals notice.WaybillID into notices
                       select new
                       {
                           ID = waybill.ID,
                           PlanDate = waybill.PlanDate,
                           CarrierName = waybill.CarrierName,
                           CarNumber1 = waybill.CarNumber1,
                           CarNumber2 = waybill.CarNumber2,
                           Driver = waybill.Driver,
                           DepartDate = waybill.DepartDate,
                           Condition = waybill.Condition,
                           CuttingOrderStatus = (CuttingOrderStatus)(waybill.CuttingOrderStatus ?? (int)CuttingOrderStatus.Waiting),
                           Notices = notices.Select(item => new
                           {
                               item.BoxCode,
                               Packer = item.Packer,
                               item.PartNumber,
                               item.Manufacturer,
                               item.Quantity,
                               item.EnterCode,
                           })
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

            int total = iquery.Count();

            return new
            {
                Total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                Data = results.ToArray(),
            };
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
            var myWaybill = this.IQueryable.Cast<MyWaybill>().Where(item => item.CarrierName.Contains(name));
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

        #endregion

        #region 内部帮助类

        private class MyWaybill
        {
            public string ID { get; set; }
            public DateTime? PlanDate { get; set; }
            public string CarrierName { get; set; }
            public string LotNumber { get; set; }

            public string CarNumber1 { get; set; }
            public string CarNumber2 { get; set; }
            public string Driver { get; set; }
            public DateTime? DepartDate { get; set; }
            public string Condition { get; set; }
            public int? CuttingOrderStatus { get; set; }
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

        #endregion

    }
}
