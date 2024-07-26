<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EasyUI.ascx.cs" Inherits="WebApp.UC.EasyUI" %>

<%--<%=Needs.Settings.SettingsManager<Needs.Settings.IErpSrcContext>.Current.Easyui%>--%>

<link href="<%=$"{System.Configuration.ConfigurationManager.AppSettings["fixed"]}"%>/frontframe/jquery-easyui-1.7.6/themes/gray/easyui.css" rel="stylesheet" />
<link href="<%=$"{System.Configuration.ConfigurationManager.AppSettings["fixed"]}"%>/frontframe/jquery-easyui-1.7.6/themes/super/css/superYellow.css" rel="stylesheet" />
<link href="<%=$"{System.Configuration.ConfigurationManager.AppSettings["fixed"]}"%>/frontframe/jquery-easyui-1.7.6/themes/super/easyui/themes/icons/css/font-awesome.min.css" rel="stylesheet" />
<script src="<%=$"{System.Configuration.ConfigurationManager.AppSettings["fixed"]}"%>/frontframe/jquery-easyui-1.7.6/themes/super/js/super.js"></script>

<link href="<%=$"{System.Configuration.ConfigurationManager.AppSettings["fixed"]}"%>/frontframe/jquery-easyui-1.7.6/themes/icon.css" rel="stylesheet" />
<link href="<%=$"{System.Configuration.ConfigurationManager.AppSettings["fixed"]}"%>/frontframe/jquery-easyui-1.7.6/themes/icon-yg-cool.css" rel="stylesheet" />

<script src="<%=$"{System.Configuration.ConfigurationManager.AppSettings["fixed"]}"%>/frontframe/jquery-easyui-1.7.6/jquery.min.js"></script>
<script src="<%=$"{System.Configuration.ConfigurationManager.AppSettings["fixed"]}"%>/frontframe/jquery-easyui-1.7.6/jquery.easyui.min.js"></script>
<script src="<%=$"{System.Configuration.ConfigurationManager.AppSettings["fixed"]}"%>/frontframe/jquery-easyui-1.7.6/locale/easyui-lang-zh_CN.js"></script>

<script src="<%=$"{System.Configuration.ConfigurationManager.AppSettings["fixed"]}"%>/frontframe/customs-easyui/Scripts/easyui.myDatagrid.js"></script>
<script src="<%=$"{System.Configuration.ConfigurationManager.AppSettings["fixed"]}"%>/frontframe/customs-easyui/Scripts/easyui.myDialog.js"></script>
<script src="<%=$"{System.Configuration.ConfigurationManager.AppSettings["fixed"]}"%>/frontframe/customs-easyui/Scripts/easyui.myWindow.js"></script>
<script src="<%=$"{System.Configuration.ConfigurationManager.AppSettings["fixed"]}"%>/frontframe/customs-easyui/Scripts/easyui.tabExtend.js"></script>
<script src="<%=$"{System.Configuration.ConfigurationManager.AppSettings["fixed"]}"%>/frontframe/customs-easyui/Scripts/main.js"></script>
<script src="<%=$"{System.Configuration.ConfigurationManager.AppSettings["fixed"]}"%>/frontframe/extends.js"></script>
<link href="<%=$"{System.Configuration.ConfigurationManager.AppSettings["fixed"]}"%>/frontframe/customs-easyui/fonts/iconfont.css" rel="stylesheet" />
<link href="<%=$"{System.Configuration.ConfigurationManager.AppSettings["fixed"]}"%>/frontframe/customs-easyui/Styles/reset.css" rel="stylesheet" />
<link href="<%=$"{System.Configuration.ConfigurationManager.AppSettings["fixed"]}"%>/frontframe/customs-easyui/Styles/main.css" rel="stylesheet" />


<script>
    /* 全局 */
    //window.gvSettings = {
    //    menu: '',
    //    fatherMenu: '',
    //    summary: ''
    //};

    function Valid() {
        var isValid = $("#form1").form("enableValidation").form("validate");
        if (!isValid) {
            //$.messager.alert('提示', '请按提示输入数据！');
            return false;
        }
        else {
            return true;
        }
    }

</script>
<%--<script src="/ga.js.aspx?rawurl=<%=Request.Url.AbsolutePath %>"></script>--%>

