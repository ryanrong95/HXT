using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Hanlders
{
    /// <summary>
    /// 当报关单生成装箱单成功时发生
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void PackingListSavedHanlder(object sender, PackingListSavedEventArgs e);

    /// <summary>
    /// 生成装箱单成功后的参数
    /// </summary>
    public class PackingListSavedEventArgs : EventArgs
    {
        public Models.DecHead DecHead { get; private set; }

        public string FileName { get; private set; }

        public string FilePath { get; private set; }

        public string FileSize { get; private set; }

        public PackingListSavedEventArgs(Models.DecHead entity,string FileName, string FilePath,string FileSize)
        {
            this.DecHead = entity;
            this.FileName = FileName;
            this.FilePath = FilePath;
            this.FileSize = FileSize;
        }

        public PackingListSavedEventArgs() { }
    }
}
