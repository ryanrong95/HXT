using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Yahv.Web.Faces
{
    /// <summary>
    /// 自定义Mvc程序启动类
    /// </summary>
    /// <remarks>
    /// 已经重写  ModelBinders.Binders
    /// </remarks>
    public class MyMvcApilication : System.Web.HttpApplication
    {
        /// <summary>
        /// 构造器
        /// </summary>
        /// <remarks>
        /// 已重写
        /// </remarks>
        public MyMvcApilication()
        {
            if (!ModelBinders.Binders.ContainsKey(typeof(JObject)))
            {
                ModelBinders.Binders.Add(typeof(JObject), new JObjectModelBinder());
            }
            if (!ModelBinders.Binders.ContainsKey(typeof(JArray)))
            {
                ModelBinders.Binders.Add(typeof(JArray), new JObjectModelBinder());
            }
            if (!ModelBinders.Binders.ContainsKey(typeof(Mvc.JArrayPost)))
            {
                ModelBinders.Binders.Add(typeof(Mvc.JArrayPost), new JArrayPostModelBinder());
            }
        }
    }
    /// <summary>
    /// JObject 模型绑定
    /// </summary>
    public class JObjectModelBinder : IModelBinder
    {
        /// <summary>
        /// 使用指定的控制器上下文和绑定上下文将模型绑定到一个值
        /// </summary>
        /// <param name="controllerContext">控制器上下文</param>
        /// <param name="bindingContext">绑定器上下文(一般用不上)</param>
        /// <returns>绑定JObject类型返回值</returns>
        /// <remarks>
        /// 已重写
        /// </remarks>
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var stream = controllerContext.RequestContext.HttpContext.Request.InputStream;
            stream.Seek(0, SeekOrigin.Begin);
            string json = new StreamReader(stream).ReadToEnd();
            return JObject.Parse(json);
        }
    }
    /// <summary>
    /// JArray 模型绑定
    /// </summary>
    public class JArrayModelBinder : IModelBinder
    {
        /// <summary>
        /// 使用指定的控制器上下文和绑定上下文将模型绑定到一个值
        /// </summary>
        /// <param name="controllerContext">控制器上下文</param>
        /// <param name="bindingContext">绑定器上下文(一般用不上)</param>
        /// <returns>绑定JArray类型返回值</returns>
        /// <remarks>
        /// 已重写
        /// </remarks>
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var stream = controllerContext.RequestContext.HttpContext.Request.InputStream;
            stream.Seek(0, SeekOrigin.Begin);
            string json = new StreamReader(stream).ReadToEnd();
            return JArray.Parse(json);
        }
    }

    /// <summary>
    /// JArrayPost 模型绑定
    /// </summary>
    public class JArrayPostModelBinder : IModelBinder
    {
        /// <summary>
        /// 使用指定的控制器上下文和绑定上下文将模型绑定到一个值
        /// </summary>
        /// <param name="controllerContext">控制器上下文</param>
        /// <param name="bindingContext">绑定器上下文(一般用不上)</param>
        /// <returns>绑定JArrayPost类型返回值</returns>
        /// <remarks>
        /// 已重写
        /// </remarks>
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var stream = controllerContext.RequestContext.HttpContext.Request.InputStream;
            stream.Seek(0, SeekOrigin.Begin);
            string json = new StreamReader(stream).ReadToEnd();
            return new Mvc.JArrayPost(json);
        }
    }
}
