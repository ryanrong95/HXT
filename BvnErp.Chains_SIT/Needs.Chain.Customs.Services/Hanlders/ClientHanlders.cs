using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Hanlders
{
    /// <summary>
    /// 客户状态改变
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">状态改变事件参数</param>
    public delegate void ClientStatusChangedEventHanlder(object sender, ClientStatusChangedEventArgs e);

    /// <summary>
    /// 状态改变事件参数
    /// </summary>
    public class ClientStatusChangedEventArgs : EventArgs
    {
        public Models.Client Client { get; private set; }

        public ClientStatusChangedEventArgs(Models.Client client)
        {
            this.Client = client;
        }

        public ClientStatusChangedEventArgs() { }
    }

    /// <summary>
    /// 客户等级改变
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">状态改变事件参数</param>
    public delegate void ClientRankChangedHanlder(object sender, ClientRankChangedEventArgs e);

    /// <summary>
    /// 状态改变事件参数
    /// </summary>
    public class ClientRankChangedEventArgs : EventArgs
    {
        public Models.Client Client { get; private set; }

        public Enums.ClientRank OldRank { get; private set; }

        public Enums.ClientRank NewRank { get; private set; }

        public ClientRankChangedEventArgs(Models.Client client, Enums.ClientRank oldRank, Enums.ClientRank newRank)
        {
            this.Client = client;
            this.OldRank = oldRank;
            this.NewRank = newRank;
        }

        public ClientRankChangedEventArgs()
        {

        }
    }


    /// <summary>
    /// 导出协议文档后
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">状态改变事件参数</param>
    public delegate void AgreementExportedEventHanlder(object sender, AgreementExportedEventArgs e);

    /// <summary>
    /// 导出协议文档参数
    /// </summary>
    public class AgreementExportedEventArgs : EventArgs
    {
        public Models.ClientAgreement Agreement { get; private set; }

        public AgreementExportedEventArgs(Models.ClientAgreement agreement)
        {
            this.Agreement = agreement;
        }

        public AgreementExportedEventArgs() { }
    }
}