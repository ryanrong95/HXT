<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="_demos_liufang_agents.aspx.cs" Inherits="WebApp.examples.controls._demos_liufang_agents" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>新空间测试地址-建议使用测试数据库连接串</title>

    <link href="http://fixed2.b1b.com/Yahv/jquery-easyui-1.7.6/themes/gray/easyui.css" rel="stylesheet" />

    <link href="http://fixed2.b1b.com/Yahv/jquery-easyui-1.7.6/themes/icon.css" rel="stylesheet" />
    <link href="http://fixed2.b1b.com/Yahv/jquery-easyui-1.7.6/themes/icon-yg-cool.css" rel="stylesheet" />


    <link href="http://fixed2.b1b.com/Yahv/standard-easyui/styles/plugin.css" rel="stylesheet" />
    <link href="http://fixed2.b1b.com/Yahv/customs-easyui/Styles/main.css" rel="stylesheet" />
    <link href="http://fixed2.b1b.com/Yahv/customs-easyui/Styles/reset.css" rel="stylesheet" />

    <script src="http://fixed2.b1b.com/Yahv/jquery-easyui-1.7.6/jquery.min.js"></script>
    <script src="http://fixed2.b1b.com/Yahv/jquery-easyui-1.7.6/jquery.easyui.min.js"></script>
    <script src="http://fixed2.b1b.com/Yahv/jquery-easyui-1.7.6/locale/easyui-lang-zh_CN.js"></script>

    <script src="http://fixed2.b1b.com/Yahv/customs-easyui/Scripts/main.js"></script>
    <script src="http://fixed2.b1b.com/Yahv/ajaxPrexUrl.js"></script>

    <%--新写的控件Start，可以直接修改文件命名，先把原有的文件做.chenhan.bak.js后把_chenhan去除--%>
    <script src="http://fixed2.b1b.com/Yahv/standard-easyui/scripts/easyui.jl.js"></script>
    <%--<script src="http://fixed2.b1b.com/Yahv/standard-easyui/scripts/fileUploader.js"></script>--%>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/timeouts.js"></script>
    <%--新写的控件End--%>
    <script>

        document.referrer = 'dydyu';
        document.referrer = 'asdfasdfasdf';
        
        alert(document.referrer);

        $(function () {
            $('#fileUploader').fileUploader({
                type: 'RfqBom',
                required: true,
                accept: 'image/x-png,image/gif,image/jpeg,image/bmp'.split(','),
                progressbarTarget: '#fileUploaderMessge',
                successTarget: '#fileUploaderSuccess',
                multiple: true,
                success: function (data) {
                    console.log(data);
                }
            });
        });
    </script>



    <%--测试--%>
    <script>
        //暂时使用  currency_chenhan
        $(function () {
            $('#agentBrands').agentBrand({
                onChange: function (newValue) {
                    var id = newValue;
                    $('#agentMaps1').agentMaps({ type: 'Company', brandID: id });
                    $('#agentMaps2').agentMaps({ type: 'Supplier', brandID: id });
                }
            });
        });
    </script>

</head>
<body>
    <form id="form1" runat="server">
        <div class="easyui-panel" data-options="title:'测试代理线控件'" style="width: 100%; padding: 3px 6px;">
            <div>
                代理线品牌：
                <input id="agentBrands" name="agentBrands" />
            </div>
            <div>
                代理线供应商：
                <input id="agentMaps1" name="agentMaps1" />
            </div>
            <div>
                代理线代理（内部）公司：
                <input id="agentMaps2" name="agentMaps2" />
            </div>
        </div>
        <div>
            <a href="#" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="$('#btnSubmit').click();return false;">提交</a>
            <asp:Button ID="btnSubmit" runat="server" Text="保存" OnClick="btnSubmit_Click" Style="display: none;" />
        </div>
    </form>

</body>
</html>
