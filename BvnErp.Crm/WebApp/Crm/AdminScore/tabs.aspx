<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="tabs.aspx.cs" Inherits="WebApp.Crm.AdminScore.tabs" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <!--全局变量配置-->
    <script>
        /* 每个需要颗粒化的页面都需要指定 menu ，否则不会写入菜单和颗粒化*/
        gvSettings.fatherMenu = 'CRM系统管理';
        gvSettings.menu = '考核明细';
        gvSettings.summary = '';
    </script>
    <script>
        $(function () {
            addTab("跟踪记录明细", "./ReportsDetail.aspx");
            addTab("新客户数明细", "./ClientsDetail.aspx");
            addTab("DI数量明细", "./DIDetail.aspx");
            addTab("DW数量明细", "./DWDetail.aspx");

            $("#tt").tabs("select", 0);
        });

        //添加Tab页
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
    <input type="hidden" id="hidIsClient" runat="server" />
    <div id="tt" class="easyui-tabs" data-options="fit:true">
    </div>
</body>
</html>
