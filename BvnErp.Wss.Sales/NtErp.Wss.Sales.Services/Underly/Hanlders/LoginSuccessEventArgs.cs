using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Underly
{
    /// <summary>
    /// 登录成功事件句柄
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">登录事件参数</param>
    public delegate void LoginSuccessHanlder(object sender, LoginEventArgs e);

    /// <summary>
    /// 登录失败事件句柄
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">登录事件参数</param>
    public delegate void LoginFailedHanlder(object sender, LoginEventArgs e);

    /// <summary>
    /// 登出事件句柄
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">登录事件参数</param>
    public delegate void LoggedoutHanlder(object sender, LoginEventArgs e);

    /// <summary>
    /// 登录事件参数
    /// </summary>
    public class LoginEventArgs : EventArgs
    {

    }
}
