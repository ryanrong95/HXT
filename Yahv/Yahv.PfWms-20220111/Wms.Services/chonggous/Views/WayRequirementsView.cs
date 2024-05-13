using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Underly.Attributes;
using Yahv.Utils.Serializers;

namespace Wms.Services.chonggous.Views
{
    public class WayRequirementsView : QueryView<MyWayRequirement, PvCenterReponsitory>
    {
        public WayRequirementsView()
        {
            var checkTest1 = new CheckRequirementForShow
            {
                ApplicationID = "Apply0001",
                IsPayCharge = null,
                DelivaryOpportunity = DelivaryOpportunity.PaymentBeforeDelivery.GetDescription(),
            };

            //var checkTest2 = new CheckRequirementForShow
            //{
            //    ApplicationID = "Apply0012s",
            //    DelivaryOpportunity = DelivaryOpportunity.PaymentAfterDelivery.GetDescription(),                
            //};

            this.CheckTests = checkTest1;

            var orderTest1 = new OrderRequirement
            {
                ID = "12f82a13-189e-4c15-a927-1aec4f4d83a0",
                Type = SpecialRequire.DivideBox,
                Name = "分箱",
                Requirement = "大(70*40*40CM)40元/个",
                Quantity = 2,
                TotalPrice = 80,
                CreateDate = DateTime.Now,
            };

            var orderTest2 = new OrderRequirement
            {
                ID = "1e696357-ca8d-4979-970c-a10a606c0add",
                Type = SpecialRequire.Label,
                Name = "撕标签",
                Requirement = "就是去除标签",
                Quantity = 2,
                TotalPrice = 80,
                CreateDate = DateTime.Now,
            };

            this.OrderTests = new[] { orderTest1, orderTest2 };
        }
        public WayRequirementsView(PvCenterReponsitory reponsitory) : base(reponsitory)
        {
        }

        public CheckRequirementForShow CheckTests { get; private set; }

        public OrderRequirement[] OrderTests { get; private set; }

        protected override IQueryable<MyWayRequirement> GetIQueryable()
        {
            var view = from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.WayRequirements>()
                       select new MyWayRequirement(entity.OrderRequirement, entity.CheckRequirement)
                       {
                           ID = entity.ID,
                           IsPayCharge = entity.IsPayCharge,
                           DelivaryOpportunity = (DelivaryOpportunity?)entity.DelivaryOpportunity
                           //Order = string.IsNullOrWhiteSpace(entity.OrderRequirement) ? null : entity.OrderRequirement.JsonTo<OrderRequirement[]>(),
                           //Check = string.IsNullOrWhiteSpace(entity.CheckRequirement) ? null : entity.CheckRequirement.JsonTo<CheckRequirement[]>(),
                       };
            return view;
        }

        /// <summary>
        /// 保存WayRequirements的订单需求
        /// </summary>
        /// <param name="waybillID"></param>
        /// <param name="requirement"></param>
        public void Enter(string waybillID, OrderRequirement[] requirements)
        {
            var orderRequirement = this.Reponsitory.GetTable<Layers.Data.Sqls.PvCenter.WayRequirements>().SingleOrDefault(item => item.ID == waybillID);
            if (orderRequirement == null)
            {
                this.Reponsitory.Insert(new Layers.Data.Sqls.PvCenter.WayRequirements
                {
                    ID = waybillID,
                    OrderRequirement = requirements.Json(),
                });
            }
            else
            {
                this.Reponsitory.Update<Layers.Data.Sqls.PvCenter.WayRequirements>(new
                {
                    OrderRequirement = requirements.Json(),
                }, item => item.ID == waybillID);
            }
        }

        /// <summary>
        /// 保存WayRequirements的支票需求
        /// </summary>
        /// <param name="waybillID"></param>
        /// <param name="requirement"></param>
        public void Enter(string waybillID, CheckRequirement requirement)
        {
            var singler = this.Reponsitory.GetTable<Layers.Data.Sqls.PvCenter.WayRequirements>().
                SingleOrDefault(item => item.ID == waybillID);

            if (singler == null)
            {
                this.Reponsitory.Insert(new Layers.Data.Sqls.PvCenter.WayRequirements
                {
                    ID = waybillID,
                    CheckRequirement = requirement.Json()
                });
            }
            else
            {
                var checks = singler.CheckRequirement.JsonTo<CheckRequirement[]>();

                this.Reponsitory.Update(new Layers.Data.Sqls.PvCenter.WayRequirements
                {
                    CheckRequirement = checks.Json()
                }, item => item.ID == waybillID);
            }
        }
    }

    public class MyWayRequirement
    {

        public MyWayRequirement()
        {

        }

        internal MyWayRequirement(string order, string check)
        {
            this.Check = string.IsNullOrWhiteSpace(check) ? null : check.JsonTo<CheckRequirement>();
            this.Order = string.IsNullOrWhiteSpace(order) ? null : order.JsonTo<OrderRequirement[]>();
        }

        public bool? IsPayCharge { get; set; }

        public DelivaryOpportunity? DelivaryOpportunity { get; set; }

        public string ID { get; set; }

        /// <summary>
        /// 支票数据
        /// </summary>
        public CheckRequirement Check { get; set; }

        /// <summary>
        /// 订单数据
        /// </summary>
        public OrderRequirement[] Order { get; set; }
    }

    /// <summary>
    /// 支票特殊处理类
    /// </summary>
    public class CheckRequirement
    {
        /// <summary>
        /// 申请ID
        /// </summary>
        public string ApplicationID { get; set; }

        /// <summary>
        /// 发货时机
        /// </summary>
        public DelivaryOpportunity? DelivaryOpportunity { get; set; }

        /// <summary>
        /// 代付货款申请
        /// </summary>
        public bool? IsPayCharge { get; set; }

        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="origin">源类型实例</param>

        public static implicit operator CheckRequirementForShow(CheckRequirement origin)
        {
            return new CheckRequirementForShow
            {
                ApplicationID = origin.ApplicationID,
                IsPayCharge = origin.IsPayCharge,
                DelivaryOpportunity = origin.DelivaryOpportunity?.GetDescription(),

            };
        }
    }


    public enum DelivaryOpportunity
    {
        /// <summary>
        /// 现货现款
        /// </summary>
        [Description("现货现款")]
        CashOn = 1,
        /// <summary>
        /// 先款后货
        /// </summary>
        [Description("先款后货")]
        PaymentBeforeDelivery = 2,
        /// <summary>
        /// 先货后款
        /// </summary>
        [Description("先货后款")]
        PaymentAfterDelivery = 3,
    }


    /// <summary>
    /// 订单特殊要求
    /// </summary>
    public class OrderRequirement
    {
        public string ID { get; set; }

        /// <summary>
        /// 特殊要求
        /// </summary>
        public SpecialRequire Type { get; set; }

        public string OrderID { get; set; }
        /// <summary>
        /// 服务要求
        /// </summary>
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// 具体要求
        /// </summary>
        public string Requirement { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }

        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="origin">源类型实例</param>

        public static implicit operator OrderRequirementForShow(OrderRequirement origin)
        {
            return new OrderRequirementForShow
            {
                ID = origin.ID,
                TypeName = origin.Type.GetDescription(),
                Servicing = origin.Name,
                Requirement = origin.Requirement,
                Quantity = origin.Quantity,
                TotalPrice = origin.TotalPrice,
                CreateDate = origin.CreateDate,
            };
        }


    }

    /// <summary>
    /// 订单特殊要求
    /// </summary>
    public class OrderRequirementForShow
    {
        public string ID { get; set; }

        /// <summary>
        /// 特殊要求
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// 服务要求
        /// </summary>
        public string Servicing { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// 具体要求
        /// </summary>
        public string Requirement { get; set; }
        public DateTime CreateDate { get; set; }
    }

    public class CheckRequirementForShow
    {
        /// <summary>
        /// 申请ID
        /// </summary>
        public string ApplicationID { get; set; }

        public bool? IsPayCharge { get; set; }

        public string DelivaryOpportunity { get; set; }
    }
}
