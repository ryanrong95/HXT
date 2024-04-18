using Needs.Interpreter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Interpreter.Extends
{
    static public class TranslateExtends
    {
        static internal Layer.Data.Sqls.BvTester.TopObjects ToLinq(this Translate entity)
        {
            return new Layer.Data.Sqls.BvTester.TopObjects
            {
                ID = entity.ID,
                Name = entity.Name,
                Language = entity.Language,
                Type = entity.Type,
                Value = entity.Value
            };
        }        
    }
}
