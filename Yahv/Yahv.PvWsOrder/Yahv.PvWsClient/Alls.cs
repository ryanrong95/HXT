using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.PvWsClient.Model;
using Yahv.PvWsOrder.Services.ClientViews;
using Yahv.Services.Models;
using Yahv.Services.Views;
using Yahv.Utils.Http;

namespace Yahv
{
    public partial class Alls
    {
        /// <summary>
        /// 承运商数据源
        /// </summary>
        public Yahv.Services.Views.CarriersTopView<PvWsOrderReponsitory> Carriers
        {
            get
            {
                return new Services.Views.CarriersTopView<PvWsOrderReponsitory>();
            }
        }

        /// <summary>
        /// 所有客户信息
        /// </summary>
        public Yahv.Services.Views.WsClientsTopView<PvWsOrderReponsitory> WsClients
        {
            get
            {
                return new Yahv.Services.Views.WsClientsTopView<PvWsOrderReponsitory>();
            }
        }

        /// <summary>
        /// 公告数据源
        /// </summary>
        public Yahv.PvWsOrder.Services.ClientViews.NoticeAlls Notices
        {
            get
            {
                return new PvWsOrder.Services.ClientViews.NoticeAlls();
            }
        }

        /// <summary>
        /// 租赁产品
        /// </summary>
        public LsProductView LsProducts
        {
            get
            {
                return new LsProductView();
            }
        }

        /// <summary>
        /// 文件视图
        /// </summary>
        public Yahv.PvWsOrder.Services.ClientViews.CenterFilesView centerFiles
        {
            get
            {
                return new PvWsOrder.Services.ClientViews.CenterFilesView();
            }
        }

        /// <summary>
        /// 平台公司收款人信息
        /// </summary>
        public wsPayee CompanyPayee
        {
            get
            {
                var companyID = Yahv.PvWsOrder.Services.PvClientConfig.CompanyID;
                return new wsPayeesTopView<PvWsOrderReponsitory>().Where(item => item.EnterpriseID == companyID).FirstOrDefault();
            }
        }

        /// <summary>
        /// 所有会员信息
        /// </summary>
        public PvWsClient.Views.UsersAlls AllUsers
        {
            get
            {
                return new PvWsClient.Views.UsersAlls();
            }
        }


        /// <summary>
        /// 芯达通币种
        /// </summary>
        public CurrenciesView Currency
        {
            get
            {
                return new CurrenciesView();
            }
        }

        /// <summary>
        /// 内部公司
        /// </summary>
        public PvWsOrder.Services.Views.CompanysAll Company
        {
            get
            {
                return new PvWsOrder.Services.Views.CompanysAll();
            }
        }

        /// <summary>
        /// 实时汇率
        /// </summary>
        public PvWsOrder.Services.XDTClientView.RealTimeExchangeRatesView RealTimeExchangeRates
        {
            get
            {
                return new PvWsOrder.Services.XDTClientView.RealTimeExchangeRatesView();
            }
        }

        /// <summary>
        /// 海关汇率
        /// </summary>
        public PvWsOrder.Services.XDTClientView.CustomExchangeRatesView CustomExchangeRates
        {
            get
            {
                return new PvWsOrder.Services.XDTClientView.CustomExchangeRatesView();
            }
        }

        /// <summary>
        /// 芯达通客户供应商银行账户
        /// </summary>
        public Yahv.PvWsOrder.Services.XDTClientView.ClientSupplierBankView XDTSupplierBank
        {
            get
            {
                return new PvWsOrder.Services.XDTClientView.ClientSupplierBankView();
            }
        }

        /// <summary>
        /// 芯达通客户供应商提货地址
        /// </summary>
        public Yahv.PvWsOrder.Services.XDTClientView.ClientSupplierAddressView XDTSupplierAddresses
        {
            get
            {
                return new PvWsOrder.Services.XDTClientView.ClientSupplierAddressView();
            }
        }

        /// <summary>
        /// 芯达通供应商
        /// </summary>
        public Yahv.PvWsOrder.Services.XDTClientView.ClientSupplierView XDTSupplier
        {
            get
            {
                return new PvWsOrder.Services.XDTClientView.ClientSupplierView();
            }
        }

        public PvWsOrder.Services.XDTClientView.PayExchangeSensitiveWordCheckView PayExchangeSensitiveWordCheck
        {
            get
            {
                return new PvWsOrder.Services.XDTClientView.PayExchangeSensitiveWordCheckView();
            }
        }

        /// <summary>
        /// 芯达通订单轨迹
        /// </summary>
        public PvWsOrder.Services.XDTClientView.OrderTracesView XDTOrderTraces
        {
            get
            {
                return new PvWsOrder.Services.XDTClientView.OrderTracesView();
            }
        }

        /// <summary>
        /// 收款人视图
        /// </summary>
        public wsPayeesTopView<PvWsOrderReponsitory> wsPayeeAll
        {
            get
            {
                return new wsPayeesTopView<PvWsOrderReponsitory>();
            }
        }


        public wsPayersTopView<PvWsOrderReponsitory> wsPayerAll
        {
            get { return new wsPayersTopView<PvWsOrderReponsitory>(); }
        }

        //芯达通发票详情试图
        public Yahv.PvWsOrder.Services.XDTClientView.XDTOrderInvoiceView XDTInvoice
        {
            get
            {
                return new PvWsOrder.Services.XDTClientView.XDTOrderInvoiceView();
            }
        }

        #region 3rm对接通用视图
        /// <summary>
        /// 所有的供应商受益人
        /// </summary>
        public wsnSupplierConsignorsTopView<PvWsOrderReponsitory> SupplierConsignors
        {
            get
            {
                return new wsnSupplierConsignorsTopView<PvWsOrderReponsitory>();
            }
        }

        /// <summary>
        /// 供应商联系人
        /// </summary>
        public wsnSupplierContactsTopView<PvWsOrderReponsitory> SupplierContacts
        {
            get
            {
                return new wsnSupplierContactsTopView<PvWsOrderReponsitory>();
            }
        }

        /// <summary>
        /// 供应商收款人
        /// </summary>
        public wsnSupplierPayeesTopView<PvWsOrderReponsitory> SupplierPayees
        {
            get
            {
                return new wsnSupplierPayeesTopView<PvWsOrderReponsitory>();
            }
        }
        #endregion


        #region 租赁订单
        /// <summary>
        /// 租赁订单项
        /// </summary>
        public LsOrderItemAlls LsOrderItems
        {
            get
            {
                return new LsOrderItemAlls();
            }
        }

        #endregion

        /// <summary>
        /// 是否记住
        /// </summary>
        public bool IsRemeber
        {
            get
            {
                var isremeber = Cookies.Current[PvWsClient.Setting.SettingsManager<IUserSetting>.Current.LoginRemeberName];
                return bool.Parse(isremeber ?? "False");
            }
        }

        /// <summary>
        /// 记住的用户名
        /// </summary>
        public string UserName
        {
            get
            {
                return Cookies.Current[PvWsClient.Setting.SettingsManager<IUserSetting>.Current.LoginUserIDName];
            }
        }
    }


    public partial class Alls
    {
        static Alls alls;
        static object locker = new object();

        /// <summary>
        /// 选择器当前示例
        /// </summary>
        static public Alls Current
        {
            get
            {
                if (alls == null)
                {
                    lock (locker)
                    {
                        if (alls == null)
                        {
                            alls = new Alls();
                        }
                    }
                }
                return alls;
            }
        }
    }
}
