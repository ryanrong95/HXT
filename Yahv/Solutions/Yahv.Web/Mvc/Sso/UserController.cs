using System.Web.Mvc;

namespace Yahv.Web.Mvc.Sso
{
    /// <summary>
    /// 登录用户控制器
    /// </summary>
    abstract public class UserController : ClientController
    {
        /// <summary>
        /// 默认构造器
        /// </summary>
        public UserController()
        {

        }

        /// <summary>
        /// 已重写 OnActionExecuting
        /// </summary>
        /// <param name="filterContext">执行上下文</param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (this.Authenticate())
            {
                this.OnDenied();
            }
            else

            {
                this.OnSucess();
            }
        }

        /// <summary>
        /// 验证调用
        /// </summary>
        /// <returns>是否通过</returns>
        abstract protected bool Authenticate();

        /// <summary>
        /// 验证成功调用
        /// </summary>
        abstract protected void OnSucess();

        /// <summary>
        /// 验证失败调用
        /// </summary>
        abstract protected void OnDenied();
    }
}
