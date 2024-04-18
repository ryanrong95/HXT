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
        public Cbs Cbs
        {
            get
            {
                return new Cbs(this);
            }
        }
    }
    public class Cbs
    {
        IGenericAdmin Admin;

        public Cbs(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        /// <summary>
        /// 汇率
        /// </summary>
        public Needs.Cbs.Services.Views.Alls.ExchangeRatesAll ExchangeRates
        {
            get
            {
                return new Needs.Cbs.Services.Views.Alls.ExchangeRatesAll();
            }
        }
        /// <summary>
        /// 海关检疫
        /// </summary>
        public Needs.Cbs.Services.Views.Alls.CustomsQuarantinesAll CustomsQuarantines
        {
            get
            {
                return new Needs.Cbs.Services.Views.Alls.CustomsQuarantinesAll();
            }
        }

        /// <summary>
        /// 海关基础数据配置
        /// </summary>
        public Needs.Cbs.Services.Views.Alls.CustomsSettingsAll CustomsSettings
        {
            get
            {
                return new Needs.Cbs.Services.Views.Alls.CustomsSettingsAll();
            }
        }

        /// <summary>
        /// 海关申报地默认关联
        /// </summary>
        public Needs.Cbs.Services.Views.Alls.CustomsMasterDefaultsAll MasterDefaults
        {
            get
            {
                return new Needs.Cbs.Services.Views.Alls.CustomsMasterDefaultsAll();
            }
        }

        /// <summary>
        /// 海关税则
        /// </summary>
        public Needs.Cbs.Services.Views.Alls.CustomsTariffsAll CustomsTariffs
        {
            get
            {
                return new Needs.Cbs.Services.Views.Alls.CustomsTariffsAll();
            }
        }

        /// <summary>
        /// 原产地税则
        /// </summary>
        public Needs.Cbs.Services.Views.Alls.CustomsOriginTariffsAll OriginTariffs
        {
            get
            {
                return new Needs.Cbs.Services.Views.Alls.CustomsOriginTariffsAll();
            }
        }
    }
}
