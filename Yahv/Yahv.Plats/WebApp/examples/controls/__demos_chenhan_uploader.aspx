<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="_demos_chenhan.aspx.cs" Inherits="WebApp.examples.controls._demos_chenhan" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>新空间测试地址-建议使用测试数据库连接串</title>

    <link href="http://fix.szhxd.net/frontframe/jquery-easyui-1.7.6/themes/gray/easyui.css" rel="stylesheet" />

    <link href="http://fix.szhxd.net/frontframe/jquery-easyui-1.7.6/themes/icon.css" rel="stylesheet" />
    <link href="http://fix.szhxd.net/frontframe/jquery-easyui-1.7.6/themes/icon-yg-cool.css" rel="stylesheet" />


    <link href="http://fix.szhxd.net/frontframe/standard-easyui/styles/plugin.css" rel="stylesheet" />
    <link href="http://fix.szhxd.net/frontframe/customs-easyui/Styles/main.css" rel="stylesheet" />
    <link href="http://fix.szhxd.net/frontframe/customs-easyui/Styles/reset.css" rel="stylesheet" />

    <script src="http://fix.szhxd.net/frontframe/jquery-easyui-1.7.6/jquery.min.js"></script>
    <script src="http://fix.szhxd.net/frontframe/jquery-easyui-1.7.6/jquery.easyui.min.js"></script>
    <script src="http://fix.szhxd.net/frontframe/jquery-easyui-1.7.6/locale/easyui-lang-zh_CN.js"></script>

    <script src="http://fix.szhxd.net/frontframe/customs-easyui/Scripts/main.js"></script>
    <script src="http://fix.szhxd.net/frontframe/ajaxPrexUrl.js"></script>

    <%--新写的控件Start，可以直接修改文件命名，先把原有的文件做.chenhan.bak.js后把_chenhan去除--%>
    <script src="http://fix.szhxd.net/frontframe/standard-easyui/scripts/easyui.jl.js"></script>
    <script src="http://fix.szhxd.net/frontframe/standard-easyui/scripts/fileUploader.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/frontframe/standard-easyui/scripts/timeouts.js"></script>
    <%--新写的控件End--%>

    <%-- 部署时候需要部署 standard-easyui/styles/plugin.css--%>


    <script>
        $(function () {
            $('#fileUploader').fileUploader({
                progressbarTarget: '#fileUploaderMessge',
                successTarget: '#fileUploaderSuccess',
                multiple: true,
                success: function (data) {
                    console.log(data);
                }
            });
        });
    </script>

</head>
<body>
    <form id="form1" runat="server">
        <div class="easyui-panel" data-options="title:'测试供应商控件'" style="width: 100%; padding: 3px 6px;">
            <div>
                上传：
                <a id="fileUploader">Excel导入</a>
                <div id="fileUploaderMessge" style="display: inline-block; width: 300px;"></div>
            </div>
            <div>
                上传后消息
                <div id="fileUploaderSuccess"></div>
            </div>

        </div>
        <div style="color:  yellow">
            <a href="#" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="$('#btnSubmit').click();return false;">提交</a>
            <%--Style="display: none;"--%>
            <asp:Button ID="btnSubmit" runat="server" Text="保存" OnClick="btnSubmit_Click" />
        </div>
    </form>
</body>
</html>
