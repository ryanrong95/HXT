using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.PlanningService
{
    public class ApiSetting
    {
        public string Client { get; set; }

        public int Timeout = 9000;

        public string ContentType { get; set; }

        public Dictionary<string, string> Headers { get; set; }

        /// <summary>
        /// TODO:完成Apis的索引器的代码  ApiSetting.Apis["key"],返回Api对象，可以根据实际需求，重新设计json
        /// </summary>
        public Apis Apis;
    }

    public class Api
    {
        /// <summary>
        /// TODO:完成Api类型的设计，使用枚举？key?还是其他类型，方便 ApiSetting.Apis["key"]的调用
        /// </summary>
        public ApiType Type;

        public string Method;

        public string Key;

        public string Url;

        public string SavePIPath;
    }
}