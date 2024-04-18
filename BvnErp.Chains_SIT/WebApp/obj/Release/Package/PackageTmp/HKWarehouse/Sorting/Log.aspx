<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Log.aspx.cs" Inherits="WebApp.HKWarehouse.Sorting.Log" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script src="../../Scripts/ccs.log-1.0.js"></script>
    <script type="text/javascript">
        $(function () {
            var logs = eval('(<%=this.Model.Logs%>)');
            debugger
            for (var i = 0; i < logs.length; i++){
                logs[i].Summary = logs[i].Summary.replace("%22", "\"").replace("%27","\'");
            }

            $('#log').ccslog({
                data: logs,
            });
        });
    </script>
</head>
<body>
    <div style="margin: 5px; min-height: 50px">
        <p id="log" style="font-size:12px"></p>
    </div>
</body>
</html>
