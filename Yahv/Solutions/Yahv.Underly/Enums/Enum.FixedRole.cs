using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 固定角色
    /// </summary>
    public enum FixedRole
    {
        /// <summary>
        /// npc
        /// </summary>
        [Fixed("NRole000")]
        [Description("新员工")]
        NewStaff,

        /// <summary>
        /// npc
        /// </summary>
        [Fixed("NRole001")]
        [Description("NPC")]
        Npc,
        /// <summary>
        /// 销售员
        /// </summary>
        [Fixed("FRole001")]
        [Description("销售")]
        Sale,
        /// <summary>
        /// 销售经理
        /// </summary>
        [Fixed("FRole002")]
        [Description("销售经理")]
        SaleManager,
        /// <summary>
        /// 预判员
        /// </summary>
        [Fixed("FRole003")]
        [Description("预判员")]
        Predictor,
        /// <summary>
        /// 采购员
        /// </summary>
        [Fixed("FRole004")]
        [Description("采购员")]
        Purchaser,
        /// <summary>
        /// 采购经理
        /// </summary>
        [Fixed("FRole005")]
        [Description("采购经理")]
        PurchasingManager,
        /// <summary>
        /// 产品经理
        /// </summary>
        [Fixed("FRole006")]
        [Description("产品经理")]
        PM,
        /// <summary>
        /// 产品经理助理
        /// </summary>
        [Fixed("FRole007")]
        [Description("产品经理助理")]
        PMa,
        /// <summary>
        /// 销售助理
        /// </summary>
        [Fixed("FRole008")]
        [Description("销售助理")]
        SaleA,
        /// <summary>
        /// 客户管理员
        /// </summary>
        [Fixed("FRole009")]
        [Description("客户管理员")]
        ClientManager,

        /// <summary>
        /// 报关业务员
        /// </summary>
        [Fixed("FRole010")]
        [Description("报关业务员")]
        ServiceManager,

        /// <summary>
        /// 报关跟单员
        /// </summary>
        [Fixed("FRole011")]
        [Description("报关跟单员")]
        Merchandiser,

        /// <summary>
        /// 报关业务部负责人
        /// </summary>
        [Fixed("FRole012")]
        [Description("报关业务部负责人")]
        ServiceManagerLeader,

        /// <summary>
        /// 客户订单合同盖章人
        /// </summary>
        [Fixed("FRole013")]
        [Description("客户订单合同盖章人")]
        Signaturer,

        /// <summary>
        /// 华芯通香港库房
        /// </summary>
        [Fixed("FRole014")]
        [Description("华芯通香港库房")]
        HkWarehouse,

        /// <summary>
        /// 华芯通深圳库房
        /// </summary>
        [Fixed("FRole015")]
        [Description("华芯通深圳库房")]
        SzWarehouse,

        /// <summary>
        /// FAE
        /// </summary>
        [Fixed("FRole016")]
        [Description("FAE")]
        FAE,

        /// <summary>
        /// 采购助理
        /// </summary>
        [Fixed("FRole017")]
        [Description("采购助理")]
        PurchasingAssistant,

        /// <summary>
        /// 采购助理
        /// </summary>
        [Fixed("FRole018")]
        [Description("传统贸易销售预判")]
        TradeSalePredictor,

        /// <summary>
        /// 采购助理
        /// </summary>
        [Fixed("FRole019")]
        [Description("传统贸易销售经理预判")]
        TradeSMPredictor,
    }
}
