#define Union 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;
using Yahv.Utils.Serializers;

namespace YaHv.Csrm.Services
{
    #region Conduct

    /// <summary>
    /// 业务类型
    /// </summary>
    public enum ConductType
    {
        /// <summary>
        /// 内部业务
        /// </summary>
        [Description("内部业务")]
        Owns = 0,

        /// <summary>
        /// 传统贸易
        /// </summary>
        [Description("传统贸易")]
        Trades,
        /// <summary>
        /// 代理线贸易
        /// </summary>
        [Description("代理线贸易")]
        AgentLines,
        /// <summary>
        /// 供应链
        /// </summary>
        [Description("供应链业务")]
        Chains,


    }

    /// <summary>
    /// 结算业务类型
    /// </summary>
    public enum SelletConductType
    {
        [Description("贸易业务")]
        Trades,
        [Description("供应链业务")]
        Chains,
#if !Union
        AgentLine,
#endif

    }

    #endregion

    #region RelationType

    public enum RelationType
    {
        /// <summary>
        /// 代理公司
        /// </summary>
        [Description("代理公司")]
        Agent,
        /// <summary>
        /// 客户公司
        /// </summary>
        [Description("客户公司")]
        Client,
        /// <summary>
        /// 下单公司
        /// </summary>
        [Description("下单公司")]
        Place,
        /// <summary>
        /// 设计公司
        /// </summary>
        [Description("设计公司")]
        Designer,
        /// <summary>
        /// 兄弟关系
        /// </summary>
        [Description("兄弟关系")]
        Brother,
        /// <summary>
        /// 母子关系
        /// </summary>
        [Description("母子关系")]
        Depend,


    }
    /// <summary>
    /// 商务关系
    /// </summary>
    //public enum BusinessRelationType
    //{
    //    /// <summary>
    //    /// 代理公司
    //    /// </summary>
    //    [Description("代理公司")]
    //    Agent,
    //    /// <summary>
    //    /// 客户公司
    //    /// </summary>
    //    [Description("客户公司")]
    //    Client,
    //    /// <summary>
    //    /// 下单公司
    //    /// </summary>
    //    [Description("下单公司")]
    //    Place,
    //    /// <summary>
    //    /// 设计公司
    //    /// </summary>
    //    [Description("设计公司")]
    //    Designer
    //}

    /// <summary>
    /// 血亲关系
    /// </summary>
    //public enum BloodRelationType
    //{
    //    /// <summary>
    //    /// 兄弟关系
    //    /// </summary>
    //    [Description("兄弟关系")]
    //    Brother,
    //    /// <summary>
    //    /// 母子关系
    //    /// </summary>
    //    [Description("母子关系")]
    //    Depend, 

    //}

    /*
     from map in maps 
     where (map.mainid = '' or map.subid ='')  
     select  {
     mainid =  mianid 
     subid = subid 
     type =  type
        }
     */

    #endregion

    #region nBrandType

    /// <summary>
    /// 商务关系
    /// </summary>
    public enum nBrandType
    {
        /// <summary>
     /// 代理
     /// </summary>
        [Description("代理")]
        Agent,
        /// <summary>
        /// 生产
        /// </summary>
        [Description("生产")]
        Produce,
        /// <summary>
        /// 经销
        /// </summary>
        [Description("经销")]
        Distribution,
    }

    /// <summary>
    /// 收货人类型
    /// </summary>
    public enum AddressType
    {
        /// <summary>
        /// 收票人
        /// </summary>
        [Description("收票地址")]
        Invoice,

        /// <summary>
        /// 收货人
        /// </summary>
        [Description("收货地址")]
        Consignee,

        /// <summary>
        /// 交货地址
        /// </summary>
        [Description("交货地址")]
        Consignor,

        /// <summary>
        /// 办公地址
        /// </summary>
        [Description("办公地址")]
        Working,

        /// <summary>
        /// 研发地址
        /// </summary>
        [Description("研发地址")]
        Devloping,

        /// <summary>
        /// 生产地址
        /// </summary>
        [Description("生产地址")]
        Produce
    }


    #endregion

    #region BookAccount 

    /// <summary>
    /// 帐号类型
    /// </summary>
    public enum BookAccountType
    {
        /// <summary>
        /// 收款人
        /// </summary>
        [Description("收款人")]
        Payee,
        /// <summary>
        /// 付款人
        /// </summary>
        [Description("付款人")]
        Payer
    }

    /// <summary>
    /// 帐号类型
    /// </summary>
    public enum BookAccountMethord
    {
        /// <summary>
        /// 微信
        /// </summary>
        [Description("微信")]
        Wx,
        /// <summary>
        /// 付款人
        /// </summary>
        [Description("支付宝")]
        Zfb,
        /// <summary>
        /// 银行转账
        /// </summary>
        [Description("银行转账")]
        Bank,
    }

    #endregion

    /// <summary>
    /// 员工类型
    /// </summary>
    /// <remarks>
    /// FixedRole
    /// </remarks>
    public enum StaffType
    {
        /// <summary>
        /// 跟单员
        /// </summary>
        Tracker,
    }

    /// <summary>
    /// 枚举转换
    /// </summary>
    static public class EnumConverter
    {
        /// <summary>
        /// 业务 与 结算 类型转换器
        /// </summary>
        /// <param name="type">业务类型</param>
        /// <returns>结算业务类型</returns>
        static public SelletConductType ToSelletment(this ConductType type)
        {
            switch (type)
            {
                //case ConductType.Chains:
                //    return SelletConductType.Chains;
                //case ConductType.Trade:
                //    return SelletConductType.Trade;
                //case ConductType.AgentLine:
#if Union
                    //return SelletConductType.Trade;
#else
                    return SelletConductType.AgentLine;
#endif

                default:
                    throw new NotImplementedException("未实现指定的类型：" + type);
            }
        }
    }



    class MyClass
    {


        public interface IMyCloneable : ICloneable
        {

            /// <summary>
            /// 
            /// </summary>
            /// <param name="isCloneDb">是否同步复制数据中的数据并返回数据库对象</param>
            /// <returns></returns>
            object Clone(bool isCloneDb);

        }

        public class Enterprise : IMyCloneable
        {

            public string ID
            {
                get; set;
            }

            /// <summary>
            /// 深度复制
            /// </summary>
            /// <remarks>
            /// 包涵数据本身的复制(JSON)
            /// 与实际数据的复制（DB）
            /// </remarks>
            public object Clone()
            {
                throw new NotImplementedException();
            }

            public object Clone(bool isCloneDb)
            {
                var json = this.Json();
                var newer = json.JsonTo<Enterprise>();

                //

                if (isCloneDb)
                {
                    return this.Clone();
                }
                else
                {
                    return newer;
                }
            }
        }

    }

}
