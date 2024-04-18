using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Hanlders
{
    /// <summary>
    /// 当报关单生成合同成功时发生
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void ContractSavedHanlder(object sender, ContractSavedEventArgs e);

    /// <summary>
    /// 生成合同成功后的参数
    /// </summary>
    public class ContractSavedEventArgs : EventArgs
    {
        public Models.DecHead DecHead { get; private set; }

        public string FileName { get; private set; }

        public string FilePath { get; private set; }

        public string FileSize { get; private set; }

        public ContractSavedEventArgs(Models.DecHead entity,string FileName, string FilePath,string FileSize)
        {
            this.DecHead = entity;
            this.FileName = FileName;
            this.FilePath = FilePath;
            this.FileSize = FileSize;
        }

        public ContractSavedEventArgs() { }
    }
}
