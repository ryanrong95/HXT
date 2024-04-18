using NtErp.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Services.Extends
{
    static public class LanguageExtends
    {
        static public Layer.Data.Sqls.BvOveralls.Languages ToLinq(this Language entity)
        {
            return new Layer.Data.Sqls.BvOveralls.Languages
            {
                ID =entity.ID,
                ShortName = entity.ShortName,
                DisplayName = entity.DisplayName,
                EnglishName = entity.EnglishName,
                DataName = entity.DataName
            };
        }
    }
}
