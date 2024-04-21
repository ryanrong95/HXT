using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms.Services.Models;
using Yahv.Linq;
using Yahv.Underly;

namespace Wms.Services.chonggous.Views
{

    public class Carrier : Yahv.Linq.IUnique
    {
        #region 属性
        string id;
        /// <summary>
        /// 唯一码
        /// </summary>
        /// <chenhan>保障全局唯一</chenhan>
        public string ID
        {
            get; set;
        }
        /// <summary>
        /// 承运商代码
        /// </summary>
        public string Code { get; set; }


        public CarrierType Type { get; set; }

        /// <summary>
        /// 快递的图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        public string Place { get; set; }

        /// <summary>
        /// 是否国际供应商
        /// </summary>
        public bool IsInternational { get; set; }

        #endregion


        static public Carrier XdtPCL;
        static object locker = new object();

        /// <summary>
        /// 静态构造器
        /// </summary>
        /// <remarks>
        /// 添加了初始化芯达通物流部承运商的逻辑
        /// </remarks>
        static Carrier()
        {
            if (XdtPCL == null)
            {
                lock (locker)
                {
                    if (XdtPCL == null)
                    {
                        string name = "芯达通物流部";
                        using (var reponsitory = new PvbCrmReponsitory())
                        {
                            try
                            {
                                if (!reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Enterprises>().Any(item => item.ID == nameof(XdtPCL)))
                                {
                                    reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.Enterprises
                                    {
                                        ID = nameof(XdtPCL),
                                        Name = name,
                                        AdminCode = nameof(XdtPCL),
                                        Status = (int)ApprovalStatus.Normal,
                                        District = null,
                                        Corporation = null,
                                        Uscc = null,
                                        RegAddress = null,
                                        Place = nameof(Origin.CHN),
                                    });
                                }
                                if (!reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Carriers>().Any(item => item.ID == nameof(XdtPCL)))
                                {
                                    reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.Carriers
                                    {
                                        ID = nameof(XdtPCL),
                                        Code = nameof(XdtPCL),
                                        CreateDate = DateTime.Now,
                                        Creator = Npc.Robot.Obtain(),
                                        Icon = "",
                                        Status = (int)GeneralStatus.Normal,
                                        Summary = "机器人创建",
                                        UpdateDate = DateTime.Now,
                                        Type = (int)CarrierType.Logistics
                                    });
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                            finally
                            {
                                using (var view = new CarriersTopView())
                                {
                                    XdtPCL = view.SingleOrDefault(item => item.ID == nameof(XdtPCL));
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 构造器
        /// </summary>
        public Carrier()
        {

        }

    }

    public class CarriersTopView : QueryView<Carrier, PvWmsRepository>
    {
        public CarriersTopView()
        {

        }
        protected override IQueryable<Carrier> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.CarriersTopView>()
                   where entity.Status == (int)GeneralStatus.Normal
                   select new Carrier
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       Code = entity.Code,
                       Place = entity.Place,
                       Type = (CarrierType)entity.Type,
                       IsInternational = entity.IsInternational,
                   };
        }
    }
}
