using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Interpreter.Extends
{
    static public class En_TopObjectExtend
    {
        static internal Layer.Data.Sqls.BvOveralls.TopObjects_En ToLinq(this Models.En_TopObject entity)
        {
            return new Layer.Data.Sqls.BvOveralls.TopObjects_En
            {
                ID = entity.ID,
                Name = entity.Name,
                Language = entity.Language,
                Type = (int)entity.Type,
                Value = entity.Value,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                Project = entity.Project
            };
        }
        static public Models.En_TopObject[] JsonToEn_TopObject(this string json)
        {
            return GetEn_TopObjects(json.JsonTo(), null).ToArray();
        }

        static List<Models.En_TopObject> GetEn_TopObjects(Newtonsoft.Json.Linq.JToken jToken, List<Models.En_TopObject> list)
        {
            if (list == null)
            {
                list = new List<Models.En_TopObject>();
            }
            foreach (var token in jToken)
            {
                if (token.Type == Newtonsoft.Json.Linq.JTokenType.Boolean)
                {
                    list.Add(new Models.En_TopObject
                    {
                        Name = token.Path,
                        Value = string.Empty
                    });
                }
                else if (token.Type == Newtonsoft.Json.Linq.JTokenType.String)
                {
                    list.Add(new Models.En_TopObject
                    {
                        Name = token.Path,
                        Value = (token as Newtonsoft.Json.Linq.JValue).ToString()
                    });
                }
                else
                {
                    GetEn_TopObjects(token, list);
                }
            }

            return list;
        }
    }
}
