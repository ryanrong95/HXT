using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvWsClient.Setting
{
    /// <summary>
    /// 配置管理器
    /// </summary>
    public class SettingsManager<TSetting> where TSetting : Yahv.Settings.ISettings
    {
        TSetting setting;
        Type currentType;

        SettingsManager()
        {
            Type type = typeof(TSetting);
            if (type.IsInterface)
            {
                string name = type.Name.Substring(1);
                var temp = Type.GetType($"Yahv.PvWsClient.Model.{name},Yahv.PvWsClient", false);
                if(temp == null)
                {
                    throw new NotImplementedException($"The system does not implement the type:{type.FullName} of interface!");
                }
                this.currentType = temp;
            }
            else
            {
                this.currentType = type;
            }

            if(setting == null)
            {
                setting = (TSetting)Activator.CreateInstance(this.currentType, true);
            }

            return;
        }

        static SettingsManager<TSetting> current;
        static object locker = new object();

        static public TSetting Current
        {
            get
            {
                if(current == null)
                {
                    lock (locker)
                    {
                        current = new SettingsManager<TSetting>();
                    }
                }
                return current.setting;
            }
        }
    }
}
