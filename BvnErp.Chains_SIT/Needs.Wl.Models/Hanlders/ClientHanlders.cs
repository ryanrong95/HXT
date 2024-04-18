using System;

namespace Needs.Wl.Models.Hanlders
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
        public Client Client { get; private set; }

        public ClientStatusChangedEventArgs(Client client)
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
        public Client Client { get; private set; }

        public Enums.ClientRank OldRank { get; private set; }

        public Enums.ClientRank NewRank { get; private set; }

        public ClientRankChangedEventArgs(Client client, Enums.ClientRank oldRank, Enums.ClientRank newRank)
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
    /// 状态改变
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">状态改变事件参数</param>
    public delegate void StatusChangedEventHanlder(object sender, StatusChangedEventArgs e);

    /// <summary>
    /// 状态改变事件参数
    /// </summary>
    public class StatusChangedEventArgs : EventArgs
    {
        public dynamic Object { get; private set; }

        public StatusChangedEventArgs(object entity)
        {
            this.Object = entity;
        }

        public StatusChangedEventArgs() { }
    }
}