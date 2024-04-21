using System.Collections.Generic;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Utils.Serializers;

namespace Yahv.Erm.Services.Extends
{
    public static class AdvantageExtend
    {
        static public IEnumerable<Manufacturer> ToManufacturerView(this string obj)
        {
            return JsonSerializerExtend.JsonTo<Newtonsoft.Json.Linq.JArray>(obj).ToObject<List<Manufacturer>>();
        }
        static public IEnumerable<PartNumber> ToPartnnumberView(this string obj)
        {
            return JsonSerializerExtend.JsonTo<Newtonsoft.Json.Linq.JArray>(obj).ToObject<List<PartNumber>>();
        }
    }
}
