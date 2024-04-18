using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PsWms.DappForm.Services
{
    /// <summary>
    /// 状态消息接口
    /// </summary>
    public interface ISimHelper
    {

        /// <summary>
        /// 打印状态
        /// </summary>
        string PrintStatus { get; set; }

        /// <summary>
        /// 上载与下载状态
        /// </summary>
        string TransferStatus { get; set; }
    }

    /// <summary>
    /// 状态信息管理
    /// </summary>
    public class SimHelper
    {
        /// <summary>
        /// 打印状态
        /// </summary>
        static public string PrintStatus
        {
            get
            {
                return current.PrintStatus;
            }
            set
            {
                current.PrintStatus = value;
            }
        }

        /// <summary>
        /// 上载与下载状态
        /// </summary>
        static public string TransferStatus
        {
            get
            {
                return current.TransferStatus;
            }
            set
            {
                if (current == null)
                {
                    return;
                }
                current.TransferStatus = value;
            }
        }


        static ISimHelper current;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="sim">状态消息接口 对象</param>
        static public void Initialize(ISimHelper sim)
        {
            if (current == null)
            {
                current = sim;
            }
            else
            {
                throw new Exception("It can only be initialized once!");
            }
        }

        /// <summary>
        /// 系统当前活跃的主窗体 的Firefox
        /// </summary>
        static public Gecko.GeckoWebBrowser Firefox { get; private set; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="sim">状态消息接口 对象</param>
        static public void Initialize(Gecko.GeckoWebBrowser firefox)
        {
            Firefox = firefox;
        }
    }
}
