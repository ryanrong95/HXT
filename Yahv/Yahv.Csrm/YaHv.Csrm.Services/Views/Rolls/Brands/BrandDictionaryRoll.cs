using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Rolls
{
    public class BrandDictionaryRoll : Origins.BrandDictionaryOrigin
    {
        string Name;
        public BrandDictionaryRoll(string name)
        {
            this.Name = name;
        }
        public BrandDictionaryRoll()
        {
            
        }
        protected override IQueryable<BrandDictionary> GetIQueryable()
        {
            if (string.IsNullOrWhiteSpace(this.Name))
            {
                return base.GetIQueryable();
            }
            else
            {
                return base.GetIQueryable().Where(item => item.Name == this.Name);
            }
        }
    }
}
