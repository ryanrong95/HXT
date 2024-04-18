using System;
using System.Threading;
using Yahv.Web.Controls.Easyui;

namespace Yahv.Web.Forms
{
    /// <summary>
    /// easyui 调用
    /// </summary>
    public class Easyui
    {
        static ClientPage Page
        {
            get
            {
                ClientPage page = Thread.GetData(Thread.GetNamedDataSlot("ClientPage")) as ClientPage;
                if (page == null)
                {
                    throw new NotImplementedException("The specified call was not implemented!");
                }
                return page;
            }
        }

        /// <summary>
        /// 本页面提示信息
        /// </summary>
        /// <param name="title"></param>
        /// <param name="context">提示内容</param>
        /// <param name="type">提示类型</param>
        static public void Alert(string title, string context, Sign type = 0, bool isClose = false, Method method = Method.None)
        {
            Page.ClientScript.RegisterClientScriptBlock(typeof(Alert), title, new Alert
            {
                Title = title,
                Context = context,
                Sign = type,
                isClose = isClose,
                Method = method,
            }.Execute(), true);
        }

        /// <summary>
        /// 本页面提示信息，消息自动关闭
        /// </summary>
        /// <param name="context">提示内容</param>
        /// <param name="type">提示类型</param>
        static public void AutoAlert(string context, AutoSign type = 0)
        {
            Page.ClientScript.RegisterClientScriptBlock(typeof(AutoAlert), context, new AutoAlert
            {
                Context = context,
                Sign = type
            }.Execute(), true);
        }

        /// <summary>
        /// 直接跳转
        /// </summary>
        /// <param name="url"></param>
        static public void Redirect(string url)
        {
            Redirect("", "", url, 0);
        }

        /// <summary>
        /// 提示后跳转
        /// </summary>
        /// <param name="title">提示标题</param>
        /// <param name="context">提示内容</param>
        /// <param name="url">跳转Url</param>
        /// <param name="type">提示类型</param>
        static public void Redirect(string title, string context, string url, Sign type = 0)
        {
            var response = Page.Response;

            if (!string.IsNullOrWhiteSpace(Page.Request[ClientPage.PageModeParamName]))
            {
                if (url.Contains("?"))
                {
                    url += "?";
                }
                url += $"&{ClientPage.PageModeParamName}={Page.Request[ClientPage.PageModeParamName]}";
            }
            response.ClearContent();
            response.Write(new Redirect
            {
                Title = title,
                Context = context,
                Sign = type,
                Url = url,
            }.Execute());
            response.End();
        }

        /// <summary>
        /// 重新加加载
        /// </summary>
        /// <param name="title"></param>
        /// <param name="context"></param>
        /// <param name="type"></param>
        static public void Reload(string title, string context, Sign type = 0)
        {
            Redirect(title, context, Page.Request.Url.OriginalString, 0);
        }

        /// <summary>
        /// 重新加加载
        /// </summary>
        /// <param name="context">提示内容</param>
        /// <param name="type">提示类型</param>
        static public void AutoReload(string context, AutoSign type = 0)
        {
            var response = Page.Response;

            response.ClearContent();
            response.Write(new AutoRedirect
            {
                Context = context,
                Sign = type,
                Url = Page.Request.Url.OriginalString,
            }.Execute());
            response.End();
        }


        /// <summary>
        /// 重新加加载
        /// </summary>
        /// <param name="context">提示内容</param>
        /// <param name="type">提示类型</param>
        static public void AutoRedirect(string context, string url, AutoSign type = 0)
        {
            var response = Page.Response;

            response.ClearContent();
            response.Write(new AutoRedirect
            {
                Context = context,
                Sign = type,
                Url = url,
            }.Execute());
            response.End();
        }



        static object windowLocker = new object();
        static EasyuiWindow window;

        /// <summary>
        /// 弹出窗体
        /// </summary>
        static public EasyuiWindow Window
        {
            get
            {
                if (window == null)
                {
                    lock (windowLocker)
                    {
                        if (window == null)
                        {
                            window = new EasyuiWindow();
                        }
                    }
                }
                return window;
            }
        }

        static object dialogLocker = new object();
        static EasyuiDialog dialog;

        /// <summary>
        /// 弹出对话框
        /// </summary>
        static public EasyuiDialog Dialog
        {
            get
            {
                if (dialog == null)
                {
                    lock (dialogLocker)
                    {
                        if (dialog == null)
                        {
                            dialog = new EasyuiDialog();
                        }
                    }
                }
                return dialog;
            }
        }

        //static object tWindowLocker = new object();
        //static EasyuiTWindow tWindow;

        ///// <summary>
        ///// 弹出T窗体
        ///// </summary>
        //static public EasyuiTWindow TWindow
        //{
        //    get
        //    {
        //        if (tWindow == null)
        //        {
        //            lock (tWindowLocker)
        //            {
        //                if (tWindow == null)
        //                {
        //                    tWindow = new EasyuiTWindow();
        //                }
        //            }
        //        }
        //        return tWindow;
        //    }
        //}

        //static object tDialogLocker = new object();
        //static EasyuiTDialog tDialog;

        ///// <summary>
        ///// 弹出T对话框
        ///// </summary>
        //static public EasyuiTDialog TDialog
        //{
        //    get
        //    {
        //        if (tDialog == null)
        //        {
        //            lock (tDialogLocker)
        //            {
        //                if (tDialog == null)
        //                {
        //                    tDialog = new EasyuiTDialog();
        //                }
        //            }
        //        }
        //        return tDialog;
        //    }
        //}

        static object tTopLocker = new object();
        static EasyuiTop tTop;

        /// <summary>
        /// 弹出T对话框
        /// </summary>
        static public EasyuiTop Ttop
        {
            get
            {
                if (tTop == null)
                {
                    lock (tTopLocker)
                    {
                        if (tTop == null)
                        {
                            tTop = new EasyuiTop();
                        }
                    }
                }
                return tTop;
            }
        }
    }

    /// <summary>
    /// EasyuiWindow 调用者
    /// </summary>
    public class EasyuiWindow
    {
        protected static ClientPage Page
        {
            get
            {
                ClientPage page = Thread.GetData(Thread.GetNamedDataSlot("ClientPage")) as ClientPage;
                if (page == null)
                {
                    throw new NotImplementedException("The specified call was not implemented!");
                }
                return page;
            }
        }

        virtual public void Close(string context, AutoSign type = 0)
        {
            string pmpn = Page.Request[ClientPage.PageModeParamName];
            if (string.IsNullOrWhiteSpace(pmpn))
            {
                var response = Page.Response;

                response.ClearContent();
                response.Write(new AutoCloseWindow
                {
                    Context = context,
                    Sign = type,
                }.Execute());
                response.End();
            }
            else
            {
                var response = Page.Response;

                response.ClearContent();
                response.Write(new TopCloseWindow
                {
                    Context = context,
                    Sign = type,
                }.Execute());
                response.End();
            }
        }
    }
    /// <summary>
    /// EasyuiDialog 调用者
    /// </summary>
    public class EasyuiDialog : EasyuiWindow
    {
        override public void Close(string context, AutoSign type = 0)
        {
            string pmpn = Page.Request[ClientPage.PageModeParamName];
            if (string.IsNullOrWhiteSpace(pmpn))
            {
                var response = Page.Response;
                response.ClearContent();
                response.Write(new AutoCloseDialog
                {
                    Context = context,
                    Sign = type,
                }.Execute());
                response.End();
            }
            else
            {
                var response = Page.Response;

                response.ClearContent();
                response.Write(new TopCloseDialog
                {
                    Context = context,
                    Sign = type,
                }.Execute());
                response.End();
            }
        }
    }

    ///// <summary>
    ///// EasyuiTWindow 调用者
    ///// </summary>
    //public class EasyuiTWindow : EasyuiWindow
    //{
    //    override public void Close(string context, AutoSign type = 0)
    //    {
    //        var response = Page.Response;

    //        response.ClearContent();
    //        response.Write(new TopCloseWindow
    //        {
    //            Context = context,
    //            Sign = type,
    //        }.Execute());
    //        response.End();
    //    }
    //}

    ///// <summary>
    ///// EasyuiTDialog 调用者
    ///// </summary>
    //public class EasyuiTDialog : EasyuiWindow
    //{
    //    override public void Close(string context, AutoSign type = 0)
    //    {
    //        var response = Page.Response;

    //        response.ClearContent();
    //        response.Write(new TopCloseDialog
    //        {
    //            Context = context,
    //            Sign = type,
    //        }.Execute());
    //        response.End();
    //    }
    //}

    /// <summary>
    /// EasyuiTDialog 调用者
    /// </summary>
    public class EasyuiTop : EasyuiWindow
    {
        override public void Close(string context, AutoSign type = 0)
        {
            string pmpn = Page.Request[ClientPage.PageModeParamName] ?? Page.Request[ClientPage.PageParamName];

            //fuse
            if (!string.IsNullOrWhiteSpace(pmpn))
            {
                var mode = (PageMode)Enum.Parse(typeof(PageMode), pmpn, true);

                switch (mode)
                {
                    case PageMode.Normal:
                        break;
                    case PageMode.Window:
                        Easyui.Window.Close(context, type);
                        break;
                    case PageMode.Dialog:
                        Easyui.Dialog.Close(context, type);
                        break;
                    default:
                        break;
                }

            }
            //传统

        }
    }

}
