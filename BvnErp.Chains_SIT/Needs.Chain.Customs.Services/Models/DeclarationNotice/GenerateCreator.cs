using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class GenerateCreator
    {
        public string getCreator()
        {
            var listModel = new Needs.Ccs.Services.Views.CurrentUnDecNoticeCountView().GetCurrentUnDecNoticeCount();

            if (listModel == null || !listModel.Any())
            {
                return null;
            }

            var minCount = listModel.OrderBy(t => t.UnDecNoticeCount).FirstOrDefault().UnDecNoticeCount;
            int[] serialNos = listModel.Where(t => t.UnDecNoticeCount == minCount).Select(t => t.SerialNo).ToArray();

            Random rand = new Random();
            int arrNum = rand.Next(0, serialNos.Count() - 1);

            var theSelectedModel = listModel.Where(t => t.SerialNo == serialNos[arrNum]).FirstOrDefault();

            for (int i = 0; i < listModel.Count; i++)
            {
                if (listModel[i].SerialNo == serialNos[arrNum])
                {
                    listModel[i].UnDecNoticeCount = listModel[i].UnDecNoticeCount + 1;
                    break;
                }
            }

            return theSelectedModel.AdminID;
        }
    }
}
