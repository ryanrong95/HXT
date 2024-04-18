using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Services
{
    public class PackingSensitiveCheck
    {
        public List<PackingSensitiveModel> PackingInfo { get; set; }

        public PackingSensitiveCheck(List<PackingSensitiveModel> packingInfo)
        {
            this.PackingInfo = packingInfo;
        }

        public bool Check(out string sensitivearea)
        {
            sensitivearea = "";
            var sensitiveAreas = new PackingSensitiveAreaView().Select(t => t.Code).Distinct().ToList();
            foreach(var item in this.PackingInfo)
            {
                List<String> boxIndexs = item.Areas.Distinct().ToList();
                var exceptArr = boxIndexs.Except(sensitiveAreas).ToList();
                if (exceptArr.Count() == boxIndexs.Count())
                {
                    //箱子 没有 敏感产地
                    continue;
                }
                else if(exceptArr.Count()==0)
                {
                    //箱子 全是 敏感产地，
                    continue;
                }
                else
                {                   
                    sensitivearea = "存在敏感产地:" + string.Join("," ,boxIndexs.Except(exceptArr).ToList());
                    return false;
                }
            }
            return true;
        }
    }
}
