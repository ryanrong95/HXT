using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Erp.Generic;

namespace Needs.Wl.Admin.Plat.Models
{
    public partial class Admin
    {
        public Warehouse Warehouse
        {
            get
            {
                return new Warehouse(this);
            }
        }
    }

    public class Warehouse
    {
        IGenericAdmin Admin;

        public Warehouse(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        /// <summary>
        /// 提货通知
        /// </summary>
        public Needs.Ccs.Services.Views.DeliveryNoticeView DeliveryNotice
        {
            get
            {
                return new Needs.Ccs.Services.Views.DeliveryNoticeView();
            }
        }

        /// <summary>
        /// 提货人
        /// </summary>
        public Needs.Ccs.Services.Views.DeliveryPickerView DeliveryPicker
        {
            get
            {
                return new Needs.Ccs.Services.Views.DeliveryPickerView();
            }
        }

        /// <summary>
        /// 提货日志
        /// </summary>
        public Needs.Ccs.Services.Views.DeliveryNoticeLogView DeliveryNoticeLog
        {
            get
            {
                return new Needs.Ccs.Services.Views.DeliveryNoticeLogView();
            }
        }

        /// <summary>
        /// 暂存记录
        /// </summary>
        public Needs.Ccs.Services.Views.TemporaryView Temporary
        {
            get
            {
                return new Needs.Ccs.Services.Views.TemporaryView();
            }
        }

        /// <summary>
        /// 暂存文件
        /// </summary>
        public Needs.Ccs.Services.Views.TemporaryFileView TemporaryFile
        {
            get
            {
                return new Needs.Ccs.Services.Views.TemporaryFileView();
            }
        }

        /// <summary>
        /// 暂存通知
        /// </summary>
        public Needs.Ccs.Services.Views.TemporaryLogView TemporaryLog
        {
            get
            {
                return new Needs.Ccs.Services.Views.TemporaryLogView();
            }
        }

        /// <summary>
        /// 入库通知
        /// </summary>
        public Needs.Ccs.Services.Views.EntryNoticeView EntryNotice
        {
            get
            {
                return new Needs.Ccs.Services.Views.EntryNoticeView();
            }
        }

        /// <summary>
        /// 香港入库通知
        /// </summary>
        public Needs.Ccs.Services.Views.HKEntryNoticeView HKEntryNotice
        {
            get
            {
                return new Needs.Ccs.Services.Views.HKEntryNoticeView();
            }
        }

        /// <summary>
        /// 香港入库通知
        /// </summary>
        public Needs.Ccs.Services.Views.HKEntryNoticeSimpleView HKEntryNoticeSimple
        {
            get
            {
                return new Needs.Ccs.Services.Views.HKEntryNoticeSimpleView();
            }
        }

        public Needs.Ccs.Services.Views.Alls.HKEntryNoticesAll HKEntryNoticesAll
        {
            get
            {
                return new Needs.Ccs.Services.Views.Alls.HKEntryNoticesAll();
            }
        }

        public Needs.Ccs.Services.Views.Alls.HKEntryNoticesWithItemsAll HKEntryNoticesWithItemsAll
        {
            get
            {
                return new Needs.Ccs.Services.Views.Alls.HKEntryNoticesWithItemsAll();
            }
        }

        /// <summary>
        /// 深圳入库通知
        /// </summary>
        public Needs.Ccs.Services.Views.SZEntryNoticeView SZEntryNotice
        {
            get
            {
                return new Needs.Ccs.Services.Views.SZEntryNoticeView();
            }
        }

        /// <summary>
        /// 深圳库房入库通知
        /// </summary>
        public Needs.Wl.Warehouse.Services.Views.SZEntryNoticeView NewSZEntryNotice
        {
            get
            {
                return new Needs.Wl.Warehouse.Services.Views.SZEntryNoticeView();
            }
        }

        /// <summary>
        /// 入库通知项
        /// </summary>
        public Needs.Ccs.Services.Views.EntryNoticeItemView EntryNoticeItems
        {
            get
            {
                return new Needs.Ccs.Services.Views.EntryNoticeItemView();
            }
        }
        /// <summary>
        /// 入库通知项
        /// </summary>
        public Needs.Ccs.Services.Views.HKEntryNoticeItemView HKEntryNoticeItems
        {
            get
            {
                return new Needs.Ccs.Services.Views.HKEntryNoticeItemView();
            }
        }
        /// <summary>
        /// 入库通知项
        /// </summary>
        public Needs.Ccs.Services.Views.SZEntryNoticeItemView SZEntryNoticeItems
        {
            get
            {
                return new Needs.Ccs.Services.Views.SZEntryNoticeItemView();
            }
        }

        /// <summary>
        /// 出库通知
        /// </summary>
        public Needs.Ccs.Services.Views.ExitNoticeView ExitNotice
        {
            get
            {
                return new Needs.Ccs.Services.Views.ExitNoticeView();
            }
        }

        /// <summary>
        /// 香港出库通知
        /// </summary>
        public Needs.Ccs.Services.Views.HKExitNoticeView HKExitNotice
        {
            get
            {
                return new Needs.Ccs.Services.Views.HKExitNoticeView();
            }
        }

        /// <summary>
        /// 深圳出库通知
        /// </summary>
        public Needs.Ccs.Services.Views.SZExitNoticeView SZExitNotice
        {
            get
            {
                return new Needs.Ccs.Services.Views.SZExitNoticeView();
            }
        }

        public Needs.Ccs.Services.Views.SZMianExitNoticeView SZMianExitNoticeView
        {
            get
            {
                return new Needs.Ccs.Services.Views.SZMianExitNoticeView();
            }
        }


        /// <summary>
        /// 深圳仓库 已出库列表
        /// </summary>
        public Needs.Ccs.Services.Views.SZExitedListView SZExitedListView
        {
            get
            {
                return new Needs.Ccs.Services.Views.SZExitedListView();
            }
        }

        /// <summary>
        /// 深圳仓库 待出库列表
        /// </summary>
        public Needs.Ccs.Services.Views.SZUnExitedListView SZUnExitedListView
        {
            get
            {
                return new Needs.Ccs.Services.Views.SZUnExitedListView();
            }
        }

        /// <summary>
        /// 深圳出库通知-未出库
        /// </summary>
        public Needs.Ccs.Services.Views.SZExitNoticeUnExitedView SZExitNoticeUnExited
        {
            get
            {
                return new Needs.Ccs.Services.Views.SZExitNoticeUnExitedView();
            }
        }
        /// <summary>
        /// 我的跟单-出库单
        /// </summary>
        public Views.MySZExitNoticesView MySZExitNotice
        {
            get
            {
                return new Views.MySZExitNoticesView(this.Admin);
            }
        }

        /// <summary>
        /// 我的跟单-出库单 中心库
        /// </summary>
        public Views.MyCenterSZExitNoticesView MyCenterSZExitNotice
        {
            get
            {
                return new Views.MyCenterSZExitNoticesView(this.Admin);
            }
        }

        public Views.MyNewCenterSZExitNoticesView MyNewCenterSZExitNotice
        {
            get
            {
                return new Views.MyNewCenterSZExitNoticesView(this.Admin);
            }
        }


        /// <summary>
        /// 出库通知项
        /// </summary>
        public Needs.Ccs.Services.Views.ExitNoticeItemView ExitNoticeItem
        {
            get
            {
                return new Needs.Ccs.Services.Views.ExitNoticeItemView();
            }
        }

        /// <summary>
        /// 香港出库通知项
        /// </summary>
        public Needs.Ccs.Services.Views.HKExitNoticeItemView HKExitNoticeItem
        {
            get
            {
                return new Needs.Ccs.Services.Views.HKExitNoticeItemView();
            }
        }

        public Needs.Ccs.Services.Views.HKExitNoticeItemView ExitProductData
        {
            get
            {
                return new Needs.Ccs.Services.Views.HKExitNoticeItemView();
            }
        }
        /// <summary>
        /// 深圳出库通知项
        /// </summary>
        public Needs.Ccs.Services.Views.SZExitNoticeItemView SZExitNoticeItem
        {
            get
            {
                return new Needs.Ccs.Services.Views.SZExitNoticeItemView();
            }
        }

        /// <summary>
        /// 出库通知文件
        /// </summary>
        public Needs.Ccs.Services.Views.ExitNoticeFileView ExitNoticeFile
        {
            get
            {
                return new Needs.Ccs.Services.Views.ExitNoticeFileView();
            }
        }

        //出库交货信息
        public Needs.Ccs.Services.Views.ExitDeliverView ExitDeliver
        {
            get
            {
                return new Needs.Ccs.Services.Views.ExitDeliverView();
            }
        }

        /// <summary>
        /// 装箱结果
        /// </summary>
        public Needs.Ccs.Services.Views.PackingsView Packing
        {
            get
            {
                return new Needs.Ccs.Services.Views.PackingsView();
            }
        }

        /// <summary>
        /// 装箱结果项
        /// </summary>
        public Needs.Ccs.Services.Views.PackingItemsView PackingItem
        {
            get
            {
                return new Needs.Ccs.Services.Views.PackingItemsView();
            }
        }

        /// <summary>
        /// 仓库费用
        /// </summary>
        public Needs.Ccs.Services.Views.OrderWhesPremiumView OrderWhesPremium
        {
            get
            {
                return new Needs.Ccs.Services.Views.OrderWhesPremiumView();
            }
        }

        public Needs.Ccs.Services.Views.Alls.OrderWhesPremiumsAll OrderWhesPremiumsAll
        {
            get
            {
                return new Needs.Ccs.Services.Views.Alls.OrderWhesPremiumsAll();
            }
        }

        /// <summary>
        /// 费用附件
        /// </summary>
        public Needs.Ccs.Services.Views.OrderWhesPremiumFileView OrderWhesPremiumFile
        {
            get
            {
                return new Needs.Ccs.Services.Views.OrderWhesPremiumFileView();
            }
        }

        /// <summary>
        /// 附件日志
        /// </summary>
        public Needs.Ccs.Services.Views.OrderWhesPremiumLogView OrderWhesPremiumLogs
        {
            get
            {
                return new Needs.Ccs.Services.Views.OrderWhesPremiumLogView();
            }
        }

        /// <summary>
        /// 库房分拣
        /// </summary>
        public Needs.Ccs.Services.Models.HKSortingContext HKSortingContext
        {
            get
            {
                return new Needs.Ccs.Services.Models.HKSortingContext();
            }
        }

        /// <summary>
        /// 分拣结果
        /// </summary>
        public Needs.Ccs.Services.Views.SortingsView Sorting
        {
            get
            {
                return new Needs.Ccs.Services.Views.SortingsView();
            }
        }

        /// <summary>
        /// 香港分拣结果
        /// </summary>
        public Needs.Ccs.Services.Views.HKSortingsView HKSorting
        {
            get
            {
                return new Needs.Ccs.Services.Views.HKSortingsView();
            }
        }

        /// <summary>
        /// 深圳分拣结果
        /// </summary>
        public Needs.Ccs.Services.Views.SZSortingsView SZSorting
        {
            get
            {
                return new Needs.Ccs.Services.Views.SZSortingsView();
            }
        }

        /// <summary>
        /// 香港出口清关
        /// </summary>
        public Needs.Ccs.Services.Views.OutputWayBillView OutputWayBill
        {
            get
            {
                return new Needs.Ccs.Services.Views.OutputWayBillView();
            }
        }

        /// <summary>
        /// 库存库
        /// </summary>
        public Needs.Ccs.Services.Views.StoreStorageView StoreStorage
        {
            get
            {
                return new Needs.Ccs.Services.Views.StoreStorageView();
            }
        }

        /// <summary>
        /// 香港库存库
        /// </summary>
        public Needs.Ccs.Services.Views.HKStoreStorageView HKStoreStorage
        {
            get
            {
                return new Needs.Ccs.Services.Views.HKStoreStorageView();
            }
        }

        /// <summary>
        /// 深圳库存库
        /// </summary>
        public Needs.Ccs.Services.Views.SZStoreStorageView SZStoreStorage
        {
            get
            {
                return new Needs.Ccs.Services.Views.SZStoreStorageView();
            }
        }

        /// <summary>
        /// 深圳库房进项
        /// </summary>
        public Ccs.Services.Views.SZInputView SZInput
        {
            get
            {
                return new Ccs.Services.Views.SZInputView();
            }
        }

        /// <summary>
        /// 深圳库房销项
        /// </summary>
        public Ccs.Services.Views.SZOutputView SZOutput
        {
            get
            {
                return new Ccs.Services.Views.SZOutputView();
            }
        }

        /// <summary>
        /// 快递公司
        /// </summary>
        public Ccs.Services.Views.ExpressCompanyView ExpressCompanies
        {
            get
            {
                return new Ccs.Services.Views.ExpressCompanyView();
            }
        }

        /// <summary>
        /// 快递方式
        /// </summary>
        public Ccs.Services.Views.ExpressTypeView ExpressTypes
        {
            get
            {
                return new Ccs.Services.Views.ExpressTypeView();
            }
        }

        /// <summary>
        /// 打印出库单项目(产品)
        /// </summary>
        public Ccs.Services.Views.ExitPrintItemsView ExitPrintItems
        {
            get
            {
                return new Ccs.Services.Views.ExitPrintItemsView();
            }
        }

        /// <summary>
        /// 深证库房 上架操作视图(原 待入库)
        /// </summary>
        public Ccs.Services.Views.SZOnStockView SZOnStockView
        {
            get
            {
                return new Ccs.Services.Views.SZOnStockView();
            }
        }

        /// <summary>
        /// 香港库房入库 已装箱视图
        /// </summary>
        public Ccs.Services.Views.HKPackedBoxView HKPackedBoxView
        {
            get
            {
                return new Ccs.Services.Views.HKPackedBoxView();
            }
        }

        /// <summary>
        /// 香港库房 查看订单列表（外单）
        /// </summary>
        public Ccs.Services.Views.HKOrderListView HKOrderListView
        {
            get
            {
                return new Ccs.Services.Views.HKOrderListView();
            }
        }

        /// <summary>
        /// 香港库房报表-报关订单
        /// </summary>
        public Ccs.Services.Views.Alls.HKOrdersAll HKOrdersAll
        {
            get
            {
                return new Ccs.Services.Views.Alls.HKOrdersAll();
            }
        }

        /// <summary>
        /// 入库通知日志
        /// </summary>
        public Needs.Ccs.Services.Views.Alls.EntryNoticeLogsAll EntryNoticeLogs
        {
            get
            {
                return new Ccs.Services.Views.Alls.EntryNoticeLogsAll();
            }
        }

        /// <summary>
        /// ExitNoticeFiles 表视图
        /// </summary>
        public Needs.Wl.Models.Views.ExitNoticeFilesView ExitNoticeFilesView
        {
            get
            {
                return new Needs.Wl.Models.Views.ExitNoticeFilesView();
            }
        }

        public Needs.Ccs.Services.Views.Origins.ExitNoticesOrigin ExitNoticesOrigin
        {
            get
            {
                return new Needs.Ccs.Services.Views.Origins.ExitNoticesOrigin();
            }
        }
    }
}