using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Erps;
using Yahv.Underly.Logs;

namespace Yahv.Systematic
{
    /// <summary>
    /// 中心数据领域
    /// </summary>
    public class PvData : IAction
    {
        private IErpAdmin admin;
        public PvData(IErpAdmin admin)
        {
            this.admin = admin;
        }

        #region Views

        /// <summary>
        /// 海关税则归类信息视图
        /// </summary>
        public YaHv.PvData.Services.Views.Alls.ClassifiedPartNumbersAll ClassifiedPartNumbers
        {
            get
            {
                return new YaHv.PvData.Services.Views.Alls.ClassifiedPartNumbersAll();
            }
        }

        /// <summary>
        /// 产品归类特殊类型的视图
        /// </summary>
        public YaHv.PvData.Services.Views.Alls.OthersAll Others
        {
            get
            {
                return new YaHv.PvData.Services.Views.Alls.OthersAll();
            }
        }

        /// <summary>
        /// 归类历史数据的视图
        /// </summary>
        public YaHv.PvData.Services.Views.Alls.ClassifiedHistoriesAll ClassifiedHistories
        {
            get
            {
                return new YaHv.PvData.Services.Views.Alls.ClassifiedHistoriesAll();
            }
        }

        /// <summary>
        /// 海关税则视图
        /// </summary>
        public YaHv.PvData.Services.Views.Alls.TariffsAll Tariffs
        {
            get
            {
                return new YaHv.PvData.Services.Views.Alls.TariffsAll();
            }
        }

        /// <summary>
        /// 产地加征关税的视图
        /// </summary>
        public YaHv.PvData.Services.Views.Alls.OriginsATRateAll OriginsATRate
        {
            get
            {
                return new YaHv.PvData.Services.Views.Alls.OriginsATRateAll();
            }
        }

        /// <summary>
        /// 产地消毒/检疫的视图
        /// </summary>
        public YaHv.PvData.Services.Views.Alls.OriginsDisinfectionAll OriginsDisinfection
        {
            get
            {
                return new YaHv.PvData.Services.Views.Alls.OriginsDisinfectionAll();
            }
        }

        /// <summary>
        /// 汇率的视图
        /// </summary>
        public YaHv.PvData.Services.Views.Alls.ExchangeRatesAll ExchangeRates
        {
            get
            {
                return new YaHv.PvData.Services.Views.Alls.ExchangeRatesAll();
            }
        }

        /// <summary>
        /// 禁运信息查询的视图
        /// </summary>
        public YaHv.PvData.Services.Views.Alls.EmbargoInfosAll EmbargoInfos
        {
            get
            {
                return new YaHv.PvData.Services.Views.Alls.EmbargoInfosAll();
            }
        }

        /// <summary>
        /// Ccc信息查询的视图
        /// </summary>
        public YaHv.PvData.Services.Views.Alls.CccInfosAll CccInfos
        {
            get
            {
                return new YaHv.PvData.Services.Views.Alls.CccInfosAll();
            }
        }

        /// <summary>
        /// 海关卡控
        /// </summary>
        public YaHv.PvData.Services.Views.Alls.CustomsControlsAll CustomsControls
        {
            get
            {
                return new YaHv.PvData.Services.Views.Alls.CustomsControlsAll();
            }
        }

        /// <summary>
        /// 申报要素品牌视图
        /// </summary>
        public YaHv.PvData.Services.Views.Alls.ElementsManufacturersAll ElementsManufacturers
        {
            get
            {
                return new YaHv.PvData.Services.Views.Alls.ElementsManufacturersAll();
            }
        }

        /// <summary>
        /// Eccn编码视图
        /// </summary>
        public YaHv.PvData.Services.Views.Alls.EccnsAll Eccns
        {
            get
            {
                return new YaHv.PvData.Services.Views.Alls.EccnsAll();
            }
        }

        /// <summary>
        /// 标准型号视图
        /// </summary>
        public YaHv.PvData.Services.Views.Alls.StandardPartnumbersForPlugin StandardPartnumbersForPlugins
        {
            get
            {
                return new YaHv.PvData.Services.Views.Alls.StandardPartnumbersForPlugin();
            }
        }


        /// <summary>
        /// 香港工贸署管制查询
        /// </summary>
        public YaHv.PvData.Services.Views.Alls.HKControlsAll HKControls
        {
            get
            {
                return new YaHv.PvData.Services.Views.Alls.HKControlsAll();

            }

        }
        #endregion

        #region Action

        public void Logs_Error(Logs_Error log)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
