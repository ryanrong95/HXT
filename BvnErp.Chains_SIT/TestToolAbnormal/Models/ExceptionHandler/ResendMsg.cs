using Needs.Ccs.Services.Models.BalanceQueue.CoreStep;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestToolAbnormal.Models.ExceptionHandler
{
    /// <summary>
    /// 重新发送消息(xml)
    /// </summary>
    public class ResendMsg : AbstractHandler
    {
        public override void Handle(ContextParam contextParam)
        {
            string originFullPath = @"D:\ImpPath\Deccus001\FailBox\2019-02-14\aaaaCDO201902120000001_201902141533076750136.zip";

            string targetDirectory = @"D:\ImpPath\Deccus001\OutBox";

            string newFullPath = targetDirectory + @"\" + Path.GetFileName(originFullPath);

            string orginalFileNameWithoutExtension = Path.GetFileNameWithoutExtension(originFullPath);
            
            if (orginalFileNameWithoutExtension.Contains("_"))
            {
                string extension = Path.GetExtension(originFullPath);
                string newFileNameWithoutExtension = orginalFileNameWithoutExtension.Split('_')[0];
                newFullPath = targetDirectory + @"\" + newFileNameWithoutExtension + extension;
            }

            FileInfo orginFileInfo = new FileInfo(originFullPath);
            if(!File.Exists(newFullPath))
            {
                orginFileInfo.CopyTo(newFullPath);
            }





        }
    }
}
