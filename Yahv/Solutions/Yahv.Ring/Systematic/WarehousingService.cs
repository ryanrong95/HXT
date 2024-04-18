using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
//using WinApp.Printers;
using Yahv.Underly;
using Yahv.Underly.Rings;

namespace Yahv.Systematic
{
    /// <summary>
    /// 仓储服务
    /// </summary>
    public class WarehousingService
    {
        IRingAdmin admin;


        WarehousingService()
        {

            this.LabelHelper = new Systematic.LabelPrinter();
        }


        /// <summary>
        /// 默认构造器
        /// </summary>
        /// <param name="admin">Admin</param>
        public WarehousingService(IRingAdmin admin) : this()
        {
            this.admin = admin;
            this._Handle = new CurrentHandle();
        }

        CurrentHandle _Handle;
        /// <summary>
        /// 当前操作
        /// </summary>
        public CurrentHandle Handle
        {
            get
            {
                return _Handle;
            }
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="fileName">本地文件地址</param>
        public void UploadFile(string fileName)
        {

            #region 服务端使用如下代码进行接收、保存并根据其他参数写入数据库

            /*
<%@ Import Namespace="System"%>
<%@ Import Namespace="System.IO"%>
<%@ Import Namespace="System.Net"%>
<%@ Import NameSpace="System.Web"%>

<Script language="C#" runat=server>
void Page_Load(object sender, EventArgs e) {
    
foreach(string f in Request.Files.AllKeys) {
HttpPostedFile file = Request.Files[f];
file.SaveAs("c:\\inetpub\\test\\UploadedFiles\\" + file.FileName);
}    
}

</Script>
<html>
<body>
<p> Upload complete.  </p>
</body>
</html>
             */

            #endregion


            var admin = this.admin;
            var currnet = this._Handle;

            string url = "" + "?用地址参数方式增加admin、current信息";

            using (WebClient clinet = new WebClient())
            {
                clinet.UploadFile(url, "POST", fileName);

                var strams = clinet.OpenWrite("");

                //strams.Write();
            }
        }

        /// <summary>
        /// 现在文件，并放到指定的位置准备打印
        /// </summary>
        /// <param name="url"></param>
        /// <returns>保存地址</returns>
        public string DownFile(string url)
        {
            return null;//返回打印的地址
        }

        /// <summary>
        /// 打印文件（可能包含标签文件）
        /// </summary>
        /// <param name="info">文件信息</param>
        /// <remarks>乔霞点击打印按钮后触发</remarks>
        public void PrintFile(Models.FileDescriptor info)
        {
            string fileName = this.DownFile(info.Url);

            //如果需要的话，判断 Url 的后缀。例如：docx、doc、xls、xlsx、txt使用某打印机进行打印。
            //目前打印只需要打印A4纸张，并且所有下载打印的非标签文件都应该使用A4打印。

            //选择配置好打印（高汇航）模块进行打印



            //如果是下载的客户自定义标签打印（这个需求还需要与朝旺联系看看如何限制？）




        }

        /// <summary>
        /// 标签打印帮助者
        /// </summary>
        public LabelPrinter LabelHelper { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public TemplatePrinter Template { get; set; }
    }

    /// <summary>
    /// 当前操作
    /// </summary>
    public class CurrentHandle
    {
        string mainID;
        string itemID;

        /// <summary>
        /// 主对象ID
        /// </summary>
        public string MainID
        {
            get
            {
                return mainID;
            }

            set
            {
                mainID = value;
            }
        }

        /// <summary>
        /// 自对象ID
        /// </summary>
        public string ItemID
        {
            get
            {
                return itemID;
            }

            set
            {
                itemID = value;
            }
        }


    }


    /// <summary>
    /// 模版打印
    /// </summary>
    /// <remarks>
    /// 帮助乔霞选择好模版、实际要打印的数据
    /// 开发上如果来不及，接受所有模版使用简单模方式开发。不要做编译，可以直接修改并起效的那种开发。
    /// </remarks>
    public class TemplatePrinter
    {
        //我们送货上门的指定文件
        //装箱单
        //快递鸟的是否需要另外规划一个，待高汇航交接过来后决定。
    }

    /// <summary>
    /// 标签打印
    /// </summary>
    /// <remarks>
    /// 帮助乔霞选择好模版、尺寸、实际要打印的数据
    /// 开发上如果来不及，接受所有模版使用简单模方式开发。不要做编译，可以直接修改并起效的那种开发。
    /// </remarks>
    public class LabelPrinter
    {
        public LabelPrinter()
        {
            //this.Product = new ProductLabel();
        }

        ///// <summary>
        ///// 库位标签
        ///// </summary>
        //public PositionLabel Position
        //{
        //    get;
        //    // {
        //    //    //YUI.Currnet
        //    //    return null;
        //    //}
        //    set;
        //}
        /// <summary>
        /// 产品标签
        /// </summary>
        //public ProductLabel Product { get; set; }

        ///// <summary>
        ///// 箱号标签
        ///// </summary>
        //public object BoxIndex { get; set; }
    }

}
