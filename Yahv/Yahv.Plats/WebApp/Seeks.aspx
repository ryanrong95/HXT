<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Seeks.aspx.cs" Inherits="WebApp.Seeks" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>资源页面</title>
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/jquery-easyui-1.7.6/themes/gray/easyui.css" rel="stylesheet" />
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/jquery-easyui-1.7.6/themes/icon.css" rel="stylesheet" />
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/jquery-easyui-1.7.6/themes/icon-yg-cool.css" rel="stylesheet" />

    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/jquery-easyui-1.7.6/jquery.min.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/jquery-easyui-1.7.6/jquery.easyui.min.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/jquery-easyui-1.7.6/locale/easyui-lang-zh_CN.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/jquery-easyui-extension/datagrid-dnd.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/extends.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/ajaxPrexUrl.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/easyui.jl.js"></script>

    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/customs-easyui/Scripts/easyui.myDatagrid.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/customs-easyui/Scripts/easyui.myDialog.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/customs-easyui/Scripts/easyui.myWindow.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/customs-easyui/Scripts/easyui.tabExtend.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/customs-easyui/Scripts/fullscreen.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/customs-easyui/Scripts/main.js"></script>
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/customs-easyui/fonts/iconfont.css" rel="stylesheet" />
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/customs-easyui/Styles/reset.css" rel="stylesheet" />
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/customs-easyui/Styles/main.css" rel="stylesheet" />

    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/iconfont/iconfont.css" rel="stylesheet" />
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/styles/plugin.css" rel="stylesheet" />
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/timeouts.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/jquery.cookie-1.4.1.min.js"></script>
</head>
<body>
    <%
        var arry = @"
//foricadmin/Temp/boot.aspx
Csrm
Erm
PvData
PvWsOrder
RFQ
".Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
    %>

    <%
        foreach (var url in arry.Where(item => !item.StartsWith("//")).
            Select(item => $"//{Request.Url.Host}/{item}/"))
        {
    %>
    <iframe src="<%=url %>"></iframe>
    <br />
    <%
        }
    %>
    <form id="form1" runat="server">
        <div>
        </div>
    </form>
</body>
</html>
