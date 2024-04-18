using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PsWms.SzMvc.Services.Models
{
    public class SettingsManager<TSetting>
    {
        TSetting setting;
        Type currentType;

        SettingsManager()
        {
            Type type = typeof(TSetting);
            if (type.IsInterface)
            {
                string name = type.Name.Substring(1);
                var temp = Type.GetType($"Yahv.PsWms.SzMvc.Services.Models.{name}", false);
                if (temp == null)
                {
                    throw new NotImplementedException($"The system does not implement the type:{type.FullName} of interface!");
                }
                this.currentType = temp;
            }
            else
            {
                this.currentType = type;
            }

            if (setting == null)
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
                if (current == null)
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
