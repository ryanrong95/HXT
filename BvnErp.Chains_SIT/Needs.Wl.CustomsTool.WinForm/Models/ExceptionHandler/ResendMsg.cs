using Needs.Ccs.Services.Models.BalanceQueueRedis.CoreStep;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.CustomsTool.WinForm.Models.ExceptionHandler
{
    /// <summary>
    /// 重新发送消息(xml)
    /// </summary>
    public class ResendMsg : AbstractHandler
    {
        public override void Handle(ContextParam contextParam)
        {
            string originFullPath = string.Empty;
            if (!string.IsNullOrEmpty(contextParam.MinProcessIDModelInCircle.FilePath) && contextParam.MinProcessIDModelInCircle.FilePath.ToLower().EndsWith(".zip"))
            {
                originFullPath = contextParam.MinProcessIDModelInCircle.FilePath;
            }
            else if (!string.IsNullOrEmpty(contextParam.PairModelInCircleAndQueue.FilePath) && contextParam.PairModelInCircleAndQueue.FilePath.ToLower().EndsWith(".zip"))
            {
                originFullPath = contextParam.PairModelInCircleAndQueue.FilePath;
            }

            if (string.IsNullOrEmpty(originFullPath) || !originFullPath.ToLower().EndsWith(".zip"))
            {
                return;
            }




            string mainDir = string.Empty;
            int subCountNum = 0;

            if (contextParam.BusinessType == Ccs.Services.Enums.BalanceQueueBusinessType.DecHead)
            {
                mainDir = Tool.Current.Folder.DecMainFolder;
                subCountNum = 1;
            }
            else if (contextParam.BusinessType == Ccs.Services.Enums.BalanceQueueBusinessType.Manifest)
            {
                mainDir = Tool.Current.Folder.RmftMainFolder;
                subCountNum = 2;
            }



            string targetDirectory = System.IO.Path.Combine(mainDir, ConstConfig.OutBox);

            string originFileNameWithoutExtension = Path.GetFileNameWithoutExtension(originFullPath);
            string extension = Path.GetExtension(originFullPath);

            string newFileNameWithoutExtension = GetNewFileNameWithoutExtension(originFileNameWithoutExtension, '_', subCountNum);
            string newFullPath = System.IO.Path.Combine(targetDirectory, newFileNameWithoutExtension + extension);

            FileInfo orginFileInfo = new FileInfo(originFullPath);
            if (!File.Exists(newFullPath))
            {
                orginFileInfo.CopyTo(newFullPath, overwrite: true);
            }

        }

        /// <summary>
        /// 从"原不带后缀的文件名"中获取"新的不带后缀的文件名"
        /// </summary>
        /// <param name="originFileNameWithoutExtension">原不带后缀的文件名</param>
        /// <param name="subFlag">切割Flag</param>
        /// <param name="subCountNum">切割到的标志数数个数</param>
        /// <returns>新的不带后缀的文件名</returns>
        static string GetNewFileNameWithoutExtension(string originFileNameWithoutExtension, char subFlag, int subCountNum)
        {
            if (string.IsNullOrEmpty(originFileNameWithoutExtension))
            {
                return string.Empty;
            }

            if (!originFileNameWithoutExtension.Contains(subFlag))
            {
                return originFileNameWithoutExtension;
            }

            string[] strs = originFileNameWithoutExtension.Split(subFlag);
            if (strs.Length - 1 < subCountNum)
            {
                subCountNum = strs.Length - 1;
            }

            StringBuilder sbResult = new StringBuilder();

            for (int i = 0; i < subCountNum; i++)
            {
                sbResult.Append(strs[i] + subFlag);
            }

            return sbResult.ToString().Trim(subFlag);
        }
    }
}
