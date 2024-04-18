namespace Needs.Wl.Settings
{
    /// <summary>
    /// 配置管理器
    /// </summary>
    public static class SettingManager
    {
        public static AppSettings AppSettings
        {
            get { return new AppSettings(); }
        }
    }

    public class AppSettings
    {
        internal AppSettings()
        {

        }

        public AppSetting this[string key]
        {
            get
            {
                //TODO:下一版本目标，使用缓存
                AppSettingsView view = new AppSettingsView(key)
                {
                    AllowPaging = false
                };
                return view.FirstOrDefault();
            }
        }
    }
}
