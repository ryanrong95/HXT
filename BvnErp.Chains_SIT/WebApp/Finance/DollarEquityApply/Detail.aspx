<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="WebApp.Finance.DollarEquityApply.Detail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script>
        var DollarEquityApply = eval('(<%=this.Model.DollarEquityApply%>)');

        $(function () {
            $('#ApplyID').textbox('setValue', DollarEquityApply.ApplyID);
            $('#Amount').textbox('setValue', DollarEquityApply.Amount);
            $('#Currency').html(DollarEquityApply.Currency);
            $('#SupplierChnName').textbox('setValue', DollarEquityApply.SupplierChnName);
            $('#SupplierEngName').textbox('setValue', DollarEquityApply.SupplierEngName);
            $('#BankName').textbox('setValue', DollarEquityApply.BankName);
            $('#BankAccount').textbox('setValue', DollarEquityApply.BankAccount);
            $('#SwiftCode').textbox('setValue', DollarEquityApply.SwiftCode);
            $('#BankAddress').textbox('setValue', DollarEquityApply.BankAddress);




        });

        //关闭窗口
        function Close() {
            $.myWindow.close();
        }
    </script>
</head>
<body class="easyui-layout">
    <div style="font-size: 14px; margin-left: 30px; margin-top: 25px;">
        <div>
            <label>申请ID：</label>
            <input class="easyui-textbox" id="ApplyID" data-options="validType:'length[1,5000]',editable:false," style="width: 220px;" />
            <label style="margin-left: 50px;">金额：</label>
            <input class="easyui-textbox" id="Amount" data-options="validType:'length[1,5000]',editable:false," style="width: 220px;" />
            <label id="Currency">币制</label>
        </div>
        <div style="margin-top: 10px;">
            <label>供应商中文名称：</label>
            <input class="easyui-textbox" id="SupplierChnName" data-options="validType:'length[1,5000]',editable:false," style="width: 485px;" />
        </div>
        <div style="margin-top: 10px;">
            <label>供应商英文名称：</label>
            <input class="easyui-textbox" id="SupplierEngName" data-options="validType:'length[1,5000]',editable:false," style="width: 485px;" />
        </div>
        <div style="margin-top: 10px;">
            <label>银行名称：</label>
            <input class="easyui-textbox" id="BankName" data-options="validType:'length[1,5000]',editable:false," style="width: 527px;" />
        </div>
        <div style="margin-top: 10px;">
            <label>银行账号：</label>
            <input class="easyui-textbox" id="BankAccount" data-options="validType:'length[1,5000]',editable:false," style="width: 220px;" />
            <label style="margin-left: 10px;">银行代码：</label>
            <input class="easyui-textbox" id="SwiftCode" data-options="validType:'length[1,5000]',editable:false," style="width: 220px;" />
        </div>
        <div style="margin-top: 10px;">
            <label>银行地址：</label>
            <input class="easyui-textbox" id="BankAddress" data-options="validType:'length[1,5000]',editable:false," style="width: 527px;" />
        </div>
        <div style="margin-top: 10px; text-align: right;">
            <a href="javascript:void(0);" class="easyui-linkbutton" data-options="" style="width: 58px; margin-right: 75px;" onclick="Close()">关闭</a>
        </div>
    </div>
</body>
</html>
