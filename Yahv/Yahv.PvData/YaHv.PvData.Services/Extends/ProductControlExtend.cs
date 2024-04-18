using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YaHv.PvData.Services.Utils;

namespace YaHv.PvData.Services.Extends
{
    public static class ProductControlExtend
    {
        public static List<object> GetMultiSysControls(this List<string> partNumbers)
        {
            List<object> controlList = new List<object>();

            using (var reponsitory = new PvDataReponsitory())
            {
                for (int i = 0; i < partNumbers.Count(); i++)
                {
                    string partNumber = partNumbers[i].FixSpecialChars();
                    var sysCcc = new Views.Alls.ProductControlsAll(reponsitory)[partNumber, ControlType.Ccc];
                    var sysEmbargo = new Views.Alls.ProductControlsAll(reponsitory)[partNumber, ControlType.Embargo];

                    controlList.Add(new
                    {
                        PartNumber = partNumber,
                        IsSysCcc = sysCcc == null ? false : true,
                        IsSysEmbargo = sysEmbargo == null ? false : true
                    });
                }
            }

            return controlList;
        }
    }
}
