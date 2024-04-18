<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Tabs.aspx.cs" Inherits="WebApp.Crm.Project_bak.Tabs" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script>
        $(function () {
            var ProjectID = getQueryString("ProjectID");
            addTab("机会详情", "./ShowList.aspx?ID=" + ProjectID);
            addTab("机会跟踪记录", "./ReportShow.aspx?ProjectID=" + ProjectID);

            $("#tt").tabs("select", 0);
        });

        function addTab(title, url) {
            if ($('#tt').tabs('exists', title)) {
                $('#tt').tabs('select', title);
            } else {
                var content = '<iframe scrolling="auto" frameborder="0"  src="' + url + '" style="width:100%;height:99%;"></iframe>';
                $('#tt').tabs('add', {
                    title: title,
                    content: content,
                });
            }
        }
    </script>
</head>
<body class="e">
    <div id="tt" class="easyui-tabs" <%--style="width: 100%; height: 900px;"--%> data-options="fit:true">
    </div>
</body>
</html>
