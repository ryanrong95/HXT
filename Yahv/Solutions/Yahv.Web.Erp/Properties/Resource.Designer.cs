﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Yahv.Web.Erp.Properties {
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
    internal class Resource {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resource() {
        }
        
        /// <summary>
        ///   返回此类使用的缓存的 ResourceManager 实例。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Yahv.Web.Erp.Properties.Resource", typeof(Resource).Assembly);
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
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   查找类似 &lt;!DOCTYPE html&gt;
        ///
        ///&lt;html lang=&quot;en&quot; xmlns=&quot;http://www.w3.org/1999/xhtml&quot;&gt;
        ///&lt;head&gt;
        ///    &lt;meta charset=&quot;utf-8&quot; /&gt;
        ///    &lt;title&gt;固定退出资源开发&lt;/title&gt;
        ///&lt;/head&gt;
        ///&lt;body&gt;
        ///    &lt;script&gt;
        ///        setTimeout(function () {
        ///            top.location.replace(&apos;/&apos;);
        ///        }, 0);
        ///    &lt;/script&gt;
        ///&lt;/body&gt;
        ///&lt;/html&gt; 的本地化字符串。
        /// </summary>
        internal static string Return {
            get {
                return ResourceManager.GetString("Return", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 &lt;!--&lt;script&gt;
        ///    if ($ &amp;&amp; $.fn.toText) {
        ///        $(function () {
        ///            //$(&apos;[easyui-input=&quot;true&quot;]&apos;).toText();
        ///            var arry = &apos;[value]&apos;.split(&apos;,&apos;);
        ///            for (var index = 0; index &lt; arry.length; index++) {
        ///                $(&apos;[&apos; + arry[index] + &apos;]&apos;).toText();
        ///            }
        ///
        ///        });
        ///    } else {
        ///        alert(&quot;请引用jquery与easyui.jl.js&quot;);
        ///    }
        ///&lt;/script&gt;--&gt;
        ///
        ///&lt;!--&lt;script&gt;
        ///
        ///    $(function () {
        ///        //$(&apos;[easyui-input=&quot;true&quot;]&apos;).toText();
        ///        var arry = &apos;[value]&apos;.spli [字符串的其余部分被截断]&quot;; 的本地化字符串。
        /// </summary>
        internal static string toText {
            get {
                return ResourceManager.GetString("toText", resourceCulture);
            }
        }
    }
}
