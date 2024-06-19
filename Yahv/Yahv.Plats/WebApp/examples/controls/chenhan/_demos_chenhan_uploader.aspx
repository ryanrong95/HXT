<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="_demos_chenhan_uploader.aspx.cs" Inherits="WebApp.examples.controls._demos_chenhan_uploader" %>

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
    <%--<script src="http://fix.szhxd.net/frontframe/standard-easyui/scripts/fileUploader.js"></script>--%>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/frontframe/standard-easyui/scripts/timeouts.js"></script>

    <style>
        body { overflow: hidden; }
    </style>
    <%--新写的控件End--%>
    <script>


        //document

        $(function () {
            $('#fileUploader').fileUploader({
                type: 'RfqBom',
                required: true,
                accept: '.png,.jpeg,.jpg'.split(','),
                progressbarTarget: '#fileUploaderMessge',
                successTarget: '#fileUploaderSuccess',
                multiple: true,
                success: function (data) {
                    console.log(data);
                }
            });
        });
    </script>

    <%--tip测试--%>
    <script>
        $(function () {
            $('#dd').tooltip({
                position: 'right',
                content: '<span>This is the tooltip message.</span>',
                onShow: function () {
                    $(this).tooltip('tip').css({
                        //backgroundColor: '#666',
                        //borderColor: '#66'
                    });
                }
            });
        });
    </script>

    <%--tip测试--%>
    <script>
        //暂时使用  currency_chenhan
        $(function () {
            $('#price').currency({
                Prex: 'standartd',
                currency: "CNY",
                invoiceType: 1,
                value1: '',
                precision: 5,
                required: true,
                onChange: function (price1) {
                    console.log('测试price.onChange:' + price1);
                }
            });
            //.currency('setCurrency', 'CNY').currency('setInvoiceType', 2).currency('setCurrency', 'USD');
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
        <div class="easyui-panel" data-options="title:'tip测试'" style="width: 100%; padding: 3px 6px;">
            <%--
            <div>
                行内显示：   
                <partnumber>B39321B3741H110</partnumber>
            </div>
            --%>
            <%--
                <div>
                Tip显示：  
                <partnumber-tip>B39321B3741H110</partnumber-tip>
            </div>
            <div>
                Tip显示：前
                <partnumber-tip data-option="target:'before'">02011602102</partnumber-tip>
            </div>
            <div>
                Tip显示：后
                <partnumber-tip>02011602102</partnumber-tip>
            </div>

            <div>
                Tip显示：空
                <partnumber-tip> </partnumber-tip>
            </div>
            <div>
                <a id="price">币种</a>
            </div>--%>
        </div>
        <div>
            <a href="#" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="$('#btnSubmit').click();return false;">提交</a>
            <asp:Button ID="btnSubmit" runat="server" Text="保存" OnClick="btnSubmit_Click" Style="display: none;" />
        </div>
    </form>

</body>
</html>
