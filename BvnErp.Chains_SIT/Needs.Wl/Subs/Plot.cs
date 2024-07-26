using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Admin.Plat
{
    /// <summary>
    /// Admin 入口
    /// </summary>
    public sealed partial class AdminPlat
    {
        /// <summary>
        /// 华芯通平台客户
        /// </summary>
        public static Needs.Ccs.Services.Views.ClientsView Clients
        {
            get
            {
                return new Needs.Ccs.Services.Views.ClientsView();
            }
        }

        /// <summary>
        /// 系统管理员
        /// </summary>
        public static Needs.Wl.Views.Admins Admins
        {
            get
            {
                return new Needs.Wl.Views.Admins();
            }
        }

        /// <summary>
        /// 基础数据-国家与地区
        /// </summary>
        public static Needs.Ccs.Services.Views.BaseCountriesView Countries
        {
            get
            {
                return new Needs.Ccs.Services.Views.BaseCountriesView();
            }
        }

        /// <summary>
        /// 基础数据-单位
        /// </summary>
        public static Needs.Ccs.Services.Views.BaseUnitsView Units
        {
            get
            {
                return new Needs.Ccs.Services.Views.BaseUnitsView();
            }
        }

        /// <summary>
        /// 基础数据-港口
        /// </summary>
        public static Needs.Ccs.Services.Views.BasePortsView Ports
        {
            get
            {
                return new Needs.Ccs.Services.Views.BasePortsView();
            }
        }

        /// <summary>
        /// 基础数据-许可证类型
        /// </summary>
        public static Needs.Ccs.Services.Views.BaseGoodsLimitView GoodsLimits
        {
            get
            {
                return new Needs.Ccs.Services.Views.BaseGoodsLimitView();
            }
        }

        /// <summary>
        /// 基础数据-包装种类
        /// </summary>
        public static Needs.Ccs.Services.Views.BaseWrapTypesView BaseWrapTypesView
        {
            get
            {
                return new Needs.Ccs.Services.Views.BaseWrapTypesView();
            }
        }

        /// <summary>
        /// 基础数据-币种
        /// </summary>
        public static Needs.Ccs.Services.Views.BaseCurrenciesView Currencies
        {
            get
            {
                return new Needs.Ccs.Services.Views.BaseCurrenciesView();
            }
        }

        /// <summary>
        /// 基础数据-币种的海关汇率
        /// </summary>
        public static Needs.Ccs.Services.Views.CustomExchangeRatesView CustomRates
        {
            get
            {
                return new Needs.Ccs.Services.Views.CustomExchangeRatesView();
            }
        }

        /// <summary>
        /// 基础数据-币种的实时汇率
        /// </summary>
        public static Needs.Ccs.Services.Views.RealTimeExchangeRatesView RealTimeRates
        {
            get
            {
                return new Needs.Ccs.Services.Views.RealTimeExchangeRatesView();
            }
        }

        /// <summary>
        /// 基础数据-币种的实时汇率
        /// </summary>
        public static Needs.Ccs.Services.Views.ZGExchangeRatesView ZGRates
        {
            get
            {
                return new Needs.Ccs.Services.Views.ZGExchangeRatesView();
            }
        }

        /// <summary>
        /// 基础数据-管控产品
        /// </summary>
        public static Needs.Ccs.Services.Views.ProductControlAllsView ProductControls
        {
            get
            {
                return new Needs.Ccs.Services.Views.ProductControlAllsView();
            }
        }
        /// <summary>
        /// 基础数据-海关分类产品
        /// </summary>
        public static Needs.Ccs.Services.Views.ProductCategoriesAllsView ProductCategories
        {
            get
            {
                return new Needs.Ccs.Services.Views.ProductCategoriesAllsView();
            }
        }
        /// <summary>
        /// 基础数据-税务分类
        /// </summary>
        public static Needs.Ccs.Services.Views.TaxCategoriesAllsView TaxCategories
        {
            get
            {
                return new Needs.Ccs.Services.Views.TaxCategoriesAllsView();
            }
        }
        /// <summary>
        /// 基础数据-产品税务分类
        /// </summary>
        public static Needs.Ccs.Services.Views.MyProductTaxCategoriesView ProductTaxCategories
        {
            get
            {
                return new Needs.Ccs.Services.Views.MyProductTaxCategoriesView();
            }
        }
        /// <summary>
        /// 引荐人设置
        /// </summary>
        public static Needs.Ccs.Services.Views.Origins.ReferrersOrigin Referrers
        {
            get
            {
                return new Needs.Ccs.Services.Views.Origins.ReferrersOrigin();
            }
        }

        #region 报关基础数据

        /// <summary>
        /// 基础数据-检验检疫机关
        /// </summary>
        public static Needs.Ccs.Services.Views.BaseOrgCodesView BaseOrgCodes
        {
            get
            {
                return new Ccs.Services.Views.BaseOrgCodesView();
            }
        }

        /// <summary>
        /// 基础数据-集装箱规格
        /// </summary>
        public static Needs.Ccs.Services.Views.BaseContainersView BaseContainers
        {
            get
            {
                return new Ccs.Services.Views.BaseContainersView();
            }
        }

        /// <summary>
        /// 基础数据-关联理由
        /// </summary>
        public static Needs.Ccs.Services.Views.BaseCorrelationReasonView BaseCorrelationReason
        {
            get
            {
                return new Ccs.Services.Views.BaseCorrelationReasonView();
            }
        }

        /// <summary>
        /// 基础数据-海关回执代码
        /// </summary>
        public static Needs.Ccs.Services.Views.BaseCusReceiptCodesView BaseCusReceiptCode
        {
            get
            {
                return new Ccs.Services.Views.BaseCusReceiptCodesView();
            }
        }

        /// <summary>
        /// 基础数据-申报地海关
        /// </summary>
        public static Needs.Ccs.Services.Views.BaseCustomMasterView BaseCustomMaster
        {
            get
            {
                return new Ccs.Services.Views.BaseCustomMasterView();
            }
        }



        /// <summary>
        /// 基础数据-申报地海关默认关联
        /// </summary>
        public static Needs.Ccs.Services.Views.BaseCustomMasterDefaultView BaseCustomMasterDefault
        {
            get
            {
                return new Ccs.Services.Views.BaseCustomMasterDefaultView();
            }
        }

        /// <summary>
        /// 基础数据-征免性质代码
        /// </summary>
        public static Needs.Ccs.Services.Views.BaseCutModeView BaseCutMode
        {
            get
            {
                return new Ccs.Services.Views.BaseCutModeView();
            }
        }

        /// <summary>
        /// 基础数据-境内目的地
        /// </summary>
        public static Needs.Ccs.Services.Views.BaseDistrictCodeView BaseDistrictCode
        {
            get
            {
                return new Ccs.Services.Views.BaseDistrictCodeView();
            }
        }

        /// <summary>
        /// 基础数据-监管证件代码
        /// </summary>
        public static Needs.Ccs.Services.Views.BaseDocuCodesView BaseDocuCode
        {
            get
            {
                return new Ccs.Services.Views.BaseDocuCodesView();
            }
        }

        /// <summary>
        /// 基础数据-征减免税方式 
        /// </summary>
        public static Needs.Ccs.Services.Views.BaseDutyModeView BaseDutyMode
        {
            get
            {
                return new Ccs.Services.Views.BaseDutyModeView();
            }
        }

        /// <summary>
        /// 基础数据-随附单证代码
        /// </summary>
        public static Needs.Ccs.Services.Views.BaseEdocCodesView BaseEdocCode
        {
            get
            {
                return new Ccs.Services.Views.BaseEdocCodesView();
            }
        }

        /// <summary>
        /// 基础数据-国内口岸代码
        /// </summary>
        public static Needs.Ccs.Services.Views.BaseEntryPortsView BaseEntryPort
        {
            get
            {
                return new Ccs.Services.Views.BaseEntryPortsView();
            }
        }

        /// <summary>
        /// 基础数据-货物属性代码
        /// </summary>
        public static Needs.Ccs.Services.Views.BaseGoodsAttrsView BaseGoodsAttr
        {
            get
            {
                return new Ccs.Services.Views.BaseGoodsAttrsView();
            }
        }

        /// <summary>
        /// 基础数据-海关通关代码
        /// </summary>
        public static Needs.Ccs.Services.Views.BaseGovProceduresView BaseGovProcedure
        {
            get
            {
                return new Ccs.Services.Views.BaseGovProceduresView();
            }
        }

        /// <summary>
        /// 基础数据-包装种类代码  舱单用
        /// </summary>
        public static Needs.Ccs.Services.Views.BasePackTypesView BasePackType
        {
            get
            {
                return new Ccs.Services.Views.BasePackTypesView();
            }
        }

        /// <summary>
        /// 基础数据-港口代码
        /// </summary>
        public static Needs.Ccs.Services.Views.BasePortsView BasePort
        {
            get
            {
                return new Ccs.Services.Views.BasePortsView();
            }
        }

        /// <summary>
        /// 基础数据-用途代码
        /// </summary>
        public static Needs.Ccs.Services.Views.BasePurposesView BasePurpose
        {
            get
            {
                return new Ccs.Services.Views.BasePurposesView();
            }
        }

        /// <summary>
        /// 基础数据-监管方式代码
        /// </summary>
        public static Needs.Ccs.Services.Views.BaseTradeModesView BaseTradeMode
        {
            get
            {
                return new Ccs.Services.Views.BaseTradeModesView();
            }
        }

        /// <summary>
        /// 基础数据-运输方式代码
        /// </summary>
        public static Needs.Ccs.Services.Views.BaseTrafModesView BaseTrafMode
        {
            get
            {
                return new Ccs.Services.Views.BaseTrafModesView();
            }
        }

        /// <summary>
        /// 基础数据-成交方式代码
        /// </summary>
        public static Needs.Ccs.Services.Views.BaseTransModesView BaseTransMode
        {
            get
            {
                return new Ccs.Services.Views.BaseTransModesView();
            }
        }

        /// <summary>
        /// 基础数据-申请单证代码
        /// </summary>
        public static Needs.Ccs.Services.Views.BaseAppCertCodeView BaseAppCertCode
        {
            get
            {
                return new Ccs.Services.Views.BaseAppCertCodeView();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static Needs.Ccs.Services.Views.BaseOriginAreaView BaseOriginArea
        {
            get
            {
                return new Ccs.Services.Views.BaseOriginAreaView();
            }
        }

        public static Needs.Ccs.Services.Views.BaseDestCodeView BaseDestCode
        {
            get
            {
                return new Ccs.Services.Views.BaseDestCodeView();
            }
        }


        #endregion
    }
}
