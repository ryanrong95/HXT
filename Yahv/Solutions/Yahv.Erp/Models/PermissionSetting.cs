namespace Yahv.Models
{
    /// <summary>
    /// 权限设置
    /// </summary>
    public class PermissionSetting
    {
        /// <summary>
        /// 所有菜单
        /// </summary>
        public string[] MenesAll { get; set; }

        /// <summary>
        /// 菜单
        /// </summary>
        public string[] MyMenes { get; set; }

        /// <summary>
        /// 颗粒化
        /// </summary>
        public string[] ParticleSettings { get; set; }
    }
}