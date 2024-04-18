<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Client.Supplier.Bank.Edit" %>

<!DOCTYPE html>


<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../../Scripts/Ccs.js"></script>
    <script type="text/javascript">
        var ClientSupplierID = '<%=this.Model.ClientSupplierID%>';//供应商ID
        var AccountID = '<%=this.Model.AccountID%>';//供应商账户ID
        var Supplier = eval('(<%=this.Model.Supplier%>)');//当前供应商公司
        var places = eval('(<%=this.Model.Places%>)');
        var banknames = eval('(<%=this.Model.BankNames%>)');//供应商所有银行账户名称
        var bankinfos = eval('(<%=this.Model.BankInfos%>)');//供应商所有银行账户信息
        var currencys = eval('(<%=this.Model.Currency%>)');
        var methords = eval('<%=this.Model.Methord%>');
        if (AccountID != '') {
            SupplierBankData = eval('(<%=this.Model.SupplierBankData != null ? this.Model.SupplierBankData:""%>)');
        }

        ///动态加载必填项
        //function showpap() {
        //    var panelStyle = Supplier.Place;
        //    if ("HKG" != panelStyle) {
        //        $("#SwiftCode").textbox('textbox').validatebox('options').required = true;
        //        $("#BankAddress").textbox('textbox').validatebox('options').required = true;

        //    } else {
        //        $("#BankAddress").textbox('textbox').validatebox('options').required = false;
        //        $("#SwiftCode").textbox('textbox').validatebox('options').required = false;
        //    }
        //    $('#form1').form('enableValidation').form('validate');
        //};

        $(function () {
            $("#Place").combobox({
                data: places,
                onLoadSuccess: function () {
                    $("#Place").combobox("setValue", "HKG");
                },
               onChange: function (n) {
                    //var required = (n != 'HKG');
                    //var options = {};
                    //options['required'] = required;
                    //$('#BankAddress').textbox(options);
                    //$('#SwiftCode').textbox(options);
                }
            });


            $("#BankNameCB").combobox({
                data: banknames,
                onLoadSuccess: function () {
                    
                },
                onSelect: function (record) {
                    $.each(bankinfos, function (i, val) {
                        if (val.BankName == record.Code) {
                            $("#BankAccount").textbox("setValue", val.BankAccount);
                            $("#BankName").textbox("setValue", val.BankName != null ? val.BankName.replace(new RegExp("&amp;", "g"), "&") : val.BankName);
                            $("#BankAddress").textbox("setValue", val.BankAddress != null ? val.BankAddress.replace(new RegExp("#39;", "g"), "'") : val.BankAddress);
                            $("#SwiftCode").textbox("setValue", val.SwiftCode);
                            $("#Summary").textbox("setValue", val.Summary);
                            $("#Place").combobox("setValue", val.Place);
                            $("#Currency").combobox("setValue", val.Currency);
                            $("#Methord").combobox("setValue", val.Methord);

                            return false;
                        }
                    }); 
                }
            });

            //
            $("#BankNameCB").next().css('background', 'whitesmoke');
            $("#BankNameCB").next().children().each(function (i, n) {
                $(n).css('background', 'whitesmoke');
            });

            $("#Currency").combobox({
                data: currencys
            });
            $("#Methord").combobox({
                data: methords,
                onLoadSuccess: function () {
                    $("#Methord").combobox("setValue", "3")
                }
            });
            if (AccountID != '') {
                $("#BankAccount").textbox("setValue", SupplierBankData.BankAccount);
                $("#BankName").textbox("setValue", SupplierBankData.BankName != null? SupplierBankData.BankName.replace(new RegExp("&amp;", "g"), "&"): SupplierBankData.BankName);
                $("#BankAddress").textbox("setValue", SupplierBankData.BankAddress != null? SupplierBankData.BankAddress.replace(new RegExp("#39;", "g"), "'"):SupplierBankData.BankAddress);
                $("#SwiftCode").textbox("setValue", SupplierBankData.SwiftCode);
                $("#Summary").textbox("setValue", SupplierBankData.Summary);
                $("#Place").combobox("setValue", SupplierBankData.Place);
                $("#Currency").combobox("setValue", SupplierBankData.Currency);
                $("#Methord").combobox("setValue", SupplierBankData.Methord);
            }
            //  showpap();

            $('#btnSave').on('click', function () {
                if (!$("#form1").form('validate')) {
                    return;
                }

                var values = FormValues("form1");//可继续添加其它参数:values[id] = $this.val();
                values['ClientSupplierID'] = ClientSupplierID;//会员供应商ID
                values['AccountID'] = AccountID;//供应商账户ID
                //提交后台
                $.post('?action=SaveClientSupplierAccount', { Model: JSON.stringify(values) }, function (res) {
                    var result = JSON.parse(res);
                    $.messager.alert('消息', result.message, 'info', function () {
                        if (result.success) {
                            closeWin();
                        }
                    });
                });
            });

            $('#btnReturn').on('click', function () {
                closeWin();
            });

        });

        function closeWin() {
            $.myWindow.close();
        }


    </script>
</head>
<body class="easyui-layout">
    <div id="content">
        <form id="form1" runat="server">
            <table id="editTable" style="width: 100%;">

                <tr>
                    <td class="lbl">供应商已有账户:</td>
                    <td>
                        <input class="easyui-combobox" style="width: 474px;height:30px" data-options="valueField:'Code',textField:'Name',limitToList:true,editable:false,tipPosition:'bottom'" id="BankNameCB" name="BankNameCB" />
                    </td>

                </tr>

                <tr></tr>

                 <tr>
                    <td class="lbl">国家/地区:</td>
                    <td>
                        <input class="easyui-combobox" style="width: 474px" data-options="valueField:'Code',textField:'Name',limitToList:true,required:true,tipPosition:'bottom'" id="Place" name="Place" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">银行名称(英文)：</td>
                    <td>
                        <input class="easyui-textbox" id="BankName"
                            data-options="required:true,tipPosition:'right',missingMessage:'请输入银行名称(英文)',tipPosition:'bottom',required:true," style="width: 474px;" />
<%--                        <span id ="Querybanks" style="color:blue;text-decoration: underline;cursor: pointer;">查询</span>--%>
                    </td>
                </tr>
                 <tr>
                    <td class="lbl">银行账号：</td>
                    <td>
                        <input class="easyui-textbox" id="BankAccount"
                            data-options="required:true,validType:'bankaccount',tipPosition:'right',missingMessage:'请输入银行账号',tipPosition:'bottom'" style="width: 80%" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">银行地址(英文)：</td>
                    <td>
                        <%-- <input class="easyui-textbox" id="BankAddress"
                            data-options="required:true,validType:'supplierAccountAddr',tipPosition:'right',missingMessage:'请输入银行地址英文)',tipPosition:'bottom'" style="width: 80%" />--%>
                        <input class="easyui-textbox" id="BankAddress"
                            data-options="tipPosition:'right',missingMessage:'请输入银行地址英文)',tipPosition:'bottom',required:true," style="width: 80%" />
                    </td>
                </tr>
               
                <tr>
                    <td class="lbl">银行代码：</td>
                    <td>
                        <input class="easyui-textbox  easyui-validatebox" id="SwiftCode"
                            data-options="required:true,validType:'length[1,50]',tipPosition:'right',missingMessage:'请输入银行代码',tipPosition:'bottom'" style="width: 80%" />
                    </td>
                </tr>
               
                <tr>
                    <td class="lbl">支付方式:</td>
                    <td>
                        <input class="easyui-combobox" data-options="valueField:'Key',textField:'Value',limitToList:true,required:true,tipPosition:'bottom'" id="Methord" name="Methord" style="width: 474px" />
                    </td>
                </tr>
                <%--<tr>
                    <td class="lbl"> 币种:</td>
                    <td> <input class="easyui-combobox"  data-options="valueField:'Key',textField:'Value',limitToList:true,required:true,tipPosition:'bottom'" id="Currency" name="Currency"  style="width: 474px"/> </td>
                </tr>--%>
                <tr>
                    <td class="lbl">摘要备注：</td>
                    <td>
                        <input class="easyui-textbox" id="Summary"
                            data-options="validType:'length[1,400]',tipPosition:'bottom',multiline:true"
                            style="width: 80%; height: 60px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl"></td>
                    <td>请仔细填写帐户信息，因委托方银行信息填写错误造成损失的，由委托方自行承担责任
                    </td>
                </tr>
            </table>
        </form>
    </div>
    <div id="dlg-buttons" data-options="region:'south',border:false">
        <a id="btnSave" class="easyui-linkbutton" data-options="iconCls:'icon-save'">保存</a>
        <a id="btnReturn" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Close()">取消</a>
    </div>
</body>
</html>

