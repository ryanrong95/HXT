using Needs.Interpreter.Model;
using Needs.Interpreter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Needs.Utils.Serializers;

namespace Needs.Interpreter.Extends
{
    static public class TopObjectExtends
    {
        static internal Layer.Data.Sqls.BvOveralls.TopObjects ToLinq(this TopObject entity)
        {
            return new Layer.Data.Sqls.BvOveralls.TopObjects
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
       
        static public Models.TopObject[] JsonToTopObject(this string json)
        {
            return GetTopObjects(json.JsonTo(), null).ToArray();
        }

        static List<TopObject> GetTopObjects(Newtonsoft.Json.Linq.JToken jToken, List<TopObject> list)
        {
            if (list == null)
            {
                list = new List<TopObject>();
            }
            foreach (var token in jToken)
            {
                if (token.Type == Newtonsoft.Json.Linq.JTokenType.Boolean)
                {
                    list.Add(new TopObject
                    {
                        Name = token.Path,
                        Value = string.Empty
                    });
                }
                else if (token.Type == Newtonsoft.Json.Linq.JTokenType.String)
                {
                    list.Add(new TopObject
                    {
                        Name = token.Path,
                        Value = (token as Newtonsoft.Json.Linq.JValue).ToString()
                    });
                }
                else
                {
                    GetTopObjects(token, list);
                }
            }

            return list;
        }
    }
}
