﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Yahv.PsWms.DappForm.Services.Properties {
    using System;
    
    
    /// <summary>
    ///   一个强类型的资源类，用于查找本地化的字符串等。
    /// </summary>
    // 此类是由 StronglyTypedResourceBuilder
    // 类通过类似于 ResGen 或 Visual Studio 的工具自动生成的。
    // 若要添加或移除成员，请编辑 .ResX 文件，然后重新运行 ResGen
    // (以 /str 作为命令选项)，或重新生成 VS 项目。
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resource {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resource() {
        }
        
        /// <summary>
        ///   返回此类使用的缓存的 ResourceManager 实例。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Yahv.PsWms.DappForm.Services.Properties.Resource", typeof(Resource).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   重写当前线程的 CurrentUICulture 属性，对
        ///   使用此强类型资源类的所有资源查找执行重写。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   查找类似 export function methodName(data) {
        ///
        ///    FireEvent(&quot;methodName&quot;, data);
        ///
        ///    if (!!window[&apos;guid&apos;]) {
        ///        var ___1 = window[&apos;guid&apos;];
        ///        window[&apos;guid&apos;] = null;
        ///        return ___1;
        ///    }
        ///} 的本地化字符串。
        /// </summary>
        public static string jsGeckoFuncHelper {
            get {
                return ResourceManager.GetString("jsGeckoFuncHelper", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 /*回调windows事件：name 回调标识; data 数据（字符串或json字符串）*/
        ///function FireEvent(name, data) {
        ///
        ///    var event = new MessageEvent(name, { &apos;view&apos;: window, &apos;bubbles&apos;: false, &apos;cancelable&apos;: false, &apos;data&apos;:data ==null ?null : JSON.stringify(data) });
        ///    document.dispatchEvent(event);
        ///} 
        /// 的本地化字符串。
        /// </summary>
        public static string jsGeckoHelper {
            get {
                return ResourceManager.GetString("jsGeckoHelper", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 function FireEvent(name, data) {
        ///    var event = new MessageEvent(name, { &apos;view&apos;: window, &apos;bubbles&apos;: false, &apos;cancelable&apos;: false, &apos;data&apos;: data });
        ///    document.dispatchEvent(event);
        ///}
        ///
        ///window.onload = function () {
        ///    if (!!window[&apos;correct&apos;]) {
        ///        window[&apos;correct&apos;]();
        ///    }
        ///    setTimeout(function () {
        ///        FireEvent(&apos;Print&apos;, &apos;data&apos;);
        ///    }, 0);
        ///}; 的本地化字符串。
        /// </summary>
        public static string printerOnload {
            get {
                return ResourceManager.GetString("printerOnload", resourceCulture);
            }
        }
    }
}
