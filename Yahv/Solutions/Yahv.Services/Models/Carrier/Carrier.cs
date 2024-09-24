using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using Yahv.Utils.Converters.Contents;

namespace Yahv.Services.Models
{

    /// <summary>
    /// 承运商
    /// </summary>
    public class Carrier : Yahv.Linq.IUnique
    {
        #region 属性
        /// <summary>
        /// 主键ID
        /// </summary>
        //public string ID { get; set; }
        string id;
        /// <summary>
        /// 唯一码
        /// </summary>
        /// <chenhan>保障全局唯一</chenhan>
        public string ID
        {
            get
            {
                return this.id ?? this.Name.MD5();

            }
            set
            {
                this.id = value;
            }
        }
        /// <summary>
        /// 承运商代码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 快递的图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 注册地址
        /// </summary>
        public string RegAddress { get; set; }

        /// <summary>
        /// 统一社会信用编码
        /// </summary>
        public string Uscc { get; set; }

        /// <summary>
        /// 公司法人
        /// </summary>
        public string Corporation { get; set; }

        /// <summary>
        /// 地区
        /// </summary>
        public string District { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator { get; set; }

        /// <summary>
        /// 地区
        /// </summary>
        public string Place { get; set; }
        /// <summary>
        /// 是否国际
        /// </summary>
        public bool IsInternational { set; get; }


        public GeneralStatus Status { set; get; }

        #endregion

        #region 持久化
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name">承运商名称</param>
        /// <param name="driver">司机姓名</param>
        /// <param name="carnumber">车牌号</param>
        /// <param name="carload">车辆类型/载重</param>
        /// <param name="driverPhone">司机联系方式</param>
        /// <param name="idcard">司机身份账号</param>
        /// <returns></returns>
        public string Enter(string Name, string driver, string carnumber, VehicleType carload, string driverPhone = "", string idcard = "")
        {
            this.Name = Name;
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                if (!repository.ReadTable<Layers.Data.Sqls.PvbCrm.Enterprises>().Any(item => item.ID == this.ID))
                {
                    repository.Insert(new Layers.Data.Sqls.PvbCrm.Enterprises
                    {
                        ID = this.ID,
                        Name = this.Name,
                        AdminCode = "",
                        Status = (int)GeneralStatus.Normal,
                        District = this.District,
                        RegAddress = this.RegAddress,
                        Corporation = this.Corporation,
                        Uscc = this.Uscc
                    });
                }
                //承运商
                if (!repository.ReadTable<Layers.Data.Sqls.PvbCrm.Carriers>().Any(item => item.ID == this.ID))
                {
                    repository.Insert(new Layers.Data.Sqls.PvbCrm.Carriers
                    {
                        ID = this.ID,
                        Icon = string.IsNullOrWhiteSpace(this.Icon) ? "" : this.Icon,
                        Code = string.IsNullOrWhiteSpace(this.Code) ? "" : this.Code,
                        Status = (int)GeneralStatus.Normal,
                        Creator = string.IsNullOrWhiteSpace(this.Creator) ? "" : this.Creator,
                        Summary = null,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                    });
                }
                //司机
                (new Driver
                {
                    EnterpriseID = this.ID,
                    Name = driver,
                    IDCard = idcard,
                    Mobile = driverPhone,
                    Status = GeneralStatus.Normal
                }).Enter();
                //运输工具
                (new Transport
                {
                    EnterpriseID = this.ID,
                    CarNumber1 = carnumber,
                    CarNumber2 = null,
                    Type = carload,
                    //Weight = loading.Carload.ToString(),
                    Status = GeneralStatus.Normal
                }).Enter();

            }
            return this.ID;
        }
        #endregion

        /// <summary>
        /// 华芯通物流部
        /// </summary>
        static public Carrier XdtPCL;

        /// <summary>
        /// 恒远物流部
        /// </summary>
        static public Carrier HyPCL;

        /// <summary>
        /// 个人承运商
        /// </summary>
        static public Carrier Personal;

        static object locker = new object();

        /// <summary>
        /// 静态构造器
        /// </summary>
        /// <remarks>
        /// 添加了初始化华芯通物流部承运商的逻辑
        /// </remarks>
        static Carrier()
        {
            lock (locker)
            {
                Init(ref XdtPCL, nameof(XdtPCL), "华芯通物流部");
                Init(ref HyPCL, nameof(HyPCL), "恒远物流部");
                Init(ref Personal, nameof(Personal), "个人承运商");
            }
        }

        static void Init(ref Carrier carrier, string id, string name = "个人承运商")
        {
            using (var reponsitory = new PvbCrmReponsitory())
            using (var view = new Views.CarriersTopView<PvbCrmReponsitory>(reponsitory))
            using (var tran = reponsitory.OpenTransaction())
            {
                try
                {
                    if (!reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Enterprises>().Any(item => item.ID == id))
                    {
                        reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.Enterprises
                        {
                            ID = id,
                            Name = name,
                            AdminCode = id,
                            Status = (int)ApprovalStatus.Normal,
                            District = null,
                            Corporation = null,
                            Uscc = null,
                            RegAddress = null,
                            Place = name == "个人承运商" ? null : nameof(Origin.CHN),
                        });
                    }
                    if (!reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Carriers>().Any(item => item.ID == id))
                    {
                        reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.Carriers
                        {
                            ID = id,
                            Code = id,
                            CreateDate = DateTime.Now,
                            Creator = Npc.Robot.Obtain(),
                            Icon = "",
                            Status = (int)GeneralStatus.Normal,
                            Summary = "机器人创建",
                            UpdateDate = DateTime.Now,
                            Type = (int)CarrierType.Logistics
                        });
                    }

                    tran.Commit();
                }
                catch (DbException ex)
                {
                    Console.WriteLine(ex);
                }
                finally
                {
                    carrier = view.SingleOrDefault(item => item.ID == id);
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
}
