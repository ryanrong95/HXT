using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Settings.Models
{
    class ErpSrcContext : ISettings, IErpSrcContext
    {
        [Label("Easyui needs an external chain file")]
        public string Easyui { get; set; }

        [Label("Easyui1.6.2 needs an external chain file")]
        public string Easyui162 { get; set; }

        /// <summary>
        /// 这个默认数据不要再修改了，这个按照我们现有的进行
        /// </summary>
        public ErpSrcContext()
        {
            this.Easyui = @"
<link href=""http://fixed2.szhxt.net/My/Content/main.css"" rel=""stylesheet"" />
<link href=""http://fixed2.szhxt.net/Content/themes/icon.css"" rel=""stylesheet"" />
<link href=""http://fixed2.szhxt.net/Content/themes/default/easyui.css"" rel=""stylesheet"" />
<link href=""http://fixed2.szhxt.net/Yahv/jquery-easyui-1.7.6/themes/icon-yg-cool.css""rel=""stylesheet"" />
<script src=""http://fixed2.szhxt.net/Scripts/jquery-1.11.3.min.js""></script>
<script src=""http://fixed2.szhxt.net/My/Scripts/extends.js""></script>
<script src=""http://fixed2.szhxt.net/Scripts/jquery.easyui-1.4.5.min.js""></script>
<script src=""http://fixed2.szhxt.net/Scripts/locale/easyui-lang-zh_CN.js""></script>
<script src=""http://fixed2.szhxt.net/My/Scripts/datagrid.js""></script>
<script src=""http://fixed2.szhxt.net/My/Scripts/easyui.myWindow.js""></script>
<script src=""http://fixed2.szhxt.net/Yahv/customs-easyui/Scripts/easyui.myDialog.js""></script>
            ";

            this.Easyui162 = @"
<link href=""http://fixed2.szhxt.net/My/Content/main.css"" rel=""stylesheet"" />
<link href=""http://fixed2.szhxt.net/Official/jquery-easyui-1.6.2/themes/icon.css"" rel=""stylesheet"" />
<link href=""http://fixed2.szhxt.net/Official/jquery-easyui-1.6.2/themes/default/easyui.css"" rel=""stylesheet"" />
<link href=""http://fixed2.szhxt.net/Yahv/jquery-easyui-1.7.6/themes/icon-yg-cool.css""rel=""stylesheet"" /><script src=""http://fixed2.szhxt.net/Official/jquery-easyui-1.6.2/jquery.min.js""></script>
<script src=""http://fixed2.szhxt.net/Official/jquery-easyui-1.6.2/jquery.easyui.min.js""></script>
<script src=""http://fixed2.szhxt.net/Official/jquery-easyui-1.6.2/locale/easyui-lang-zh_CN.js""></script>
<script src=""http://fixed2.szhxt.net/My/Scripts/extends.js""></script>
<script src=""http://fixed2.szhxt.net/My/Scripts/datagrid.js""></script>
<script src=""http://fixed2.szhxt.net/My/Scripts/easyui.myWindow.js""></script>
<script src=""http://fixed2.szhxt.net/Yahv/customs-easyui/Scripts/easyui.myDialog.js""></script>
";

        }
    }
}
