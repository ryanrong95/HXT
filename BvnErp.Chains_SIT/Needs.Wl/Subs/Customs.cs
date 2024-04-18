using Needs.Erp.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Admin.Plat.Models
{
    public partial class Admin
    {
        public Customs Customs
        {
            get
            {
                return new Customs(this);
            }
        }
    }

    /// <summary>
    /// 报关入口
    /// </summary>
    public class Customs
    {
        IGenericAdmin Admin;

        public Customs(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        /// <summary>
        /// Admin
        /// </summary>
        public Needs.Ccs.Services.Views.AdminsTopView Admins
        {
            get
            {
                return new Needs.Ccs.Services.Views.AdminsTopView();
            }
        }

        /// <summary>
        /// 承运商
        /// </summary>
        public Needs.Ccs.Services.Views.CarriersView Carriers
        {
            get
            {
                return new Needs.Ccs.Services.Views.CarriersView();
            }
        }

        /// <summary>
        /// 承运商 New
        /// </summary>
        public Needs.Wl.Models.Views.CarriersView CarriersNew
        {
            get
            {
                return new Needs.Wl.Models.Views.CarriersView();
            }
        }

        /// <summary>
        /// 承运商 联系人 New
        /// </summary>
        public Needs.Wl.Models.Views.ContactsView ContactsNew
        {
            get
            {
                return new Needs.Wl.Models.Views.ContactsView();
            }
        }

        /// <summary>
        /// 车辆
        /// </summary>
        public Needs.Ccs.Services.Views.VehicleView Vehicles
        {
            get
            {
                return new Needs.Ccs.Services.Views.VehicleView();
            }
        }

        /// <summary>
        /// 车辆 New
        /// </summary>
        public Needs.Wl.Models.Views.VehiclesView VehiclesNew
        {
            get
            {
                return new Needs.Wl.Models.Views.VehiclesView();
            }
        }

        /// <summary>
        /// 驾驶员
        /// </summary>
        public Needs.Ccs.Services.Views.DriverView Drivers
        {
            get
            {
                return new Needs.Ccs.Services.Views.DriverView();
            }
        }

        /// <summary>
        /// 驾驶员 New
        /// </summary>
        public Needs.Wl.Models.Views.DriversView DriversNew
        {
            get
            {
                return new Needs.Wl.Models.Views.DriversView();
            }
        }

        /// <summary>
        /// 公司
        /// </summary>
        public Needs.Ccs.Services.Views.CompaniesView Companies
        {
            get
            {
                return new Needs.Ccs.Services.Views.CompaniesView();
            }
        }

        /// <summary>
        /// 客户
        /// </summary>
        public Needs.Ccs.Services.Views.ClientsView Clients
        {
            get
            {
                return new Needs.Ccs.Services.Views.ClientsView();
            }
        }

        /// <summary>
        /// 收件地址
        /// </summary>
        public Needs.Ccs.Services.Views.ClientConsigneesView ClientConsignees
        {
            get
            {
                return new Needs.Ccs.Services.Views.ClientConsigneesView();
            }
        }

        /// <summary>
        /// 供应商信息
        /// </summary>
        public Needs.Ccs.Services.Views.ClientSuppliersView ClientSuppliers
        {
            get
            {
                return new Needs.Ccs.Services.Views.ClientSuppliersView();
            }
        }

        /// <summary>
        /// 供应商地址信息
        /// </summary>
        public Needs.Ccs.Services.Views.ClientSupplierAddressesView ClientSupplierAddresses
        {
            get
            {
                return new Needs.Ccs.Services.Views.ClientSupplierAddressesView();
            }
        }

        /// <summary>
        /// 供应商账户信息
        /// </summary>
        public Needs.Ccs.Services.Views.ClientSupplierBanksView ClientSupplierBanks
        {
            get
            {
                return new Needs.Ccs.Services.Views.ClientSupplierBanksView();
            }
        }

        /// <summary>
        /// 供应商账户信息（通过供应商名称查询）
        /// </summary>
        public Needs.Ccs.Services.Views.ClientSupplierBanksByNameView ClientSupplierBanksByName
        {
            get
            {
                return new Needs.Ccs.Services.Views.ClientSupplierBanksByNameView();
            }
        }

        public Needs.Ccs.Services.Views.ClientFilesView ClientFiles
        {
            get
            {
                return new Needs.Ccs.Services.Views.ClientFilesView();
            }
        }

        /// <summary>
        /// 税则
        /// </summary>
        public Needs.Ccs.Services.Views.CustomsTariffsView CustomsTariffs
        {
            get
            {
                return new Needs.Ccs.Services.Views.CustomsTariffsView();
            }
        }

        /// <summary>
        /// 原产地税则
        /// </summary>
        public Needs.Ccs.Services.Views.OriginTariffsView OriginTariffs
        {
            get
            {
                return new Needs.Ccs.Services.Views.OriginTariffsView();
            }
        }

        public Needs.Ccs.Services.Views.CustomsQuarantinesView CustomsQuarantines
        {
            get
            {
                return new Needs.Ccs.Services.Views.CustomsQuarantinesView();
            }
        }

        public Needs.Ccs.Services.Views.CustomsCiqCodeView CustomsCiqCodes
        {
            get
            {
                return new Needs.Ccs.Services.Views.CustomsCiqCodeView();
            }
        }

        /// <summary>
        /// 检验检疫机构
        /// </summary>
        public Needs.Ccs.Services.Views.BaseCIQsView BaseCIQs
        {
            get
            {
                return new Needs.Ccs.Services.Views.BaseCIQsView();
            }
        }

        /// <summary>
        /// 国内口岸
        /// </summary>
        public Needs.Ccs.Services.Views.BaseDomesticPortView BaseDomesticPorts
        {
            get
            {
                return new Needs.Ccs.Services.Views.BaseDomesticPortView();
            }
        }

        /// <summary>
        /// 成交方式
        /// </summary>
        public Needs.Ccs.Services.Views.BaseTradeView BaseTrade
        {
            get
            {
                return new Needs.Ccs.Services.Views.BaseTradeView();
            }
        }

        /// <summary>
        /// 征免方式
        /// </summary>
        public Needs.Ccs.Services.Views.BaseLeviesView BaseLevy
        {
            get
            {
                return new Needs.Ccs.Services.Views.BaseLeviesView();
            }
        }

        /// <summary>
        /// 监管方式
        /// </summary>
        public Needs.Ccs.Services.Views.BaseMonitorWaysView BaseMonitorWays
        {
            get
            {
                return new Needs.Ccs.Services.Views.BaseMonitorWaysView();
            }
        }

        /// <summary>
        /// 集装箱规格
        /// </summary>
        public Needs.Ccs.Services.Views.BaseContainersView BaseContainers
        {
            get
            {
                return new Needs.Ccs.Services.Views.BaseContainersView();
            }
        }

        /// <summary>
        /// 关区代码
        /// </summary>
        public Needs.Ccs.Services.Views.BaseCustomsView BaseCustoms
        {
            get
            {
                return new Needs.Ccs.Services.Views.BaseCustomsView();
            }
        }

        /// <summary>
        /// 运输方式
        /// </summary>
        public Needs.Ccs.Services.Views.BaseTrafsView BaseTrafs
        {
            get
            {
                return new Needs.Ccs.Services.Views.BaseTrafsView();
            }
        }

       
        ///// <summary>
        ///// 标准产品
        ///// </summary>
        //public Needs.Ccs.Services.Views.ProductsViews Products
        //{
        //    get
        //    {
        //        return new Needs.Ccs.Services.Views.ProductsViews();
        //    }
        //}

        /// <summary>
        /// 报关单表头
        /// </summary>
        public Needs.Ccs.Services.Views.DecHeadsView DecHeads
        {
            get
            {
                return new Needs.Ccs.Services.Views.DecHeadsView();
            }
        }

        /// <summary>
        /// 报关单-电子随附单据
        /// </summary>
        public Needs.Ccs.Services.Views.EdocRealationsView EdocRealations
        {
            get
            {
                return new Ccs.Services.Views.EdocRealationsView();
            }
        }

        /// <summary>
        /// 舱单列表
        /// </summary>
        public Needs.Ccs.Services.Views.ManifestConsignmentListView ManifestConsignmentLists
        {
            get
            {
                return new Needs.Ccs.Services.Views.ManifestConsignmentListView();
            }
        }

        /// <summary>
        /// 舱单表头
        /// </summary>
        public Needs.Ccs.Services.Views.ManifestsView Manifests
        {
            get
            {
                return new Needs.Ccs.Services.Views.ManifestsView();
            }
        }

        /// <summary>
        /// 用提运单信息作为新的舱单
        /// </summary>
        public Needs.Ccs.Services.Views.ManifestConsignmentsView ManifestsNew
        {
            get
            {
                return new Needs.Ccs.Services.Views.ManifestConsignmentsView();
            }
        }


        /// <summary>
        /// 回执信息
        /// </summary>
        public Needs.Ccs.Services.Views.ManifestConsignmentTracesView ManifestConsignmentTraces
        {
            get
            {
                return new Needs.Ccs.Services.Views.ManifestConsignmentTracesView();
            }
        }

        /// <summary>
        /// 舱单(运单)
        /// </summary>
        public Needs.Ccs.Services.Views.ManifestConsignmentsView ManifestConsignments
        {
            get
            {
                return new Needs.Ccs.Services.Views.ManifestConsignmentsView();
            }
        }

        /// <summary>
        /// 舱单申报信息
        /// </summary>
        public Needs.Ccs.Services.Views.ManifestConsignmentInfosView ManifestConsignmentInfos
        {
            get
            {
                return new Needs.Ccs.Services.Views.ManifestConsignmentInfosView();
            }
        }

        /// <summary>
        /// 运输批次信息 New
        /// </summary>
        public Needs.Wl.Models.Views.VoyagesView VoyagesNew
        {
            get
            {
                return new Needs.Wl.Models.Views.VoyagesView();
            }
        }

        /// <summary>
        /// 报关单附件
        /// </summary>
        public Needs.Ccs.Services.Views.DecFileView DecFileView
        {
            get
            {
                return new Ccs.Services.Views.DecFileView();
            }
        }

        /// <summary>
        /// 口岸默认设置
        /// </summary>
        public Needs.Ccs.Services.Views.BaseCustomMasterDefaultView MasterDefault
        {
            get
            {
                return new Needs.Ccs.Services.Views.BaseCustomMasterDefaultView();
            }
        }

    }
}