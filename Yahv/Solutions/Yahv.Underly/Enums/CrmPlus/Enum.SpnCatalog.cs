using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 标准型号分类
    /// </summary>
    public enum SpnCatalog
    {
        /// <summary>
        /// 处理器
        /// </summary>
        [Description("处理器")]
        Processor = 1,
        /// <summary>
        /// 信号/接口
        /// </summary>
        [Description("信号/接口")]
        SignalOrInterface = 2,
        /// <summary>
        /// 电源管理
        /// </summary>
        [Description("电源管理")]
        PMU = 3,
        /// <summary>
        /// 存储
        /// </summary>
        [Description("存储")]
        Storage = 4,
        /// <summary>
        /// 马达驱动
        /// </summary>
        [Description("马达驱动")]
        MotorDrive = 5,
        /// <summary>
        /// 语音/功放
        /// </summary>
        [Description("语音/功放")]
        VoiceOrPowerAmplifier = 6,
        /// <summary>
        /// 传感器
        /// </summary>
        [Description("传感器")]
        Sensor = 7,
        /// <summary>
        /// 分立器件
        /// </summary>
        [Description("分立器件")]
        OSD = 8,
        /// <summary>
        /// 保护器件
        /// </summary>
        [Description("保护器件")]
        ProtectionDevice = 9,
        /// <summary>
        /// 电容
        /// </summary>
        [Description("电容")]
        Capacitance = 10,
        /// <summary>
        /// 电阻
        /// </summary>
        [Description("电阻")]
        Resistance = 11,
        /// <summary>
        /// 电感
        /// </summary>
        [Description("电感")]
        Inductance = 12,
        /// <summary>
        /// 晶振/RTC
        /// </summary>
        [Description("晶振/RTC")]
        CrystalOscillatorOrRTC =13,
        // <summary>
        ///  连接器
        /// </summary>
        [Description("连接器")]
        Connector = 14,
        // <summary>
        /// LED/光耦
        /// </summary>
        [Description("LED/光耦")]
        LEDOrOptocoupler = 15,
        // <summary>
        /// 无限射频
        /// </summary>
        [Description("无限射频")]
        InfiniteRadioFrequency = 16,
        // <summary>
        /// 定位/授时
        /// </summary>
        [Description("定位/授时")]
        OsitioningTiming = 17,
        // <summary>
        /// 电源模块
        /// </summary>
        [Description("电源模块")]
        PowerModule = 18,
        // <summary>
        /// 继电器
        /// </summary>
        [Description("继电器")]
        Relay = 19,
        // <summary>
        /// 继电器
        /// </summary>
        [Description("继电器")]
        Transformer = 20,
        

    }
}
