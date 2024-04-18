namespace Yahv.Models
{
    /// <summary>
    /// Erp 管理员的系统领域
    /// </summary>
    partial class ErpAdmin
    {
        /// <summary>
        /// 询报价领域
        /// </summary>
        public Yahv.Systematic.RFQ RFQ
        {
            get
            {
                return new Systematic.RFQ(this);
            }
        }

        /// <summary>
        /// Crm 领域
        /// </summary>
        public Systematic.Crm Crm
        {
            get
            {
                return new Systematic.Crm(this);
            }
        }
        /// <summary>
        /// Crm 领域
        /// </summary>
        public Systematic.Srm Srm
        {
            get
            {
                return new Systematic.Srm(this);
            }
        }
        /// <summary>
        /// Whs 领域(代仓储客户)
        /// </summary>
        public Systematic.Whs Whs
        {
            get
            {
                return new Systematic.Whs(this);
            }
        }
        /// <summary>
        /// 系统平台 领域
        /// </summary>
        public Systematic.Plat Plat
        {
            get
            {
                return new Systematic.Plat(this);
            }
        }

        /// <summary>
        /// Erm 领域
        /// </summary>
        public Systematic.Erm Erm
        {
            get
            {
                return new Systematic.Erm(this);
            }
        }

        public Systematic.WsOrder WsOrder
        {
            get
            {
                return new Systematic.WsOrder(this);
            }
        }

        /// <summary>
        /// 中心数据 领域
        /// </summary>
        public Systematic.PvData PvData
        {
            get
            {
                return new Systematic.PvData(this);
            }
        }

        /// <summary>
        /// 财务 领域
        /// </summary>
        public Systematic.Pays Pays
        {
            get
            {
                return new Systematic.Pays(this);
            }
        }



        /// <summary>
        /// WareHourse领域
        /// </summary>
        public Systematic.WareHourse WareHourse
        {
            get
            {
                return new Systematic.WareHourse(this);
            }
        }





        public CenterLog OperatingLog
        {
            get
            {
                return new CenterLog(this);
            }
        }

        /// <summary>
        /// 中心财务 领域
        /// </summary>
        public Systematic.Finance Finance
        {
            get
            {
                return new Systematic.Finance(this);
            }
        }

        /// <summary>
        /// 新CRM
        /// </summary>
        public Systematic.CrmPlus CrmPlus
        {
            get
            {
                return new Systematic.CrmPlus(this);
            }
        }

        /// <summary>
        /// 物流管理
        /// </summary>
        public Systematic.PvRoute PvRoute
        {
            get
            {
                return new Systematic.PvRoute(this);
            }
        }
    }

}
