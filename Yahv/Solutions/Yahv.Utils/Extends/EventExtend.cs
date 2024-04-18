using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.Utils.EventExtend
{
    public static class EventExtend
    {
        public static T AddEvent<T>(this T instance, string eventname, Delegate handler) where T : class
        {
            var eventInfo = instance.GetType().GetEvent(eventname);
            eventInfo.AddEventHandler(instance, handler);
            return instance;
        }
    }
}
