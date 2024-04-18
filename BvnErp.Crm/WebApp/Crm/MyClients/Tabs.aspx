<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Tabs.aspx.cs" Inherits="WebApp.Crm.MyClients.Tabs" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script>
        $(function () {
            var clientID = getQueryString("ClientID");
            addTab("客户信息", "./Show.aspx?ID=" + clientID);

            if ($("#hidIsClient").val() == "True") {
                addTab("客户联系人", "../Contacts/List.aspx?ClientID=" + clientID);
            }
            addTab("客户费用", "../Charges/List.aspx?ClientID=" + clientID);
            addTab("跟踪记录", "../Trace/list.aspx?ClientID=" + clientID);
            addTab("开票信息", "../Invoice/List.aspx?ClientID=" + clientID);
            addTab("地址簿", "../Consignees/List.aspx?ClientID=" + clientID);

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
