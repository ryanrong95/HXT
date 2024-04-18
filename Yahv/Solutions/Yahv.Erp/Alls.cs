using Layers.Data.Sqls;
using Yahv.Services;

namespace Yahv
{
    /// <summary>
    /// 全部的
    /// </summary>
    public class Alls
    {
        /// <summary>
        /// Admin列表
        /// </summary>
        /// <remarks>Erp专用</remarks>
        public Erm.Services.Views.AdminsAll Admins
        {
            get
            {
                return new Erm.Services.Views.AdminsAll();
            }
        }

        public Yahv.Erm.Services.Views.MenusAll Menus
        {
            get
            {
                return new Yahv.Erm.Services.Views.MenusAll();
            }
        }

        public Yahv.Erm.Services.Views.RolesAll Roles
        {

            get
            {
                return new Yahv.Erm.Services.Views.RolesAll();
            }
        }

        public Yahv.Erm.Services.Views.WageItemAlls WageItems
        {
            get
            {
                return new Erm.Services.Views.WageItemAlls();
            }
        }

        public Yahv.Erm.Services.Views.PersonalRatesRoll PersonalRates
        {
            get
            {
                return new Yahv.Erm.Services.Views.PersonalRatesRoll();
            }
        }

        public Yahv.Erm.Services.Views.PastsWageItemAlls PastsWageItems
        {
            get
            {
                return new Erm.Services.Views.PastsWageItemAlls();
            }
        }

        public Yahv.Erm.Services.Views.StaffAlls Staffs
        {
            get
            {
                return new Erm.Services.Views.StaffAlls();
            }
        }

        public Erm.Services.Origins.Views.ParticlesAll Particles
        {
            get { return new Erm.Services.Origins.Views.ParticlesAll(); }
        }

        public Erm.Services.Views.PostionsAll Postions
        {
            get { return new Erm.Services.Views.PostionsAll(); }
        }

        public Erm.Services.Views.Rolls.LeaguesRoll LeaguesRolls
        {
            get { return new Erm.Services.Views.Rolls.LeaguesRoll(); }
        }

        public Erm.Services.Views.Rolls.PersonalsRoll Personals
        {
            get { return new Erm.Services.Views.Rolls.PersonalsRoll(); }
        }

        public Erm.Services.Views.Rolls.MypersonalsRoll Mypersonals
        {
            get { return new Erm.Services.Views.Rolls.MypersonalsRoll(); }
        }

        public Yahv.Services.Views.CompaniesTopView<PvbCrmReponsitory> Companies
        {
            get { return new Yahv.Services.Views.CompaniesTopView<PvbCrmReponsitory>(); }
        }

        public Erm.Services.Views.Rolls.LaboursRoll Labours
        {
            get { return new Erm.Services.Views.Rolls.LaboursRoll(); }
        }

        public Erm.Services.Views.Rolls.BankCardsRoll BankCards
        {
            get { return new Erm.Services.Views.Rolls.BankCardsRoll(); }
        }

        public Erm.Services.Views.Rolls.MyWageItemsRoll MyWageItems
        {
            get { return new Erm.Services.Views.Rolls.MyWageItemsRoll(); }
        }

        public Erm.Services.Views.Rolls.PayBillsRoll PayBills
        {
            get { return new Erm.Services.Views.Rolls.PayBillsRoll(); }
        }

        public Erm.Services.Views.Rolls.StaffPayItems PayItems
        {
            get { return new Erm.Services.Views.Rolls.StaffPayItems(); }
        }

        public Erm.Services.Views.WarehousePlatesTopView Warehouses
        {
            get { return new Erm.Services.Views.WarehousePlatesTopView(); }
        }

        /// <summary>
        /// 库房管理员
        /// </summary>
        public Erm.Services.Views.Rolls.AdminsWmsRoll AdminsPfWms
        {
            get { return new Erm.Services.Views.Rolls.AdminsWmsRoll(SysBusiness.PfWms); }
        }

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
