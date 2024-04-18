<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Crm.Beneficiaries.Edit" ValidateRequest="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script type="text/javascript">
        var Company = eval('(<%=this.Model.CompanyData%>)');
        var AllData = eval('(<%=this.Model.AllData%>)');
        //页面加载时
        $(function () {
            //下拉框加载数据
            $("#CompanyID").combobox({
                data: Company
            });

            //初始化赋值
            if (AllData != null) {
                $("#Bank").textbox("setValue", AllData["Bank"]);
                $("#BankCode").textbox("setValue", AllData["BankCode"]);
                $("#Address").textbox("setValue", AllData["Address"]);
                $("#CompanyID").combobox("setValue", AllData["CompanyID"]);
            }

            $("#BankCode").textbox('textbox').bind('keyup', function (e) {
                $("#BankCode").textbox('setValue', $(this).val().replace(/\D/g, ''));
            });

            //校验输入框内容
            $("#CompanyID").combobox("textbox").bind("blur", function () {
                var value = $("#CompanyID").combobox("getValue");
                var data = $("#CompanyID").combobox("getData");
                var valuefiled = $("#CompanyID").combobox("options").valueField;
                var index = $.easyui.indexOfArray(data, valuefiled, value);
                if (index < 0) {
                    $("#CompanyID").combobox("clear");
                }
            });
        });

        function Close() {
            $.myWindow.close();
        }
    </script>
    <style type="text/css">
        .auto-style1 {
            width: 320px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server" method="post">
        <table id="table1" style="margin: 0 auto; height: 50px">
            <tr>
                <td class="lbl" style="text-align: center; width: 80px" colspan="1">银行名称</td>
                <td colspan="1">
                    <input class="easyui-textbox" id="Bank" name="Bank"
                        data-options="required:true,validType:'length[1,150]'" style="width: 150px" />
                </td>
                <td class="lbl" style="text-align: center; width: 80px">账户</td>
                <td>
                    <input class="easyui-textbox" id="BankCode" name="BankCode"
                        data-options="required:true,min:0,validType:'length[15,19]'" style="width: 200px" />
                </td>
                <td class="lbl" style="text-align: center; width: 80px">公司</td>
                <td>
                    <input class="easyui-combobox" id="CompanyID"  name="CompanyID" style="width: 150px" 
                        data-options="valueField:'ID',textField:'Name',required:true, panelMaxHeight:'100px',"/>
                </td>
            </tr>
            <tr>
                <td class="lbl" colspan="1" style="text-align: center; width: 80px">地址</td>
                <td colspan="5">
                    <input class="easyui-textbox" id="Address" name="Address"
                        data-options="required:true,validType:'length[1,350]'" style="width: 500px" />
                </td>
            </tr>
        </table>
        <div id="divSave" style="text-align: center; margin-top: 30px">
            <asp:Button ID="btnSumit" Text="保存" runat="server" OnClientClick="return Valid();" OnClick="btnSave_Click" />
            <asp:Button ID="btnClose" Text="取消" runat="server" OnClientClick="Close()" />
        </div>
    </form>
</body>
</html>
