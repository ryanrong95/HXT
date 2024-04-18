<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LogTest.aspx.cs" Inherits="WebApp.Temp.LogTest" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>日志记录</title>
    <uc:EasyUI runat="server" />
    <link href="../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../Components/ccslog.js"></script>
    <script type="text/javascript">

        //数据初始化
        $(function () {
            var logs = eval('(<%=this.Model.Logs%>)');

            $('#logContainer').ccslog({
                data: logs
            });
        });
    </script>
</head>
<body>
<div style="margin-top: 5px; margin-left: 2px;">
    <div id="logContainer" title="日志记录" class="easyui-panel">
    </div>
</div>
</body>
</html>

