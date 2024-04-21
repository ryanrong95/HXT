<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Tab.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm.Staffs.Tab" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/jquery-easyui-1.7.6/themes/gray/easyui.css" rel="stylesheet" />

    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/jquery-easyui-1.7.6/themes/icon.css" rel="stylesheet" />
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/jquery-easyui-1.7.6/themes/icon-yg-cool.css" rel="stylesheet" />

    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/jquery-easyui-1.7.6/jquery.min.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/jquery-easyui-1.7.6/jquery.easyui.min.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/jquery-easyui-1.7.6/locale/easyui-lang-zh_CN.js"></script>

    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/extends.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/customs-easyui/Scripts/easyui.myDatagrid.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/customs-easyui/Scripts/easyui.myDialog.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/customs-easyui/Scripts/easyui.myWindow.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/customs-easyui/Scripts/easyui.tabExtend.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/customs-easyui/Scripts/main.js"></script>
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/customs-easyui/fonts/iconfont.css" rel="stylesheet" />
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/customs-easyui/Styles/reset.css" rel="stylesheet" />
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/customs-easyui/Styles/main.css" rel="stylesheet" />
    <script type="text/javascript">
        var tempID=0;
        $(function () {            
            var id = getQueryString("ID");
            addTab("员工明细", "./StaffEdit.aspx?ID=" + id);
            addTab("劳资信息", "./LabourEdit.aspx?ID=" + id);
            addTab("银行卡信息", "./BankcardEdit.aspx?ID=" + id);
            addTab("薪资历史记录", "./PastsHistory.aspx?ID=" + id);
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
<body>
    <div id="tt" class="easyui-tabs" data-options="fit:true">
    </div>
</body>
</html>
